namespace ARCtrl

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
    | Data of Data

    member this.isUnitized = match this with | Unitized _ -> true | _ -> false
    member this.isTerm = match this with | Term _ -> true | _ -> false
    member this.isFreeText = match this with | FreeText _ -> true | _ -> false
    member this.isData = match this with | Data _ -> true | _ -> false

    /// <summary>
    /// This returns the default empty cell from an existing CompositeCell.
    /// </summary>
    member this.GetEmptyCell() =
        match this with
        | CompositeCell.Term _ -> CompositeCell.emptyTerm
        | CompositeCell.Unitized _ -> CompositeCell.emptyUnitized
        | CompositeCell.FreeText _ -> CompositeCell.emptyFreeText
        | CompositeCell.Data _ -> CompositeCell.emptyData

    /// <summary>
    /// This function returns an array of all values as string
    ///
    /// ```fsharp
    /// match this with
    /// | FreeText s -> [|s|]
    /// | Term oa -> [| oa.NameText; defaultArg oa.TermSourceREF ""; defaultArg oa.TermAccessionNumber ""|]
    /// | Unitized (v,oa) -> [| v; oa.NameText; defaultArg oa.TermSourceREF ""; defaultArg oa.TermAccessionNumber ""|]
    /// ```
    /// </summary>
    member this.GetContent() = 
        match this with
        | FreeText s -> [|s|]
        | Term oa -> [| oa.NameText; defaultArg oa.TermSourceREF ""; defaultArg oa.TermAccessionNumber ""|]
        | Unitized (v,oa) -> [| v; oa.NameText; defaultArg oa.TermSourceREF ""; defaultArg oa.TermAccessionNumber ""|]
        | Data d -> [| defaultArg d.Name ""; defaultArg d.Format ""; defaultArg d.SelectorFormat ""|]

    /// FreeText string will be converted to unit term name.
    ///
    /// Term will be converted to unit term.
    member this.ToUnitizedCell() =
        match this with
        | Unitized _ -> this
        | FreeText text -> CompositeCell.Unitized ("", OntologyAnnotation.create(text))
        | Term term -> CompositeCell.Unitized ("", term)
        | Data d -> CompositeCell.Unitized ("", OntologyAnnotation.create(d.NameText))

    /// FreeText string will be converted to term name.
    ///
    /// Unit term will be converted to term and unit value is dropped.
    member this.ToTermCell() =
        match this with
        | Term _ -> this
        | Unitized (_,unit) -> CompositeCell.Term unit
        | FreeText text -> CompositeCell.Term(OntologyAnnotation.create(text))
        | Data d -> CompositeCell.Term(OntologyAnnotation(d.NameText))

    /// Will always keep `OntologyAnnotation.NameText` from Term or Unit.
    member this.ToFreeTextCell() =
        match this with
        | FreeText _ -> this
        | Term term -> FreeText(term.NameText)
        | Unitized (_,unit) -> FreeText(unit.NameText)
        | Data d -> FreeText (d.NameText)

    member this.ToDataCell() =
        match this with
        | Unitized (_, unit) -> CompositeCell.createDataFromString unit.NameText
        | FreeText txt -> CompositeCell.createDataFromString txt
        | Term term -> CompositeCell.createDataFromString term.NameText
        | Data _ -> this

    // Suggest this syntax for easy "of-something" access
    member this.AsUnitized  =
        match this with
        | Unitized (v,u) -> v,u
        | _ -> failwith "Not a Unitized cell."

    member this.AsTerm =
        match this with
        | Term c -> c
        | _ -> failwith "Not a Term Cell."

    member this.AsFreeText =
        match this with
        | FreeText c -> c
        | _ -> failwith "Not a FreeText Cell."

    member this.AsData =
        match this with
        | Data d -> d
        | _ -> failwith "Not a Data Cell."

    // TODO: i would really love to have an overload here accepting string input
    static member createTerm (oa:OntologyAnnotation) = Term oa

    static member createTermFromString (?name: string, ?tsr: string, ?tan: string) =
        Term <| OntologyAnnotation.create(?name = name, ?tsr = tsr, ?tan = tan)

    static member createUnitized (value: string, ?oa:OntologyAnnotation) = Unitized (value, Option.defaultValue (OntologyAnnotation()) oa)

    static member createUnitizedFromString (value: string, ?name: string, ?tsr: string, ?tan: string) = 
        Unitized <| (value, OntologyAnnotation.create(?name = name, ?tsr = tsr, ?tan = tan))

    static member createFreeText (value: string) = FreeText value
    
    static member createData (d:Data) = Data d

    static member createDataFromString (value : string, ?format : string, ?selectorFormat : string) =
        Data(Data.create(Name = value, ?Format = format, ?SelectorFormat = selectorFormat))

    static member emptyTerm = Term (OntologyAnnotation())
    static member emptyFreeText = FreeText ""
    static member emptyUnitized = Unitized ("", OntologyAnnotation())
    static member emptyData = Data(Data.create())

    /// <summary>
    /// Updates current CompositeCell with information from OntologyAnnotation.
    ///
    /// For `Term`, OntologyAnnotation (oa) is fully set. For `Unitized`, oa is set as unit while value is untouched.
    /// For `FreeText` oa.NameText is set.
    /// </summary>
    /// <param name="oa"></param>
    member this.UpdateWithOA (oa:OntologyAnnotation) =
        match this with
        | CompositeCell.Term _ -> CompositeCell.createTerm oa
        | CompositeCell.Unitized (v,_) -> CompositeCell.createUnitized (v,oa)
        | CompositeCell.FreeText _ -> CompositeCell.createFreeText oa.NameText
        | CompositeCell.Data d ->
            d.Name <- Some oa.NameText
            CompositeCell.Data d

    /// <summary>
    /// Updates current CompositeCell with information from OntologyAnnotation.
    ///
    /// For `Term`, OntologyAnnotation (oa) is fully set. For `Unitized`, oa is set as unit while value is untouched.
    /// For `FreeText` oa.NameText is set.
    /// </summary>
    /// <param name="oa"></param>
    /// <param name="cell"></param>
    static member updateWithOA (oa:OntologyAnnotation) (cell: CompositeCell) =
        cell.UpdateWithOA oa

    override this.ToString() = 
        match this with
        | Term oa -> $"{oa.NameText}"
        | FreeText s -> s
        | Unitized (v,oa) -> $"{v} {oa.NameText}"
        | Data d -> $"{d.NameText}"

    member this.Copy() =
        match this with
        | Term oa -> Term (oa.Copy())
        | FreeText s -> FreeText s
        | Unitized (v,oa) -> Unitized (v, oa.Copy())
        | Data d -> Data (d.Copy())

#if FABLE_COMPILER
    //[<CompiledName("Term")>]
    static member term (oa:OntologyAnnotation) = CompositeCell.Term(oa)

    //[<CompiledName("FreeText")>]
    static member freeText (s:string) = CompositeCell.FreeText(s)

    //[<CompiledName("Unitized")>]
    static member unitized (v:string, oa:OntologyAnnotation) = CompositeCell.Unitized(v, oa)

    //[<CompiledName("Data")>]
    static member data (d:Data) = CompositeCell.Data(d)
#else
#endif