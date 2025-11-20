namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate


///
[<AttachMembers>]
type LDSample =

    static member schemaType = "https://bioschemas.org/Sample"

    static member name = "http://schema.org/name"

    static member additionalProperty = "http://schema.org/additionalProperty"

    static member tryGetNameAsString(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(LDSample.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(LDSample.name, ?context = context) with
        | Some (:? string as n) -> n
        | _ -> failwith $"Could not access property `name` of object with @id `{s.Id}`"

    static member setNameAsString(s : LDNode, n : string) =
        s.SetProperty(LDSample.name, n)

    static member getAdditionalProperties(s : LDNode, ?graph : LDGraph, ?context : LDContext) : ResizeArray<LDNode> =
        let filter ldObject context = LDPropertyValue.validate(ldObject, ?context = context)
        s.GetPropertyNodes(LDSample.additionalProperty, filter = filter, ?graph = graph, ?context = context)

    static member setAdditionalProperties(s : LDNode, additionalProperties : ResizeArray<LDNode>, ?context : LDContext) =
        s.SetProperty(LDSample.additionalProperty, additionalProperties, ?context = context)

    static member getCharacteristics(s : LDNode, ?graph : LDGraph, ?context : LDContext) : ResizeArray<LDNode> =
        let filter ldObject context = LDPropertyValue.validateCharacteristicValue(ldObject, ?context = context)
        s.GetPropertyNodes(LDSample.additionalProperty, filter = filter, ?graph = graph, ?context = context)

    static member getFactors(s : LDNode, ?graph : LDGraph, ?context : LDContext) : ResizeArray<LDNode> =
        let filter ldObject context = LDPropertyValue.validateFactorValue(ldObject, ?context = context)
        s.GetPropertyNodes(LDSample.additionalProperty, filter = filter, ?graph = graph, ?context = context)

    static member validate(s : LDNode, ?context : LDContext) =
        s.HasType(LDSample.schemaType, ?context = context)
        && s.HasProperty(LDSample.name, ?context = context)

    static member genIDSample(name : string) =
        $"#Sample_{name}"
        |> Helper.ID.clean

    static member genIDSource(name : string) =
        $"#Source_{name}"
        |> Helper.ID.clean

    static member genIDMaterial(name : string) =
        $"#Material_{name}"
        |> Helper.ID.clean

    static member validateSample (s : LDNode, ?context : LDContext) =
        LDSample.validate(s, ?context = context)
        && s.AdditionalType.Contains("Sample")

    static member validateSource (s : LDNode, ?context : LDContext) =
        LDSample.validate(s, ?context = context)
        && s.AdditionalType.Contains("Source")

    static member validateMaterial (s : LDNode, ?context : LDContext) =
        LDSample.validate(s, ?context = context)
        && s.AdditionalType.Contains("Material")

    static member create(id : string, name : string, ?additionalProperties : ResizeArray<LDNode>, ?context : LDContext) =
        let s = LDNode(id, ResizeArray [LDSample.schemaType], ?context = context)
        s.SetProperty(LDSample.name, name, ?context = context)
        s.SetOptionalProperty(LDSample.additionalProperty, additionalProperties, ?context = context)
        s

    static member createSample (name : string, ?id : string, ?additionalProperties : ResizeArray<LDNode>, ?context : LDContext) =
        let id = match id with
                 | Some id -> id
                 | None -> LDSample.genIDSample name
        let s = LDSample.create(id, name, ?additionalProperties = additionalProperties, ?context = context)
        s.AdditionalType <- ResizeArray ["Sample"]
        s

    static member createSource (name : string, ?id : string, ?additionalProperties : ResizeArray<LDNode>, ?context : LDContext) =
        let id = match id with
                 | Some id -> id
                 | None -> LDSample.genIDSource name
        let s = LDSample.create(id, name, ?additionalProperties = additionalProperties, ?context = context)
        s.AdditionalType <- ResizeArray ["Source"]
        s

    static member createMaterial (name : string, ?id, ?additionalProperties : ResizeArray<LDNode>, ?context : LDContext) =
        let id = match id with
                 | Some id -> id
                 | None -> LDSample.genIDMaterial name
        let s = LDSample.create(id, name, ?additionalProperties = additionalProperties, ?context = context)
        s.AdditionalType <- ResizeArray ["Material"]
        s