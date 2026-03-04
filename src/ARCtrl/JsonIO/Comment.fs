namespace ARCtrl.Json

open ARCtrl

[<AutoOpen>]
module CommentExtensions =
    type Comment with

        static member fromJsonString (s:string)  = 
            Decode.fromJsonString Comment.decoder s

        static member toJsonString(?spaces) = 
            fun (c:Comment) ->
                Comment.encoder c
                |> Encode.toJsonString (Encode.defaultSpaces spaces)                  

        member this.toJsonString(?spaces) = 
            Comment.toJsonString(?spaces=spaces) this

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Comment.ISAJson.decoder s

        static member toISAJsonString(?spaces) =
            fun (c:Comment) ->
                Comment.ISAJson.encoder None c
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toISAJsonString(?spaces) =
            Comment.toISAJsonString(?spaces=spaces) this
    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (c:Comment) = 
    //    File.WriteAllText(path,toString c)