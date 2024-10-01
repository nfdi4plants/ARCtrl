namespace ARCtrl.Json

open ARCtrl.Process

[<AutoOpen>]
module ProcessParameterValueExtensions =
    
    type ProcessParameterValue with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString ProcessParameterValue.ISAJson.decoder s   

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (f:ProcessParameterValue) ->
                ProcessParameterValue.ISAJson.encoder idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces, ?useIDReferencing) =
            ProcessParameterValue.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this

        static member fromROCrateJsonString (s:string) =
            Decode.fromJsonString ProcessParameterValue.ROCrate.decoder s

        static member toROCrateJsonString(?spaces) =
            fun (f:ProcessParameterValue) ->
                ProcessParameterValue.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?spaces) =
            ProcessParameterValue.toROCrateJsonString(?spaces=spaces) this