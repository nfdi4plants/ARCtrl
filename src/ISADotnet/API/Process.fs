namespace ISADotNet.API

open ISADotNet

/// Functions for handling the ProcessInput Type
module ProcessInput =

    /// Returns name of processInput
    let getName (pi : ProcessInput) =
        match pi with
        | ProcessInput.Sample s     -> s.Name
        | ProcessInput.Source s     -> s.Name
        | ProcessInput.Material m   -> m.Name
        | ProcessInput.Data d       -> d.Name

    /// Returns true, if given name equals name of processInput
    let nameEquals (name : string) (pi : ProcessInput) =
        match pi with
        | ProcessInput.Sample s     -> s.Name = (Some name)
        | ProcessInput.Source s     -> s.Name = (Some name)
        | ProcessInput.Material m   -> m.Name = (Some name)
        | ProcessInput.Data d       -> d.Name = (Some name)

    /// Returns true, if Process Input is Sample
    let isSample (pi : ProcessInput) =
        match pi with
        | ProcessInput.Sample _ -> true
        | _ -> false

    /// Returns true, if Process Input is Source
    let isSource (pi : ProcessInput) =
        match pi with
        | ProcessInput.Source _ -> true
        | _ -> false

    /// Returns true, if Process Input is Data
    let isData (pi : ProcessInput) =
        match pi with
        | ProcessInput.Data _ -> true
        | _ -> false

    /// Returns true, if Process Input is Material
    let isMaterial (pi : ProcessInput) =
        match pi with
        | ProcessInput.Material _ -> true
        | _ -> false

    /// If given process input is a sample, returns it, else returns None
    let trySample (pi : ProcessInput) =
        match pi with
        | ProcessInput.Sample s -> Some s
        | _ -> None

    /// If given process input is a source, returns it, else returns None
    let trySource (pi : ProcessInput) =
        match pi with
        | ProcessInput.Source s -> Some s
        | _ -> None

    /// If given process input is a data, returns it, else returns None
    let tryData (pi : ProcessInput) =
        match pi with
        | ProcessInput.Data d -> Some d
        | _ -> None

    /// If given process input is a material, returns it, else returns None
    let tryMaterial (pi : ProcessInput) =
        match pi with
        | ProcessInput.Material m -> Some m
        | _ -> None


/// Functions for handling the ProcessOutput Type
module ProcessOutput =

    /// Returns name of processOutput
    let getName (po : ProcessOutput) =
        match po with
        | ProcessOutput.Sample s     -> s.Name
        | ProcessOutput.Material m   -> m.Name
        | ProcessOutput.Data d       -> d.Name

    /// Returns true, if given name equals name of processOutput
    let nameEquals (name : string) (po : ProcessOutput) =
        match po with
        | ProcessOutput.Sample s     -> s.Name = (Some name)
        | ProcessOutput.Material m   -> m.Name = (Some name)
        | ProcessOutput.Data d       -> d.Name = (Some name)

    /// Returns true, if Process Output is Sample
    let isSample (po : ProcessOutput) =
        match po with
        | ProcessOutput.Sample _ -> true
        | _ -> false

    /// Returns true, if Process Output is Data
    let isData (po : ProcessOutput) =
        match po with
        | ProcessOutput.Data _ -> true
        | _ -> false

    /// Returns true, if Process Output is Material
    let isMaterial (po : ProcessOutput) =
        match po with
        | ProcessOutput.Material _ -> true
        | _ -> false

    /// If given process output is a sample, returns it, else returns None
    let trySample (po : ProcessOutput) =
        match po with
        | ProcessOutput.Sample s -> Some s
        | _ -> None

    /// If given process output is a data, returns it, else returns None
    let tryData (po : ProcessOutput) =
        match po with
        | ProcessOutput.Data d -> Some d
        | _ -> None

    /// If given process output is a material, returns it, else returns None
    let tryMaterial (po : ProcessOutput) =
        match po with
        | ProcessOutput.Material m -> Some m
        | _ -> None


/// Functions for handling ISA Processes
module Process =

    /// Returns the parameters describing the process
    let getParameters (p: Process) =
        match p.ParameterValues with
        | Some paramValues ->
            paramValues
            |> List.choose (fun pv -> pv.Category)
        | None -> []

    /// If the process implements the given parameter, return the list of input files together with their according parameter values of this parameter
    let tryGetInputsWithParameterBy (predicate : ProtocolParameter -> bool) (p : Process) =
        match p.ParameterValues with
        | Some paramValues ->
            match paramValues |> List.tryFind (fun pv -> Option.defaultValue ProtocolParameter.empty pv.Category |> predicate ) with
            | Some paramValue ->
                p.Inputs
                |> Option.map (List.map (fun i -> i,paramValue))
            | None -> None
        | None -> None

    
    /// If the process implements the given parameter, return the list of output files together with their according parameter values of this parameter
    let tryGetOutputsWithParameterBy (predicate : ProtocolParameter -> bool) (p : Process) =
        match p.ParameterValues with
        | Some paramValues ->
            match paramValues |> List.tryFind (fun pv -> Option.defaultValue ProtocolParameter.empty pv.Category |> predicate ) with
            | Some paramValue ->
                p.Outputs
                |> Option.map (List.map (fun i -> i,paramValue))
            | None -> None
        | None -> None
