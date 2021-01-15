namespace ISADotNet.XSLX


open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open Comment
open Remark
open System.Collections.Generic

module Factors = 
    
    let nameLabel = "Name"
    let factorTypeLabel = "Type"
    let typeTermAccessionNumberLabel = "Type Term Accession Number"
    let typeTermSourceREFLabel = "Type Term Source REF"

    let readFactors (prefix : string) lineNumber (en:IEnumerator<Row>) =
        let rec loop 
            names factorTypes typeTermAccessionNumbers typeTermSourceREFs
            comments remarks lineNumber = 

            let create () = 
                let length =
                    [|names;factorTypes;typeTermAccessionNumbers;typeTermSourceREFs|]
                    |> Array.map Array.length
                    |> Array.max

                List.init length (fun i ->
                    let factorType = 
                        OntologyAnnotation.create 
                            (Array.tryItemDefault i "" factorTypes)
                            (Array.tryItemDefault i "" typeTermAccessionNumbers)
                            (Array.tryItemDefault i "" typeTermSourceREFs)
                            []
                        |> REF.Item
                    let comments = 
                        List.map (fun (key,values) -> 
                            Comment.create "" key (Array.tryItemDefault i "" values)
                        ) comments
                    Factor.create
                        ""
                        (Array.tryItemDefault i "" names)
                        factorType
                        comments
                    |> REF.Item
                )
            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v) |> Array.ofIndexedSeq
                match Array.tryItem 0 row , Array.trySkip 1 row with

                | Comment k, Some v -> 
                    loop 
                        names factorTypes typeTermAccessionNumbers typeTermSourceREFs
                        ((k,v) :: comments) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop 
                        names factorTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments (Remark.create lineNumber k :: remarks) (lineNumber + 1)

                | Some k, Some names when k = prefix + " " + nameLabel -> 
                    loop 
                        names factorTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some factorTypes when k = prefix + " " + factorTypeLabel -> 
                    loop 
                        names factorTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some typeTermAccessionNumbers when k = prefix + " " + typeTermAccessionNumberLabel -> 
                    loop 
                        names factorTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some typeTermSourceREFs when k = prefix + " " + typeTermSourceREFLabel -> 
                    loop 
                        names factorTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,create ()
                | _ -> None, lineNumber,remarks,create ()
            else
                None,lineNumber,remarks,create ()
        loop [||] [||] [||] [||] [] [] lineNumber

    
    
    let writeFactors prefix (factors : Factor list) =
        let commentKeys = factors |> List.collect (fun factor -> factor.Comments |> List.map (fun c -> c.Name)) |> List.distinct
    
        seq {
            let factorTypes,factorTypeAccessions,factorTypeSourceRefs = factors |> List.map (fun p -> dismantleOntology p.FactorType) |> List.unzip3
            yield   ( Row.ofValues None 0u (prefix + " " + nameLabel                      :: (factors |> List.map (fun factor -> factor.Name))))
            yield   ( Row.ofValues None 0u (prefix + " " + factorTypeLabel                      :: factorTypes))
            yield   ( Row.ofValues None 0u (prefix + " " + typeTermAccessionNumberLabel                      :: factorTypeAccessions))
            yield   ( Row.ofValues None 0u (prefix + " " + typeTermSourceREFLabel                      :: factorTypeSourceRefs))
    
            for key in commentKeys do
                let values = 
                    factors |> List.map (fun factor -> 
                        List.tryPickDefault (fun (c : Comment) -> if c.Name = key then Some c.Value else None) "" factor.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }