namespace ARCtrl.Json

open ARCtrl
open System
open ARCtrl.ROCrate
open Thoth.Json.Core
open DynamicObj


module rec LDGraph =   

    let encoder(obj: LDGraph) =

        [
            Encode.tryInclude "@id" Encode.string obj.Id
            Encode.tryInclude "@context" LDContext.encoder (obj.TryGetContext())
            for kv in (obj.GetProperties true) do
                let l = kv.Key.ToLower()
                if l <> "id" && l <> "@context" && l <> "nodes" then 
                    kv.Key, Some (LDNode.genericEncoder kv.Value)
            "@graph", obj.Nodes |> Seq.map LDNode.encoder |> Encode.seq |> Some
        ]
        |> Encode.choose
        |> Encode.object


    let decoder : Decoder<LDGraph> = 
        { new Decoder<LDGraph> with
            member _.Decode(helpers, value) =
                if helpers.isObject value then
                    let getters = Decode.Getters(helpers, value)
                    let properties = helpers.getProperties value
                    let builder =
                        fun (get : Decode.IGetters) ->
                            let id = get.Optional.Field "@id" Decode.string
                            let context = get.Optional.Field "@context" LDContext.decoder
                            let nodes = get.Required.Field "@graph" (Decode.seq LDNode.decoder)            
                            let o = LDGraph(?id = id, ?context = context)
                            for property in properties do
                                if property <> "@id" && property <> "@graph" && property <> "@context" then
                                    o.SetProperty(property,get.Required.Field property LDNode.genericDecoder)
                            for node in nodes do
                                o.AddNode node
                            o
                    let result = builder getters               
                    match getters.Errors with
                        | [] -> Ok result
                        | fst :: _ as errors ->
                            if errors.Length > 1 then
                                ("", BadOneOf errors) |> Error
                            else
                                Error fst
                else ("", BadPrimitive("an object", value)) |> Error

        }

