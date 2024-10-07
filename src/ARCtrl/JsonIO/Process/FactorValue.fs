namespace ARCtrl.Json

open ARCtrl.Process

[<AutoOpen>]
module FactorValueExtensions =
    
    type FactorValue with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString FactorValue.ISAJson.decoder s   

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (f:FactorValue) ->
                FactorValue.ISAJson.encoder idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces, ?useIDReferencing) =
            FactorValue.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this