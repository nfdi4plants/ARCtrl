namespace ARCtrl.ISA.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ARCtrl.ISA
open System.IO


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
    
    let genID (m:MaterialAttribute) : string = 
        match m.ID with
            | Some id -> URI.toString id
            | None -> "#EmptyMaterialAttribute"

    let encoder (options : ConverterOptions) (oa : obj) = 
        [
            if options.SetID then "@id", GEncode.encodeToString (oa :?> MaterialAttribute |> genID)
                else GEncode.tryInclude "@id" GEncode.encodeToString (oa |> GEncode.tryGetPropertyValue "ID")
            if options.IncludeType then "@type", GEncode.encodeToString "MaterialAttribute"
            GEncode.tryInclude "characteristicType" (OntologyAnnotation.encoder options) (oa |> GEncode.tryGetPropertyValue "CharacteristicType")
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

    let decodeFromString (s:string) = 
        GDecode.decodeFromString (decoder (ConverterOptions())) s

    let encodeToString (m:MaterialAttribute) = 
        encoder (ConverterOptions()) m
        |> Encode.toString 2
    
    /// exports in json-ld format
    let toStringLD (m:MaterialAttribute) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) m
        |> Encode.toString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (m:MaterialAttribute) = 
    //    File.WriteAllText(path,toString m)

module MaterialAttributeValue =
    
    let genID (m:MaterialAttributeValue) : string = 
        match m.ID with
        | Some id -> URI.toString id
        | None -> "#EmptyMaterialAttributeValue"

    let encoder (options : ConverterOptions) (oa : obj) = 
        [
            if options.SetID then "@id", GEncode.encodeToString (oa :?> MaterialAttributeValue |> genID)
                else GEncode.tryInclude "@id" GEncode.encodeToString (oa |> GEncode.tryGetPropertyValue "ID")
            if options.IncludeType then "@type", GEncode.encodeToString "MaterialAttributeValue"
            GEncode.tryInclude "category" (MaterialAttribute.encoder options) (oa |> GEncode.tryGetPropertyValue "Category")
            GEncode.tryInclude "value" (Value.encoder options) (oa |> GEncode.tryGetPropertyValue "Value")
            GEncode.tryInclude "unit" (OntologyAnnotation.encoder options) (oa |> GEncode.tryGetPropertyValue "Unit")
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

    let decodeFromString (s:string) = 
        GDecode.decodeFromString (decoder (ConverterOptions())) s

    let encodeToString (m:MaterialAttributeValue) = 
        encoder (ConverterOptions()) m
        |> Encode.toString 2
    
    /// exports in json-ld format
    let toStringLD (m:MaterialAttributeValue) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) m
        |> Encode.toString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (m:MaterialAttributeValue) = 
    //    File.WriteAllText(path,toString m)


module Material = 
    
    let genID (m:Material) : string = 
        match m.ID with
            | Some id -> id
            | None -> match m.Name with
                        | Some n -> "#Material_" + n.Replace(" ","_")
                        | None -> "#EmptyMaterial"
    
    let rec encoder (options : ConverterOptions) (oa : obj) = 
        [
            if options.SetID then "@id", GEncode.encodeToString (oa :?> Material |> genID)
                else GEncode.tryInclude "@id" GEncode.encodeToString (oa |> GEncode.tryGetPropertyValue "ID")
            if options.IncludeType then "@type", GEncode.encodeToString "Material"
            GEncode.tryInclude "name" GEncode.encodeToString (oa |> GEncode.tryGetPropertyValue "Name")
            GEncode.tryInclude "type" (MaterialType.encoder options) (oa |> GEncode.tryGetPropertyValue "MaterialType")
            GEncode.tryInclude "characteristics" (MaterialAttributeValue.encoder options) (oa |> GEncode.tryGetPropertyValue "Characteristics")
            GEncode.tryInclude "derivesFrom" (encoder options) (oa |> GEncode.tryGetPropertyValue "DerivesFrom")
        ]
        |> GEncode.choose
        |> Encode.object

    let rec decoder (options : ConverterOptions) : Decoder<Material> =
        fun s json ->
            if GDecode.hasUnknownFields ["@id";"@type";"name";"type";"characteristics";"derivesFrom"] json then
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

    let decodeFromString (s:string) = 
        GDecode.decodeFromString (decoder (ConverterOptions())) s

    let encodeToString (m:Material) = 
        encoder (ConverterOptions()) m
        |> Encode.toString 2
    
    /// exports in json-ld format
    let toStringLD (m:Material) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) m
        |> Encode.toString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (m:Material) = 
    //    File.WriteAllText(path,toString m)