namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

///
[<AttachMembers>]
type Sample =

    static member schemaType = "https://bioschemas.org/Sample"

    static member name = "http://schema.org/name"

    static member additionalProperty = "http://schema.org/additionalProperty"

    static member tryGetNameAsString(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(Sample.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(s : LDNode, ?context : LDContext) =
        match s.TryGetPropertyAsSingleton(Sample.name, ?context = context) with
        | Some (:? string as n) -> n
        | _ -> failwith $"Could not access property `name` of object with @id `{s.Id}`"

    static member setNameAsString(s : LDNode, n : string) =
        s.SetProperty(Sample.name, n)

    static member getAdditionalProperties(s : LDNode, ?graph : LDGraph, ?context : LDContext) : ResizeArray<LDNode> =
        let filter ldObject context = PropertyValue.validate(ldObject, ?context = context)
        s.GetPropertyNodes(Sample.additionalProperty, filter = filter, ?graph = graph, ?context = context)

    static member setAdditionalProperties(s : LDNode, additionalProperties : ResizeArray<LDNode>, ?context : LDContext) =
        s.SetProperty(Sample.additionalProperty, additionalProperties, ?context = context)

    static member getCharacteristics(s : LDNode, ?graph : LDGraph, ?context : LDContext) : ResizeArray<LDNode> =
        let filter ldObject context = PropertyValue.validateCharacteristicValue(ldObject, ?context = context)
        s.GetPropertyNodes(Sample.additionalProperty, filter = filter, ?graph = graph, ?context = context)

    static member getFactors(s : LDNode, ?graph : LDGraph, ?context : LDContext) : ResizeArray<LDNode> =
        let filter ldObject context = PropertyValue.validateFactorValue(ldObject, ?context = context)
        s.GetPropertyNodes(Sample.additionalProperty, filter = filter, ?graph = graph, ?context = context)

    static member validate(s : LDNode, ?context : LDContext) =
        s.HasType(Sample.schemaType, ?context = context)
        && s.HasProperty(Sample.name, ?context = context)

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
        Sample.validate(s, ?context = context)
        && s.AdditionalType.Contains("Sample")

    static member validateSource (s : LDNode, ?context : LDContext) =
        Sample.validate(s, ?context = context)
        && s.AdditionalType.Contains("Source")

    static member validateMaterial (s : LDNode, ?context : LDContext) =
        Sample.validate(s, ?context = context)
        && s.AdditionalType.Contains("Material")

    static member create(id : string, name : string, ?additionalProperties : ResizeArray<LDNode>, ?context : LDContext) =
        let s = LDNode(id, ResizeArray [Sample.schemaType], ?context = context)
        s.SetProperty(Sample.name, name, ?context = context)
        s.SetOptionalProperty(Sample.additionalProperty, additionalProperties, ?context = context)
        s

    static member createSample (name : string, ?id : string, ?additionalProperties : ResizeArray<LDNode>, ?context : LDContext) =
        let id = match id with
                 | Some id -> id
                 | None -> Sample.genIDSample name
        let s = Sample.create(id, name, ?additionalProperties = additionalProperties, ?context = context)
        s.AdditionalType <- ResizeArray ["Sample"]
        s

    static member createSource (name : string, ?id : string, ?additionalProperties : ResizeArray<LDNode>, ?context : LDContext) =
        let id = match id with
                 | Some id -> id
                 | None -> Sample.genIDSource name
        let s = Sample.create(id, name, ?additionalProperties = additionalProperties, ?context = context)
        s.AdditionalType <- ResizeArray ["Source"]
        s

    static member createMaterial (name : string, ?id, ?additionalProperties : ResizeArray<LDNode>, ?context : LDContext) =
        let id = match id with
                 | Some id -> id
                 | None -> Sample.genIDMaterial name
        let s = Sample.create(id, name, ?additionalProperties = additionalProperties, ?context = context)
        s.AdditionalType <- ResizeArray ["Material"]
        s