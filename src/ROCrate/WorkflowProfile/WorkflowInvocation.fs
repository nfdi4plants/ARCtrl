namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

[<AttachMembers>]
type WorkflowInvocation(
    id: string,
    instrument: ResizeArray<obj>,
    agent: obj,
    result: ResizeArray<obj>,
    object: ResizeArray<obj>,
    name: string,
    ?executesLabProtocol: obj,
    ?parameterValue: obj,
    ?description: obj,
    ?endTime: System.DateTime,
    ?disambiguatingDescription: string
) as this =
    inherit LDNode(
        id = id,
        schemaType = ResizeArray[|"schema.org/CreateAction"; "bioschemas.org/LabProcess"|]
    )
    do
        // Required properties
        DynObj.setProperty (nameof instrument) instrument this
        DynObj.setProperty (nameof agent) agent this
        DynObj.setProperty (nameof result) result this
        DynObj.setProperty (nameof object) object this
        DynObj.setProperty (nameof name) name this

        // Optional properties
        DynObj.setOptionalProperty (nameof executesLabProtocol) executesLabProtocol this
        DynObj.setOptionalProperty (nameof parameterValue) parameterValue this
        DynObj.setOptionalProperty (nameof description) description this
        DynObj.setOptionalProperty (nameof endTime) endTime this
        DynObj.setOptionalProperty (nameof disambiguatingDescription) disambiguatingDescription this

    // Required properties
    member this.GetInstrument() = 
        DynObj.getMandatoryDynamicPropertyOrThrow<ResizeArray<obj>> "WorkflowInvocation" (nameof instrument) this
    static member getInstrument = fun (wi: WorkflowInvocation) -> wi.GetInstrument()

    member this.GetAgent() = 
        DynObj.getMandatoryDynamicPropertyOrThrow<obj> "WorkflowInvocation" (nameof agent) this
    static member getAgent = fun (wi: WorkflowInvocation) -> wi.GetAgent()

    member this.GetResult() = 
        DynObj.getMandatoryDynamicPropertyOrThrow<ResizeArray<obj>> "WorkflowInvocation" (nameof result) this
    static member getResult = fun (wi: WorkflowInvocation) -> wi.GetResult()

    member this.GetObject() = 
        DynObj.getMandatoryDynamicPropertyOrThrow<ResizeArray<obj>> "WorkflowInvocation" (nameof object) this
    static member getObject = fun (wi: WorkflowInvocation) -> wi.GetObject()

    member this.GetName() = 
        DynObj.getMandatoryDynamicPropertyOrThrow<string> "WorkflowInvocation" (nameof name) this
    static member getName = fun (wi: WorkflowInvocation) -> wi.GetName()

    // Optional properties
    member this.GetExecutesLabProtocol() = 
        match DynObj.tryGetPropertyValue "executesLabProtocol" this with
        | Some protocol -> Some protocol
        | None -> None
    static member getExecutesLabProtocol = fun (wi: WorkflowInvocation) -> wi.GetExecutesLabProtocol()

    member this.GetParameterValue() = 
        match DynObj.tryGetPropertyValue "parameterValue" this with
        | Some value -> Some value
        | None -> None
    static member getParameterValue = fun (wi: WorkflowInvocation) -> wi.GetParameterValue()

    member this.GetDescription() = 
        match DynObj.tryGetPropertyValue "description" this with
        | Some description -> Some description
        | None -> None
    static member getDescription = fun (wi: WorkflowInvocation) -> wi.GetDescription()

    member this.GetEndTime() = 
        match DynObj.tryGetPropertyValue "endTime" this with
        | Some time -> Some time
        | None -> None
    static member getEndTime = fun (wi: WorkflowInvocation) -> wi.GetEndTime()

    member this.GetDisambiguatingDescription() = 
        match DynObj.tryGetPropertyValue "disambiguatingDescription" this with
        | Some description -> Some description
        | None -> None
    static member getDisambiguatingDescription = fun (wi: WorkflowInvocation) -> wi.GetDisambiguatingDescription()
