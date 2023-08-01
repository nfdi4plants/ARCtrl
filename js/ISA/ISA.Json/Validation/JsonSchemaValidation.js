import { singleton } from "../../../fable_modules/fable-library.4.1.4/AsyncBuilder.js";
import { validate } from "./Fable.js";
import { ValidationResult, ValidationResult_OfJSchemaOutput_Z6EC48F6B } from "./ValidationResult.js";

export function Validation_validate(schemaURL, objectString) {
    return singleton.Delay(() => singleton.TryWith(singleton.Delay(() => singleton.Bind(validate(schemaURL, objectString), (_arg) => singleton.Return(ValidationResult_OfJSchemaOutput_Z6EC48F6B([_arg[0], _arg[1]])))), (_arg_1) => singleton.Return(new ValidationResult(1, [[_arg_1.message]]))));
}

export function Validation_validateAssay(assayString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/assay_schema.json", assayString);
}

export function Validation_validateComment(commentString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/comment_schema.json", commentString);
}

export function Validation_validateData(dataString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/data_schema.json", dataString);
}

export function Validation_validateFactor(factorString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/factor_schema.json", factorString);
}

export function Validation_validateFactorValue(factorValueString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/factor_value_schema.json", factorValueString);
}

export function Validation_validateInvestigation(investigationString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/investigation_schema.json", investigationString);
}

export function Validation_validateMaterialAttribute(materialAttributeString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/material_attribute_schema.json", materialAttributeString);
}

export function Validation_validateMaterialAttributeValue(materialAttributeValueString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/material_attribute_value_schema.json", materialAttributeValueString);
}

export function Validation_validateMaterial(materialString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/material_schema.json", materialString);
}

export function Validation_validateOntologyAnnotation(ontologyAnnotationString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/ontology_annotation_schema.json", ontologyAnnotationString);
}

export function Validation_validateOntologySourceReference(ontologySourceReferenceString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/ontology_source_reference_schema.json", ontologySourceReferenceString);
}

export function Validation_validatePerson(personString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/person_schema.json", personString);
}

export function Validation_validateProcessParameterValue(processParameterValueString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/process_parameter_value_schema.json", processParameterValueString);
}

export function Validation_validateProcess(processString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/process_schema.json", processString);
}

export function Validation_validateProtocolParameter(protocolParameterString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/protocol_parameter_schema.json", protocolParameterString);
}

export function Validation_validateProtocol(protocolString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/protocol_schema.json", protocolString);
}

export function Validation_validatePublication(publicationString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/publication_schema.json", publicationString);
}

export function Validation_validateSample(sampleString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/sample_schema.json", sampleString);
}

export function Validation_validateSource(sourceString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/source_schema.json", sourceString);
}

export function Validation_validateStudy(studyString) {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/study_schema.json", studyString);
}

