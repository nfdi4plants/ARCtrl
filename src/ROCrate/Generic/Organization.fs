namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
///
[<AttachMembers>]
type Organization =

    static member schemaType = "http://schema.org/Organization"

    static member name = "http://schema.org/name"

    static member tryGetNameAsString(o : LDNode, ?context : LDContext) =
        match o.TryGetPropertyAsSingleton(Organization.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(o : LDNode, ?context : LDContext) =
        match o.TryGetPropertyAsSingleton(Organization.name, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Property of `name` of object with @id `{o.Id}` was not a string"
        | _ -> failwith $"Could not access property `name` of object with @id `{o.Id}`"

    static member setNameAsString(o : LDNode, n : string, ?context : LDContext) =
        o.SetProperty(Organization.name, n, ?context = context)

    static member validate(o : LDNode, ?context : LDContext) =
        o.HasType(Organization.schemaType, ?context = context)
        && o.HasProperty(Organization.name, ?context = context)

    static member create(id : string, name : string, ?context : LDContext) =
        let o = LDNode(id, ResizeArray [Organization.schemaType], ?context = context)
        o.SetProperty(Organization.name, name, ?context = context)
        o