namespace ISADotNet

open System.Text.Json.Serialization

type ProcessParameterValue =
    {
        [<JsonPropertyName(@"category")>]
        Category    : ProtocolParameter option
        [<JsonPropertyName(@"value")>]
        Value       : Value option
        [<JsonPropertyName(@"unit")>]
        Unit        : OntologyAnnotation option
    }

    static member create category value unit : ProcessParameterValue = 
        {
            Category = category
            Value = value
            Unit = unit
        }

    static member empty =
        ProcessParameterValue.create None None None

[<AnyOf>]
type ProcessInput =
    
    | [<SerializationOrder(1)>] Source of Source
    | [<SerializationOrder(0)>] Sample of Sample
    | [<SerializationOrder(0)>] Data of Data
    | [<SerializationOrder(0)>] Material of Material 

[<AnyOf>]
type ProcessOutput =
    | Sample of Sample
    | Data of Data
    | Material of Material 

type Process = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"name")>]
        Name : string option
        [<JsonPropertyName(@"executesProtocol")>]
        ExecutesProtocol : Protocol option
        [<JsonPropertyName(@"parameterValues")>]
        ParameterValues : ProcessParameterValue list option
        [<JsonPropertyName(@"performer")>]
        Performer : string option
        [<JsonPropertyName(@"date")>]
        Date : string option
        [<JsonPropertyName(@"previousProcess")>]
        PreviousProcess : Process  option
        [<JsonPropertyName(@"nextProcess")>]
        NextProcess : Process option
        [<JsonPropertyName(@"inputs")>]
        Inputs : ProcessInput list option
        [<JsonPropertyName(@"outputs")>]
        Outputs : ProcessOutput list option
        [<JsonPropertyName(@"comments")>]
        Comments : Comment list option
    }

    static member create id name executesProtocol parameterValues performer date previousProcess nextProcess inputs outputs comments : Process= 
        {       
            ID                  = id
            Name                = name
            ExecutesProtocol    = executesProtocol
            ParameterValues     = parameterValues
            Performer           = performer
            Date                = date
            PreviousProcess     = previousProcess
            NextProcess         = nextProcess
            Inputs              = inputs
            Outputs             = outputs
            Comments            = comments       
        }

    static member empty =
        Process.create None None None None None None None None None None None
