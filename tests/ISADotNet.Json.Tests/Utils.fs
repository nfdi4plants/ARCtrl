module TestingUtils

open Expecto
   
module Utils = 

    let extractWords (json:string) = 
        json.Split([|'{';'}';'[';']';',';':'|])
        |> Array.map (fun s -> s.Trim())
        |> Array.filter ((<>) "")

//module MyExpect =

//    let matchingResult (vr : ValidationResult)=
//        Expect.isTrue vr.Success (sprintf "Json Object did not match Json Schema: %A" (vr.GetErrors()))

//    let matchingAssay (assayString : string) =
//        ISADotNet.Validation.JSchema.validateAssay assayString
//        |> matchingResult

//    let matchingComment (commentString : string) =
//        ISADotNet.Validation.JSchema.validateComment commentString
//        |> matchingResult

//    let matchingData (dataString : string) =
//        ISADotNet.Validation.JSchema.validateData dataString
//        |> matchingResult
    
//    let matchingFactor (factorString : string) =
//        ISADotNet.Validation.JSchema.validateFactor factorString
//        |> matchingResult

//    let matchingFactorValue (factorValueString : string) =
//        ISADotNet.Validation.JSchema.validateFactorValue factorValueString
//        |> matchingResult

//    let matchingInvestigation (investigationString : string) =
//        ISADotNet.Validation.JSchema.validateInvestigation investigationString
//        |> matchingResult

//    let matchingMaterialAttribute (materialAttributeString : string) =
//        ISADotNet.Validation.JSchema.validateMaterialAttribute materialAttributeString
//        |> matchingResult

//    let matchingMaterialAttributeValue (materialAttributeValueString : string) =
//        ISADotNet.Validation.JSchema.validateMaterialAttributeValue materialAttributeValueString
//        |> matchingResult

//    let matchingMaterial (materialString : string) =
//        ISADotNet.Validation.JSchema.validateMaterial materialString
//        |> matchingResult

//    let matchingOntologyAnnotation (ontologyAnnotationString : string) =
//        ISADotNet.Validation.JSchema.validateOntologyAnnotation ontologyAnnotationString
//        |> matchingResult

//    let matchingOntologySourceReference (ontologySourceReferenceString : string) =
//        ISADotNet.Validation.JSchema.validateOntologySourceReference ontologySourceReferenceString
//        |> matchingResult
    
//    let matchingPerson (personString : string) =
//        ISADotNet.Validation.JSchema.validatePerson personString
//        |> matchingResult

//    let matchingProcessParameterValue (processParameterValueString : string) =
//        ISADotNet.Validation.JSchema.validateProcessParameterValue processParameterValueString
//        |> matchingResult

//    let matchingProcess (processString : string) =
//        ISADotNet.Validation.JSchema.validateProcess processString
//        |> matchingResult

//    let matchingProtocolParameter (protocolParameterString : string) =
//        ISADotNet.Validation.JSchema.validateProtocolParameter protocolParameterString
//        |> matchingResult

//    let matchingProtocol (protocolString : string) =
//        ISADotNet.Validation.JSchema.validateProtocol protocolString
//        |> matchingResult

//    let matchingPublication (publicationString : string) =
//        ISADotNet.Validation.JSchema.validatePublication publicationString
//        |> matchingResult

//    let matchingSample (sampleString : string) =
//        ISADotNet.Validation.JSchema.validateSample sampleString
//        |> matchingResult

//    let matchingSource (sourceString : string) =
//        ISADotNet.Validation.JSchema.validateSource sourceString
//        |> matchingResult

//    let matchingStudy (studyString : string) =
//        ISADotNet.Validation.JSchema.validateStudy studyString
//        |> matchingResult

module Result =

    let getMessage res =
        match res with
        | Ok m -> m
        | Error m -> m

let private firstDiff s1 s2 =
  let s1 = Seq.append (Seq.map Some s1) (Seq.initInfinite (fun _ -> None))
  let s2 = Seq.append (Seq.map Some s2) (Seq.initInfinite (fun _ -> None))
  Seq.mapi2 (fun i s p -> i,s,p) s1 s2
  |> Seq.find (function |_,Some s,Some p when s=p -> false |_-> true)

/// Expects the `actual` sequence to equal the `expected` one.
let mySequenceEqual actual expected message =
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