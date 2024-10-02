namespace ARCtrl.Json

open ARCtrl.Process

[<AutoOpen>]
module MaterialAttributeValueExtensions =
    
    type MaterialAttributeValue with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString MaterialAttributeValue.ISAJson.decoder s   

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (f:MaterialAttributeValue) ->
                MaterialAttributeValue.ISAJson.encoder idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces, ?useIDReferencing) =
            MaterialAttributeValue.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString MaterialAttributeValue.ROCrate.decoder s

        static member toROCrateJsonString(?spaces) =
            fun (f:MaterialAttributeValue) ->
                MaterialAttributeValue.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?spaces) =
            MaterialAttributeValue.toROCrateJsonString(?spaces=spaces) this