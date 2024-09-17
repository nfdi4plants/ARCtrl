namespace ARCtrl.CWL

open DynamicObj
open CWLTypes

module Requirements =
    type DockerRequirement = {
        DockerPull: string option
        DockerFile: Map<string,string> option
        DockerImageId: string option
    }

    type InputRecordSchema () =
        inherit DynamicObj ()

    type InputEnumSchema () =
        inherit DynamicObj ()

    type InputArraySchema () =
        inherit DynamicObj ()

    type SchemaDefRequirementType =
        | InputRecordSchema of InputRecordSchema
        | InputEnumSchema of InputEnumSchema
        | InputArraySchema of InputArraySchema

    type SoftwarePackage = {
        Package: string
        Version: string [] option
        Specs: string [] option
    }

    type EnvironmentDef = {
        EnvName: string
        EnvValue: string
    }

    type ResourceRequirementInstance () =
        inherit DynamicObj ()

    type Requirement = 
        | InlineJavascriptRequirement
        | SchemaDefRequirement of SchemaDefRequirementType []
        | DockerRequirement of DockerRequirement
        | SoftwareRequirement of SoftwarePackage []
        | InitialWorkDirRequirement of CWLType []
        | EnvVarRequirement of EnvironmentDef []
        | ShellCommandRequirement
        | ResourceRequirement of ResourceRequirementInstance
        | NetworkAccessRequirement

