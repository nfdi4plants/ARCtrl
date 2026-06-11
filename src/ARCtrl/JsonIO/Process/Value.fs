namespace ARCtrl.Json

open ARCtrl

[<AutoOpen>]
module ValueExtensions =

    type PropertyValue with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Value.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (v:PropertyValue) ->
                Value.ISAJson.encoder None v
                |> Encode.toJsonString (Encode.defaultSpaces spaces)
            

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (v:Value) = 
    //    File.WriteAllText(path,toString v)