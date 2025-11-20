namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
open ARCtrl.Helper

[<AttachMembers>]
type LDComputationalWorkflow =

    static member schemaType = "https://bioschemas.org/ComputationalWorkflow"
    // Recommended properties
    static member input = "https://bioschemas.org/input"
    static member output = "https://bioschemas.org/output"
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
    static member additionalType = "http://schema.org/additionalType"
    static member comment = "http://schema.org/comment"

    // Getters and setters for recommended properties
    static member getInputs(cw : LDNode, ?graph : LDGraph, ?context : LDContext) =
        cw.GetPropertyNodes(LDComputationalWorkflow.input, ?graph = graph, ?context = context)

    static member getInputsAsFormalParameters(cw : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDFormalParameter.validate(ldObject, ?context = context)
        cw.GetPropertyNodes(LDComputationalWorkflow.input, filter = filter, ?graph = graph, ?context = context)

    static member setInputs(cw : LDNode, inputs : ResizeArray<LDNode>, ?context : LDContext) =
        cw.SetProperty(LDComputationalWorkflow.input, inputs, ?context = context)

    static member getOutputs(cw : LDNode, ?graph : LDGraph, ?context : LDContext) =
        cw.GetPropertyNodes(LDComputationalWorkflow.output, ?graph = graph, ?context = context)

    static member getOutputsAsFormalParameter(cw : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDFormalParameter.validate(ldObject, ?context = context)
        cw.GetPropertyNodes(LDComputationalWorkflow.output, filter = filter, ?graph = graph, ?context = context)

    static member setOutputs(cw : LDNode, outputs : ResizeArray<LDNode>, ?context : LDContext) =
        cw.SetProperty(LDComputationalWorkflow.output, outputs, ?context = context)

    static member getCreator(cw : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDPerson.validate(ldObject, ?context = context) || LDOrganization.validate(ldObject, ?context = context)
        cw.GetPropertyNodes(LDComputationalWorkflow.creator, filter = filter, ?graph = graph, ?context = context)

    static member setCreator(cw : LDNode, creator : LDNode, ?context : LDContext) =
        cw.SetProperty(LDComputationalWorkflow.creator, creator, ?context = context)
    static member tryGetDateCreated(cw : LDNode, ?context : LDContext) =
        match cw.TryGetPropertyAsSingleton(LDComputationalWorkflow.dateCreated, ?context = context) with
        | Some (:? string as d) -> Some d
        | _ -> None
    static member setDateCreated(cw : LDNode, date : string, ?context : LDContext) =
        cw.SetProperty(LDComputationalWorkflow.dateCreated, date, ?context = context)

    static member getLicenses(cw : LDNode, ?graph : LDGraph, ?context : LDContext) =
        cw.GetPropertyNodes(LDComputationalWorkflow.license, ?graph = graph, ?context = context)
    static member setLicenses(cw : LDNode, licenses : ResizeArray<LDNode>, ?context : LDContext) =
        cw.SetProperty(LDComputationalWorkflow.license, licenses, ?context = context)

    static member tryGetNameAsString(cw : LDNode, ?context : LDContext) =
        match cw.TryGetPropertyAsSingleton(LDComputationalWorkflow.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    
    static member getNameAsString(cw : LDNode, ?context : LDContext) =
        match cw.TryGetPropertyAsSingleton(LDComputationalWorkflow.name, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `name` of object with @id `{cw.Id}` was not a string"
        | _ -> failwith $"Could not access property `name` of object with @id `{cw.Id}`"


    static member setNameAsString(cw : LDNode, name : string, ?context : LDContext) =
        cw.SetProperty(LDComputationalWorkflow.name, name, ?context = context)

    static member getProgrammingLanguages(cw : LDNode, ?graph : LDGraph, ?context : LDContext) =
        cw.GetPropertyNodes(LDComputationalWorkflow.programmingLanguage, ?graph = graph, ?context = context)

    static member setProgrammingLanguages(cw : LDNode, programmingLanguages : ResizeArray<LDNode>, ?context : LDContext) =
        cw.SetProperty(LDComputationalWorkflow.programmingLanguage, programmingLanguages, ?context = context)

    static member tryGetSdPublisher(cw : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDPerson.validate(ldObject, ?context = context) || LDOrganization.validate(ldObject, ?context = context)
        match cw.TryGetPropertyAsSingleNode(LDComputationalWorkflow.sdPublisher, ?graph = graph, ?context = context) with
        | Some a when filter a context -> Some a
        | _ -> None

    static member setSdPublisher(cw : LDNode, sdPublisher : LDNode, ?context : LDContext) =
        cw.SetProperty(LDComputationalWorkflow.sdPublisher, sdPublisher, ?context = context)

    static member tryGetUrl(cw : LDNode, ?context : LDContext) =
        match cw.TryGetPropertyAsSingleton(LDComputationalWorkflow.url, ?context = context) with
        | Some (:? string as u) -> Some u
        | _ -> None

    static member setUrl(cw : LDNode, url : string, ?context : LDContext) =
        cw.SetProperty(LDComputationalWorkflow.url, url, ?context = context)

    static member tryGetVersion(cw : LDNode, ?context : LDContext) =
        match cw.TryGetPropertyAsSingleton(LDComputationalWorkflow.version, ?context = context) with
        | Some (:? string as v) -> Some v
        | _ -> None

    static member setVersion(cw : LDNode, version : string, ?context : LDContext) =
        cw.SetProperty(LDComputationalWorkflow.version, version, ?context = context)

    static member tryGetDescription(cw : LDNode, ?context : LDContext) =
        match cw.TryGetPropertyAsSingleton(LDComputationalWorkflow.description, ?context = context) with
        | Some (:? string as d) -> Some d
        | _ -> None

    static member setDescription(cw : LDNode, description : string, ?context : LDContext) =
        cw.SetProperty(LDComputationalWorkflow.description, description, ?context = context)

    static member getHasPart(cw : LDNode, ?graph : LDGraph, ?context : LDContext) =
        cw.GetPropertyNodes(LDComputationalWorkflow.hasPart, ?graph = graph, ?context = context)

    static member setHasPart(cw : LDNode, hasParts : string list, ?context : LDContext) =
        cw.SetProperty(LDComputationalWorkflow.hasPart, ResizeArray hasParts, ?context = context)

    static member tryGetAdditionalTypeAsString(cw : LDNode, ?context : LDContext) =
        match cw.TryGetPropertyAsSingleton(LDComputationalWorkflow.additionalType, ?context = context) with
        | Some (:? string as at) -> Some at
        | _ -> None

    static member getAdditionalTypeAsString(cw : LDNode, ?context : LDContext) =
        match cw.TryGetPropertyAsSingleton(LDComputationalWorkflow.additionalType, ?context = context) with
        | Some (:? string as at) -> at
        | Some _ -> failwith $"property `additionalType` of object with @id `{cw.Id}` was not a string"
        | _ -> failwith $"Could not access property `additionalType` of object with @id `{cw.Id}`"

    static member setAdditionalTypeAsString(cw : LDNode, additionalType : string, ?context : LDContext) =
        cw.SetProperty(LDComputationalWorkflow.additionalType, additionalType, ?context = context)

    static member getComments(cw : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDComment.validate(ldObject, ?context = context)
        cw.GetPropertyNodes(LDComputationalWorkflow.comment, filter = filter, ?graph = graph, ?context = context)

    static member setComments(cw : LDNode, comments : ResizeArray<LDNode>, ?context : LDContext) =
        cw.SetProperty(LDComputationalWorkflow.comment, comments, ?context = context)

    static member validate(lp : LDNode, ?context : LDContext) =
        lp.HasType(LDComputationalWorkflow.schemaType, ?context = context)

    //static member validateWorkflowDescription(lp : LDNode, ?context : LDContext) =
    //    lp.HasType(LDComputationalWorkflow.schemaType, ?context = context)
    //    && (LDComputationalWorkflow.getAdditionalTypeAsString(lp, ?context = context) = "WorkflowDescription")

    //static member validateToolDescription(lp : LDNode, ?context : LDContext) =
    //    lp.HasType(LDComputationalWorkflow.schemaType, ?context = context)
    //    && (LDComputationalWorkflow.getAdditionalTypeAsString(lp, ?context = context) = "ToolDescription")
            

    static member create
        (
            ?id : string,
            ?inputs : ResizeArray<LDNode>,
            ?outputs : ResizeArray<LDNode>,
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
        let cw = LDNode(id, ResizeArray [LDComputationalWorkflow.schemaType], ?context = context)
        cw.SetOptionalProperty(LDComputationalWorkflow.input, inputs, ?context = context)
        cw.SetOptionalProperty(LDComputationalWorkflow.output, outputs, ?context = context)
        cw.SetOptionalProperty(LDComputationalWorkflow.creator, creator, ?context = context)
        cw.SetOptionalProperty(LDComputationalWorkflow.dateCreated, dateCreated, ?context = context)
        cw.SetOptionalProperty(LDComputationalWorkflow.license, licenses, ?context = context)
        cw.SetOptionalProperty(LDComputationalWorkflow.name, name, ?context = context)
        cw.SetOptionalProperty(LDComputationalWorkflow.programmingLanguage, programmingLanguages, ?context = context)
        cw.SetOptionalProperty(LDComputationalWorkflow.sdPublisher, sdPublisher, ?context = context)
        cw.SetOptionalProperty(LDComputationalWorkflow.url, url, ?context = context)
        cw.SetOptionalProperty(LDComputationalWorkflow.version, version, ?context = context)
        cw.SetOptionalProperty(LDComputationalWorkflow.description, description, ?context = context)
        cw.SetOptionalProperty(LDComputationalWorkflow.hasPart, hasParts, ?context = context)
        cw.SetOptionalProperty(LDComputationalWorkflow.comment, comments, ?context = context)
        cw
    