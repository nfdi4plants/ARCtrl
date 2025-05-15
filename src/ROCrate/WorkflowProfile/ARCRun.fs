namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

[<AttachMembers>]
type ARCRun(
    id: string,
    identifier: obj,
    additionalType: ResizeArray<string>,
    ?name: obj,
    ?description: obj,
    ?about: ResizeArray<WorkflowInvocation>,
    ?mentions: ResizeArray<WorkflowInvocation>,
    ?creator: ResizeArray<Person>,
    ?hasPart: ResizeArray<Dataset>,
    ?measurementMethod: obj,
    ?measurementTechnique: obj,
    ?url: obj,
    ?variableMeasured: obj
) as this =
    inherit LDNode(
        id = id,
        schemaType = ResizeArray[|"schema.org/Dataset"|]
    )
    do
        // Required properties
        DynObj.setProperty (nameof identifier) identifier this
        DynObj.setProperty (nameof additionalType) additionalType this

        // Recommended properties
        DynObj.setOptionalProperty (nameof name) name this
        DynObj.setOptionalProperty (nameof description) description this
        DynObj.setOptionalProperty (nameof about) about this
        DynObj.setOptionalProperty (nameof mentions) mentions this
        DynObj.setOptionalProperty (nameof creator) creator this
        DynObj.setOptionalProperty (nameof hasPart) hasPart this
        DynObj.setOptionalProperty (nameof measurementMethod) measurementMethod this
        DynObj.setOptionalProperty (nameof measurementTechnique) measurementTechnique this

        // Optional properties
        DynObj.setOptionalProperty (nameof url) url this
        DynObj.setOptionalProperty (nameof variableMeasured) variableMeasured this

    // Required properties
    member this.GetIdentifier() = DynObj.getMandatoryDynamicPropertyOrThrow<obj> "ARCRun" (nameof identifier) this
    static member getIdentifier = fun (ar: ARCRun) -> ar.GetIdentifier()

    member this.GetAdditionalType() = 
        DynObj.getMandatoryDynamicPropertyOrThrow<ResizeArray<string>> "ARCRun" (nameof additionalType) this
    static member getAdditionalType = fun (ar: ARCRun) -> ar.GetAdditionalType()

    // Recommended properties
    member this.GetName() = 
        match DynObj.tryGetPropertyValue "name" this with
        | Some name -> Some name
        | None -> None
    static member getName = fun (ar: ARCRun) -> ar.GetName()

    member this.GetDescription() = 
        match DynObj.tryGetPropertyValue "description" this with
        | Some description -> Some description
        | None -> None
    static member getDescription = fun (ar: ARCRun) -> ar.GetDescription()

    member this.GetAbout() = 
        match DynObj.tryGetTypedPropertyValueAsResizeArray<WorkflowInvocation> "about" this with
        | Some about -> Some about
        | None -> None
    static member getAbout = fun (ar: ARCRun) -> ar.GetAbout()

    member this.GetMentions() = 
        match DynObj.tryGetTypedPropertyValueAsResizeArray<WorkflowInvocation> "mentions" this with
        | Some mentions -> Some mentions
        | None -> None
    static member getMentions = fun (ar: ARCRun) -> ar.GetMentions()

    member this.GetCreator() = 
        match DynObj.tryGetTypedPropertyValueAsResizeArray<Person> "creator" this with
        | Some creator -> Some creator
        | None -> None
    static member getCreator = fun (ar: ARCRun) -> ar.GetCreator()

    member this.GetHasPart() = 
        match DynObj.tryGetTypedPropertyValueAsResizeArray<Dataset> "hasPart" this with
        | Some hasPart -> Some hasPart
        | None -> None
    static member getHasPart = fun (ar: ARCRun) -> ar.GetHasPart()

    member this.GetMeasurementMethod() = 
        match DynObj.tryGetPropertyValue "measurementMethod" this with
        | Some method -> Some method
        | None -> None
    static member getMeasurementMethod = fun (ar: ARCRun) -> ar.GetMeasurementMethod()

    member this.GetMeasurementTechnique() = 
        match DynObj.tryGetPropertyValue "measurementTechnique" this with
        | Some technique -> Some technique
        | None -> None
    static member getMeasurementTechnique = fun (ar: ARCRun) -> ar.GetMeasurementTechnique()

    // Optional properties
    member this.GetUrl() = 
        match DynObj.tryGetPropertyValue "url" this with
        | Some url -> Some url
        | None -> None
    static member getUrl = fun (ar: ARCRun) -> ar.GetUrl()

    member this.GetVariableMeasured() = 
        match DynObj.tryGetPropertyValue "variableMeasured" this with
        | Some variable -> Some variable
        | None -> None
    static member getVariableMeasured = fun (ar: ARCRun) -> ar.GetVariableMeasured()
