namespace ARCtrl.ROCrate.Generic

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

    static member validate(s : LDNode, ?context : LDContext) =
        s.HasType(Sample.schemaType, ?context = context)
        && s.HasProperty(Sample.name, ?context = context)

    static member create(id : string, name : string, ?additionalProperties : ResizeArray<LDNode>, ?context : LDContext) =
        let s = LDNode(id, ResizeArray [Sample.schemaType], ?context = context)
        s.SetProperty(Sample.name, name, ?context = context)
        s.SetOptionalProperty(Sample.additionalProperty, additionalProperties, ?context = context)