namespace ISADotNet.QueryModel.Linq

open ISADotNet

module Errors =

    exception MissingParameterException         of ProtocolParameter
    exception MissingCharacteristicException    of MaterialAttribute
    exception MissingFactorException            of Factor
    exception MissingCategoryException          of OntologyAnnotation

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


    let missingValue (oa : OntologyAnnotation) =
        raise (MissingValueException(oa))

    let noSynonymInTargetOntology (oa : OntologyAnnotation) (targetOnt : string) =
        raise (NoSynonymInTargOntologyException(oa,targetOnt))

    let missingProtocolWithType (oa : OntologyAnnotation) =
        raise (MissingProtocolWithTypeException(oa))

    let protocolHasNoDescription (name : string) =
        raise (ProtocolHasNoDescriptionException(name))