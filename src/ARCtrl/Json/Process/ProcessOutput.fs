namespace ARCtrl.Json

open ARCtrl.Process

[<AutoOpen>]
module ProcessOutputExtensions =
    
    type ProcessOutput with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString ProcessOutput.ISAJson.decoder s   

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = defaultArg useIDReferencing false
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (f:ProcessOutput) ->
                ProcessOutput.ISAJson.encoder idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toISAJsonString(?spaces, ?useIDReferencing) = 
            ProcessOutput.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this

        static member fromROCrateJsonString (s:string) =
            Decode.fromJsonString ProcessOutput.ROCrate.decoder s

        static member toROCrateJsonString(?spaces) =
            fun (f:ProcessOutput) ->
                ProcessOutput.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toROCrateJsonString(?spaces) =
            ProcessOutput.toROCrateJsonString(?spaces=spaces) this