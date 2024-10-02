namespace ARCtrl.Json

open ARCtrl.Process

[<AutoOpen>]
module ProtocolExtensions =
    
    type Protocol with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Protocol.ISAJson.decoder s   

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (f:Protocol) ->
                Protocol.ISAJson.encoder None None None idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)
        
        member this.ToISAJsonString(?spaces) =
            Protocol.toISAJsonString(?spaces=spaces) this

        static member fromROCrateString (s:string) =
            Decode.fromJsonString (Protocol.ROCrate.decoder) s

        static member toROCrateString (?studyName:string,?assayName:string,?processName:string,?spaces) =
            fun (f:Protocol) ->
                Protocol.ROCrate.encoder studyName assayName processName f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateString (?studyName:string,?assayName:string,?processName:string,?spaces) =
            Protocol.toROCrateString (?studyName=studyName,?assayName=assayName,?processName=processName,?spaces=spaces) this