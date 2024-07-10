namespace ARCtrl.Process

open ARCtrl

open ARCtrl.Helper
open Regex.ActivePatterns

type Component = 
    {
        /// This can be the main column value of the component column. (e.g. "SCIEX instrument model" as `OntologyAnnotation`; 14;..)
        ComponentValue : Value option
        /// This can be the unit describing a non `OntologyAnnotation` value in `ComponentValue`. (e.g. "degree celcius")
        ComponentUnit : OntologyAnnotation option
        /// This can be the component column header (e.g. "instrument model")
        ComponentType : OntologyAnnotation option
    }

    member this.ComponentName = 
        this.ComponentValue
        |> Option.map (fun v -> Component.composeName v (this.ComponentUnit))
        
    static member make value unit componentType =
        {
            ComponentValue = value
            ComponentUnit = unit
            ComponentType = componentType
        }

    static member create (?value,?unit,?componentType) : Component =
        Component.make value unit componentType

    static member empty =
        Component.create()
    
    /// This function creates a string containing full isa term triplet information about the component
    ///
    /// Components do not have enough fields in ISA-JSON to include all existing ontology term information. 
    /// This function allows us, to add the same information as `Parameter`, `Characteristics`.., to `Component`. 
    /// Without this string composition we loose the ontology information for the header value.
    static member composeName (value : Value) (unit : OntologyAnnotation option) = 
        match value,unit with
        | Value.Ontology oa, _ ->
            $"{oa.NameText} ({oa.TermAccessionShort})"
        | v, None ->
            $"{v.Text}"
        | v, Some u ->
            $"{v.Text} {u.NameText} ({u.TermAccessionShort})"

    /// This function parses the given Component header string format into the ISA-JSON Component type
    ///
    /// Components do not have enough fields in ISA-JSON to include all existing ontology term information. 
    /// This function allows us, to add the same information as `Parameter`, `Characteristics`.., to `Component`. 
    /// Without this string composition we loose the ontology information for the header value.
    static member decomposeName (name : string) = 
        let pattern = """^(?<value>[^\(]+) \((?<ontology>[^(]*:[^)]*)\)"""
        let emptyOAPattern = """^(?<value>[^\(\)]+) \(\)"""
        let unitPattern = """^(?<value>[\d\.]+) (?<unit>.+) \((?<ontology>[^(]*:[^)]*)\)"""
        match name with
        | Regex unitPattern unitr ->
            let oa = (unitr.Groups.Item "ontology").Value   |> OntologyAnnotation.fromTermAnnotation 
            let v =  (unitr.Groups.Item "value").Value      |> Value.fromString
            let u =  (unitr.Groups.Item "unit").Value
            oa.Name <- Some u
            v, Some oa
        | Regex pattern r ->
            let oa = (r.Groups.Item "ontology").Value   |> OntologyAnnotation.fromTermAnnotation 
            let v =  (r.Groups.Item "value").Value      
            oa.Name <- Some v
            Value.Ontology oa, None
        | Regex emptyOAPattern r ->
            let oa =  OntologyAnnotation((r.Groups.Item "value").Value)
            let v = Value.Ontology oa
            v, None
        | _ -> 
            Value.Name (name), None       

    /// Create a ISAJson Component from ISATab string entries
    static member fromISAString (?name: string, ?term:string, ?source:string, ?accession:string, ?comments : ResizeArray<Comment>) = 
        let cType = OntologyAnnotation.create (?name = term, ?tsr=source, ?tan=accession, ?comments = comments) |> Option.fromValueWithDefault (OntologyAnnotation())
        match name with
        | Some n -> 
            let v,u = Component.decomposeName n
            Component.make (Option.fromValueWithDefault (Value.Name "") v) u cType
        | None ->
            Component.make None None cType

    /// Get ISATab string entries from an ISAJson Component object
    static member toStringObject (c : Component) =
        let oa = c.ComponentType |> Option.map OntologyAnnotation.toStringObject |> Option.defaultValue {|TermName = ""; TermAccessionNumber = ""; TermSourceREF = ""|}
        c.ComponentName |> Option.defaultValue "", oa

    member this.NameText =
        this.ComponentType
        |> Option.map (fun c -> c.NameText)
        |> Option.defaultValue ""

    /// Returns the ontology of the category of the Value as string
    member this.UnitText = 
        this.ComponentUnit
        |> Option.map (fun c -> c.NameText)
        |> Option.defaultValue ""

    member this.ValueText = 
        this.ComponentValue
        |> Option.map (fun c -> c.Text)
        |> Option.defaultValue ""

    member this.ValueWithUnitText =
        let unit = 
            this.ComponentUnit |> Option.map (fun oa -> oa.NameText)
        let v = this.ValueText
        match unit with
        | Some u    -> sprintf "%s %s" v u
        | None      -> v

    member this.MapCategory(f : OntologyAnnotation -> OntologyAnnotation) =
        {this with ComponentType = Option.map f this.ComponentType}

    member this.SetCategory(c : OntologyAnnotation) =
        {this with ComponentType = Some c}

    interface IPropertyValue with
        member this.AlternateName() = this.ComponentName
        member this.MeasurementMethod() = None
        member this.Description() = None
        member this.GetCategory() = this.ComponentType
        member this.GetAdditionalType() = "Component"
        member this.GetValue() = this.ComponentValue
        member this.GetUnit() = this.ComponentUnit

    static member createAsPV (alternateName : string option) (measurementMethod : string option) (description : string option) (category : OntologyAnnotation option) (value : Value option) (unit : OntologyAnnotation option) =
        Component.create(?componentType = category, ?value = value, ?unit = unit)