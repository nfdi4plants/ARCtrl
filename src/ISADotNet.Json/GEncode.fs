namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

module GEncode = 

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

    let tryGetPropertyValue (name : string) (object : 'T) =
        let property = 
            FSharp.Reflection.FSharpType.GetRecordFields(object.GetType())
            |> Array.tryFind (fun property -> property.Name.Contains name)
        property
        |> Option.bind (fun property -> 
            match FSharp.Reflection.FSharpValue.GetRecordField (object,property) with
            | SomeObj o -> 
                Some o
            | o when isNull o -> 
                None
            | o -> 
                Some o
        )

    let string (value : obj) = 
        match value with
        | :? string as s -> Encode.string s
        | _ -> Encode.nil

    let choose (kvs : (string * JsonValue) list) = 
        kvs
        |> List.choose (fun (k,v) -> 
            if v = Encode.nil then None
            else Some (k,v)
        )

    let tryInclude name (encoder : obj -> JsonValue) (value : obj option) = 
        name,
        match value with
            | Some (:? seq<obj> as os) -> 
                //if Seq.isEmpty os then Encode.nil
                //else 
                Seq.map encoder os |> Encode.seq
            | Some (o) -> encoder o
            | _ -> Encode.nil
