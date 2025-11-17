namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
open ARCtrl.Helper

[<AttachMembers>]
type LDSoftwareSourceCode =

    static member schemaType = "http://schema.org/SoftwareSourceCode"
    // Recommended properties
    static member creator = "http://schema.org/creator"
    static member dateCreated = "http://schema.org/dateCreated"
    static member license = "http://schema.org/license"
    static member name = "http://schema.org/name"
    static member programmingLanguage = "http://schema.org/programmingLanguage"
    static member sdPublisher = "http://schema.org/sdPublisher"
    static member url = "http://schema.org/url"
    static member version = "http://schema.org/version"
    // Optional properties
    static member description = "http://schema.org/description"
    static member hasPart = "http://schema.org/hasPart"
    static member comment = "http://schema.org/comment"

    // Getters and setters for recommended properties
    static member getCreator(ssc : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDPerson.validate(ldObject, ?context = context) || LDOrganization.validate(ldObject, ?context = context)
        ssc.GetPropertyNodes(LDSoftwareSourceCode.creator, filter = filter, ?graph = graph, ?context = context)

    static member setCreator(ssc : LDNode, creator : LDNode, ?context : LDContext) =
        ssc.SetProperty(LDSoftwareSourceCode.creator, creator, ?context = context)
    static member tryGetDateCreated(ssc : LDNode, ?context : LDContext) =
        match ssc.TryGetPropertyAsSingleton(LDSoftwareSourceCode.dateCreated, ?context = context) with
        | Some (:? string as d) -> Some d
        | _ -> None
    static member setDateCreated(ssc : LDNode, date : string, ?context : LDContext) =
        ssc.SetProperty(LDSoftwareSourceCode.dateCreated, date, ?context = context)

    static member getLicenses(ssc : LDNode, ?graph : LDGraph, ?context : LDContext) =
        ssc.GetPropertyNodes(LDSoftwareSourceCode.license, ?graph = graph, ?context = context)
    static member setLicenses(ssc : LDNode, licenses : ResizeArray<LDNode>, ?context : LDContext) =
        ssc.SetProperty(LDSoftwareSourceCode.license, licenses, ?context = context)

    static member tryGetNameAsString(ssc : LDNode, ?context : LDContext) =
        match ssc.TryGetPropertyAsSingleton(LDSoftwareSourceCode.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member setNameAsString(ssc : LDNode, name : string, ?context : LDContext) =
        ssc.SetProperty(LDSoftwareSourceCode.name, name, ?context = context)

    static member getProgrammingLanguages(ssc : LDNode, ?graph : LDGraph, ?context : LDContext) =
        ssc.GetPropertyNodes(LDSoftwareSourceCode.programmingLanguage, ?graph = graph, ?context = context)

    static member setProgrammingLanguages(ssc : LDNode, programmingLanguages : ResizeArray<LDNode>, ?context : LDContext) =
        ssc.SetProperty(LDSoftwareSourceCode.programmingLanguage, programmingLanguages, ?context = context)

    static member tryGetSdPublisher(ssc : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDPerson.validate(ldObject, ?context = context) || LDOrganization.validate(ldObject, ?context = context)
        match ssc.TryGetPropertyAsSingleNode(LDSoftwareSourceCode.sdPublisher, ?graph = graph, ?context = context) with
        | Some a when filter a context -> Some a
        | _ -> None

    static member setSdPublisher(ssc : LDNode, sdPublisher : LDNode, ?context : LDContext) =
        ssc.SetProperty(LDSoftwareSourceCode.sdPublisher, sdPublisher, ?context = context)

    static member tryGetUrl(ssc : LDNode, ?context : LDContext) =
        match ssc.TryGetPropertyAsSingleton(LDSoftwareSourceCode.url, ?context = context) with
        | Some (:? string as u) -> Some u
        | _ -> None

    static member setUrl(ssc : LDNode, url : string, ?context : LDContext) =
        ssc.SetProperty(LDSoftwareSourceCode.url, url, ?context = context)

    static member tryGetVersion(ssc : LDNode, ?context : LDContext) =
        match ssc.TryGetPropertyAsSingleton(LDSoftwareSourceCode.version, ?context = context) with
        | Some (:? string as v) -> Some v
        | _ -> None

    static member setVersion(ssc : LDNode, version : string, ?context : LDContext) =
        ssc.SetProperty(LDSoftwareSourceCode.version, version, ?context = context)

    static member tryGetDescription(ssc : LDNode, ?context : LDContext) =
        match ssc.TryGetPropertyAsSingleton(LDSoftwareSourceCode.description, ?context = context) with
        | Some (:? string as d) -> Some d
        | _ -> None

    static member setDescription(ssc : LDNode, description : string, ?context : LDContext) =
        ssc.SetProperty(LDSoftwareSourceCode.description, description, ?context = context)

    static member getHasPart(ssc : LDNode, ?graph : LDGraph, ?context : LDContext) =
        ssc.GetPropertyNodes(LDSoftwareSourceCode.hasPart, ?graph = graph, ?context = context)

    static member setHasPart(ssc : LDNode, hasParts : string list, ?context : LDContext) =
        ssc.SetProperty(LDSoftwareSourceCode.hasPart, ResizeArray hasParts, ?context = context)

    static member getComments(ssc : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDComment.validate(ldObject, ?context = context)
        ssc.GetPropertyNodes(LDSoftwareSourceCode.comment, filter = filter, ?graph = graph, ?context = context)

    static member setComments(ssc : LDNode, comments : ResizeArray<LDNode>, ?context : LDContext) =
        ssc.SetProperty(LDSoftwareSourceCode.comment, comments, ?context = context)

    static member validate(lp : LDNode, ?context : LDContext) =
        lp.HasType(LDSoftwareSourceCode.schemaType, ?context = context)

    static member create
        (
            ?id : string,
            ?creator : LDNode,
            ?dateCreated : string,
            ?licenses : ResizeArray<LDNode>,
            ?name : string,
            ?programmingLanguages : ResizeArray<LDNode>,
            ?sdPublisher : LDNode,
            ?url : string,
            ?version : string,
            ?description : string,
            ?hasParts : ResizeArray<LDNode>,
            ?comments : ResizeArray<LDNode>,
            ?context : LDContext
        ) =
        let id =
            match id with
            | Some i -> i
            | None -> $"#ComputationalWorkflow_{ARCtrl.Helper.Identifier.createMissingIdentifier()}" |> Helper.ID.clean
        let ssc = LDNode(id, ResizeArray [LDSoftwareSourceCode.schemaType], ?context = context)
        ssc.SetOptionalProperty(LDSoftwareSourceCode.creator, creator, ?context = context)
        ssc.SetOptionalProperty(LDSoftwareSourceCode.dateCreated, dateCreated, ?context = context)
        ssc.SetOptionalProperty(LDSoftwareSourceCode.license, licenses, ?context = context)
        ssc.SetOptionalProperty(LDSoftwareSourceCode.name, name, ?context = context)
        ssc.SetOptionalProperty(LDSoftwareSourceCode.programmingLanguage, programmingLanguages, ?context = context)
        ssc.SetOptionalProperty(LDSoftwareSourceCode.sdPublisher, sdPublisher, ?context = context)
        ssc.SetOptionalProperty(LDSoftwareSourceCode.url, url, ?context = context)
        ssc.SetOptionalProperty(LDSoftwareSourceCode.version, version, ?context = context)
        ssc.SetOptionalProperty(LDSoftwareSourceCode.description, description, ?context = context)
        ssc.SetOptionalProperty(LDSoftwareSourceCode.hasPart, hasParts, ?context = context)
        ssc.SetOptionalProperty(LDSoftwareSourceCode.comment, comments, ?context = context)
        ssc
    