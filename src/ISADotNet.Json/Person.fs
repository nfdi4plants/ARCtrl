namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISADotNet
open System.IO
open GEncode

module Person =    

    let rec encoder (options : ConverterOptions) (oa : obj) = 
        [
            tryInclude "@id" GEncode.string (oa |> tryGetPropertyValue "ID")
            tryInclude "firstName" GEncode.string (oa |> tryGetPropertyValue "FirstName")
            tryInclude "lastName" GEncode.string (oa |> tryGetPropertyValue "LastName")
            tryInclude "midInitials" GEncode.string (oa |> tryGetPropertyValue "MidInitials")
            tryInclude "email" GEncode.string (oa |> tryGetPropertyValue "EMail")
            tryInclude "phone" GEncode.string (oa |> tryGetPropertyValue "Phone")
            tryInclude "fax" GEncode.string (oa |> tryGetPropertyValue "Fax")
            tryInclude "address" GEncode.string (oa |> tryGetPropertyValue "Address")
            tryInclude "affiliation" GEncode.string (oa |> tryGetPropertyValue "Affiliation")
            tryInclude "roles" (OntologyAnnotation.encoder options) (oa |> tryGetPropertyValue "Roles")
            tryInclude "comments" (Comment.encoder options) (oa |> tryGetPropertyValue "Comments")
        ]
        |> GEncode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<Person> =
        Decode.object (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                FirstName = get.Optional.Field "firstName" Decode.string
                LastName = get.Optional.Field "lastName" Decode.string
                MidInitials = get.Optional.Field "midInitials" Decode.string
                EMail = get.Optional.Field "email" Decode.string
                Phone = get.Optional.Field "phone" Decode.string
                Fax = get.Optional.Field "fax" Decode.string
                Address = get.Optional.Field "address" Decode.string
                Affiliation = get.Optional.Field "affiliation" Decode.string
                Roles = get.Optional.Field "roles" (Decode.list (OntologyAnnotation.decoder options))
                Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))
            }
            
        )

    let fromString (s:string) = 
        GDecode.fromString (decoder (ConverterOptions())) s

    let toString (p:Person) = 
        encoder (ConverterOptions()) p
        |> Encode.toString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (p:Person) = 
    //    File.WriteAllText(path,toString p)