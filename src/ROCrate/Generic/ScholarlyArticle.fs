namespace ARCtrl.ROCrate.Generic

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
///

[<AttachMembers>]
type ScholarlyArticle =

    static member schemaType = "http://schema.org/ScholarlyArticle"

    static member headline = "http://schema.org/headline"

    static member identifier = "http://schema.org/identifier"

    static member author = "http://schema.org/author"

    static member url = "http://schema.org/url"

    static member creativeWorkStatus = "http://schema.org/creativeWorkStatus"

    static member disambiguatingDescription = "http://schema.org/disambiguatingDescription"

    static member tryGetHeadlineAsString(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(ScholarlyArticle.headline, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getHeadlineAsString(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(ScholarlyArticle.headline, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Value of property `headline` of object with @id `{s.Id}` should have been a string"
        | None -> failwith $"Could not access property `headline` of object with @id `{s.Id}`"

    static member setHeadlineAsString(s : LDNode, n : string) =
        s.SetProperty(ScholarlyArticle.headline, n)

    static member getIdentifiers(s : LDNode, ?context : LDContext) =
        s.GetPropertyValues(ScholarlyArticle.identifier, ?context = context)

    static member setIdentifiers(s : LDNode, identifiers : ResizeArray<obj>) =
        s.SetProperty(ScholarlyArticle.identifier, identifiers)

    static member tryGetAuthors(s : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = Person.validate(ldObject, ?context = context)
        s.GetPropertyNodes(ScholarlyArticle.author, filter = filter, ?graph = graph, ?context = context)

    static member setAuthors(s : LDNode, authors : ResizeArray<LDNode>, ?context : LDContext) =
        s.SetProperty(ScholarlyArticle.author, authors, ?context = context)

    static member tryGetUrl(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(ScholarlyArticle.url, ?context = context) with
        | Some (:? string as u) -> Some u
        | _ -> None

    static member setUrl(s : LDNode, u : string, ?context : LDContext) =
        s.SetProperty(ScholarlyArticle.url, u, ?context = context)

    static member tryGetCreativeWorkStatus(s : LDNode, ?graph : LDGraph, ?context : LDContext) =
        match s.TryGetPropertyAsSingleNode(ScholarlyArticle.creativeWorkStatus, ?graph  = graph, ?context = context) with
        | Some cws when DefinedTerm.validate cws -> Some cws
        | _ -> None

    static member setCreativeWorkStatus(s : LDNode, cws : LDNode, ?context : LDContext) =
        s.SetProperty(ScholarlyArticle.creativeWorkStatus, cws, ?context = context)

    static member tryGetDisambiguatingDescriptionAsString(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(ScholarlyArticle.disambiguatingDescription, ?context = context) with
        | Some (:? string as dd) -> Some dd
        | _ -> None

    static member setDisambiguatingDescriptionAsString(s : LDNode, dd : string, ?context : LDContext) =
        s.SetProperty(ScholarlyArticle.disambiguatingDescription, dd, ?context = context)

    static member validate(s : LDNode, ?context : LDContext) =
        s.HasType(ScholarlyArticle.schemaType, ?context = context)
        && s.HasProperty(ScholarlyArticle.headline, ?context = context)
        && s.HasProperty(ScholarlyArticle.identifier, ?context = context)

    static member create(id : string, headline : string, identifiers : ResizeArray<obj>, ?authors : ResizeArray<LDNode>, ?url : string, ?creativeWorkStatus : LDNode, ?disambiguatingDescription : string, ?context : LDContext) =
        let s = LDNode(id, ResizeArray [ScholarlyArticle.schemaType], ?context = context)
        s.SetProperty(ScholarlyArticle.headline, headline, ?context = context)
        s.SetProperty(ScholarlyArticle.identifier, identifiers, ?context = context)
        s.SetOptionalProperty(ScholarlyArticle.author, authors, ?context = context)
        s.SetOptionalProperty(ScholarlyArticle.url, url, ?context = context)
        s.SetOptionalProperty(ScholarlyArticle.creativeWorkStatus, creativeWorkStatus, ?context = context)
        s.SetOptionalProperty(ScholarlyArticle.disambiguatingDescription, disambiguatingDescription, ?context = context)
        s
        
        