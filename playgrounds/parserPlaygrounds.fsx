#I "../src/ARCtrl/bin/Release/netstandard2.0"
#I "../src/ARCtrl/bin/Debug/netstandard2.0"

#r "ARCtrl.Spreadsheet.dll"
#r "ARCtrl.Core.dll"

#r "nuget: FsSpreadsheet.ExcelIO"


open ARCtrl.Spreadsheet
open ARCtrl.Spreadsheet.ArcInvestigation
open FsSpreadsheet
open FsSpreadsheet.ExcelIO


let fromRows (rows : seq<SparseRow>) =
    if Seq.isEmpty rows then failwith "Investigation file is empty"

    let en = rows.GetEnumerator()              
        
    let emptyInvestigationInfo = InvestigationInfo.create "" "" "" "" "" []

    let rec loop lastLine ontologySourceReferences investigationInfo publications contacts studies remarks lineNumber =
        match en.MoveNext() with
        | true ->
            printfn $"last line is: {lastLine}"
            //let currentLine = en.Current |> SparseRow.tryGetValueAt 0
            match lastLine with

            | Some k when k = ontologySourceReferenceLabel -> 
                let currentLine, lineNumber, newRemarks, ontologySourceReferences = OntologySourceReference.fromRows (lineNumber + 1) en
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = investigationLabel -> 
                let currentLine,lineNumber,newRemarks,investigationInfo = InvestigationInfo.fromRows (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = publicationsLabel -> 
                let currentLine,lineNumber,newRemarks,publications = Publications.fromRows (Some publicationsLabelPrefix) (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = contactsLabel -> 
                let currentLine,lineNumber, newRemarks, contacts = Contacts.fromRows (Some contactsLabelPrefix) (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = studyLabel -> 
                let currentLine, lineNumber, newRemarks, study = Studies.fromRows (lineNumber + 1) en  
                if study.IsSome then
                    loop currentLine ontologySourceReferences investigationInfo publications contacts (study.Value::studies) (List.append remarks newRemarks) lineNumber
                else 
                    loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | _ -> loop (en.Current |> SparseRow.tryGetValueAt 0) ontologySourceReferences investigationInfo publications contacts studies remarks (lineNumber + 1)

        | false ->
            let studies, assays = 
                studies 
                |> List.unzip 
                |> fun (s, a) -> 
                    s |> List.rev, 
                    a |> List.concat |> List.distinctBy (fun a -> a.Identifier)
            fromParts investigationInfo ontologySourceReferences publications contacts studies assays remarks

    //let currentLine = en.Current |> SparseRow.tryGetValueAt 0
    let arcInvestigation = loop None [] emptyInvestigationInfo [] [] [] [] 0

    //if arcInvestigation.Identifier.Equals System.String.Empty then failwith "Mandatory Investigation identifier is not present"

    arcInvestigation


let invFile = FsWorkbook.fromXlsxFile @"C:\Users\olive\OneDrive\CSB-Stuff\NFDI\errorARCs\investigation1ExtraMadeUpKey\isa.investigation.xlsx"

invFile
|> ArcInvestigation.tryGetMetadataSheet
|> Option.get
|> FsWorksheet.getRows
|> Seq.map SparseRow.fromFsRow
|> ArcInvestigation.fromRows
//|> fromRows
