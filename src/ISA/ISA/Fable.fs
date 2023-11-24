[<RequireQualifiedAccess>]
module ARCtrl.ISA.Fable

open Fable.Core

#if FABLE_COMPILER_PYTHON
open Fable.Core.PyInterop
#else
open Fable.Core.JsInterop
#endif

[<Emit("console.log($0)")>]
let print (msg:obj) : unit = 
    printfn "%O" msg

let isMap_generic l1 = l1.ToString().StartsWith("map [")

let isList_generic l1 = 
    let s = l1.ToString()
    printfn "%A" (s)
    s.StartsWith("[") && (s.StartsWith "seq [" |> not)

let append_generic l1 l2 =
    // This isNull check is necessary because in the API.update functionality we only check the type of l1. 
    // There l1 can be a sequence (Some sequence in dotnet) and l2 would be an undefined (None in dotnet).
    // We need to check if l2 is null and if so return l1.
    if !!isNull l2 then l1

    else
        if isList_generic l1 then 
            !!List.append l1 l2
        else
            !!Array.append l1 l2

let distinct_generic l1 =       
    if isList_generic l1 then 
        !!List.distinct l1
    else
        !!Array.distinct l1

let hashSeq (s:seq<'a>) = 
    s
    |> Seq.map (fun x -> x.GetHashCode())
    |> Seq.reduce (fun a b -> a + b)