namespace ISADotNet.API

open ISADotNet

module Process =

    let tryGetInputsWithParameterBy (predicate : ProtocolParameter -> bool) (p : Process) =
        match p.ParameterValues with
        | Some paramValues ->
            match paramValues |> List.tryFind (fun pv -> Option.defaultValue ProtocolParameter.empty pv.Category |> predicate ) with
            | Some paramValue ->
                p.Inputs
                |> Option.map (List.map (fun i -> i,paramValue))
            | None -> None
        | None -> None

    let tryGetOutputsWithParameterBy (predicate : ProtocolParameter -> bool) (p : Process) =
        match p.ParameterValues with
        | Some paramValues ->
            match paramValues |> List.tryFind (fun pv -> Option.defaultValue ProtocolParameter.empty pv.Category |> predicate ) with
            | Some paramValue ->
                p.Outputs
                |> Option.map (List.map (fun i -> i,paramValue))
            | None -> None
        | None -> None