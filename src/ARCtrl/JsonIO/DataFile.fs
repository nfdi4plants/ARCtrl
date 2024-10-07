namespace ARCtrl.Json

open ARCtrl

[<AutoOpen>]
module DataFileExtensions =
    
    type DataFile with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString DataFile.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:DataFile) ->
                DataFile.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces) =
            DataFile.toISAJsonString(?spaces=spaces) this

        static member fromROCrateJsonString (s:string) =
            Decode.fromJsonString DataFile.ROCrate.decoder s

        static member toROCrateJsonString(?spaces) =    
            fun (f:DataFile) ->
                DataFile.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?spaces) =
            DataFile.toROCrateJsonString(?spaces=spaces) this