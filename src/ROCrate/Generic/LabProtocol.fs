namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

[<AttachMembers>]
type LabProtocol =

    static member schemaType = "https://bioschemas.org/LabProtocol"

    static member description = "http://schema.org/description"

    static member intendedUse = "https://bioschemas.org/intendedUse"

    static member name = "http://schema.org/name"

    static member comment = "http://schema.org/comment"

    static member computationalTool = "https://bioschemas.org/computationalTool"

    static member labEquipment = "https://bioschemas.org/labEquipment"

    static member reagent = "https://bioschemas.org/reagent"

    static member url = "http://schema.org/url"

    static member version = "http://schema.org/version"


    static member tryGetDescriptionAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LabProtocol.description, ?context = context) with
        | Some (:? string as d) -> Some d
        | _ -> None

    static member setDescriptionAsString(lp : LDNode, description : string, ?context : LDContext) =
        lp.SetProperty(LabProtocol.description, description, ?context = context)

    static member tryGetIntendedUseAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LabProtocol.intendedUse, ?context = context) with
        | Some (:? string as iu) -> Some iu
        | _ -> None

    static member setIntendedUseAsString(lp : LDNode, intendedUse : string, ?context : LDContext) =
        lp.SetProperty(LabProtocol.intendedUse, intendedUse, ?context = context)

    static member tryGetIntendedUseAsDefinedTerm(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = DefinedTerm.validate(ldObject, ?context = context)
        match lp.TryGetPropertyAsSingleNode(LabProtocol.intendedUse, ?graph = graph, ?context = context) with
        | Some iu when filter iu context -> Some iu
        | _ -> None

    static member setIntendedUseAsDefinedTerm(lp : LDNode, intendedUse : LDNode, ?context : LDContext) =
        lp.SetProperty(LabProtocol.intendedUse, intendedUse, ?context = context)

    static member tryGetNameAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LabProtocol.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LabProtocol.name, ?context = context) with
        | Some (:? string as n) -> n
        | _ -> failwith $"Could not access property `name` of object with @id `{lp.Id}`"

    static member setNameAsString(lp : LDNode, name : string, ?context : LDContext) =
        lp.SetProperty(LabProtocol.name, name, ?context = context)

    static member getComments(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = Comment.validate(ldObject, ?context = context)
        lp.GetPropertyNodes(LabProtocol.comment, filter = filter, ?graph = graph, ?context = context)

    static member setComments(lp : LDNode, comments : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LabProtocol.comment, comments, ?context = context)

    static member getComputationalTools(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        lp.GetPropertyNodes(LabProtocol.computationalTool, ?graph = graph, ?context = context)
        
    static member setComputationalTools(lp : LDNode, computationalTools : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LabProtocol.computationalTool, computationalTools, ?context = context)

    static member getLabEquipments(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        lp.GetPropertyNodes(LabProtocol.labEquipment, ?graph = graph, ?context = context)

    static member setLabEquipments(lp : LDNode, labEquipments : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LabProtocol.labEquipment, labEquipments, ?context = context)

    static member getReagents(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        lp.GetPropertyNodes(LabProtocol.reagent, ?graph = graph, ?context = context)

    static member setReagents(lp : LDNode, reagents : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LabProtocol.reagent, reagents, ?context = context)

    static member getComponents(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        LabProtocol.getLabEquipments(lp, ?graph = graph, ?context = context)
        |> Seq.append (LabProtocol.getReagents(lp, ?graph = graph, ?context = context))
        |> Seq.append (LabProtocol.getComputationalTools(lp, ?graph = graph, ?context = context))
        |> ResizeArray

    static member tryGetUrl(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LabProtocol.url, ?context = context) with
        | Some (:? string as u) -> Some u
        | _ -> None

    static member setUrl(lp : LDNode, url : string, ?context : LDContext) =
        lp.SetProperty(LabProtocol.url, url, ?context = context)

    static member tryGetVersionAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LabProtocol.version, ?context = context) with
        | Some (:? string as v) -> Some v
        | _ -> None

    static member setVersionAsString(lp : LDNode, version : string, ?context : LDContext) =
        lp.SetProperty(LabProtocol.version, version, ?context = context)

    static member validate(lp : LDNode, ?context : LDContext) =
        lp.HasType(LabProtocol.schemaType, ?context = context)
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
        let lp = LDNode(id, ResizeArray [LabProtocol.schemaType], ?context = context)
        lp.SetOptionalProperty(LabProtocol.name, name, ?context = context)
        lp.SetOptionalProperty(LabProtocol.description, description, ?context = context)
        lp.SetOptionalProperty(LabProtocol.intendedUse, intendedUse, ?context = context)
        lp.SetOptionalProperty(LabProtocol.comment, comments, ?context = context)
        lp.SetOptionalProperty(LabProtocol.computationalTool, computationalTools, ?context = context)
        lp.SetOptionalProperty(LabProtocol.labEquipment, labEquipments, ?context = context)
        lp.SetOptionalProperty(LabProtocol.reagent, reagents, ?context = context)
        lp.SetOptionalProperty(LabProtocol.url, url, ?context = context)
        lp.SetOptionalProperty(LabProtocol.version, version, ?context = context)
        lp