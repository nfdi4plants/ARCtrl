namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Person(id: string, ?additionalType: string) =
    inherit DynamicObj()

    let mutable _schemaType = "schema.org/Person"
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
        givenName,
        ?familyName,
        ?email,
        ?identifier,
        ?affiliation,
        ?jobTitle,
        ?additionalName,
        ?address,
        ?telephone,
        ?faxNumber,
        ?disambiguatingDescription
    ) =
        let p = Person(id)

        DynObj.setValue p (nameof givenName) givenName

        DynObj.setValueOpt p (nameof familyName) familyName
        DynObj.setValueOpt p (nameof email) email
        DynObj.setValueOpt p (nameof identifier) identifier
        DynObj.setValueOpt p (nameof affiliation) affiliation
        DynObj.setValueOpt p (nameof jobTitle) jobTitle
        DynObj.setValueOpt p (nameof additionalName) additionalName
        DynObj.setValueOpt p (nameof address) address
        DynObj.setValueOpt p (nameof telephone) telephone
        DynObj.setValueOpt p (nameof faxNumber) faxNumber
        DynObj.setValueOpt p (nameof disambiguatingDescription) disambiguatingDescription

        p
