namespace ARCtrl.ISA.Json

open Thoth.Json.Core

open ARCtrl.ISA
open System.IO

module DataFile = 

    let encoder (options : ConverterOptions) (value : DataFile) = 
        match value with
        | DataFile.RawDataFile -> 
            Encode.string "Raw Data File"
        | DataFile.DerivedDataFile  -> 
            Encode.string "Derived Data File"
        | DataFile.ImageFile  -> 
            Encode.string "Image File"

    let decoder (options : ConverterOptions) : Decoder<DataFile> =
        { new Decoder<DataFile> with
            member this.Decode (s,json) = 
                match Decode.string.Decode(s,json) with
                | Ok "Raw Data File" -> 
                    Ok DataFile.RawDataFile
                | Ok "Derived Data File" -> 
                    Ok DataFile.DerivedDataFile
                | Ok "Image File" -> 
                    Ok DataFile.ImageFile
                | Ok s -> Error (DecoderError($"Could not parse {s}.", ErrorReason.BadPrimitive(s,json)))
                | Error e -> Error e
        }

module Data = 
    
    let genID (d:Data) : string = 
        match d.ID with
        | Some id -> URI.toString id
        | None -> match d.Name with
                  | Some n -> n.Replace(" ","_")
                  | None -> "#EmptyData"
    
    let rec encoder (options : ConverterOptions) (oa : Data) = 
        [
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            else 
                GEncode.tryInclude "@id" Encode.string (oa.ID)
            if options.IsJsonLD then 
                "@type", (Encode.list [Encode.string "Data"])
            GEncode.tryInclude "name" Encode.string (oa.Name)
            GEncode.tryInclude "type" (DataFile.encoder options) (oa.DataType)
            GEncode.tryIncludeList "comments" (Comment.encoder options) (oa.Comments)
            if options.IsJsonLD then
                "@context", ROCrateContext.Data.context_jsonvalue
        ]
        |> GEncode.choose
        |> Encode.object

    let allowedFields = ["@id";"name";"type";"comments";"@type"; "@context"]

    let rec decoder (options : ConverterOptions) : Decoder<Data> =
        GDecode.object allowedFields (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                Name = get.Optional.Field "name" Decode.string
                DataType = get.Optional.Field "type" (DataFile.decoder options)
                Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))
            }
            
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (m:Data) = 
        encoder (ConverterOptions()) m
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (d:Data) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) d
        |> GEncode.toJsonString 2
    let toJsonldStringWithContext (a:Data) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (m:Data) = 
    //    File.WriteAllText(path,toString m)


module Source = 
    
    let genID (s:Source) : string = 
        match s.ID with
        | Some id -> URI.toString id
        | None -> match s.Name with
                  | Some n -> "#Source_" + n.Replace(" ","_")
                  | None -> "#EmptySource"
    
    let rec encoder (options : ConverterOptions) (oa : Source) = 
        [
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            else 
                GEncode.tryInclude "@id" Encode.string (oa.ID)
            if options.IsJsonLD then 
                "@type", (Encode.list [ Encode.string "Source"])
            GEncode.tryInclude "name" Encode.string (oa.Name)
            GEncode.tryIncludeList "characteristics" (MaterialAttributeValue.encoder options) (oa.Characteristics)      
            if options.IsJsonLD then
                "@context", ROCrateContext.Source.context_jsonvalue
            ]
        |> GEncode.choose
        |> Encode.object

    let allowedFields = ["@id";"name";"characteristics";"@type"; "@context"]

    let rec decoder (options : ConverterOptions) : Decoder<Source> =     
        GDecode.object allowedFields (fun get ->
            
                {
                    ID = get.Optional.Field "@id" GDecode.uri
                    Name = get.Optional.Field "name" Decode.string
                    Characteristics = get.Optional.Field "characteristics" (Decode.list (MaterialAttributeValue.decoder options))
                } 
            
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (m:Source) = 
        encoder (ConverterOptions()) m
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (s:Source) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) s
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:Source) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (m:Source) = 
    //    File.WriteAllText(path,toString m)

module Sample = 
    
    let genID (s:Sample) : string = 
        match s.ID with
        | Some id -> id
        | None -> match s.Name with
                  | Some n -> "#Sample_" + n.Replace(" ","_")
                  | None -> "#EmptySample"
    
    let encoder (options : ConverterOptions) (oa : Sample) = 
        [
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            else 
                GEncode.tryInclude "@id" Encode.string (oa.ID)
            if options.IsJsonLD then 
                "@type", (Encode.list [ Encode.string "Sample"])
            GEncode.tryInclude "name" Encode.string (oa.Name)
            GEncode.tryIncludeList "characteristics" (MaterialAttributeValue.encoder options) (oa.Characteristics)
            GEncode.tryIncludeList "factorValues" (FactorValue.encoder options) (oa.FactorValues)
            if not options.IsJsonLD then 
                GEncode.tryIncludeList "derivesFrom" (Source.encoder options) (oa.DerivesFrom)
            if options.IsJsonLD then
                "@context", ROCrateContext.Sample.context_jsonvalue
        ]
        |> GEncode.choose
        |> Encode.object

    let allowedFields = ["@id";"name";"characteristics";"factorValues";"derivesFrom";"@type"; "@context"]

    let decoder (options : ConverterOptions) : Decoder<Sample> =       
        GDecode.object allowedFields (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                Name = get.Optional.Field "name" Decode.string
                Characteristics = get.Optional.Field "characteristics" (Decode.list (MaterialAttributeValue.decoder options))
                FactorValues = get.Optional.Field "factorValues" (Decode.list (FactorValue.decoder options))
                DerivesFrom = get.Optional.Field "derivesFrom" (Decode.list (Source.decoder options))
            }
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (m:Sample) = 
        encoder (ConverterOptions()) m
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (s:Sample) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) s
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:Sample) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (m:Sample) = 
    //    File.WriteAllText(path,toString m)