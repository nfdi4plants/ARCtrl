namespace ISADotNet.XLSX.AssayFile


open FSharpSpreadsheetML
open ISADotNet
open ISADotNet.XLSX

module MetaData =

    let assaysLabel = "ASSAY METADATA"
    let contactsLabel = "ASSAY PERFORMERS"

    /// Map Assay Metadata to excel rows
    let toRows (assay:Assay) (contacts : Person list) =
        seq {          

            yield  Row.ofValues None 0u [assaysLabel]
            yield! Assays.writeAssays (None) [assay]

            yield  Row.ofValues None 0u [contactsLabel]
            yield! Contacts.writePersons (None) contacts
        }
        |> Seq.mapi (fun i row -> Row.updateRowIndex (i+1 |> uint) row)

    let fromRows (rows: seq<DocumentFormat.OpenXml.Spreadsheet.Row>) =
        let en = rows.GetEnumerator()
        let rec loop lastLine assays contacts lineNumber =
               
            match lastLine with

            | Some k when k = assaysLabel -> 
                let currentLine,lineNumber,_,assays = Assays.readAssays None (lineNumber + 1) en       
                loop currentLine assays contacts lineNumber

            | Some k when k = contactsLabel -> 
                let currentLine,lineNumber,_,contacts = Contacts.readPersons None (lineNumber + 1) en  
                loop currentLine assays contacts lineNumber

            | k -> 
                assays |> Seq.tryHead,contacts
        
        if en.MoveNext () then
            let currentLine = en.Current |> Row.tryGetValueAt None 1u
            loop currentLine [] [] 1
            
        else
            failwith "emptyInvestigationFile"



    /// Append an assay metadata sheet with the given sheetname to an existing assay file excel spreadsheet
    let init sheetName (doc: DocumentFormat.OpenXml.Packaging.SpreadsheetDocument) = 

        let sheet = SheetData.empty()

        let worksheetComment = Comment.create None (Some "Worksheet") None
        let personWithComment = Person.create None None None None None None None None None None (Some [worksheetComment])
            
        toRows Assay.empty [personWithComment]
        |> Seq.fold (fun s r -> 
            SheetData.appendRow r s
        ) sheet
        |> ignore

        doc
        |> Spreadsheet.getWorkbookPart
        |> WorkbookPart.appendSheet sheetName sheet
        |> ignore 

        doc


