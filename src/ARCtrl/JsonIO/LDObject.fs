namespace ARCtrl.Json

open ARCtrl
open ARCtrl.ROCrate
open System
open ARCtrl.ROCrate
open Thoth.Json.Core
open DynamicObj
open Fable.Core

[<AutoOpen>]
module LDNodeExtensions =

    type LDNode with

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString LDNode.decoder s

        /// exports in json-ld format
        static member toROCrateJsonString(?spaces) =
            fun (obj:LDNode) ->
                LDNode.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?spaces) = 
            LDNode.toROCrateJsonString(?spaces=spaces) this

    [<AttachMembers>]
    type PyJsInterop =

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString LDNode.decoder s

        static member toROCrateJsonString(node : LDNode, ?spaces) =
            LDNode.encoder node
            |> Encode.toJsonString (Encode.defaultSpaces spaces)

[<AutoOpen>]
module LDGraphExtensions =

    type LDGraph with

        /// Prefixes relative File and Dataset ids with "./", so that the same path is always represented by the same entity.
        ///
        /// Names equal to the id are changed accordingly. References to changed ids are updated and entities ending up with the same id are merged.
        static member normalizeDataEntityIds (graph : LDGraph) : LDGraph =
            let context = graph.TryGetContext()
            let normalizeId (id : string) =
                if id.StartsWith "./" || id.StartsWith "../" || id.StartsWith "/" || id.StartsWith "#" || id.Contains ":" then id
                else "./" + id
            let newIds = System.Collections.Generic.Dictionary<string,string>()
            for node in graph.Nodes do
                if node.HasType(LDFile.schemaType, ?context = context) || node.HasType(LDDataset.schemaType, ?context = context) then
                    let newId = normalizeId node.Id
                    if newId <> node.Id then newIds.Add(node.Id, newId)
            if newIds.Count = 0 then
                graph
            else
                let rec updateRefs (o : obj) : obj =
                    match o with
                    | :? LDRef as r when newIds.ContainsKey r.Id -> LDRef(newIds.[r.Id])
                    | ActivePattern.NonStringEnumerable e ->
                        // References that point to the same entity after normalization are only kept once
                        let refIds = System.Collections.Generic.HashSet<string>()
                        [for v in e do updateRefs v]
                        |> List.filter (fun v -> match v with | :? LDRef as r -> refIds.Add r.Id | _ -> true)
                        |> ResizeArray
                        |> box
                    | _ -> o
                let normalizedGraph = LDGraph(?id = graph.Id)
                graph.GetProperties(false) |> Seq.iter (fun kv -> normalizedGraph.SetProperty(kv.Key, kv.Value))
                for node in graph.Nodes do
                    let normalizedNode =
                        match newIds.TryGetValue node.Id with
                        | true, newId ->
                            let n = LDNode(newId, node.SchemaType, additionalType = node.AdditionalType)
                            node.GetProperties(false) |> Seq.iter (fun kv -> n.SetProperty(kv.Key, kv.Value))
                            match n.TryGetPropertyAsSingleton(LDFile.name, ?context = context) with
                            | Some (:? string as name) when name = node.Id -> n.SetProperty(LDFile.name, newId, ?context = context)
                            | _ -> ()
                            n
                        | _ -> node
                    normalizedNode.GetProperties(false)
                    |> Seq.toArray
                    |> Array.iter (fun kv -> normalizedNode.SetProperty(kv.Key, updateRefs kv.Value))
                    normalizedGraph.AddNode normalizedNode
                normalizedGraph

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString LDGraph.decoder s
            |> LDGraph.normalizeDataEntityIds

        /// exports in json-ld format
        static member toROCrateJsonString(?spaces) =
            fun (obj:LDGraph) ->
                LDGraph.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?spaces) = 
            LDGraph.toROCrateJsonString(?spaces=spaces) this

    [<AttachMembers>]
    type PyJsInterop =

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString LDGraph.decoder s
            |> LDGraph.normalizeDataEntityIds

        static member toROCrateJsonString(graph : LDGraph, ?spaces) =
            LDGraph.encoder graph
            |> Encode.toJsonString (Encode.defaultSpaces spaces)