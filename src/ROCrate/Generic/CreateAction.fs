namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
open ARCtrl.Helper

[<AttachMembers>]
type LDCreateAction =

    static member schemaType = "https://bioschemas.org/CreateAction"

    static member name = "http://schema.org/name"

    static member instrument = "http://schema.org/instrument"

    static member agent = "http://schema.org/agent"

    static member object_ = "http://schema.org/object"

    static member result = "http://schema.org/result"

    static member description = "http://schema.org/description"

    static member endTime = "http://schema.org/endTime"

    static member disambiguatingDescription = "http://schema.org/disambiguatingDescription"

    static member tryGetNameAsString(ca : LDNode, ?context : LDContext) =
        match ca.TryGetPropertyAsSingleton(LDCreateAction.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(ca : LDNode, ?context : LDContext) =
        match ca.TryGetPropertyAsSingleton(LDCreateAction.name, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `name` of object with @id `{ca.Id}` was not a string"
        | _ -> failwith $"Could not access property `name` of object with @id `{ca.Id}`"

    static member setNameAsString(ca : LDNode, name : string, ?context : LDContext) =
        ca.SetProperty(LDCreateAction.name, name, ?context = context)

    static member getInstruments(ca : LDNode, ?graph : LDGraph, ?context : LDContext) =
        ca.GetPropertyNodes(LDCreateAction.instrument, ?graph = graph, ?context = context)
    static member getInstrumentsAsComputationalWorkflow(ca : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDComputationalWorkflow.validate(ldObject, ?context = context)
        ca.GetPropertyNodes(LDCreateAction.instrument, filter = filter, ?graph = graph, ?context = context)
    static member setInstruments(ca : LDNode, instruments : ResizeArray<LDNode>, ?context : LDContext) =
        ca.SetProperty(LDCreateAction.instrument, instruments, ?context = context)
        
    static member tryGetAgent(ca : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDPerson.validate(ldObject, ?context = context)
        match ca.TryGetPropertyAsSingleNode(LDCreateAction.agent, ?graph = graph, ?context = context) with
        | Some a when filter a context -> Some a
        | _ -> None

    static member getAgent(ca : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDPerson.validate(ldObject, ?context = context)
        match ca.TryGetPropertyAsSingleNode(LDCreateAction.agent, ?graph = graph, ?context = context) with
        | Some a when filter a context -> a
        | Some _ -> failwith $"Property of `agent` of object with @id `{ca.Id}` was not a valid Person"
        | _ -> failwith $"Could not access property `agent` of object with @id `{ca.Id}`"

    static member setAgent(ca : LDNode, agent : LDNode, ?context : LDContext) =
        ca.SetProperty(LDCreateAction.agent, agent, ?context = context)

    static member getObjects(ca : LDNode, ?graph : LDGraph, ?context : LDContext) =
        ca.GetPropertyNodes(LDCreateAction.object_, ?graph = graph, ?context = context)

    static member getObjectsAsSample(ca : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDSample.validate(ldObject, ?context = context)
        ca.GetPropertyNodes(LDCreateAction.object_, filter = filter, ?graph = graph, ?context = context)

    static member getObjectsAsData(ca : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDFile.validate(ldObject, ?context = context)
        ca.GetPropertyNodes(LDCreateAction.object_, filter = filter, ?graph = graph, ?context = context)

    static member setObjects(ca : LDNode, objects : ResizeArray<LDNode>, ?context : LDContext) =
        ca.SetProperty(LDCreateAction.object_, objects, ?context = context)

    static member getResults(ca : LDNode, ?graph : LDGraph, ?context : LDContext) =
        ca.GetPropertyNodes(LDCreateAction.result, ?graph = graph, ?context = context)

    static member getResultsAsSample(ca : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDSample.validate(ldObject, ?context = context)
        ca.GetPropertyNodes(LDCreateAction.result, filter = filter, ?graph = graph, ?context = context)

    static member getResultsAsData(ca : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDFile.validate(ldObject, ?context = context)
        ca.GetPropertyNodes(LDCreateAction.result, filter = filter, ?graph = graph, ?context = context)

    static member setResults(ca : LDNode, results : ResizeArray<LDNode>, ?context : LDContext) =
        ca.SetProperty(LDCreateAction.result, results, ?context = context)

    static member tryGetDescription(ca : LDNode, ?context : LDContext) =
        match ca.TryGetPropertyAsSingleton(LDCreateAction.description, ?context = context) with
        | Some (:? string as d) -> Some d
        | _ -> None

    static member setDescription(ca : LDNode, description : string, ?context : LDContext) =
        ca.SetProperty(LDCreateAction.description, description, ?context = context)
    static member tryGetEndTime(ca : LDNode, ?context : LDContext) =
        match ca.TryGetPropertyAsSingleton(LDCreateAction.endTime, ?context = context) with
        | Some (:? System.DateTime as et) -> Some et
        | _ -> None

    static member setEndTime(ca : LDNode, endTime : System.DateTime, ?context : LDContext) =
        ca.SetProperty(LDCreateAction.endTime, endTime, ?context = context)

    static member getDisambiguatingDescriptionsAsString(ca : LDNode, ?context : LDContext) =
        let filter = fun (o : obj) context -> o :? string
        ca.GetPropertyValues(LDCreateAction.disambiguatingDescription, filter = filter, ?context = context)
        |> ResizeArray.map (fun (o : obj) -> o :?> string)

    static member setDisambiguatingDescriptionsAsString(ca : LDNode, disambiguatingDescriptions : ResizeArray<string>, ?context : LDContext) =
        ca.SetProperty(LDCreateAction.disambiguatingDescription, disambiguatingDescriptions, ?context = context)

    static member validate(ca : LDNode, ?context : LDContext) =
        ca.HasType(LDCreateAction.schemaType, ?context = context)
        //&& ca.HasProperty(LDCreateAction.agent, ?context = context)
        //&& ca.HasProperty(LDCreateAction.object_, ?context = context)
        //&& ca.HasProperty(LDCreateAction.result, ?context = context)
        && ca.HasProperty(LDCreateAction.instrument, ?context = context)

    static member create(name : string, agent : LDNode, instrument : LDNode, ?objects : ResizeArray<LDNode>, ?results : ResizeArray<LDNode>, ?description : string, ?id : string, ?endTime : System.DateTime, ?disambiguatingDescriptions : ResizeArray<string>, ?context : LDContext) =
        let id =
            match id with
            | Some i -> i
            | None -> $"#ComputationalWorkflow_{ARCtrl.Helper.Identifier.createMissingIdentifier()}" |> Helper.ID.clean
        let objects = Option.defaultValue (ResizeArray []) objects
        let results = Option.defaultValue (ResizeArray []) results
        let ca = LDNode(id, ResizeArray [LDCreateAction.schemaType], ?context = context)
        ca.SetProperty(LDCreateAction.name, name, ?context = context)
        ca.SetProperty(LDCreateAction.agent, agent, ?context = context)
        ca.SetProperty(LDCreateAction.object_, objects, ?context = context)
        ca.SetProperty(LDCreateAction.result, results, ?context = context)
        ca.SetProperty(LDCreateAction.instrument, instrument, ?context = context)
        ca.SetOptionalProperty(LDCreateAction.description, description, ?context = context)
        ca.SetOptionalProperty(LDCreateAction.endTime, endTime, ?context = context)
        ca.SetOptionalProperty(LDCreateAction.disambiguatingDescription, disambiguatingDescriptions, ?context = context)
        ca