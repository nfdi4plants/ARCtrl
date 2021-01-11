namespace ISADotNet


type Comment = 
    {
        Name : string
        Value : string
    }
  
    static member NameLabel = "name"
    static member ValueLabel = "value"


type OntologyAnnotation =
    {
        Name : string
        TermSourceREF : string
        TermAccessionNumber : string
        Comments : Comment []
    }

    static member NameLabel = "annotationValue"
    static member TermSourceREFLabel = "termSource"
    static member TermAccessionNumberLabel = "termAccession"
    static member CommentsLabel = "comments"


type Value =
    | Ontology of OntologyAnnotation
    | Numeric of float
    | Name of string

type Component = 
    {
        ComponentName : string
        ComponentType : OntologyAnnotation Option
    }

    static member NameLabel = "componentName"
    static member TypeLabel = "componentType"

type Protocol =
    {       
        Name :          string
        ProtocolType :  OntologyAnnotation Option
        Description :   string
        Uri :           string
        Version :       string
        Parameters :    OntologyAnnotation []
        Components :    Component []
        Comments :      Comment []
    }

    static member NameLabel =           "name"
    static member TypeLabel =           "protocolType"
    static member DescriptionLabel =    "description"
    static member UriLabel =            "uri"
    static member VersionLabel =        "version"
    static member ParametersLabel =     "parameters"
    static member ComponentsLabel =     "components"
    static member CommentsLabel =       "comments"

type ParameterValue =
    {
        Category    : OntologyAnnotation
        Value       : Value
        Unit        : OntologyAnnotation option   
    }

    static member CategoryLabel = "category"
    static member ValueLabel = "value"
    static member UnitLabel = "unit"


type Process = 
    {
        Name : string
        ExecutesProtocol : Protocol option
        ParameterValues : ParameterValue []
        Performer : string
        Date : string
        PreviousProcess : Process Option
        NextProcess : Process Option
        Inputs : string [] 
        Outputs : string []
        Comments : Comment []  
    }

    static member NameLabel = "name"
    static member ExecutesProtocolLabel = "executesProtocol"
    static member ParameterValuesLabel = "parameterValues"
    static member PerformerLabel = "performer"
    static member DateLabel = "date"
    static member PreviousProcessLabel = "previousProcess"
    static member NextProcessLabel = "nextProcess"
    static member InputsLabel = "inputs"
    static member OutputsLabel = "outputs"
    static member CommentsLabel = "comments"


