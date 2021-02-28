namespace ISADotNet.XLSX.AssayFile

open FSharpSpreadsheetML

open ISADotNet

open FSharpSpreadsheetML
open FSharpSpreadsheetML.Table
open DocumentFormat.OpenXml.Spreadsheet

//module private Table = 
//    /// Reads a complete table. Values are stored sparsely in a dictionary, with the key being a column header and row index tuple
//    let toSparseValueMatrix (sst:SharedStringTable Option) sheetData (table:Table) =
//        let area = getArea table
//        let dictionary = System.Collections.Generic.Dictionary<string*int,string>()
//        [Area.leftBoundary area .. Area.rightBoundary area]
//        |> List.iter (fun c ->
//            let upperBoundary = Area.upperBoundary area
//            let lowerBoundary = Area.lowerBoundary area
//            let header = SheetData.tryGetCellValueAt sst upperBoundary c sheetData |> Option.get
//            List.init (lowerBoundary - upperBoundary |> int) (fun i ->
//                let r = uint i + upperBoundary + 1u
//                match SheetData.tryGetCellValueAt sst r c sheetData with
//                | Some v -> dictionary.Add((header,i),v)
//                | None -> ()                              
//            )
//            |> ignore
//        )
//        dictionary

module AssayFile =

    /// Create a new ISADotNet.XLSX assay file constisting of two sheets. The first has the name of the assayIdentifier and is meant to store parameters used in the assay. The second stores additional assay metadata
    let init metadataSheetName assayIdentifier path =
        Spreadsheet.initWithSST assayIdentifier path
        |> MetaData.init metadataSheetName 
        |> Spreadsheet.close

    let fromFile (path:string) = 

        let doc = Spreadsheet.fromFile path false

        let sst = Spreadsheet.tryGetSharedStringTable doc

        try

            let assayMetaData,contacts = 
                Spreadsheet.tryGetSheetBySheetName "Investigation" doc
                |> Option.map (fun sheet -> 
                    sheet
                    |> SheetData.getRows
                    |> Seq.map (Row.mapCells (Cell.includeSharedStringValue sst.Value))
                    |> MetaData.fromRows
                )
                |> Option.defaultValue (None,[])
                

            let sheetNames = 
                Spreadsheet.getWorkbookPart doc
                |> Workbook.get
                |> Sheet.Sheets.get
                |> Sheet.Sheets.getSheets
                |> Seq.map Sheet.getName
        
            let namedProtocols = []

            let characteristics,factors,protocols,processes =
                sheetNames
                |> Seq.collect (fun sheetName ->
                    match Spreadsheet.tryGetWorksheetPartBySheetName sheetName doc with
                    | Some wsp ->
                        match Table.tryGetByNameBy (fun s -> s.Contains "annotationTable") wsp with
                        | Some table -> 
                            let sheet = Worksheet.getSheetData wsp.Worksheet
                            let headers = Table.getColumnHeaders table
                            let m = Table.toSparseValueMatrix sst sheet table
                            AnnotationTable.splitBySamples headers
                            |> Seq.collect (AnnotationTable.splitByNamedProtocols namedProtocols)
                            |> AnnotationTable.indexProtocolsBySheetName sheetName
                            |> Seq.map (fun (protocolMetaData,headers) ->
                                let characteristic,factors,protocol,processGetter = 
                                    AnnotationNode.splitIntoNodes headers
                                    |> AnnotationTable.getProcessGetter protocolMetaData
                                characteristic,factors,protocol,
                                Table.getArea table
                                |> fun area -> Table.Area.lowerBoundary area - Table.Area.upperBoundary area |> int
                                |> fun length -> 
                                    Seq.init length (processGetter m)
                                    |> AnnotationTable.mergeIdenticalProcesses
                                    |> AnnotationTable.indexRelatedProcessesByProtocolName
                    
                            )
                        | None -> Seq.empty
                    | None -> Seq.empty                
                )
                |> Seq.fold (fun (characteristics',factors',protocols',processes') (characteristics,factors,protocol,processes) ->
                    List.append characteristics' characteristics |> List.distinct,
                    List.append factors' factors |> List.distinct,
                    Seq.append protocols' (Seq.singleton protocol),
                    Seq.append processes' processes
                ) (List.empty,List.empty,Seq.empty,Seq.empty)

            let processes = AnnotationTable.updateSamplesByReference processes processes
            
            let assay = assayMetaData |> Option.defaultValue Assay.empty
            
            factors,
            protocols |> Seq.toList,
            contacts,
            {assay with 
                    ProcessSequence = API.Option.fromValueWithDefault [] (processes |> Seq.toList)
                    CharacteristicCategories = API.Option.fromValueWithDefault [] characteristics
            }

        finally 
        Spreadsheet.close doc