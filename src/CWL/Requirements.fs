namespace ARCtrl.CWL

open DynamicObj
open CWLTypes

module Requirements =

    type DockerRequirement = {
        DockerPull: string option
        DockerFile: Map<string,string> option
        DockerImageId: string option
    }

    /// Define an environment variable that will be set in the runtime environment by the workflow platform when executing the command line tool.
    type EnvironmentDef = {
        EnvName: string
        EnvValue: string
    }

    /// "min" is the minimum amount of a resource that must be reserved to schedule a job. If "min" cannot be satisfied, the job should not be run.
    /// "max" is the maximum amount of a resource that the job shall be permitted to use. If a node has sufficient resources, multiple jobs may be scheduled on a single node provided each job's "max" resource requirements are met.
    /// If a job attempts to exceed its "max" resource allocation, an implementation may deny additional resources, which may result in job failure.
    /// If "min" is specified but "max" is not, then "max" == "min" If "max" is specified by "min" is not, then "min" == "max".
    /// It is an error if max < min.
    /// It is an error if the value of any of these fields is negative.
    /// If neither "min" nor "max" is specified for a resource, default values are used.
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
        /// Indicates that the workflow platform must support inline Javascript expressions.
        | InlineJavascriptRequirement
        /// This field consists of an array of type definitions which must be used when interpreting the inputs and outputs fields.
        | SchemaDefRequirement of ResizeArray<SchemaDefRequirementType>
        /// Indicates that a workflow component should be run in a Docker or Docker-compatible (such as Singularity and udocker) container environment and specifies how to fetch or build the image.
        | DockerRequirement of DockerRequirement
        /// A list of software packages that should be configured in the environment of the defined process.
        | SoftwareRequirement of ResizeArray<SoftwarePackage>
        /// Define a list of files and subdirectories that must be created by the workflow platform in the designated output directory prior to executing the command line tool.
        | InitialWorkDirRequirement of ResizeArray<CWLType>
        /// Define a list of environment variables which will be set in the execution environment of the tool. See EnvironmentDef for details.
        | EnvVarRequirement of ResizeArray<EnvironmentDef>
        /// Modify the behavior of CommandLineTool to generate a single string containing a shell command line.
        | ShellCommandRequirement
        /// Specify basic hardware resource requirements.
        | ResourceRequirement of ResourceRequirementInstance
        /// For implementations that support reusing output from past work (on the assumption that same code and same input produce same results), control whether to enable or disable the reuse behavior for a particular tool or step.
        | WorkReuseRequirement
        /// Indicate whether a process requires outgoing IPv4/IPv6 network access. Choice of IPv4 or IPv6 is implementation and site specific, correct tools must support both.
        | NetworkAccessRequirement
        /// If inplaceUpdate is true, then an implementation supporting this feature may permit tools to directly update files with writable: true in InitialWorkDirRequirement. 
        | InplaceUpdateRequirement
        /// Set an upper limit on the execution time of a CommandLineTool.
        | ToolTimeLimitRequirement of float
        /// Indicates that the workflow platform must support nested workflows in the run field of WorkflowStep.
        | SubworkflowFeatureRequirement
        /// Indicates that the workflow platform must support the scatter and scatterMethod fields of WorkflowStep.
        | ScatterFeatureRequirement
        /// Indicates that the workflow platform must support multiple inbound data links listed in the source field of WorkflowStepInput.
        | MultipleInputFeatureRequirement
        /// Indicate that the workflow platform must support the valueFrom field of WorkflowStepInput.
        | StepInputExpressionRequirement

