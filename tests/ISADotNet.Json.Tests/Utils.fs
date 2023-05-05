module TestingUtils

open Expecto
    
module Result =

    let getMessage res =
        match res with
        | Ok m -> m
        | Error m -> m