namespace ISA

type CompositeCell = 
    /// ISA-TAB term columns
    ///
    /// https://isa-specs.readthedocs.io/en/latest/isatab.html#ontology-annotations
    | Term of OntologyAnnotation
    /// Single columns like Input, Output, ProtocolREF, ..
    | FreeText of string
    /// ISA-TAB unit columns
    ///
    /// https://isa-specs.readthedocs.io/en/latest/isatab.html#unit
    | Unit of string*OntologyAnnotation

    member this.isUnit = match this with | Unit _ -> true | _ -> false
    member this.isTerm = match this with | Term _ -> true | _ -> false
    member this.isFreetext = match this with | FreeText _ -> true | _ -> false

    /// FreeText string will be converted to unit term name.
    ///
    /// Term will be converted to unit term.
    member this.toUnitCell() =
        match this with
        | Unit _ -> this
        | FreeText text -> CompositeCell.Unit ("", OntologyAnnotation.create(Name = AnnotationValue.Text text))
        | Term term -> CompositeCell.Unit ("", term)
    /// FreeText string will be converted to term name.
    ///
    /// Unit term will be converted to term and unit value is dropped.
    member this.toTermCell() =
        match this with
        | Term _ -> this
        | Unit (_,unit) -> CompositeCell.Term unit
        | FreeText text -> CompositeCell.Term(OntologyAnnotation.create(Name = AnnotationValue.Text text))
    /// Will always keep `OntologyAnnotation.NameText` from Term or Unit.
    member this.toFreetextCell() =
        match this with
        | FreeText _ -> this
        | Term term -> FreeText(term.NameText)
        | Unit (v,unit) -> FreeText(unit.NameText)
    /// Suggest this syntax for easy "of-something" access
    member this.AsUnit  =
        match this with
        | Unit (v,u) -> v,u
        | _ -> failwith "Not a WithUnit cell."
    member this.AsTerm =
        match this with
        | Term c -> c
        | _ -> failwith "Not a Swate TermCell."
    member this.AsFreetext =
        match this with
        | FreeText c -> c
        | _ -> failwith "Not a Swate TermCell."
    static member createTerm (oa:OntologyAnnotation) = Term oa
    static member createUnit (value: string, ?oa:OntologyAnnotation) = Unit (value, Option.defaultValue (OntologyAnnotation.empty) oa)
    static member createFreeText (value: string) = FreeText value
