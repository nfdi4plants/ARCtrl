namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
open ARCtrl.Helper

[<AttachMembers>]
type LDPerson =

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
        match p.TryGetPropertyAsSingleton(LDPerson.givenName, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getGivenNameAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(LDPerson.givenName, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Property of `givenName` of object with @id `{p.Id}` was not a string"
        | _ -> failwith $"Could not access property `givenName` of object with @id `{p.Id}`"

    static member setGivenNameAsString(p : LDNode, n : string, ?context : LDContext) =
        p.SetProperty(LDPerson.givenName, n, ?context = context)

    static member tryGetAffiliation(p : LDNode, ?graph :LDGraph, ?context : LDContext) =
        match p.TryGetPropertyAsSingleNode(LDPerson.affiliation, ?graph = graph, ?context = context) with
        | Some n when LDOrganization.validate(n, ?context = context) -> Some n
        | _ -> None

    static member setAffiliation(p : LDNode, a : LDNode, ?context : LDContext) =
        p.SetProperty(LDPerson.affiliation, a, ?context = context)

    static member tryGetEmailAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(LDPerson.email, ?context = context) with
        | Some (:? string as e) -> Some e
        | _ -> None

    static member setEmailAsString(p : LDNode, e : string, ?context : LDContext) =
        p.SetProperty(LDPerson.email, e, ?context = context)

    static member tryGetFamilyNameAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(LDPerson.familyName, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getFamilyNameAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(LDPerson.familyName, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Property of `familyName` of object with @id `{p.Id}` was not a string"
        | _ -> failwith $"Could not access property `familyName` of object with @id `{p.Id}`"

    static member setFamilyNameAsString(p : LDNode, n : string, ?context : LDContext) =
        p.SetProperty(LDPerson.familyName, n, ?context = context)

    static member tryGetIdentifier(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(LDPerson.identifier, ?context = context) with
        | Some (:? LDNode as i) -> Some i
        | _ -> None

    static member setIdentifier(p : LDNode, i : LDNode, ?context : LDContext) =
        p.SetProperty(LDPerson.identifier, i, ?context = context)

    static member getJobTitlesAsDefinedTerm(p : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDDefinedTerm.validate(ldObject, ?context = context)
        p.GetPropertyNodes(LDPerson.jobTitle, filter = filter, ?graph = graph, ?context = context)


    static member setJobTitleAsDefinedTerm(p : LDNode, j : ResizeArray<LDNode>, ?context : LDContext) =
        p.SetProperty(LDPerson.jobTitle, j, ?context = context)

    static member tryGetAdditionalNameAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(LDPerson.additionalName, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member setAdditionalNameAsString(p : LDNode, n : string, ?context : LDContext) =
        p.SetProperty(LDPerson.additionalName, n, ?context = context)

    static member tryGetAddress(p : LDNode, ?graph : LDGraph, ?context : LDContext) : obj option =
        match p.TryGetPropertyAsSingleton(LDPerson.address, ?context = context) with
        | Some (:? LDRef as r) when graph.IsSome -> graph.Value.TryGetNode(r.Id) |> Option.map box
        | Some (:? LDNode as a) -> Some a
        | Some (:? string as s) -> Some s
        | _ -> None

    static member tryGetAddressAsPostalAddress(p : LDNode, ?graph : LDGraph, ?context : LDContext) =
        match p.TryGetPropertyAsSingleNode(LDPerson.address, ?graph = graph, ?context = context) with
        | Some n when LDPostalAddress.validate(n, ?context = context) -> Some n
        | _ -> None

    static member tryGetAddressAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(LDPerson.address, ?context = context) with
        | Some (:? string as a) -> Some a
        | _ -> None

    static member setAddressAsPostalAddress(p : LDNode, a : LDNode, ?context : LDContext) =
        p.SetProperty(LDPerson.address, a, ?context = context)

    static member setAddressAsString(p : LDNode, a : string, ?context : LDContext) =
        p.SetProperty(LDPerson.address, a, ?context = context)

    static member getDisambiguatingDescriptionsAsString(p : LDNode, ?context : LDContext) =
        let filter (value : obj) context = value :? string
        p.GetPropertyValues(LDPerson.disambiguatingDescription, filter = filter, ?context = context)
        |> ResizeArray.map (fun v -> v :?> string)

    static member setDisambiguatingDescriptionsAsString(p : LDNode, d : ResizeArray<string>, ?context : LDContext) =
        p.SetProperty(LDPerson.disambiguatingDescription, d, ?context = context)

    static member tryGetFaxNumberAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(LDPerson.faxNumber, ?context = context) with
        | Some (:? string as f) -> Some f
        | _ -> None

    static member setFaxNumberAsString(p : LDNode, f : string, ?context : LDContext) =
        p.SetProperty(LDPerson.faxNumber, f, ?context = context)

    static member tryGetTelephoneAsString(p : LDNode, ?context : LDContext) =
        match p.TryGetPropertyAsSingleton(LDPerson.telephone, ?context = context) with
        | Some (:? string as t) -> Some t
        | _ -> None

    static member setTelephoneAsString(p : LDNode, t : string, ?context : LDContext) =
        p.SetProperty(LDPerson.telephone, t, ?context = context)

    static member genId(givenName, ?orcid, ?familyName) =
        match orcid |> Option.bind ORCID.tryGetOrcidURL with
        | Some orcid -> orcid
        | None ->
            match familyName with
            | Some familyName -> $"#Person_{givenName}_{familyName}"
            | None -> $"#Person_{givenName}"
        |> Helper.ID.clean

    static member validate(p : LDNode, ?context : LDContext) =
        p.HasType(LDPerson.schemaType, ?context = context)
        && p.HasProperty(LDPerson.givenName, ?context = context)

    static member create(givenName : string, ?orcid : string, ?id : string, ?affiliation : #obj, ?email : string, ?familyName : string, ?identifier, ?jobTitles : ResizeArray<LDNode>, ?additionalName : string, ?address : #obj, ?disambiguatingDescriptions : ResizeArray<string>, ?faxNumber : string, ?telephone : string, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> LDPerson.genId(givenName, ?orcid = orcid, ?familyName = familyName)
        let person = LDNode(id, ResizeArray [LDPerson.schemaType], ?context = context)
        person.SetProperty(LDPerson.givenName, givenName, ?context = context)
        person.SetOptionalProperty(LDPerson.affiliation, affiliation, ?context = context)
        person.SetOptionalProperty(LDPerson.email, email, ?context = context)
        person.SetOptionalProperty(LDPerson.familyName, familyName, ?context = context)
        person.SetOptionalProperty(LDPerson.identifier, identifier, ?context = context)
        person.SetOptionalProperty(LDPerson.jobTitle, jobTitles, ?context = context)
        person.SetOptionalProperty(LDPerson.additionalName, additionalName, ?context = context)
        person.SetOptionalProperty(LDPerson.address, address, ?context = context)
        person.SetOptionalProperty(LDPerson.disambiguatingDescription, disambiguatingDescriptions, ?context = context)
        person.SetOptionalProperty(LDPerson.faxNumber, faxNumber, ?context = context)
        person.SetOptionalProperty(LDPerson.telephone, telephone, ?context = context)
        person