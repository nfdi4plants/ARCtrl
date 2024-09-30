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
        DynObj.setProperty (nameof name) name     this
        DynObj.setProperty (nameof agent) agent   this
        DynObj.setProperty (nameof object) object this
        DynObj.setProperty (nameof result) result this

        DynObj.setOptionalProperty (nameof executesLabProtocol) executesLabProtocol             this 
        DynObj.setOptionalProperty (nameof parameterValue) parameterValue                       this 
        DynObj.setOptionalProperty (nameof endTime) endTime                                     this 
        DynObj.setOptionalProperty (nameof disambiguatingDescription) disambiguatingDescription this 