namespace ISADotNet

open System.Text.Json.Serialization


type ProtocolParameter = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"parameterName")>]
        ParameterName : OntologyAnnotation option
    }

    static member create id parameterName : ProtocolParameter= 
        {
            ID = id
            ParameterName = parameterName
        
        }
    
    static member Create (?Id,?ParameterName) =
        ProtocolParameter.create Id ParameterName

    static member empty =
        ProtocolParameter.create None None

    /// Create a ISAJson Protocol Parameter from ISATab string entries
    static member fromString (term:string) (accession:string) (source:string) =
        OntologyAnnotation.fromString term accession source
        |> Option.fromValueWithDefault OntologyAnnotation.empty
        |> ProtocolParameter.create None

    /// Get ISATab string entries from an ISAJson ProtocolParameter object
    static member toString (pp : ProtocolParameter) =
        pp.ParameterName |> Option.map OntologyAnnotation.toString |> Option.defaultValue ("","","")        

    /// Returns the name of the parameter as string
    member this.NameAsString =
        this.ParameterName
        |> Option.map (fun oa -> oa.NameAsStringWithNumber)
        |> Option.defaultValue ""

    /// Returns the name of the parameter with the number as string (e.g. "temperature #2")
    member this.NameAsStringWithNumber =       
        this.ParameterName
        |> Option.map (fun oa -> oa.NameAsStringWithNumber)
        |> Option.defaultValue ""

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            "OA " + this.NameAsStringWithNumber

type Component = 
    {
        [<JsonPropertyName(@"componentName")>]
        ComponentName : string option
        [<JsonPropertyName(@"componentType")>]
        ComponentType : OntologyAnnotation option
    }

    static member create name componentType =
        {
            ComponentName = name
            ComponentType = componentType
        }
       
    static member empty =
        Component.create None None 

    static member Create (?Name,?ComponentType) =
        Component.create Name ComponentType

    
    /// Create a ISAJson Component from ISATab string entries
    static member fromString (name: string) (term:string) (accession:string) (source:string) =
        OntologyAnnotation.fromString term accession source
        |> Option.fromValueWithDefault OntologyAnnotation.empty
        |> Component.create (Option.fromValueWithDefault "" name)

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

    static member create id name protocolType description uri version parameters components comments : Protocol= 
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

    static member empty = 
        Protocol.create None None None None None None None None None

    static member Create(?Id,?Name,?ProtocolType,?Description,?Uri,?Version,?Parameter,?Components,?Comments) =
        Protocol.create Id Name ProtocolType Description Uri Version Parameter Components Comments

