namespace ISADotNet.XSLX

open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open Comment
open Remark
open System.Collections.Generic

module OntologySourceReference = 

    let nameLabel = "Term Source Name"
    let fileLabel = "Term Source File"
    let versionLabel = "Term Source Version"
    let descriptionLabel = "Term Source Description"

    let readTermSources lineNumber (en:IEnumerator<Row>) =
        let rec loop 
            names files versions descriptions
            comments remarks lineNumber = 

            let create () = 
                let length =
                    [|names;files;versions;descriptions|]
                    |> Array.map Array.length
                    |> Array.max

                List.init length (fun i ->
                    let comments = 
                        List.map (fun (key,values) -> 
                            Comment.create "" key (Array.tryItemDefault i "" values)
                        ) comments
                    OntologySourceReference.create
                        (Array.tryItemDefault i "" descriptions)
                        (Array.tryItemDefault i "" files)
                        (Array.tryItemDefault i "" names)
                        (Array.tryItemDefault i "" versions)
                        comments
                )
            
            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v) |> Array.ofIndexedSeq

                match Array.tryItem 0 row , Array.trySkip 1 row with

                | Comment k, Some v -> 
                    loop 
                        names files versions descriptions
                        ((k,v) :: comments) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop 
                        names files versions descriptions
                        comments (Remark.create lineNumber k :: remarks) (lineNumber + 1)

                | Some k, Some names when k = nameLabel -> 
                    loop 
                        names files versions descriptions
                        comments remarks (lineNumber + 1)

                | Some k, Some files when k = fileLabel -> 
                    loop 
                        names files versions descriptions
                        comments remarks (lineNumber + 1)

                | Some k, Some versions when k = versionLabel -> 
                    loop 
                        names files versions descriptions
                        comments remarks (lineNumber + 1)

                | Some k, Some descriptions when k = descriptionLabel -> 
                    loop 
                        names files versions descriptions
                        comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,create ()
                | _ -> None, lineNumber,remarks,create ()
            else
                None,lineNumber,remarks,create ()
        loop [||] [||] [||] [||] [] [] lineNumber

    
    let writeTermSources (termSources : OntologySourceReference list) =
        let commentKeys = termSources |> List.collect (fun termSource -> termSource.Comments |> List.map (fun c -> c.Name))

        seq {
            yield   (Row.ofValues None 0u (nameLabel          :: (termSources |> List.map (fun termSource -> termSource.Name))))
            yield   (Row.ofValues None 0u (fileLabel          :: (termSources |> List.map (fun termSource -> termSource.File))))
            yield   (Row.ofValues None 0u (versionLabel       :: (termSources |> List.map (fun termSource -> termSource.Version))))
            yield   (Row.ofValues None 0u (descriptionLabel   :: (termSources |> List.map (fun termSource -> termSource.Description))))

            for key in commentKeys do
                let values = 
                    termSources |> List.map (fun termSource -> 
                        List.tryPickDefault (fun (c : Comment) -> if c.Name = key then Some c.Value else None) "" termSource.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }