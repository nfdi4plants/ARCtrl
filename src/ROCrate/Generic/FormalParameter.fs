namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

[<AttachMembers>]
type LDFormalParameter =

    static member schemaType = "https://bioschemas.org/FormalParameter"
    static member additionalType = "http://schema.org/additionalType"
    // Recommended properties
    static member encodingFormat = "http://schema.org/encodingFormat"
    // Optional properties
    static member name = "http://schema.org/name"
    static member sameAs = "http://schema.org/sameAs"
    static member description = "http://schema.org/description"
    static member workExample = "http://schema.org/workExample"
    static member defaultValue = "http://schema.org/defaultValue"
    static member valueRequired = "http://schema.org/valueRequired"
    static member identifier = "http://schema.org/identifier"
    static member image = "http://schema.org/image"
    static member mainEntityOfPage = "http://schema.org/mainEntityOfPage"
    static member potentialAction = "http://schema.org/potentialAction"
    static member subjectOf = "http://schema.org/subjectOf"
    static member url = "http://schema.org/url"
    static member alternateName = "http://schema.org/alternateName"
    static member disambiguatingDescription = "http://schema.org/disambiguatingDescription"

    static member getAdditionalType(fp : LDNode, ?context : LDContext) =
        match fp.TryGetPropertyAsSingleton(LDFormalParameter.additionalType, ?context = context) with
        | Some (:? string as at) -> at
        | _ -> failwith $"property `additionalType` of object with @id `{fp.Id}` was not a string"

    static member setAdditionalTypeAsString(fp : LDNode, additionalType : string, ?context : LDContext) =
        fp.SetProperty(LDFormalParameter.additionalType, additionalType, ?context = context)

    static member getEncodingFormats(fp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        fp.GetPropertyNodes(LDFormalParameter.encodingFormat, ?graph = graph, ?context = context)

    static member setEncodingFormats(fp : LDNode, encodingFormats : ResizeArray<LDNode>, ?context : LDContext) =
        fp.SetProperty(LDFormalParameter.encodingFormat, encodingFormats, ?context = context)

    static member tryGetNameAsString(fp : LDNode, ?context : LDContext) =
        match fp.TryGetPropertyAsSingleton(LDFormalParameter.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member setNameAsString(fp : LDNode, name : string, ?context : LDContext) =
        fp.SetProperty(LDFormalParameter.name, name, ?context = context)

    static member getSameAs(fp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        fp.GetPropertyNodes(LDFormalParameter.sameAs, ?graph = graph, ?context = context)

    static member setSameAs(fp : LDNode, sames : ResizeArray<LDNode>, ?context : LDContext) =
        fp.SetProperty(LDFormalParameter.sameAs, sames, ?context = context)

    static member tryGetDescriptionAsString(fp : LDNode, ?context : LDContext) =
        match fp.TryGetPropertyAsSingleton(LDFormalParameter.description, ?context = context) with
        | Some (:? string as d) -> Some d
        | _ -> None

    static member setDescriptionAsString(fp : LDNode, description : string, ?context : LDContext) =
        fp.SetProperty(LDFormalParameter.description, description, ?context = context)

    static member tryGetWorkExampleAsString(fp : LDNode, ?context : LDContext) =
        match fp.TryGetPropertyAsSingleton(LDFormalParameter.workExample, ?context = context) with
        | Some (:? string as we) -> Some we
        | _ -> None

    static member setWorkExampleAsString(fp : LDNode, workExample : string, ?context : LDContext) =
        fp.SetProperty(LDFormalParameter.workExample, workExample, ?context = context)

    static member tryGetDefaultValueAsString(fp : LDNode, ?context : LDContext) =
        match fp.TryGetPropertyAsSingleton(LDFormalParameter.defaultValue, ?context = context) with
        | Some (:? string as dv) -> Some dv
        | _ -> None

    static member setDefaultValueAsString(fp : LDNode, defaultValue : string, ?context : LDContext) =
        fp.SetProperty(LDFormalParameter.defaultValue, defaultValue, ?context = context)

    static member tryGetValueRequiredAsBoolean(fp : LDNode, ?context : LDContext) =
        match fp.TryGetPropertyAsSingleton(LDFormalParameter.valueRequired, ?context = context) with
        | Some (:? bool as vr) -> Some vr
        | _ -> None

    static member setValueRequiredAsBoolean(fp : LDNode, valueRequired : bool, ?context : LDContext) =
        fp.SetProperty(LDFormalParameter.valueRequired, valueRequired, ?context = context)

    static member getIdentifiers(fp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        fp.GetPropertyNodes(LDFormalParameter.identifier, ?graph = graph, ?context = context)

    static member setIdentifiers(fp : LDNode, identifiers : ResizeArray<LDNode>, ?context : LDContext) =
        fp.SetProperty(LDFormalParameter.identifier, identifiers, ?context = context)

    static member tryGetImageAsString(fp : LDNode, ?context : LDContext) =
        match fp.TryGetPropertyAsSingleton(LDFormalParameter.image, ?context = context) with
        | Some (:? string as img) -> Some img
        | _ -> None

    static member setImageAsString(fp : LDNode, image : string, ?context : LDContext) =
        fp.SetProperty(LDFormalParameter.image, image, ?context = context)
    static member tryGetMainEntityOfPageAsString(fp : LDNode, ?context : LDContext) =
        match fp.TryGetPropertyAsSingleton(LDFormalParameter.mainEntityOfPage, ?context = context) with
        | Some (:? string as meop) -> Some meop
        | _ -> None

    static member setMainEntityOfPageAsString(fp : LDNode, mainEntityOfPage : string, ?context : LDContext) =
        fp.SetProperty(LDFormalParameter.mainEntityOfPage, mainEntityOfPage, ?context = context)

    static member getPotentialActions(fp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        fp.GetPropertyNodes(LDFormalParameter.potentialAction, ?graph = graph, ?context = context)

    static member setPotentialActions(fp : LDNode, potentialActions : ResizeArray<LDNode>, ?context : LDContext) =
        fp.SetProperty(LDFormalParameter.potentialAction, potentialActions, ?context = context)

    static member getSubjectOfs(fp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        fp.GetPropertyNodes(LDFormalParameter.subjectOf, ?graph = graph, ?context = context)

    static member setSubjectOfs (fp : LDNode, subjectOfs : ResizeArray<LDNode>, ?context : LDContext) =
        fp.SetProperty(LDFormalParameter.subjectOf, subjectOfs, ?context = context)

    static member tryGetUrlAsString(fp : LDNode, ?context : LDContext) =
        match fp.TryGetPropertyAsSingleton(LDFormalParameter.url, ?context = context) with
        | Some (:? string as url) -> Some url
        | _ -> None

    static member setUrlAsString(fp : LDNode, url : string, ?context : LDContext) =
        fp.SetProperty(LDFormalParameter.url, url, ?context = context)

    static member getAlternateNames(fp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        fp.GetPropertyNodes(LDFormalParameter.alternateName, ?graph = graph, ?context = context)

    static member setAlternateNames(fp : LDNode, alternateNames : ResizeArray<LDNode>, ?context : LDContext) =
        fp.SetProperty(LDFormalParameter.alternateName, alternateNames, ?context = context)

    static member tryGetDisambiguatingDescriptionAsString(fp : LDNode, ?context : LDContext) =
        match fp.TryGetPropertyAsSingleton(LDFormalParameter.disambiguatingDescription, ?context = context) with
        | Some (:? string as desc) -> Some desc
        | _ -> None

    static member setDisambiguatingDescriptionAsString(fp : LDNode, disambiguatingDescription : string, ?context : LDContext) =
        fp.SetProperty(LDFormalParameter.disambiguatingDescription, disambiguatingDescription, ?context = context)

    static member validate(fp : LDNode, ?context : LDContext) =
        fp.HasType(LDFormalParameter.schemaType, ?context = context)
        && fp.HasProperty(LDFormalParameter.additionalType, ?context = context)

    static member create(additionalType : string, ?id : string, ?encodingFormats : ResizeArray<LDNode>, ?name : string, ?sameAs : ResizeArray<LDNode>, ?description : string, ?workExample : string, ?defaultValue : string, ?valueRequired : bool, ?identifiers : ResizeArray<LDNode>, ?image : string, ?subjectOfs : ResizeArray<LDNode>, ?url : string, ?alternateNames : ResizeArray<LDNode>, ?disambiguatingDescription : string, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> $"#FormalParameter_{ARCtrl.Helper.Identifier.createMissingIdentifier()}" |> Helper.ID.clean
        let fp = LDNode(id, ResizeArray [LDFormalParameter.schemaType], ?context = context)
        fp.SetProperty(LDFormalParameter.additionalType, additionalType, ?context = context)
        fp.SetOptionalProperty(LDFormalParameter.encodingFormat, encodingFormats, ?context = context)
        fp.SetOptionalProperty(LDFormalParameter.name, name, ?context = context)
        fp.SetOptionalProperty(LDFormalParameter.sameAs, sameAs, ?context = context)
        fp.SetOptionalProperty(LDFormalParameter.description, description, ?context = context)
        fp.SetOptionalProperty(LDFormalParameter.workExample, workExample, ?context = context)
        fp.SetOptionalProperty(LDFormalParameter.defaultValue, defaultValue, ?context = context)
        fp.SetOptionalProperty(LDFormalParameter.valueRequired, valueRequired, ?context = context)
        fp.SetOptionalProperty(LDFormalParameter.identifier, identifiers, ?context = context)
        fp.SetOptionalProperty(LDFormalParameter.image, image, ?context = context)
        fp.SetOptionalProperty(LDFormalParameter.subjectOf, subjectOfs, ?context = context)
        fp.SetOptionalProperty(LDFormalParameter.url, url, ?context = context)
        fp.SetOptionalProperty(LDFormalParameter.alternateName, alternateNames, ?context = context)
        fp.SetOptionalProperty(LDFormalParameter.disambiguatingDescription, disambiguatingDescription, ?context = context)
        fp
    