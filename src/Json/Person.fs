namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open System.IO

module Person =   
    
    let genID (p:Person) : string = 
        match p.ID with
        | Some id -> URI.toString id
        | None -> 
            let orcid = match p.Comments with
                        | Some cl -> cl |> Array.tryPick (fun c ->
                            match (c.Name,c.Value) with
                            | (Some n,Some v) -> if (n="orcid" || n="Orcid" || n="ORCID") then Some v else None
                            | _ -> None )
                        | None -> None
            match orcid with
            | Some o -> o
            | None -> match p.EMail with
                      | Some e -> e.ToString()
                      | None -> match (p.FirstName,p.MidInitials,p.LastName) with 
                                | (Some fn,Some mn,Some ln) -> "#" + fn.Replace(" ","_") + "_" + mn.Replace(" ","_") + "_" + ln.Replace(" ","_")
                                | (Some fn,None,Some ln) -> "#" + fn.Replace(" ","_") + "_" + ln.Replace(" ","_")
                                | (None,None,Some ln) -> "#" + ln.Replace(" ","_")
                                | (Some fn,None,None) -> "#" + fn.Replace(" ","_")
                                | _ -> "#EmptyPerson"

    let affiliationEncoder (options : ConverterOptions) (affiliation : string) =
        if options.IsJsonLD then
            let idStr = $"#Organization_{affiliation}"
            [
                ("@type",Encode.string "Organization")
                ("@id",Encode.string (idStr.Replace(" ","_")))
                ("name",Encode.string affiliation)               
                if options.IsJsonLD then
                    "@context", ROCrateContext.Organization.context_jsonvalue
            ]
            |> Encode.object
        else
            Encode.string affiliation
    
    let affiliationDecoder (options : ConverterOptions) : Decoder<string> =
        if options.IsJsonLD then
            Decode.object (fun get ->
                get.Required.Field "name" Decode.string
            ) 
        else
            Decode.string


    let encoder (options : ConverterOptions) (oa : Person) = 
        let oa = oa |> Person.setCommentFromORCID
        let commentEncoder = if options.IsJsonLD then Comment.encoderDisambiguatingDescription else Comment.encoder options
        [
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            else 
                Encode.tryInclude "@id" Encode.string (oa.ID)
            if options.IsJsonLD then 
                "@type", Encode.string "Person"
            Encode.tryInclude "firstName" Encode.string (oa.FirstName)
            Encode.tryInclude "lastName" Encode.string (oa.LastName)
            Encode.tryInclude "midInitials" Encode.string (oa.MidInitials)
            Encode.tryInclude "email" Encode.string (oa.EMail)
            Encode.tryInclude "phone" Encode.string (oa.Phone)
            Encode.tryInclude "fax" Encode.string (oa.Fax)
            Encode.tryInclude "address" Encode.string (oa.Address)
            Encode.tryInclude "affiliation" (affiliationEncoder options) (oa.Affiliation)
            Encode.tryIncludeArray "roles" (OntologyAnnotation.encoder options) (oa.Roles)
            Encode.tryIncludeArray "comments" commentEncoder (oa.Comments)
            if options.IsJsonLD then 
                "@context", ROCrateContext.Person.context_jsonvalue
        ]
        |> Encode.choose
        |> Encode.object

    let allowedFields = ["@id";"firstName";"lastName";"midInitials";"email";"phone";"fax";"address";"affiliation";"roles";"comments";"@type"; "@context"]

    let decoder (options : ConverterOptions) : Decoder<Person> =
        GDecode.object allowedFields (fun get ->
            {
                ID = get.Optional.Field "@id" GDecode.uri
                ORCID = None
                FirstName = get.Optional.Field "firstName" Decode.string
                LastName = get.Optional.Field "lastName" Decode.string
                MidInitials = get.Optional.Field "midInitials" Decode.string
                EMail = get.Optional.Field "email" Decode.string
                Phone = get.Optional.Field "phone" Decode.string
                Fax = get.Optional.Field "fax" Decode.string
                Address = get.Optional.Field "address" Decode.string
                Affiliation = get.Optional.Field "affiliation" (affiliationDecoder options)
                Roles = get.Optional.Field "roles" (Decode.array (OntologyAnnotation.decoder options))
                Comments = get.Optional.Field "comments" (Decode.array (Comment.decoder options))
            }
            |> Person.setOrcidFromComments
            
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s

    let fromJsonldString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions(IsJsonLD=true))) s

    let toJsonString (p:Person) = 
        encoder (ConverterOptions()) p
        |> Encode.toJsonString 2

    /// exports in json-ld format
    let toJsonldString (p:Person) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) p
        |> Encode.toJsonString 2

    let toJsonldStringWithContext (a:Person) = 
        encoder (ConverterOptions(SetID=true,IsJsonLD=true)) a
        |> Encode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (p:Person) = 
    //    File.WriteAllText(path,toString p)