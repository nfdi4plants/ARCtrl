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
                "@id", Encode.string (comment |> genID) |> Some
                "@type", Encode.string "Comment" |> Some
                Encode.tryInclude "name" Encode.string (comment.Name)
                Encode.tryInclude "value" Encode.string (comment.Value)
                "@context", ROCrateContext.Comment.context_jsonvalue |> Some
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
            let name = Option.defaultValue "" comment.Name
            let value = Option.defaultValue "" comment.Value
            $"{name}:{value}" |> Encode.string

        let decoderDisambiguatingDescription : Decoder<Comment> = 
            Decode.string 
            |> Decode.map (fun s ->
                let a = s.Trim().Split(':')
                let name,value =
                    match a.Length with
                        | 1 -> Some a.[0], None
                        | 2 -> Some a.[0], Some a.[1]
                        | _ -> Some a.[0], Some(Array.reduce (fun acc x -> acc + ":" + x) (Array.skip 1 a))
                Comment(?name = name, ?value = value)

            )

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