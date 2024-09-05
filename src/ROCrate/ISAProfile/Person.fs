namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core

///
[<AttachMembers>]
type Person(
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
) as this=
    inherit ROCrateObject(id = id, schemaType = "schema.org/Person", ?additionalType = additionalType)
    do

        DynObj.setValue this (nameof givenName) givenName

        DynObj.setValueOpt this (nameof familyName) familyName
        DynObj.setValueOpt this (nameof email) email
        DynObj.setValueOpt this (nameof identifier) identifier
        DynObj.setValueOpt this (nameof affiliation) affiliation
        DynObj.setValueOpt this (nameof jobTitle) jobTitle
        DynObj.setValueOpt this (nameof additionalName) additionalName
        DynObj.setValueOpt this (nameof address) address
        DynObj.setValueOpt this (nameof telephone) telephone
        DynObj.setValueOpt this (nameof faxNumber) faxNumber
        DynObj.setValueOpt this (nameof disambiguatingDescription) disambiguatingDescription
