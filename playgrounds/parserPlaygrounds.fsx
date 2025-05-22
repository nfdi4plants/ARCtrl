#I "../src/ARCtrl/bin/Release/netstandard2.0"
#I "../src/ARCtrl/bin/Debug/netstandard2.0"

#r "ARCtrl.Spreadsheet.dll"
#r "ARCtrl.Core.dll"

#r "nuget: FsSpreadsheet.ExcelIO"


open ARCtrl
open ARCtrl.Spreadsheet
open ARCtrl.Spreadsheet.ArcInvestigation
open FsSpreadsheet
open FsSpreadsheet.ExcelIO
open System.Collections.Generic


let invFile = FsWorkbook.fromXlsxFile @"C:\Users\olive\OneDrive\CSB-Stuff\NFDI\errorARCs\investigation1ExtraMadeUpKey\isa.investigation.xlsx"
let invFileGesund = FsWorkbook.fromXlsxFile @"C:\Users\olive\OneDrive\CSB-Stuff\NFDI\testARC37\isa.investigation.xlsx"


type SparseTable with
    static member fromRows(en:IEnumerator<SparseRow>,labels,lineNumber,?prefix) =
        try
            let prefix = match prefix with | Option.Some p -> p + " " | None -> ""
            let rec loop (matrix : SparseTable) remarks lineNumber = 

                if en.MoveNext() then  
                    let row = en.Current |> Seq.map (fun (i,v) -> int i - 1,v)
                    let key,vals = Seq.tryItem 0 row |> Option.map snd, Seq.trySkip 1 row
                    match key,vals with
                    | Option.Some (Comment.Comment k), Option.Some v -> 
                        loop (SparseTable.AddComment k v matrix) remarks (lineNumber + 1)

                    | Remark.Remark k, _  ->
                        loop matrix (Remark.make lineNumber k :: remarks) (lineNumber + 1)

                    | Option.Some k, Option.Some v when List.exists (fun label -> k = prefix + label) labels -> 
                        let label = List.find (fun label -> k = prefix + label) labels
                        loop (SparseTable.AddRow label v matrix) remarks (lineNumber + 1)

                    | Option.Some k, _ -> Option.Some k,lineNumber,remarks, matrix
                    | _ -> loop matrix remarks (lineNumber + 1)
                else
                    None,lineNumber,remarks, matrix
            loop (SparseTable.Create()) [] lineNumber
        with
        | err -> failwithf "Error parsing block in investigation file starting from line number %i: %s" lineNumber err.Message


module OntologySourceReference =

    let nameLabel = "Term Source Name"
    let fileLabel = "Term Source File"
    let versionLabel = "Term Source Version"
    let descriptionLabel = "Term Source Description"
    let labels = [nameLabel;fileLabel;versionLabel;descriptionLabel]

    let fromString description file name version comments =
        OntologySourceReference.make
            (description)
            (file)
            (name)
            (version)
            comments

    let fromSparseTable (matrix : SparseTable) =
        if matrix.ColumnCount = 0 && matrix.CommentKeys.Length <> 0 then
            let comments = SparseTable.GetEmptyComments matrix
            OntologySourceReference.create(Comments = comments)
            |> List.singleton
        else
            List.init matrix.ColumnCount (fun i -> 

                let comments = 
                    matrix.CommentKeys 
                    |> List.map (fun k -> 
                        Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))
                    |> ResizeArray

                fromString
                    (matrix.TryGetValue(descriptionLabel,i))
                    (matrix.TryGetValue(fileLabel,i))
                    (matrix.TryGetValue(nameLabel,i))
                    (matrix.TryGetValue(versionLabel,i))
                    comments
            )

    let fromRows lineNumber (rows : IEnumerator<SparseRow>) =
        SparseTable.FromRows(rows,labels,lineNumber)
        |> fun (s,ln,rs,sm) -> (s,ln,rs, fromSparseTable sm)


let fromRows (rows : seq<SparseRow>) =
    if Seq.isEmpty rows then failwith "Investigation file is empty"

    let en = rows.GetEnumerator()              
        
    let emptyInvestigationInfo = InvestigationInfo.create "" "" "" "" "" []

    let rec loop lastLine ontologySourceReferences investigationInfo publications contacts studies remarks lineNumber =
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

        | _ ->
            match en.MoveNext() with
            | true ->
                let currentLine = en.Current |> SparseRow.tryGetValueAt 0
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies remarks lineNumber
            | false ->
                let studies, assays = 
                    studies 
                    |> List.unzip 
                    |> fun (s, a) -> 
                        s |> List.rev, 
                        a |> List.concat |> List.distinctBy (fun a -> a.Identifier)
                fromParts investigationInfo ontologySourceReferences publications contacts studies assays remarks

    let arcInvestigation =
        en.MoveNext() |> ignore
        let currentLine = en.Current |> SparseRow.tryGetValueAt 0
        loop currentLine [] emptyInvestigationInfo [] [] [] [] 1

    if arcInvestigation.Identifier.Equals System.String.Empty then failwith "Mandatory Investigation identifier is not present"

    arcInvestigation


invFile
|> ArcInvestigation.tryGetMetadataSheet
|> Option.get
|> FsWorksheet.getRows
|> Seq.map SparseRow.fromFsRow
|> ArcInvestigation.fromRows
//|> fromRows

invFileGesund
|> ArcInvestigation.tryGetMetadataSheet
|> Option.get
|> FsWorksheet.getRows
|> Seq.map SparseRow.fromFsRow
//|> ArcInvestigation.fromRows
|> fromRows
