namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
///

[<AttachMembers>]
type LDScholarlyArticle =

    static member schemaType = "http://schema.org/ScholarlyArticle"

    static member headline = "http://schema.org/headline"

    static member identifier = "http://schema.org/identifier"

    static member author = "http://schema.org/author"

    static member url = "http://schema.org/url"

    static member creativeWorkStatus = "http://schema.org/creativeWorkStatus"

    static member comment = "http://schema.org/comment"

    static member tryGetHeadlineAsString(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(LDScholarlyArticle.headline, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getHeadlineAsString(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(LDScholarlyArticle.headline, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Value of property `headline` of object with @id `{s.Id}` should have been a string"
        | None -> failwith $"Could not access property `headline` of object with @id `{s.Id}`"

    static member setHeadlineAsString(s : LDNode, n : string) =
        s.SetProperty(LDScholarlyArticle.headline, n)

    static member getIdentifiers(s : LDNode, ?context : LDContext) =
        s.GetPropertyValues(LDScholarlyArticle.identifier, ?context = context)

    static member getIdentifiersAsPropertyValue(s : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter = fun ldObject context -> LDPropertyValue.validate(ldObject, ?context = context)
        s.GetPropertyNodes(LDScholarlyArticle.identifier, filter = filter, ?graph = graph, ?context = context)

    static member setIdentifiers(s : LDNode, identifiers : ResizeArray<obj>) =
        s.SetProperty(LDScholarlyArticle.identifier, identifiers)

    static member getAuthors(s : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDPerson.validate(ldObject, ?context = context)
        s.GetPropertyNodes(LDScholarlyArticle.author, filter = filter, ?graph = graph, ?context = context)

    static member setAuthors(s : LDNode, authors : ResizeArray<LDNode>, ?context : LDContext) =
        s.SetProperty(LDScholarlyArticle.author, authors, ?context = context)

    static member tryGetUrl(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(LDScholarlyArticle.url, ?context = context) with
        | Some (:? string as u) -> Some u
        | _ -> None

    static member setUrl(s : LDNode, u : string, ?context : LDContext) =
        s.SetProperty(LDScholarlyArticle.url, u, ?context = context)

    static member tryGetCreativeWorkStatus(s : LDNode, ?graph : LDGraph, ?context : LDContext) =
        match s.TryGetPropertyAsSingleNode(LDScholarlyArticle.creativeWorkStatus, ?graph  = graph, ?context = context) with
        | Some cws when LDDefinedTerm.validate(cws,?context = context) -> Some cws
        | _ -> None

    static member setCreativeWorkStatus(s : LDNode, cws : LDNode, ?context : LDContext) =
        s.SetProperty(LDScholarlyArticle.creativeWorkStatus, cws, ?context = context)

    static member getComments(s : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter = fun ldObject context -> LDComment.validate(ldObject, ?context = context)
        s.GetPropertyNodes(LDScholarlyArticle.comment, filter = filter, ?graph = graph, ?context = context)

    static member setcomments(s : LDNode, comments : ResizeArray<LDNode>, ?context : LDContext) =
        s.SetProperty(LDScholarlyArticle.comment, comments, ?context = context)

    static member genID(headline : string, ?url : string) =
        match url with
        | Some u -> u
        | None -> $"#{headline}"
        |> Helper.ID.clean

    static member validate(s : LDNode, ?context : LDContext) =
        s.HasType(LDScholarlyArticle.schemaType, ?context = context)
        && s.HasProperty(LDScholarlyArticle.headline, ?context = context)
        //&& s.HasProperty(ScholarlyArticle.identifier, ?context = context)

    static member create(headline : string, identifiers : ResizeArray<#obj>, ?id : string, ?authors : ResizeArray<LDNode>, ?url : string, ?creativeWorkStatus : LDNode, ?comments : ResizeArray<LDNode>, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> LDScholarlyArticle.genID(headline, ?url = url)
        let s = LDNode(id, ResizeArray [LDScholarlyArticle.schemaType], ?context = context)
        s.SetProperty(LDScholarlyArticle.headline, headline, ?context = context)
        s.SetProperty(LDScholarlyArticle.identifier, identifiers, ?context = context)
        s.SetOptionalProperty(LDScholarlyArticle.author, authors, ?context = context)
        s.SetOptionalProperty(LDScholarlyArticle.url, url, ?context = context)
        s.SetOptionalProperty(LDScholarlyArticle.creativeWorkStatus, creativeWorkStatus, ?context = context)
        s.SetOptionalProperty(LDScholarlyArticle.comment, comments, ?context = context)
        s
        
        