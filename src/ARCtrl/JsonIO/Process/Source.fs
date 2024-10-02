namespace ARCtrl.Json

open ARCtrl.Process

[<AutoOpen>]
module SourceExtensions =
    
    type Source with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Source.ISAJson.decoder s   

        static member toISAJsonString(?spaces,?useIDReferencing) =
            let useIDReferencing = useIDReferencing |> Option.defaultValue false
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (f:Source) ->
                Source.ISAJson.encoder idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces, ?useIDReferencing) =
            Source.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this

        static member fromROCrateString (s:string) = 
            Decode.fromJsonString Source.ROCrate.decoder s

        static member toROCrateString(?spaces) =
            fun (f:Source) ->
                Source.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateString(?spaces) =
            Source.toROCrateString(?spaces=spaces) this