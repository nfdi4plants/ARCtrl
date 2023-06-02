namespace ISA

// TODO: The design principles say we should use Erase for js compilation. This would also 

//[<Erase>]
//[<AttachMembersAttribute>]
// FABLE: Erased unions with multiple cases cannot have more than one field: Test.CompositeCell
//type CompositeCell = 
//    | Term of string
//    | Freetext of {| t: string; f: float |}
//    | WithUnit of {| t: string; b: bool |}

//    member this.getString =
//        match this with
//        | Term s -> s
//        | Freetext t -> t.t
//        | WithUnit t -> t.t

//let test_t = Term ("Test me")
//let test_f = Freetext {|t = "Test me"; f = 12.|}

//test_t.getString |> printfn "%A" // Does return "undefined"

// if we erase we cannot use members

type CompositeCell = 
    | Term of OntologyAnnotation
    | FreeText of string
    | Unit of string*OntologyAnnotation

    member this.isUnit = match this with | Unit _ -> true | _ -> false
    member this.isTerm = match this with | Term _ -> true | _ -> false
    member this.isFreetext = match this with | FreeText _ -> true | _ -> false

    member this.toUnitCell() =
        match this with
        | Unit _ -> this
        | FreeText text -> CompositeCell.Unit ("", OntologyAnnotation.create(Name = AnnotationValue.Text text))
        | Term term -> CompositeCell.Unit ("", term)
    /// Will drop value from unit
    member this.toTermCell() =
        match this with
        | Term _ -> this
        | Unit (_,unit) -> CompositeCell.Term unit
        | FreeText text -> CompositeCell.Term(OntologyAnnotation.create(Name = AnnotationValue.Text text))
    /// Will always keep `OntologyAnnotation.NameText` from Term or WithUnit.
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
