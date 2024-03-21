namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open Fable.Core

module internal Helpers =

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

    let helpers = 
        #if FABLE_COMPILER_PYTHON
        Thoth.Json.Python.Decode.helpers
        #endif
        #if FABLE_COMPILER_JAVASCRIPT
        Thoth.Json.JavaScript.Decode.helpers
        #endif
        #if !FABLE_COMPILER
        Thoth.Json.Newtonsoft.Decode.helpers
        #endif


    let isURI (s : string) = 
        true
        //s.StartsWith("http://") || s.StartsWith("https://")
    
    let uri =
         { new Decoder<URI> with
             member this.Decode(s,json) = 
                    match Decode.string.Decode(s,json) with
                    | Ok s when isURI s -> Ok s
                    | Ok s -> Error (DecoderError(s,ErrorReason.FailMessage (sprintf "Expected URI, got %s" s)))
                    | Error e -> Error e
        }

    let inline fromJsonString (decoder : Decoder<'a>) (s : string) : 'a = 
        #if FABLE_COMPILER_PYTHON
        match Thoth.Json.Python.Decode.fromString decoder s with
        #endif
        #if FABLE_COMPILER_JAVASCRIPT
        match Thoth.Json.JavaScript.Decode.fromString decoder s with
        #endif
        #if !FABLE_COMPILER
        match Thoth.Json.Newtonsoft.Decode.fromString decoder s with
        #endif
        | Ok a -> a
        | Error e -> failwith (sprintf "Error decoding string: %O" e)
    

    let hasUnknownFields (helpers : IDecoderHelpers<'JsonValue>) (knownFields : Set<string>) (json : 'JsonValue) = 
        helpers.getProperties json
        |> Seq.exists (fun x -> not (knownFields |> Set.contains x))

    let object (allowedFields : string seq) (builder: Decode.IGetters -> 'value) : Decoder<'value> =
        let allowedFields = Set.ofSeq allowedFields
        { new Decoder<'value> with
            member _.Decode(helpers, value) =
                let getters = Decode.Getters(helpers, value)
                if hasUnknownFields helpers allowedFields value then
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

    let resizeArray (decoder: Decoder<'value>) : Decoder<ResizeArray<'value>> =
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
                    ("", BadPrimitive("an array", value)) |> Error
        }