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

    static member make category value unit : ProcessParameterValue = 
        {
            Category = category
            Value = value
            Unit = unit
        }

    static member create (?Category,?Value,?Unit) : ProcessParameterValue = 
        ProcessParameterValue.make Category Value Unit

    static member empty =
        ProcessParameterValue.create()


    /// Returns the name of the category as string
    [<System.Obsolete("This function is deprecated. Use the member \"NameText\" instead.")>]
    member this.GetName =
        this.Category
        |> Option.map (fun oa -> oa.GetName)
        |> Option.defaultValue ""

    /// Returns the name of the category with the number as string (e.g. "temperature #2")
    [<System.Obsolete("This function is deprecated. Numbering support will soon be dropped")>]
    member this.GetNameWithNumber =       
        this.Category
        |> Option.map (fun oa -> oa.GetNameWithNumber)
        |> Option.defaultValue ""

    /// Returns the name of the category as string
    member this.NameText =
        this.Category
        |> Option.map (fun oa -> oa.NameText)
        |> Option.defaultValue ""

    member this.ValueText =
    
        this.Value
        |> Option.map (fun oa ->
            match oa with
            | Value.Ontology oa  -> oa.GetName
            | Value.Float f -> string f
            | Value.Int i   -> string i
            | Value.Name s  -> s
        )
        |> Option.defaultValue ""

    [<System.Obsolete("This function is deprecated. Use the member \"ValueText\" instead.")>]
    member this.GetValue = this.ValueText

    member this.ValueWithUnitText =
        let unit = 
            this.Unit |> Option.map (fun oa -> oa.NameText)
        let v = this.ValueText
        match unit with
        | Some u    -> sprintf "%s %s" v u
        | None      -> v

    [<System.Obsolete("This function is deprecated. Use the member \"ValueWithUnitText\" instead.")>]
    member this.GetValueWithUnit = this.ValueWithUnitText

    member this.MapCategory(f : OntologyAnnotation -> OntologyAnnotation) =
        {this with Category = this.Category |> Option.map (fun p -> p.MapCategory f) }

    member this.SetCategory(c : OntologyAnnotation) =
        {this with Category = 
            match this.Category with
            | Some p -> Some (p.SetCategory c)
            | None -> Some (ProtocolParameter.create(ParameterName = c))
        }

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            let category = this.Category |> Option.map (fun f -> f.NameText)
            let unit = this.Unit |> Option.map (fun oa -> oa.NameText)
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
    
    | [<SerializationOrder(0)>] Source of Source
    | [<SerializationOrder(1)>] Sample of Sample
    | [<SerializationOrder(1)>] Data of Data
    | [<SerializationOrder(1)>] Material of Material 
    
    member this.TryGetName =
        match this with
        | ProcessInput.Sample s     -> s.Name
        | ProcessInput.Source s     -> s.Name
        | ProcessInput.Material m   -> m.Name
        | ProcessInput.Data d       -> d.Name

    [<System.Obsolete("This function is deprecated. Use the member \"GetNameWithNumber\" instead.")>]
    member this.NameAsString =
        this.TryGetName
        |> Option.defaultValue ""

    member this.GetName =
        this.TryGetName
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

    member this.TryGetName =
        match this with
        | ProcessOutput.Sample s     -> s.Name
        | ProcessOutput.Material m   -> m.Name
        | ProcessOutput.Data d       -> d.Name

    member this.GetName =
        this.TryGetName
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

    static member make id name executesProtocol parameterValues performer date previousProcess nextProcess inputs outputs comments : Process= 
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

    static member create (?Id,?Name,?ExecutesProtocol,?ParameterValues,?Performer,?Date,?PreviousProcess,?NextProcess,?Inputs,?Outputs,?Comments) : Process= 
        Process.make Id Name ExecutesProtocol ParameterValues Performer Date PreviousProcess NextProcess Inputs Outputs Comments

    static member empty =
        Process.create()

    interface IISAPrintable with
        member this.Print() = 
            this.ToString()
        member this.PrintCompact() =
            let inputCount = this.Inputs |> Option.defaultValue [] |> List.length
            let outputCount = this.Outputs |> Option.defaultValue [] |> List.length
            let paramCount = this.ParameterValues |> Option.defaultValue [] |> List.length

            let name = this.Name |> Option.defaultValue "Unnamed Process"

            sprintf "%s [%i Inputs -> %i Params -> %i Outputs]" name inputCount paramCount outputCount
            
    static member composeName (processNameRoot : string) (i : int) =
        $"{processNameRoot}_{i}"

    static member decomposeName (name : string) =
        let pattern = """(?<name>.+)_(?<num>\d+)"""
        let r = System.Text.RegularExpressions.Regex.Match(name,pattern)

        if r.Success then
            (r.Groups.Item "name").Value, Some ((r.Groups.Item "num").Value |> int)
        else 
            name, None
