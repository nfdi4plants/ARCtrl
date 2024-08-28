namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type LabProcess(
    id,
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
        DynObj.setValue this (nameof name) name
        DynObj.setValue this (nameof agent) agent
        DynObj.setValue this (nameof object) object
        DynObj.setValue this (nameof result) result

        DynObj.setValueOpt this (nameof executesLabProtocol) executesLabProtocol
        DynObj.setValueOpt this (nameof parameterValue) parameterValue
        DynObj.setValueOpt this (nameof endTime) endTime
        DynObj.setValueOpt this (nameof disambiguatingDescription) disambiguatingDescription