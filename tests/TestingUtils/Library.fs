namespace TestingUtils

open FsSpreadsheet
open ARCtrl.ISA
open ARCtrl.FileSystem

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

module Async = 

    let map (f : 'U -> 'T) (a : Async<'U>) =
        async {
            let! a' =  a
            return f a'           
        }

module Utils = 

    let extractWords (json:string) = 
        json.Split([|'{';'}';'[';']';',';':'|])
        |> Array.map (fun s -> s.Trim())
        |> Array.filter ((<>) "")

    let wordFrequency (json:string) = 
        json
        |> extractWords
        |> Array.countBy id
        |> Array.sortBy fst

    let firstDiff s1 s2 =
      let s1 = Seq.append (Seq.map Some s1) (Seq.initInfinite (fun _ -> None))
      let s2 = Seq.append (Seq.map Some s2) (Seq.initInfinite (fun _ -> None))
      Seq.mapi2 (fun i s p -> i,s,p) s1 s2
      |> Seq.find (function |_,Some s,Some p when s=p -> false |_-> true)


module Result =

    let getMessage res =
        match res with
        | Ok m -> m
        | Error m -> m

module Expect = 
    open Utils
    /// Expects the `actual` sequence to equal the `expected` one.
    let inline mySequenceEqual actual expected message =
      match firstDiff actual expected with
      | _,None,None -> ()
      | i,Some a, Some e ->
        failwithf "%s. Sequence does not match at position %i. Expected item: %A, but got %A."
          message i e a
      | i,None,Some e ->
        failwithf "%s. Sequence actual shorter than expected, at pos %i for expected item %A."
          message i e
      | i,Some a,None ->
        failwithf "%s. Sequence actual longer than expected, at pos %i found item %A."
          message i a

    
    let workSheetEqual (actual : FsWorksheet) (expected : FsWorksheet) message =
        let f (ws : FsWorksheet) = 
            ws.RescanRows()
            ws.Rows
            |> Seq.map (fun r -> r.Cells |> Seq.map (fun c -> c.Value) |> Seq.reduce (fun a b -> a + b)) 
        if actual.Name <> expected.Name then
            failwithf $"{message}. Worksheet names do not match. Expected {expected.Name} but got {actual.Name}"
        mySequenceEqual (f actual) (f expected) $"{message}. Worksheet does not match"

    let columnsEqual (actual : FsCell seq seq) (expected : FsCell seq seq) message =     
        let f (cols : FsCell seq seq) = 
            cols
            |> Seq.map (fun r -> r |> Seq.map (fun c -> c.Value) |> Seq.reduce (fun a b -> a + b)) 
        mySequenceEqual (f actual) (f expected) $"{message}. Columns do not match"

    let arcTableEqual (t1 : ArcTable) (t2 : ArcTable) (message : string) = 
        let sortVals (dict : System.Collections.Generic.Dictionary<int*int,CompositeCell>) = 
            dict |> Seq.sortBy (fun kv -> kv.Key)
        Expect.equal t1.Name t2.Name (sprintf "%s: Name" message)
        mySequenceEqual t1.Headers t2.Headers (sprintf "%s: Headers" message)
        mySequenceEqual (t1.Values |> sortVals) (t2.Values |> sortVals) (sprintf "%s: Values" message)

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