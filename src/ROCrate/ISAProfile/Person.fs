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

        DynObj.setProperty (nameof givenName) givenName this

        DynObj.setOptionalProperty (nameof familyName) familyName         this
        DynObj.setOptionalProperty (nameof email) email                   this
        DynObj.setOptionalProperty (nameof identifier) identifier         this
        DynObj.setOptionalProperty (nameof affiliation) affiliation       this
        DynObj.setOptionalProperty (nameof jobTitle) jobTitle             this
        DynObj.setOptionalProperty (nameof additionalName) additionalName this
        DynObj.setOptionalProperty (nameof address) address               this
        DynObj.setOptionalProperty (nameof telephone) telephone           this
        DynObj.setOptionalProperty (nameof faxNumber) faxNumber           this
        DynObj.setOptionalProperty (nameof disambiguatingDescription) disambiguatingDescription this

    member this.GetGivenName() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "Person" (nameof givenName) this
    static member getGivenName = fun (p: Person) -> p.GetGivenName()