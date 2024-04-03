namespace ARCtrl

open ARCtrl
open ARCtrl.Helper
open Fable.Core

/// This module contains unchecked helper functions for templates
[<RequireQualifiedAccess>]
module TemplatesAux =

    let getComparer (matchAll: bool option) =
        let matchAll = defaultArg matchAll false
        let comparer = if matchAll then (&&) else (||)
        comparer

    let filterOnTags (tagGetter: Template -> OntologyAnnotation ResizeArray) (queryTags: OntologyAnnotation ResizeArray) (comparer: bool -> bool -> bool) (templates: Template ResizeArray) =
        templates 
        |> ResizeArray.filter(fun t ->
            let templateTags = tagGetter t
            let mutable isValid = None
            for qt in queryTags do
                let contains = templateTags |> Seq.contains qt
                match isValid, contains with
                | None, any -> isValid <- Some any
                | Some maybe, any -> isValid <- Some (comparer maybe any)
            Option.defaultValue false isValid
        )

[<AttachMembers>]
type Templates =

    static member getDistinctTags (templates: Template ResizeArray) =
        templates |> ResizeArray.collect (fun t -> t.Tags) |> ResizeArray.distinct

    /// <summary>
    /// Returns all **distinct** `template.Tags` and `template.EndpointRepositories`
    /// </summary>
    /// <param name="templates"></param>
    static member getDistinctEndpointRepositories (templates: Template ResizeArray) =
        templates |> ResizeArray.collect (fun t -> t.EndpointRepositories) |> ResizeArray.distinct

    /// <summary>
    /// Returns all **distinct** `template.Tags` and `template.EndpointRepositories`
    /// </summary>
    /// <param name="templates"></param>
    static member getDistinctOntologyAnnotations (templates: Template []) =
        let oas = ResizeArray()
        for t in templates do
            oas.AddRange(t.Tags)
            oas.AddRange(t.EndpointRepositories)
        oas
        |> Array.ofSeq
        |> Array.distinct

    /// <summary>
    /// Filter templates by `template.Tags`.
    /// </summary>
    /// <param name="queryTags">The ontology annotation to filter by.</param>
    /// <param name="matchAll">Default: false. If true all `queryTags` must be contained in template, if false only 1 tags must be contained in template.</param>
    static member filterByTags(queryTags: OntologyAnnotation ResizeArray, ?matchAll: bool) = 
        fun (templates: Template ResizeArray) ->
            let comparer = TemplatesAux.getComparer matchAll
            TemplatesAux.filterOnTags (fun t -> t.Tags) queryTags comparer templates

    /// <summary>
    /// Filter templates by `template.EndpointRepositories`.
    /// </summary>
    /// <param name="queryTags">The ontology annotation to filter by.</param>
    /// <param name="matchAll">Default: false. If true all `queryTags` must be contained in template, if false only 1 tags must be contained in template.</param>
    static member filterByEndpointRepositories(queryTags: OntologyAnnotation ResizeArray, ?matchAll: bool) = 
        fun (templates: Template ResizeArray) ->
            let comparer = TemplatesAux.getComparer matchAll
            TemplatesAux.filterOnTags (fun t -> t.EndpointRepositories) queryTags comparer templates

    /// <summary>
    /// Filters templates by template.Tags and template.EndpointRepositories
    /// </summary>
    /// <param name="queryTags">The ontology annotation to filter by.</param>
    /// <param name="matchAll">Default: false. If true all `queryTags` must be contained in template, if false only 1 tags must be contained in template.</param>
    static member filterByOntologyAnnotation(queryTags: OntologyAnnotation ResizeArray, ?matchAll: bool) = 
        fun (templates: Template ResizeArray) ->
            let comparer = TemplatesAux.getComparer matchAll
            TemplatesAux.filterOnTags (fun t -> ResizeArray.append t.Tags t.EndpointRepositories) queryTags comparer templates

    /// <summary>
    /// Filters templates by template.Organisation = `Organisation.DataPLANT`/`"DataPLANT"`.
    /// </summary>
    /// <param name="templates"></param>
    static member filterByDataPLANT (templates: Template ResizeArray) =
        templates
        |> ResizeArray.filter (fun t -> t.Organisation.IsOfficial())
        