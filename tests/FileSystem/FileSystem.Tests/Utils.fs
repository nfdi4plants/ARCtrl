module TestingUtils

module Result =

    let getMessage (r : Result<'T,string>) =
        match r with
        | Ok _ -> ""
        | Error msg -> msg

let private firstDiff s1 s2 =
  let s1 = Seq.append (Seq.map Some s1) (Seq.initInfinite (fun _ -> None))
  let s2 = Seq.append (Seq.map Some s2) (Seq.initInfinite (fun _ -> None))
  Seq.mapi2 (fun i s p -> i,s,p) s1 s2
  |> Seq.find (function |_,Some s,Some p when s=p -> false |_-> true)

/// Expects the `actual` sequence to equal the `expected` one.
let mySequenceEqual actual expected message =
    match firstDiff actual expected with
    | _,None,None -> ()
    | i,Some a, Some e ->
        failwithf "%s. Sequence does not match at position %i. Expected item: %A, but got %A." message i e a
    | i,None,Some e ->
        failwithf "%s. Sequence actual shorter than expected, at pos %i for expected item %A." message i e
    | i,Some a,None ->
        failwithf "%s. Sequence actual longer than expected, at pos %i found item %A." message i a

open FileSystem

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

let rec testFileSystemTree (actual:FileSystemTree) (expected:FileSystemTree) = 
    match actual, expected with
    | File n1, File n2 -> Expect.equal n1 n2 $"Expects file names to be equal: {n1} = {n2}"
    | Folder (n1, children1), Folder (n2, children2) -> 
        Expect.equal n1 n2 $"Expects folder names to be equal: {n1} = {n2}"
        let sortByName (children: FileSystemTree []) = children |> Array.sortBy (fun c -> c.Name)
        Array.iter2 testFileSystemTree (sortByName children1) (sortByName children2)
    | anyActual, anyExpected ->
        failwith $"Testing FileSystemTree found an issue with unequal states: 
Actual:        
{anyActual}

Expected:
{anyExpected}"