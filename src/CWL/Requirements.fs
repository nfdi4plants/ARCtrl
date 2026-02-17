namespace ARCtrl.CWL

open DynamicObj
open YAMLicious.YAMLiciousTypes

type DockerRequirement = {
        DockerPull: string option
        DockerFile: SchemaSaladString option
        DockerImageId: string option
        DockerLoad: string option
        DockerImport: string option
        DockerOutputDirectory: string option
    }
    with 
    /// Create a DockerRequirement from a plain docker file path or explicit schema-salad reference.
    /// If both `dockerFileReference` and `dockerFile` are provided, `dockerFileReference` takes precedence.
    static member create(?dockerPull, ?dockerFile: string, ?dockerFileReference: SchemaSaladString, ?dockerImageId, ?dockerLoad, ?dockerImport, ?dockerOutputDirectory) =
        let resolvedDockerFile =
            match dockerFileReference, dockerFile with
            | Some referenceValue, _ -> Some referenceValue
            | None, Some file -> Some (Literal file)
            | None, None -> None

        {
            DockerPull = dockerPull
            DockerFile = resolvedDockerFile
            DockerImageId = dockerImageId
            DockerLoad = dockerLoad
            DockerImport = dockerImport
            DockerOutputDirectory = dockerOutputDirectory
        }

    static member createFromLegacyMap(?dockerPull, ?dockerFileMap: Map<string,string>, ?dockerImageId, ?dockerLoad, ?dockerImport, ?dockerOutputDirectory) =
        // Legacy normalization keeps the first recognized schema-salad directive.
        // If "$include"/"$import" exists, use it; otherwise fall back to the first map value.
        // Any additional legacy keys are dropped during normalization.
        let dockerFile =
            match dockerFileMap with
            | Some fileMap when fileMap.ContainsKey "$include" ->
                Some (Include fileMap["$include"])
            | Some fileMap when fileMap.ContainsKey "$import" ->
                Some (Import fileMap["$import"])
            | Some fileMap ->
                fileMap
                |> Seq.tryHead
                |> Option.map (fun kv -> Literal kv.Value)
            | None -> None

        {
            DockerPull = dockerPull
            DockerFile = dockerFile
            DockerImageId = dockerImageId
            DockerLoad = dockerLoad
            DockerImport = dockerImport
            DockerOutputDirectory = dockerOutputDirectory
        }

/// Define an environment variable that will be set in the runtime environment by the workflow platform when executing the command line tool.
type EnvironmentDef = {
    EnvName: string
    EnvValue: string
}

type LoadListingEnum =
    | NoListing
    | ShallowListing
    | DeepListing

[<RequireQualifiedAccess>]
module LoadListingEnum =

    let toCwlString = function
        | NoListing -> "no_listing"
        | ShallowListing -> "shallow_listing"
        | DeepListing -> "deep_listing"

    let tryParse (value: string) =
        match value with
        | "no_listing" -> Some NoListing
        | "shallow_listing" -> Some ShallowListing
        | "deep_listing" -> Some DeepListing
        | _ -> None

type LoadListingRequirementValue = {
    LoadListing: LoadListingEnum
}

type WorkReuseRequirementValue = {
    EnableReuse: bool
}

type NetworkAccessRequirementValue = {
    NetworkAccess: bool
}

type InplaceUpdateRequirementValue = {
    InplaceUpdate: bool
}

type ToolTimeLimitValue =
    | ToolTimeLimitSeconds of int64
    | ToolTimeLimitExpression of string

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

/// Entry in InitialWorkDirRequirement listing.
/// CWL allows either a Dirent object or a string/expression entry.
type InitialWorkDirEntry =
    | DirentEntry of DirentInstance
    | StringEntry of SchemaSaladString
    | FileEntry of FileInstance
    | DirectoryEntry of DirectoryInstance

type InlineJavascriptRequirementValue = {
    ExpressionLib: ResizeArray<string> option
}

type HintUnknownValue = {
    Class: string option
    Raw: YAMLElement
}

type Requirement =
    /// Indicates that the workflow platform must support inline Javascript expressions.
    | InlineJavascriptRequirement of InlineJavascriptRequirementValue
    /// This field consists of an array of type definitions which must be used when interpreting the inputs and outputs fields.
    | SchemaDefRequirement of ResizeArray<SchemaDefRequirementType>
    /// Indicates that a workflow component should be run in a Docker or Docker-compatible (such as Singularity and udocker) container environment and specifies how to fetch or build the image.
    | DockerRequirement of DockerRequirement
    /// A list of software packages that should be configured in the environment of the defined process.
    | SoftwareRequirement of ResizeArray<SoftwarePackage>
    /// Configure how directory listings are loaded for File/Directory inputs.
    | LoadListingRequirement of LoadListingRequirementValue
    /// Define a list of files and subdirectories that must be created by the workflow platform in the designated output directory prior to executing the command line tool.
    /// CWL supports string/expression entries and Dirent objects.
    | InitialWorkDirRequirement of ResizeArray<InitialWorkDirEntry>
    /// Define a list of environment variables which will be set in the execution environment of the tool. See EnvironmentDef for details.
    | EnvVarRequirement of ResizeArray<EnvironmentDef>
    /// Modify the behavior of CommandLineTool to generate a single string containing a shell command line.
    | ShellCommandRequirement
    /// Specify basic hardware resource requirements.
    | ResourceRequirement of ResourceRequirementInstance
    /// For implementations that support reusing output from past work (on the assumption that same code and same input produce same results), control whether to enable or disable the reuse behavior for a particular tool or step.
    | WorkReuseRequirement of WorkReuseRequirementValue
    /// Indicate whether a process requires outgoing IPv4/IPv6 network access. Choice of IPv4 or IPv6 is implementation and site specific, correct tools must support both.
    | NetworkAccessRequirement of NetworkAccessRequirementValue
    /// If inplaceUpdate is true, then an implementation supporting this feature may permit tools to directly update files with writable: true in InitialWorkDirRequirement. 
    | InplaceUpdateRequirement of InplaceUpdateRequirementValue
    /// Set an upper limit on the execution time of a CommandLineTool.
    | ToolTimeLimitRequirement of ToolTimeLimitValue
    /// Indicates that the workflow platform must support nested workflows in the run field of WorkflowStep.
    | SubworkflowFeatureRequirement
    /// Indicates that the workflow platform must support the scatter and scatterMethod fields of WorkflowStep.
    | ScatterFeatureRequirement
    /// Indicates that the workflow platform must support multiple inbound data links listed in the source field of WorkflowStepInput.
    | MultipleInputFeatureRequirement
    /// Indicate that the workflow platform must support the valueFrom field of WorkflowStepInput.
    | StepInputExpressionRequirement

type HintEntry =
    | KnownHint of Requirement
    | UnknownHint of HintUnknownValue

[<RequireQualifiedAccess>]
module HintEntry =

    /// Wraps a known requirement as a known hint entry.
    let ofRequirement (requirement: Requirement) =
        KnownHint requirement

    /// Wraps all requirements as known hint entries.
    let ofRequirements (requirements: ResizeArray<Requirement>) =
        requirements
        |> Seq.map KnownHint
        |> ResizeArray

    /// Returns the underlying requirement for KnownHint values.
    let tryAsRequirement = function
        | KnownHint requirement -> Some requirement
        | UnknownHint _ -> None

module RequirementDefaults =

    let inlineJavascriptRequirement = InlineJavascriptRequirement { ExpressionLib = None }

    let loadListingNoListing = LoadListingRequirement { LoadListing = NoListing }

    let workReuseEnabled = WorkReuseRequirement { EnableReuse = true }

    let networkAccessEnabled = NetworkAccessRequirement { NetworkAccess = true }

    let inplaceUpdateEnabled = InplaceUpdateRequirement { InplaceUpdate = true }

    let toolTimeLimitSeconds seconds = ToolTimeLimitRequirement (ToolTimeLimitSeconds seconds)

