namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open Fable.Core
open System.Collections.Generic

#if FABLE_COMPILER_PYTHON
module PyTime = 

    open Fable.Core
    open Fable.Core.PyInterop

    // Currently in Fable, a created datetime object will contain a timezone. This is not allowed in python xlsx, so we need to remove it.
    // Unfortunately, the timezone object in python is read-only, so we need to create a new datetime object without timezone.
    // For this, we use the fromtimestamp method of the datetime module and convert the timestamp to a new datetime object without timezone.

    type DateTimeStatic =
        [<Emit("$0.fromtimestamp(timestamp=$1)")>]
        abstract member fromTimeStamp: timestamp:float -> System.DateTime

    [<Import("datetime", "datetime")>]
    let DateTime : DateTimeStatic = nativeOnly

    let toUniversalTimePy (dt:System.DateTime) = 
        
        dt.ToUniversalTime()?timestamp()
        |> DateTime.fromTimeStamp
#endif
    
module Helpers =

    let prependPath
        (path: string)
        (err: DecoderError<'JsonValue>)
        : DecoderError<'JsonValue>
        =
        let (oldPath, reason) = err
        (path + oldPath, reason)

    let inline prependPathToResult<'T, 'JsonValue>
        (path: string)
        (res: Result<'T, DecoderError<'JsonValue>>)
        =
        res |> Result.mapError (prependPath path)

module Decode =

    let isURI (s : string) = 
        true
        //s.StartsWith("http://") || s.StartsWith("https://")
    
    let uri : Decoder<URI> =
        { new Decoder<URI> with
             member this.Decode(s,json) = 
                    match Decode.string.Decode(s,json) with
                    | Ok s when isURI s -> Ok s
                    | Ok s -> Error (DecoderError(s,ErrorReason.FailMessage (sprintf "Expected URI, got %s" s)))
                    | Error e -> Error e
        }

    let hasUnknownFields (helpers : IDecoderHelpers<'JsonValue>) (knownFields : Set<string>) (json : 'JsonValue) = 
        helpers.getProperties json
        |> Seq.exists (fun x -> not (knownFields |> Set.contains x))

    let objectNoAdditionalProperties (allowedProperties : string seq) (builder: Decode.IGetters -> 'value) : Decoder<'value> =
        let allowedProperties = Set.ofSeq allowedProperties
        { new Decoder<'value> with
            member _.Decode(helpers, value) =
                let getters = Decode.Getters(helpers, value)
                if hasUnknownFields helpers allowedProperties value then
                    Error (DecoderError("Unknown fields in object", ErrorReason.BadPrimitive("",value)))
                else
                    let result = builder getters
                
                    match getters.Errors with
                    | [] -> Ok result
                    | fst :: _ as errors ->
                        if errors.Length > 1 then
                            ("", BadOneOf errors) |> Error
                        else
                            Error fst
        }

    let noAdditionalProperties (allowedProperties : string seq) (decoder : Decoder<'value>) : Decoder<'value> =
        let allowedProperties = Set.ofSeq allowedProperties
        { new Decoder<'value> with
            member _.Decode(helpers, value) =
                let getters = Decode.Getters(helpers, value)
                if hasUnknownFields helpers allowedProperties value then
                    Error (DecoderError("Unknown fields in object", ErrorReason.BadPrimitive("",value)))
                else
                    decoder.Decode(helpers,value)
        }

    let resizeArrayOrSingleton (decoder: Decoder<'value>) : Decoder<ResizeArray<'value>> =
        { new Decoder<ResizeArray<'value>> with
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
                            match decoder.Decode(helpers, value) with
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
                    decoder.Decode(helpers, value) |> Result.map (fun x -> ResizeArray[x])
        }

    let datetime: Decoder<System.DateTime> =
        #if FABLE_COMPILER_PYTHON
        Decode.datetimeLocal |> Decode.map PyTime.toUniversalTimePy
        #else
        { new Decoder<System.DateTime> with
            member _.Decode(helpers, value) =
                if helpers.isString value then
                    match System.DateTime.TryParse(helpers.asString value) with
                    | true, datetime -> datetime |> Ok
                    | _ -> ("", BadPrimitive("a datetime", value)) |> Error
                else
                    ("", BadPrimitive("a datetime", value)) |> Error
        }
        //Decode.datetimeUtc
        #endif


    let dictionary (keyDecoder : Decoder<'key>) (valueDecoder : Decoder<'value>) : Decoder<Dictionary<'key,'value>> =
        { new Decoder<Dictionary<'key,'value>> with
            member _.Decode(helpers, value) =
                if helpers.isArray value then
                    let mutable errors = []
                    let tokens = helpers.asArray value
                    let dict = Dictionary<'key,'value>()
                    let decoder = Decode.tuple2 keyDecoder valueDecoder

                    (Ok dict, tokens)
                    ||> Array.fold (fun acc value ->
                        match acc with
                        | Error _ -> acc
                        | Ok acc ->
                            match decoder.Decode(helpers, value) with
                            | Error er ->
                                Error(
                                    er                                    
                                )
                            | Ok value ->
                                acc.Add value
                                Ok acc
                    )
                    
                else
                    ("", BadPrimitive("an object", value)) |> Error
        }

    let intDictionary (valueDecoder : Decoder<'value>) : Decoder<Dictionary<int,'value>> =
        { new Decoder<Dictionary<int,'value>> with
            member _.Decode(helpers, value) =
                if helpers.isArray value then
                    let mutable errors = []
                    let tokens = helpers.asArray value
                    let dict = Dictionary<int,'value>()
                    let decoder = Decode.tuple2 Decode.int valueDecoder

                    (Ok dict, tokens)
                    ||> Array.fold (fun acc value ->
                        match acc with
                        | Error _ -> acc
                        | Ok acc ->
                            match decoder.Decode(helpers, value) with
                            | Error er ->
                                Error(
                                    er                                    
                                )
                            | Ok value ->
                                acc.Add value
                                Ok acc
                    )
                    
                else
                    ("", BadPrimitive("an object", value)) |> Error
        }