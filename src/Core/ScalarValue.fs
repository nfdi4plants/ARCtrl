namespace ARCtrl

open ARCtrl
open Fable.Core
open System.Globalization
 
open ARCtrl.Helper 


[<AttachMembers>]
type ScalarValue =
    | Ontology of OntologyAnnotation
    | Int of int
    | Float of float
    | Name of string

    static member fromString (value : string) =
        match System.Int32.TryParse value  with
        | (true, i) -> ScalarValue.Int i
        | _ -> 
            let parser =
                #if FABLE_COMPILER
                fun (v: string) -> System.Double.TryParse(v)
                #else
                fun (v: string) -> System.Double.TryParse(v,NumberStyles.Any,CultureInfo.InvariantCulture) // this throws warnings in fable as options are ignored
                #endif
            match parser value with
            | (true, f) -> ScalarValue.Float f
            | _ -> ScalarValue.Name value

    static member fromOptions (value : string Option) (termSource: string Option) (termAccesssion: string Option) =
        match value, termSource, termAccesssion with
        | Some value, None, None ->
            ScalarValue.fromString value
            |> Some
        | None, None, None -> 
            None
        | _ -> 
            OntologyAnnotation.create (Option.defaultValue "" value, ?tsr = termSource, ?tan = termAccesssion)
            |> ScalarValue.Ontology
            |> Some

    static member toOptions (value : ScalarValue) =
        match value with
        | Ontology oa -> oa.Name,oa.TermAccessionNumber,oa.TermSourceREF
        | Int i -> string i |> Some, None, None
        | Float f -> string f |> Some, None, None
        | Name s -> s |> Some, None, None

    member this.Text =         
        match this with
        | ScalarValue.Ontology oa  -> oa.NameText
        | ScalarValue.Float f -> string f
        | ScalarValue.Int i   -> string i
        | ScalarValue.Name s  -> s

    member this.AsName() =         
        match this with
        | ScalarValue.Name s  -> s
        | _ -> failwith $"Value {this} is not of case name"

    member this.AsInt() =         
        match this with           
        | ScalarValue.Int i   -> i
        | _ -> failwith $"Value {this} is not of case int"

    member this.AsFloat() = 
        match this with
        | ScalarValue.Float f -> f
        | _ -> failwith $"Value {this} is not of case float"

    member this.AsOntology() =         
        match this with
        | ScalarValue.Ontology oa  -> oa
        | _ -> failwith $"Value {this} is not of case ontology"

    member this.IsAnOntology = 
        match this with
        | Ontology oa -> true
        | _ -> false

    member this.IsNumerical = 
        match this with
        | Int _ | Float _ -> true
        | _ -> false

    member this.IsAnInt = 
        match this with
        | Int _ -> true
        | _ -> false

    member this.IsAFloat = 
        match this with
        | Float _ -> true
        | _ -> false

    member this.IsAText = 
        match this with
        | Name _ -> true
        | _ -> false

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            match this with
            | Ontology oa   -> oa.NameText
            | Int i         -> sprintf "%i" i
            | Float f       -> sprintf "%f" f        
            | Name n        -> n

    static member getText (v: ScalarValue) =
        v.Text
