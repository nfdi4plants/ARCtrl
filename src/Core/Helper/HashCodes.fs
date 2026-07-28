module ARCtrl.Helper.HashCodes

let mergeHashes (hash1 : int) (hash2 : int) : int =
    0x9e3779b9 + hash2 + (hash1 <<< 6) + (hash1 >>> 2)

let hashDateTime (dt : System.DateTime) : int =
    let mutable acc = 0
    acc <- mergeHashes acc dt.Year
    acc <- mergeHashes acc dt.Month
    acc <- mergeHashes acc dt.Day
    acc <- mergeHashes acc dt.Hour
    acc <- mergeHashes acc dt.Minute
    acc <- mergeHashes acc dt.Second
    acc
    

/// djb2 string hash with explicit 32-bit wrapping.
///
/// Fable's Python runtime accumulates `string_hash` in an unbounded Python integer and only narrows
/// to int32 at the very end, where the conversion saturates instead of wrapping. Every string of
/// roughly ten characters or more therefore collapses to Int32.MaxValue, so distinct values collide.
/// Plain F# `int` arithmetic wraps on every target, so this reproduces the intended djb2 values
/// identically on .NET, JavaScript and Python.
let hashString (s: string) : int =
    let mutable h = 5381
    for c in s do
        h <- (h * 33) ^^^ int c
    h

let hash (o: 'a) : int =
    match box o with
    | :? string as s -> hashString s
    | _ -> o.GetHashCode()

let boxHashOption (a: 'a option) : obj =
    if a.IsSome then hash a.Value else (0).GetHashCode()
    |> box

let boxHashArray (a: 'a []) : obj =
    a 
    // from https://stackoverflow.com/a/53507559
    |> Array.fold (fun acc o -> 
        hash o
        |> mergeHashes acc) 0
    |> box

let boxHashSeq (a: seq<'a>) : obj =
    a 
    // from https://stackoverflow.com/a/53507559
    |> Seq.fold (fun acc o -> 
        hash o
        |> mergeHashes acc) 0
    |> box