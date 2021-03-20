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
    
    static member empty =
        ProtocolParameter.create None None

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
