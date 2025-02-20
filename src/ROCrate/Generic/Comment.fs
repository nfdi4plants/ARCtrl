namespace ARCtrl.ROCrate.Generic

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

///
[<AttachMembers>]
type Comment =

    static member schemaType = "http://schema.org/Comment"

    static member text = "http://schema.org/text"

    static member name = "http://schema.org/name"

    static member tryGetTextAsString(dt : LDNode, ?context : LDContext) =
        match dt.TryGetPropertyAsSingleton(Comment.text, ?context = context) with
        | Some (:? string as tc) -> Some tc
        | _ -> None

    static member getTextAsString(dt : LDNode, ?context : LDContext) =
        match dt.TryGetPropertyAsSingleton(Comment.text, ?context = context) with
        | Some (:? string as tc) -> tc
        | Some _ -> failwith $"Property of `text` of object with @id `{dt.Id}` was not a string"
        | _ -> failwith $"Could not access property `text` of object with @id `{dt.Id}`"

    static member setTextAsString(dt : LDNode, text : string, ?context : LDContext) =
        dt.SetProperty(Comment.text, text, ?context = context)

    static member tryGetNameAsString(dt : LDNode, ?context : LDContext) =
        match dt.TryGetPropertyAsSingleton(Comment.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(dt : LDNode, ?context : LDContext) =
        match dt.TryGetPropertyAsSingleton(Comment.name, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Property of `name` of object with @id `{dt.Id}` was not a string"
        | _ -> failwith $"Could not access property `name` of object with @id `{dt.Id}`"

    static member setNameAsString(dt : LDNode, name : string, ?context : LDContext) =
        dt.SetProperty(Comment.name, name, ?context = context)

    static member validate(dt : LDNode, ?context : LDContext) =
        dt.HasType(Comment.schemaType, ?context = context)
        && dt.HasProperty(Comment.name, ?context = context)

    static member create(id : string, name : string, ?text : string, ?context : LDContext) =
        let dt = LDNode(id, ResizeArray [Comment.schemaType], ?context = context)
        dt.SetProperty(Comment.name, name, ?context = context)
        dt.SetOptionalProperty(Comment.text, text, ?context = context)
        dt