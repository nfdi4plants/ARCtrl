namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Sample(id: string, ?additionalType: string) =
    inherit DynamicObj()

    let mutable _schemaType = "bioschemas.org/Sample"
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
        ?additionalProperty,
        ?derivesFrom
    ) =
        let s = Sample(id)

        DynObj.setValue s (nameof name) name

        DynObj.setValueOpt s (nameof additionalProperty) additionalProperty
        DynObj.setValueOpt s (nameof derivesFrom) derivesFrom

        s