namespace ISA

type Component = 
    {
        ComponentName : string option
        ComponentValue : Value option
        ComponentUnit : OntologyAnnotation option
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
    
    /// TODO: What does this function do @HLWeil? When is it meant to be used?
    static member composeName (value : Value Option) (unit : OntologyAnnotation option) = 
        match value,unit with
        | Some (Value.Ontology oa), _ ->
            $"{oa.NameText} ({oa.TermAccessionShort})"
        | Some v, None ->
            $"{v.AsString}"
        | Some v, Some u ->
            $"{v.AsString} {u.NameText} ({u.TermAccessionShort})"
        | None, _ -> ""

    /// TODO: What does this function do @HLWeil? When is it meant to be used?
    /// TODO: Move regex to regex module
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
    static member fromString (name: string, term:string, source:string, accession:string, ?comments : Comment list) = 
        let cType = OntologyAnnotation.fromString (term, source, accession, ?comments = comments) |> Option.fromValueWithDefault OntologyAnnotation.empty
        let v,u = Component.decomposeName name
        Component.make (Option.fromValueWithDefault "" name) (Option.fromValueWithDefault (Value.Name "") v) u cType
        
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
