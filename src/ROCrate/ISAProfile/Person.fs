namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Person(id: string, ?additionalType: string) =
    inherit ROCrateObject(id = id, schemaType = "schema.org/Person", ?additionalType = additionalType)

    static member create(
        // mandatory
        id,
        givenName,
        ?additionalType,
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
        let p = Person(id, ?additionalType = additionalType)

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
