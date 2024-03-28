module ARCtrl.Helper.HashCodes
    
open Fable.Core

// This is necessary until https://github.com/fable-compiler/Fable/issues/3778 is resolved

#if FABLE_COMPILER_PYTHON
[<Emit("hasattr($0,\"__hash__\")")>]
let pyHasCustomHash (obj) : bool = nativeOnly
        
[<Emit("$0.__hash__()")>]
let pyCustomHash (obj) : int = nativeOnly
#endif

let hash obj =
    #if FABLE_COMPILER_PYTHON
        if pyHasCustomHash obj then
            pyCustomHash obj
        else
            obj.GetHashCode()
    #else
        obj.GetHashCode()
    #endif

let boxHashOption (a: 'a option) : obj =
    if a.IsSome then a.Value.GetHashCode() else (0).GetHashCode()
    |> box

let boxHashArray (a: 'a []) : obj =
    a 
    // from https://stackoverflow.com/a/53507559
    |> Array.fold (fun acc o -> 0x9e3779b9 + (hash o) + (acc <<< 6) + (acc >>> 2) ) 0
    |> box

let boxHashSeq (a: seq<'a>) : obj =
    a 
    // from https://stackoverflow.com/a/53507559
    |> Seq.fold (fun acc o -> 0x9e3779b9 + o.GetHashCode() + (acc <<< 6) + (acc >>> 2) ) 0
    |> box