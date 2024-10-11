namespace ARCtrl.Json

open ARCtrl
open System
open ARCtrl.ROCrate
open Thoth.Json.Core
open DynamicObj

module rec ROCrateObject =

    #if !FABLE_COMPILER
    let (|SomeObj|_|) =
        // create generalized option type
        let ty = typedefof<option<_>>
        fun (a:obj) ->
            // Check for nulls otherwise 'a.GetType()' would fail
            if isNull a 
            then 
                None 
            else
                let aty = a.GetType()
                // Get option'.Value
                let v = aty.GetProperty("Value")
                if aty.IsGenericType && aty.GetGenericTypeDefinition() = ty then
                    // return value if existing
                    Some(v.GetValue(a, [| |]))
                else 
                    None
    #endif


    let genericEncoder (obj : obj) : IEncodable =
        match obj with
        | :? string as s -> Encode.string s
        | :? int as i -> Encode.int i
        | :? bool as b -> Encode.bool b
        | :? float as f -> Encode.float f
        | :? DateTime as d -> Encode.dateTime d
        | :? ROCrateObject as o -> encoder o
        #if !FABLE_COMPILER
        | SomeObj o -> genericEncoder o
        #endif
        | null -> Encode.nil
        | :? System.Collections.IEnumerable as l -> [ for x in l -> genericEncoder x] |> Encode.list
        | _ -> failwith "Unknown type"

    let rec encoder(obj: ROCrateObject) =
        obj.GetProperties true
        |> Seq.map (fun kv ->
            kv.Key,
            genericEncoder obj
        )
        |> Encode.object


    let rec decoderWith (constructorByTypeFunc : string -> (string -> string -> string option -> #ROCrateObject)) : Decoder<obj> = 
        let rec decode() = 
            let decodeObject : Decoder<ROCrateObject> =
                { new Decoder<ROCrateObject> with
                    member _.Decode(helpers, value) =     
                        if helpers.isObject value then
                            let getters = Decode.Getters(helpers, value)
                            let properties = helpers.getProperties value
                            let builder =
                                fun (get : Decode.IGetters) ->
                                    let t = get.Required.Field "@type" Decode.string
                                    let id = get.Required.Field "@id" Decode.string
                                    let additionalType = get.Optional.Field "@additionalType" Decode.string
                                    let f = constructorByTypeFunc t
                                    let o = f t id additionalType :> ROCrateObject
                                    for property in properties do
                                        if property <> "@id" && property <> "@type" then
                                            o.SetProperty(property,get.Required.Field property (decode()))
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
            let resizeArray : Decoder<ResizeArray<obj>> =
                { new Decoder<ResizeArray<obj>> with
                    member _.Decode(helpers, value) =
                        if helpers.isArray value then
                            let mutable i = -1
                            let tokens = helpers.asArray value
                            let arr = ResizeArray()

                            (Ok arr, tokens)
                            ||> Array.fold (fun acc value ->
                                i <- i + 1

                                match acc with
                                | Error _ -> acc
                                | Ok acc ->
                                    match decode().Decode(helpers, value) with
                                    | Error er ->
                                        Error(
                                            er
                                            |> Helpers.prependPath (
                                                ".[" + (i.ToString()) + "]"
                                            )
                                        )
                                    | Ok value ->
                                        acc.Add value
                                        Ok acc
                            )
                        else
                            ("", BadPrimitive("an array", value)) |> Error
                }
            Decode.oneOf [
                Decode.map box (decodeObject)
                Decode.map box (resizeArray)
                Decode.map box (Decode.string)
                Decode.map box (Decode.int)
                Decode.map box (Decode.decimal)

            ]
        decode()

    let untypedDecoder = decoderWith (fun _ ->
        fun id t additionalType -> ROCrateObject(id,t,?additionalType = additionalType)      
    )

    let typings : (string*(string->string->string option->#ROCrateObject)) list=
        [
            ROCrate.Assay


        ]
