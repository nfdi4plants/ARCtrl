namespace ISADotNet.XSLX

open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open Comment
open Remark
open System.Collections.Generic

module DesignDescriptors = 

    let designTypeLabel = "Type"
    let designTypeTermAccessionNumberLabel = "Type Term Accession Number"
    let designTypeTermSourceREFLabel = "Type Term Source REF"

    let createDesign designType typeTermAccessionNumber typeTermSourceREF comments =
        OntologyAnnotation.create null (AnnotationValue.fromString designType) typeTermAccessionNumber typeTermSourceREF comments

    let readDesigns (prefix : string) lineNumber (en:IEnumerator<Row>) =
        let rec loop 
            designTypes typeTermAccessionNumbers typeTermSourceREFs
            comments remarks lineNumber = 

            let create () = 
                let length =
                    [|designTypes;typeTermAccessionNumbers;typeTermSourceREFs|]
                    |> Array.map Array.length
                    |> Array.max

                List.init length (fun i ->
                    let comments = 
                        List.map (fun (key,values) -> 
                            Comment.create "" key (Array.tryItemDefault i "" values)
                        ) comments
                    createDesign
                        (Array.tryItemDefault i "" designTypes)
                        (Array.tryItemDefault i "" typeTermAccessionNumbers)
                        (Array.tryItemDefault i "" typeTermSourceREFs)
                        comments
                )

            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v) |> Array.ofIndexedSeq
                match Array.tryItem 0 row , Array.trySkip 1 row with

                | Comment k, Some v -> 
                    loop 
                        designTypes typeTermAccessionNumbers typeTermSourceREFs
                        ((k,v) :: comments) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop 
                        designTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments (Remark.create lineNumber k :: remarks) (lineNumber + 1)

                | Some k, Some designTypes when k = prefix + " " + designTypeLabel -> 
                    loop 
                        designTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some typeTermAccessionNumbers when k = prefix + " " + designTypeTermAccessionNumberLabel -> 
                    loop 
                        designTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some typeTermSourceREFs when k = prefix + " " + designTypeTermSourceREFLabel -> 
                    loop 
                        designTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,create ()
                | _ -> None, lineNumber,remarks,create ()
            else
                None,lineNumber,remarks,create ()
        loop [||] [||] [||] [] [] lineNumber

    
    
    let writeDesigns prefix (designs : OntologyAnnotation list) =
        let commentKeys = designs |> List.collect (fun design -> design.Comments |> List.map (fun c -> c.Name))
    
        seq {

            yield   ( Row.ofValues None 0u (prefix + " " + designTypeLabel                      :: (designs |> List.map (fun design -> design.Name))))
            yield   ( Row.ofValues None 0u (prefix + " " + designTypeTermAccessionNumberLabel   :: (designs |> List.map (fun design -> design.TermAccessionNumber))))
            yield   ( Row.ofValues None 0u (prefix + " " + designTypeTermSourceREFLabel         :: (designs |> List.map (fun design -> design.TermSourceREF))))
    
            for key in commentKeys do
                let values = 
                    designs |> List.map (fun design -> 
                        List.tryPickDefault (fun (c : Comment) -> if c.Name = key then Some c.Value else None) "" design.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }