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
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            if options.IsJsonLD then 
                "@type", (Encode.list [Encode.string "ProcessParameterValue"])
                "additionalType", Encode.string "ProcessParameterValue"
            if options.IsJsonLD then
                if oa.Category.IsSome && oa.Category.Value.ParameterName.IsSome then
                    GEncode.tryInclude "category" Encode.string (oa.Category.Value.ParameterName.Value.Name)
                if oa.Category.IsSome && oa.Category.Value.ParameterName.IsSome then
                    GEncode.tryInclude "categoryCode" Encode.string (oa.Category.Value.ParameterName.Value.TermAccessionNumber)
                if oa.Value.IsSome then
                    "value", 
                        (match oa.Value.Value with
                            | Value.Float f -> 
                                Encode.float f
                            | Value.Int i -> 
                                Encode.int i
                            | Value.Name s -> 
                                Encode.string s
                            | Value.Ontology s -> 
                                Encode.string oa.ValueText
                        )
                if oa.Value.IsSome && oa.Value.Value.IsAnOntology then
                    GEncode.tryInclude "valueCode" Encode.string (oa.Value.Value.AsOntology()).TermAccessionNumber
                if oa.Unit.IsSome then GEncode.tryInclude "unit" Encode.string (oa.Unit.Value.Name)
                if oa.Unit.IsSome then GEncode.tryInclude "unitCode" Encode.string (oa.Unit.Value.TermAccessionNumber)
            else
                GEncode.tryInclude "category" (ProtocolParameter.encoder options) (oa.Category)
                GEncode.tryInclude "value" (Value.encoder options) (oa.Value)
                GEncode.tryInclude "unit" (OntologyAnnotation.encoder options) (oa.Unit)
            if options.IsJsonLD then
                "@context", ROCrateContext.ProcessParameterValue.context_jsonvalue
        ]
        |> GEncode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<ProcessParameterValue> =
        if not options.IsJsonLD then
            Decode.object (fun get ->
                {
                    Category = get.Optional.Field "category" (ProtocolParameter.decoder options)
                    Value = get.Optional.Field "value" (Value.decoder options)
                    Unit = get.Optional.Field "unit" (OntologyAnnotation.decoder options)
                }
            )
        else
            Decode.object (fun get ->
                let categoryName = get.Optional.Field "category" (Decode.string)
                let categoryCode = get.Optional.Field "categoryCode" (Decode.string)
                let category =
                    match categoryName,categoryCode with
                    | None,None -> None
                    | _ -> Some (ProtocolParameter.make None (Some (OntologyAnnotation.make None categoryName None categoryCode None)))
                let valueName = get.Optional.Field "value" (Value.decoder options)
                let valueCode = get.Optional.Field "valueCode" (Decode.string)
                let value =
                    match valueName,valueCode with
                    | Some (Value.Name name), Some code ->
                        let oa = OntologyAnnotation.make None (Some name) None (Some (URI.fromString code)) None
                        let vo = Value.Ontology(oa)
                        Some vo
                    | None, Some code ->
                        let oa = OntologyAnnotation.make None None None (Some (URI.fromString code)) None
                        let vo = Value.Ontology(oa)
                        Some vo
                    | Some (Value.Name name), None -> valueName
                    | Some (Value.Float name), None -> valueName
                    | Some (Value.Int name), None -> valueName
                    | _ -> None
                let unitName = get.Optional.Field "unit" (Decode.string)
                let unitCode = get.Optional.Field "unitCode" (Decode.string)
                let unit = 
                    match unitName,unitCode with
                    | None,None -> None
                    | _ -> Some (OntologyAnnotation.make None unitName None unitCode None)
                {
                    Category = category
                    Value = value
                    Unit = unit
                }
            )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (p:ProcessParameterValue) = 
        encoder (ConverterOptions()) p
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (p:ProcessParameterValue) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) p
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:ProcessParameterValue) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
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
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (m:ProcessInput) = 
        encoder (ConverterOptions()) m
        |> GEncode.toJsonString 2

    let toJsonldString (m:ProcessInput) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) m
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:ProcessInput) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
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
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (m:ProcessOutput) = 
        encoder (ConverterOptions()) m
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:ProcessOutput) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
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

    let rec encoder (options : ConverterOptions) (studyName:string Option) (assayName:string Option) (oa : Process) = 
        let performer = if options.IsJsonLD then ROCrateHelper.Person.authorListStringEncoder else Encode.string
        [
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            else 
                GEncode.tryInclude "@id" Encode.string (oa.ID)
            if options.IsJsonLD then 
                "@type", (Encode.list [Encode.string "Process"])
            GEncode.tryInclude "name" Encode.string (oa.Name)
            GEncode.tryInclude "executesProtocol" (Protocol.encoder options studyName assayName oa.Name) (oa.ExecutesProtocol)
            GEncode.tryIncludeList "parameterValues" (ProcessParameterValue.encoder options) (oa.ParameterValues)
            GEncode.tryInclude "performer" performer (oa.Performer)
            GEncode.tryInclude "date" Encode.string (oa.Date)
            if not options.IsJsonLD then
                GEncode.tryInclude "previousProcess" (encoder options studyName assayName) (oa.PreviousProcess)
                GEncode.tryInclude "nextProcess" (encoder options studyName assayName) (oa.NextProcess)
            GEncode.tryIncludeList "inputs" (ProcessInput.encoder options) (oa.Inputs)
            GEncode.tryIncludeList "outputs" (ProcessOutput.encoder options) (oa.Outputs)
            GEncode.tryIncludeList "comments" (Comment.encoder options) (oa.Comments)
            if options.IsJsonLD then 
                "@context", ROCrateContext.Process.context_jsonvalue
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
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (p:Process) = 
        encoder (ConverterOptions()) None None p
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (p:Process) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) None None p
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:Process) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) None None a
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
        |> List.map (Process.encoder (ConverterOptions()) None None)
        |> Encode.list
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (p:Process list) = 
        p
        |> List.map (Process.encoder (ConverterOptions(SetID=true,IsJsonLD=true)) None None)
        |> Encode.list
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (p:Process list) = 
        p
        |> List.map (Process.encoder (ConverterOptions(SetID=true,IsJsonLD=true)) None None)
        |> Encode.list
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (p:Process list) = 
    //    File.WriteAllText(path,toString p)