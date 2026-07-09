namespace ARCtrl.Yaml.ROCrate

open ARCtrl.ROCrate
open YAMLicious.YAMLiciousTypes

module LDContext =

    let rec decoder (value: YAMLElement) : LDContext =
        match Helpers.unwrapSingleObject value with
        | YAMLElement.Object _ ->
            let context = ARCtrl.ROCrate.LDContext()
            for (k, v) in Helpers.getMappings value do
                context.AddMapping(k, Helpers.decodeString v)
            context
        | YAMLElement.Sequence items ->
            let baseContexts = items |> List.map decoder |> ResizeArray
            ARCtrl.ROCrate.LDContext(baseContexts = baseContexts)
        | YAMLElement.Value v ->
            let s = v.Value
            if s = Context.proxy_V1_2DRAFT || s = Context.proxy_V1_2 then
                Context.initV1_2()
            elif s = Context.proxy_V1_1 then
                Context.initV1_1()
            else
                failwithf "Unsupported context string: %s" s
        | _ ->
            failwithf "Unsupported @context YAML format: %A" value

    let rec encoder (ctx: LDContext) =
        match ctx.Name with
        | Some Context.proxy_V1_2DRAFT -> Helpers.yamlValue Context.proxy_V1_2DRAFT
        | Some Context.proxy_V1_2 -> Helpers.yamlValue Context.proxy_V1_2
        | Some Context.proxy_V1_1 -> Helpers.yamlValue Context.proxy_V1_1
        | _ ->
            let mappings =
                ctx.Mappings
                |> Seq.map (fun kv -> kv.Key, Helpers.yamlValue (string kv.Value))
                |> Seq.toList
                |> Helpers.yamlMap
            if ctx.BaseContexts.Count = 0 then
                mappings
            elif ctx.BaseContexts.Count = 1 && ctx.Mappings.Count = 0 then
                ctx.BaseContexts.[0] |> encoder
            else
                ctx.BaseContexts
                |> Seq.map encoder
                |> Seq.append [if ctx.Mappings.Count <> 0 then mappings]
                |> Seq.toList
                |> Helpers.yamlSeq
