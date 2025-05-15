namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

[<AttachMembers>]
type Workflow(
    id: string,
    identifier: obj,
    mainEntity: ResizeArray<WorkflowProtocol>,
    additionalType: ResizeArray<string>,
    ?hasPart: ResizeArray<Dataset>,
    ?intendedUse: obj,
    ?url: obj,
    ?version: obj,
    ?name: obj,
    ?description: obj
) as this =
    inherit LDNode(
        id = id,
        schemaType = ResizeArray[|"schema.org/Dataset"|]
    )
    do
        // Required properties
        DynObj.setProperty (nameof identifier) identifier this
        DynObj.setProperty (nameof mainEntity) mainEntity this
        DynObj.setProperty (nameof additionalType) additionalType this

        // Recommended properties
        DynObj.setOptionalProperty (nameof name) name this
        DynObj.setOptionalProperty (nameof description) description this
        DynObj.setOptionalProperty (nameof hasPart) hasPart this
        DynObj.setOptionalProperty (nameof intendedUse) intendedUse this

        // Optional properties
        DynObj.setOptionalProperty (nameof url) url this
        DynObj.setOptionalProperty (nameof version) version this

    // Required properties
    member this.GetIdentifier() = DynObj.getMandatoryDynamicPropertyOrThrow<obj> "Workflow" (nameof identifier) this
    static member getIdentifier = fun (w: Workflow) -> w.GetIdentifier()

    member this.GetMainEntity() = 
        DynObj.getMandatoryDynamicPropertyOrThrow<ResizeArray<WorkflowProtocol>> "Workflow" (nameof mainEntity) this
    static member getMainEntity = fun (w: Workflow) -> w.GetMainEntity()

    member this.GetAdditionalType() = 
        DynObj.getMandatoryDynamicPropertyOrThrow<ResizeArray<string>> "Workflow" (nameof additionalType) this
    static member getAdditionalType = fun (w: Workflow) -> w.GetAdditionalType()

    // Recommended properties
    member this.GetName() = 
        match DynObj.tryGetPropertyValue "name" this with
        | Some name -> Some name
        | None -> None
    static member getName = fun (w: Workflow) -> w.GetName()

    member this.GetDescription() = 
        match DynObj.tryGetPropertyValue "description" this with
        | Some description -> Some description
        | None -> None
    static member getDescription = fun (w: Workflow) -> w.GetDescription()

    member this.GetHasPart() = 
        match DynObj.tryGetTypedPropertyValueAsResizeArray<Dataset> "hasPart" this with
        | Some hasPart -> Some hasPart
        | None -> None
    static member getHasPart = fun (w: Workflow) -> w.GetHasPart()

    member this.GetIntendedUse() = 
        match DynObj.tryGetPropertyValue "intendedUse" this with
        | Some intendedUse -> Some intendedUse
        | None -> None
    static member getIntendedUse = fun (w: Workflow) -> w.GetIntendedUse()

    // Optional properties
    member this.GetUrl() = 
        match DynObj.tryGetPropertyValue "url" this with
        | Some url -> Some url
        | None -> None
    static member getUrl = fun (w: Workflow) -> w.GetUrl()

    member this.GetVersion() = 
        match DynObj.tryGetPropertyValue "version" this with
        | Some version -> Some version
        | None -> None
    static member getVersion = fun (w: Workflow) -> w.GetVersion()
