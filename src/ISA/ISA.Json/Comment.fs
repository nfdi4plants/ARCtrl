namespace ARCtrl.ISA.Json


#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
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
        if options.IsRoCrate then
            let c = comment :?> Comment
            match c.Name,c.Value with
            | Some n, Some v -> GEncode.toJsonString (n+":"+v)
            | Some n, None ->  GEncode.toJsonString n
            | None, Some v ->  GEncode.toJsonString v
            | _ ->  GEncode.toJsonString ""
        else
            [
                if options.SetID then "@id",  GEncode.toJsonString (comment :?> Comment |> genID)
                    else GEncode.tryInclude "@id"  GEncode.toJsonString (comment |> GEncode.tryGetPropertyValue "ID")
                if options.IncludeType then "@type",  GEncode.toJsonString "Comment"
                GEncode.tryInclude "name"  GEncode.toJsonString (comment |> GEncode.tryGetPropertyValue "Name")
                GEncode.tryInclude "value"  GEncode.toJsonString (comment |> GEncode.tryGetPropertyValue "Value")
                if options.IncludeContext then ("@context",Newtonsoft.Json.Linq.JObject.Parse(ROCrateContext.Comment.context).GetValue("@context"))
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
        |> Encode.toString 2

    /// exports in json-ld format
    let toJsonldString (c:Comment) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) c
        |> Encode.toString 2
    let toJsonldStringWithContext (a:Comment) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true,IncludeContext=true)) a
        |> Encode.toString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (c:Comment) = 
    //    File.WriteAllText(path,toString c)
