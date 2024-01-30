namespace TestingUtils

open ARCtrl.ISA
open ARCtrl.FileSystem

#if FABLE_COMPILER_PYTHON
open Fable.Pyxpecto
#endif
#if FABLE_COMPILER_JAVASCRIPT
open Fable.Mocha
#endif
#if !FABLE_COMPILER
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

open System
open Fable.Core

[<AttachMembers>]
type Stopwatch() =
    member val StartTime: DateTime option = None with get, set
    member val StopTime: DateTime option = None with get, set
    member this.Start() = this.StartTime <- Some DateTime.Now
    member this.Stop() = 
        match this.StartTime with
        | Some _ -> this.StopTime <- Some DateTime.Now
        | None -> failwith "Error. Unable to call `Stop` before `Start`."
    member this.Elapsed : TimeSpan = 
        match this.StartTime, this.StopTime with
        | Some start, Some stop -> stop - start
        | _, _ -> failwith "Error. Unable to call `Elapsed` without calling `Start` and `Stop` before."

/// Fable compatible Expecto/Mocha/Pyxpecto unification
module Expect = 
    open Utils

    let inline equal actual expected message = Expect.equal actual expected message
    let notEqual actual expected message = Expect.notEqual actual expected message

    let isNull actual message = Expect.isNull actual message 
    let isNotNull actual message = Expect.isNotNull actual message 

    let isSome actual message = Expect.isSome actual message 
    let isNone actual message = Expect.isNone actual message 
    let wantSome actual message = Expect.wantSome actual message 

    let isEmpty actual message = Expect.isEmpty actual message 
    let hasLength actual expectedLength message = Expect.hasLength actual expectedLength message

    let isTrue actual message = Expect.isTrue actual message 
    let isFalse actual message = Expect.isFalse actual message 

    let wantError actual message = Expect.wantError actual message 
    let wantOk actual message = Expect.wantOk actual message 
    let isOk actual message = Expect.isOk actual message 
    let isError actual message = Expect.isError actual message 

    let throws actual message = Expect.throws actual message
    let throwsC actual message = Expect.throwsC actual message 

    let exists actual asserter message = Expect.exists actual asserter message 
    let containsAll actual expected message = Expect.containsAll actual expected message

    let wantFaster (f : unit -> 'T) (maxMilliseconds : int) (message : string) = 
        let stopwatch = Stopwatch()
        stopwatch.Start()
        let res = f()
        stopwatch.Stop()
        let elapsed = stopwatch.Elapsed
        if elapsed.TotalMilliseconds > float maxMilliseconds then
            failwithf $"{message}. Expected to be faster than {maxMilliseconds}ms, but took {elapsed.TotalMilliseconds}ms"
        res

    let isFasterThan (f1 : unit -> _) (f2 : unit -> _) (message : string) =
        let stopwatch = Stopwatch()
        stopwatch.Start()
        f1()
        stopwatch.Stop()
        let elapsed1 = stopwatch.Elapsed
        stopwatch.Start()
        f2()
        stopwatch.Stop()
        let elapsed2 = stopwatch.Elapsed
        if elapsed1.TotalMilliseconds > elapsed2.TotalMilliseconds then
            failwithf $"{message}. Expected {elapsed1.TotalMilliseconds}ms to be faster than {elapsed2.TotalMilliseconds}ms"
        ()

    /// Expects the `actual` sequence to equal the `expected` one.
    let sequenceEqual actual expected message =
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

    let arcTableEqual (t1 : ArcTable) (t2 : ArcTable) (message : string) = 
        let sortVals (dict : System.Collections.Generic.Dictionary<int*int,CompositeCell>) = 
            dict |> Seq.sortBy (fun kv -> kv.Key)
        Expect.equal t1.Name t2.Name (sprintf "%s: Name" message)
        sequenceEqual t1.Headers t2.Headers (sprintf "%s: Headers" message)
        sequenceEqual (t1.Values |> sortVals) (t2.Values |> sortVals) (sprintf "%s: Values" message)

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

/// Fable compatible Expecto/Mocha/Pyxpecto unification
[<AutoOpen>]
module Test =

    let test = test
    let testAsync = testAsync
    let testSequenced = testSequenced

    let testCase = testCase
    let ptestCase = ptestCase
    let ftestCase = ftestCase
    let testCaseAsync = testCaseAsync
    let ptestCaseAsync = ptestCaseAsync
    let ftestCaseAsync = ftestCaseAsync


    let testList = testList


