namespace ISADotNet.XLSX.AssayFile


open FsSpreadsheet.ExcelIO
open ISADotNet
open ISADotNet.XLSX

/// Functions for reading and writing the additional information stored in the assay metadata sheet
module MetaData =

    let obsoloteAssaysLabel = "ASSAY METADATA"
    let assaysLabel = "ASSAY"
    let contactsLabel = "ASSAY PERFORMERS"

    /// Write Assay Metadata to excel rows
    let toRows (assay:Assay) (contacts : Person list) =
        seq {          

            yield  SparseRow.fromValues [assaysLabel]
            yield! Assays.toRows (None) [assay]

            yield  SparseRow.fromValues [contactsLabel]
            yield! Contacts.toRows (None) contacts
        }
        

    /// Read Assay Metadata from excel rows
    let fromRows (rows: seq<SparseRow>) =
        let en = rows.GetEnumerator()
        let rec loop lastLine assays contacts lineNumber =
               
            match lastLine with

            | Some k when k = assaysLabel || k = obsoloteAssaysLabel -> 
                let currentLine,lineNumber,_,assays = Assays.fromRows None (lineNumber + 1) en       
                loop currentLine assays contacts lineNumber

            | Some k when k = contactsLabel -> 
                let currentLine,lineNumber,_,contacts = Contacts.fromRows None (lineNumber + 1) en  
                loop currentLine assays contacts lineNumber

            | k -> 
                assays |> Seq.tryHead,contacts
        
        if en.MoveNext () then
            let currentLine = en.Current |> SparseRow.tryGetValueAt 0
            loop currentLine [] [] 1
            
        else
            failwith "emptyInvestigationFile"

    let toDSLSheet a c =
        toRows a c 
        |> Seq.map SparseRow.toDSLRow

    ///let doc = Spreadsheet.fromFile path true  
    ///  
    ///MetadataSheet.overwriteWithAssayInfo "Investigation" testAssay2 doc
    ///
    ///MetadataSheet.overwriteWithPersons "Investigation" [person] doc
    /// 
    ///MetadataSheet.getPersons "Investigation" doc
    ///
    ///MetadataSheet.tryGetAssay "Investigation" doc
    ///  
    ///doc.Close()

    /// Diesen Block durch JS ersetzen ----> 

    open DocumentFormat.OpenXml.Packaging
    open DocumentFormat.OpenXml.Spreadsheet

    /// Creates a new row from the given values.
    let ofSparseValues rowIndex (vals : 'T option seq) =
        let spans = Row.Spans.fromBoundaries 1u (Seq.length vals |> uint)
        vals
        |> Seq.mapi (fun i value -> 
            value
            |> Option.map (Cell.fromValue None (i + 1 |> uint) rowIndex)
        )
        |> Seq.choose id
        |> Row.create rowIndex spans 

    /// Append an assay metadata sheet with the given sheetname to an existing assay file excel spreadsheet
    let init sheetName (assay : Assay option) (persons : Person list option) (doc: DocumentFormat.OpenXml.Packaging.SpreadsheetDocument) = 

        let sheet = SheetData.empty()

        let worksheetComment = Comment.make None (Some "Worksheet") None
        let personWithComment = [Person.make None None None None None None None None None None (Some [worksheetComment])]
        
        let assay = Option.defaultValue Assay.empty assay
        let persons = Option.defaultValue personWithComment persons

        toRows assay persons
        |> Seq.mapi (fun i row -> 
            row
            |> SparseRow.getAllValues
            |> ofSparseValues (i+1 |> uint)
        )
        |> Seq.fold (fun s r -> 
            SheetData.appendRow r s
        ) sheet
        |> ignore

        doc
        |> Spreadsheet.getWorkbookPart
        |> WorkbookPart.appendSheet sheetName sheet
        |> ignore 

        doc


    /// Try get assay from metadatasheet with given sheetName
    let tryGetAssay sheetName (doc : SpreadsheetDocument) = 
        let sst = Spreadsheet.tryGetSharedStringTable doc
        match Spreadsheet.tryGetSheetBySheetName sheetName doc with
        | Some sheet -> 
            sheet
            |> SheetData.getRows
            |> Seq.map (Row.getIndexedValues sst >> Seq.map (fun (i,v) -> (int i) - 1, v))
            |> fromRows
            |> fun (a,p) ->
                a
        | None -> failwithf "Metadata sheetname %s could not be found" sheetName

    /// Try get persons from metadatasheet with given sheetName
    let getPersons sheetName (doc : SpreadsheetDocument) = 
        let sst = Spreadsheet.tryGetSharedStringTable doc
        match Spreadsheet.tryGetSheetBySheetName sheetName doc with
        | Some sheet -> 
            sheet
            |> SheetData.getRows
            |> Seq.map (Row.getIndexedValues sst >> Seq.map (fun (i,v) -> (int i) - 1, v))
            |> fromRows
            |> fun (a,p) ->
                p
        | None -> failwithf "Metadata sheetname %s could not be found" sheetName

    /// Replaces assay metadata from metadatasheet with given sheetName
    let overwriteWithAssayInfo sheetName assay (doc : SpreadsheetDocument) = 

        let sst = Spreadsheet.tryGetSharedStringTable doc
        let workBookPart = Spreadsheet.getWorkbookPart doc
        let newSheet = SheetData.empty()    

        match Spreadsheet.tryGetSheetBySheetName sheetName doc with
        | Some sheet -> 
            sheet
            |> SheetData.getRows
            |> Seq.map (Row.getIndexedValues sst >> Seq.map (fun (i,v) -> (int i) - 1, v))
            |> fromRows
            |> fun (_,p) ->
            
                toRows assay p
                |> Seq.mapi (fun i row -> 
                    row
                    |> SparseRow.getAllValues
                    |> ofSparseValues (i+1 |> uint)
                )
                |> Seq.fold (fun s r -> 
                    SheetData.appendRow r s
                ) newSheet
                |> fun s -> WorkbookPart.replaceSheetDataByName sheetName s workBookPart
        | None -> failwithf "Metadata sheetname %s could not be found" sheetName
        |> ignore

        doc.Save() 

    /// Replaces persons from metadatasheet with given sheetName
    let overwriteWithPersons sheetName persons (doc : SpreadsheetDocument) = 

        let sst = Spreadsheet.tryGetSharedStringTable doc
        let workBookPart = Spreadsheet.getWorkbookPart doc
        let newSheet = SheetData.empty()       

        match Spreadsheet.tryGetSheetBySheetName sheetName doc with
        | Some sheet -> 
            sheet
            |> SheetData.getRows
            |> Seq.map (Row.getIndexedValues sst >> Seq.map (fun (i,v) -> (int i) - 1, v))
            |> fromRows
            |> fun (a,_) ->            
                toRows (Option.defaultValue Assay.empty a) persons
                |> Seq.mapi (fun i row -> 
                    row
                    |> SparseRow.getAllValues
                    |> ofSparseValues (i+1 |> uint)
                )
                |> Seq.fold (fun s r -> 
                    SheetData.appendRow r s
                ) newSheet
                |> fun s -> WorkbookPart.replaceSheetDataByName sheetName s workBookPart
        | None -> failwithf "Metadata sheetname %s could not be found" sheetName
        |> ignore

        doc.Save() 

    /// ---->  Bis hier
