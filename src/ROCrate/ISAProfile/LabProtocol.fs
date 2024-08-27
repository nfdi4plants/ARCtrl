namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type LabProtocol(id: string, ?additionalType: string) =
    inherit DynamicObj()

    let mutable _schemaType = "bioschemas.org/LabProtocol"
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
        ?name,
        ?intendedUse,
        ?description,
        ?url,
        ?comment,
        ?version,
        ?labEquipment,
        ?reagent,
        ?computationalTool
    ) =
        let lp = LabProcess(id)

        DynObj.setValueOpt lp (nameof name) name
        DynObj.setValueOpt lp (nameof intendedUse) intendedUse
        DynObj.setValueOpt lp (nameof description) description
        DynObj.setValueOpt lp (nameof url) url
        DynObj.setValueOpt lp (nameof comment) comment
        DynObj.setValueOpt lp (nameof version) version
        DynObj.setValueOpt lp (nameof labEquipment) labEquipment
        DynObj.setValueOpt lp (nameof reagent) reagent
        DynObj.setValueOpt lp (nameof computationalTool) computationalTool

        lp
