namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type LabProcess(id: string, ?additionalType: string) =
    inherit DynamicObj()

    let mutable _schemaType = "bioschemas.org/LabProcess"
    let mutable _additionalType = additionalType

    member this.Id 
        with get() = id
    
    member this.SchemaType 
        with get() = _schemaType
        and set(value) = _schemaType <- value

    member this.AdditionalType
        with get() = _additionalType
        and set(value) = _additionalType <- value

     //interface implementations
    interface IROCrateObject with 
        member this.Id with get () = this.Id
        member this.SchemaType with get (): string = this.SchemaType
        member this.AdditionalType with get (): string option = this.AdditionalType

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
        let lp = LabProcess(id)

        DynObj.setValue lp (nameof name) name
        DynObj.setValue lp (nameof agent) agent
        DynObj.setValue lp (nameof object) object
        DynObj.setValue lp (nameof result) result

        DynObj.setValueOpt lp (nameof additionalType) additionalType
        DynObj.setValueOpt lp (nameof executesLabProtocol) executesLabProtocol
        DynObj.setValueOpt lp (nameof parameterValue) parameterValue
        DynObj.setValueOpt lp (nameof endTime) endTime
        DynObj.setValueOpt lp (nameof disambiguatingDescription) disambiguatingDescription

        lp