namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
///
[<AttachMembers>]
type LDOrganization =

    static member schemaType = "http://schema.org/Organization"

    static member name = "http://schema.org/name"

    static member tryGetNameAsString(o : LDNode, ?context : LDContext) =
        match o.TryGetPropertyAsSingleton(LDOrganization.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(o : LDNode, ?context : LDContext) =
        match o.TryGetPropertyAsSingleton(LDOrganization.name, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Property of `name` of object with @id `{o.Id}` was not a string"
        | _ -> failwith $"Could not access property `name` of object with @id `{o.Id}`"

    static member setNameAsString(o : LDNode, n : string, ?context : LDContext) =
        o.SetProperty(LDOrganization.name, n, ?context = context)

    static member genID(name : string) =
        $"#Organization_{name}"
        |> Helper.ID.clean

    static member validate(o : LDNode, ?context : LDContext) =
        o.HasType(LDOrganization.schemaType, ?context = context)
        && o.HasProperty(LDOrganization.name, ?context = context)

    static member create(name : string, ?id : string,  ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> LDOrganization.genID name
        let o = LDNode(id, ResizeArray [LDOrganization.schemaType], ?context = context)
        o.SetProperty(LDOrganization.name, name, ?context = context)
        o