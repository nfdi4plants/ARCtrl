namespace ISADotNet.XLSX.AssayFile


open FSharpSpreadsheetML
open ISADotNet
open ISADotNet.XLSX

module MetaData =

    let assayLabel = "ASSAY METADATA"
    let contactsLabel = "ASSAY PERFORMERS"

    /// Map Assay Metadata to excel rows
    let toRows (assay:Assay) (contacts : Person list) =
        seq {          

            yield  Row.ofValues None 0u [assayLabel]
            yield! Assays.writeAssays (None) [assay]

            yield  Row.ofValues None 0u [contactsLabel]
            yield! Contacts.writePersons (None) contacts
        }
        |> Seq.mapi (fun i row -> Row.updateRowIndex (i+1 |> uint) row)

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


