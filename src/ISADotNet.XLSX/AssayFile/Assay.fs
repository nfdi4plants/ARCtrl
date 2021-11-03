namespace ISADotNet.XLSX.AssayFile

open System.Collections.Generic
open FSharpSpreadsheetML

open ISADotNet

module Process = 

    /// Returns processes and other additional information from a sparseMatrix represntation of an assay.xlsx sheet
    ///
    /// processNameRoot is the sheetName (or the protocol name you want to use)
    ///
    /// matrixHeaders are the column headers of the table
    ///
    /// sparseMatrix is a sparse representation of the sheet table, with the first part of the key being the column header and the second part being a zero based row index
    let fromSparseMatrix (processNameRoot:string) matrixHeaders (sparseMatrix : Dictionary<int*string,string>) = 
        let len = 
            let mutable i = 0
            for kv in sparseMatrix do 
                let j = kv.Key |> fst
                if j > i  then i <- j
            i + 1
        let characteristic,factors,protocol,processGetter = 
            AnnotationNode.splitIntoNodes matrixHeaders
            |> AnnotationTable.getProcessGetter ({Protocol.empty with Name = Some processNameRoot}) 
        characteristic,factors,protocol,
            
        Seq.init len (processGetter sparseMatrix)
        |> AnnotationTable.mergeIdenticalProcesses
        |> AnnotationTable.indexRelatedProcessesByProtocolName

/// Functions for parsing an ISAXLSX Assay File
///
/// This is based on the ISA.Tab Format: https://isa-specs.readthedocs.io/en/latest/isatab.html#assay-table-file
///
/// But with the table being modified according to the SWATE tool: https://github.com/nfdi4plants/Swate
///
/// Additionally, the file can contain several sheets containing parameter tables and a sheet containing additional assay metadata
module Assay =

    /// Returns an assay from a sparseMatrix represntation of an assay.xlsx sheet
    ///
    /// processNameRoot is the sheetName (or the protocol name you want to use)
    ///
    /// matrixHeaders are the column headers of the table
    ///
    /// sparseMatrix is a sparse representation of the sheet table, with the first part of the key being the column header and the second part being a zero based row index
    let fromSparseMatrix (processNameRoot:string) matrixHeaders (sparseMatrix : Dictionary<int*string,string>) = 
        let characteristics,factors,protocols,processes = Process.fromSparseMatrix processNameRoot matrixHeaders sparseMatrix
        factors,protocols,Assay.create(CharacteristicCategories = characteristics,ProcessSequence = Seq.toList processes)

    /// Returns an assay from a sequence of sparseMatrix representations of assay.xlsx sheets
    ///
    /// See "fromSparseMatrix" function for parameter documentation
    let fromSparseMatrices (sheets : (string*(string seq)*Dictionary<int*string,string>) seq) = 
        let characteristics,factors,protocols,processes =
            sheets
            |> Seq.map (fun (name,matrixHeaders,matrix) -> Process.fromSparseMatrix name matrixHeaders matrix)
            |> Seq.fold (fun (characteristics',factors',protocols',processes') (characteristics,factors,protocol,processes) ->
                List.append characteristics' characteristics |> List.distinct,
                List.append factors' factors |> List.distinct,
                Seq.append protocols' (Seq.singleton protocol),
                Seq.append processes' processes
            ) (List.empty,List.empty,Seq.empty,Seq.empty)

        let processes = AnnotationTable.updateSamplesByThemselves processes

        factors,protocols,Assay.create(CharacteristicCategories = characteristics,ProcessSequence = Seq.toList processes)

/// Diesen Block durch JS ersetzen ----> 

    /// Create a new ISADotNet.XLSX assay file constisting of two sheets. The first has the name of the assayIdentifier and is meant to store parameters used in the assay. The second stores additional assay metadata
    let init metadataSheetName assay persons assayIdentifier path =
        Spreadsheet.initWithSst assayIdentifier path
        |> MetaData.init metadataSheetName assay persons
        |> Spreadsheet.close

    /// Reads an assay from an xlsx spreadsheetdocument
    ///
    /// As factors and protocols are used for the investigation file, they are returned individually
    ///
    /// The persons from the metadata sheet are returned independently as they are not a part of the assay object
    let fromSpreadsheet (doc:DocumentFormat.OpenXml.Packaging.SpreadsheetDocument) = 
        
        let sst = Spreadsheet.tryGetSharedStringTable doc

        // Reading the "Investigation" metadata sheet. Here metadata 
        let assayMetaData,contacts = 
            Spreadsheet.tryGetSheetBySheetName "Investigation" doc
            |> Option.map (fun sheet -> 
                sheet
                |> SheetData.getRows
                |> Seq.map (Row.mapCells (Cell.includeSharedStringValue sst.Value))
                |> Seq.map (Row.getIndexedValues None >> Seq.map (fun (i,v) -> (int i) - 1, v))
                |> MetaData.fromRows
                |> fun (a,p) -> Option.defaultValue Assay.empty a, p
            )
            |> Option.defaultValue (Assay.empty,[])          
        
        // All sheetnames in the spreadsheetDocument
        let sheetNames = 
            Spreadsheet.getWorkbookPart doc
            |> Workbook.get
            |> Sheet.Sheets.get
            |> Sheet.Sheets.getSheets
            |> Seq.map Sheet.getName
        
        let factors,protocols,assay =
            sheetNames
            |> Seq.collect (fun sheetName ->                    
                match Spreadsheet.tryGetWorksheetPartBySheetName sheetName doc with
                | Some wsp ->
                    match Table.tryGetByNameBy (fun s -> s.StartsWith "annotationTable") wsp with
                    | Some table -> 
                        // Extract the sheetdata as a sparse matrix
                        let sheet = Worksheet.getSheetData wsp.Worksheet
                        let headers = Table.getColumnHeaders table
                        let m = Table.toSparseValueMatrix sst sheet table
                        Seq.singleton (sheetName,headers,m)     
                    | None -> Seq.empty
                | None -> Seq.empty                
            )
            |> fromSparseMatrices // Feed the sheets (represented as sparse matrices) into the assay parser function
            
        factors,
        protocols |> Seq.toList,
        contacts,
        API.Update.UpdateByExisting.updateRecordType assayMetaData assay // Merges the assay containing the assay meta data and the assay containing the processes retrieved from the sheets

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

    /// ---->  Bis hier