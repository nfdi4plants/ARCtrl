namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.FileSystem
open System.Collections.Generic
//open ColumnIndex

open ColumnIndex
open ARCtrl.Helper.Regex.ActivePatterns



type PersonConversion = 

    static member orcidKey = "ORCID"   

    static member composeAffiliation (affiliation : string) : LDNode =
        try 
            ARCtrl.Json.Decode.fromJsonString Json.LDNode.decoder affiliation
        with
        | _ -> LDOrganization.create(name = affiliation)

    static member decomposeAffiliation (affiliation : LDNode, ?context : LDContext) : string =
        let hasOnlyName = 
            affiliation.GetPropertyNames(?context = context)
            |> Seq.filter(fun n -> n <> LDOrganization.name)
            |> Seq.isEmpty
        if hasOnlyName then
            LDOrganization.getNameAsString(affiliation, ?context = context)
        else
            Json.LDNode.encoder affiliation
            |> ARCtrl.Json.Encode.toJsonString 0

    static member composeAddress (address : string) : obj =
        try 
            ARCtrl.Json.Decode.fromJsonString Json.LDNode.decoder address
            |> box
        with
        | _ -> address

    static member decomposeAddress (address : obj) : string =
        match address with
        | :? string as s -> s
        | :? LDNode as n -> 
            Json.LDNode.encoder n
            |> ARCtrl.Json.Encode.toJsonString 0
        | _ -> failwith "Address must be a string or a Json.LDNode"

    static member composePerson (person : ARCtrl.Person) =
        let givenName =
            match person.FirstName with
            | Some fn -> fn
            | None -> failwith "Person must have a given name"
        let jobTitles = 
            person.Roles
            |> ResizeArray.map BaseTypes.composeDefinedTerm
            |> Option.fromSeq
        let disambiguatingDescriptions = 
            person.Comments
            |> ResizeArray.map (fun c -> c.ToString())
            |> Option.fromSeq
        let address =
            person.Address
            |> Option.map PersonConversion.composeAddress
        let affiliation = 
            person.Affiliation
            |> Option.map PersonConversion.composeAffiliation
        LDPerson.create(givenName, ?orcid = person.ORCID, ?affiliation = affiliation, ?email = person.EMail, ?familyName = person.LastName, ?jobTitles = jobTitles, ?additionalName = person.MidInitials, ?address = address, ?disambiguatingDescriptions = disambiguatingDescriptions, ?faxNumber = person.Fax, ?telephone = person.Phone)

    static member decomposePerson (person : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let orcid = ORCID.tryGetOrcidNumber person.Id
        let address = 
            match LDPerson.tryGetAddressAsString(person, ?context = context) with
            | Some s -> 
                Some s
            | None ->
                match LDPerson.tryGetAddressAsPostalAddress(person, ?graph = graph, ?context = context) with
                | Some a -> Some (PersonConversion.decomposeAddress a)
                | None -> None
        let roles = 
            LDPerson.getJobTitlesAsDefinedTerm(person, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun r -> BaseTypes.decomposeDefinedTerm(r, ?context = context))
        let comments =
            LDPerson.getDisambiguatingDescriptionsAsString(person, ?context = context)
            |> ResizeArray.map Comment.fromString
        let affiliation =
            LDPerson.tryGetAffiliation(person, ?graph = graph, ?context = context)
            |> Option.map (fun a -> PersonConversion.decomposeAffiliation(a, ?context = context))
        Person.create(
            firstName = LDPerson.getGivenNameAsString(person, ?context = context),
            ?lastName = LDPerson.tryGetFamilyNameAsString(person, ?context = context),
            ?midInitials = LDPerson.tryGetAdditionalNameAsString(person, ?context = context),
            ?email = LDPerson.tryGetEmailAsString(person, ?context = context),
            ?fax = LDPerson.tryGetFaxNumberAsString(person, ?context = context),
            ?phone = LDPerson.tryGetTelephoneAsString(person, ?context = context),
            ?orcid = orcid,
            ?affiliation = affiliation,
            roles = roles,
            ?address = address,
            comments = comments
        )
        