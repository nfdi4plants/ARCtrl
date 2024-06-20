namespace ARCtrl.Json

open System.Collections.Generic
open Thoth.Json.Core
open ARCtrl

open ARCtrl.Helper

module IDTable =

    type IDTableWrite = Dictionary<URI,Json>

    type IDTableRead = Dictionary<URI,obj>

    let encode (genID: 'Value -> URI) (encoder : 'Value -> Json) (value : 'Value) (table:IDTableWrite) =
        match table.TryGetValue(genID value) with
        | true, v -> v
        | false, _ -> 
            let v = encoder value
            table.Add(genID value, v)
            v
