namespace ARCtrl.ROCrate

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

    static member comment = "http://schema.org/comment"

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

    static member getAuthors(s : LDNode, ?graph : LDGraph, ?context : LDContext) =
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

    static member getComments(s : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter = fun ldObject context -> Comment.validate(ldObject, ?context = context)
        s.GetPropertyNodes(ScholarlyArticle.comment, filter = filter, ?graph = graph, ?context = context)

    static member setcomments(s : LDNode, comments : ResizeArray<LDNode>, ?context : LDContext) =
        s.SetProperty(ScholarlyArticle.comment, comments, ?context = context)

    static member genID(headline : string, ?url : string) =
        match url with
        | Some u -> u
        | None -> headline

    static member validate(s : LDNode, ?context : LDContext) =
        s.HasType(ScholarlyArticle.schemaType, ?context = context)
        && s.HasProperty(ScholarlyArticle.headline, ?context = context)
        //&& s.HasProperty(ScholarlyArticle.identifier, ?context = context)

    static member create(headline : string, identifiers : ResizeArray<obj>, ?id : string, ?authors : ResizeArray<LDNode>, ?url : string, ?creativeWorkStatus : LDNode, ?comments : ResizeArray<LDNode>, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> ScholarlyArticle.genID(headline, ?url = url)
        let s = LDNode(id, ResizeArray [ScholarlyArticle.schemaType], ?context = context)
        s.SetProperty(ScholarlyArticle.headline, headline, ?context = context)
        s.SetProperty(ScholarlyArticle.identifier, identifiers, ?context = context)
        s.SetOptionalProperty(ScholarlyArticle.author, authors, ?context = context)
        s.SetOptionalProperty(ScholarlyArticle.url, url, ?context = context)
        s.SetOptionalProperty(ScholarlyArticle.creativeWorkStatus, creativeWorkStatus, ?context = context)
        s.SetOptionalProperty(ScholarlyArticle.comment, comments, ?context = context)
        s
        
        