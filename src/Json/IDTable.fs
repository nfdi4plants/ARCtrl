namespace ARCtrl.Json

open System.Collections.Generic
open Thoth.Json.Core
open ARCtrl

open ARCtrl.Process
open ARCtrl.Helper

module IDTable =

    type IDTableWrite = Dictionary<URI,obj>

    type IDTableRead = Dictionary<URI,obj>

    let encodeID id =
        ["@id",Encode.string id]
        |> Encode.object

    let inline encode<'Value> (genID: 'Value -> URI) (encoder : 'Value -> IEncodable) (value : 'Value) (table:IDTableWrite) =
        let id = genID value
        if table.ContainsKey id then
            match box value with
            | :? Sample as s ->
                let otherS = table.[id] :?> Sample
                let characteristics =
                    List.append (Option.defaultValue [] otherS.Characteristics) (Option.defaultValue [] s.Characteristics)
                    |> List.distinctBy (fun c -> c.NameText, c.ValueWithUnitText)
                    |> Option.fromValueWithDefault []
                let factorValues =
                    List.append (Option.defaultValue [] otherS.FactorValues) (Option.defaultValue [] s.FactorValues)
                    |> List.distinctBy (fun c -> c.NameText, c.ValueWithUnitText)
                    |> Option.fromValueWithDefault []
                table.[id] <- Sample.make (Some id) s.Name characteristics factorValues s.DerivesFrom
                encodeID id
            | :? Source as s ->
                let otherS = table.[id] :?> Source
                let characteristics =
                    List.append (Option.defaultValue [] otherS.Characteristics) (Option.defaultValue [] s.Characteristics)
                    |> List.distinctBy (fun c -> c.NameText, c.ValueWithUnitText)
                    |> Option.fromValueWithDefault []
                table.[id] <- Source.make (Some id) s.Name characteristics
                encodeID id
            | _ ->
                encodeID id
        else
            let v = box value
            if v :? Sample || v :? Source then
                encoder value |> ignore
                table.Add(genID value, v)
                Encode.delay (fun _ ->
                    encoder (table.[id] :?> 'Value)
                )
            else
                let v = encoder value
                table.Add(genID value, v)
                v
