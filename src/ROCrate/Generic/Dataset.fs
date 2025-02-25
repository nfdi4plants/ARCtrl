namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
open ARCtrl.Helper

[<AttachMembers>]
type Dataset =

    static member schemaType = "http://schema.org/Dataset"

    static member additionalType = "http://schema.org/Dataset"

    static member identifier = "http://schema.org/identifier"

    static member creator = "http://schema.org/creator"

    static member dateCreated = "http://schema.org/dateCreated"

    static member datePublished = "http://schema.org/datePublished"

    static member dateModified = "http://schema.org/datemodified"

    static member description = "http://schema.org/description"

    static member hasPart = "http://schema.org/hasPart"

    static member name = "http://schema.org/name"

    static member citation = "http://schema.org/citation"

    static member comment = "http://schema.org/comment"

    static member mentions = "http://schema.org/mentions"

    static member url = "http://schema.org/url"

    static member about = "http://schema.org/about"

    static member measurementMethod = "http://schema.org/measurementMethod"

    static member measurementTechnique = "http://schema.org/measurementTechnique"

    static member variableMeasured = "http://schema.org/variableMeasured"

    static member tryGetIdentifierAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.identifier, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getIdentifierAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.identifier, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `identifier` of object with @id `{lp.Id}` was not a string"
        | _ -> failwith $"Could not access property `identifier` of object with @id `{lp.Id}`"

    static member setIdentifierAsString(lp : LDNode, identifier : string, ?context : LDContext) =
        lp.SetProperty(Dataset.identifier, identifier, ?context = context)

    static member getCreators(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter (p : LDNode) (ctx : LDContext option) = Person.validate(p, ?context = ctx)
        lp.GetPropertyNodes(Dataset.creator, filter = filter, ?graph = graph, ?context = context)

    //static member getCreatorsAsPerson(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
    //    let filter (p : LDNode) (ctx : LDContext option) = Person.validate(p, ?context = ctx)
    //    lp.GetPropertyNodes(Dataset.creator, filter = filter, ?graph = graph, ?context = context)

    static member setCreators(lp : LDNode, creators : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(Dataset.creator, creators, ?context = context)

    static member tryGetDateCreatedAsDateTime(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.dateCreated, ?context = context) with
        | Some (:? System.DateTime as n) -> Some n
        | _ -> None

    static member setDateCreatedAsDateTime(lp : LDNode, dateCreated : System.DateTime, ?context : LDContext) =
        lp.SetProperty(Dataset.dateCreated, dateCreated, ?context = context)

    static member tryGetDatePublishedAsDateTime(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.datePublished, ?context = context) with
        | Some (:? System.DateTime as n) -> Some n
        | _ -> None

    static member setDatePublishedAsDateTime(lp : LDNode, datePublished : System.DateTime, ?context : LDContext) =
        lp.SetProperty(Dataset.datePublished, datePublished, ?context = context)

    static member tryGetDateModifiedAsDateTime(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.dateModified, ?context = context) with
        | Some (:? System.DateTime as n) -> Some n
        | _ -> None

    static member setDateModifiedAsDateTime(lp : LDNode, dateModified : System.DateTime, ?context : LDContext) =
        lp.SetProperty(Dataset.dateModified, dateModified, ?context = context)

    static member tryGetDescriptionAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.description, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getDescriptionAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.description, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `description` of object with @id `{lp.Id}` was not a string"
        | _ -> failwith $"Could not access property `description` of object with @id `{lp.Id}`"

    static member setDescriptionAsString(lp : LDNode, description : string, ?context : LDContext) =
        lp.SetProperty(Dataset.description, description, ?context = context)

    static member getHasParts(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        lp.GetPropertyNodes(Dataset.hasPart, ?graph = graph, ?context = context)

    static member getHasPartsAsDataset(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = Dataset.validate(ldObject, ?context = context)
        lp.GetPropertyNodes(Dataset.hasPart, filter = filter, ?graph = graph, ?context = context)

    static member getHasPartsAsFile(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = File.validate(ldObject, ?context = context)
        lp.GetPropertyNodes(Dataset.hasPart, filter = filter, ?graph = graph, ?context = context)

    static member setHasParts(lp : LDNode, hasParts : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(Dataset.hasPart, hasParts, ?context = context)

    static member tryGetNameAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.name, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `name` of object with @id `{lp.Id}` was not a string"
        | _ -> failwith $"Could not access property `name` of object with @id `{lp.Id}`"

    static member setNameAsString(lp : LDNode, name : string, ?context : LDContext) =
        lp.SetProperty(Dataset.name, name, ?context = context)

    static member getCitations(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = ScholarlyArticle.validate(ldObject, ?context = context)
        lp.GetPropertyNodes(Dataset.citation, filter = filter, ?graph = graph, ?context = context)

    static member setCitations(lp : LDNode, citations : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(Dataset.citation, citations, ?context = context)

    static member getComments(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = Comment.validate(ldObject, ?context = context)
        lp.GetPropertyNodes(Dataset.comment, filter = filter, ?graph = graph, ?context = context)

    static member setComments(lp : LDNode, comments : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(Dataset.comment, comments, ?context = context)

    //static member getMentions(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
    //    let filter ldObject context = DefinedTermSet.validate(ldObject, ?context = context)
    //    lp.GetPropertyNodes(Dataset.mentions, filter = filter, ?graph = graph, ?context = context)

    //static member setMentions(lp : LDNode, mentions : ResizeArray<LDNode>, ?context : LDContext) =
    //    lp.SetProperty(Dataset.mentions, mentions, ?context = context)

    static member tryGetUrlAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.url, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getUrlAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.url, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `url` of object with @id `{lp.Id}` was not a string"
        | _ -> failwith $"Could not access property `url` of object with @id `{lp.Id}`"

    static member setUrlAsString(lp : LDNode, url : string, ?context : LDContext) =
        lp.SetProperty(Dataset.url, url, ?context = context)

    static member getAbouts(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        lp.GetPropertyNodes(Dataset.about, ?graph = graph, ?context = context)

    static member getAboutsAsLabProcess(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LabProcess.validate(ldObject, ?context = context)
        lp.GetPropertyNodes(Dataset.about, filter = filter, ?graph = graph, ?context = context)

    static member setAbouts(lp : LDNode, abouts : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(Dataset.about, abouts, ?context = context)

    static member tryGetMeasurementMethodAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.measurementMethod, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member tryGetMeasurementMethodAsDefinedTerm(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleNode(Dataset.measurementMethod, ?graph = graph, ?context = context) with
        | Some n when DefinedTerm.validate(n, ?context = context) -> Some n
        | _ -> None

    static member setMeasurementMethodAsString(lp : LDNode, measurementMethod : string, ?context : LDContext) =
        lp.SetProperty(Dataset.measurementMethod, measurementMethod, ?context = context)

    static member setMeasurementMethodAsDefinedTerm(lp : LDNode, measurementMethod : LDNode, ?context : LDContext) =
        lp.SetProperty(Dataset.measurementMethod, measurementMethod, ?context = context)

    static member tryGetMeasurementTechniqueAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.measurementTechnique, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member tryGetMeasurementTechniqueAsDefinedTerm(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleNode(Dataset.measurementTechnique, ?graph = graph, ?context = context) with
        | Some n when DefinedTerm.validate(n, ?context = context) -> Some n
        | _ -> None
        

    static member setMeasurementTechniqueAsString(lp : LDNode, measurementTechnique : string, ?context : LDContext) =
        lp.SetProperty(Dataset.measurementTechnique, measurementTechnique, ?context = context)

    static member setMeasurementTechniqueAsDefinedTerm(lp : LDNode, measurementTechnique : LDNode, ?context : LDContext) =
        lp.SetProperty(Dataset.measurementTechnique, measurementTechnique, ?context = context)

    static member tryGetVariableMeasuredAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.variableMeasured, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member tryGetVariableMeasuredAsPropertyValue(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleNode(Dataset.variableMeasured, ?graph = graph, ?context = context) with
        | Some n when PropertyValue.validate(n, ?context = context) -> Some n
        | _ -> None

    static member setVariableMeasuredAsString(lp : LDNode, variableMeasured : string, ?context : LDContext) =
        lp.SetProperty(Dataset.variableMeasured, variableMeasured, ?context = context)

    static member setVariableMeasuredAsPropertyValue(lp : LDNode, variableMeasured : LDNode, ?context : LDContext) =
        lp.SetProperty(Dataset.variableMeasured, variableMeasured, ?context = context)

    static member genIDInvesigation() =
        "./"

    static member genIDStudy(identifier : string) =
        $"Study_{identifier}"

    static member genIDAssay(identifier : string) =
        $"Assay_{identifier}"
    
    static member validate(lp : LDNode, ?context : LDContext) =
        lp.HasType(Dataset.schemaType, ?context = context)

    static member validateInvestigation(lp : LDNode, ?context : LDContext) =
        Dataset.validate(lp, ?context = context)
        && lp.AdditionalType.Contains("Investigation")

    static member validateStudy (lp : LDNode, ?context : LDContext) =
        Dataset.validate(lp, ?context = context)
        && lp.AdditionalType.Contains("Study")

    static member validateAssay (lp : LDNode, ?context : LDContext) =
        Dataset.validate(lp, ?context = context)
        && lp.AdditionalType.Contains("Assay")

    static member create(id : string, ?identier : string, ?creators : ResizeArray<LDNode>, ?dateCreated : System.DateTime, ?datePublished : System.DateTime, ?dateModified : System.DateTime, ?description : string, ?hasParts : ResizeArray<LDNode>, ?name : string, ?citations : ResizeArray<LDNode>, ?comments : ResizeArray<LDNode>, ?mentions : ResizeArray<LDNode>, ?url : string, ?abouts : ResizeArray<LDNode>, ?measurementMethod : LDNode, ?measurementTechnique : LDNode, ?variableMeasured : LDNode, ?context : LDContext) =
        let s = LDNode(id, ResizeArray [Dataset.schemaType], ?context = context)
        s.SetOptionalProperty(Dataset.identifier, identier, ?context = context)
        s.SetOptionalProperty(Dataset.creator, creators, ?context = context)
        s.SetOptionalProperty(Dataset.dateCreated, dateCreated, ?context = context)
        s.SetOptionalProperty(Dataset.datePublished, datePublished, ?context = context)
        s.SetOptionalProperty(Dataset.dateModified, dateModified, ?context = context)
        s.SetOptionalProperty(Dataset.description, description, ?context = context)
        s.SetOptionalProperty(Dataset.hasPart, hasParts, ?context = context)
        s.SetOptionalProperty(Dataset.name, name, ?context = context)
        s.SetOptionalProperty(Dataset.citation, citations, ?context = context)
        s.SetOptionalProperty(Dataset.comment, comments, ?context = context)
        s.SetOptionalProperty(Dataset.mentions, mentions, ?context = context)
        s.SetOptionalProperty(Dataset.url, url, ?context = context)
        s.SetOptionalProperty(Dataset.about, abouts, ?context = context)
        s.SetOptionalProperty(Dataset.measurementMethod, measurementMethod, ?context = context)
        s.SetOptionalProperty(Dataset.measurementTechnique, measurementTechnique, ?context = context)
        s.SetOptionalProperty(Dataset.variableMeasured, variableMeasured, ?context = context)
        s

    static member createInvestigation(identifier : string, name : string, ?id : string, ?creators : ResizeArray<LDNode>, ?dateCreated : System.DateTime, ?datePublished : System.DateTime, ?dateModified : System.DateTime, ?description : string, ?hasParts : ResizeArray<LDNode>, ?citations : ResizeArray<LDNode>, ?comments : ResizeArray<LDNode>, ?mentions : ResizeArray<LDNode>, ?url : string, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> Dataset.genIDInvesigation()
        let s = Dataset.create(id, identier = identifier, ?creators = creators, ?dateCreated = dateCreated, ?datePublished = datePublished, ?dateModified = dateModified, ?description = description, ?hasParts = hasParts, name = name, ?citations = citations, ?comments = comments, ?mentions = mentions, ?url = url, ?context = context)
        s.AdditionalType <- ResizeArray ["Investigation"]
        s

    static member createStudy(identifier : string, ?id : string, ?creators : ResizeArray<LDNode>, ?dateCreated : System.DateTime, ?datePublished : System.DateTime, ?dateModified : System.DateTime, ?description : string, ?hasParts : ResizeArray<LDNode>, ?name : string, ?citations : ResizeArray<LDNode>, ?comments : ResizeArray<LDNode>, ?url : string, ?abouts : ResizeArray<LDNode>, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> Dataset.genIDStudy(identifier)
        let s = Dataset.create(id, identier = identifier, ?creators = creators, ?dateCreated = dateCreated, ?datePublished = datePublished, ?dateModified = dateModified, ?description = description, ?hasParts = hasParts, ?name = name, ?citations = citations, ?comments = comments, ?url = url, ?abouts = abouts, ?context = context)
        s.AdditionalType <- ResizeArray ["Study"]
        s

    static member createAssay(identifier : string, ?id : string, ?description : string, ?creators : ResizeArray<LDNode>, ?hasParts : ResizeArray<LDNode>, ?measurementMethod : LDNode, ?measurementTechnique : LDNode, ?variableMeasured : LDNode, ?abouts : ResizeArray<LDNode>, ?comments : ResizeArray<LDNode>, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> Dataset.genIDAssay(identifier)
        let s = Dataset.create(id, identier = identifier, ?description = description, ?creators = creators, ?hasParts = hasParts, ?measurementMethod = measurementMethod, ?measurementTechnique = measurementTechnique, ?variableMeasured = variableMeasured, ?abouts = abouts, ?comments = comments, ?context = context)
        s.AdditionalType <- ResizeArray ["Assay"]
        s
