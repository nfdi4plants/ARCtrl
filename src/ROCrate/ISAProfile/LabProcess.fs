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
    inherit ROCrateObject(id = id, schemaType = "bioschemas.org/LabProcess", ?additionalType = additionalType)
    do
        DynObj.setProperty (nameof name) name     this
        DynObj.setProperty (nameof agent) agent   this
        DynObj.setProperty (nameof object) object this
        DynObj.setProperty (nameof result) result this

        DynObj.setOptionalProperty (nameof executesLabProtocol) executesLabProtocol this
        DynObj.setOptionalProperty (nameof parameterValue) parameterValue this
        DynObj.setOptionalProperty (nameof endTime) endTime this
        DynObj.setOptionalProperty (nameof disambiguatingDescription) disambiguatingDescription this

    member this.GetName() = DynObj.tryGetPropertyValue (nameof name) this |> Option.get
    static member getName = fun (lp: LabProcess) -> lp.GetName()

    member this.GetAgent() = DynObj.tryGetTypedPropertyValue<string> (nameof agent) this |> Option.get
    static member getAgent = fun (lp: LabProcess) -> lp.GetAgent()

    member this.GetObject() = DynObj.tryGetTypedPropertyValue<string> (nameof object) this |> Option.get
    static member getObject = fun (lp: LabProcess) -> lp.GetObject()

    member this.GetResult() = DynObj.tryGetTypedPropertyValue<string> (nameof result) this |> Option.get
    static member getResult = fun (lp: LabProcess) -> lp.GetResult()
