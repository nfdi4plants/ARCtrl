namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

//Is based on the Bioschemas bioschemas.org/LabProtocol type and maps to the ISA-JSON Protocol

//Property	Required	Expected Type	Description
//@id	MUST	Text or URL	Could be the url pointing to the protocol resource.
//@type	MUST	Text	must be 'bioschemas.org/LabProtocol'
//description	SHOULD	Text	A short description of the protocol (e.g. an abstract)
//intendedUse	SHOULD	schema.org/DefinedTerm or Text or URL	The protocol type as an ontology term
//name	SHOULD	Text	Main title of the LabProtocol.
//comment	COULD	schema.org/Comment	Comment
//computationalTool	COULD	schema.org/DefinedTerm or schema.org/PropertyValue or schema.org/SoftwareApplication	Software or tool used as part of the lab protocol to complete a part of it.
//labEquipment	COULD	schema.org/DefinedTerm or schema.org/PropertyValue or Text or URL	For LabProtocols it would be a laboratory equipment use by a person to follow one or more steps described in this LabProtocol.
//reagent	COULD	schema.org/BioChemEntity or schema.org/DefinedTerm or schema.org/PropertyValue or Text or URL	Reagents used in the protocol.
//url	COULD	URL	Pointer to protocol resources external to the ISA-Tab that can be accessed by their Uniform Resource Identifier (URI).
//version	COULD	Number or Text	An identifier for the version to ensure protocol tracking.


[<AttachMembers>]
type LabProtocol =

    static member schemaType = "https://bioschemas.org/LabProtocol"

    static member description = "http://schema.org/description"

    static member intendedUse = "http://schema.org/intendedUse"

    static member name = "http://schema.org/name"

    static member comment = "http://schema.org/comment"

    static member computationalTool = "http://schema.org/computationalTool"

    static member labEquipment = "http://schema.org/labEquipment"

    static member reagent = "http://schema.org/reagent"

    static member url = "http://schema.org/url"

    static member version = "http://schema.org/version"


    static member tryGetDescriptionAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LabProtocol.description, ?context = context) with
        | Some (:? string as d) -> Some d
        | _ -> None

    static member setDescriptionAsString(lp : LDNode, description : string, ?context : LDContext) =
        lp.SetProperty(LabProtocol.description, description, ?context = context)

    static member tryGetIntendedUseAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LabProtocol.intendedUse, ?context = context) with
        | Some (:? string as iu) -> Some iu
        | _ -> None

    static member setIntendedUseAsString(lp : LDNode, intendedUse : string, ?context : LDContext) =
        lp.SetProperty(LabProtocol.intendedUse, intendedUse, ?context = context)

    static member tryGetIntendedUseAsDefinedTerm(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = DefinedTerm.validate(ldObject, ?context = context)
        match lp.TryGetPropertyAsSingleNode(LabProtocol.intendedUse, ?graph = graph, ?context = context) with
        | Some iu when filter iu context -> Some iu
        | _ -> None

    static member setIntendedUseAsDefinedTerm(lp : LDNode, intendedUse : LDNode, ?context : LDContext) =
        lp.SetProperty(LabProtocol.intendedUse, intendedUse, ?context = context)

    static member tryGetNameAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LabProtocol.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LabProtocol.name, ?context = context) with
        | Some (:? string as n) -> n
        | _ -> failwith $"Could not access property `name` of object with @id `{lp.Id}`"

    static member setNameAsString(lp : LDNode, name : string, ?context : LDContext) =
        lp.SetProperty(LabProtocol.name, name, ?context = context)

    static member getComments(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let filter ldObject context = Comment.validate(ldObject, ?context = context)
        lp.GetPropertyNodes(LabProtocol.comment, filter = filter, ?graph = graph, ?context = context)

    static member setComments(lp : LDNode, comments : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LabProtocol.comment, comments, ?context = context)

    static member getComputationalTools(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        lp.GetPropertyNodes(LabProtocol.computationalTool, ?graph = graph, ?context = context)
        
    static member setComputationalTools(lp : LDNode, computationalTools : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LabProtocol.computationalTool, computationalTools, ?context = context)

    static member getLabEquipments(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        lp.GetPropertyNodes(LabProtocol.labEquipment, ?graph = graph, ?context = context)

    static member setLabEquipments(lp : LDNode, labEquipments : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LabProtocol.labEquipment, labEquipments, ?context = context)

    static member getReagents(lp : LDNode, ?graph : LDGraph, ?context : LDContext) =
        lp.GetPropertyNodes(LabProtocol.reagent, ?graph = graph, ?context = context)

    static member setReagents(lp : LDNode, reagents : ResizeArray<LDNode>, ?context : LDContext) =
        lp.SetProperty(LabProtocol.reagent, reagents, ?context = context)

    static member tryGetUrl(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LabProtocol.url, ?context = context) with
        | Some (:? string as u) -> Some u
        | _ -> None

    static member setUrl(lp : LDNode, url : string, ?context : LDContext) =
        lp.SetProperty(LabProtocol.url, url, ?context = context)

    static member tryGetVersionAsString(lp : LDNode, ?context : LDContext) =
        match lp.TryGetPropertyAsSingleton(LabProtocol.version, ?context = context) with
        | Some (:? string as v) -> Some v
        | _ -> None

    static member setVersionAsString(lp : LDNode, version : string, ?context : LDContext) =
        lp.SetProperty(LabProtocol.version, version, ?context = context)

    static member validate(lp : LDNode, ?context : LDContext) =
        lp.HasType(LabProtocol.schemaType, ?context = context)
        //&& lp.HasProperty(LabProtocol.name, ?context = context)

    static member create(id : string, ?name : string, ?description : string, ?intendedUse : string, ?comments : ResizeArray<LDNode>, ?computationalTools : ResizeArray<LDNode>, ?labEquipments : ResizeArray<LDNode>, ?reagents : ResizeArray<LDNode>, ?url : string, ?version : string, ?context : LDContext) =
        let lp = LDNode(id, ResizeArray [LabProtocol.schemaType], ?context = context)
        lp.SetOptionalProperty(LabProtocol.name, name, ?context = context)
        lp.SetOptionalProperty(LabProtocol.description, description, ?context = context)
        lp.SetOptionalProperty(LabProtocol.intendedUse, intendedUse, ?context = context)
        lp.SetOptionalProperty(LabProtocol.comment, comments, ?context = context)
        lp.SetOptionalProperty(LabProtocol.computationalTool, computationalTools, ?context = context)
        lp.SetOptionalProperty(LabProtocol.labEquipment, labEquipments, ?context = context)
        lp.SetOptionalProperty(LabProtocol.reagent, reagents, ?context = context)
        lp.SetOptionalProperty(LabProtocol.url, url, ?context = context)
        lp.SetOptionalProperty(LabProtocol.version, version, ?context = context)
        lp