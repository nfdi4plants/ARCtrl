namespace ARCtrl.CWL

open DynamicObj
open CWLTypes

module Requirements =

    type DockerRequirement = {
        DockerPull: string option
        DockerFile: Map<string,string> option
        DockerImageId: string option
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
            DynObj.setOptionalProperty (nameof coresMin) coresMin this
            DynObj.setOptionalProperty (nameof coresMax) coresMax this
            DynObj.setOptionalProperty (nameof ramMin) ramMin this
            DynObj.setOptionalProperty (nameof ramMax) ramMax this
            DynObj.setOptionalProperty (nameof tmpdirMin) tmpdirMin this
            DynObj.setOptionalProperty (nameof tmpdirMax) tmpdirMax this
            DynObj.setOptionalProperty (nameof outdirMin) outdirMin this
            DynObj.setOptionalProperty (nameof outdirMax) outdirMax this

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
        | SubworkflowFeatureRequirement
        | ScatterFeatureRequirement
        | MultipleInputFeatureRequirement
        | StepInputExpressionRequirement

