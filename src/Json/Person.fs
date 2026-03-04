namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open System.IO
open ARCtrl.Process.Conversion

module Person = 
              
    let encoder (person : Person) = 
        [
            Encode.tryInclude "firstName" Encode.string person.FirstName
            Encode.tryInclude "lastName" Encode.string person.LastName
            Encode.tryInclude "midInitials" Encode.string person.MidInitials
            Encode.tryInclude "orcid" Encode.string person.ORCID
            Encode.tryInclude "email" Encode.string person.EMail
            Encode.tryInclude "phone" Encode.string person.Phone
            Encode.tryInclude "fax" Encode.string person.Fax
            Encode.tryInclude "address" Encode.string person.Address
            Encode.tryInclude "affiliation" Encode.string person.Affiliation
            Encode.tryIncludeSeq "roles" OntologyAnnotation.encoder person.Roles
            Encode.tryIncludeSeq "comments" Comment.encoder person.Comments
        ]
        |> Encode.choose
        |> Encode.object

    let decoder : Decoder<Person> =
        Decode.object (fun get ->
            Person(
                ?orcid=get.Optional.Field "orcid" Decode.string, 
                ?lastName=get.Optional.Field "lastName" Decode.string,
                ?firstName=get.Optional.Field "firstName" Decode.string,
                ?midInitials=get.Optional.Field "midInitials" Decode.string,
                ?email=get.Optional.Field "email" Decode.string,
                ?phone=get.Optional.Field "phone" Decode.string,
                ?fax=get.Optional.Field "fax" Decode.string,
                ?address=get.Optional.Field "address" Decode.string,
                ?affiliation=get.Optional.Field "affiliation" Decode.string,
                ?roles=get.Optional.Field "roles" (Decode.resizeArray OntologyAnnotation.decoder),
                ?comments=get.Optional.Field "comments" (Decode.resizeArray Comment.decoder)
            )
        )

    let genID (p:Person) : string = 
        let orcid = 
            p.Comments |> Seq.tryPick (fun c ->
            match (c.Name,c.Value) with
            | (Some n,Some v) -> if (n="orcid" || n="Orcid" || n="ORCID") then Some v else None
            | _ -> None )
        match orcid with
        | Some o -> o
        | None -> 
            match p.EMail with
            | Some e -> e.ToString()
            | None -> 
                match (p.FirstName,p.MidInitials,p.LastName) with 
                | (Some fn,Some mn,Some ln) -> "#" + fn.Replace(" ","_") + "_" + mn.Replace(" ","_") + "_" + ln.Replace(" ","_")
                | (Some fn,None,Some ln) -> "#" + fn.Replace(" ","_") + "_" + ln.Replace(" ","_")
                | (None,None,Some ln) -> "#" + ln.Replace(" ","_")
                | (Some fn,None,None) -> "#" + fn.Replace(" ","_")
                | _ -> "#EmptyPerson"

    module ISAJson =

        let allowedFields = ["@id";"firstName";"lastName";"midInitials";"email";"phone";"fax";"address";"affiliation";"roles";"comments";"@type"; "@context"]

        let encoder (idMap : IDTable.IDTableWrite option) (person : Person) = 
            let f (person : Person) =
                let person = Person.setCommentFromORCID person
                [
                    Encode.tryInclude "@id" Encode.string (person |> genID |> Some)
                    Encode.tryInclude "firstName" Encode.string person.FirstName
                    Encode.tryInclude "lastName" Encode.string person.LastName
                    Encode.tryInclude "midInitials" Encode.string person.MidInitials
                    Encode.tryInclude "email" Encode.string person.EMail
                    Encode.tryInclude "phone" Encode.string person.Phone
                    Encode.tryInclude "fax" Encode.string person.Fax
                    Encode.tryInclude "address" Encode.string person.Address
                    Encode.tryInclude "affiliation" Encode.string person.Affiliation
                    Encode.tryIncludeSeq "roles" OntologyAnnotation.encoder person.Roles
                    Encode.tryIncludeSeq "comments" Comment.encoder person.Comments
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f person
            | Some idMap -> IDTable.encode genID f person idMap


        let decoder: Decoder<Person> =
            Decode.objectNoAdditionalProperties allowedFields (fun get ->
                Person(
                    ?orcid=None, //is set later by "Person.setOrcidFromComments" 
                    ?lastName=get.Optional.Field "lastName" Decode.string,
                    ?firstName=get.Optional.Field "firstName" Decode.string,
                    ?midInitials=get.Optional.Field "midInitials" Decode.string,
                    ?email=get.Optional.Field "email" Decode.string,
                    ?phone=get.Optional.Field "phone" Decode.string,
                    ?fax=get.Optional.Field "fax" Decode.string,
                    ?address=get.Optional.Field "address" Decode.string,
                    ?affiliation=get.Optional.Field "affiliation" Decode.string,
                    ?roles=get.Optional.Field "roles" (Decode.resizeArray OntologyAnnotation.decoder),
                    ?comments=get.Optional.Field "comments" (Decode.resizeArray Comment.decoder)
                )
                |> Person.setOrcidFromComments
            )