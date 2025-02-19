namespace ARCtrl.Json

open ARCtrl
open System
open ARCtrl.ROCrate
open Thoth.Json.Core
open DynamicObj

module LDContext =

    let decoder : Decoder<LDContext> =
        { new Decoder<LDContext> with
            member _.Decode(helpers, value) =     
                if helpers.isObject value then
                    let getters = Decode.Getters(helpers, value)
                    let properties = helpers.getProperties value
                    let builder =
                        fun (get : Decode.IGetters) ->
                            let o = LDContext()
                            for property in properties do
                                if property <> "@id" && property <> "@type" then
                                    o.AddMapping(property,get.Required.Field property Decode.string)
                            o
                    let result = builder getters               
                    match getters.Errors with
                    | [] -> Ok result
                    | fst :: _ as errors ->
                        if errors.Length > 1 then
                            ("", BadOneOf errors) |> Error
                        else
                            Error fst                
                else 
                    ("", BadPrimitive("an object", value)) |> Error
        }

    let encoder (ctx: LDContext) =
        ctx.Mappings
        |> Seq.map (fun kv -> kv.Key, kv.Value |> string |> Encode.string )
        |> Encode.object
