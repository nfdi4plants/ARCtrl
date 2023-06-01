namespace ISA

type ProtocolParameter = 
    {
        ID : URI option
        ParameterName : OntologyAnnotation option
    }
    
    static member make id parameterName : ProtocolParameter= 
        {
            ID = id
            ParameterName = parameterName
        
        }

    static member create (?Id,?ParameterName) : ProtocolParameter =
        ProtocolParameter.make Id ParameterName

    static member empty =
        ProtocolParameter.create()

    /// Create a ISAJson Protocol Parameter from ISATab string entries
    static member fromString (term:string) (source:string) (accession:string) =
        let oa = OntologyAnnotation.fromString term source accession
        ProtocolParameter.make None (Option.fromValueWithDefault OntologyAnnotation.empty oa)

    /// Create a ISAJson Protocol parameter from ISATab string entries
    static member fromStringWithComments (term:string) (source:string) (accession:string) (comments : Comment list) =
        let oa = OntologyAnnotation.fromStringWithComments term source accession comments
        ProtocolParameter.make None (Option.fromValueWithDefault OntologyAnnotation.empty oa)


    /// Get ISATab string entries from an ISAJson ProtocolParameter object (name,source,accession)
    static member toString (pp : ProtocolParameter) =
        pp.ParameterName |> Option.map OntologyAnnotation.toString |> Option.defaultValue ("","","")        

    /// Returns the name of the parameter as string
    member this.NameText =
        this.ParameterName
        |> Option.map (fun oa -> oa.NameText)
        |> Option.defaultValue ""

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            "OA " + this.NameText

    member this.MapCategory(f : OntologyAnnotation -> OntologyAnnotation) =
        {this with ParameterName = Option.map f this.ParameterName}

    member this.SetCategory(c : OntologyAnnotation) =
        {this with ParameterName = Some c}

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
    
    static member composeName (value : Value Option) (unit : OntologyAnnotation option) = 
        match value,unit with
        | Some (Value.Ontology oa), _ ->
            $"{oa.NameText} ({oa.ShortAnnotationString})"
        | Some v, None ->
            $"{v.AsString}"
        | Some v, Some u ->
            $"{v.AsString} {u.NameText} ({u.ShortAnnotationString})"
        | None, _ -> ""

    static member decomposeName (name : string) = 
        let pattern = """(?<value>[^\(]+) \((?<ontology>[^(]*:[^)]*)\)"""
        let unitPattern = """(?<value>[\d\.]+) (?<unit>.+) \((?<ontology>[^(]*:[^)]*)\)"""

        let r = System.Text.RegularExpressions.Regex.Match(name,pattern)
        let unitr = System.Text.RegularExpressions.Regex.Match(name,unitPattern)

        if unitr.Success then
            let oa = (unitr.Groups.Item "ontology").Value   |> OntologyAnnotation.fromAnnotationId 
            let v =  (unitr.Groups.Item "value").Value      |> Value.fromString
            let u =  (unitr.Groups.Item "unit").Value
            v, Some {oa with Name = (Some (AnnotationValue.Text u))}
        elif r.Success then
            let oa = (r.Groups.Item "ontology").Value   |> OntologyAnnotation.fromAnnotationId 
            let v =  (r.Groups.Item "value").Value      |> Value.fromString
            Value.Ontology {oa with Name = (Some (AnnotationValue.Text v.AsString))}, None
        else 
            Value.Name (name), None       

    /// Create a ISAJson Component from ISATab string entries
    static member fromString (name: string) (term:string) (source:string) (accession:string) = 
        let cType = OntologyAnnotation.fromString term source accession |> Option.fromValueWithDefault OntologyAnnotation.empty
        let v,u = Component.decomposeName name
        Component.make (Option.fromValueWithDefault "" name) (Option.fromValueWithDefault (Value.Name "") v) u cType
        
    /// Create a ISAJson Component from ISATab string entries
    static member fromStringWithComments (name: string) (term:string) (source:string) (accession:string) (comments : Comment list)  = 
        let cType = OntologyAnnotation.fromStringWithComments term source accession comments |> Option.fromValueWithDefault OntologyAnnotation.empty
        let v,u = Component.decomposeName name
        Component.make (Option.fromValueWithDefault "" name) (Option.fromValueWithDefault (Value.Name "") v) u cType
        

    static member fromOptions (value: Value option) (unit: OntologyAnnotation Option) (header:OntologyAnnotation option) = 
        let name = Component.composeName value unit |> Option.fromValueWithDefault ""
        Component.make name value unit header


    /// Get ISATab string entries from an ISAJson Component object
    static member toString (c : Component) =
        let (n,t,a) = c.ComponentType |> Option.map OntologyAnnotation.toString |> Option.defaultValue ("","","")
        c.ComponentName |> Option.defaultValue "",n,t,a

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


type Protocol =
    {       
        ID : URI option
        Name :          string option
        ProtocolType :  OntologyAnnotation option
        Description :   string option
        Uri :           URI option
        Version :       string option
        Parameters :    ProtocolParameter list option
        Components :    Component list option
        Comments :      Comment list option
    }

    static member make id name protocolType description uri version parameters components comments : Protocol= 
        {       
            ID              = id
            Name            = name
            ProtocolType    = protocolType
            Description     = description
            Uri             = uri
            Version         = version
            Parameters      = parameters
            Components      = components
            Comments        = comments
        }

    static member create(?Id,?Name,?ProtocolType,?Description,?Uri,?Version,?Parameters,?Components,?Comments) : Protocol =
        Protocol.make Id Name ProtocolType Description Uri Version Parameters Components Comments

    static member empty = 
        Protocol.create()
