namespace ISADotNet.Json


#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISADotNet
open System.IO
open GEncode

module Comment = 
    
    let genID (c:Comment) = 
        match c.ID with
            | Some id -> URI.toString id
            | None -> match c.Name with
                        | Some n -> "#Comment_" + n
                        | None -> "#EmptyComment"

    let encoder (options : ConverterOptions) (comment : obj) = 
        [
            if options.SetID then "@id", GEncode.string (comment :?> Comment |> genID)
                else tryInclude "@id" GEncode.string (comment |> tryGetPropertyValue "ID")
            if options.IncludeType then "@type", GEncode.string "Comment"
            tryInclude "name" GEncode.string (comment |> tryGetPropertyValue "Name")
            tryInclude "value" GEncode.string (comment |> tryGetPropertyValue "Value")
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

    let fromString (s:string)  = 
        GDecode.fromString (decoder (ConverterOptions())) s

    let toString (c:Comment) = 
        encoder (ConverterOptions()) c
        |> Encode.toString 2

    /// exports in json-ld format
    let toStringLD (c:Comment) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) c
        |> Encode.toString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (c:Comment) = 
    //    File.WriteAllText(path,toString c)
