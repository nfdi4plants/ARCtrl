namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
open ARCtrl.Helper

[<AttachMembers>]
type LDCreativeWork =

    static member schemaType = "http://schema.org/Dataset"

    static member identifier = "http://schema.org/identifier"

    static member creator = "http://schema.org/creator"

    static member dateCreated = "http://schema.org/dateCreated"

    static member datePublished = "http://schema.org/datePublished"

    static member sdDatePublished = "http://schema.org/datePublished"

    static member license = "http://schema.org/license"

    static member dateModified = "http://schema.org/dateModified"

    static member description = "http://schema.org/description"

    static member hasPart = "http://schema.org/hasPart"

    static member headline = "http://schema.org/headline"

    static member name = "http://schema.org/name"

    static member text = "http://schema.org/text"

    static member citation = "http://schema.org/citation"

    static member comment = "http://schema.org/comment"

    static member mentions = "http://schema.org/mentions"

    static member url = "http://schema.org/url"

    static member about = "http://schema.org/about"


    static member tryGetIdentifierAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDCreativeWork.identifier, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getIdentifierAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDCreativeWork.identifier, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `identifier` of object with @id `{lp.Id}` was not a string"
        | _ -> failwith $"Could not access property `identifier` of object with @id `{lp.Id}`"

    static member setIdentifierAsString(lp : LDNode, identifier : string, ?context : LDContext) =
        lp.SetProperty(LDCreativeWork.identifier, identifier, ?context = context)

    static member getCreators(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter (p : LDNode) (ctx : LDContext option) = LDPerson.validate(p, ?context = ctx)
        lp.GetPropertyNodes(LDCreativeWork.creator, filter = filter, ?graph = graph, ?context = context)

    //static member getCreatorsAsPerson(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
    //    let filter (p : LDNode) (ctx : LDContext option) = Person.validate(p, ?context = ctx)
    //    lp.GetPropertyNodes(Dataset.creator, filter = filter, ?graph = graph, ?context = context)

    static member setCreators(lp : LDNode, creators : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDCreativeWork.creator, creators, ?context = context)

    static member tryGetDateCreatedAsDateTime(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDCreativeWork.dateCreated, ?context = context) with
        | Some (:? System.DateTime as n) -> Some n
        | _ -> None

    static member setDateCreatedAsDateTime(lp : LDNode, dateCreated : System.DateTime, ?context : LDContext) =
        lp.SetProperty(LDCreativeWork.dateCreated, dateCreated, ?context = context)

    static member tryGetDatePublishedAsDateTime(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDCreativeWork.datePublished, ?context = context) with
        | Some (:? System.DateTime as n) -> Some n
        | _ -> None

    static member setDatePublishedAsDateTime(lp : LDNode, datePublished : System.DateTime, ?context : LDContext) =
        lp.SetProperty(LDCreativeWork.datePublished, datePublished, ?context = context)

    static member tryGetSDDatePublishedAsDateTime(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDCreativeWork.sdDatePublished, ?context = context) with
        | Some (:? System.DateTime as n) -> Some n
        | _ -> None

    static member setSDDatePublishedAsDateTime(lp : LDNode, sdDatePublished : System.DateTime, ?context : LDContext) =
        lp.SetProperty(LDCreativeWork.sdDatePublished, sdDatePublished, ?context = context)

    static member tryGetLicenseAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDCreativeWork.license, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    //static member tryGetLicenseAsCreativeWork(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
    //    match lp.TryGetPropertyAsSingleNode(Dataset.license, ?graph = graph, ?context = context) with
    //    | Some n when CreativeWork.validate(n, ?context = context) -> Some n
    //    | _ -> None

    static member setLicenseAsString(lp : LDNode, license : string, ?context : LDContext) =
        lp.SetProperty(LDCreativeWork.license, license, ?context = context)

    static member setLicenseAsCreativeWork(lp : LDNode, license : obj, ?context : LDContext) =
        lp.SetProperty(LDCreativeWork.license, license, ?context = context)

    static member tryGetDateModifiedAsDateTime(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDCreativeWork.dateModified, ?context = context) with
        | Some (:? System.DateTime as n) -> Some n
        | _ -> None

    static member setDateModifiedAsDateTime(lp : LDNode, dateModified : System.DateTime, ?context : LDContext) =
        lp.SetProperty(LDCreativeWork.dateModified, dateModified, ?context = context)

    static member tryGetDescriptionAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDCreativeWork.description, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getDescriptionAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDCreativeWork.description, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `description` of object with @id `{lp.Id}` was not a string"
        | _ -> failwith $"Could not access property `description` of object with @id `{lp.Id}`"

    static member setDescriptionAsString(lp : LDNode, description : string, ?context : LDContext) =
        lp.SetProperty(LDCreativeWork.description, description, ?context = context)

    static member getHasParts(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        lp.GetPropertyNodes(LDCreativeWork.hasPart, ?graph = graph, ?context = context)

    static member getHasPartsAsDataset(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldnode context = LDCreativeWork.validate(ldnode, ?context = context)
        lp.GetPropertyNodes(LDCreativeWork.hasPart, filter = filter, ?graph = graph, ?context = context)

    static member getHasPartsAsFile(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldnode context = LDFile.validate(ldnode, ?context = context)
        lp.GetPropertyNodes(LDCreativeWork.hasPart, filter = filter, ?graph = graph, ?context = context)

    static member setHasParts(lp : LDNode, hasParts : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDCreativeWork.hasPart, hasParts, ?context = context)

    static member tryGetHeadlineAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDCreativeWork.headline, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member tryGetNameAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDCreativeWork.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDCreativeWork.name, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `name` of object with @id `{lp.Id}` was not a string"
        | _ -> failwith $"Could not access property `name` of object with @id `{lp.Id}`"

    static member setNameAsString(lp : LDNode, name : string, ?context : LDContext) =
        lp.SetProperty(LDCreativeWork.name, name, ?context = context)

    static member tryGetTextAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDCreativeWork.text, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getTextAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDCreativeWork.text, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `text` of object with @id `{lp.Id}` was not a string"
        | _ -> failwith $"Could not access property `text` of object with @id `{lp.Id}`"

    static member setTextAsString(lp : LDNode, text : string, ?context : LDContext) =
        lp.SetProperty(LDCreativeWork.text, text, ?context = context)

    static member getCitations(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldnode context = LDScholarlyArticle.validate(ldnode, ?context = context)
        lp.GetPropertyNodes(LDCreativeWork.citation, filter = filter, ?graph = graph, ?context = context)

    static member setCitations(lp : LDNode, citations : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDCreativeWork.citation, citations, ?context = context)

    static member getComments(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldnode context = LDComment.validate(ldnode, ?context = context)
        lp.GetPropertyNodes(LDCreativeWork.comment, filter = filter, ?graph = graph, ?context = context)

    static member setComments(lp : LDNode, comments : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDCreativeWork.comment, comments, ?context = context)

    //static member getMentions(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
    //    let filter ldnode context = DefinedTermSet.validate(ldnode, ?context = context)
    //    lp.GetPropertyNodes(Dataset.mentions, filter = filter, ?graph = graph, ?context = context)

    //static member setMentions(lp : LDNode, mentions : ResizeArray<LDNode>, ?context : LDContext) =
    //    lp.SetProperty(Dataset.mentions, mentions, ?context = context)

    static member tryGetUrlAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDCreativeWork.url, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getUrlAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDCreativeWork.url, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `url` of object with @id `{lp.Id}` was not a string"
        | _ -> failwith $"Could not access property `url` of object with @id `{lp.Id}`"

    static member setUrlAsString(lp : LDNode, url : string, ?context : LDContext) =
        lp.SetProperty(LDCreativeWork.url, url, ?context = context)

    static member getAbouts(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        lp.GetPropertyNodes(LDCreativeWork.about, ?graph = graph, ?context = context)

    static member getAboutsAsLabProcess(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldnode context = LDLabProcess.validate(ldnode, ?context = context)
        lp.GetPropertyNodes(LDCreativeWork.about, filter = filter, ?graph = graph, ?context = context)

    static member setAbouts(lp : LDNode, abouts : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDCreativeWork.about, abouts, ?context = context)

    //static member genIDLicense(identifier : string) =
    //    $"assays/{identifier}/"
    
    static member validate(lp : LDNode, ?context : LDContext) =
        lp.HasType(LDCreativeWork.schemaType, ?context = context)

    static member create(id : string, ?identifier : string, ?creators : ResizeArray<LDNode>, ?dateCreated : System.DateTime, ?datePublished : System.DateTime, ?dateModified : System.DateTime, ?description : string, ?hasParts : ResizeArray<LDNode>, ?name : string, ?text : string, ?citations : ResizeArray<LDNode>, ?comments : ResizeArray<LDNode>, ?mentions : ResizeArray<LDNode>, ?url : string, ?abouts : ResizeArray<LDNode>, ?context : LDContext) =
        let s = LDNode(id, ResizeArray [LDCreativeWork.schemaType], ?context = context)
        s.SetOptionalProperty(LDCreativeWork.identifier, identifier, ?context = context)
        s.SetOptionalProperty(LDCreativeWork.creator, creators, ?context = context)
        s.SetOptionalProperty(LDCreativeWork.dateCreated, dateCreated, ?context = context)
        s.SetOptionalProperty(LDCreativeWork.datePublished, datePublished, ?context = context)
        s.SetOptionalProperty(LDCreativeWork.dateModified, dateModified, ?context = context)
        s.SetOptionalProperty(LDCreativeWork.description, description, ?context = context)
        s.SetOptionalProperty(LDCreativeWork.hasPart, hasParts, ?context = context)
        s.SetOptionalProperty(LDCreativeWork.name, name, ?context = context)
        s.SetOptionalProperty(LDCreativeWork.text, text, ?context = context)
        s.SetOptionalProperty(LDCreativeWork.citation, citations, ?context = context)
        s.SetOptionalProperty(LDCreativeWork.comment, comments, ?context = context)
        s.SetOptionalProperty(LDCreativeWork.mentions, mentions, ?context = context)
        s.SetOptionalProperty(LDCreativeWork.url, url, ?context = context)
        s.SetOptionalProperty(LDCreativeWork.about, abouts, ?context = context)
        s