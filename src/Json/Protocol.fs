namespace ARCtrl.ISA.Json

open Thoth.Json.Core

open ARCtrl.ISA

module ProtocolParameter =
    
    let genID (pp:ProtocolParameter) : string = 
        match pp.ID with
        | Some id -> URI.toString id
        | None -> match pp.ParameterName with
                  | Some n when not n.ID.IsNone -> "#Param_" + n.ID.Value
                  | _ -> "#EmptyProtocolParameter"

    let encoder (options : ConverterOptions) (oa : ProtocolParameter) = 
        if options.IsJsonLD then
            match oa.ParameterName with
            | Some n -> OntologyAnnotation.encoder options n
            | None -> [] |> GEncode.choose |> Encode.object
        else
            [
                if options.SetID then 
                    "@id", Encode.string (oa |> genID)
                else 
                    GEncode.tryInclude "@id" Encode.string (oa.ID)
                if options.IsJsonLD then 
                    "@type", (Encode.list [Encode.string "ProtocolParameter"])
                GEncode.tryInclude "parameterName" (OntologyAnnotation.encoder options) (oa.ParameterName)
                if options.IsJsonLD then
                    "@context", ROCrateContext.ProtocolParameter.context_jsonvalue
            ]
            |> GEncode.choose
            |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<ProtocolParameter> =
        Decode.object (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                ParameterName = get.Optional.Field "parameterName" (OntologyAnnotation.decoder options)
            }
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toString (p:ProtocolParameter) = 
        encoder (ConverterOptions()) p
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (p:ProtocolParameter) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) p
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:ProtocolParameter) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (p:ProtocolParameter) = 
    //    File.WriteAllText(path,toString p)

module Component =
    
    let genID (c:Component) = 
        let name = Component.composeName c.ComponentValue c.ComponentUnit
        "#Component_" + name.Replace(" ","_")
        // match c.ComponentName with
        // | Some cn -> "#Component_" + cn.Replace(" ","_")
        // | None -> "#EmptyComponent"

    let encoder (options : ConverterOptions) (oa : Component) = 
        if options.IsJsonLD then
            [
                if options.SetID then 
                    "@id", Encode.string (oa |> genID)
                "@type", (Encode.list [Encode.string "Component"])
                if oa.ComponentType.IsSome then
                    GEncode.tryInclude "category" Encode.string (oa.ComponentType.Value.Name)
                    GEncode.tryInclude "categoryCode" Encode.string (oa.ComponentType.Value.TermAccessionNumber)
                if oa.ComponentValue.IsSome then "value", Encode.string (oa.ValueText)
                if oa.ComponentValue.IsSome && oa.ComponentValue.Value.IsAnOntology then
                    GEncode.tryInclude "valueCode" Encode.string (oa.ComponentValue.Value.AsOntology()).TermAccessionNumber
                if oa.ComponentUnit.IsSome then GEncode.tryInclude "unit" Encode.string (oa.ComponentUnit.Value.Name)
                if oa.ComponentUnit.IsSome then GEncode.tryInclude "unitCode" Encode.string (oa.ComponentUnit.Value.TermAccessionNumber)
                "@context", ROCrateContext.Component.context_jsonvalue
            ]
        else
            [
                if options.SetID then 
                    "@id", Encode.string (oa |> genID)
                "componentName", Encode.string (Component.composeName oa.ComponentValue oa.ComponentUnit)
                GEncode.tryInclude "componentType" (OntologyAnnotation.encoder options) (oa.ComponentType)
            ]
        |> GEncode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<Component> =
        if not options.IsJsonLD then
            Decode.object (fun get ->
                let name = get.Optional.Field "componentName" GDecode.uri
                let value, unit =
                    match name with
                    | Some n -> 
                        let v,u = Component.decomposeName n
                        Some v, u
                    | None -> None, None
                {
                    ComponentName = None
                    ComponentValue = value
                    ComponentUnit = unit
                    ComponentType = get.Optional.Field "componentType" (OntologyAnnotation.decoder options)
                }
            )
        else 
            Decode.object (fun get ->
                let categoryName = get.Optional.Field "category" (Decode.string)
                let categoryCode = get.Optional.Field "categoryCode" (Decode.string)
                let category =
                    match categoryName,categoryCode with
                    | None,None -> None
                    | _ -> Some (OntologyAnnotation.make None categoryName None categoryCode None)
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
                    ComponentName = None
                    ComponentValue = value
                    ComponentUnit = unit
                    ComponentType = category
                }
            )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (p:Component) = 
        encoder (ConverterOptions()) p
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (p:Component) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) p
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:Component) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (p:Component) = 
    //    File.WriteAllText(path,toString p)

module Protocol =   
    
    let genID (studyName:string Option) (assayName:string Option) (processName:string Option) (p:Protocol): string = 
        match p.ID with
        | Some id -> URI.toString id 
        | None -> match p.Uri with
                  | Some u -> u
                  | None -> match p.Name with
                            | Some n -> "#Protocol_" + n.Replace(" ","_")
                            | None -> match (studyName,assayName,processName) with
                                      | (Some sn, Some an, Some pn) -> "#Protocol_" + sn.Replace(" ","_") + "_" + an.Replace(" ","_") + "_" + pn.Replace(" ","_")
                                      | (Some sn, None, Some pn) -> "#Protocol_" + sn.Replace(" ","_") + "_" + pn.Replace(" ","_")
                                      | (None, None, Some pn) -> "#Protocol_" + pn.Replace(" ","_")
                                      | _ -> "#EmptyProtocol" 

    let encoder (options : ConverterOptions) (studyName:string Option) (assayName:string Option) (processName:string Option) (oa : Protocol) = 
        [
            if options.SetID then 
                "@id", Encode.string (genID studyName assayName processName oa)
            else 
                GEncode.tryInclude "@id" Encode.string (oa.ID)
            if options.IsJsonLD then 
                "@type", (Encode.list [Encode.string "Protocol"])
            GEncode.tryInclude "name" Encode.string (oa.Name)
            GEncode.tryInclude "protocolType" (OntologyAnnotation.encoder options) (oa.ProtocolType)
            GEncode.tryInclude "description" Encode.string (oa.Description)
            GEncode.tryInclude "uri" Encode.string (oa.Uri)
            GEncode.tryInclude "version" Encode.string (oa.Version)
            if not options.IsJsonLD then 
                GEncode.tryIncludeList "parameters" (ProtocolParameter.encoder options) (oa.Parameters)
            GEncode.tryIncludeList "components" (Component.encoder options) (oa.Components)
            GEncode.tryIncludeList "comments" (Comment.encoder options) (oa.Comments)
            if options.IsJsonLD then 
                "@context", ROCrateContext.Protocol.context_jsonvalue
        ]
        |> GEncode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<Protocol> =
        Decode.object (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                Name = get.Optional.Field "name" Decode.string
                ProtocolType = get.Optional.Field "protocolType" (OntologyAnnotation.decoder options)
                Description = get.Optional.Field "description" Decode.string
                Uri = get.Optional.Field "uri" GDecode.uri
                Version = get.Optional.Field "version" Decode.string
                Parameters = get.Optional.Field "parameters" (Decode.list (ProtocolParameter.decoder options))
                Components = get.Optional.Field "components" (Decode.list (Component.decoder options))
                Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))
            }
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (p:Protocol) = 
        encoder (ConverterOptions()) None None None p
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toJsonldString (p:Protocol) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) None None None p
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:Protocol) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) None None None a
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (p:Protocol) = 
    //    File.WriteAllText(path,toString p)