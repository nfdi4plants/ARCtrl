namespace ARCtrl.Json

open ARCtrl
open ARCtrl.Helper

[<AutoOpen>]
module AssayExtensions =

    type ArcAssay with
       
        static member fromJsonString (s:string)  = 
            Decode.fromJsonString Assay.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:ArcAssay) ->
                Assay.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToJsonString(?spaces) = 
            ArcAssay.toJsonString(?spaces=spaces) this

        static member fromCompressedJsonString (s: string) =
            try Decode.fromJsonString (Compression.decode Assay.decoderCompressed) s with
            | e -> failwithf "Error. Unable to parse json string to ArcAssay: %s" e.Message

        static member toCompressedJsonString(?spaces) =
            fun (obj:ArcAssay) ->
                let spaces = defaultArg spaces 0
                Encode.toJsonString spaces (Compression.encode Assay.encoderCompressed obj)

        member this.ToCompressedJsonString(?spaces) = 
            ArcAssay.toCompressedJsonString(?spaces=spaces) this

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString Assay.ROCrate.decoder s

        /// exports in json-ld format
        static member toROCrateJsonString(?studyName, ?spaces) =
            fun (obj: ArcAssay) ->
                Assay.ROCrate.encoder studyName obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?studyName, ?spaces) = 
            ArcAssay.toROCrateJsonString(?studyName=studyName, ?spaces=spaces) this

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (obj:ArcAssay) ->
                Assay.ISAJson.encoder None idMap obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Assay.ISAJson.decoder s

        member this.ToISAJsonString(?spaces, ?useIDReferencing) =
            ArcAssay.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this