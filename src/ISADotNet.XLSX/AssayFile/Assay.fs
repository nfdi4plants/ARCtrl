namespace ISADotNet.XLSX.AssayFile

open System.Collections.Generic
open FSharpSpreadsheetML

open ISADotNet

/// Functions for parsing an ISAXLSX Assay File
///
/// This is based on the ISA.Tab Format: https://isa-specs.readthedocs.io/en/latest/isatab.html#assay-table-file
///
/// But with the table being modified according to the SWATE tool: https://github.com/nfdi4plants/Swate
///
/// Additionally, the file can contain several sheets containing parameter tables and a sheet containing additional assay metadata
module AssayFile =

    /// Create a new ISADotNet.XLSX assay file constisting of two sheets. The first has the name of the assayIdentifier and is meant to store parameters used in the assay. The second stores additional assay metadata
    let init metadataSheetName assayIdentifier path =
        Spreadsheet.initWithSST assayIdentifier path
        |> MetaData.init metadataSheetName 
        |> Spreadsheet.close

    let fromSparseMatrix (processNameRoot:string) matrixHeaders (matrixLength:int) (sparseMatrix : Dictionary<string*int,string>) = 
        let characteristic,factors,protocol,processGetter = 
            AnnotationNode.splitIntoNodes matrixHeaders
            |> AnnotationTable.getProcessGetter ({Protocol.empty with Name = Some processNameRoot}) 
        characteristic,factors,protocol,
            
        Seq.init matrixLength (processGetter sparseMatrix)
        |> AnnotationTable.mergeIdenticalProcesses
        |> AnnotationTable.indexRelatedProcessesByProtocolName


    /// Parses the assay file
    let fromSpreadsheet (doc:DocumentFormat.OpenXml.Packaging.SpreadsheetDocument) = 
        
        let sst = Spreadsheet.tryGetSharedStringTable doc

        // Get the metadata from the metadata sheet
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
        
        let characteristics,factors,protocols,processes =
            sheetNames
            |> Seq.collect (fun sheetName ->                    
                match Spreadsheet.tryGetWorksheetPartBySheetName sheetName doc with
                | Some wsp ->
                    match Table.tryGetByNameBy (fun s -> s.StartsWith "annotationTable") wsp with
                    | Some table -> 
                        let sheet = Worksheet.getSheetData wsp.Worksheet
                        let headers = Table.getColumnHeaders table
                        let m = Table.toSparseValueMatrix sst sheet table
                        let length = 
                            Table.getArea table
                            |> fun area -> Table.Area.lowerBoundary area - Table.Area.upperBoundary area |> int

                        Seq.singleton (fromSparseMatrix sheetName headers length m)
                    
                    | None -> Seq.empty
                | None -> Seq.empty                
            )
            |> Seq.fold (fun (characteristics',factors',protocols',processes') (characteristics,factors,protocol,processes) ->
                List.append characteristics' characteristics |> List.distinct,
                List.append factors' factors |> List.distinct,
                Seq.append protocols' (Seq.singleton protocol),
                Seq.append processes' processes
            ) (List.empty,List.empty,Seq.empty,Seq.empty)

        let processes = AnnotationTable.updateSamplesByThemselves processes
            
        let assay = assayMetaData |> Option.defaultValue Assay.empty
            
        factors,
        protocols |> Seq.toList,
        contacts,
        {assay with 
                ProcessSequence = Option.fromValueWithDefault [] (processes |> Seq.toList)
                CharacteristicCategories = Option.fromValueWithDefault [] characteristics
        }

    /// Parses the assay file
    let fromFile (path:string) = 
        let doc = Spreadsheet.fromFile path false
        try
            fromSpreadsheet doc
        finally
            Spreadsheet.close doc

    /// Parses the assay file
    let fromStream (stream:#System.IO.Stream) = 

        let doc = Spreadsheet.fromStream stream false
        try
            fromSpreadsheet doc
        finally
            Spreadsheet.close doc