namespace ISADotNet.XLSX

open DocumentFormat.OpenXml.Spreadsheet
open FsSpreadsheet.ExcelIO
open ISADotNet
open ISADotNet.API
open Comment
open Remark
open System.Collections.Generic

module OntologySourceReference = 

    let nameLabel = "Term Source Name"
    let fileLabel = "Term Source File"
    let versionLabel = "Term Source Version"
    let descriptionLabel = "Term Source Description"

    
    let labels = [nameLabel;fileLabel;versionLabel;descriptionLabel]

    let fromString description file name version comments =
        OntologySourceReference.make
            (Option.fromValueWithDefault "" description)
            (Option.fromValueWithDefault "" file)
            (Option.fromValueWithDefault "" name)
            (Option.fromValueWithDefault "" version)
            (Option.fromValueWithDefault [] comments)

    let fromSparseTable (matrix : SparseTable) =
        
        List.init matrix.Length (fun i -> 

            let comments = 
                matrix.CommentKeys 
                |> List.map (fun k -> 
                    Comment.fromString k (matrix.TryGetValueDefault("",(k,i))))

            fromString
                (matrix.TryGetValueDefault("",(descriptionLabel,i)))
                (matrix.TryGetValueDefault("",(fileLabel,i)))
                (matrix.TryGetValueDefault("",(nameLabel,i)))
                (matrix.TryGetValueDefault("",(versionLabel,i)))
                comments
        )

    let toSparseTable (ontologySources: OntologySourceReference list) =
        let matrix = SparseTable.Create (keys = labels,length=ontologySources.Length + 1)
        let mutable commentKeys = []
        ontologySources
        |> List.iteri (fun i o ->
            let i = i + 1
            do matrix.Matrix.Add ((nameLabel,i),        (Option.defaultValue "" o.Name))
            do matrix.Matrix.Add ((fileLabel,i),        (Option.defaultValue "" o.File))
            do matrix.Matrix.Add ((versionLabel,i),     (Option.defaultValue "" o.Version))
            do matrix.Matrix.Add ((descriptionLabel,i), (Option.defaultValue "" o.Description))

            match o.Comments with 
            | None -> ()
            | Some c ->
                c
                |> List.iter (fun comment -> 
                    let n,v = comment |> Comment.toString
                    commentKeys <- n :: commentKeys
                    matrix.Matrix.Add((n,i),v)
                )     
        )
        {matrix with CommentKeys = commentKeys |> List.distinct |> List.rev}

    let fromRows lineNumber (rows : IEnumerator<SparseRow>) =
        SparseTable.FromRows(rows,labels,lineNumber)
        |> fun (s,ln,rs,sm) -> (s,ln,rs, fromSparseTable sm)
    
    let toRows (termSources : OntologySourceReference list) =
        termSources
        |> toSparseTable
        |> fun m -> SparseTable.ToRows(m)