namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization

open System.Collections.Generic
open System.Collections

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
        Vals : ISAValue list
    }

    static member create (?Input,?Output,?InputType,?OutputType,?Values) : QRow =

        {
            Input = Input |> Option.defaultValue ""
            Output = Output |> Option.defaultValue ""
            InputType = InputType
            OutputType = OutputType
            Vals = Values |> Option.defaultValue []
        }

    static member create(?Input,?Output,?InputType,?OutputType,?CharValues,?ParamValues,?FactorValues,?Components) : QRow =
        let combineValues (characteristics : MaterialAttributeValue list) (parameters : ProcessParameterValue list) (factors : FactorValue list) (components : Component list) : ISAValue list =           
            (characteristics |> List.map Characteristic)
            @ (parameters |> List.map Parameter)
            @ (factors |> List.map Factor)
            @ (components |> List.map Component)
            |> List.sortBy (fun v -> v.TryValueIndex() |> Option.defaultValue System.Int32.MaxValue)

        {
            Input = Input |> Option.defaultValue ""
            Output = Output |> Option.defaultValue ""
            InputType = InputType
            OutputType = OutputType
            Vals = combineValues (CharValues |> Option.defaultValue []) (ParamValues |> Option.defaultValue []) (FactorValues |> Option.defaultValue []) (Components |> Option.defaultValue []) 
        }

    static member fromProcess (proc : Process) : QRow list =
        let parameterValues = proc.ParameterValues |> Option.defaultValue []
        List.zip (proc.Inputs |> Option.defaultValue [ProcessInput.Default]) (proc.Outputs |> Option.defaultValue [ProcessOutput.Default])
        |> List.groupBy (fun (i,o) -> i.GetName,o.GetName)
        |> List.map (fun ((inputName,outputName),ios) ->
            
            let characteristics = 
                ios |> List.collect (fst >> API.ProcessInput.tryGetCharacteristicValues >> (Option.defaultValue []))
                |> List.distinct
            let factors = 
                ios |> List.collect (snd >> API.ProcessOutput.tryGetFactorValues >> (Option.defaultValue []))
                |> List.distinct
            let components = proc.ExecutesProtocol |> Option.bind (fun p -> p.Components) |> Option.defaultValue []

            let inputType = ios |> List.map (fst >> IOType.fromInput) |> IOType.reduce
            let outputType = ios |> List.map (snd >> IOType.fromOutput) |> IOType.reduce

            QRow.create(inputName, outputName, inputType, outputType, characteristics, parameterValues, factors, components)
            
        )
       

    member this.Item (i : int) =
        this.Vals.[i]

    member this.Item (s : string) =
        let item = 
            this.Vals 
            |> List.tryFind (fun v -> 
                s = v.HeaderText || v.NameText = s
            )
        match item with
        | Some i -> i
        | None -> failwith $"Row with input \"{this.Input}\" does not contain item with name or header \"{s}\""

    member this.Item (oa : OntologyAnnotation) =
        let item =
            this.Vals 
            |> List.tryFind (fun v -> v.Category = oa)
        match item with
        | Some i -> i
        | None -> failwith $"Row with input \"{this.Input}\" does not contain item with ontology \"{oa.NameText}\""


    member this.ValueCount =
        this.Vals 
        |> List.length

    member this.ValueNames =
        this.Vals 
        |> List.map (fun value -> value.NameText)

    member this.Headers =
        this.Vals 
        |> List.map (fun value -> value.HeaderText)

    member this.Values() = 
        this.Vals |> ValueCollection

    member this.Factors() = 
        this.Values().Factors()

    member this.Characteristics() =
        this.Values().Characteristics()

    member this.Parameters() = 
        this.Values().Parameters()

    member this.MapVals(f : ISAValue list -> ISAValue list) =
        {this with Vals = f this.Vals}

    interface IEnumerable<ISAValue> with
        member this.GetEnumerator() : System.Collections.Generic.IEnumerator<ISAValue> = (seq this.Vals).GetEnumerator()

    interface IEnumerable with
        member this.GetEnumerator() = (this :> IEnumerable<ISAValue>).GetEnumerator() :> IEnumerator
