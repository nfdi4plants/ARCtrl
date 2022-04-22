namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization

open System.Collections.Generic
open System.Collections

type IOType =
    | Source
    | Sample
    | Data
    | RawData
    | ProcessedData
    | Material

    member this.isSource =
        match this with
        | Source -> true
        | _ -> false

    member this.isSample =
        match this with
        | Sample -> true
        | _ -> false
    
    member this.isData =
        match this with
        | Data | RawData | ProcessedData -> true
        | _ -> false

    member this.isRawData =
        match this with
        | RawData -> true
        | _ -> false

    member this.isProcessedData =
        match this with
        | ProcessedData -> true
        | _ -> false

    member this.isMaterial =
        match this with
        | Material -> true
        | _ -> false

    static member fromInput (inp : ProcessInput) = 
        match inp with
        | ProcessInput.Source s -> Source
        | ProcessInput.Sample s -> Sample
        | ProcessInput.Material m -> Material
        | ProcessInput.Data d when d.DataType.IsNone                -> Data
        | ProcessInput.Data d when d.DataType.Value.IsDerivedData   -> ProcessedData
        | ProcessInput.Data d when d.DataType.Value.IsRawData       -> RawData
        | ProcessInput.Data d                                       -> Data
    
    static member fromOutput (out : ProcessOutput) = 
        match out with
        | ProcessOutput.Sample s -> Sample
        | ProcessOutput.Material m -> Material
        | ProcessOutput.Data d when d.DataType.IsNone                -> Data
        | ProcessOutput.Data d when d.DataType.Value.IsDerivedData   -> ProcessedData
        | ProcessOutput.Data d when d.DataType.Value.IsRawData       -> RawData
        | ProcessOutput.Data d                                       -> Data


type QRow = 
    {
        [<JsonPropertyName(@"input")>]
        Input : string
        [<JsonPropertyName(@"output")>]
        Output : string
        [<JsonPropertyName(@"inputType")>]
        InputType : IOType option
        [<JsonPropertyName(@"outputType")>]
        OutputType : IOType option
        [<JsonPropertyName(@"values")>]
        Values : ISAValue list
    }

    static member create (?Input,?Output,?InputType,?OutputType,?Values) : QRow =

        {
            Input = Input |> Option.defaultValue ""
            Output = Output |> Option.defaultValue ""
            InputType = InputType
            OutputType = OutputType
            Values = Values |> Option.defaultValue []
        }

    static member create(?Input,?Output,?InputType,?OutputType,?CharValues,?ParamValues,?FactorValues) : QRow =
        let combineValues (characteristics : MaterialAttributeValue list) (parameters : ProcessParameterValue list) (factors : FactorValue list) : ISAValue list =           
            (characteristics |> List.map Characteristic)
            @ (parameters |> List.map Parameter)
            @ (factors |> List.map Factor)
            |> List.sortBy (fun v -> v.ValueIndex())

        {
            Input = Input |> Option.defaultValue ""
            Output = Output |> Option.defaultValue ""
            InputType = InputType
            OutputType = OutputType
            Values = combineValues (CharValues |> Option.defaultValue []) (ParamValues |> Option.defaultValue []) (FactorValues |> Option.defaultValue [])
        }

    static member fromProcess (proc : Process) : QRow list =
        let parameterValues = proc.ParameterValues |> Option.defaultValue []
        (proc.Inputs.Value,proc.Outputs.Value)
        ||> List.map2 (fun inp out ->
            let characteristics = API.ProcessInput.tryGetCharacteristics inp |> Option.defaultValue []
            let factors = API.ProcessOutput.tryGetFactorValues out |> Option.defaultValue []

            let inputName = inp.GetName
            let outputName = out.GetName

            let inputType = IOType.fromInput inp
            let outputType = IOType.fromOutput out

            QRow.create(inputName, outputName, inputType, outputType, characteristics, parameterValues, factors)
        )

    member this.Item (i : int) =
        this.Values.[i]

    member this.Item (s : string) =
        let item = 
            this.Values 
            |> List.tryFind (fun v -> 
                s = v.HeaderText || v.NameText = s
            )
        match item with
        | Some i -> i
        | None -> failwith $"Row with input \"{this.Input}\" does not contain item with name or header \"{s}\""

    member this.Item (oa : OntologyAnnotation) =
        let item =
            this.Values 
            |> List.tryFind (fun v -> v.Category = oa)
        match item with
        | Some i -> i
        | None -> failwith $"Row with input \"{this.Input}\" does not contain item with ontology \"{oa.GetName}\""

    member this.ValueCount =
        this.Values 
        |> List.length

    member this.ValueNames =
        this.Values 
        |> List.map (fun value -> value.NameText)

    member this.Headers =
        this.Values 
        |> List.map (fun value -> value.HeaderText)

    interface IEnumerable<ISAValue> with
        member this.GetEnumerator() : System.Collections.Generic.IEnumerator<ISAValue> = (seq this.Values).GetEnumerator()

    interface IEnumerable with
        member this.GetEnumerator() = (this :> IEnumerable<ISAValue>).GetEnumerator() :> IEnumerator
