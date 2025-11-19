namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

[<AttachMembers>]
type LDComputerLanguage =

    static member schemaType = "http://schema.org/ComputerLanguage"
    // Optional properties
    static member name = "http://schema.org/name"
    static member alternateName = "http://schema.org/alternateName"
    static member identifier = "http://schema.org/identifier"
    static member url = "http://schema.org/url"
    static member sameAs = "http://schema.org/sameAs"

    static member tryGetNameAsString(cl : LDNode, ?context : LDContext) =
        match cl.TryGetPropertyAsSingleton(LDFormalParameter.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(cl : LDNode, ?context : LDContext) =
        match cl.TryGetPropertyAsSingleton(LDComputerLanguage.name, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Property of `name` of object with @id `{cl.Id}` was not a string"
        | _ -> failwith $"Could not access property `name` of object with @id `{cl.Id}`"

    static member setNameAsString(cl : LDNode, name : string, ?context : LDContext) =
        cl.SetProperty(LDComputerLanguage.name, name, ?context = context)

    static member tryGetAlternateNameAsString(cl : LDNode, ?context : LDContext) =
        match cl.TryGetPropertyAsSingleton(LDComputerLanguage.alternateName, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getAlternateNameAsString(cl : LDNode, ?context : LDContext) =
        match cl.TryGetPropertyAsSingleton(LDComputerLanguage.alternateName, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Property of `alternateName` of object with @id `{cl.Id}` was not a string"
        | _ -> failwith $"Could not access property `alternateName` of object with @id `{cl.Id}`"

    static member setAlternateNameAsString(cl : LDNode, alternateName : string, ?context : LDContext) =
        cl.SetProperty(LDComputerLanguage.alternateName, alternateName, ?context = context)

    static member tryGetIdentifierAsLDRef(cl : LDNode, ?context : LDContext) =
        match cl.TryGetPropertyAsSingleton(LDComputerLanguage.identifier, ?context = context) with
        | Some (:? LDRef as id) -> Some id
        | _ -> None

    static member getIdentifierAsLDRef(cl : LDNode, ?context : LDContext) =
        match cl.TryGetPropertyAsSingleton(LDComputerLanguage.identifier, ?context = context) with
        | Some (:? LDRef as id) -> id
        | Some _ -> failwith $"Property of `identifier` of object with @id `{cl.Id}` was not a LDRef"
        | _ -> failwith $"Could not access property `identifier` of object with @id `{cl.Id}`"

    static member setIdentifierAsLDRef(cl : LDNode, identifier : LDRef, ?context : LDContext) =
        cl.SetProperty(LDComputerLanguage.identifier, identifier, ?context = context)

    static member tryGetUrlAsLDRef(cl : LDNode, ?context : LDContext) =
        match cl.TryGetPropertyAsSingleton(LDComputerLanguage.url, ?context = context) with
        | Some (:? LDRef as url) -> Some url
        | _ -> None

    static member getUrlAsLDRef(cl : LDNode, ?context : LDContext) =
        match cl.TryGetPropertyAsSingleton(LDComputerLanguage.url, ?context = context) with
        | Some (:? LDRef as url) -> url
        | Some _ -> failwith $"Property of `url` of object with @id `{cl.Id}` was not a LDRef"
        | _ -> failwith $"Could not access property `url` of object with @id `{cl.Id}`"

    static member setUrlAsLDRef(cl : LDNode, url : LDRef, ?context : LDContext) =
        cl.SetProperty(LDComputerLanguage.url, url, ?context = context)

    static member validate(cl : LDNode, ?context : LDContext) =
        cl.HasType(LDComputerLanguage.schemaType, ?context = context)

    static member validateCWL(cl : LDNode, ?context : LDContext) =
        LDComputerLanguage.validate(cl, ?context = context)
        && cl.Id = "https://w3id.org/workflowhub/workflow-ro-crate#cwl"

    static member validateGalaxy(cl : LDNode, ?context : LDContext) =
        LDComputerLanguage.validate(cl, ?context = context)
        && cl.Id = "https://w3id.org/workflowhub/workflow-ro-crate#galaxy"

    static member validateKNIME(cl : LDNode, ?context : LDContext) =
        LDComputerLanguage.validate(cl, ?context = context)
        && cl.Id = "https://w3id.org/workflowhub/workflow-ro-crate#knime"

    static member validateNextflow(cl : LDNode, ?context : LDContext) =
        LDComputerLanguage.validate(cl, ?context = context)
        && cl.Id = "https://w3id.org/workflowhub/workflow-ro-crate#nextflow"

    static member validateSnakemake(cl : LDNode, ?context : LDContext) =
        LDComputerLanguage.validate(cl, ?context = context)
        && cl.Id = "https://w3id.org/workflowhub/workflow-ro-crate#snakemake"

    static member create(id: string, ?name : string, ?alternateName : string, ?identifier : string, ?url : string, ?context : LDContext) =
        let cl = LDNode(id = id, schemaType = ResizeArray[|LDComputerLanguage.schemaType|], ?context = context)
        let identifier = identifier |> Option.map (fun id -> LDRef(id))
        let url = url |> Option.map (fun u -> LDRef(u))
        cl.SetOptionalProperty(LDComputerLanguage.name, name, ?context = context)
        cl.SetOptionalProperty(LDComputerLanguage.alternateName, alternateName, ?context = context)
        cl.SetOptionalProperty(LDComputerLanguage.identifier, identifier, ?context = context)
        cl.SetOptionalProperty(LDComputerLanguage.url, url, ?context = context)
        cl

    static member createCWL(?context : LDContext) =
        LDComputerLanguage.create(
            "https://w3id.org/workflowhub/workflow-ro-crate#cwl",
            name = "Common Workflow Language",
            alternateName = "CWL",
            identifier = "https://w3id.org/cwl/v1.2/",
            url = "https://www.commonwl.org/",
            ?context = context)

    static member createGalaxy(?context : LDContext) =
        LDComputerLanguage.create(
            "https://w3id.org/workflowhub/workflow-ro-crate#galaxy",
            name = "Galaxy",
            identifier = "https://galaxyproject.org/",
            url = "https://galaxyproject.org/",
            ?context = context)

    static member createKNIME(?context : LDContext) =
        LDComputerLanguage.create(
            "https://w3id.org/workflowhub/workflow-ro-crate#knime",
            name = "KNIME",
            identifier = "https://www.knime.com/",
            url = "https://www.knime.com/",
            ?context = context)

    static member createNextflow(?context : LDContext) =
        LDComputerLanguage.create(
            "https://w3id.org/workflowhub/workflow-ro-crate#nextflow",
            name = "Nextflow",
            identifier = "https://www.nextflow.io/",
            url = "https://www.nextflow.io/",
            ?context = context)

    static member createSnakemake(?context : LDContext) =
        LDComputerLanguage.create(
            "https://w3id.org/workflowhub/workflow-ro-crate#snakemake",
            name = "Snakemake",
            identifier = "https://doi.org/10.1093/bioinformatics/bts480",
            url = "https://snakemake.readthedocs.io",
            ?context = context)