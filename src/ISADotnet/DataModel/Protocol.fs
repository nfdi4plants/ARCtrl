namespace ISADotNet

open System.Text.Json.Serialization


type ProtocolParameter = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"parameterName")>]
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

    /// Create a ISAJson Protocol Parameter from string entries, where the term name can contain a # separated number. e.g: "temperature unit #2"
    static member fromStringWithNumber (term:string) (source:string) (accession:string) =
        let oa = OntologyAnnotation.fromStringWithNumber term source accession
        ProtocolParameter.make None (Option.fromValueWithDefault OntologyAnnotation.empty oa)

    /// Create a ISAJson Protocol Parameter from string entries, where the term name can contain a # separated number. e.g: "temperature unit #2"
    static member fromStringWithNumberAndComments (term:string) (source:string) (accession:string) (comments : Comment list) =
        let oa = OntologyAnnotation.fromStringWithNumberAndComments term source accession comments
        ProtocolParameter.make None (Option.fromValueWithDefault OntologyAnnotation.empty oa)

    /// Get ISATab string entries from an ISAJson ProtocolParameter object (name,source,accession)
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

    static member make name componentType =
        {
            ComponentName = name
            ComponentType = componentType
        }

    static member create (?Name,?ComponentType) : Component =
        Component.make Name ComponentType

    static member empty =
        Component.create()
    
    /// Create a ISAJson Component from ISATab string entries
    static member fromString (name: string) (term:string) (source:string) (accession:string) =
        let oa = OntologyAnnotation.fromString term source accession
        Component.make (Option.fromValueWithDefault "" name) (Option.fromValueWithDefault OntologyAnnotation.empty oa)

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
