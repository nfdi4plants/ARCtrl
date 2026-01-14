namespace ARCtrl.Json

open ARCtrl

[<AutoOpen>]
module InvestigationExtensions =

    type ArcInvestigation with
       
        static member fromJsonString (s:string)  = 
            Decode.fromJsonString Investigation.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:ArcInvestigation) ->
                Investigation.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)                  

        member this.ToJsonString(?spaces) = 
            ArcInvestigation.toJsonString(?spaces=spaces) this

        static member fromCompressedJsonString (s: string) =
            try Decode.fromJsonString (Compression.decode Investigation.decoderCompressed) s with
            | e -> failwithf "Error. Unable to parse json string to ArcStudy: %s" e.Message

        static member toCompressedJsonString(?spaces) =
            fun (obj:ArcInvestigation) ->
                let spaces = defaultArg spaces 0
                Encode.toJsonString spaces (Compression.encode Investigation.encoderCompressed obj)

        member this.ToCompressedJsonString(?spaces) = 
            ArcInvestigation.toCompressedJsonString(?spaces=spaces) this

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (obj:ArcInvestigation) ->
                Investigation.ISAJson.encoder idMap obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Investigation.ISAJson.decoder s

        member this.ToISAJsonString(?spaces, ?useIDReferencing) =
            ArcInvestigation.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this