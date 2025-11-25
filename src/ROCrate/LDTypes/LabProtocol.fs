namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

[<AttachMembers>]
type LDLabProtocol =

    static member schemaType = "https://bioschemas.org/LabProtocol"

    static member description = "http://schema.org/description"

    static member name = "http://schema.org/name"

    static member computationalTool = "https://bioschemas.org/properties/computationalTool"

    static member computationalToolDeprecated = "https://bioschemas.org/computationalTool"

    static member labEquipment = "https://bioschemas.org/properties/labEquipment"

    static member labEquipmentDeprecated = "https://bioschemas.org/labEquipment"

    static member reagent = "https://bioschemas.org/properties/reagent"

    static member reagentDeprecated = "https://bioschemas.org/reagent"

    static member intendedUse = "https://bioschemas.org/properties/intendedUse"

    static member intendedUseDeprecated = "https://bioschemas.org/intendedUse"

    static member url = "http://schema.org/url"

    static member version = "http://schema.org/version"

    static member comment = "http://schema.org/comment"

    static member tryGetDescriptionAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDLabProtocol.description, ?context = context) with
        | Some (:? string as d) -> Some d
        | _ -> None

    static member setDescriptionAsString(lp : LDNode, description : string, ?context : LDContext) =
        lp.SetProperty(LDLabProtocol.description, description, ?context = context)

    static member tryGetIntendedUseAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDLabProtocol.intendedUse, ?context = context) with
        | Some (:? string as iu) -> Some iu
        | _ ->
            match lp.TryGetPropertyAsSingleton(LDLabProtocol.intendedUseDeprecated, ?context = context) with
            | Some (:? string as iu) -> Some iu
            | _ -> None

    static member setIntendedUseAsString(lp : LDNode, intendedUse : string, ?context : LDContext) =
        lp.SetProperty(LDLabProtocol.intendedUse, intendedUse, ?context = context)

    static member tryGetIntendedUseAsDefinedTerm(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDDefinedTerm.validate(ldObject, ?context = context)
        match lp.TryGetPropertyAsSingleNode(LDLabProtocol.intendedUse, ?graph = graph, ?context = context) with
        | Some iu when filter iu context -> Some iu
        | _ ->
            match lp.TryGetPropertyAsSingleNode(LDLabProtocol.intendedUseDeprecated, ?graph = graph, ?context = context) with
            | Some iu when filter iu context -> Some iu
            | _ -> None

    static member setIntendedUseAsDefinedTerm(lp : LDNode, intendedUse : LDNode, ?context : LDContext) =
        lp.SetProperty(LDLabProtocol.intendedUse, intendedUse, ?context = context)

    static member tryGetNameAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDLabProtocol.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDLabProtocol.name, ?context = context) with
        | Some (:? string as n) -> n
        | _ -> failwith $"Could not access property `name` of object with @id `{lp.Id}`"

    static member setNameAsString(lp : LDNode, name : string, ?context : LDContext) =
        lp.SetProperty(LDLabProtocol.name, name, ?context = context)

    static member getComments(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = LDComment.validate(ldObject, ?context = context)
        lp.GetPropertyNodes(LDLabProtocol.comment, filter = filter, ?graph = graph, ?context = context)

    static member setComments(lp : LDNode, comments : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDLabProtocol.comment, comments, ?context = context)

    static member getComputationalTools(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let l = lp.GetPropertyNodes(LDLabProtocol.computationalTool, ?graph = graph, ?context = context)
        if l.Count = 0 then
            // try deprecated property
            lp.GetPropertyNodes(LDLabProtocol.computationalToolDeprecated, ?graph = graph, ?context = context)
        else
            l
        
    static member setComputationalTools(lp : LDNode, computationalTools : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDLabProtocol.computationalTool, computationalTools, ?context = context)

    static member getLabEquipments(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let l = lp.GetPropertyNodes(LDLabProtocol.labEquipment, ?graph = graph, ?context = context)
        if l.Count = 0 then
            // try deprecated property
            lp.GetPropertyNodes(LDLabProtocol.labEquipmentDeprecated, ?graph = graph, ?context = context)
        else
            l

    static member setLabEquipments(lp : LDNode, labEquipments : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDLabProtocol.labEquipment, labEquipments, ?context = context)

    static member getReagents(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let l = lp.GetPropertyNodes(LDLabProtocol.reagent, ?graph = graph, ?context = context)
        if l.Count = 0 then
            // try deprecated property
            lp.GetPropertyNodes(LDLabProtocol.reagentDeprecated, ?graph = graph, ?context = context)
        else
            l

    static member setReagents(lp : LDNode, reagents : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LDLabProtocol.reagent, reagents, ?context = context)

    static member getComponents(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        LDLabProtocol.getLabEquipments(lp, ?graph = graph, ?context = context)
        |> Seq.append (LDLabProtocol.getReagents(lp, ?graph = graph, ?context = context))
        |> Seq.append (LDLabProtocol.getComputationalTools(lp, ?graph = graph, ?context = context))
        |> ResizeArray

    static member tryGetUrl(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDLabProtocol.url, ?context = context) with
        | Some (:? string as u) -> Some u
        | _ -> None

    static member setUrl(lp : LDNode, url : string, ?context : LDContext) =
        lp.SetProperty(LDLabProtocol.url, url, ?context = context)

    static member tryGetVersionAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LDLabProtocol.version, ?context = context) with
        | Some (:? string as v) -> Some v
        | _ -> None

    static member setVersionAsString(lp : LDNode, version : string, ?context : LDContext) =
        lp.SetProperty(LDLabProtocol.version, version, ?context = context)

    static member validate(lp : LDNode, ?context : LDContext) =
        lp.HasType(LDLabProtocol.schemaType, ?context = context)
        //&& lp.HasProperty(LabProtocol.name, ?context = context)

    static member genId(?name : string, ?processName : string, ?assayName : string, ?studyName : string) =
        [
            if name.IsSome then name.Value
            if processName.IsSome then processName.Value
            if assayName.IsSome then assayName.Value
            if studyName.IsSome then studyName.Value
        ]
        |> fun vals ->
            if vals.IsEmpty then [ARCtrl.Helper.Identifier.createMissingIdentifier()]
            else vals
        |> List.append ["#Protocol"]
        |> String.concat "_"
        |> Helper.ID.clean

    static member create(id : string, ?name : string, ?description : string, ?intendedUse : LDNode, ?comments : ResizeArray<LDNode>, ?computationalTools : ResizeArray<LDNode>, ?labEquipments : ResizeArray<LDNode>, ?reagents : ResizeArray<LDNode>, ?url : string, ?version : string, ?context : LDContext) =
        let lp = LDNode(id, ResizeArray [LDLabProtocol.schemaType], ?context = context)
        lp.SetOptionalProperty(LDLabProtocol.name, name, ?context = context)
        lp.SetOptionalProperty(LDLabProtocol.description, description, ?context = context)
        lp.SetOptionalProperty(LDLabProtocol.intendedUse, intendedUse, ?context = context)
        lp.SetOptionalProperty(LDLabProtocol.comment, comments, ?context = context)
        lp.SetOptionalProperty(LDLabProtocol.computationalTool, computationalTools, ?context = context)
        lp.SetOptionalProperty(LDLabProtocol.labEquipment, labEquipments, ?context = context)
        lp.SetOptionalProperty(LDLabProtocol.reagent, reagents, ?context = context)
        lp.SetOptionalProperty(LDLabProtocol.url, url, ?context = context)
        lp.SetOptionalProperty(LDLabProtocol.version, version, ?context = context)
        lp