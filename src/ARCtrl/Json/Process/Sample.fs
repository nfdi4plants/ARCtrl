namespace ARCtrl.Json

open ARCtrl.Process

[<AutoOpen>]
module SampleExtensions =
    
    type Sample with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Sample.ISAJson.decoder s   

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = useIDReferencing |> Option.defaultValue false
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (f:Sample) ->
                Sample.ISAJson.encoder idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces, ?useIDReferencing) =
            Sample.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this

        static member fromROCrateString (s:string) =
            Decode.fromJsonString Sample.ROCrate.decoder s

        static member toROCrateString(?spaces) =
            fun (f:Sample) ->
                Sample.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateString(?spaces) =
            Sample.toROCrateString(?spaces=spaces) this
