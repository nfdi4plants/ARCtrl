module JsonSchemaValidation

open Expecto
open FSharp.Data
open NJsonSchema

open ISADotNet.Validation

module Expect =


    let matchingResult (vr : ValidationResult)=
        Expect.isTrue vr.Success (sprintf "Json Object did not match Json Schema: %A" (vr.GetErrors()))

    let matchingAssay (assayString : string) =
        ISADotNet.Validation.JSchema.validateAssay assayString
        |> matchingResult

    let matchingComment (commentString : string) =
        ISADotNet.Validation.JSchema.validateComment commentString
        |> matchingResult

    let matchingData (dataString : string) =
        ISADotNet.Validation.JSchema.validateData dataString
        |> matchingResult
    
    let matchingFactor (factorString : string) =
        ISADotNet.Validation.JSchema.validateFactor factorString
        |> matchingResult

    let matchingFactorValue (factorValueString : string) =
        ISADotNet.Validation.JSchema.validateFactorValue factorValueString
        |> matchingResult

    let matchingInvestigation (investigationString : string) =
        ISADotNet.Validation.JSchema.validateInvestigation investigationString
        |> matchingResult

    let matchingMaterialAttribute (materialAttributeString : string) =
        ISADotNet.Validation.JSchema.validateMaterialAttribute materialAttributeString
        |> matchingResult

    let matchingMaterialAttributeValue (materialAttributeValueString : string) =
        ISADotNet.Validation.JSchema.validateMaterialAttributeValue materialAttributeValueString
        |> matchingResult

    let matchingMaterial (materialString : string) =
        ISADotNet.Validation.JSchema.validateMaterial materialString
        |> matchingResult

    let matchingOntologyAnnotation (ontologyAnnotationString : string) =
        ISADotNet.Validation.JSchema.validateOntologyAnnotation ontologyAnnotationString
        |> matchingResult

    let matchingOntologySourceReference (ontologySourceReferenceString : string) =
        ISADotNet.Validation.JSchema.validateOntologySourceReference ontologySourceReferenceString
        |> matchingResult
    
    let matchingPerson (personString : string) =
        ISADotNet.Validation.JSchema.validatePerson personString
        |> matchingResult

    let matchingProcessParameterValue (processParameterValueString : string) =
        ISADotNet.Validation.JSchema.validateProcessParameterValue processParameterValueString
        |> matchingResult

    let matchingProcess (processString : string) =
        ISADotNet.Validation.JSchema.validateProcess processString
        |> matchingResult

    let matchingProtocolParameter (protocolParameterString : string) =
        ISADotNet.Validation.JSchema.validateProtocolParameter protocolParameterString
        |> matchingResult

    let matchingProtocol (protocolString : string) =
        ISADotNet.Validation.JSchema.validateProtocol protocolString
        |> matchingResult

    let matchingPublication (publicationString : string) =
        ISADotNet.Validation.JSchema.validatePublication publicationString
        |> matchingResult

    let matchingSample (sampleString : string) =
        ISADotNet.Validation.JSchema.validateSample sampleString
        |> matchingResult

    let matchingSource (sourceString : string) =
        ISADotNet.Validation.JSchema.validateSource sourceString
        |> matchingResult

    let matchingStudy (studyString : string) =
        ISADotNet.Validation.JSchema.validateStudy studyString
        |> matchingResult
