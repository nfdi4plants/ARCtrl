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
                elif helpers.isString value then
                    let s = helpers.asString value
                    if s = Context.proxy_V1_2DRAFT then
                        Ok (Context.initV1_2DRAFT())
                    elif s = Context.proxy_V1_1 then
                        Ok (Context.initV1_1())
                    else
                        ("", BadPrimitive("an object", value)) |> Error                     
                else 
                    ("", BadPrimitive("an object", value)) |> Error
        }

    let encoder (ctx: LDContext) =
        match ctx.Name with
        | Some Context.proxy_V1_2DRAFT -> Encode.string Context.proxy_V1_2DRAFT
        | Some Context.proxy_V1_1 -> Encode.string Context.proxy_V1_1
        | _ ->
            ctx.Mappings
            |> Seq.map (fun kv -> kv.Key, kv.Value |> string |> Encode.string )
            |> Encode.object
