namespace ARCtrl.Json

open ARCtrl

[<AutoOpen>]
module StudyExtensions =

    open System.Collections.Generic

    type ArcStudy with
       
        static member fromJsonString (s:string)  = 
            Decode.fromJsonString Study.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:ArcStudy) ->
                Study.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToJsonString(?spaces) = 
            ArcStudy.toJsonString(?spaces=spaces) this

        static member fromCompressedJsonString (s: string) =
            try Decode.fromJsonString (Compression.decode Study.decoderCompressed) s with
            | e -> failwithf "Error. Unable to parse json string to ArcStudy: %s" e.Message

        static member toCompressedJsonString(?spaces) =
            fun (obj:ArcStudy) ->
                let spaces = defaultArg spaces 0
                Encode.toJsonString spaces (Compression.encode Study.encoderCompressed obj)

        member this.ToCompressedJsonString(?spaces) = 
            ArcStudy.toCompressedJsonString(?spaces=spaces) this

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString Study.ROCrate.decoder s

        /// exports in json-ld format
        static member toROCrateJsonString(?assays: ArcAssay list, ?spaces) =
            fun (obj:ArcStudy) ->
                Study.ROCrate.encoder assays obj 
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?assays,?spaces) = 
            ArcStudy.toROCrateJsonString(?assays=assays,?spaces=spaces) this

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Study.ISAJson.decoder s

        static member toISAJsonString(?assays: ArcAssay list, ?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (obj:ArcStudy) ->
                Study.ISAJson.encoder idMap assays obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?assays,?spaces, ?useIDReferencing) = 
            ArcStudy.toISAJsonString(?assays=assays,?spaces=spaces, ?useIDReferencing = useIDReferencing) this