namespace ISADotNet

open System.Text.Json.Serialization


type ProtocolParameter = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI
        [<JsonPropertyName(@"parameterName")>]
        ParameterName : OntologyAnnotation
    }

    static member create id parameterName : ProtocolParameter= 
        {
            ID = id
            ParameterName = parameterName
        
        }



type Component = 
    {
        [<JsonPropertyName(@"componentName")>]
        ComponentName : string
        [<JsonPropertyName(@"componentType")>]
        ComponentType : OntologyAnnotation
    }

    static member create name componentType =
        {
            ComponentName = name
            ComponentType = componentType
        }


type Protocol =
    {       
        [<JsonPropertyName(@"@id")>]
        ID : URI
        [<JsonPropertyName(@"name")>]
        Name :          string
        [<JsonPropertyName(@"protocolType")>]
        ProtocolType :  OntologyAnnotation
        [<JsonPropertyName(@"description")>]
        Description :   string
        [<JsonPropertyName(@"uri")>]
        Uri :           URI
        [<JsonPropertyName(@"version")>]
        Version :       string
        [<JsonPropertyName(@"parameters")>]
        Parameters :    ProtocolParameter list
        [<JsonPropertyName(@"components")>]
        Components :    Component list
        [<JsonPropertyName(@"comments")>]
        Comments :      Comment list
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

