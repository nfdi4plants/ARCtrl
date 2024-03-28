namespace ARCtrl.Json

open Thoth.Json.Core

open Fable.Core

#if FABLE_COMPILER_PYTHON
open Fable.Core.PyInterop
#endif
#if FABLE_COMPILER_JAVASCRIPT
open Fable.Core.JsInterop
#endif

[<RequireQualifiedAccess>]
module Encode = 

    let inline toJsonString spaces (value : Json) = 
        #if FABLE_COMPILER_PYTHON
        Thoth.Json.Python.Encode.toString spaces value
        #endif
        #if FABLE_COMPILER_JAVASCRIPT
        Thoth.Json.JavaScript.Encode.toString spaces value
        #endif
        #if !FABLE_COMPILER
        Thoth.Json.Newtonsoft.Encode.toString spaces value
        #endif

    let inline choose (kvs : (string * Json) list) = 
        kvs
        |> List.choose (fun (k,v) -> 
            if v = Encode.nil then None
            else Some (k,v)
        )       

    /// Try to encode the given object using the given encoder, or return Encode.nil if the object is null
    let tryInclude (name : string) (encoder : 'Value -> Json) (value : 'Value option) = 
        name,
        match value with
        | Some(o) -> encoder o
        | _ -> Encode.nil

    /// Try to encode the given object using the given encoder, or return Encode.nil if the object is null
    let tryIncludeSeq name (encoder : 'Value -> Json) (value : #seq<'Value>) = 
        name,
        if Seq.isEmpty value then Encode.nil
        else value |> Seq.map encoder |> Encode.seq

    let tryIncludeArray name (encoder : 'Value -> Json) (value : 'Value array) = 
        name,
        if Array.isEmpty value then Encode.nil
        else value |> Array.map encoder |> Encode.array

    let tryIncludeList name (encoder : 'Value -> Json) (value : 'Value list) = 
        name,
        if List.isEmpty value then Encode.nil
        else value |> List.map encoder |> Encode.list

    let tryIncludeListOpt name (encoder : 'Value -> Json) (value : 'Value list option) = 
        name,
        match value with
        | Some(o) -> 
            if List.isEmpty o then Encode.nil
            else o |> List.map encoder |> Encode.list
        | _ -> 
            Encode.nil
        
    let DefaultSpaces = 0

    let defaultSpaces spaces = defaultArg spaces DefaultSpaces