namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
open ARCtrl.Helper

[<AttachMembers>]
type LDLabProcess =

    static member schemaType = "https://bioschemas.org/LabProcess"

    static member name = "http://schema.org/name"

    static member agent = "http://schema.org/agent"

    static member object_ = "http://schema.org/object"

    static member result = "http://schema.org/result"

    static member executesLabProtocol = "https://bioschemas.org/executesLabProtocol"

    static member parameterValue = "https://bioschemas.org/parameterValue"

    static member endTime = "http://schema.org/endTime"

    static member disambiguatingDescription = "http://schema.org/disambiguatingDescription"

    static member tryGetNameAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDLabProcess.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDLabProcess.name, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"property `name` of object with @id `{lp.Id}` was not a string"
        | _ -> failwith $"Could not access property `name` of object with @id `{lp.Id}`"

    static member setNameAsString(lp : LDNode, name : string, ?context : LDContext) =
        lp.SetProperty(LDLabProcess.name, name, ?context = context)

    static member tryGetAgent(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDPerson.validate(ldObject, ?context = context)
        match lp.TryGetPropertyAsSingleNode(LDLabProcess.agent, ?graph = graph, ?context = context) with
        | Some a when filter a context -> Some a
        | _ -> None

    static member getAgent(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDPerson.validate(ldObject, ?context = context)
        match lp.TryGetPropertyAsSingleNode(LDLabProcess.agent, ?graph = graph, ?context = context) with
        | Some a when filter a context -> a
        | Some _ -> failwith $"Property of `agent` of object with @id `{lp.Id}` was not a valid Person"
        | _ -> failwith $"Could not access property `agent` of object with @id `{lp.Id}`"

    static member setAgent(lp : LDNode, agent : LDNode, ?context : LDContext) =
        lp.SetProperty(LDLabProcess.agent, agent, ?context = context)

    static member getObjects(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        lp.GetPropertyNodes(LDLabProcess.object_, ?graph = graph, ?context = context)

    static member getObjectsAsSample(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDSample.validate(ldObject, ?context = context)
        lp.GetPropertyNodes(LDLabProcess.object_, filter = filter, ?graph = graph, ?context = context)

    static member getObjectsAsData(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDFile.validate(ldObject, ?context = context)
        lp.GetPropertyNodes(LDLabProcess.object_, filter = filter, ?graph = graph, ?context = context)

    static member setObjects(lp : LDNode, objects : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDLabProcess.object_, objects, ?context = context)

    static member getResults(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        lp.GetPropertyNodes(LDLabProcess.result, ?graph = graph, ?context = context)

    static member getResultsAsSample(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDSample.validate(ldObject, ?context = context)
        lp.GetPropertyNodes(LDLabProcess.result, filter = filter, ?graph = graph, ?context = context)

    static member getResultsAsData(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDFile.validate(ldObject, ?context = context)
        lp.GetPropertyNodes(LDLabProcess.result, filter = filter, ?graph = graph, ?context = context)

    static member setResults(lp : LDNode, results : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDLabProcess.result, results, ?context = context)

    static member tryGetExecutesLabProtocol(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDLabProtocol.validate(ldObject, ?context = context)
        match lp.TryGetPropertyAsSingleNode(LDLabProcess.executesLabProtocol, ?graph = graph, ?context = context) with
        | Some l when filter l context -> Some l
        | _ -> None

    static member setExecutesLabProtocol(lp : LDNode, executesLabProtocol : LDNode, ?context : LDContext) =
        lp.SetProperty(LDLabProcess.executesLabProtocol, executesLabProtocol, ?context = context)

    static member getParameterValues(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDPropertyValue.validate(ldObject, ?context = context)
        lp.GetPropertyNodes(LDLabProcess.parameterValue, filter = filter, ?graph = graph, ?context = context)

    static member setParameterValues(lp : LDNode, parameterValues : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDLabProcess.parameterValue, parameterValues, ?context = context)

    static member tryGetEndTime(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDLabProcess.endTime, ?context = context) with
        | Some (:? System.DateTime as et) -> Some et
        | _ -> None

    static member setEndTime(lp : LDNode, endTime : System.DateTime, ?context : LDContext) =
        lp.SetProperty(LDLabProcess.endTime, endTime, ?context = context)


    static member getDisambiguatingDescriptionsAsString(lp : LDNode, ?context : LDContext) =
        let filter = fun (o : obj) context -> o :? string
        lp.GetPropertyValues(LDLabProcess.disambiguatingDescription, filter = filter, ?context = context)
        |> ResizeArray.map (fun (o : obj) -> o :?> string)

    static member setDisambiguatingDescriptionsAsString(lp : LDNode, disambiguatingDescriptions : ResizeArray<string>, ?context : LDContext) =
        lp.SetProperty(LDLabProcess.disambiguatingDescription, disambiguatingDescriptions, ?context = context)

    static member validate(lp : LDNode, ?context : LDContext) =
        lp.HasType(LDLabProcess.schemaType, ?context = context)
        && lp.HasProperty(LDLabProcess.name, ?context = context)
        //&& lp.HasProperty(LDLabProcess.agent, ?context = context)
        //&& lp.HasProperty(LDLabProcess.object_, ?context = context)
        //&& lp.HasProperty(LDLabProcess.result, ?context = context)

    static member genId(name, ?assayName, ?studyName) =
        match assayName, studyName with
        | Some assay, Some study -> $"#Process_{study}_{assay}_{name}"
        | Some assay, None -> $"#Process_{assay}_{name}"
        | None, Some study -> $"#Process_{study}_{name}"
        | _ -> $"#Process_{name}"
        |> Helper.ID.clean


    static member create(name : string, ?objects : ResizeArray<LDNode>, ?results : ResizeArray<LDNode>, ?id : string, ?agent : LDNode, ?executesLabProtocol : LDNode, ?parameterValues : ResizeArray<LDNode>, ?endTime : System.DateTime, ?disambiguatingDescriptions : ResizeArray<string>, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> LDLabProcess.genId(name)
        let objects = Option.defaultValue (ResizeArray []) objects
        let results = Option.defaultValue (ResizeArray []) results
        let lp = LDNode(id, ResizeArray [LDLabProcess.schemaType], ?context = context)
        lp.SetProperty(LDLabProcess.name, name, ?context = context)
        lp.SetOptionalProperty(LDLabProcess.agent, agent, ?context = context) // Optional?
        lp.SetProperty(LDLabProcess.object_, objects, ?context = context)
        lp.SetProperty(LDLabProcess.result, results, ?context = context)
        lp.SetOptionalProperty(LDLabProcess.executesLabProtocol, executesLabProtocol, ?context = context)
        lp.SetOptionalProperty(LDLabProcess.parameterValue, parameterValues, ?context = context)
        lp.SetOptionalProperty(LDLabProcess.endTime, endTime, ?context = context)
        lp.SetOptionalProperty(LDLabProcess.disambiguatingDescription, disambiguatingDescriptions, ?context = context)
        lp