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

    let inline choose (kvs : (string * IEncodable option) list) = 
        kvs
        |> List.choose (fun (k,v) ->
            v
            |> Option.map (fun v -> k,v)
        )       

    /// Try to encode the given object using the given encoder, or return Encode.nil if the object is null
    let tryInclude (name : string) (encoder : 'Value -> IEncodable) (value : 'Value option) = 
        name,
        value
        |> Option.map encoder

    /// Try to encode the given object using the given encoder, or return Encode.nil if the object is null
    let tryIncludeSeq name (encoder : 'Value -> IEncodable) (value : #seq<'Value>) = 
        name,
        if Seq.isEmpty value then None 
        else value |> Seq.map encoder |> Encode.seq |> Some

    let tryIncludeArray name (encoder : 'Value -> IEncodable) (value : 'Value array) = 
        name,
        if Array.isEmpty value then None
        else value |> Array.map encoder |> Encode.array |> Some

    let tryIncludeList name (encoder : 'Value -> IEncodable) (value : 'Value list) = 
        name,
        if List.isEmpty value then None
        else value |> List.map encoder |> Encode.list |> Some

    let tryIncludeListOpt name (encoder : 'Value -> IEncodable) (value : 'Value list option) = 
        name,
        match value with
        | Some(o) -> 
            if List.isEmpty o then None
            else o |> List.map encoder |> Encode.list |> Some
        | _ -> 
            None
        
    let DefaultSpaces = 0

    let defaultSpaces spaces = defaultArg spaces DefaultSpaces

    let dateTime(d : System.DateTime) =         
        d.ToString("O", System.Globalization.CultureInfo.InvariantCulture).Split('+').[0]
        |> Encode.string

    let addPropertyToObject (name : string) (value : Json) (obj : Json) = 
        match obj with
        | Json.Object kvs -> Json.Object (Seq.append kvs [name, value] )
        | _ -> failwith "Expected object"

    let resizeArrayOrSingleton (encoder : 'T -> IEncodable) (values: ResizeArray<'T>) =
        if values.Count = 1 then
            values.[0] |> encoder
        else
            values
            |> Seq.map encoder
            |> Encode.seq