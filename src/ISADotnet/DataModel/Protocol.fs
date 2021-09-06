namespace ISADotNet

open System.Text.Json.Serialization


type ProtocolParameter = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"parameterName")>]
        ParameterName : OntologyAnnotation option
    }
    
    static member create (?Id,?ParameterName) : ProtocolParameter =
        {
            ID              = Id
            ParameterName   = ParameterName
        }

    static member empty =
        ProtocolParameter.create()

    /// Create a ISAJson Protocol Parameter from ISATab string entries
    static member fromString (term:string) (accession:string) (source:string) =
        let oa = OntologyAnnotation.fromString term accession source
        ProtocolParameter.create(ParameterName=oa)

    /// Get ISATab string entries from an ISAJson ProtocolParameter object
    static member toString (pp : ProtocolParameter) =
        pp.ParameterName |> Option.map OntologyAnnotation.toString |> Option.defaultValue ("","","")        

    /// Returns the name of the parameter as string
    [<System.Obsolete("This function is deprecated. Use the member \"GetName\" instead.")>]
    member this.NameAsString =
        this.ParameterName
        |> Option.map (fun oa -> oa.GetName)
        |> Option.defaultValue ""

    /// Returns the name of the parameter with the number as string (e.g. "temperature #2")
    [<System.Obsolete("This function is deprecated. Use the member \"GetNameWithNumber\" instead.")>]
    member this.NameAsStringWithNumber =       
        this.ParameterName
        |> Option.map (fun oa -> oa.GetNameWithNumber)
        |> Option.defaultValue ""

    /// Returns the name of the parameter as string
    member this.GetName =
        this.ParameterName
        |> Option.map (fun oa -> oa.GetName)
        |> Option.defaultValue ""

    /// Returns the name of the parameter with the number as string (e.g. "temperature #2")
    member this.GetNameWithNumber =       
        this.ParameterName
        |> Option.map (fun oa -> oa.GetNameWithNumber)
        |> Option.defaultValue ""

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            "OA " + this.GetNameWithNumber

type Component = 
    {
        [<JsonPropertyName(@"componentName")>]
        ComponentName : string option
        [<JsonPropertyName(@"componentType")>]
        ComponentType : OntologyAnnotation option
    }

    static member create (?Name,?ComponentType) : Component =
        {
            ComponentName = Name
            ComponentType = ComponentType
        }

    static member empty =
        Component.create()
    
    /// Create a ISAJson Component from ISATab string entries
    static member fromString (name: string) (term:string) (accession:string) (source:string) =
        let oa = OntologyAnnotation.fromString term accession source
        Component.create(name, oa)

    /// Get ISATab string entries from an ISAJson Component object
    static member toString (c : Component) =
        let (n,t,a) = c.ComponentType |> Option.map OntologyAnnotation.toString |> Option.defaultValue ("","","")
        c.ComponentName |> Option.defaultValue "",n,t,a


type Protocol =
    {       
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"name")>]
        Name :          string option
        [<JsonPropertyName(@"protocolType")>]
        ProtocolType :  OntologyAnnotation option
        [<JsonPropertyName(@"description")>]
        Description :   string option
        [<JsonPropertyName(@"uri")>]
        Uri :           URI option
        [<JsonPropertyName(@"version")>]
        Version :       string option
        [<JsonPropertyName(@"parameters")>]
        Parameters :    ProtocolParameter list option
        [<JsonPropertyName(@"components")>]
        Components :    Component list option
        [<JsonPropertyName(@"comments")>]
        Comments :      Comment list option
    }

    static member create(?Id,?Name,?ProtocolType,?Description,?Uri,?Version,?Parameters,?Components,?Comments) : Protocol =
        {       
            ID              = Id
            Name            = Name
            ProtocolType    = ProtocolType
            Description     = Description
            Uri             = Uri
            Version         = Version
            Parameters      = Parameters
            Components      = Components
            Comments        = Comments
        }


    static member empty = 
        Protocol.create()
