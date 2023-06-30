module ISA.Spreadsheet.ArcAssay

open ISA
open FsSpreadsheet

let obsoloteAssaysLabel = "ASSAY METADATA"
let assaysLabel = "ASSAY"
let contactsLabel = "ASSAY PERFORMERS"

let metaDataSheetName = "Assay"

let toMetadataSheet (assay : ArcAssay) : FsWorksheet =
    let toRows (assay:ArcAssay) =
        seq {          

            yield  SparseRow.fromValues [assaysLabel]
            yield! Assays.toRows (None) [assay]

            yield  SparseRow.fromValues [contactsLabel]
            yield! Contacts.toRows (None) (assay.Performers |> Option.defaultValue [])
        }
    let sheet = FsWorksheet(metaDataSheetName)
    assay
    |> toRows
    |> Seq.iteri (fun rowI r -> SparseRow.writeToSheet rowI r sheet)    
    sheet

let fromMetadataSheet (sheet : FsWorksheet) : ArcAssay =
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
                assays |> Seq.tryHead 
                |> Option.defaultValue (ArcAssay.create(sheet.Name)) 
                |> ArcAssay.setPerformers (Option.fromValueWithDefault [] contacts)
        
        if en.MoveNext () then
            let currentLine = en.Current |> SparseRow.tryGetValueAt 0
            loop currentLine [] [] 1
            
        else
            failwith "empty assay metadata sheet"
    sheet.Rows 
    |> Seq.map SparseRow.fromFsRow
    |> fromRows


/// Reads an assay from a spreadsheet
let fromFsWorkbook (doc:FsWorkbook) = 
    // Reading the "Assay" metadata sheet. Here metadata 
    let assayMetaData = 
        
        match doc.TryGetWorksheetByName metaDataSheetName with 
        | Option.Some sheet ->
            fromMetadataSheet sheet
        | None -> 
            printfn "Cannot retrieve metadata: Assay file does not contain \"%s\" sheet." metaDataSheetName
            ArcAssay.create("New Assay")
    let sheets = 
        doc.GetWorksheets()
        |> List.choose ArcTable.tryFromFsWorksheet
    if sheets.IsEmpty then
        assayMetaData
    else {
        assayMetaData with Tables = ResizeArray(sheets)
    }

let toFsWorkbook (assay : ArcAssay) =
    let doc = new FsWorkbook()
    let metaDataSheet = toMetadataSheet assay
    doc.AddWorksheet metaDataSheet

    assay.Tables
    |> Seq.iter (ArcTable.toFsWorksheet >> doc.AddWorksheet)

    doc