namespace ISA

open ISA.Aux

type Component = 
    {
        // TODO: Maybe remove as field and add as member?
        ComponentName : string option
        /// This can be the main column value of the component column. (e.g. "SCIEX instrument model" as `OntologyAnnotation`; 14;..)
        ComponentValue : Value option
        /// This can be the unit describing a non `OntologyAnnotation` value in `ComponentValue`. (e.g. "degree celcius")
        ComponentUnit : OntologyAnnotation option
        /// This can be the component column header (e.g. "instrument model")
        ComponentType : OntologyAnnotation option
    }

    static member make name value unit componentType =
        {
            ComponentName = name
            ComponentValue = value
            ComponentUnit = unit
            ComponentType = componentType
        }

    static member create (?Name,?Value,?Unit,?ComponentType) : Component =
        Component.make Name Value Unit ComponentType

    static member empty =
        Component.create()
    
    /// This function creates a string containing full isa term triplet information about the component
    ///
    /// Components do not have enough fields in ISA-JSON to include all existing ontology term information. 
    /// This function allows us, to add the same information as `Parameter`, `Characteristics`.., to `Component`. 
    /// Without this string composition we loose the ontology information for the header value.
    static member composeName (value : Value Option) (unit : OntologyAnnotation option) = 
        match value,unit with
        | Some (Value.Ontology oa), _ ->
            $"{oa.NameText} ({oa.TermAccessionShort})"
        | Some v, None ->
            $"{v.AsString}"
        | Some v, Some u ->
            $"{v.AsString} {u.NameText} ({u.TermAccessionShort})"
        | None, _ -> ""

    /// This function parses the given Component header string format into the ISA-JSON Component type
    ///
    /// Components do not have enough fields in ISA-JSON to include all existing ontology term information. 
    /// This function allows us, to add the same information as `Parameter`, `Characteristics`.., to `Component`. 
    /// Without this string composition we loose the ontology information for the header value.
    static member decomposeName (name : string) = 
        let pattern = """(?<value>[^\(]+) \((?<ontology>[^(]*:[^)]*)\)"""
        let unitPattern = """(?<value>[\d\.]+) (?<unit>.+) \((?<ontology>[^(]*:[^)]*)\)"""

        let r = System.Text.RegularExpressions.Regex.Match(name,pattern)
        let unitr = System.Text.RegularExpressions.Regex.Match(name,unitPattern)

        if unitr.Success then
            let oa = (unitr.Groups.Item "ontology").Value   |> OntologyAnnotation.fromTermAccession 
            let v =  (unitr.Groups.Item "value").Value      |> Value.fromString
            let u =  (unitr.Groups.Item "unit").Value
            v, Some {oa with Name = (Some (AnnotationValue.Text u))}
        elif r.Success then
            let oa = (r.Groups.Item "ontology").Value   |> OntologyAnnotation.fromTermAccession 
            let v =  (r.Groups.Item "value").Value      |> Value.fromString
            Value.Ontology {oa with Name = (Some (AnnotationValue.Text v.AsString))}, None
        else 
            Value.Name (name), None       

    /// Create a ISAJson Component from ISATab string entries
    static member fromString (?name: string, ?term:string, ?source:string, ?accession:string, ?comments : Comment list) = 
        let cType = OntologyAnnotation.fromString (?term = term, ?tsr=source, ?tan=accession, ?comments = comments) |> Option.fromValueWithDefault OntologyAnnotation.empty
        match name with
        | Some n -> 
            let v,u = Component.decomposeName n
            Component.make (name) (Option.fromValueWithDefault (Value.Name "") v) u cType
        | None ->
            Component.make None None None cType
        
    static member fromOptions (value: Value option) (unit: OntologyAnnotation Option) (header:OntologyAnnotation option) = 
        let name = Component.composeName value unit |> Option.fromValueWithDefault ""
        Component.make name value unit header

    /// Get ISATab string entries from an ISAJson Component object
    static member toString (c : Component) =
        let oa = c.ComponentType |> Option.map OntologyAnnotation.toString |> Option.defaultValue {|TermName = ""; TermAccessionNumber = ""; TermSourceREF = ""|}
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
        |> Option.map (fun c -> c.AsString)
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
