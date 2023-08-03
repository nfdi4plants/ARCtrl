module TestingUtils

open Expecto
open ARCtrl.ISA.Json

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

module Result =

    let getMessage res =
        match res with
        | Ok m -> m
        | Error m -> m

let firstDiff s1 s2 =
  let s1 = Seq.append (Seq.map Some s1) (Seq.initInfinite (fun _ -> None))
  let s2 = Seq.append (Seq.map Some s2) (Seq.initInfinite (fun _ -> None))
  Seq.mapi2 (fun i s p -> i,s,p) s1 s2
  |> Seq.find (function |_,Some s,Some p when s=p -> false |_-> true)

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