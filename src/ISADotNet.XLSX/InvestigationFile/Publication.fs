namespace ISADotNet.XSLX

open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open Comment
open Remark
open System.Collections.Generic

module Publications = 

    let readPublications (prefix : string) lineNumber (en:IEnumerator<Row>) =
        let rec loop 
            pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
            comments remarks lineNumber = 

            let create () = 
                let length =
                    [|pubMedIDs;dois;authorLists;titles;statuss;statusTermAccessionNumbers;statusTermSourceREFs|]
                    |> Array.map Array.length
                    |> Array.max

                List.init length (fun i ->
                    let status = 
                        OntologyAnnotation.create 
                            (Array.tryItemDefault i "" statuss)
                            (Array.tryItemDefault i "" statusTermAccessionNumbers)
                            (Array.tryItemDefault i "" statusTermSourceREFs)
                            []
                        |> REF.Item
                    let comments = 
                        List.map (fun (key,values) -> 
                            Comment.create "" key (Array.tryItemDefault i "" values)
                        ) comments
                    Publication.create
                        (Array.tryItemDefault i "" pubMedIDs)
                        (Array.tryItemDefault i "" dois)
                        (Array.tryItemDefault i "" authorLists)
                        (Array.tryItemDefault i "" titles)
                        status
                        comments
                    |> REF.Item
                )
            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v) |> Array.ofIndexedSeq

                match Array.tryItem 0 row , Array.trySkip 1 row with

                | Comment k, Some v -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        ((k,v) :: comments) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments (Remark.create lineNumber k :: remarks) (lineNumber + 1)

                | Some k, Some pubMedIDs when k = prefix + " " + Publication.PubMedIDTab -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)
                | Some k, Some pubMedIDs when k = prefix.Replace(" Publication","") + " " + Publication.PubMedIDTab ->               
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some dois when k = prefix + " " + Publication.DOITab -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some authorLists when k = prefix + " " + Publication.AuthorListTab -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some titles when k = prefix + " " + Publication.TitleTab -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some statuss when k = prefix + " " + Publication.StatusTab -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some statusTermAccessionNumbers when k = prefix + " " + Publication.StatusTermAccessionNumberTab -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some statusTermSourceREFs when k = prefix + " " + Publication.StatusTermSourceREFTab -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,create ()
                | _ -> None, lineNumber,remarks,create ()
            else
                None,lineNumber,remarks,create ()
        loop [||] [||] [||] [||] [||] [||] [||] [] [] lineNumber

    
    let writePublications prefix (publications : Publication list) =
        let commentKeys = publications |> List.collect (fun publication -> publication.Comments |> List.map (fun c -> c.Name)) |> List.distinct

        seq {
            let statusTerms,statusTermAccession,statusTermSources = publications |> List.map (fun p -> dismantleOntology p.Status) |> List.unzip3
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.PubMedIDTab                      :: (publications |> List.map (fun publication -> publication.PubMedID))))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.DOITab                      :: (publications |> List.map (fun publication -> publication.DOI))))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.AuthorListTab                      :: (publications |> List.map (fun publication -> publication.Authors))))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.TitleTab                      :: (publications |> List.map (fun publication -> publication.Title))))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.StatusTab                      :: statusTerms))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.StatusTermAccessionNumberTab                      :: statusTermAccession))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.StatusTermSourceREFTab                      :: statusTermSources))

            for key in commentKeys do
                let values = 
                    publications |> List.map (fun publication -> 
                        List.tryPickDefault (fun (c : Comment) -> if c.Name = key then Some c.Value else None) "" publication.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }
