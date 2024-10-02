namespace ARCtrl.Json

open ARCtrl

[<AutoOpen>]
module PublicationExtensions =

    type Publication with
       
        static member fromJsonString (s:string)  = 
            Decode.fromJsonString Publication.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:Publication) ->
                Publication.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)                  

        member this.ToJsonString(?spaces) =
            Publication.toJsonString(?spaces=spaces) this

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString Publication.ROCrate.decoder s

        /// exports in json-ld format
        static member toROCrateJsonString(?spaces) =
            fun (obj:Publication) ->
                Publication.ROCrate.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?spaces) =
            Publication.toROCrateJsonString(?spaces=spaces) this

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Publication.ISAJson.decoder s
       
        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (obj:Publication) ->
                Publication.ISAJson.encoder idMap obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces, ?useIDReferencing) =
            Publication.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this