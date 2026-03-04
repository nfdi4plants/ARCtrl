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
