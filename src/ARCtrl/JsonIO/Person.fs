namespace ARCtrl.Json

open ARCtrl

[<AutoOpen>]
module PersonExtensions =

    type Person with
       
        static member fromJsonString (s:string)  = 
            Decode.fromJsonString Person.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:Person) ->
                Person.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)                  

        member this.toJsonString(?spaces) =
            Person.toJsonString(?spaces=spaces) this

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString Person.ROCrate.decoder s

        /// exports in json-ld format
        static member toROCrateJsonString(?spaces) =
            fun (obj:Person) ->
                Person.ROCrate.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toROCrateJsonString(?spaces) =
            Person.toROCrateJsonString(?spaces=spaces) this

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Person.ISAJson.decoder s

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (obj:Person) ->
                Person.ISAJson.encoder idMap obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toISAJsonString(?spaces, ?useIDReferencing) =
            Person.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this