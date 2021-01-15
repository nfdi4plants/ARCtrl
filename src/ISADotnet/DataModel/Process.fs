namespace ISADotNet

open System.Text.Json.Serialization

type ProcessParameterValue =
    {
        [<JsonPropertyName(@"category")>]
        Category    : ProtocolParameter
        [<JsonPropertyName(@"value")>]
        Value       : Value
        [<JsonPropertyName(@"unit")>]
        Unit        : OntologyAnnotation option   
    }

    static member create category value unit : ProcessParameterValue = 
        {
            Category = category
            Value = value
            Unit = unit
        }

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
        ID : URI
        [<JsonPropertyName(@"name")>]
        Name : string
        [<JsonPropertyName(@"executesProtocol")>]
        ExecutesProtocol : Protocol
        [<JsonPropertyName(@"parameterValues")>]
        ParameterValues : ProcessParameterValue list
        [<JsonPropertyName(@"performer")>]
        Performer : string
        [<JsonPropertyName(@"date")>]
        Date : string
        [<JsonPropertyName(@"previousProcess")>]
        PreviousProcess : Process 
        [<JsonPropertyName(@"nextProcess")>]
        NextProcess : Process
        [<JsonPropertyName(@"inputs")>]
        Inputs : ProcessInput list
        [<JsonPropertyName(@"outputs")>]
        Outputs : ProcessOutput list
        [<JsonPropertyName(@"comments")>]
        Comments : Comment list
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
