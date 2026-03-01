namespace ARCtrl.Yaml.ROCrate

open ARCtrl.ROCrate
open YAMLicious.YAMLiciousTypes

module LDGraph =

    let decoder (value: YAMLElement) : ARCtrl.ROCrate.LDGraph =
        let id = Helpers.tryGetField "@id" value |> Option.map Helpers.decodeString
        let context = Helpers.tryGetField "@context" value |> Option.map LDContext.decoder
        let graphValues = Helpers.requireField "@graph" value

        let nodes =
            match Helpers.tryDecodeSequence graphValues with
            | Some elements -> elements |> List.map LDNode.decoder |> ResizeArray
            | None -> failwithf "Expected '@graph' to be a sequence but got %A" graphValues

        let graph = ARCtrl.ROCrate.LDGraph(?id = id, ?context = context)

        for (property, propertyValue) in Helpers.getMappings value do
            if property <> "@id" && property <> "@graph" && property <> "@context" then
                graph.SetProperty(property, LDNode.genericDecoder propertyValue)

        for node in nodes do
            graph.AddNode node

        graph

    let encoder (obj: ARCtrl.ROCrate.LDGraph) =
        [
            match obj.Id with
            | Some id -> yield "@id", Helpers.yamlValue id
            | None -> ()
            match obj.TryGetContext() with
            | Some ctx -> yield "@context", LDContext.encoder ctx
            | None -> ()
            for kv in (obj.GetProperties true) do
                let l = kv.Key.ToLower()
                if l <> "id" && l <> "@context" && l <> "nodes" && l <> "mappings" then
                    yield kv.Key, LDNode.genericEncoder kv.Value
            yield "@graph", (obj.Nodes |> Seq.map LDNode.encoder |> Seq.toList |> Helpers.yamlSeq)
        ]
        |> Helpers.yamlMap

    let fromROCrateYamlString (s: string) =
        s
        |> Helpers.sanitizeROCrateYamlKeys
        |> ARCtrl.Yaml.Decode.fromYamlString decoder

    let toROCrateYamlString (whitespace: int option) (obj: ARCtrl.ROCrate.LDGraph) =
        encoder obj
        |> ARCtrl.Yaml.Encode.toYamlString (ARCtrl.Yaml.Encode.defaultWhitespace whitespace)
