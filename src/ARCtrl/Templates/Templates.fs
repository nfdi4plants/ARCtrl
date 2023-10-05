namespace ARCtrl.Template

open ARCtrl.ISA
open Fable.Core

/// This module contains unchecked helper functions for templates
[<RequireQualifiedAccess>]
module TemplatesAux =

    let getDefault_comparer (isAnd: bool option) =
        let isAnd = defaultArg isAnd false
        let comparer = if isAnd then (&&) else (||)
        comparer

    let filterOnTags (tagGetter: Template -> OntologyAnnotation []) (queryTags: OntologyAnnotation []) (comparer: bool -> bool -> bool) (templates: Template []) =
        templates 
        |> Array.filter(fun t ->
            let templateTags = tagGetter t
            let mutable isValid = None
            for qt in queryTags do
                let contains = templateTags |> Array.contains qt
                match isValid, contains with
                | None, any -> isValid <- Some any
                | Some maybe, any -> isValid <- Some (comparer maybe any)
            Option.defaultValue false isValid
        )

[<AttachMembers>]
type Templates =

    /// <summary>
    /// Filter templates by `template.Tags`.
    /// </summary>
    /// <param name="queryTags">The ontology annotation to filter by.</param>
    /// <param name="isAnd">Default: false. If true all `queryTags` must be contained in template, if false only 1 tags must be contained in template.</param>
    static member filterByTags(queryTags: OntologyAnnotation [], ?isAnd: bool) = 
        fun (templates: Template []) ->
            let comparer = TemplatesAux.getDefault_comparer isAnd
            TemplatesAux.filterOnTags (fun t -> t.Tags) queryTags comparer templates

    /// <summary>
    /// Filter templates by `template.EndpointRepositories`.
    /// </summary>
    /// <param name="queryTags">The ontology annotation to filter by.</param>
    /// <param name="isAnd">Default: false. If true all `queryTags` must be contained in template, if false only 1 tags must be contained in template.</param>
    static member filterByEndpointRepositories(queryTags: OntologyAnnotation [], ?isAnd: bool) = 
        fun (templates: Template []) ->
            let comparer = TemplatesAux.getDefault_comparer isAnd
            TemplatesAux.filterOnTags (fun t -> t.EndpointRepositories) queryTags comparer templates

    /// <summary>
    /// Filters templates by template.Tags and template.EndpointRepositories
    /// </summary>
    /// <param name="queryTags">The ontology annotation to filter by.</param>
    /// <param name="isAnd">Default: false. If true all `queryTags` must be contained in template, if false only 1 tags must be contained in template.</param>
    static member filterByOntologyAnnotation(queryTags: OntologyAnnotation [], ?isAnd: bool) = 
        fun (templates: Template []) ->
            let comparer = TemplatesAux.getDefault_comparer isAnd
            TemplatesAux.filterOnTags (fun t -> Array.append t.Tags t.EndpointRepositories) queryTags comparer templates

    /// <summary>
    /// Filters templates by template.Organisation = `Organisation.DataPLANT`/`"DataPLANT"`.
    /// </summary>
    /// <param name="templates"></param>
    static member filterByDataPLANT (templates: Template [])=
        templates
        |> Array.filter (fun t -> t.Organisation.IsOfficial())
        