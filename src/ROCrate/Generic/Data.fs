namespace ARCtrl.ROCrate.Generic

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

///
[<AttachMembers>]
type Data =

    static member schemaType = "http://schema.org/MediaObject"

    static member name = "http://schema.org/name"

    static member comment = "http://schema.org/comment"

    static member disambiguatingDescription = "http://schema.org/disambiguatingDescription"

    static member encodingFormat = "http://schema.org/encodingFormat"

    static member tryGetNameAsString(dt : LDNode, ?context : LDContext) =
        match dt.TryGetPropertyAsSingleton(Data.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(dt : LDNode, ?context : LDContext) =
        match dt.TryGetPropertyAsSingleton(Data.name, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Property of `name` of object with @id `{dt.Id}` was not a string"
        | _ -> failwith $"Could not access property `name` of object with @id `{dt.Id}`"

    static member setNameAsString(dt : LDNode, name : string, ?context : LDContext) =
        dt.SetProperty(Data.name, name, ?context = context)

    static member getComments(dt : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = Comment.validate(ldObject, ?context = context)
        dt.GetPropertyNodes(Data.comment, filter = filter, ?graph = graph, ?context = context)

    static member setComments(dt : LDNode, comment : ResizeArray<LDNode>, ?context : LDContext) =
        dt.SetProperty(Data.comment, comment, ?context = context)

    static member tryGetDisambiguatingDescriptionAsString(dt : LDNode, ?context : LDContext) =
        match dt.TryGetPropertyAsSingleton(Data.disambiguatingDescription, ?context = context) with
        | Some (:? string as dd) -> Some dd
        | _ -> None

    static member setDisambiguatingDescriptionAsString(dt : LDNode, disambiguatingDescription : string, ?context : LDContext) =
        dt.SetProperty(Data.disambiguatingDescription, disambiguatingDescription, ?context = context)

    static member tryGetEncodingFormatAsString(dt : LDNode, ?context : LDContext) =
        match dt.TryGetPropertyAsSingleton(Data.encodingFormat, ?context = context) with
        | Some (:? string as ef) -> Some ef
        | _ -> None

    static member setEncodingFormatAsString(dt : LDNode, encodingFormat : string, ?context : LDContext) =
        dt.SetProperty(Data.encodingFormat, encodingFormat, ?context = context)

    static member validate(dt : LDNode, ?context : LDContext) =
        dt.HasType(Data.schemaType, ?context = context)
        && dt.HasProperty(Data.name, ?context = context)

    static member create(id : string, name : string, ?comments : ResizeArray<LDNode>, ?disambiguatingDescription : string, ?encodingFormat : string, ?context : LDContext) =
        let dt = LDNode(id, ResizeArray [Data.schemaType], ?context = context)
        dt.SetProperty(Data.name, name, ?context = context)
        dt.SetOptionalProperty(Data.comment, comments, ?context = context)
        dt.SetOptionalProperty(Data.disambiguatingDescription, disambiguatingDescription, ?context = context)
        dt.SetOptionalProperty(Data.encodingFormat, encodingFormat, ?context = context)
        dt

    //static member tryGetTermCodeAsString(dt : LDNode, ?context : LDContext) =
    //    match dt.TryGetProperty(DefinedTerm.termCode, ?context = context) with
    //    | Some (:? string as tc) -> Some tc
    //    | _ -> None

    //static member getTermCodeAsString(dt : LDNode, ?context : LDContext) =
    //    match dt.TryGetProperty(DefinedTerm.termCode, ?context = context) with
    //    | Some (:? string as tc) -> tc
    //    | Some _ -> failwith $"Property of `termCode` of object with @id `{dt.Id}` was not a string"
    //    | _ -> failwith $"Could not access property `termCode` of object with @id `{dt.Id}`"

    //static member setTermCodeAsString(dt : LDNode, termCode : string, ?context : LDContext) =
    //    dt.SetProperty(DefinedTerm.termCode, termCode, ?context = context)

    //static member tryGetNameAsString(dt : LDNode, ?context : LDContext) =
    //    match dt.TryGetProperty(DefinedTerm.name, ?context = context) with
    //    | Some (:? string as n) -> Some n
    //    | _ -> None

    //static member getNameAsString(dt : LDNode, ?context : LDContext) =
    //    match dt.TryGetProperty(DefinedTerm.name, ?context = context) with
    //    | Some (:? string as n) -> n
    //    | Some _ -> failwith $"Property of `name` of object with @id `{dt.Id}` was not a string"
    //    | _ -> failwith $"Could not access property `name` of object with @id `{dt.Id}`"

    //static member setNameAsString(dt : LDNode, name : string, ?context : LDContext) =
    //    dt.SetProperty(DefinedTerm.name, name, ?context = context)

    //static member validate(dt : LDNode, ?context : LDContext) =
    //    dt.HasType(DefinedTerm.schemaType, ?context = context)
    //    && dt.HasProperty(DefinedTerm.name, ?context = context)

    //static member create(id : string, name : string, ?termCode : string, ?context : LDContext) =
    //    let dt = LDNode(id, ResizeArray [DefinedTerm.schemaType], ?context = context)
    //    dt.SetProperty(DefinedTerm.name, name, ?context = context)
    //    dt.SetOptionalProperty(DefinedTerm.termCode, termCode, ?context = context)
    //    dt