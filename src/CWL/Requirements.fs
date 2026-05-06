namespace ARCtrl.CWL

open DynamicObj
open YAMLicious.YAMLiciousTypes

type DockerRequirement (
    ?dockerPull: string,
    ?dockerFile: SchemaSaladString,
    ?dockerImageId: string,
    ?dockerLoad: string,
    ?dockerImport: string,
    ?dockerOutputDirectory: string,
    ?dockerRunOptions: ResizeArray<string>
) =
    inherit DynamicObj ()

    let mutable _dockerPull = dockerPull
    let mutable _dockerFile = dockerFile
    let mutable _dockerImageId = dockerImageId
    let mutable _dockerLoad = dockerLoad
    let mutable _dockerImport = dockerImport
    let mutable _dockerOutputDirectory = dockerOutputDirectory
    let mutable _dockerRunOptions = dockerRunOptions

    member this.DockerPull
        with get() = _dockerPull
        and set(value) = _dockerPull <- value

    member this.DockerFile
        with get() = _dockerFile
        and set(value) = _dockerFile <- value

    member this.DockerImageId
        with get() = _dockerImageId
        and set(value) = _dockerImageId <- value

    member this.DockerLoad
        with get() = _dockerLoad
        and set(value) = _dockerLoad <- value

    member this.DockerImport
        with get() = _dockerImport
        and set(value) = _dockerImport <- value

    member this.DockerOutputDirectory
        with get() = _dockerOutputDirectory
        and set(value) = _dockerOutputDirectory <- value

    member this.DockerRunOptions
        with get() = _dockerRunOptions
        and set(value) = _dockerRunOptions <- value

    override this.Equals(o: obj) =
        match o with
        | :? DockerRequirement as other ->
            this.DockerPull = other.DockerPull &&
            this.DockerFile = other.DockerFile &&
            this.DockerImageId = other.DockerImageId &&
            this.DockerLoad = other.DockerLoad &&
            this.DockerImport = other.DockerImport &&
            this.DockerOutputDirectory = other.DockerOutputDirectory &&
            this.DockerRunOptions = other.DockerRunOptions &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (
            this.DockerPull,
            this.DockerFile,
            this.DockerImageId,
            this.DockerLoad,
            this.DockerImport,
            this.DockerOutputDirectory,
            this.DockerRunOptions,
            HashHelpers.hashDynamicProperties this
        )

    /// Create a DockerRequirement from a plain docker file path or explicit schema-salad reference.
    /// If both `dockerFileReference` and `dockerFile` are provided, `dockerFileReference` takes precedence.
    static member create(?dockerPull, ?dockerFile: string, ?dockerFileReference: SchemaSaladString, ?dockerImageId, ?dockerLoad, ?dockerImport, ?dockerOutputDirectory, ?dockerRunOptions: ResizeArray<string>) =
        let resolvedDockerFile =
            match dockerFileReference, dockerFile with
            | Some referenceValue, _ -> Some referenceValue
            | None, Some file -> Some (SchemaSaladString.Literal file)
            | None, None -> None

        DockerRequirement(
            ?dockerPull = dockerPull,
            ?dockerFile = resolvedDockerFile,
            ?dockerImageId = dockerImageId,
            ?dockerLoad = dockerLoad,
            ?dockerImport = dockerImport,
            ?dockerOutputDirectory = dockerOutputDirectory,
            ?dockerRunOptions = dockerRunOptions
        )

    static member KnownFieldNames =
        ResizeArray [| "class"; "dockerPull"; "dockerFile"; "dockerImageId"; "dockerLoad"; "dockerImport"; "dockerOutputDirectory"; "cwltool:dockerRunOptions" |]

/// Define an environment variable that will be set in the runtime environment by the workflow platform when executing the command line tool.
type EnvironmentDef (envName: string, envValue: string) =
    inherit DynamicObj ()

    let mutable _envName = envName
    let mutable _envValue = envValue

    member this.EnvName
        with get() = _envName
        and set(value) = _envName <- value

    member this.EnvValue
        with get() = _envValue
        and set(value) = _envValue <- value

    override this.Equals(o: obj) =
        match o with
        | :? EnvironmentDef as other ->
            this.EnvName = other.EnvName &&
            this.EnvValue = other.EnvValue &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.EnvName, this.EnvValue, HashHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        ResizeArray [| "envName"; "envValue" |]

type LoadListingEnum =
    | NoListing
    | ShallowListing
    | DeepListing

    static member toCwlString = function
        | NoListing -> "no_listing"
        | ShallowListing -> "shallow_listing"
        | DeepListing -> "deep_listing"

    static member tryParse (value: string) =
        match value with
        | "no_listing" -> Some NoListing
        | "shallow_listing" -> Some ShallowListing
        | "deep_listing" -> Some DeepListing
        | _ -> None

type LoadListingRequirementValue (loadListing: LoadListingEnum) =
    inherit DynamicObj ()

    let mutable _loadListing = loadListing

    member this.LoadListing
        with get() = _loadListing
        and set(value) = _loadListing <- value

    override this.Equals(o: obj) =
        match o with
        | :? LoadListingRequirementValue as other ->
            this.LoadListing = other.LoadListing &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.LoadListing, HashHelpers.hashDynamicProperties this)

    static member defaultNoListing =
        LoadListingRequirementValue(NoListing)

    static member KnownFieldNames =
        ResizeArray [| "class"; "loadListing" |]

type WorkReuseRequirementValue (enableReuse: bool) =
    inherit DynamicObj ()

    let mutable _enableReuse = enableReuse

    member this.EnableReuse
        with get() = _enableReuse
        and set(value) = _enableReuse <- value

    override this.Equals(o: obj) =
        match o with
        | :? WorkReuseRequirementValue as other ->
            this.EnableReuse = other.EnableReuse &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.EnableReuse, HashHelpers.hashDynamicProperties this)

    static member defaultEnabled =
        WorkReuseRequirementValue(true)

    static member KnownFieldNames =
        ResizeArray [| "class"; "enableReuse" |]

type NetworkAccessRequirementValue (networkAccess: bool) =
    inherit DynamicObj ()

    let mutable _networkAccess = networkAccess

    member this.NetworkAccess
        with get() = _networkAccess
        and set(value) = _networkAccess <- value

    override this.Equals(o: obj) =
        match o with
        | :? NetworkAccessRequirementValue as other ->
            this.NetworkAccess = other.NetworkAccess &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.NetworkAccess, HashHelpers.hashDynamicProperties this)

    static member defaultEnabled =
        NetworkAccessRequirementValue(true)

    static member KnownFieldNames =
        ResizeArray [| "class"; "networkAccess" |]

type InplaceUpdateRequirementValue (inplaceUpdate: bool) =
    inherit DynamicObj ()

    let mutable _inplaceUpdate = inplaceUpdate

    member this.InplaceUpdate
        with get() = _inplaceUpdate
        and set(value) = _inplaceUpdate <- value

    override this.Equals(o: obj) =
        match o with
        | :? InplaceUpdateRequirementValue as other ->
            this.InplaceUpdate = other.InplaceUpdate &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.InplaceUpdate, HashHelpers.hashDynamicProperties this)

    static member defaultEnabled =
        InplaceUpdateRequirementValue(true)

    static member KnownFieldNames =
        ResizeArray [| "class"; "inplaceUpdate" |]

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
    ?coresMin: obj,
    ?coresMax: obj,
    ?ramMin: obj,
    ?ramMax: obj,
    ?tmpdirMin: obj,
    ?tmpdirMax: obj,
    ?outdirMin: obj,
    ?outdirMax: obj
) =
    inherit DynamicObj ()

    let mutable _coresMin = coresMin
    let mutable _coresMax = coresMax
    let mutable _ramMin = ramMin
    let mutable _ramMax = ramMax
    let mutable _tmpdirMin = tmpdirMin
    let mutable _tmpdirMax = tmpdirMax
    let mutable _outdirMin = outdirMin
    let mutable _outdirMax = outdirMax

    member this.CoresMin
        with get() = _coresMin
        and set(value) = _coresMin <- value

    member this.CoresMax
        with get() = _coresMax
        and set(value) = _coresMax <- value

    member this.RamMin
        with get() = _ramMin
        and set(value) = _ramMin <- value

    member this.RamMax
        with get() = _ramMax
        and set(value) = _ramMax <- value

    member this.TmpdirMin
        with get() = _tmpdirMin
        and set(value) = _tmpdirMin <- value

    member this.TmpdirMax
        with get() = _tmpdirMax
        and set(value) = _tmpdirMax <- value

    member this.OutdirMin
        with get() = _outdirMin
        and set(value) = _outdirMin <- value

    member this.OutdirMax
        with get() = _outdirMax
        and set(value) = _outdirMax <- value

    member this.TryGetKnownField(name: string) =
        match name with
        | "coresMin" -> this.CoresMin
        | "coresMax" -> this.CoresMax
        | "ramMin" -> this.RamMin
        | "ramMax" -> this.RamMax
        | "tmpdirMin" -> this.TmpdirMin
        | "tmpdirMax" -> this.TmpdirMax
        | "outdirMin" -> this.OutdirMin
        | "outdirMax" -> this.OutdirMax
        | _ -> None

    member this.TryGetInt64(name: string) =
        this.TryGetKnownField(name)
        |> Option.bind (function
            | :? int64 as value -> Some value
            | :? int as value -> Some (int64 value)
            | _ -> None)

    member this.TryGetFloat(name: string) =
        this.TryGetKnownField(name)
        |> Option.bind (function
            | :? float as value -> Some value
            | _ -> None)

    member this.TryGetExpression(name: string) =
        this.TryGetKnownField(name)
        |> Option.bind (function
            | :? string as value -> Some value
            | _ -> None)

    member this.KnownFieldValues =
        [
            "coresMin", this.CoresMin
            "coresMax", this.CoresMax
            "ramMin", this.RamMin
            "ramMax", this.RamMax
            "tmpdirMin", this.TmpdirMin
            "tmpdirMax", this.TmpdirMax
            "outdirMin", this.OutdirMin
            "outdirMax", this.OutdirMax
        ]

    override this.Equals(o: obj) =
        match o with
        | :? ResourceRequirementInstance as other ->
            this.KnownFieldValues = other.KnownFieldValues &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.KnownFieldValues, HashHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        ResizeArray [| "class"; "coresMin"; "coresMax"; "ramMin"; "ramMax"; "tmpdirMin"; "tmpdirMax"; "outdirMin"; "outdirMax" |]

/// Entry in InitialWorkDirRequirement listing.
/// CWL allows either a Dirent object or a string/expression entry.
type InitialWorkDirEntry =
    | DirentEntry of DirentInstance
    | StringEntry of SchemaSaladString
    | FileEntry of FileInstance
    | DirectoryEntry of DirectoryInstance

type InlineJavascriptRequirementValue (?expressionLib: ResizeArray<string>) =
    inherit DynamicObj ()

    let mutable _expressionLib = expressionLib

    member this.ExpressionLib
        with get() = _expressionLib
        and set(value) = _expressionLib <- value

    override this.Equals(o: obj) =
        match o with
        | :? InlineJavascriptRequirementValue as other ->
            this.ExpressionLib = other.ExpressionLib &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.ExpressionLib, HashHelpers.hashDynamicProperties this)

    static member defaultEmpty =
        InlineJavascriptRequirementValue()

    static member KnownFieldNames =
        ResizeArray [| "class"; "expressionLib" |]

type HintUnknownValue (class_: string option, raw: YAMLElement) =
    inherit DynamicObj ()

    let mutable _class = class_
    let mutable _raw = raw

    member this.Class
        with get() = _class
        and set(value) = _class <- value

    member this.Raw
        with get() = _raw
        and set(value) = _raw <- value

    override this.Equals(o: obj) =
        match o with
        | :? HintUnknownValue as other ->
            this.Class = other.Class &&
            this.Raw = other.Raw &&
            HashHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.Class, this.Raw, HashHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        ResizeArray [| "class"; "raw" |]

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
    /// Expression payload form of WorkReuse enableReuse.
    | WorkReuseExpressionRequirement of string
    /// Indicate whether a process requires outgoing IPv4/IPv6 network access. Choice of IPv4 or IPv6 is implementation and site specific, correct tools must support both.
    | NetworkAccessRequirement of NetworkAccessRequirementValue
    /// Expression payload form of NetworkAccess networkAccess.
    | NetworkAccessExpressionRequirement of string
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

    with
    static member defaultInlineJavascriptRequirement =
        InlineJavascriptRequirement InlineJavascriptRequirementValue.defaultEmpty

    static member defaultLoadListingNoListing =
        LoadListingRequirement LoadListingRequirementValue.defaultNoListing

    static member defaultWorkReuseEnabled =
        WorkReuseRequirement WorkReuseRequirementValue.defaultEnabled

    static member defaultNetworkAccessEnabled =
        NetworkAccessRequirement NetworkAccessRequirementValue.defaultEnabled

    static member defaultInplaceUpdateEnabled =
        InplaceUpdateRequirement InplaceUpdateRequirementValue.defaultEnabled

    static member defaultToolTimeLimitSeconds(seconds: int64) =
        ToolTimeLimitRequirement (ToolTimeLimitSeconds seconds)

type HintEntry =
    | KnownHint of Requirement
    | UnknownHint of HintUnknownValue

    /// Wraps a known requirement as a known hint entry.
    static member ofRequirement (requirement: Requirement) =
        KnownHint requirement

    /// Wraps all requirements as known hint entries.
    static member ofRequirements (requirements: ResizeArray<Requirement>) =
        requirements
        |> Seq.map KnownHint
        |> ResizeArray

    /// Returns the underlying requirement for KnownHint values.
    static member tryAsRequirement = function
        | KnownHint requirement -> Some requirement
        | UnknownHint _ -> None

