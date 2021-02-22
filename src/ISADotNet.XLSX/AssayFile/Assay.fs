namespace ISADotNet.XLSX.AssayFile

open FSharpSpreadsheetML

open ISADotNet

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
                |> Option.get
                |> SheetData.getRows
                |> Seq.map (Row.mapCells (Cell.includeSharedStringValue sst.Value))
                |> MetaData.fromRows

            let sheetNames = 
                Spreadsheet.getWorkbookPart doc
                |> Workbook.get
                |> Sheet.Sheets.get
                |> Sheet.Sheets.getSheets
                |> Seq.map Sheet.getName
        
            let characteristics,factors,protocols,processes =
                sheetNames
                |> Seq.collect (fun sheetname ->
                    match Spreadsheet.tryGetWorksheetPartBySheetName sheetname doc with
                    | Some wsp ->
                        match Table.tryGetByNameBy (fun s -> s.Contains "annotationTable") wsp with
                        | Some table -> 
                            let sheet = Worksheet.getSheetData wsp.Worksheet
                            let headers = Table.getColumnHeaders table
                            let m = Table.toSparseValueMatrix sst sheet table
                            AnnotationTable.splitIntoProtocols sheetname [] headers
                            |> Seq.map (fun (protocolMetaData,headers) ->
                                let characteristic,factors,protocol,processGetter = 
                                    AnnotationTable.splitIntoColumns headers
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