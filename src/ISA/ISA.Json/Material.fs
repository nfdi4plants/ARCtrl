namespace ARCtrl.ISA.Json

open Thoth.Json.Core

open ARCtrl.ISA
open System.IO


module MaterialType = 

    let encoder (options : ConverterOptions) (value : MaterialType) = 
        match value with
        | MaterialType.ExtractName -> 
            Encode.string "Extract Name"
        | MaterialType.LabeledExtractName -> 
            Encode.string "Labeled Extract Name"
        | _ -> Encode.nil

    let decoder (options : ConverterOptions) : Decoder<MaterialType> =
        { new Decoder<MaterialType> with
            member this.Decode (s,json) = 
                match Decode.string.Decode(s,json) with
                | Ok "Extract Name" -> Ok MaterialType.ExtractName
                | Ok "Labeled Extract Name" -> Ok MaterialType.LabeledExtractName
                | Ok s -> Error (DecoderError($"Could not parse {s}No other value than \"Extract Name\" or \"Labeled Extract Name\" allowed for materialtype", ErrorReason.BadPrimitive(s,json)))
                | Error e -> Error e
        
        }


module MaterialAttribute =
    
    let genID (m:MaterialAttribute) : string = 
        match m.ID with
            | Some id -> URI.toString id
            | None -> "#EmptyMaterialAttribute"

    let encoder (options : ConverterOptions) (oa : MaterialAttribute) = 
        if options.IsJsonLD then
            match oa.CharacteristicType with
            | Some t -> OntologyAnnotation.encoder options t
            | None -> [] |> GEncode.choose |> Encode.object
        else
            [
                if options.SetID then 
                    "@id", Encode.string (oa |> genID)
                else 
                    GEncode.tryInclude "@id" Encode.string (oa.ID)
                if options.IsJsonLD then 
                    "@type", (Encode.list [Encode.string "MaterialAttribute"])
                GEncode.tryInclude "characteristicType" (OntologyAnnotation.encoder options) (oa.CharacteristicType)
                if options.IsJsonLD then
                    "@context", ROCrateContext.MaterialAttribute.context_jsonvalue
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

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (m:MaterialAttribute) = 
        encoder (ConverterOptions()) m
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (m:MaterialAttribute) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) m
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:MaterialAttribute) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> GEncode.toJsonString 2

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

    let encoder (options : ConverterOptions) (oa : MaterialAttributeValue) = 
        [
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            else 
                GEncode.tryInclude "@id" Encode.string (oa.ID)
            if options.IsJsonLD then 
                "@type", (Encode.list [Encode.string "MaterialAttributeValue"])
            if options.IsJsonLD then
                if oa.Category.IsSome && oa.Category.Value.CharacteristicType.IsSome then
                    GEncode.tryInclude "category" Encode.string (oa.Category.Value.CharacteristicType.Value.Name)
                if oa.Category.IsSome && oa.Category.Value.CharacteristicType.IsSome then
                    GEncode.tryInclude "categoryCode" Encode.string (oa.Category.Value.CharacteristicType.Value.TermAccessionNumber)
                if oa.Value.IsSome then "value", Encode.string (oa.ValueText)
                if oa.Value.IsSome && oa.Value.Value.IsAnOntology then
                    GEncode.tryInclude "valueCode" Encode.string (oa.Value.Value.AsOntology()).TermAccessionNumber
                if oa.Unit.IsSome then GEncode.tryInclude "unit" Encode.string (oa.Unit.Value.Name)
                if oa.Unit.IsSome then GEncode.tryInclude "unitCode" Encode.string (oa.Unit.Value.TermAccessionNumber)
            else
                GEncode.tryInclude "category" (MaterialAttribute.encoder options) (oa.Category)
                GEncode.tryInclude "value" (Value.encoder options) (oa.Value)
                GEncode.tryInclude "unit" (OntologyAnnotation.encoder options) (oa.Unit)
            if options.IsJsonLD then 
                "@context", ROCrateContext.MaterialAttributeValue.context_jsonvalue
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

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (m:MaterialAttributeValue) = 
        encoder (ConverterOptions()) m
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (m:MaterialAttributeValue) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) m
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:MaterialAttributeValue) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> GEncode.toJsonString 2

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
    
    let rec encoder (options : ConverterOptions) (oa : Material) = 
        [
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            else 
                GEncode.tryInclude "@id" Encode.string (oa.ID)
            if options.IsJsonLD then 
                "@type", (Encode.list [Encode.string "Material"])
            GEncode.tryInclude "name" Encode.string (oa.Name)
            GEncode.tryInclude "type" (MaterialType.encoder options) (oa.MaterialType)
            GEncode.tryIncludeList "characteristics" (MaterialAttributeValue.encoder options) (oa.Characteristics)
            GEncode.tryIncludeList "derivesFrom" (encoder options) (oa.DerivesFrom)
            if options.IsJsonLD then 
                "@context", ROCrateContext.Material.context_jsonvalue
        ]
        |> GEncode.choose
        |> Encode.object

    let allowedFields = ["@id";"@type";"name";"type";"characteristics";"derivesFrom"; "@context"]

    let rec decoder (options : ConverterOptions) : Decoder<Material> =       
        GDecode.object allowedFields (fun get -> 
            {                       
                ID = get.Optional.Field "@id" GDecode.uri
                Name = get.Optional.Field "name" Decode.string
                MaterialType = get.Optional.Field "type" (MaterialType.decoder options)
                Characteristics = get.Optional.Field "characteristics" (Decode.list (MaterialAttributeValue.decoder options))
                DerivesFrom = get.Optional.Field "derivesFrom" (Decode.list (decoder options))
            }
        )
        
    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (m:Material) = 
        encoder (ConverterOptions()) m
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (m:Material) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) m
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:Material) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (m:Material) = 
    //    File.WriteAllText(path,toString m)