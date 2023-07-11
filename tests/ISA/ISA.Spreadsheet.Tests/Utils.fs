module TestingUtils

open FsSpreadsheet

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
    failwithf "%s. Sequence does not match at position %i. Expected item: %A, but got %A."
      message i e a
  | i,None,Some e ->
    failwithf "%s. Sequence actual shorter than expected, at pos %i for expected item %A."
      message i e
  | i,Some a,None ->
    failwithf "%s. Sequence actual longer than expected, at pos %i found item %A."
      message i a

module Expect = 

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