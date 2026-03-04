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


    let genID (c:Comment) : string = 
        match c.Name with
        | Some n -> 
        let v = if c.Value.IsSome then "_" + c.Value.Value.Replace(" ","_") else ""
        "#Comment_" + n.Replace(" ","_") + v
        | None -> "#EmptyComment"
 

    module ISAJson =

        let encoder (idMap : IDTable.IDTableWrite option) (comment : Comment) = 
            let f (comment : Comment) =
                [
                    Encode.tryInclude "@id" Encode.string (genID comment |> Some)
                    Encode.tryInclude "name" Encode.string (comment.Name)
                    Encode.tryInclude "value" Encode.string (comment.Value)
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | Some idMap -> IDTable.encode genID f comment idMap
            | None -> f comment


        let decoder = decoder