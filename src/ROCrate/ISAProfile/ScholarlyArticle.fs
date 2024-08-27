namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type ScholarlyArticle(id: string, ?additionalType: string) =
    inherit DynamicObj()

    let mutable _schemaType = "schema.org/ScholarlyArticle"
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
        headline,
        identifier,
        ?author,
        ?url,
        ?creativeWorkStatus,
        ?disambiguatingDescription
    ) =
        let sa = ScholarlyArticle(id)

        DynObj.setValue sa (nameof headline) headline
        DynObj.setValue sa (nameof identifier) identifier

        DynObj.setValueOpt sa (nameof author) author
        DynObj.setValueOpt sa (nameof url) url
        DynObj.setValueOpt sa (nameof creativeWorkStatus) creativeWorkStatus
        DynObj.setValueOpt sa (nameof disambiguatingDescription) disambiguatingDescription

        sa