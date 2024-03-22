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
    
    #if FABLE_COMPILER_JAVASCRIPT
    [<Emit("$1[$0]")>]
    let getFieldFable (name : string) (object : 'T) = jsNative
    #endif

    /// Try to get a property value from a record by its name 
    let inline tryGetPropertyValue (name : string) (object : 'T) =
        #if FABLE_COMPILER_JAVASCRIPT
        getFieldFable name object
        #else
        let property = 
            FSharp.Reflection.FSharpType.GetRecordFields(object.GetType())
            |> Array.tryFind (fun property -> property.Name.Contains name)
        property
        |> Option.bind (fun property -> 
            match FSharp.Reflection.FSharpValue.GetRecordField (object,property) with
            | ARCtrl.Helper.Update.SomeObj o -> 
                Some o
            | o when isNull o -> 
                None
            | o -> 
                Some o
        )
        #endif

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
    ///
    /// If the object is a sequence, encode each element using the given encoder and return the resulting sequence
    let tryIncludeObj name (encoder : obj -> Json) (value : obj option) = 
        name,
        match value with
        #if FABLE_COMPILER_JAVASCRIPT
        | Some (:? System.Collections.IEnumerable as v) ->                  
            !!Seq.map encoder v |> Encode.seq
        #else
        | Some(:? seq<obj> as os) ->                 
            Seq.map encoder os |> Encode.seq
        #endif
        | Some(o) -> encoder o
        | _ -> Encode.nil

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

    let tryIncludeList name (encoder : 'Value -> Json) (value : 'Value list option) = 
        name,
        if value.IsNone then Encode.nil
        else value.Value |> List.map encoder |> Encode.list

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