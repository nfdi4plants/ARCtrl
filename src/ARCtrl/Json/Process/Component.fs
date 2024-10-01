namespace ARCtrl.Json

open ARCtrl.Process

[<AutoOpen>]
module ComponentExtensions =
    
    type Component with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Component.ISAJson.decoder s   

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (f:Component) ->
                Component.ISAJson.encoder idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toISAJsonString(?spaces) =
            Component.toISAJsonString(?spaces=spaces) this

        static member fromROCrateJsonString (s:string) =
            Decode.fromJsonString Component.ROCrate.decoder s

        static member toROCrateJsonString(?spaces) =
            fun (f:Component) ->
                Component.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toROCrateJsonString(?spaces) =
            Component.toROCrateJsonString(?spaces=spaces) this