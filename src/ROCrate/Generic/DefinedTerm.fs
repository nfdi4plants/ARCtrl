namespace ARCtrl.ROCrate.Generic

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

///
[<AttachMembers>]
type DefinedTerm =

    static member schemaType = "http://schema.org/DefinedTerm"

    static member termCode = "http://schema.org/termCode"

    static member name = "http://schema.org/name"

    static member tryGetTermCodeAsString(dt : LDNode, ?context : LDContext) =
        match dt.TryGetPropertyAsSingleton(DefinedTerm.termCode, ?context = context) with
        | Some (:? string as tc) -> Some tc
        | _ -> None

    static member getTermCodeAsString(dt : LDNode, ?context : LDContext) =
        match dt.TryGetPropertyAsSingleton(DefinedTerm.termCode, ?context = context) with
        | Some (:? string as tc) -> tc
        | Some _ -> failwith $"Property of `termCode` of object with @id `{dt.Id}` was not a string"
        | _ -> failwith $"Could not access property `termCode` of object with @id `{dt.Id}`"

    static member setTermCodeAsString(dt : LDNode, termCode : string, ?context : LDContext) =
        dt.SetProperty(DefinedTerm.termCode, termCode, ?context = context)

    static member tryGetNameAsString(dt : LDNode, ?context : LDContext) =
        match dt.TryGetPropertyAsSingleton(DefinedTerm.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(dt : LDNode, ?context : LDContext) =
        match dt.TryGetPropertyAsSingleton(DefinedTerm.name, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Property of `name` of object with @id `{dt.Id}` was not a string"
        | _ -> failwith $"Could not access property `name` of object with @id `{dt.Id}`"

    static member setNameAsString(dt : LDNode, name : string, ?context : LDContext) =
        dt.SetProperty(DefinedTerm.name, name, ?context = context)

    static member validate(dt : LDNode, ?context : LDContext) =
        dt.HasType(DefinedTerm.schemaType, ?context = context)
        && dt.HasProperty(DefinedTerm.name, ?context = context)

    static member create(id : string, name : string, ?termCode : string, ?context : LDContext) =
        let dt = LDNode(id, ResizeArray [DefinedTerm.schemaType], ?context = context)
        dt.SetProperty(DefinedTerm.name, name, ?context = context)
        dt.SetOptionalProperty(DefinedTerm.termCode, termCode, ?context = context)
        dt