namespace ARCtrl.Json

open ARCtrl

[<AutoOpen>]
module OntologySourceReferenceExtensions =

    type OntologySourceReference with
       
        static member fromJsonString (s:string)  = 
            Decode.fromJsonString OntologySourceReference.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:OntologySourceReference) ->
                OntologySourceReference.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)                  

        member this.ToJsonString(?spaces) =
            OntologySourceReference.toJsonString(?spaces=spaces) this

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString OntologySourceReference.ROCrate.decoder s

        /// exports in json-ld format
        static member toROCrateJsonString(?spaces) =
            fun (obj:OntologySourceReference) ->
                OntologySourceReference.ROCrate.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?spaces) =
            OntologySourceReference.toROCrateJsonString(?spaces=spaces) this

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString OntologySourceReference.ISAJson.decoder s

        static member toISAJsonString(?spaces) =
            fun (obj:OntologySourceReference) ->
                OntologySourceReference.ISAJson.encoder None obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces) =
            OntologySourceReference.toISAJsonString(?spaces=spaces) this
