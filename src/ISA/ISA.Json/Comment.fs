namespace ARCtrl.ISA.Json


open Thoth.Json.Core

open ARCtrl.ISA
open System.IO

module Comment = 
    
    let genID (c:Comment) : string = 
        match c.ID with
        | Some id -> URI.toString id
        | None -> match c.Name with
                  | Some n -> 
                    let v = if c.Value.IsSome then "_" + c.Value.Value.Replace(" ","_") else ""
                    "#Comment_" + n.Replace(" ","_") + v
                  | None -> "#EmptyComment"



    let encoder (options : ConverterOptions) (comment : Comment) = 
        [
            if options.SetID then 
                "@id", Encode.string (comment |> genID)
            else 
                GEncode.tryInclude "@id" Encode.string (comment.ID)
            if options.IsJsonLD then 
                "@type", Encode.string "Comment"
            GEncode.tryInclude "name" Encode.string (comment.Name)
            GEncode.tryInclude "value" Encode.string (comment.Value)
            if options.IsJsonLD then
                "@context", ROCrateContext.Comment.context_jsonvalue
        ]
        |> GEncode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<Comment> =
        Decode.object (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                Name = get.Optional.Field "name" Decode.string
                Value = get.Optional.Field "value" Decode.string
            }
        )

    let fromJsonString (s:string)  = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s
    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (c:Comment) = 
        encoder (ConverterOptions()) c
        |> GEncode.toJsonString 2

    /// exports in json-ld format
    let toJsonldString (c:Comment) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) c
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:Comment) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (c:Comment) = 
    //    File.WriteAllText(path,toString c)