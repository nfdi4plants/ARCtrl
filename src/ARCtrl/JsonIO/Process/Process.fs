namespace ARCtrl.Json

open ARCtrl.Process

[<AutoOpen>]
module ProcessExtensions =
    
    type Process with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Process.ISAJson.decoder s   

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (f:Process) ->
                Process.ISAJson.encoder None None idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces,?useIDReferencing) =
            Process.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this

        static member fromROCrateString (s:string) =
            Decode.fromJsonString Process.ROCrate.decoder s

        static member toROCrateString(?studyName:string,?assayName:string,?spaces) =
            fun (f:Process) ->
                Process.ROCrate.encoder studyName assayName f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateString(?studyName:string,?assayName:string,?spaces) =
            Process.toROCrateString(?studyName=studyName,?assayName=assayName,?spaces=spaces) this