namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISADotNet
open System.IO
open GEncode

module ProcessParameterValue =

    let encoder (options : ConverterOptions) (oa : obj) = 

        [
            tryInclude "category" (ProtocolParameter.encoder options) (oa |> tryGetPropertyValue "Category")
            tryInclude "value" (Value.encoder options) (oa |> tryGetPropertyValue "Value")
            tryInclude "unit" (OntologyAnnotation.encoder options) (oa |> tryGetPropertyValue "Unit")
        ]
        |> GEncode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<ProcessParameterValue> =
        Decode.object (fun get ->
            {
                Category = get.Optional.Field "category" (ProtocolParameter.decoder options)
                Value = get.Optional.Field "value" (Value.decoder options)
                Unit = get.Optional.Field "unit" (OntologyAnnotation.decoder options)
            }
        )

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s

    let toString (p:ProcessParameterValue) = 
        encoder (ConverterOptions()) p
        |> Encode.toString 2

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (p:ProcessParameterValue) = 
        File.WriteAllText(path,toString p)

/// Functions for handling the ProcessInput Type
module ProcessInput =

    let encoder (options : ConverterOptions) (value : obj) = 
        match value with
        | :? ProcessInput as ProcessInput.Source s-> 
            Source.encoder options s
        | :? ProcessInput as ProcessInput.Sample s -> 
            Sample.encoder options s
        | :? ProcessInput as ProcessInput.Data d -> 
            Data.encoder options d
        | :? ProcessInput as ProcessInput.Material m -> 
            Material.encoder options m
        | _ -> Encode.nil

    let decoder (options : ConverterOptions) : Decoder<ProcessInput> =
        fun s json ->
            match Source.decoder options s json with
            | Ok s -> Ok (ProcessInput.Source s)
            | Error _ -> 
                match Sample.decoder options s json with
                | Ok s -> Ok (ProcessInput.Sample s)
                | Error _ -> 
                    match Data.decoder options s json with
                    | Ok s -> Ok (ProcessInput.Data s)
                    | Error _ -> 
                        match Material.decoder options s json with
                        | Ok s -> Ok (ProcessInput.Material s)
                        | Error e -> Error e

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s

    let toString (m:ProcessInput) = 
        encoder (ConverterOptions()) m
        |> Encode.toString 2

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (m:ProcessInput) = 
        File.WriteAllText(path,toString m)

/// Functions for handling the ProcessOutput Type
module ProcessOutput =

    let encoder (options : ConverterOptions) (value : obj) = 
        match value with
        | :? ProcessOutput as ProcessOutput.Sample s -> 
            Sample.encoder options s
        | :? ProcessOutput as ProcessOutput.Data d -> 
            Data.encoder options d
        | :? ProcessOutput as ProcessOutput.Material m -> 
            Material.encoder options m
        | _ -> Encode.nil

    let decoder (options : ConverterOptions) : Decoder<ProcessOutput> =
        fun s json ->
            match Sample.decoder options s json with
            | Ok s -> Ok (ProcessOutput.Sample s)
            | Error _ -> 
                match Data.decoder options s json with
                | Ok s -> Ok (ProcessOutput.Data s)
                | Error _ -> 
                    match Material.decoder options s json with
                    | Ok s -> Ok (ProcessOutput.Material s)
                    | Error e -> Error e

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s

    let toString (m:ProcessInput) = 
        encoder (ConverterOptions()) m
        |> Encode.toString 2

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (m:ProcessInput) = 
        File.WriteAllText(path,toString m)


module Process =    

    let rec encoder (options : ConverterOptions) (oa : obj) = 
        [
            tryInclude "@id" GEncode.string (oa |> tryGetPropertyValue "ID")
            tryInclude "name" GEncode.string (oa |> tryGetPropertyValue "Name")
            tryInclude "executesProtocol" (Protocol.encoder options) (oa |> tryGetPropertyValue "ExecutesProtocol")
            tryInclude "parameterValues" (ProcessParameterValue.encoder options) (oa |> tryGetPropertyValue "ParameterValues")
            tryInclude "performer" GEncode.string (oa |> tryGetPropertyValue "Performer")
            tryInclude "date" GEncode.string (oa |> tryGetPropertyValue "Date")
            tryInclude "previousProcess" (encoder options) (oa |> tryGetPropertyValue "PreviousProcess")
            tryInclude "nextProcess" (encoder options) (oa |> tryGetPropertyValue "NextProcess")
            tryInclude "inputs" (ProcessInput.encoder options) (oa |> tryGetPropertyValue "Inputs")
            tryInclude "outputs" (ProcessOutput.encoder options) (oa |> tryGetPropertyValue "Outputs")
            tryInclude "comments" (Comment.encoder options) (oa |> tryGetPropertyValue "Comments")
        ]
        |> GEncode.choose
        |> Encode.object

    let rec decoder (options : ConverterOptions) : Decoder<Process> =
        Decode.object (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                Name = get.Optional.Field "name" Decode.string
                ExecutesProtocol = get.Optional.Field "executesProtocol" (Protocol.decoder options)
                ParameterValues = get.Optional.Field "parameterValues" (Decode.list (ProcessParameterValue.decoder options))
                Performer = get.Optional.Field "performer" Decode.string
                Date = get.Optional.Field "date" Decode.string
                PreviousProcess = get.Optional.Field "previousProcess" (decoder options)
                NextProcess = get.Optional.Field "nextProcess" (decoder options)
                Inputs = get.Optional.Field "inputs" (Decode.list (ProcessInput.decoder options))
                Outputs = get.Optional.Field "outputs" (Decode.list (ProcessOutput.decoder options))
                Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))
            }
        )

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s

    let toString (p:Process) = 
        encoder (ConverterOptions()) p
        |> Encode.toString 2

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (p:Process) = 
        File.WriteAllText(path,toString p)

module ProcessSequence = 

    let fromString (s:string) = 
        GDecode.fromString (Decode.list (Process.decoder (ConverterOptions()))) s

    let toString (p:Process list) = 
        p
        |> List.map (Process.encoder (ConverterOptions()))
        |> Encode.list
        |> Encode.toString 2

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (p:Process list) = 
        File.WriteAllText(path,toString p)