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

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            let category = this.Category |> Option.map (fun f -> f.NameAsString)
            let unit = this.Unit |> Option.map (fun oa -> oa.NameAsString)
            let value = 
                this.Value
                |> Option.map (fun v ->
                    let s = (v :> IISAPrintable).PrintCompact()
                    match unit with
                    | Some u -> s + " " + u
                    | None -> s
                )
            match category,value with
            | Some category, Some value -> category + ":" + value
            | Some category, None -> category + ":" + "No Value"
            | None, Some value -> value
            | None, None -> ""

[<AnyOf>]
type ProcessInput =
    
    | [<SerializationOrder(1)>] Source of Source
    | [<SerializationOrder(0)>] Sample of Sample
    | [<SerializationOrder(0)>] Data of Data
    | [<SerializationOrder(0)>] Material of Material 

    member this.Name =
        match this with
        | ProcessInput.Sample s     -> s.Name
        | ProcessInput.Source s     -> s.Name
        | ProcessInput.Material m   -> m.Name
        | ProcessInput.Data d       -> d.Name

    member this.NameAsString =
        this.Name
        |> Option.defaultValue ""

    interface IISAPrintable with
        member this.Print() = 
            this.ToString()
        member this.PrintCompact() =
            match this with 
            | ProcessInput.Sample s     -> sprintf "Sample {%s}" ((s :> IISAPrintable).PrintCompact())
            | ProcessInput.Source s     -> sprintf "Source {%s}" ((s :> IISAPrintable).PrintCompact())
            | ProcessInput.Material m   -> sprintf "Material {%s}" ((m :> IISAPrintable).PrintCompact())
            | ProcessInput.Data d       -> sprintf "Data {%s}" ((d :> IISAPrintable).PrintCompact())


[<AnyOf>]
type ProcessOutput =
    | Sample of Sample
    | Data of Data
    | Material of Material 

    member this.Name =
        match this with
        | ProcessOutput.Sample s     -> s.Name
        | ProcessOutput.Material m   -> m.Name
        | ProcessOutput.Data d       -> d.Name

    member this.NameAsString =
        this.Name
        |> Option.defaultValue ""

    interface IISAPrintable with
        member this.Print() = 
            this.ToString()
        member this.PrintCompact() =
            match this with 
            | ProcessOutput.Sample s     -> sprintf "Sample {%s}" ((s :> IISAPrintable).PrintCompact())
            | ProcessOutput.Material m   -> sprintf "Material {%s}" ((m :> IISAPrintable).PrintCompact())
            | ProcessOutput.Data d       -> sprintf "Data {%s}" ((d :> IISAPrintable).PrintCompact())

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

    interface IISAPrintable with
        member this.Print() = 
            this.ToString()
        member this.PrintCompact() =
            let inputCount = this.Inputs |> Option.defaultValue [] |> List.length
            let outputCount = this.Outputs |> Option.defaultValue [] |> List.length
            let paramCount = this.ParameterValues |> Option.defaultValue [] |> List.length

            let name = this.Name |> Option.defaultValue "Unnamed Process"

            sprintf "%s [%i Inputs -> %i Params -> %i Outputs]" name inputCount outputCount paramCount 
            

