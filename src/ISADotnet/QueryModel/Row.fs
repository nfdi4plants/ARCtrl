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
        InputType : string option
        [<JsonPropertyName(@"outputType")>]
        OutputType : string option
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
            let l = 
                //charac

                (characteristics |> List.map Characteristic)
                @ (parameters |> List.map Parameter)
                @ (factors |> List.map Factor)
            l
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

            QRow.create(inputName,outputName,ParamValues = parameterValues, CharValues = characteristics,FactorValues = factors)
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
