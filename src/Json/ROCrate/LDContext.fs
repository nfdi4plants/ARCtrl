namespace ARCtrl.Json

open ARCtrl
open System
open ARCtrl.ROCrate
open Thoth.Json.Core
open DynamicObj

module LDContext =

    /// The IRI of a term, given as a string or as the @id of an expanded term definition.
    /// Any other entry of a context maps no term to an IRI, as the keyword @version does, and gives None.
    let private termDefinitionDecoder : Decoder<string option> =
        Decode.oneOf [
            Decode.map Some Decode.string
            Decode.map Some (Decode.field "@id" Decode.string)
            Decode.succeed None
        ]

    let decoder : Decoder<LDContext> =
        { new Decoder<LDContext> with
            member this.Decode(helpers, value) =     
                if helpers.isObject value then
                    let getters = Decode.Getters(helpers, value)
                    let properties = helpers.getProperties value
                    let builder =
                        fun (get : Decode.IGetters) ->
                            let o = LDContext()
                            for property in properties do
                                if property <> "@id" && property <> "@type" then
                                    match get.Required.Field property termDefinitionDecoder with
                                    | Some definition -> o.AddMapping(property,definition)
                                    | None -> ()
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
                    if s = Context.proxy_V1_3 then
                        Ok (Context.initV1_3())
                    elif s = Context.proxy_V1_2DRAFT || s = Context.proxy_V1_2 then
                        Ok (Context.initV1_2())
                    elif s = Context.proxy_V1_1 then
                        Ok (Context.initV1_1())
                    elif s = Context.proxy_ARC then
                        Ok (Context.initArcContext())
                    else
                        // The terms of a context that is not known cannot be resolved. The context keeps the URL as
                        // its name, so that it is written as it was read.
                        let unknownContext = LDContext()
                        unknownContext.Name <- Some s
                        Ok unknownContext
                elif helpers.isArray value then
                    match Decode.resizeArray(this).Decode(helpers,value) with
                    | Ok baseContexts -> Ok (LDContext(baseContexts = baseContexts))
                    | Error e -> Error e
                else 
                    ("", BadPrimitive("an object", value)) |> Error
        }

    let rec encoder (ctx: LDContext) =
        match ctx.Name with
        | Some Context.proxy_V1_3 -> Encode.string Context.proxy_V1_3
        | Some Context.proxy_V1_2DRAFT -> Encode.string Context.proxy_V1_2DRAFT
        | Some Context.proxy_V1_2 -> Encode.string Context.proxy_V1_2
        | Some Context.proxy_V1_1 -> Encode.string Context.proxy_V1_1
        | Some Context.proxy_ARC -> Encode.string Context.proxy_ARC
        // A context that was read from a URL that is not known
        | Some name when ctx.Mappings.Count = 0 && ctx.BaseContexts.Count = 0 -> Encode.string name
        | _ ->
            let mappings = 
                ctx.Mappings
                |> Seq.map (fun kv -> kv.Key, kv.Value |> string |> Encode.string )
                |> Encode.object
            if ctx.BaseContexts.Count = 0 then
                mappings
            elif ctx.BaseContexts.Count = 1 && ctx.Mappings.Count = 0 then
                ctx.BaseContexts.[0] |> encoder
            else
                ctx.BaseContexts |> Seq.map encoder
                |> Seq.append [ if ctx.Mappings.Count <> 0 then mappings ]
                |> Encode.seq