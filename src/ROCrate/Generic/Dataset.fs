namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
open ARCtrl.Helper

[<AttachMembers>]
type LDDataset =

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

    static member citation = "http://schema.org/citation"

    static member comment = "http://schema.org/comment"

    static member mentions = "http://schema.org/mentions"

    static member url = "http://schema.org/url"

    static member about = "http://schema.org/about"

    static member measurementMethod = "http://schema.org/measurementMethod"

    static member measurementTechnique = "http://schema.org/measurementTechnique"

    static member variableMeasured = "http://schema.org/variableMeasured"

    static member tryGetIdentifierAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.identifier, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getIdentifierAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.identifier, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `identifier` of object with @id `{lp.Id}` was not a string"
        | _ -> failwith $"Could not access property `identifier` of object with @id `{lp.Id}`"

    static member setIdentifierAsString(lp : LDNode, identifier : string, ?context : LDContext) =
        lp.SetProperty(LDDataset.identifier, identifier, ?context = context)

    static member getCreators(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter (p : LDNode) (ctx : LDContext option) = LDPerson.validate(p, ?context = ctx)
        lp.GetPropertyNodes(LDDataset.creator, filter = filter, ?graph = graph, ?context = context)

    //static member getCreatorsAsPerson(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
    //    let filter (p : LDNode) (ctx : LDContext option) = Person.validate(p, ?context = ctx)
    //    lp.GetPropertyNodes(Dataset.creator, filter = filter, ?graph = graph, ?context = context)

    static member setCreators(lp : LDNode, creators : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDDataset.creator, creators, ?context = context)

    static member tryGetDateCreatedAsDateTime(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.dateCreated, ?context = context) with
        | Some (:? System.DateTime as n) -> Some n
        | _ -> None

    static member setDateCreatedAsDateTime(lp : LDNode, dateCreated : System.DateTime, ?context : LDContext) =
        lp.SetProperty(LDDataset.dateCreated, dateCreated, ?context = context)

    static member tryGetDatePublishedAsDateTime(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.datePublished, ?context = context) with
        | Some (:? System.DateTime as n) -> Some n
        | _ -> None

    static member setDatePublishedAsDateTime(lp : LDNode, datePublished : System.DateTime, ?context : LDContext) =
        lp.SetProperty(LDDataset.datePublished, datePublished, ?context = context)

    static member tryGetSDDatePublishedAsDateTime(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.sdDatePublished, ?context = context) with
        | Some (:? System.DateTime as n) -> Some n
        | _ -> None

    static member setSDDatePublishedAsDateTime(lp : LDNode, sdDatePublished : System.DateTime, ?context : LDContext) =
        lp.SetProperty(LDDataset.sdDatePublished, sdDatePublished, ?context = context)

    static member tryGetLicenseAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.license, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    //static member tryGetLicenseAsCreativeWork(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
    //    match lp.TryGetPropertyAsSingleNode(Dataset.license, ?graph = graph, ?context = context) with
    //    | Some n when CreativeWork.validate(n, ?context = context) -> Some n
    //    | _ -> None

    static member setLicenseAsString(lp : LDNode, license : string, ?context : LDContext) =
        lp.SetProperty(LDDataset.license, license, ?context = context)

    static member setLicenseAsCreativeWork(lp : LDNode, license : obj, ?context : LDContext) =
        lp.SetProperty(LDDataset.license, license, ?context = context)

    static member tryGetDateModifiedAsDateTime(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.dateModified, ?context = context) with
        | Some (:? System.DateTime as n) -> Some n
        | _ -> None

    static member setDateModifiedAsDateTime(lp : LDNode, dateModified : System.DateTime, ?context : LDContext) =
        lp.SetProperty(LDDataset.dateModified, dateModified, ?context = context)

    static member tryGetDescriptionAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.description, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getDescriptionAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.description, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `description` of object with @id `{lp.Id}` was not a string"
        | _ -> failwith $"Could not access property `description` of object with @id `{lp.Id}`"

    static member setDescriptionAsString(lp : LDNode, description : string, ?context : LDContext) =
        lp.SetProperty(LDDataset.description, description, ?context = context)

    static member getHasParts(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        lp.GetPropertyNodes(LDDataset.hasPart, ?graph = graph, ?context = context)

    static member getHasPartsAsDataset(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldnode context = LDDataset.validate(ldnode, ?context = context)
        lp.GetPropertyNodes(LDDataset.hasPart, filter = filter, ?graph = graph, ?context = context)

    static member getHasPartsAsFile(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldnode context = LDFile.validate(ldnode, ?context = context)
        lp.GetPropertyNodes(LDDataset.hasPart, filter = filter, ?graph = graph, ?context = context)

    static member setHasParts(lp : LDNode, hasParts : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDDataset.hasPart, hasParts, ?context = context)

    static member tryGetHeadlineAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.headline, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member tryGetNameAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.name, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `name` of object with @id `{lp.Id}` was not a string"
        | _ -> failwith $"Could not access property `name` of object with @id `{lp.Id}`"

    static member setNameAsString(lp : LDNode, name : string, ?context : LDContext) =
        lp.SetProperty(LDDataset.name, name, ?context = context)

    static member getCitations(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldnode context = LDScholarlyArticle.validate(ldnode, ?context = context)
        lp.GetPropertyNodes(LDDataset.citation, filter = filter, ?graph = graph, ?context = context)

    static member setCitations(lp : LDNode, citations : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDDataset.citation, citations, ?context = context)

    static member getComments(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldnode context = LDComment.validate(ldnode, ?context = context)
        lp.GetPropertyNodes(LDDataset.comment, filter = filter, ?graph = graph, ?context = context)

    static member setComments(lp : LDNode, comments : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDDataset.comment, comments, ?context = context)

    //static member getMentions(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
    //    let filter ldnode context = DefinedTermSet.validate(ldnode, ?context = context)
    //    lp.GetPropertyNodes(Dataset.mentions, filter = filter, ?graph = graph, ?context = context)

    //static member setMentions(lp : LDNode, mentions : ResizeArray<LDNode>, ?context : LDContext) =
    //    lp.SetProperty(Dataset.mentions, mentions, ?context = context)

    static member tryGetUrlAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.url, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getUrlAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.url, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `url` of object with @id `{lp.Id}` was not a string"
        | _ -> failwith $"Could not access property `url` of object with @id `{lp.Id}`"

    static member setUrlAsString(lp : LDNode, url : string, ?context : LDContext) =
        lp.SetProperty(LDDataset.url, url, ?context = context)

    static member getAbouts(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        lp.GetPropertyNodes(LDDataset.about, ?graph = graph, ?context = context)

    static member getAboutsAsLabProcess(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldnode context = LDLabProcess.validate(ldnode, ?context = context)
        lp.GetPropertyNodes(LDDataset.about, filter = filter, ?graph = graph, ?context = context)

    static member setAbouts(lp : LDNode, abouts : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDDataset.about, abouts, ?context = context)

    static member tryGetMeasurementMethodAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.measurementMethod, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member tryGetMeasurementMethodAsDefinedTerm(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleNode(LDDataset.measurementMethod, ?graph = graph, ?context = context) with
        | Some n when LDDefinedTerm.validate(n, ?context = context) -> Some n
        | _ -> None

    static member setMeasurementMethodAsString(lp : LDNode, measurementMethod : string, ?context : LDContext) =
        lp.SetProperty(LDDataset.measurementMethod, measurementMethod, ?context = context)

    static member setMeasurementMethodAsDefinedTerm(lp : LDNode, measurementMethod : LDNode, ?context : LDContext) =
        lp.SetProperty(LDDataset.measurementMethod, measurementMethod, ?context = context)

    static member tryGetMeasurementTechniqueAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.measurementTechnique, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member tryGetMeasurementTechniqueAsDefinedTerm(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleNode(LDDataset.measurementTechnique, ?graph = graph, ?context = context) with
        | Some n when LDDefinedTerm.validate(n, ?context = context) -> Some n
        | _ -> None
        

    static member setMeasurementTechniqueAsString(lp : LDNode, measurementTechnique : string, ?context : LDContext) =
        lp.SetProperty(LDDataset.measurementTechnique, measurementTechnique, ?context = context)

    static member setMeasurementTechniqueAsDefinedTerm(lp : LDNode, measurementTechnique : LDNode, ?context : LDContext) =
        lp.SetProperty(LDDataset.measurementTechnique, measurementTechnique, ?context = context)

    static member tryGetVariableMeasuredAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDDataset.variableMeasured, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member tryGetVariableMeasuredAsPropertyValue(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleNode(LDDataset.variableMeasured, ?graph = graph, ?context = context) with
        | Some n when LDPropertyValue.validate(n, ?context = context) -> Some n
        | _ -> None

    static member tryGetVariableMeasuredAsMeasurementType(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldnode context = LDPropertyValue.validateFragmentDescriptor(ldnode, ?context = context) |> not
        lp.GetPropertyNodes(LDDataset.variableMeasured, ?graph = graph, ?context = context)
        |> ResizeArray.tryPick (fun n -> if filter n context then Some n else None)

    static member getVariableMeasuredAsFragmentDescriptors(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldnode context = LDPropertyValue.validateFragmentDescriptor(ldnode, ?context = context)
        lp.GetPropertyNodes(LDDataset.variableMeasured, filter = filter, ?graph = graph, ?context = context)

    static member setVariableMeasuredAsString(lp : LDNode, variableMeasured : string, ?context : LDContext) =
        lp.SetProperty(LDDataset.variableMeasured, variableMeasured, ?context = context)

    static member setVariableMeasuredAsPropertyValue(lp : LDNode, variableMeasured : LDNode, ?context : LDContext) =
        lp.SetProperty(LDDataset.variableMeasured, variableMeasured, ?context = context)

    static member setVariableMeasuredAsPropertyValues(lp : LDNode, variableMeasured : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDDataset.variableMeasured, variableMeasured, ?context = context)

    static member genIDInvesigation() =
        "./"

    static member genIDStudy(identifier : string) =
        $"studies/{identifier}/"

    static member genIDAssay(identifier : string) =
        $"assays/{identifier}/"
    
    static member validate(lp : LDNode, ?context : LDContext) =
        lp.HasType(LDDataset.schemaType, ?context = context)

    static member validateInvestigation(lp : LDNode, ?context : LDContext) =
        LDDataset.validate(lp, ?context = context)
        && lp.AdditionalType.Contains("Investigation")

    static member validateStudy (lp : LDNode, ?context : LDContext) =
        LDDataset.validate(lp, ?context = context)
        && lp.AdditionalType.Contains("Study")

    static member validateAssay (lp : LDNode, ?context : LDContext) =
        LDDataset.validate(lp, ?context = context)
        && lp.AdditionalType.Contains("Assay")

    static member create(id : string, ?identier : string, ?creators : ResizeArray<LDNode>, ?dateCreated : System.DateTime, ?datePublished : System.DateTime, ?dateModified : System.DateTime, ?description : string, ?hasParts : ResizeArray<LDNode>, ?name : string, ?citations : ResizeArray<LDNode>, ?comments : ResizeArray<LDNode>, ?mentions : ResizeArray<LDNode>, ?url : string, ?abouts : ResizeArray<LDNode>, ?measurementMethod : LDNode, ?measurementTechnique : LDNode, ?variableMeasureds : ResizeArray<LDNode>, ?context : LDContext) =
        let s = LDNode(id, ResizeArray [LDDataset.schemaType], ?context = context)
        s.SetOptionalProperty(LDDataset.identifier, identier, ?context = context)
        s.SetOptionalProperty(LDDataset.creator, creators, ?context = context)
        s.SetOptionalProperty(LDDataset.dateCreated, dateCreated, ?context = context)
        s.SetOptionalProperty(LDDataset.datePublished, datePublished, ?context = context)
        s.SetOptionalProperty(LDDataset.dateModified, dateModified, ?context = context)
        s.SetOptionalProperty(LDDataset.description, description, ?context = context)
        s.SetOptionalProperty(LDDataset.hasPart, hasParts, ?context = context)
        s.SetOptionalProperty(LDDataset.name, name, ?context = context)
        s.SetOptionalProperty(LDDataset.citation, citations, ?context = context)
        s.SetOptionalProperty(LDDataset.comment, comments, ?context = context)
        s.SetOptionalProperty(LDDataset.mentions, mentions, ?context = context)
        s.SetOptionalProperty(LDDataset.url, url, ?context = context)
        s.SetOptionalProperty(LDDataset.about, abouts, ?context = context)
        s.SetOptionalProperty(LDDataset.measurementMethod, measurementMethod, ?context = context)
        s.SetOptionalProperty(LDDataset.measurementTechnique, measurementTechnique, ?context = context)
        s.SetOptionalProperty(LDDataset.variableMeasured, variableMeasureds, ?context = context)
        s

    static member createInvestigation(identifier : string, name : string, ?id : string, ?creators : ResizeArray<LDNode>, ?dateCreated : System.DateTime, ?datePublished : System.DateTime, ?dateModified : System.DateTime, ?description : string, ?hasParts : ResizeArray<LDNode>, ?citations : ResizeArray<LDNode>, ?comments : ResizeArray<LDNode>, ?mentions : ResizeArray<LDNode>, ?url : string, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> LDDataset.genIDInvesigation()
        let s = LDDataset.create(id, identier = identifier, ?creators = creators, ?dateCreated = dateCreated, ?datePublished = datePublished, ?dateModified = dateModified, ?description = description, ?hasParts = hasParts, name = name, ?citations = citations, ?comments = comments, ?mentions = mentions, ?url = url, ?context = context)
        s.AdditionalType <- ResizeArray ["Investigation"]
        s

    static member createStudy(identifier : string, ?id : string, ?creators : ResizeArray<LDNode>, ?dateCreated : System.DateTime, ?datePublished : System.DateTime, ?dateModified : System.DateTime, ?description : string, ?hasParts : ResizeArray<LDNode>, ?name : string, ?citations : ResizeArray<LDNode>, ?variableMeasureds : ResizeArray<LDNode>, ?comments : ResizeArray<LDNode>, ?url : string, ?abouts : ResizeArray<LDNode>, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> LDDataset.genIDStudy(identifier)
        let s = LDDataset.create(id, identier = identifier, ?creators = creators, ?dateCreated = dateCreated, ?datePublished = datePublished, ?dateModified = dateModified, ?description = description, ?hasParts = hasParts, ?name = name, ?citations = citations, ?variableMeasureds = variableMeasureds, ?comments = comments, ?url = url, ?abouts = abouts, ?context = context)
        s.AdditionalType <- ResizeArray ["Study"]
        s

    static member createAssay(identifier : string, ?id : string, ?name : string, ?description : string, ?creators : ResizeArray<LDNode>, ?hasParts : ResizeArray<LDNode>, ?measurementMethod : LDNode, ?measurementTechnique : LDNode, ?variableMeasureds : ResizeArray<LDNode>, ?abouts : ResizeArray<LDNode>, ?comments : ResizeArray<LDNode>, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> LDDataset.genIDAssay(identifier)
        let s = LDDataset.create(id, identier = identifier, ?name = name, ?description = description, ?creators = creators, ?hasParts = hasParts, ?measurementMethod = measurementMethod, ?measurementTechnique = measurementTechnique, ?variableMeasureds = variableMeasureds, ?abouts = abouts, ?comments = comments, ?context = context)
        s.AdditionalType <- ResizeArray ["Assay"]
        s
