namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

///
[<AttachMembers>]
type LDComment =

    static member schemaType = "http://schema.org/Comment"

    static member text = "http://schema.org/text"

    static member name = "http://schema.org/name"

    static member tryGetTextAsString(dt : LDNode, ?context : LDContext) =
        match dt.TryGetPropertyAsSingleton(LDComment.text, ?context = context) with
        | Some (:? string as tc) -> Some tc
        | _ -> None

    static member getTextAsString(dt : LDNode, ?context : LDContext) =
        match dt.TryGetPropertyAsSingleton(LDComment.text, ?context = context) with
        | Some (:? string as tc) -> tc
        | Some _ -> failwith $"Property of `text` of object with @id `{dt.Id}` was not a string"
        | _ -> failwith $"Could not access property `text` of object with @id `{dt.Id}`"

    static member setTextAsString(dt : LDNode, text : string, ?context : LDContext) =
        dt.SetProperty(LDComment.text, text, ?context = context)

    static member tryGetNameAsString(dt : LDNode, ?context : LDContext) =
        match dt.TryGetPropertyAsSingleton(LDComment.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(dt : LDNode, ?context : LDContext) =
        match dt.TryGetPropertyAsSingleton(LDComment.name, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Property of `name` of object with @id `{dt.Id}` was not a string"
        | _ -> failwith $"Could not access property `name` of object with @id `{dt.Id}`"

    static member setNameAsString(dt : LDNode, name : string, ?context : LDContext) =
        dt.SetProperty(LDComment.name, name, ?context = context)

    static member genID(name : string, ?text : string) =
        match text with
        | Some t -> $"#LDComment_{name}_{t}"
        | None -> $"#LDComment_{name}"
        |> Helper.ID.clean

    static member validate(dt : LDNode, ?context : LDContext) =
        dt.HasType(LDComment.schemaType, ?context = context)
        && dt.HasProperty(LDComment.name, ?context = context)

    static member create(name : string, ?id : string, ?text : string, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> LDComment.genID(name, ?text = text)
        let dt = LDNode(id, ResizeArray [LDComment.schemaType], ?context = context)
        dt.SetProperty(LDComment.name, name, ?context = context)
        dt.SetOptionalProperty(LDComment.text, text, ?context = context)
        dt