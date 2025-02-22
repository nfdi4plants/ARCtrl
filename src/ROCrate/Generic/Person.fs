namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

///
//Property	Required	Expected Type	Description
//@id	MUST	Text or URL	
//@type	MUST	Text	must be 'schema.org/Person'
//givenName	MUST	Text	Given name of a person. Can be used for any type of name.
//affiliation	SHOULD	schema.org/Organization	
//email	SHOULD	Text	
//familyName	SHOULD	Text	Family name of a person.
//identifier	SHOULD	Text or URL or schema.org/PropertyValue	One or many identifiers for this person, e.g. an ORCID. Can be of type PropertyValue to indicate the kind of reference.
//jobTitle	SHOULD	schema.org/DefinedTerm	
//additionalName	COULD	Text	
//address	COULD	PostalAddress or Text	
//disambiguatingDescription	COULD	Text	
//faxNumber	COULD	Text	
//telephone	COULD	Text	



[<AttachMembers>]
type Person =

    static member schemaType = "http://schema.org/Person"
    static member givenName = "http://schema.org/givenName"
    static member affiliation = "http://schema.org/affiliation"
    static member email = "http://schema.org/email"
    static member familyName = "http://schema.org/familyName"
    static member identifier = "http://schema.org/identifier"
    static member jobTitle = "http://schema.org/jobTitle"
    static member additionalName = "http://schema.org/additionalName"
    static member address = "http://schema.org/address"
    static member disambiguatingDescription = "http://schema.org/disambiguatingDescription"
    static member faxNumber = "http://schema.org/faxNumber"
    static member telephone = "http://schema.org/telephone"

    static member tryGetGivenNameAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(Person.givenName, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getGivenNameAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(Person.givenName, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Property of `givenName` of object with @id `{p.Id}` was not a string"
        | _ -> failwith $"Could not access property `givenName` of object with @id `{p.Id}`"

    static member setGivenNameAsString(p : LDNode, n : string, ?context : LDContext) =
        p.SetProperty(Person.givenName, n, ?context = context)

    static member tryGetAffiliation(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(Person.affiliation, ?context = context) with
        | Some (:? LDNode as a) -> Some a
        | _ -> None

    static member setAffiliation(p : LDNode, a : LDNode, ?context : LDContext) =
        p.SetProperty(Person.affiliation, a, ?context = context)

    static member tryGetEmailAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(Person.email, ?context = context) with
        | Some (:? string as e) -> Some e
        | _ -> None

    static member setEmailAsString(p : LDNode, e : string, ?context : LDContext) =
        p.SetProperty(Person.email, e, ?context = context)

    static member tryGetFamilyNameAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(Person.familyName, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getFamilyNameAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(Person.familyName, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Property of `familyName` of object with @id `{p.Id}` was not a string"
        | _ -> failwith $"Could not access property `familyName` of object with @id `{p.Id}`"

    static member setFamilyNameAsString(p : LDNode, n : string, ?context : LDContext) =
        p.SetProperty(Person.familyName, n, ?context = context)

    static member tryGetIdentifier(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(Person.identifier, ?context = context) with
        | Some (:? LDNode as i) -> Some i
        | _ -> None

    static member setIdentifier(p : LDNode, i : LDNode, ?context : LDContext) =
        p.SetProperty(Person.identifier, i, ?context = context)

    static member tryGetJobTitleAsDefinedTerm(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(Person.jobTitle, ?context = context) with
        | Some (:? LDNode as j) when DefinedTerm.validate j-> Some j
        | _ -> None

    static member setJobTitleAsDefinedTerm(p : LDNode, j : LDNode, ?context : LDContext) =
        p.SetProperty(Person.jobTitle, j, ?context = context)

    static member tryGetAdditionalNameAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(Person.additionalName, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member setAdditionalNameAsString(p : LDNode, n : string, ?context : LDContext) =
        p.SetProperty(Person.additionalName, n, ?context = context)

    static member tryGetAddress(p : LDNode, ?context : LDContext) : obj option =
        match p.TryGetPropertyAsSingleton(Person.address, ?context = context) with
        | Some (:? LDNode as a) -> Some a
        | Some (:? string as s) -> Some s
        | _ -> None

    static member tryGetAddressAsPostalAddress(p : LDNode, ?graph : LDGraph, ?context : LDContext) =
        match p.TryGetPropertyAsSingleNode(Person.address, ?graph = graph, ?context = context) with
        | Some n when PostalAddress.validate n -> Some n
        | _ -> None

    static member tryGetAddressAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(Person.address, ?context = context) with
        | Some (:? string as a) -> Some a
        | _ -> None

    static member setAddressAsPostalAddress(p : LDNode, a : LDNode, ?context : LDContext) =
        p.SetProperty(Person.address, a, ?context = context)

    static member setAddressAsString(p : LDNode, a : string, ?context : LDContext) =
        p.SetProperty(Person.address, a, ?context = context)

    static member tryGetDisambiguatingDescriptionAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(Person.disambiguatingDescription, ?context = context) with
        | Some (:? string as d) -> Some d
        | _ -> None

    static member setDisambiguatingDescriptionAsString(p : LDNode, d : string, ?context : LDContext) =
        p.SetProperty(Person.disambiguatingDescription, d, ?context = context)

    static member tryGetFaxNumberAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(Person.faxNumber, ?context = context) with
        | Some (:? string as f) -> Some f
        | _ -> None

    static member setFaxNumberAsString(p : LDNode, f : string, ?context : LDContext) =
        p.SetProperty(Person.faxNumber, f, ?context = context)

    static member tryGetTelephoneAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(Person.telephone, ?context = context) with
        | Some (:? string as t) -> Some t
        | _ -> None

    static member setTelephoneAsString(p : LDNode, t : string, ?context : LDContext) =
        p.SetProperty(Person.telephone, t, ?context = context)

    static member validate(p : LDNode, ?context : LDContext) =
        p.HasType(Person.schemaType, ?context = context)
        && p.HasProperty(Person.givenName, ?context = context)

    static member create(id : string, givenName : string, ?affiliation : obj, ?email : string, ?familyName : string, ?identifier, ?jobTitle : LDNode, ?additionalName : string, ?address : obj, ?disambiguatingDescription : string, ?faxNumber : string, ?telephone : string, ?context : LDContext) =
        let person = LDNode(id, ResizeArray [Person.schemaType], ?context = context)
        person.SetProperty(Person.givenName, givenName, ?context = context)
        person.SetOptionalProperty(Person.affiliation, affiliation, ?context = context)
        person.SetOptionalProperty(Person.email, email, ?context = context)
        person.SetOptionalProperty(Person.familyName, familyName, ?context = context)
        person.SetOptionalProperty(Person.identifier, identifier, ?context = context)
        person.SetOptionalProperty(Person.jobTitle, jobTitle, ?context = context)
        person.SetOptionalProperty(Person.additionalName, additionalName, ?context = context)
        person.SetOptionalProperty(Person.address, address, ?context = context)
        person.SetOptionalProperty(Person.disambiguatingDescription, disambiguatingDescription, ?context = context)
        person.SetOptionalProperty(Person.faxNumber, faxNumber, ?context = context)
        person.SetOptionalProperty(Person.telephone, telephone, ?context = context)
        person