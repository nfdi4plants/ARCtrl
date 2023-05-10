module TestingUtils

open Expecto
open ISADotNet.Json

module Async = 

    let map (f : 'U -> 'T) (a : Async<'U>) =
        async {
            let! a' =  a
            return f a'           
        }

module Utils = 

    let extractWords (json:string) = 
        json.Split([|'{';'}';'[';']';',';':'|])
        |> Array.map (fun s -> s.Trim())
        |> Array.filter ((<>) "")

    let wordFrequency (json:string) = 
        json
        |> extractWords
        |> Array.countBy id
        |> Array.sortBy fst

//module MyExpect =

 

//    let matchingResult (vr : ValidationTypes.ValidationResult)=
//        Expect.isTrue vr.Success (sprintf "Json Object did not match Json Schema: %A" (vr.GetErrors()))

//    let matchingAssay (assayString : string) =
//        Validation.validateAssay assayString
//        |> Async.map matchingResult

//    let matchingComment (commentString : string) =
//        Validation.validateComment commentString
//        |> Async.map matchingResult

//    let matchingData (dataString : string) =
//        Validation.validateData dataString
//        |> Async.map matchingResult
    
//    let matchingFactor (factorString : string) =
//        Validation.validateFactor factorString
//        |> Async.map matchingResult

//    let matchingFactorValue (factorValueString : string) =
//        Validation.validateFactorValue factorValueString
//        |> Async.map matchingResult

//    let matchingInvestigation (investigationString : string) =
//        Validation.validateInvestigation investigationString
//        |> Async.map matchingResult

//    let matchingMaterialAttribute (materialAttributeString : string) =
//        Validation.validateMaterialAttribute materialAttributeString
//        |> Async.map matchingResult

//    let matchingMaterialAttributeValue (materialAttributeValueString : string) =
//        Validation.validateMaterialAttributeValue materialAttributeValueString
//        |> Async.map matchingResult

//    let matchingMaterial (materialString : string) =
//        Validation.validateMaterial materialString
//        |> Async.map matchingResult

//    let matchingOntologyAnnotation (ontologyAnnotationString : string) =
//        Validation.validateOntologyAnnotation ontologyAnnotationString
//        |> Async.map matchingResult

//    let matchingOntologySourceReference (ontologySourceReferenceString : string) =
//        Validation.validateOntologySourceReference ontologySourceReferenceString
//        |> Async.map matchingResult
    
//    let matchingPerson (personString : string) =
//        Validation.validatePerson personString
//        |> Async.map matchingResult

//    let matchingProcessParameterValue (processParameterValueString : string) =
//        Validation.validateProcessParameterValue processParameterValueString
//        |> Async.map matchingResult

//    let matchingProcess (processString : string) =
//        Validation.validateProcess processString
//        |> Async.map matchingResult

//    let matchingProtocolParameter (protocolParameterString : string) =
//        Validation.validateProtocolParameter protocolParameterString
//        |> Async.map matchingResult

//    let matchingProtocol (protocolString : string) =
//        Validation.validateProtocol protocolString
//        |> Async.map matchingResult

//    let matchingPublication (publicationString : string) =
//        Validation.validatePublication publicationString
//        |> Async.map matchingResult

//    let matchingSample (sampleString : string) =
//        Validation.validateSample sampleString
//        |> Async.map matchingResult

//    let matchingSource (sourceString : string) =
//        Validation.validateSource sourceString
//        |> Async.map matchingResult

//    let matchingStudy (studyString : string) =
//        Validation.validateStudy studyString
//        |> Async.map matchingResult

module Result =

    let getMessage res =
        match res with
        | Ok m -> m
        | Error m -> m

let firstDiff s1 s2 =
  let s1 = Seq.append (Seq.map Some s1) (Seq.initInfinite (fun _ -> None))
  let s2 = Seq.append (Seq.map Some s2) (Seq.initInfinite (fun _ -> None))
  Seq.mapi2 (fun i s p -> i,s,p) s1 s2
  |> Seq.find (function |_,Some s,Some p when s=p -> false |_-> true)

/// Expects the `actual` sequence to equal the `expected` one.
let inline mySequenceEqual actual expected message =
  match firstDiff actual expected with
  | _,None,None -> ()
  | i,Some a, Some e ->
    failwithf "%s. Sequence does not match at position %i. Expected item: %A, but got %A."
      message i e a
  | i,None,Some e ->
    failwithf "%s. Sequence actual shorter than expected, at pos %i for expected item %A."
      message i e
  | i,Some a,None ->
    failwithf "%s. Sequence actual longer than expected, at pos %i found item %A."
      message i a