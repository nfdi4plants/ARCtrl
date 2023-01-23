namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

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

    static member reduce (ioTypes : IOType list) =
        let comparer (iot : IOType) = 
            match iot with
            | Source        -> 1
            | Sample        -> 2
            | Material      -> 3
            | Data          -> 4
            | RawData       -> 5
            | ProcessedData -> 6
        ioTypes
        |> List.reduce (fun a b ->
            if comparer a > comparer b then a else b
        )
