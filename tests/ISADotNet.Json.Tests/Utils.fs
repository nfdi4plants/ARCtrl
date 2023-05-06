module TestingUtils

open Expecto
   
module Utils = 

    let extractWords (json:string) = 
        json.Split([|'{';'}';'[';']';',';':'|])
        |> Array.map (fun s -> s.Trim())
        |> Array.filter ((<>) "")

module Result =



    let getMessage res =
        match res with
        | Ok m -> m
        | Error m -> m