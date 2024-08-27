namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type LabProcess(id: string, ?additionalType: string) =

    inherit ROCrateObject(id = id, schemaType = "bioschemas.org/LabProcess", ?additionalType = additionalType)

    static member create(
        // mandatory
        id,
        name,
        agent,
        object,
        result,
        // optional
        ?additionalType,
        ?executesLabProtocol,
        ?parameterValue,
        ?endTime,
        ?disambiguatingDescription
    ) =
        let lp = LabProcess(id, ?additionalType = additionalType)

        DynObj.setValue lp (nameof name) name
        DynObj.setValue lp (nameof agent) agent
        DynObj.setValue lp (nameof object) object
        DynObj.setValue lp (nameof result) result

        DynObj.setValueOpt lp (nameof executesLabProtocol) executesLabProtocol
        DynObj.setValueOpt lp (nameof parameterValue) parameterValue
        DynObj.setValueOpt lp (nameof endTime) endTime
        DynObj.setValueOpt lp (nameof disambiguatingDescription) disambiguatingDescription

        lp