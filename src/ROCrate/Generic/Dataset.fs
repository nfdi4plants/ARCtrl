namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
open ARCtrl.Helper

//Investigation
//Is based upon schema.org/Dataset and maps to the ISA-JSON Investigation

//Property	Required	Expected Type	Description
//@id	MUST	Text or URL	Should be “./”, the investigation object represents the root data entity.
//@type	MUST	Text	must be 'schema.org/Dataset'
//additionalType	MUST	Text or URL	‘Investigation’ or ontology term to identify it as an Investigation
//identifier	MUST	Text or URL	Identifying descriptor of the investigation (e.g. repository name).
//creator	SHOULD	schema.org/Person	The creator(s)/authors(s)/owner(s)/PI(s) of the investigation.
//dateCreated	SHOULD	DateTime	When the Investigation was created
//datePublished	SHOULD	DateTime	When the Investigation was published
//description	SHOULD	Text	A description of the investigation (e.g. an abstract).
//hasPart	SHOULD	schema.org/Dataset (Study)	An Investigation object should contain other datasets representing the studies of the investigation. They must follow the Study profile.
//headline	SHOULD	Text	A title of the investigation (e.g. a paper title).
//citation	COULD	schema.org/ScholarlyArticle	Publications corresponding with this investigation.
//comment	COULD	schema.org/Comment	Comment
//dateModified	COULD	DateTime	When the Investigation was last modified
//mentions	COULD	schema.org/DefinedTermSet	Ontologies referenced in this investigation.
//url	COULD	URL	The filename or path of the metadata file describing the investigation. Optional, since in some contexts like an ARC the filename is implicit.
//Study
//Is based upon schema.org/Dataset and maps to the ISA-JSON Study

//Property	Required	Expected Type	Description
//@id	MUST	Text or URL	Should be a subdirectory corresponding to this study.
//@type	MUST	Text	must be 'schema.org/Dataset'
//additionalType	MUST	Text or URL	‘Study’ or ontology term to identify it as a Study
//identifier	MUST	Text or URL	Identifying descriptor of the study.
//about	SHOULD	bioschemas.org/LabProcess	The experimental processes performed in this study.
//creator	SHOULD	schema.org/Person	The performer of the study.
//dateCreated	SHOULD	DateTime	When the Study was created
//datePublished	SHOULD	DateTime	When the Study was published
//description	SHOULD	Text	A short description of the study (e.g. an abstract).
//hasPart	SHOULD	schema.org/Dataset (Assay) or File	Assays contained in this study or actual data files resulting from the process sequence.
//headline	SHOULD	Text	A title of the study.
//citation	COULD	schema.org/ScholarlyArticle	A publication corresponding to the study.
//comment	COULD	schema.org/Comment	Comment
//dateModified	COULD	DateTime	When the Study was last modified
//url	COULD	URL	The filename or path of the metadata file describing the study. Optional, since in some contexts like an ARC the filename is implicit.
//Assay
//Is based upon schema.org/Dataset and maps to the ISA-JSON Assay

//Property	Required	Expected Type	Description
//@id	MUST	Text or URL	Should be a subdirectory corresponding to this assay.
//@type	MUST	Text	must be 'schema.org/Dataset'
//additionalType	MUST	Text or URL	‘Assay’ or ontology term to identify it as an Assay
//identifier	MUST	Text or URL	Identifying descriptor of the assay.
//about	SHOULD	bioschemas.org/LabProcess	The experimental processes performed in this assay.
//creator	SHOULD	schema.org/Person	The performer of the experiments.
//hasPart	SHOULD	File	The data files resulting from the process sequence
//measurementMethod	SHOULD	URL or schema.org/DefinedTerm	Describes the type measurement e.g Complexomics or transcriptomics as an ontology term
//measurementTechnique	SHOULD	URL or schema.org/DefinedTerm	Describes the type of technology used to take the measurement, e.g mass spectrometry or deep sequencing
//comment	COULD	schema.org/Comment	Comment
//url	COULD	URL	The filename or path of the metadata file describing the assay. Optional, since in some contexts like an ARC the filename is implicit.
//variableMeasured	COULD	Text or schema.org/PropertyValue	The target variable being measured E.g protein concentration

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

    static member headline = "http://schema.org/headline"

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

    static member tryGetHeadlineAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.headline, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getHeadlineAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.headline, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `headline` of object with @id `{lp.Id}` was not a string"
        | _ -> failwith $"Could not access property `headline` of object with @id `{lp.Id}`"

    static member setHeadlineAsString(lp : LDNode, headline : string, ?context : LDContext) =
        lp.SetProperty(Dataset.headline, headline, ?context = context)

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

    static member tryGetVariableMeasuredAsPropertyValue(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(Dataset.variableMeasured, ?context = context) with
        | Some (:? LDNode as n) -> Some n
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

    static member create(id : string, ?identier : string, ?creators : ResizeArray<LDNode>, ?dateCreated : System.DateTime, ?datePublished : System.DateTime, ?dateModified : System.DateTime, ?description : string, ?hasParts : ResizeArray<LDNode>, ?headline : string, ?citations : ResizeArray<LDNode>, ?comments : ResizeArray<LDNode>, ?mentions : ResizeArray<LDNode>, ?url : string, ?abouts : ResizeArray<LDNode>, ?measurementMethod : string, ?measurementTechnique : string, ?variableMeasured : string, ?context : LDContext) =
        let s = LDNode(id, ResizeArray [Dataset.schemaType], ?context = context)
        s.SetOptionalProperty(Dataset.identifier, identier, ?context = context)
        s.SetOptionalProperty(Dataset.creator, creators, ?context = context)
        s.SetOptionalProperty(Dataset.dateCreated, dateCreated, ?context = context)
        s.SetOptionalProperty(Dataset.datePublished, datePublished, ?context = context)
        s.SetOptionalProperty(Dataset.dateModified, dateModified, ?context = context)
        s.SetOptionalProperty(Dataset.description, description, ?context = context)
        s.SetOptionalProperty(Dataset.hasPart, hasParts, ?context = context)
        s.SetOptionalProperty(Dataset.headline, headline, ?context = context)
        s.SetOptionalProperty(Dataset.citation, citations, ?context = context)
        s.SetOptionalProperty(Dataset.comment, comments, ?context = context)
        s.SetOptionalProperty(Dataset.mentions, mentions, ?context = context)
        s.SetOptionalProperty(Dataset.url, url, ?context = context)
        s.SetOptionalProperty(Dataset.about, abouts, ?context = context)
        s.SetOptionalProperty(Dataset.measurementMethod, measurementMethod, ?context = context)
        s.SetOptionalProperty(Dataset.measurementTechnique, measurementTechnique, ?context = context)
        s.SetOptionalProperty(Dataset.variableMeasured, variableMeasured, ?context = context)
        s

    static member createInvestigation(identifier : string, ?creators : ResizeArray<LDNode>, ?dateCreated : System.DateTime, ?datePublished : System.DateTime, ?dateModified : System.DateTime, ?description : string, ?hasParts : ResizeArray<LDNode>, ?headline : string, ?citations : ResizeArray<LDNode>, ?comments : ResizeArray<LDNode>, ?mentions : ResizeArray<LDNode>, ?url : string, ?context : LDContext) =
        let id = Dataset.genIDInvesigation()
        let s = Dataset.create(id, identier = identifier, ?creators = creators, ?dateCreated = dateCreated, ?datePublished = datePublished, ?dateModified = dateModified, ?description = description, ?hasParts = hasParts, ?headline = headline, ?citations = citations, ?comments = comments, ?mentions = mentions, ?url = url, ?context = context)
        s.AdditionalType <- ResizeArray ["Investigation"]
        s

    static member createStudy(identifier : string, ?creators : ResizeArray<LDNode>, ?dateCreated : System.DateTime, ?datePublished : System.DateTime, ?dateModified : System.DateTime, ?description : string, ?hasParts : ResizeArray<LDNode>, ?headline : string, ?citations : ResizeArray<LDNode>, ?comments : ResizeArray<LDNode>, ?mentions : ResizeArray<LDNode>, ?url : string, ?abouts : ResizeArray<LDNode>, ?measurementMethod : string, ?measurementTechnique : string, ?variableMeasured : string, ?context : LDContext) =
        let id = Dataset.genIDStudy(identifier)
        let s = Dataset.create(id, identier = identifier, ?creators = creators, ?dateCreated = dateCreated, ?datePublished = datePublished, ?dateModified = dateModified, ?description = description, ?hasParts = hasParts, ?headline = headline, ?citations = citations, ?comments = comments, ?mentions = mentions, ?url = url, ?abouts = abouts, ?measurementMethod = measurementMethod, ?measurementTechnique = measurementTechnique, ?variableMeasured = variableMeasured, ?context = context)
        s.AdditionalType <- ResizeArray ["Study"]
        s

    static member createAssay(identifier : string, ?creators : ResizeArray<LDNode>, ?hasParts : ResizeArray<LDNode>, ?measurementMethod : string, ?measurementTechnique : string, ?context : LDContext) =
        let id = Dataset.genIDAssay(identifier)
        let s = Dataset.create(id, identier = identifier, ?creators = creators, ?hasParts = hasParts, ?measurementMethod = measurementMethod, ?measurementTechnique = measurementTechnique, ?context = context)
        s.AdditionalType <- ResizeArray ["Assay"]
        s
