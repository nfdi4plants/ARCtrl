namespace ISA

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
