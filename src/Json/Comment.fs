namespace ARCtrl.Json

open Thoth.Json.Core
open ARCtrl
open System.IO

module Comment = 

    let encoder (comment : Comment) = 
        [
            Encode.tryInclude "name" Encode.string (comment.Name)
            Encode.tryInclude "value" Encode.string (comment.Value)
        ]
        |> Encode.choose
        |> Encode.object

    let decoder : Decoder<Comment> =
        Decode.object (fun get ->
            Comment(
                ?name = get.Optional.Field "name" Decode.string,
                ?value = get.Optional.Field "value" Decode.string
            )
        )    

    module ROCrate = 

        let genID (c:Comment) : string = 
            match c.Name with
            | Some n -> 
            let v = if c.Value.IsSome then "_" + c.Value.Value.Replace(" ","_") else ""
            "#Comment_" + n.Replace(" ","_") + v
            | None -> "#EmptyComment"
    
        let encoder (comment : Comment) = 
            [
                "@id", Encode.string (comment |> genID)
                "@type", Encode.string "Comment"
                Encode.tryInclude "name" Encode.string (comment.Name)
                Encode.tryInclude "value" Encode.string (comment.Value)
                "@context", ROCrateContext.Comment.context_jsonvalue
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<Comment> =
            Decode.object (fun get ->
                Comment(
                    ?name = get.Optional.Field "name" Decode.string,
                    ?value = get.Optional.Field "value" Decode.string
                )
            )    

        let encoderDisambiguatingDescription (comment : Comment) = 
            encoder comment |> Encode.toJsonString 2 |> Encode.string

        let decoderDisambiguatingDescription : Decoder<Comment> = 
            Decode.string 
            |> Decode.map (Decode.fromJsonString decoder)

    module ISAJson =

        let encoder = encoder
        
        let decoder = decoder

[<AutoOpen>]
module CommentExtensions =
    type Comment with

        static member fromJsonString (s:string)  = 
            Decode.fromJsonString Comment.decoder s

        static member toJsonString(?spaces) = 
            fun (c:Comment) ->
                Comment.encoder c
                |> Encode.toJsonString (defaultArg spaces 2)                  

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString Comment.ROCrate.decoder s

        /// exports in json-ld format
        static member toROCrateJsonString(?spaces) =
            fun (c:Comment) ->
                Comment.ROCrate.encoder c
                |> Encode.toJsonString (defaultArg spaces 2)

        static member toISAJsonString(?spaces) =
            fun (c:Comment) ->
                Comment.ISAJson.encoder c
                |> Encode.toJsonString (defaultArg spaces 2)

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Comment.ISAJson.decoder s

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (c:Comment) = 
    //    File.WriteAllText(path,toString c)