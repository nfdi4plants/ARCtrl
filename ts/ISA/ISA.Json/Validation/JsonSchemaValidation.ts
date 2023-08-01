import { singleton } from "../../../fable_modules/fable-library-ts/AsyncBuilder.js";
import { validate } from "./Fable.js";
import { ValidationResult_Failed, ValidationResult_$union, ValidationResult_OfJSchemaOutput_Z6EC48F6B } from "./ValidationResult.js";

export function Validation_validate(schemaURL: string, objectString: string): any {
    return singleton.Delay<ValidationResult_$union>((): any => singleton.TryWith<ValidationResult_$union>(singleton.Delay<ValidationResult_$union>((): any => singleton.Bind<[boolean, string[]], ValidationResult_$union>(validate(schemaURL, objectString), (_arg: [boolean, string[]]): any => singleton.Return<ValidationResult_$union>(ValidationResult_OfJSchemaOutput_Z6EC48F6B([_arg[0], _arg[1]] as [boolean, string[]])))), (_arg_1: Error): any => singleton.Return<ValidationResult_$union>(ValidationResult_Failed([_arg_1.message]))));
}

export function Validation_validateAssay(assayString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/assay_schema.json", assayString);
}

export function Validation_validateComment(commentString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/comment_schema.json", commentString);
}

export function Validation_validateData(dataString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/data_schema.json", dataString);
}

export function Validation_validateFactor(factorString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/factor_schema.json", factorString);
}

export function Validation_validateFactorValue(factorValueString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/factor_value_schema.json", factorValueString);
}

export function Validation_validateInvestigation(investigationString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/investigation_schema.json", investigationString);
}

export function Validation_validateMaterialAttribute(materialAttributeString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/material_attribute_schema.json", materialAttributeString);
}

export function Validation_validateMaterialAttributeValue(materialAttributeValueString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/material_attribute_value_schema.json", materialAttributeValueString);
}

export function Validation_validateMaterial(materialString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/material_schema.json", materialString);
}

export function Validation_validateOntologyAnnotation(ontologyAnnotationString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/ontology_annotation_schema.json", ontologyAnnotationString);
}

export function Validation_validateOntologySourceReference(ontologySourceReferenceString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/ontology_source_reference_schema.json", ontologySourceReferenceString);
}

export function Validation_validatePerson(personString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/person_schema.json", personString);
}

export function Validation_validateProcessParameterValue(processParameterValueString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/process_parameter_value_schema.json", processParameterValueString);
}

export function Validation_validateProcess(processString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/process_schema.json", processString);
}

export function Validation_validateProtocolParameter(protocolParameterString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/protocol_parameter_schema.json", protocolParameterString);
}

export function Validation_validateProtocol(protocolString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/protocol_schema.json", protocolString);
}

export function Validation_validatePublication(publicationString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/publication_schema.json", publicationString);
}

export function Validation_validateSample(sampleString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/sample_schema.json", sampleString);
}

export function Validation_validateSource(sourceString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/source_schema.json", sourceString);
}

export function Validation_validateStudy(studyString: string): any {
    return Validation_validate("https://raw.githubusercontent.com/HLWeil/isa-specs/anyof/source/_static/isajson/study_schema.json", studyString);
}

