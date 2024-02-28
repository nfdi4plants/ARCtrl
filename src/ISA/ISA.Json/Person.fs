namespace ARCtrl.ISA.Json

open Thoth.Json.Core

open ARCtrl.ISA
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
        if options.IsRoCrate then
            [
                ("@type",Encode.string "Organization")
                ("@id",Encode.string $"Organization/{affiliation}")
                ("name",Encode.string affiliation)               
                if options.IncludeContext then
                    "@context", ROCrateContext.Organization.context_jsonvalue
            ]
            |> Encode.object
        else
            Encode.string affiliation


    let rec encoder (options : ConverterOptions) (oa : Person) = 
        let oa = oa |> Person.setCommentFromORCID
        [
            if options.SetID then 
                "@id", Encode.string (oa |> genID)
            else 
                GEncode.tryInclude "@id" Encode.string (oa.ID)
            if options.IncludeType then 
                "@type", Encode.string "Person"
            GEncode.tryInclude "firstName" Encode.string (oa.FirstName)
            GEncode.tryInclude "lastName" Encode.string (oa.LastName)
            GEncode.tryInclude "midInitials" Encode.string (oa.MidInitials)
            GEncode.tryInclude "email" Encode.string (oa.EMail)
            GEncode.tryInclude "phone" Encode.string (oa.Phone)
            GEncode.tryInclude "fax" Encode.string (oa.Fax)
            GEncode.tryInclude "address" Encode.string (oa.Address)
            GEncode.tryInclude "affiliation" (affiliationEncoder options) (oa.Affiliation)
            GEncode.tryIncludeArray "roles" (OntologyAnnotation.encoder options) (oa.Roles)
            GEncode.tryIncludeArray "comments" (Comment.encoder options) (oa.Comments)
            if options.IncludeContext then 
                "@context", ROCrateContext.Person.context_jsonvalue
        ]
        |> GEncode.choose
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
                Affiliation = get.Optional.Field "affiliation" Decode.string
                Roles = get.Optional.Field "roles" (Decode.array (OntologyAnnotation.decoder options))
                Comments = get.Optional.Field "comments" (Decode.array (Comment.decoder options))
            }
            |> Person.setOrcidFromComments
            
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s

    let toJsonString (p:Person) = 
        encoder (ConverterOptions()) p
        |> GEncode.toJsonString 2

    /// exports in json-ld format
    let toJsonldString (p:Person) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) p
        |> GEncode.toJsonString 2

    let toJsonldStringWithContext (a:Person) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true,IncludeContext=true)) a
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (p:Person) = 
    //    File.WriteAllText(path,toString p)