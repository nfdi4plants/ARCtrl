namespace ARCtrl.Json

open ARCtrl

[<AutoOpen>]
module DataExtensions =
    
    type Data with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Data.ISAJson.decoder s   

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = useIDReferencing |> Option.defaultValue false
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (f:Data) ->
                Data.ISAJson.encoder idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toISAJsonString(?spaces, ?useIDReferencing) =
            Data.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this

        static member fromROCrateJsonString (s:string) =
            Decode.fromJsonString Data.ROCrate.decoder s

        static member toROCrateJsonString(?spaces) =
            fun (f:Data) ->
                Data.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toROCrateJsonString(?spaces) =
            Data.toROCrateJsonString(?spaces=spaces) this