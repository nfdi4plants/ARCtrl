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
                    | :? LDNode -> o
                    | :? DynamicObj as record ->
                        // A record, such as the value of a CWL parameter, holds its references in its fields.
                        // A renamed node shares its value objects with the node it replaces, so the record is copied.
                        let rewritten = DynamicObj()
                        record.GetProperties(false)
                        |> Seq.iter (fun kv -> rewritten.SetProperty(kv.Key, updateRefs kv.Value))
                        rewritten :> obj
                    | ActivePattern.NonStringEnumerable e ->
                        // CWL arrays are ordered and may repeat an item, so a repeated reference stays in the list.
                        // The normalization only merges references whose ids differ and become equal, as "a.txt" and
                        // "./a.txt" do. The first reference to a new id claims it, and a later reference to the same
                        // new id is dropped when its original id is another one.
                        let claimedBy = System.Collections.Generic.Dictionary<string, string>()
                        [for v in e do
                            match v with
                            | :? LDRef as r ->
                                let newId = match newIds.TryGetValue r.Id with | true, newId -> newId | _ -> r.Id
                                match claimedBy.TryGetValue newId with
                                | true, originalId when originalId <> r.Id -> ()
                                | true, _ -> yield updateRefs v
                                | _ ->
                                    claimedBy.Add(newId, r.Id)
                                    yield updateRefs v
                            | _ -> yield updateRefs v]
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