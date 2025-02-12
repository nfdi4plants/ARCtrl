namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type LabProcess(
    id: string,
    name,
    agent,
    object,
    result,
    ?additionalType,
    ?executesLabProtocol,
    ?parameterValue,
    ?endTime,
    ?disambiguatingDescription
) as this =
    inherit LDNode(
        id = id,
        schemaType = ResizeArray[|"bioschemas.org/LabProcess"|],
        additionalType = defaultArg additionalType (ResizeArray[||])
    )
    do
        DynObj.setProperty (nameof name) name     this
        DynObj.setProperty (nameof agent) agent   this
        DynObj.setProperty (nameof object) object this
        DynObj.setProperty (nameof result) result this

        DynObj.setOptionalProperty (nameof executesLabProtocol) executesLabProtocol this
        DynObj.setOptionalProperty (nameof parameterValue) parameterValue this
        DynObj.setOptionalProperty (nameof endTime) endTime this
        DynObj.setOptionalProperty (nameof disambiguatingDescription) disambiguatingDescription this

    member this.GetName() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "LabProcess" (nameof name) this
    static member getName = fun (lp: LabProcess) -> lp.GetName()

    member this.GetAgent() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "LabProcess" (nameof agent) this
    static member getAgent = fun (lp: LabProcess) -> lp.GetAgent()

    member this.GetObject() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "LabProcess" (nameof object) this
    static member getObject = fun (lp: LabProcess) -> lp.GetObject()

    member this.GetResult() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "LabProcess" (nameof result) this
    static member getResult = fun (lp: LabProcess) -> lp.GetResult()
