namespace ARCtrl.Json

open System.Collections.Generic
open Thoth.Json.Core
open ARCtrl

open ARCtrl.Helper

module IDTable =

    type IDTableWrite = Dictionary<URI,Json>

    type IDTableRead = Dictionary<URI,obj>

    let encodeID id =
        ["@id",Encode.string id]
        |> Encode.object

    let encode (genID: 'Value -> URI) (encoder : 'Value -> Json) (value : 'Value) (table:IDTableWrite) =
        let id = genID value
        if table.ContainsKey id then 
           encodeID id
        else
            let v = encoder value
            table.Add(genID value, v)
            v            
