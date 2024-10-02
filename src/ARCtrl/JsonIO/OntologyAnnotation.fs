namespace ARCtrl.Json
open ARCtrl

[<AutoOpen>]
module OntologyAnnotationExtensions =

    type OntologyAnnotation with
       
        static member fromJsonString (s:string)  = 
            Decode.fromJsonString OntologyAnnotation.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:OntologyAnnotation) ->
                OntologyAnnotation.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)                  

        member this.ToJsonString(?spaces) =
            OntologyAnnotation.toJsonString(?spaces=spaces) this

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString OntologyAnnotation.ROCrate.decoderDefinedTerm s

        /// exports in json-ld format
        static member toROCrateJsonString(?spaces) =
            fun (obj:OntologyAnnotation) ->
                OntologyAnnotation.ROCrate.encoderDefinedTerm obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?spaces) =
            OntologyAnnotation.toROCrateJsonString(?spaces=spaces) this

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString OntologyAnnotation.ISAJson.decoder s

        static member toISAJsonString(?spaces) =
            fun (obj:OntologyAnnotation) ->
                OntologyAnnotation.ISAJson.encoder None obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces) =
            OntologyAnnotation.toISAJsonString(?spaces=spaces) this