namespace ARCtrl.Yaml.ROCrate

open ARCtrl.ROCrate
open YAMLicious.YAMLiciousTypes

module rec LDNode =

    #if !FABLE_COMPILER
    let (|SomeObj|_|) =
        let ty = typedefof<option<_>>
        fun (a:obj) ->
            if isNull a then
                None
            else
                let aty = a.GetType()
                let v = aty.GetProperty("Value")
                if aty.IsGenericType && aty.GetGenericTypeDefinition() = ty then
                    Some(v.GetValue(a, [| |]))
                else
                    None
    #endif

    let rec genericDecoder (value: YAMLElement) : obj =
        match Helpers.unwrapSingleObject value with
        | YAMLElement.Value v -> Helpers.parseScalarToObj v.Value
        | YAMLElement.Sequence elements ->
            elements
            |> List.map genericDecoder
            |> ResizeArray
            |> box
        | YAMLElement.Object _ when Helpers.tryGetField "@value" value |> Option.isSome ->
            LDValue.decoder value |> box
        | YAMLElement.Object _ when Helpers.tryGetField "@type" value |> Option.isSome ->
            decoder value |> box
        | YAMLElement.Object _ when Helpers.tryGetField "@id" value |> Option.isSome ->
            LDRef.decoder value |> box
        | _ ->
            failwithf "Unsupported YAML element for generic RO-Crate value: %A" value

    let rec genericEncoder (obj : obj) =
        match obj with
        | :? string as s -> Helpers.yamlValue s
        | :? int as i -> Helpers.yamlValue (string i)
        | :? bool as b -> Helpers.yamlValue (if b then "true" else "false")
        | :? float as f -> Helpers.yamlValue (f.ToString(System.Globalization.CultureInfo.InvariantCulture))
        | :? System.DateTime as d -> Helpers.yamlValue (d.ToString("O", System.Globalization.CultureInfo.InvariantCulture))
        | :? LDValue as v -> LDValue.encoder v
        | :? LDRef as r -> LDRef.encoder r
        | :? ARCtrl.ROCrate.LDNode as o -> encoder o
        #if !FABLE_COMPILER
        | SomeObj o -> genericEncoder o
        #endif
        | null -> Helpers.yamlValue "null"
        | :? System.Collections.IEnumerable as l ->
            l
            |> Seq.cast<obj>
            |> Seq.map genericEncoder
            |> Seq.toList
            |> Helpers.yamlSeq
        | _ -> failwith "Unknown type"

    and decoder (value: YAMLElement) : ARCtrl.ROCrate.LDNode =
        let schemaType = Helpers.requireField "@type" value |> Helpers.decodeStringResizeArray
        let id = Helpers.requireField "@id" value |> Helpers.decodeString
        let context = Helpers.tryGetField "@context" value |> Option.map LDContext.decoder
        let additionalType = Helpers.tryGetField "additionalType" value |> Option.map Helpers.decodeStringResizeArray

        let node = ARCtrl.ROCrate.LDNode(id, schemaType, ?additionalType = additionalType)

        for (property, propertyValue) in Helpers.getMappings value do
            if property <> "@id" && property <> "@type" && property <> "@context" then
                let decoded = genericDecoder propertyValue
                if decoded <> null then
                    node.SetProperty(property, decoded)

        match context with
        | Some ctx -> node.SetContext ctx
        | None -> ()

        node

    and encoder (obj: ARCtrl.ROCrate.LDNode) =
        [
            yield "@id", Helpers.yamlValue obj.Id
            yield "@type", (obj.SchemaType |> Seq.map Helpers.yamlValue |> Seq.toList |> Helpers.yamlSeq)
            if obj.AdditionalType.Count <> 0 then
                yield "additionalType", (obj.AdditionalType |> Seq.map Helpers.yamlValue |> Seq.toList |> Helpers.yamlSeq)
            match obj.TryGetContext() with
            | Some ctx -> yield "@context", LDContext.encoder ctx
            | _ -> ()
            for kv in (obj.GetProperties true) do
                let l = kv.Key.ToLower()
                if l <> "id" && l <> "schematype" && l <> "additionaltype" && l <> "@context" && (l.StartsWith("init@") |> not) && (l.StartsWith("init_") |> not) then
                    yield kv.Key, genericEncoder kv.Value
        ]
        |> Helpers.yamlMap

    let fromROCrateYamlString (s: string) =
        s
        |> Helpers.ensureSingleYamlDocument
        |> ARCtrl.Yaml.Decode.fromYamlString decoder

    let toROCrateYamlString (whitespace: int option) (obj: ARCtrl.ROCrate.LDNode) =
        encoder obj
        |> ARCtrl.Yaml.Encode.toYamlString (ARCtrl.Yaml.Encode.defaultWhitespace whitespace)
