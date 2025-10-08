namespace TestingUtils

open FsSpreadsheet
open ARCtrl
open ARCtrl.FileSystem

open Fable.Pyxpecto


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

    let trim (path : string) =
        if path.StartsWith("./") then
            path.Replace("./","").Trim('/')
        else path.Trim('/')

module Result =

    let getMessage res =
        match res with
        | Ok m -> m
        | Error m -> m

open System
open Fable.Core

module Fable =
    
    module JS =

        [<Emit("process.stdout.write($0)")>]
        let print (s:string) : unit = nativeOnly

    module Py =

        [<Emit("print($0, end = \"\")")>]
        let print (s:string) : unit = nativeOnly

    let fprint(s: string) =
        #if FABLE_COMPILER_JAVASCRIPT
        JS.print(s)
        #endif
        #if FABLE_COMPILER_PYTHON
        Py.print(s)
        #endif
        #if !FABLE_COMPILER
        printf "%s" s
        #endif

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

    static member measure f = 
        let stopwatch = Stopwatch()
        stopwatch.Start()
        let res = f()
        stopwatch.Stop()
        stopwatch.Elapsed.TotalMilliseconds

/// Fable compatible Expecto/Mocha/Pyxpecto unification
module Expect = 
    open Utils

    let inline equal actual expected message = Expect.equal actual expected message
    let notEqual actual expected message = Expect.notEqual actual expected message


    /// Trims whitespace and normalizes lineendings to "\n"
    let trimEqual (actual: string) (expected: string) message =
        let a = actual.Trim().Replace("\r\n", "\n")
        let e = expected.Trim().Replace("\r\n", "\n")
        Expect.equal a e message

    /// <summary>
    /// This function only verifies non-whitespace characters
    /// </summary>
    let stringEqual actual expected message =
        let pattern = @"\s+"
        let regex = System.Text.RegularExpressions.Regex(pattern, Text.RegularExpressions.RegexOptions.Singleline)
        let actual = regex.Replace(actual, "")
        let expected = regex.Replace(expected, "")
        let mutable isSame = true
        Seq.iter2 
            (fun s1 s2 -> 
                if isSame && s1 = s2 then 
                    ()
                elif isSame && s1 <> s2 then
                    isSame <- false
                    Fable.fprint (sprintf "%s" (string s1))
                else
                    Fable.fprint (sprintf "%s" (string s1))
            ) 
            actual 
            expected
        equal actual expected message

    let isNull actual message = Expect.isNull actual message 
    let isNotNull actual message = Expect.isNotNull actual message 

    let isSome actual message = Expect.isSome actual message 
    let isNone actual message = Expect.isNone actual message 
    let wantSome actual message = Expect.wantSome actual message 

    let isEmpty actual message = Expect.isEmpty actual message 
    let isNotEmpty actual message = Expect.isNotEmpty actual message 
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

    let dateTimeEqual (actual : DateTime) (expected : DateTime) message = 
        let actual = actual.ToString("yyyy-MM-dd HH:mm:ss")
        let expected = expected.ToString("yyyy-MM-dd HH:mm:ss")
        equal actual expected message

    /// Expects the `actual` sequence to equal the `expected` one.
    let sequenceEqual actual expected message =
      match firstDiff actual expected with
      | _,None,None -> ()
      | i,Some a, Some e ->
        failwithf "%s. Sequence does not match at position %i. Expected item: %O, but got %O."
          message i e a
      | i,None,Some e ->
        failwithf "%s. Sequence actual shorter than expected, at pos %i for expected item %O."
          message i e
      | i,Some a,None ->
        failwithf "%s. Sequence actual longer than expected, at pos %i found item %O."
          message i a

    let genericSequenceEqual (actual : System.Collections.IEnumerable) (expected : System.Collections.IEnumerable) message = 
        let actual = [for v in actual do v]
        let expected = [for v in expected do v]
        sequenceEqual actual expected message

    let pathSequenceEqual actual expected message = 
        let actual = actual |> Seq.map trim |> Seq.sort
        let expected = expected |> Seq.map trim |> Seq.sort
        sequenceEqual actual expected message

    let workSheetEqual (actual : FsWorksheet) (expected : FsWorksheet) message =
        let f (ws : FsWorksheet) = 
            ws.RescanRows()
            ws.Rows
            |> Seq.map (fun r -> r.Cells |> Seq.map (fun c -> c.ValueAsString()) |> Seq.reduce (fun a b -> a + b)) 
        if actual.Name <> expected.Name then
            failwithf $"{message}. Worksheet names do not match. Expected {expected.Name} but got {actual.Name}"
        sequenceEqual (f actual) (f expected) $"{message}. Worksheet does not match"

    let workBookEqual (actual : FsWorkbook) (expected : FsWorkbook) message = 
        Seq.iter2 (fun a e -> workSheetEqual a e message) (actual.GetWorksheets()) (expected.GetWorksheets())


    let columnsEqual (actual : FsCell seq seq) (expected : FsCell seq seq) message =     
        let f (cols : FsCell seq seq) = 
            cols
            |> Seq.map (fun r -> r |> Seq.map (fun c -> c.ValueAsString()) |> Seq.reduce (fun a b -> a + b)) 
        sequenceEqual (f actual) (f expected) $"{message}. Columns do not match"

    let arcTableEqual (t1 : ArcTable) (t2 : ArcTable) (message : string) = 
        let sortVals (dict : ArcTableAux.ArcTableValues) = 
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
            Expect.equal children1.Length children2.Length $"Expects folder names to be equal in length: {n1}:{children1.Length} = {n2}:{children2.Length}"
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

    open CrossAsync

    let test = test
    let testAsync = testAsync
    let testSequenced = testSequenced

    let testCase = testCase
    let ptestCase = ptestCase
    let ftestCase = ftestCase


    let testCaseCrossAsync (text : string) (ca : CrossAsync<unit>) =
        ca
        |> catchWith (fun exn -> failwithf "%s" exn.Message)
        |> asAsync
        |> testCaseAsync text
        
    let ptestCaseCrossAsync (text : string) (ca : CrossAsync<unit>) = 
        ptestCaseAsync text (asAsync ca)
    let ftestCaseCrossAsync (text : string) (ca : CrossAsync<unit>) =
        ftestCaseAsync text (asAsync ca)

    let testCaseAsync = testCaseAsync
    let ptestCaseAsync = ptestCaseAsync
    let ftestCaseAsync = ftestCaseAsync

    let testList = testList
    let ftestList = ftestList


#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
module TSUtils = 

    open Fable.Pyxpecto
    open Fable.Core
    open Fable.Core.JS


    [<Import("it", from = "vitest")>]
    let it(name: string, test: unit -> unit) = jsNative

    [<Import("it", from = "vitest")>]
    let itAsync(name: string, test: unit -> Promise<unit>) = jsNative

    [<Import("describe", from = "vitest")>]
    let describe(name: string, testSuit: unit -> unit) = jsNative 


    // module Promise = 
    //     [<Emit("void $0")>]
    //     let start (pr: JS.Promise<'T>): unit = jsNative

    //     [<Emit("void ($1.then($0))")>]
    //     let iter (a: 'T -> unit) (pr: JS.Promise<'T>): unit = jsNative

    //     [<Emit("$1.then($0)")>]
    //     let map (a: 'T1 -> 'T2) (pr: JS.Promise<'T1>): JS.Promise<'T2> = jsNative
    //     let runTests = runTests   
#endif