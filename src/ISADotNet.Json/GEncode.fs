namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

open Fable.Core

module GEncode = 

    
    [<Emit("$0[$1]")>]
    let getFieldFable (name : string) (object : 'T) = jsNative

    let inline tryGetPropertyValue (name : string) (object : 'T) =
        #if FABLE_COMPILER
            getFieldFable name object
        #else
        let property = 
            FSharp.Reflection.FSharpType.GetRecordFields(object.GetType())
            |> Array.tryFind (fun property -> property.Name.Contains name)
        property
        |> Option.bind (fun property -> 
            match FSharp.Reflection.FSharpValue.GetRecordField (object,property) with
            | ISADotNet.API.Update.SomeObj o -> 
                Some o
            | o when isNull o -> 
                None
            | o -> 
                Some o
        )
        #endif

    let inline string (value : obj) = 
        match value with
        | :? string as s -> Encode.string s
        | _ -> Encode.nil

    let inline choose (kvs : (string * JsonValue) list) = 
        kvs
        |> List.choose (fun (k,v) -> 
            if v = Encode.nil then None
            else Some (k,v)
        )

    let inline tryInclude name (encoder : obj -> JsonValue) (value : obj option) = 
        name,
        match value with
            | Some (:? seq<obj> as os) -> 
                //if Seq.isEmpty os then Encode.nil
                //else 
                Seq.map encoder os |> Encode.seq
            | Some (o) -> encoder o
            | _ -> Encode.nil
