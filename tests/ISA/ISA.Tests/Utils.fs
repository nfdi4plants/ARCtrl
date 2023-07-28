module TestingUtils

open ISA

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

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
    
    let arcTableEqual (t1 : ArcTable) (t2 : ArcTable) (message : string) = 
        let sortVals (dict : System.Collections.Generic.Dictionary<int*int,CompositeCell>) = 
            dict |> Seq.sortBy (fun kv -> kv.Key)
        Expect.equal t1.Name t2.Name (sprintf "%s: Name" message)
        mySequenceEqual t1.Headers t2.Headers (sprintf "%s: Headers" message)
        mySequenceEqual (t1.Values |> sortVals) (t2.Values |> sortVals) (sprintf "%s: Values" message)