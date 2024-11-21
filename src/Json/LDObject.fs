namespace ARCtrl.Json

open ARCtrl
open System
open ARCtrl.ROCrate
open Thoth.Json.Core
open DynamicObj

module rec LDObject =
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
        | :? LDObject as o -> encoder o
        #if !FABLE_COMPILER
        | SomeObj o -> genericEncoder o
        #endif
        | null -> Encode.nil
        | :? System.Collections.IEnumerable as l -> [ for x in l -> genericEncoder x] |> Encode.list
        | _ -> failwith "Unknown type"

    let rec encoder(obj: LDObject) =

        [
            yield "@id", Encode.string obj.Id |> Some
            yield "@type", Encode.seq (obj.SchemaType |> Seq.map Encode.string) |> Some
            yield Encode.tryIncludeSeq "additionalType" Encode.string obj.AdditionalType
            for kv in (obj.GetProperties true) do
                let l = kv.Key.ToLower()
                if l <> "id" && l <> "schematype" && l <> "additionaltype" then 
                    yield kv.Key, Some (genericEncoder kv.Value)
        ]
        |> Encode.choose
        |> Encode.object

    /// Returns a decoder
    ///
    /// If expectObject is set to true, decoder fails if top-level value is not an ROCrate object
    let rec getDecoder (expectObject : bool) : Decoder<obj> = 
        let rec decode(expectObject) = 
            let decodeObject : Decoder<LDObject> =
                { new Decoder<LDObject> with
                    member _.Decode(helpers, value) =     
                        if helpers.isObject value then
                            let getters = Decode.Getters(helpers, value)
                            let properties = helpers.getProperties value
                            let builder =
                                fun (get : Decode.IGetters) ->
                                    let t = get.Required.Field "@type" (Decode.resizeArrayOrSingleton Decode.string)
                                    let id = get.Required.Field "@id" Decode.string
                                    let at = get.Optional.Field "additionalType" (Decode.resizeArrayOrSingleton Decode.string)
                                    let o = LDObject(id, t, ?additionalType = at)
                                    for property in properties do
                                        if property <> "@id" && property <> "@type" && property <> "additionalType" then
                                            o.SetProperty(property,get.Required.Field property (decode(false)))
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
                                    match decode(false).Decode(helpers, value) with
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
            if expectObject then
                Decode.map box (decodeObject)
            else
                Decode.oneOf [
                    Decode.map box (decodeObject)
                    Decode.map box (resizeArray)
                    Decode.map box (Decode.string)
                    Decode.map box (Decode.int)
                    Decode.map box (Decode.decimal)

                ]
        decode(expectObject)

    let decoder : Decoder<LDObject> = Decode.map unbox (getDecoder(true))

    let genericDecoder : Decoder<obj> = getDecoder(false)
