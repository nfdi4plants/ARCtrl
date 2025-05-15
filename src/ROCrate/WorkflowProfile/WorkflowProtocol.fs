namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

[<AttachMembers>]
type WorkflowProtocol(
    id: string,
    conformsTo: ResizeArray<string>,
    ?input: ResizeArray<FormalParameter>,
    ?output: ResizeArray<FormalParameter>,
    ?creator: ResizeArray<Person>,
    ?dateCreated: System.DateTime,
    ?license: obj,
    ?name: obj,
    ?programmingLanguage: ResizeArray<string>,
    ?sdPublisher: ResizeArray<Person>,
    ?url: obj,
    ?version: obj,
    ?description: obj,
    ?hasPart: ResizeArray<obj>,
    ?intendedUse: obj,
    ?comment: ResizeArray<obj>,
    ?computationalTool: ResizeArray<obj>
) as this =
    inherit LDNode(
        id = id,
        schemaType = ResizeArray[|"schema.org/Text"; "schema.org/SoftwareSourceCode"; "bioschemas.org/ComputationalWorkflow"; "bioschemas.org/LabProtocol"|]
    )
    do
        // Required properties
        DynObj.setProperty (nameof conformsTo) conformsTo this
        
        // Recommended properties
        DynObj.setOptionalProperty (nameof input) input this
        DynObj.setOptionalProperty (nameof output) output this
        DynObj.setOptionalProperty (nameof creator) creator this
        DynObj.setOptionalProperty (nameof dateCreated) dateCreated this
        DynObj.setOptionalProperty (nameof license) license this
        DynObj.setOptionalProperty (nameof name) name this
        DynObj.setOptionalProperty (nameof programmingLanguage) programmingLanguage this
        DynObj.setOptionalProperty (nameof sdPublisher) sdPublisher this
        DynObj.setOptionalProperty (nameof url) url this
        DynObj.setOptionalProperty (nameof version) version this

        // Optional properties
        DynObj.setOptionalProperty (nameof description) description this
        DynObj.setOptionalProperty (nameof hasPart) hasPart this
        DynObj.setOptionalProperty (nameof intendedUse) intendedUse this
        DynObj.setOptionalProperty (nameof comment) comment this
        DynObj.setOptionalProperty (nameof computationalTool) computationalTool this

    // Required properties
    member this.GetConformsTo() = 
        DynObj.getMandatoryDynamicPropertyOrThrow<ResizeArray<string>> "WorkflowProtocol" (nameof conformsTo) this
    static member getConformsTo = fun (wp: WorkflowProtocol) -> wp.GetConformsTo()

    // Recommended properties
    member this.GetInput() = 
        match DynObj.tryGetTypedPropertyValueAsResizeArray<FormalParameter> "input" this with
        | Some input -> Some input
        | None -> None
    static member getInput = fun (wp: WorkflowProtocol) -> wp.GetInput()

    // Optional properties
    member this.GetDescription() = 
        match DynObj.tryGetPropertyValue "description" this with
        | Some description -> Some description
        | None -> None
    static member getDescription = fun (wp: WorkflowProtocol) -> wp.GetDescription()

    member this.GetHasPart() = 
        match DynObj.tryGetTypedPropertyValueAsResizeArray<obj> "hasPart" this with
        | Some parts -> Some parts
        | None -> None
    static member getHasPart = fun (wp: WorkflowProtocol) -> wp.GetHasPart()

    member this.GetIntendedUse() = 
        match DynObj.tryGetPropertyValue "intendedUse" this with
        | Some intendedUse -> Some intendedUse
        | None -> None
    static member getIntendedUse = fun (wp: WorkflowProtocol) -> wp.GetIntendedUse()

    member this.GetComment() = 
        match DynObj.tryGetTypedPropertyValueAsResizeArray<obj> "comment" this with
        | Some comments -> Some comments
        | None -> None
    static member getComment = fun (wp: WorkflowProtocol) -> wp.GetComment()

    member this.GetComputationalTool() = 
        match DynObj.tryGetTypedPropertyValueAsResizeArray<obj> "computationalTool" this with
        | Some tools -> Some tools
        | None -> None
    static member getComputationalTool = fun (wp: WorkflowProtocol) -> wp.GetComputationalTool()

    member this.GetOutput() = 
        match DynObj.tryGetTypedPropertyValueAsResizeArray<FormalParameter> "output" this with
        | Some output -> Some output
        | None -> None
    static member getOutput = fun (wp: WorkflowProtocol) -> wp.GetOutput()

    member this.GetCreator() = 
        match DynObj.tryGetTypedPropertyValueAsResizeArray<Person> "creator" this with
        | Some creator -> Some creator
        | None -> None
    static member getCreator = fun (wp: WorkflowProtocol) -> wp.GetCreator()

    member this.GetDateCreated() = 
        match DynObj.tryGetPropertyValue "dateCreated" this with
        | Some date -> Some date
        | None -> None
    static member getDateCreated = fun (wp: WorkflowProtocol) -> wp.GetDateCreated()

    member this.GetLicense() = 
        match DynObj.tryGetPropertyValue "license" this with
        | Some license -> Some license
        | None -> None
    static member getLicense = fun (wp: WorkflowProtocol) -> wp.GetLicense()

    member this.GetName() = 
        match DynObj.tryGetPropertyValue "name" this with
        | Some name -> Some name
        | None -> None
    static member getName = fun (wp: WorkflowProtocol) -> wp.GetName()

    member this.GetProgrammingLanguage() = 
        match DynObj.tryGetTypedPropertyValueAsResizeArray<string> "programmingLanguage" this with
        | Some langs -> Some langs
        | None -> None
    static member getProgrammingLanguage = fun (wp: WorkflowProtocol) -> wp.GetProgrammingLanguage()

    member this.GetSdPublisher() = 
        match DynObj.tryGetTypedPropertyValueAsResizeArray<Person> "sdPublisher" this with
        | Some publishers -> Some publishers
        | None -> None
    static member getSdPublisher = fun (wp: WorkflowProtocol) -> wp.GetSdPublisher()

    member this.GetUrl() = 
        match DynObj.tryGetPropertyValue "url" this with
        | Some url -> Some url
        | None -> None
    static member getUrl = fun (wp: WorkflowProtocol) -> wp.GetUrl()

    member this.GetVersion() = 
        match DynObj.tryGetPropertyValue "version" this with
        | Some version -> Some version
        | None -> None
    static member getVersion = fun (wp: WorkflowProtocol) -> wp.GetVersion()
