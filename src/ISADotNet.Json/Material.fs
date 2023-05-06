namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISADotNet
open System.IO
open GEncode

module MaterialType = 

    let encoder (options : ConverterOptions) (value : obj) = 
        match value with
        | :? MaterialType as MaterialType.ExtractName -> 
            Encode.string "Extract Name"
        | :? MaterialType as MaterialType.LabeledExtractName -> 
            Encode.string "Labeled Extract Name"
        | _ -> Encode.nil

    let decoder (options : ConverterOptions) : Decoder<MaterialType> =
        fun s json ->
            match Decode.string s json with
            | Ok "Extract Name" -> Ok (MaterialType.ExtractName)
            | Ok "Labeled Extract Name" -> Ok (MaterialType.LabeledExtractName)
            | Ok s -> Error (DecoderError($"Could not parse {s}No other value than \"Extract Name\" or \"Labeled Extract Name\" allowed for materialtype", ErrorReason.BadPrimitive(s,Encode.nil)))
            | Error e -> Error e


module MaterialAttribute =

    let encoder (options : ConverterOptions) (oa : obj) = 
        [
            tryInclude "@id" GEncode.string (oa |> tryGetPropertyValue "ID")
            tryInclude "characteristicType" (OntologyAnnotation.encoder options) (oa |> tryGetPropertyValue "CharacteristicType")
        ]
        |> GEncode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<MaterialAttribute> =
        Decode.object (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                CharacteristicType = get.Optional.Field "characteristicType" (OntologyAnnotation.decoder options)
            }
        )

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s

    let toString (m:MaterialAttribute) = 
        encoder (ConverterOptions()) m
        |> Encode.toString 2

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (m:MaterialAttribute) = 
        File.WriteAllText(path,toString m)

module MaterialAttributeValue =

    let encoder (options : ConverterOptions) (oa : obj) = 
        [
            tryInclude "@id" GEncode.string (oa |> tryGetPropertyValue "ID")
            tryInclude "category" (MaterialAttribute.encoder options) (oa |> tryGetPropertyValue "Category")
            tryInclude "value" (Value.encoder options) (oa |> tryGetPropertyValue "Value")
            tryInclude "unit" (OntologyAnnotation.encoder options) (oa |> tryGetPropertyValue "Unit")
        ]
        |> GEncode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<MaterialAttributeValue> =
        Decode.object (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                Category = get.Optional.Field "category" (MaterialAttribute.decoder options)
                Value = get.Optional.Field "value" (Value.decoder options)
                Unit = get.Optional.Field "unit" (OntologyAnnotation.decoder options)
            }
        )

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s

    let toString (m:MaterialAttributeValue) = 
        encoder (ConverterOptions()) m
        |> Encode.toString 2

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (m:MaterialAttributeValue) = 
        File.WriteAllText(path,toString m)


module Material = 
    
    let rec encoder (options : ConverterOptions) (oa : obj) = 
        [
            tryInclude "@id" GEncode.string (oa |> tryGetPropertyValue "ID")
            tryInclude "name" GEncode.string (oa |> tryGetPropertyValue "Name")
            tryInclude "type" (MaterialType.encoder options) (oa |> tryGetPropertyValue "MaterialType")
            tryInclude "characteristics" (MaterialAttributeValue.encoder options) (oa |> tryGetPropertyValue "Characteristics")
            tryInclude "derivesFrom" (encoder options) (oa |> tryGetPropertyValue "DerivesFrom")
        ]
        |> GEncode.choose
        |> Encode.object

    let rec decoder (options : ConverterOptions) : Decoder<Material> =
        fun s json ->
            if GDecode.hasUnknownFields ["@id";"name";"type";"characteristics";"derivesFrom"] json then
                Error (DecoderError("Unknown fields in material", ErrorReason.BadPrimitive(s,Encode.nil)))
            else
                Decode.object (fun get ->
                    {
                        ID = get.Optional.Field "@id" GDecode.uri
                        Name = get.Optional.Field "name" Decode.string
                        MaterialType = get.Optional.Field "type" (MaterialType.decoder options)
                        Characteristics = get.Optional.Field "characteristics" (Decode.list (MaterialAttributeValue.decoder options))
                        DerivesFrom = get.Optional.Field "derivesFrom" (Decode.list (decoder options))
                    }
                ) s json

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s

    let toString (m:Material) = 
        encoder (ConverterOptions()) m
        |> Encode.toString 2

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (m:Material) = 
        File.WriteAllText(path,toString m)