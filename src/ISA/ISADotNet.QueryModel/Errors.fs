namespace ISADotNet.QueryModel

open ISADotNet

module Errors =

    exception MissingParameterException         of ProtocolParameter
    exception MissingCharacteristicException    of MaterialAttribute
    exception MissingFactorException            of Factor
    exception MissingCategoryException          of OntologyAnnotation

    exception MissingOATextException            of OntologyAnnotation

    exception MissingProtocolException          of string
    exception MissingProtocolWithTypeException  of OntologyAnnotation
    exception ProtocolHasNoDescriptionException of string

    exception MissingValueException             of OntologyAnnotation
    exception MissingUnitException              of OntologyAnnotation
    exception NoSynonymInTargOntologyException  of OntologyAnnotation*string

    exception FieldNotFilledException           of string


    let missingCategory (oa : OntologyAnnotation) =
        raise (MissingCategoryException(oa))

    let missingFactor (oa : OntologyAnnotation) =
        raise (MissingFactorException(Factor.create(FactorType = oa)))

    let missingParameter (oa : OntologyAnnotation) =
        raise (MissingParameterException(ProtocolParameter.create(ParameterName = oa)))

    let missingCharacteristic (oa : OntologyAnnotation) =
        raise (MissingCharacteristicException(MaterialAttribute.create(CharacteristicType = oa)))

    let missingOAText (oa : OntologyAnnotation) =
        raise (MissingOATextException(oa))


    let missingValue (oa : OntologyAnnotation) =
        raise (MissingValueException(oa))

    let noSynonymInTargetOntology (oa : OntologyAnnotation) (targetOnt : string) =
        raise (NoSynonymInTargOntologyException(oa,targetOnt))

    let missingProtocolWithType (oa : OntologyAnnotation) =
        raise (MissingProtocolWithTypeException(oa))

    let protocolHasNoDescription (name : string) =
        raise (ProtocolHasNoDescriptionException(name))


    exception InvestigationHasNoIDException
    exception InvestigationHasNoFileNameException
    exception InvestigationHasNoIdentifierException
    exception InvestigationHasNoTitleException
    exception InvestigationHasNoDescriptionException
    exception InvestigationHasNoSubmissionDateException
    exception InvestigationHasNoPublicReleaseDateException
    exception InvestigationHasNoOntologySourceReferencesException
    exception InvestigationHasNoPublicationsException
    exception InvestigationHasNoContactsException
    exception InvestigationHasNoStudiesException
    exception InvestigationHasNoCommentsException

    exception StudyHasNoIDException
    exception StudyHasNoFileNameException
    exception StudyHasNoIdentifierException
    exception StudyHasNoTitleException
    exception StudyHasNoDescriptionException
    exception StudyHasNoSubmissionDateException
    exception StudyHasNoPublicReleaseDateException
    exception StudyHasNoPublicationsException
    exception StudyHasNoContactsException
    exception StudyHasNoDesignDescriptorsException
    exception StudyHasNoProtocolsException
    exception StudyHasNoMaterialsException
    exception StudyHasNoProcessSequenceException
    exception StudyHasNoAssaysException
    exception StudyHasNoFactorsException
    exception StudyHasNoCharacteristicCategoriesException
    exception StudyHasNoUnitCategoriesException
    exception StudyHasNoStudyHasNoCommentsExceptionException




    exception PersonHasNoIDException
    exception PersonHasNoLastNameException
    exception PersonHasNoFirstNameException
    exception PersonHasNoMidInitialsException
    exception PersonHasNoEMailException
    exception PersonHasNoPhoneException
    exception PersonHasNoFaxException
    exception PersonHasNoAddressException
    exception PersonHasNoAffiliationException
    exception PersonHasNoRolesException
    exception PersonHasNoCommentsException 

module ErrorHandling = 

    open Errors 
    open ISADotNet.Builder

    let tryMessageToStudyTransformation (processName : string) (e : exn) =

        match e with
        | :? MissingCategoryException as exc -> 
            let processTransformation = 
                [
                    ProcessTransformation.AddProtocol [ProtocolTransformation.AddName processName]
                    ProcessTransformation.AddParameter (ProcessParameterValue.create(ProtocolParameter.create(ParameterName = exc.Data0)))
                    ProcessTransformation.AddName processName
                ]

            StudyTransformation.AddProcess processTransformation
            |> Option.Some
        | :? MissingParameterException  as exc -> 
            let processTransformation = 
                [
                    ProcessTransformation.AddProtocol [ProtocolTransformation.AddName processName]
                    ProcessTransformation.AddParameter (ProcessParameterValue.create(exc.Data0))
                    ProcessTransformation.AddName processName
                ]

            StudyTransformation.AddProcess processTransformation
            |> Option.Some
        | :? MissingCharacteristicException     as exc -> 
            let processTransformation = 
                [
                    ProcessTransformation.AddProtocol [ProtocolTransformation.AddName processName]
                    ProcessTransformation.AddCharacteristic (MaterialAttributeValue.create(Category = exc.Data0))
                    ProcessTransformation.AddName processName
                ]

            StudyTransformation.AddProcess processTransformation
            |> Option.Some
        | :? MissingFactorException             as exc -> 
            let processTransformation = 
                [
                    ProcessTransformation.AddProtocol [ProtocolTransformation.AddName processName]
                    ProcessTransformation.AddFactor (FactorValue.create(Category = exc.Data0))
                    ProcessTransformation.AddName processName
                ]

            StudyTransformation.AddProcess processTransformation
            |> Option.Some
                                                                      
        | :? MissingProtocolException as exc ->
            let processTransformation = 
                [
                    ProcessTransformation.AddProtocol [ProtocolTransformation.AddName exc.Data0]
                    ProcessTransformation.AddName exc.Data0
                ]

            StudyTransformation.AddProcess processTransformation
            |> Option.Some
        | :? MissingProtocolWithTypeException   as exc ->                                    
            let processTransformation = 
                [
                    ProcessTransformation.AddProtocol [
                        ProtocolTransformation.AddName exc.Data0.NameText
                        ProtocolTransformation.AddProtocolType exc.Data0
                    ]
                    ProcessTransformation.AddName exc.Data0.NameText
                ]

            StudyTransformation.AddProcess processTransformation
            |> Option.Some
        //| :? ProtocolHasNoDescriptionException  as exc -> [NoneRequired([message exc])]
                                                                      
        //| :? MissingValueException              as exc -> [NoneRequired([message exc])]
        //| :? MissingUnitException               as exc -> [NoneRequired([message exc])]
        //| :? NoSynonymInTargOntologyException   as exc -> [NoneRequired([message exc])]
        | err -> None


    let tryMessageToTransformation (processName : string) (e : exn) =

        match e with
        | :? MissingCategoryException as exc -> 
            let processTransformation = 
                [
                    ProcessTransformation.AddProtocol [ProtocolTransformation.AddName processName]
                    ProcessTransformation.AddParameter (ProcessParameterValue.create(ProtocolParameter.create(ParameterName = exc.Data0)))
                    ProcessTransformation.AddName processName
                ]

            AssayTransformation.AddProcess processTransformation
            |> Option.Some
        | :? MissingParameterException  as exc -> 
            let processTransformation = 
                [
                    ProcessTransformation.AddProtocol [ProtocolTransformation.AddName processName]
                    ProcessTransformation.AddParameter (ProcessParameterValue.create(exc.Data0))
                    ProcessTransformation.AddName processName
                ]

            AssayTransformation.AddProcess processTransformation
            |> Option.Some
        | :? MissingCharacteristicException     as exc -> 
            let processTransformation = 
                [
                    ProcessTransformation.AddProtocol [ProtocolTransformation.AddName processName]
                    ProcessTransformation.AddCharacteristic (MaterialAttributeValue.create(Category = exc.Data0))
                    ProcessTransformation.AddName processName
                ]

            AssayTransformation.AddProcess processTransformation
            |> Option.Some
        | :? MissingFactorException             as exc -> 
            let processTransformation = 
                [
                    ProcessTransformation.AddProtocol [ProtocolTransformation.AddName processName]
                    ProcessTransformation.AddFactor (FactorValue.create(Category = exc.Data0))
                    ProcessTransformation.AddName processName
                ]

            AssayTransformation.AddProcess processTransformation
            |> Option.Some
                                                                      
        | :? MissingProtocolException as exc ->
            let processTransformation = 
                [
                    ProcessTransformation.AddProtocol [ProtocolTransformation.AddName exc.Data0]
                    ProcessTransformation.AddName exc.Data0
                ]

            AssayTransformation.AddProcess processTransformation
            |> Option.Some
        | :? MissingProtocolWithTypeException   as exc ->                                    
            let processTransformation = 
                [
                    ProcessTransformation.AddProtocol [
                        ProtocolTransformation.AddName exc.Data0.NameText
                        ProtocolTransformation.AddProtocolType exc.Data0
                    ]
                    ProcessTransformation.AddName exc.Data0.NameText
                ]

            AssayTransformation.AddProcess processTransformation
            |> Option.Some
        //| :? ProtocolHasNoDescriptionException  as exc -> [NoneRequired([message exc])]
                                                                      
        //| :? MissingValueException              as exc -> [NoneRequired([message exc])]
        //| :? MissingUnitException               as exc -> [NoneRequired([message exc])]
        //| :? NoSynonymInTargOntologyException   as exc -> [NoneRequired([message exc])]
        | err -> None

    let getTransformations (processName : string) (es : exn list) =
        es 
        |> List.choose (tryMessageToTransformation processName)

    let getStudyformations (processName : string) (es : exn list) =
        es 
        |> List.choose (tryMessageToStudyTransformation processName)