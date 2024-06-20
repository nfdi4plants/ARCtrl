namespace ARCtrl.Json

open Thoth.Json.Core
open ARCtrl

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
            encoder comment |> Encode.toJsonString 0 |> Encode.string

        let decoderDisambiguatingDescription : Decoder<Comment> = 
            Decode.string 
            |> Decode.map (fun s -> s.Trim() |> Decode.fromJsonString decoder)

    module ISAJson =

        let encoder (idMap : IDTable.IDTableWrite option) (comment : Comment) = 
            let f (comment : Comment) =
                [
                    Encode.tryInclude "@id" Encode.string (ROCrate.genID comment |> Some)
                    Encode.tryInclude "name" Encode.string (comment.Name)
                    Encode.tryInclude "value" Encode.string (comment.Value)
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | Some idMap -> IDTable.encode ROCrate.genID f comment idMap
            | None -> f comment


        let decoder = decoder

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

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString Comment.ROCrate.decoder s

        /// exports in json-ld format
        static member toROCrateJsonString(?spaces) =
            fun (c:Comment) ->
                Comment.ROCrate.encoder c
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toROCrateJsonString(?spaces) = 
            Comment.toROCrateJsonString(?spaces=spaces) this

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