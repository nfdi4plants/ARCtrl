namespace ARCtrl.ISA.Json

open Thoth.Json.Core

open ARCtrl.ISA
open System.IO

module ProcessParameterValue =
    
    let genID (p:ProcessParameterValue) = 
        match (p.Value,p.Category) with
        | (Some v, Some c) -> 
            "#Param_" 
            + (ProtocolParameter.getNameText c).Replace(" ","_") 
            + "_" 
            + (Value.getText v).Replace(" ","_")
        | _ -> "#EmptyParameterValue"

    let encoder (options : ConverterOptions) (oa : ProcessParameterValue) = 
    
        [
            if options.SetID then "@id", Encode.string (oa |> genID)
            if options.IncludeType then "@type", Encode.string "ProcessParameterValue"
            GEncode.tryInclude "category" (ProtocolParameter.encoder options) (oa.Category)
            GEncode.tryInclude "value" (Value.encoder options) (oa.Value)
            GEncode.tryInclude "unit" (OntologyAnnotation.encoder options) (oa.Unit)
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

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s

    let toJsonString (p:ProcessParameterValue) = 
        encoder (ConverterOptions()) p
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toStringLD (p:ProcessParameterValue) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) p
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (p:ProcessParameterValue) = 
    //    File.WriteAllText(path,toString p)

/// Functions for handling the ProcessInput Type
module ProcessInput =

    let encoder (options : ConverterOptions) (value : ProcessInput) = 
        match value with
        | ProcessInput.Source s-> 
            Source.encoder options s
        | ProcessInput.Sample s -> 
            Sample.encoder options s
        | ProcessInput.Data d -> 
            Data.encoder options d
        | ProcessInput.Material m -> 
            Material.encoder options m
        | _ -> Encode.nil

    let decoder (options : ConverterOptions) : Decoder<ProcessInput> =
        { new Decoder<ProcessInput> with
            member this.Decode(s,json) = 
                match Source.decoder(options).Decode(s,json) with
                | Ok s -> Ok (ProcessInput.Source s)
                | Error _ -> 
                    match Sample.decoder(options).Decode(s,json) with
                    | Ok s -> Ok (ProcessInput.Sample s)
                    | Error _ -> 
                        match Data.decoder(options).Decode(s,json) with
                        | Ok s -> Ok (ProcessInput.Data s)
                        | Error _ -> 
                            match Material.decoder(options).Decode(s,json) with
                            | Ok s -> Ok (ProcessInput.Material s)
                            | Error e -> Error e

        }

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s

    let toJsonString (m:ProcessInput) = 
        encoder (ConverterOptions()) m
        |> GEncode.toJsonString 2

    let toStringLD (m:ProcessInput) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) m
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (m:ProcessInput) = 
    //    File.WriteAllText(path,toString m)

/// Functions for handling the ProcessOutput Type
module ProcessOutput =

    let encoder (options : ConverterOptions) (value : ProcessOutput) = 
        match value with
        | ProcessOutput.Sample s -> 
            Sample.encoder options s
        | ProcessOutput.Data d -> 
            Data.encoder options d
        | ProcessOutput.Material m -> 
            Material.encoder options m
        | _ -> Encode.nil

    let decoder (options : ConverterOptions) : Decoder<ProcessOutput> =
        { new Decoder<ProcessOutput> with
            member this.Decode(s,json) = 
                match Sample.decoder(options).Decode(s,json) with
                | Ok s -> Ok (ProcessOutput.Sample s)
                | Error _ -> 
                    match Data.decoder(options).Decode(s,json) with
                    | Ok s -> Ok (ProcessOutput.Data s)
                    | Error _ -> 
                        match Material.decoder(options).Decode(s,json) with
                        | Ok s -> Ok (ProcessOutput.Material s)
                        | Error e -> Error e
        }

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s

    let toJsonString (m:ProcessOutput) = 
        encoder (ConverterOptions()) m
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (m:ProcessInput) = 
    //    File.WriteAllText(path,toString m)


module Process =    
    
    let genID (p:Process) : string = 
        match p.ID with
            | Some id -> URI.toString id
            | None -> match p.Name with
                        | Some n -> "#Process_" + n.Replace(" ","_")
                        | None -> "#EmptyProcess"

    let rec encoder (options : ConverterOptions) (oa : Process) = 
        [
            if options.SetID then "@id", Encode.string (oa |> genID)
                else GEncode.tryInclude "@id" Encode.string (oa.ID)
            if options.IncludeType then "@type", Encode.string "Process"
            GEncode.tryInclude "name" Encode.string (oa.Name)
            GEncode.tryInclude "executesProtocol" (Protocol.encoder options) (oa.ExecutesProtocol)
            GEncode.tryIncludeList "parameterValues" (ProcessParameterValue.encoder options) (oa.ParameterValues)
            GEncode.tryInclude "performer" Encode.string (oa.Performer)
            GEncode.tryInclude "date" Encode.string (oa.Date)
            GEncode.tryInclude "previousProcess" (encoder options) (oa.PreviousProcess)
            GEncode.tryInclude "nextProcess" (encoder options) (oa.NextProcess)
            GEncode.tryIncludeList "inputs" (ProcessInput.encoder options) (oa.Inputs)
            GEncode.tryIncludeList "outputs" (ProcessOutput.encoder options) (oa.Outputs)
            GEncode.tryIncludeList "comments" (Comment.encoder options) (oa.Comments)
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

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s

    let toJsonString (p:Process) = 
        encoder (ConverterOptions()) p
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toStringLD (p:Process) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) p
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (p:Process) = 
    //    File.WriteAllText(path,toString p)

module ProcessSequence = 

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (Decode.list (Process.decoder (ConverterOptions()))) s

    let toJsonString (p:Process list) = 
        p
        |> List.map (Process.encoder (ConverterOptions()))
        |> Encode.list
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toStringLD (p:Process list) = 
        p
        |> List.map (Process.encoder (ConverterOptions(SetID=true,IncludeType=true)))
        |> Encode.list
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (p:Process list) = 
    //    File.WriteAllText(path,toString p)