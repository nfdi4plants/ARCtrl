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

    type SchemaDefRequirementType (types, definitions) as this =
        inherit DynamicObj ()
        do
            DynObj.setValue this (nameof types) definitions

    type SoftwarePackage = {
        Package: string
        Version: string [] option
        Specs: string [] option
    }

    type EnvironmentDef = {
        EnvName: string
        EnvValue: string
    }

    type ResourceRequirementInstance (
        ?coresMin,
        ?coresMax,
        ?ramMin,
        ?ramMax,
        ?tmpdirMin,
        ?tmpdirMax,
        ?outdirMin,
        ?outdirMax
    ) as this =
        inherit DynamicObj ()
        do
            DynObj.setValueOpt this (nameof coresMin) coresMin
            DynObj.setValueOpt this (nameof coresMax) coresMax
            DynObj.setValueOpt this (nameof ramMin) ramMin
            DynObj.setValueOpt this (nameof ramMax) ramMax
            DynObj.setValueOpt this (nameof tmpdirMin) tmpdirMin
            DynObj.setValueOpt this (nameof tmpdirMax) tmpdirMax
            DynObj.setValueOpt this (nameof outdirMin) outdirMin
            DynObj.setValueOpt this (nameof outdirMax) outdirMax

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

