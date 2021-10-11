namespace ISADotNet.XLSX.AssayFile


open FSharpSpreadsheetML
open ISADotNet
open ISADotNet.XLSX

/// Functions for reading and writing the additional information stored in the assay metadata sheet
module MetaData =

    let assaysLabel = "ASSAY METADATA"
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

            | Some k when k = assaysLabel -> 
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


    /// Diesen Block durch JS ersetzen ----> 

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
    let init sheetName (doc: DocumentFormat.OpenXml.Packaging.SpreadsheetDocument) = 

        let sheet = SheetData.empty()

        let worksheetComment = Comment.make None (Some "Worksheet") None
        let personWithComment = Person.make None None None None None None None None None None (Some [worksheetComment])
            
        toRows Assay.empty [personWithComment]
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

    /// ---->  Bis hier
