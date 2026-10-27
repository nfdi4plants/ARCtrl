namespace ARCtrl.CWL

open DynamicObj
open Fable.Core

[<AttachMembers>]
type DockerRequirement (
    ?dockerPull: SchemaSaladString,
    ?dockerFile: SchemaSaladString,
    ?dockerImageId: SchemaSaladString,
    ?dockerLoad: SchemaSaladString,
    ?dockerImport: SchemaSaladString,
    ?dockerOutputDirectory: SchemaSaladString,
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
            HashHelpers.optionSeqEqual this.DockerRunOptions other.DockerRunOptions &&
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        HashHelpers.hashFields [
            box this.DockerPull
            box this.DockerFile
            box this.DockerImageId
            box this.DockerLoad
            box this.DockerImport
            box this.DockerOutputDirectory
            box (HashHelpers.hashOptionSeq this.DockerRunOptions)
            box (DynamicObjHelpers.hashDynamicProperties this)
        ]

    /// Create a DockerRequirement from a plain docker file path or explicit schema-salad reference.
    /// If both `dockerFileReference` and `dockerFile` are provided, `dockerFileReference` takes precedence.
    static member create(?dockerPull: SchemaSaladString, ?dockerFile: string, ?dockerFileReference: SchemaSaladString, ?dockerImageId: SchemaSaladString, ?dockerLoad: SchemaSaladString, ?dockerImport: SchemaSaladString, ?dockerOutputDirectory: SchemaSaladString, ?dockerRunOptions: ResizeArray<string>) =
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
        CWLKnownFieldNames.dockerRequirement

/// Define an environment variable that will be set in the runtime environment by the workflow platform when executing the command line tool.
[<AttachMembers>]
type EnvironmentDef (envName: SchemaSaladString, envValue: SchemaSaladString) =
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
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.EnvName, this.EnvValue, DynamicObjHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        CWLKnownFieldNames.environmentDef

/// How much of a directory a port lists. A directive stands for the name of the symbol, and the decoder reads no file, so
/// it stays as written. An $import and a $mixin take the data of a file, and an $include takes its text, which is the
/// name of the symbol.
type LoadListingEnum =
    | NoListing
    | ShallowListing
    | DeepListing
    | ImportedLoadListing of string
    | IncludedLoadListing of string
    /// A mapping with a $mixin directive, alone or beside other fields, kept as it was written.
    | MixedLoadListing of RawYaml

    /// The name of the symbol as a document spells it, and for a directive the text with the directive in front of the
    /// path. The text of a directive serves for display, because the encoder writes the directive as a mapping.
    static member toCwlString = function
        | NoListing -> "no_listing"
        | ShallowListing -> "shallow_listing"
        | DeepListing -> "deep_listing"
        | ImportedLoadListing path -> (SchemaSaladString.Import path).AsDirectiveString
        | IncludedLoadListing path -> (SchemaSaladString.Include path).AsDirectiveString
        | MixedLoadListing raw -> (SchemaSaladString.Mixed raw).AsDirectiveString

    static member tryParse (value: string) =
        match value with
        | "no_listing" -> Some NoListing
        | "shallow_listing" -> Some ShallowListing
        | "deep_listing" -> Some DeepListing
        | _ -> None

[<AttachMembers>]
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
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.LoadListing, DynamicObjHelpers.hashDynamicProperties this)

    static member defaultNoListing =
        LoadListingRequirementValue(NoListing)

    static member KnownFieldNames =
        CWLKnownFieldNames.loadListingRequirementValue

[<AttachMembers>]
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
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.EnableReuse, DynamicObjHelpers.hashDynamicProperties this)

    static member defaultEnabled =
        WorkReuseRequirementValue(true)

    static member KnownFieldNames =
        CWLKnownFieldNames.workReuseRequirementValue

[<AttachMembers>]
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
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.NetworkAccess, DynamicObjHelpers.hashDynamicProperties this)

    static member defaultEnabled =
        NetworkAccessRequirementValue(true)

    static member KnownFieldNames =
        CWLKnownFieldNames.networkAccessRequirementValue

[<AttachMembers>]
type InplaceUpdateRequirementValue (inplaceUpdate: SchemaSaladBool) =
    inherit DynamicObj ()

    let mutable _inplaceUpdate = inplaceUpdate

    member this.InplaceUpdate
        with get() = _inplaceUpdate
        and set(value) = _inplaceUpdate <- value

    override this.Equals(o: obj) =
        match o with
        | :? InplaceUpdateRequirementValue as other ->
            this.InplaceUpdate = other.InplaceUpdate &&
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.InplaceUpdate, DynamicObjHelpers.hashDynamicProperties this)

    static member defaultEnabled =
        InplaceUpdateRequirementValue(SchemaSaladBool.Value true)

    static member KnownFieldNames =
        CWLKnownFieldNames.inplaceUpdateRequirementValue

type ToolTimeLimitValue =
    | ToolTimeLimitSeconds of int64
    | ToolTimeLimitExpression of SchemaSaladString

/// "min" is the minimum amount of a resource that must be reserved to schedule a job. If "min" cannot be satisfied, the job should not be run.
/// "max" is the maximum amount of a resource that the job shall be permitted to use. If a node has sufficient resources, multiple jobs may be scheduled on a single node provided each job's "max" resource requirements are met.
/// If a job attempts to exceed its "max" resource allocation, an implementation may deny additional resources, which may result in job failure.
/// If "min" is specified but "max" is not, then "max" == "min" If "max" is specified by "min" is not, then "min" == "max".
/// It is an error if max < min.
/// It is an error if the value of any of these fields is negative. cwltool accepts a negative value, so the decoder reads
/// it and warns.
/// If neither "min" nor "max" is specified for a resource, default values are used.
[<AttachMembers>]
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

    /// The value as an int64, and none for a value that holds no integer. A float that holds a whole number, such as 4.0,
    /// is an integer value on every target, because JavaScript has one number type for both.
    member this.TryGetInt64(name: string) =
        this.TryGetKnownField(name)
        |> Option.bind (function
            | :? int64 as value -> Some value
            | :? int as value -> IntegerOf.tryNumber (float value)
            | :? float as value -> IntegerOf.tryNumber value
            | _ -> None)

    member this.TryGetFloat(name: string) =
        this.TryGetKnownField(name)
        |> Option.bind (function
            | :? float as value -> Some value
            | _ -> None)

    member this.TryGetExpression(name: string) =
        this.TryGetKnownField(name)
        |> Option.bind (function
            | :? SchemaSaladString as value -> Some value
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
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.KnownFieldValues, DynamicObjHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        CWLKnownFieldNames.resourceRequirementInstance

/// An entry of the types field of a SchemaDefRequirement: a type that the document defines, or a file whose types the
/// document imports. A defined type is the record or enum schema itself, with its own optional name and extension
/// fields. A type field names an imported type as path#Name, and the path counts with its . and .. segments
/// resolved. The name inside the file stays unchecked, because the decoder reads no file. A types field that is an
/// $import mapping as a whole holds one Imported entry, because Schema Salad replaces that mapping with the list of
/// the file. The model does not tell the two spellings apart, and the encoder writes the list.
type SchemaDefEntry =
    | Defined of SchemaDefinition
    | Imported of string
    /// A mapping with a $mixin directive, kept as it was written. The file may define the types of the entry, and the
    /// decoder reads no file.
    | Mixed of RawYaml
    /// A schema whose type is a directive, as in {type: {$import: path}, name: Shape}, kept as it was written. The file
    /// decides which kind of schema it is, and the decoder reads no file.
    | Raw of RawYaml

/// A File or a Directory, the only items that an array inside the listing of an InitialWorkDirRequirement may hold, or
/// the directive that stands for one. The decoder reads no file, so a directive stays an item of its own, in its place.
type FileOrDirectory =
    | FileItem of FileInstance
    | DirectoryItem of DirectoryInstance
    /// A file that the array imports, as {$import: path}. The file holds one File or Directory or a list of them, and a
    /// list takes the place of the item.
    | ImportedItem of string
    /// A mapping with a $mixin directive, alone or beside the fields that complete the File or the Directory of the file.
    /// The mapping is kept as it was written.
    | MixedItem of RawYaml
    /// A File or a Directory whose class is a directive, as in {class: {$import: path}, location: a.txt}, kept as it was
    /// written. The file decides which of the two it is, and the decoder reads no file.
    | RawItem of RawYaml

/// Entry in InitialWorkDirRequirement listing.
/// CWL allows null, a Dirent object, a string/expression entry, a File, a Directory and an array of Files and
/// Directories. The lists of the array compare by their elements.
[<CustomEquality; NoComparison>]
type InitialWorkDirEntry =
    | DirentEntry of DirentInstance
    | StringEntry of SchemaSaladString
    | FileEntry of FileInstance
    | DirectoryEntry of DirectoryInstance
    /// An item without a value, which the runner skips.
    | NullEntry
    /// An array of Files and Directories as one item.
    | FilesEntry of ResizeArray<FileOrDirectory>
    /// A File or a Directory whose class is a directive, kept as it was written. The file decides which of the two it is, and
    /// the decoder reads no file.
    | RawEntry of RawYaml

    with
    override this.Equals(o: obj) =
        match o with
        | :? InitialWorkDirEntry as other ->
            match this, other with
            | DirentEntry left, DirentEntry right -> left.Equals right
            | StringEntry left, StringEntry right -> left = right
            | FileEntry left, FileEntry right -> left.Equals right
            | DirectoryEntry left, DirectoryEntry right -> left.Equals right
            | NullEntry, NullEntry -> true
            | FilesEntry left, FilesEntry right -> HashHelpers.seqEqual left right
            | RawEntry left, RawEntry right -> left.Equals right
            | _ -> false
        | _ -> false

    override this.GetHashCode() =
        match this with
        | DirentEntry value -> HashHelpers.mergeHashes 1 (hash value)
        | StringEntry value -> HashHelpers.mergeHashes 2 (hash value)
        | FileEntry value -> HashHelpers.mergeHashes 3 (hash value)
        | DirectoryEntry value -> HashHelpers.mergeHashes 4 (hash value)
        | NullEntry -> 5
        | FilesEntry values -> HashHelpers.mergeHashes 6 (HashHelpers.hashSeq values)
        | RawEntry value -> HashHelpers.mergeHashes 7 (hash value)

[<AttachMembers>]
type InlineJavascriptRequirementValue (?expressionLib: ResizeArray<SchemaSaladString>) =
    inherit DynamicObj ()

    let mutable _expressionLib = expressionLib

    member this.ExpressionLib
        with get() = _expressionLib
        and set(value) = _expressionLib <- value

    override this.Equals(o: obj) =
        match o with
        | :? InlineJavascriptRequirementValue as other ->
            HashHelpers.optionSeqEqual this.ExpressionLib other.ExpressionLib &&
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (HashHelpers.hashOptionSeq this.ExpressionLib, DynamicObjHelpers.hashDynamicProperties this)

    static member defaultEmpty =
        InlineJavascriptRequirementValue()

    static member KnownFieldNames =
        CWLKnownFieldNames.inlineJavascriptRequirementValue

[<AttachMembers>]
type HintUnknownValue (class_: string option, raw: RawYaml) =
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
            DynamicObjHelpers.dynamicPropertiesEqual this other
        | _ -> false

    override this.GetHashCode() =
        hash (this.Class, this.Raw, DynamicObjHelpers.hashDynamicProperties this)

    static member KnownFieldNames =
        CWLKnownFieldNames.hintUnknownValue

/// The equality of a requirement compares the lists of its payload by their elements, and the objects of the payload by
/// their own equality.
[<CustomEquality; NoComparison>]
type Requirement =
    /// Indicates that the workflow platform must support inline Javascript expressions.
    | InlineJavascriptRequirement of InlineJavascriptRequirementValue
    /// This field consists of an array of type definitions which must be used when interpreting the inputs and outputs fields.
    | SchemaDefRequirement of ResizeArray<SchemaDefEntry>
    /// Indicates that a workflow component should be run in a Docker or Docker-compatible (such as Singularity and udocker) container environment and specifies how to fetch or build the image.
    | DockerRequirement of DockerRequirement
    /// A list of software packages that should be configured in the environment of the defined process. An entry is a
    /// package, or the import of a file that holds a package or a list of packages. The decoder reads no file, so the
    /// import stays an entry of its own, in its place. A document that writes the whole field as one $import gets the list
    /// with that one import, and the encoder writes it as a list.
    | SoftwareRequirement of ResizeArray<ListEntry<SoftwarePackage>>
    /// Configure how directory listings are loaded for File/Directory inputs.
    | LoadListingRequirement of LoadListingRequirementValue
    /// Define a list of files and subdirectories that must be created by the workflow platform in the designated output directory prior to executing the command line tool.
    /// CWL supports string/expression entries and Dirent objects.
    | InitialWorkDirRequirement of ResizeArray<InitialWorkDirEntry>
    /// Expression form of the InitialWorkDirRequirement listing, as in listing: $(inputs.directory.listing). A directive
    /// names a file with the expression.
    | InitialWorkDirExpressionRequirement of SchemaSaladString
    /// Define a list of environment variables which will be set in the execution environment of the tool. See EnvironmentDef for details.
    /// An entry is a definition, or the import of a file that holds a definition or a list of definitions. The decoder
    /// reads no file, so the import stays an entry of its own, in its place. A document that writes the whole field as one
    /// $import gets the list with that one import, and the encoder writes it as a list.
    | EnvVarRequirement of ResizeArray<ListEntry<EnvironmentDef>>
    /// Modify the behavior of CommandLineTool to generate a single string containing a shell command line.
    | ShellCommandRequirement
    /// Specify basic hardware resource requirements.
    | ResourceRequirement of ResourceRequirementInstance
    /// For implementations that support reusing output from past work (on the assumption that same code and same input produce same results), control whether to enable or disable the reuse behavior for a particular tool or step.
    | WorkReuseRequirement of WorkReuseRequirementValue
    /// Expression payload form of WorkReuse enableReuse.
    | WorkReuseExpressionRequirement of SchemaSaladString
    /// Indicate whether a process requires outgoing IPv4/IPv6 network access. Choice of IPv4 or IPv6 is implementation and site specific, correct tools must support both.
    | NetworkAccessRequirement of NetworkAccessRequirementValue
    /// Expression payload form of NetworkAccess networkAccess.
    | NetworkAccessExpressionRequirement of SchemaSaladString
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
    /// A file of requirements that the document imports, as {$import: path}. The decoder reads no file, so the entry
    /// stands for the requirements that the file holds. A list in the file adds its requirements to the list.
    | ImportedRequirement of string
    /// A mapping with a $mixin directive, as in {$mixin: path, class: NetworkAccess}. The file may supply the fields of the
    /// requirement, so the mapping is kept as it was written.
    | MixedRequirement of RawYaml
    /// A requirement whose class is a directive, as in {class: {$import: path}}, kept as it was written. The file decides
    /// the class, and the decoder reads no file.
    | RawRequirement of RawYaml

    with
    override this.Equals(o: obj) =
        match o with
        | :? Requirement as other ->
            match this, other with
            | InlineJavascriptRequirement left, InlineJavascriptRequirement right -> left.Equals right
            | SchemaDefRequirement left, SchemaDefRequirement right -> HashHelpers.seqEqual left right
            | DockerRequirement left, DockerRequirement right -> left.Equals right
            | SoftwareRequirement left, SoftwareRequirement right -> HashHelpers.seqEqual left right
            | LoadListingRequirement left, LoadListingRequirement right -> left.Equals right
            | InitialWorkDirRequirement left, InitialWorkDirRequirement right -> HashHelpers.seqEqual left right
            | InitialWorkDirExpressionRequirement left, InitialWorkDirExpressionRequirement right -> left = right
            | EnvVarRequirement left, EnvVarRequirement right -> HashHelpers.seqEqual left right
            | ShellCommandRequirement, ShellCommandRequirement -> true
            | ResourceRequirement left, ResourceRequirement right -> left.Equals right
            | WorkReuseRequirement left, WorkReuseRequirement right -> left.Equals right
            | WorkReuseExpressionRequirement left, WorkReuseExpressionRequirement right -> left = right
            | NetworkAccessRequirement left, NetworkAccessRequirement right -> left.Equals right
            | NetworkAccessExpressionRequirement left, NetworkAccessExpressionRequirement right -> left = right
            | InplaceUpdateRequirement left, InplaceUpdateRequirement right -> left.Equals right
            | ToolTimeLimitRequirement left, ToolTimeLimitRequirement right -> left = right
            | SubworkflowFeatureRequirement, SubworkflowFeatureRequirement -> true
            | ScatterFeatureRequirement, ScatterFeatureRequirement -> true
            | MultipleInputFeatureRequirement, MultipleInputFeatureRequirement -> true
            | StepInputExpressionRequirement, StepInputExpressionRequirement -> true
            | ImportedRequirement left, ImportedRequirement right -> left = right
            | MixedRequirement left, MixedRequirement right -> left.Equals right
            | RawRequirement left, RawRequirement right -> left.Equals right
            | _ -> false
        | _ -> false

    override this.GetHashCode() =
        match this with
        | InlineJavascriptRequirement value -> HashHelpers.mergeHashes 1 (hash value)
        | SchemaDefRequirement values -> HashHelpers.mergeHashes 2 (HashHelpers.hashSeq values)
        | DockerRequirement value -> HashHelpers.mergeHashes 3 (hash value)
        | SoftwareRequirement values -> HashHelpers.mergeHashes 4 (HashHelpers.hashSeq values)
        | LoadListingRequirement value -> HashHelpers.mergeHashes 5 (hash value)
        | InitialWorkDirRequirement values -> HashHelpers.mergeHashes 6 (HashHelpers.hashSeq values)
        | EnvVarRequirement values -> HashHelpers.mergeHashes 7 (HashHelpers.hashSeq values)
        | ShellCommandRequirement -> 8
        | ResourceRequirement value -> HashHelpers.mergeHashes 9 (hash value)
        | WorkReuseRequirement value -> HashHelpers.mergeHashes 10 (hash value)
        | WorkReuseExpressionRequirement value -> HashHelpers.mergeHashes 11 (hash value)
        | NetworkAccessRequirement value -> HashHelpers.mergeHashes 12 (hash value)
        | NetworkAccessExpressionRequirement value -> HashHelpers.mergeHashes 13 (hash value)
        | InplaceUpdateRequirement value -> HashHelpers.mergeHashes 14 (hash value)
        | ToolTimeLimitRequirement value -> HashHelpers.mergeHashes 15 (hash value)
        | SubworkflowFeatureRequirement -> 16
        | ScatterFeatureRequirement -> 17
        | MultipleInputFeatureRequirement -> 18
        | StepInputExpressionRequirement -> 19
        | InitialWorkDirExpressionRequirement value -> HashHelpers.mergeHashes 20 (hash value)
        | ImportedRequirement value -> HashHelpers.mergeHashes 21 (hash value)
        | MixedRequirement value -> HashHelpers.mergeHashes 22 (hash value)
        | RawRequirement value -> HashHelpers.mergeHashes 23 (hash value)

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
    /// A file of hints that the document imports, as {$import: path}. The entry stands for the hints that the file holds,
    /// and the decoder cannot tell whether they are known.
    | ImportedHint of string
    /// A mapping with a $mixin directive, kept as it was written. The file may supply the class and the fields.
    | MixedHint of RawYaml
    /// A hint whose class is a directive, kept as it was written. The file decides the class, and the decoder reads no file.
    | RawHint of RawYaml

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
        | UnknownHint _
        | ImportedHint _
        | MixedHint _
        | RawHint _ -> None

