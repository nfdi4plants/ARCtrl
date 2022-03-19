namespace ISADotNet.XLSX.StudyFile

open System.Collections.Generic
open FsSpreadsheet.ExcelIO

open ISADotNet


/// Functions for parsing an ISAXLSX Study File
///
/// This is based on the ISA.Tab Format: https://isa-specs.readthedocs.io/en/latest/isatab.html#assay-table-file
///
/// But with the table being modified according to the SWATE tool: https://github.com/nfdi4plants/Swate
///
/// Additionally, the file can contain several sheets containing parameter tables and a sheet containing additional study metadata
module Study =

    /// Returns a stuy from a sparseMatrix represntation of an study.xlsx sheet
    ///
    /// processNameRoot is the sheetName (or the protocol name you want to use)
    ///
    /// matrixHeaders are the column headers of the table
    ///
    /// sparseMatrix is a sparse representation of the sheet table, with the first part of the key being the column header and the second part being a zero based row index
    let fromSparseMatrix (processNameRoot:string) matrixHeaders (sparseMatrix : Dictionary<int*string,string>) = 
        let characteristics,factors,protocol,processes = ISADotNet.XLSX.AssayFile.Process.fromSparseMatrix processNameRoot matrixHeaders sparseMatrix
        Study.create(CharacteristicCategories = characteristics,Factors = factors, Protocols = [protocol], ProcessSequence = processes)

    /// Returns a study from a sequence of sparseMatrix representations of study.xlsx sheets
    ///
    /// See "fromSparseMatrix" function for parameter documentation
    let fromSparseMatrices (sheets : (string*(string seq)*Dictionary<int*string,string>) seq) = 
        let characteristics,factors,protocols,processes =
            sheets
            |> Seq.map (fun (name,matrixHeaders,matrix) -> ISADotNet.XLSX.AssayFile.Process.fromSparseMatrix name matrixHeaders matrix)
            |> Seq.fold (fun (characteristics',factors',protocols',processes') (characteristics,factors,protocol,processes) ->
                List.append characteristics' characteristics |> List.distinct,
                List.append factors' factors |> List.distinct,
                List.append protocols' (List.singleton protocol),
                List.append processes' processes
            ) (List.empty,List.empty,List.empty,List.empty)

        let processes = ISADotNet.XLSX.AssayFile.AnnotationTable.updateSamplesByThemselves processes |> Seq.toList

        Study.create(CharacteristicCategories = characteristics,Factors = factors, Protocols = protocols, ProcessSequence = processes)

/// Diesen Block durch JS ersetzen ----> 

    /// Create a new ISADotNet.XLSX study file constisting of two sheets. The first has the name of the studyIdentifier and is meant to store parameters used in the study. The second stores additional study metadata
    let init study studyIdentifier path =
        Spreadsheet.initWithSst studyIdentifier path
        |> MetaData.init "Study" study
        |> Spreadsheet.close

    /// Reads a study from an xlsx spreadsheetdocument
    ///
    /// As factors and protocols are used for the investigation file, they are returned individually
    let fromSpreadsheet (doc:DocumentFormat.OpenXml.Packaging.SpreadsheetDocument) = 
        
        let sst = Spreadsheet.tryGetSharedStringTable doc

        // Reading the "Study" metadata sheet. Here metadata 
        let studyMetaData = 
            Spreadsheet.tryGetSheetBySheetName "Study" doc
            |> Option.map (fun sheet -> 
                sheet
                |> SheetData.getRows
                |> Seq.map (Row.mapCells (Cell.includeSharedStringValue sst.Value))
                |> Seq.map (Row.getIndexedValues None >> Seq.map (fun (i,v) -> (int i) - 1, v))
                |> MetaData.fromRows
                
            )
            |> Option.defaultValue (Study.empty)          
        
        // All sheetnames in the spreadsheetDocument
        let sheetNames = 
            Spreadsheet.getWorkbookPart doc
            |> Workbook.get
            |> Sheet.Sheets.get
            |> Sheet.Sheets.getSheets
            |> Seq.map Sheet.getName
        
        let study =
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
            |> fromSparseMatrices // Feed the sheets (represented as sparse matrices) into the study parser function
            
        API.Update.UpdateByExisting.updateRecordType studyMetaData study // Merges the study containing the sutdy meta data and the study containing the processes retrieved from the sheets

    /// Parses the study file
    let fromFile (path:string) = 
        let doc = Spreadsheet.fromFile path false
        try
            fromSpreadsheet doc
        finally
            Spreadsheet.close doc

    /// Parses the study file
    let fromStream (stream:#System.IO.Stream) = 

        let doc = Spreadsheet.fromStream stream false
        try
            fromSpreadsheet doc
        finally
            Spreadsheet.close doc

    /// ---->  Bis hier