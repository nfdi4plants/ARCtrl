namespace ISADotNet

open System.Text.Json.Serialization


type ProtocolParameter = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : string
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
        ID : string
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

    static member NameTab = "Name"
    static member ProtocolTypeTab = "Type"
    static member TypeTermAccessionNumberTab = "Type Term Accession Number"
    static member TypeTermSourceREFTab = "Type Term Source REF"
    static member DescriptionTab = "Description"
    static member URITab = "URI"
    static member VersionTab = "Version"
    static member ParametersNameTab = "Parameters Name"
    static member ParametersTermAccessionNumberTab = "Parameters Term Accession Number"
    static member ParametersTermSourceREFTab = "Parameters Term Source REF"
    static member ComponentsNameTab = "Components Name"
    static member ComponentsTypeTab = "Components Type"
    static member ComponentsTypeTermAccessionNumberTab = "Components Type Term Accession Number"
    static member ComponentsTypeTermSourceREFTab = "Components Type Term Source REF"
