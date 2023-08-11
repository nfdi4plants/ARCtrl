﻿namespace ARCtrl.ISA

open Fable.Core

[<AttachMembers>]
[<RequireQualifiedAccess>]
type CompositeCell = 
    /// ISA-TAB term columns as ontology annotation.
    ///
    /// https://isa-specs.readthedocs.io/en/latest/isatab.html#ontology-annotations
    | Term of OntologyAnnotation
    /// Single columns like Input, Output, ProtocolREF, .. .
    | FreeText of string
    /// ISA-TAB unit columns, consisting of value and unit as ontology annotation.
    ///
    /// https://isa-specs.readthedocs.io/en/latest/isatab.html#unit
    | Unitized of string*OntologyAnnotation


    static member fromValue(value : Value, ?unit : OntologyAnnotation) =
        match value,unit with
        | Value.Ontology t, None -> CompositeCell.Term t
        | Value.Int i, None -> CompositeCell.FreeText (string i)
        | Value.Int i, Some u -> CompositeCell.Unitized(string i, u)
        | Value.Float f, None -> CompositeCell.FreeText (string f)
        | Value.Float f, Some u -> CompositeCell.Unitized(string f, u)
        | Value.Name s, None -> CompositeCell.FreeText s
        | _ -> failwith "could not convert value to cell, invalid combination of value and unit"

    member this.isUnitized = match this with | Unitized _ -> true | _ -> false
    member this.isTerm = match this with | Term _ -> true | _ -> false
    member this.isFreeText = match this with | FreeText _ -> true | _ -> false

    /// FreeText string will be converted to unit term name.
    ///
    /// Term will be converted to unit term.
    member this.ToUnitizedCell() =
        match this with
        | Unitized _ -> this
        | FreeText text -> CompositeCell.Unitized ("", OntologyAnnotation.create(Name = AnnotationValue.Text text))
        | Term term -> CompositeCell.Unitized ("", term)

    /// FreeText string will be converted to term name.
    ///
    /// Unit term will be converted to term and unit value is dropped.
    member this.ToTermCell() =
        match this with
        | Term _ -> this
        | Unitized (_,unit) -> CompositeCell.Term unit
        | FreeText text -> CompositeCell.Term(OntologyAnnotation.create(Name = AnnotationValue.Text text))

    /// Will always keep `OntologyAnnotation.NameText` from Term or Unit.
    member this.ToFreeTextCell() =
        match this with
        | FreeText _ -> this
        | Term term -> FreeText(term.NameText)
        | Unitized (v,unit) -> FreeText(unit.NameText)

    // Suggest this syntax for easy "of-something" access
    member this.AsUnitized  =
        match this with
        | Unitized (v,u) -> v,u
        | _ -> failwith "Not a Unitized cell."

    member this.AsTerm =
        match this with
        | Term c -> c
        | _ -> failwith "Not a Swate TermCell."

    member this.AsFreeText =
        match this with
        | FreeText c -> c
        | _ -> failwith "Not a Swate TermCell."

    // TODO: i would really love to have an overload here accepting string input
    static member createTerm (oa:OntologyAnnotation) = Term oa
    static member createTermFromString (?name: string, ?tsr: string, ?tan: string) =
        Term <| OntologyAnnotation.fromString(?termName = name, ?tsr = tsr, ?tan = tan)
    static member createUnitized (value: string, ?oa:OntologyAnnotation) = Unitized (value, Option.defaultValue (OntologyAnnotation.empty) oa)
    static member createUnitizedFromString (value: string, ?name: string, ?tsr: string, ?tan: string) = 
        Unitized <| (value, OntologyAnnotation.fromString(?termName = name, ?tsr = tsr, ?tan = tan))
    static member createFreeText (value: string) = FreeText value
    
    static member emptyTerm = Term OntologyAnnotation.empty
    static member emptyFreeText = FreeText ""
    static member emptyUnitized = Unitized ("", OntologyAnnotation.empty)

    override this.ToString() = 
        match this with
        | Term oa -> $"{oa.NameText}"
        | FreeText s -> s
        | Unitized (v,oa) -> $"{v} {oa.NameText}"