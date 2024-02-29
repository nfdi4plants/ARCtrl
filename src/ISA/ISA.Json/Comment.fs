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



    let encoder (options : ConverterOptions) (comment : obj) = 
        [
            if options.SetID then "@id", GEncode.includeString (comment :?> Comment |> genID)
                else GEncode.tryInclude "@id" GEncode.includeString (comment |> GEncode.tryGetPropertyValue "ID")
            if options.IncludeType then "@type", GEncode.includeString "Comment"
            GEncode.tryInclude "name" GEncode.includeString (comment |> GEncode.tryGetPropertyValue "Name")
            GEncode.tryInclude "value" GEncode.includeString (comment |> GEncode.tryGetPropertyValue "Value")
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

    let toJsonString (c:Comment) = 
        encoder (ConverterOptions()) c
        |> GEncode.toJsonString 2

    /// exports in json-ld format
    let toStringLD (c:Comment) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) c
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (c:Comment) = 
    //    File.WriteAllText(path,toString c)
