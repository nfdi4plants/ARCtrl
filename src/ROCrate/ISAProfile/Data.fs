namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Data(id: string, ?additionalType: string) =
    inherit DynamicObj()

    let mutable _schemaType = "schema.org/MediaObject"
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
        ?comment,
        ?encodingFormat,
        ?disambiguatingDescription
    ) =
        let d = Data(id)

        DynObj.setValue d (nameof name) name

        DynObj.setValueOpt d (nameof comment) comment
        DynObj.setValueOpt d (nameof encodingFormat) encodingFormat
        DynObj.setValueOpt d (nameof disambiguatingDescription) disambiguatingDescription

        d