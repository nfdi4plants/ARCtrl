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

    module ROCrate =
        
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

        module Affiliation =
            let encoder  (affiliation : string) =
                let idStr = $"#Organization_{affiliation}"
                [
                    ("@type",Encode.string "Organization")
                    ("@id",Encode.string (idStr.Replace(" ","_")))
                    ("name",Encode.string affiliation)               
                    "@context", ROCrateContext.Organization.context_jsonvalue
                ]
                |> Encode.object

            let decoder: Decoder<string> =
                Decode.object (fun get ->
                    get.Required.Field "name" Decode.string
                ) 

        let encoder (oa : Person) = 
            [
                "@id", Encode.string (oa |> genID) |> Some
                "@type", Encode.string "Person" |> Some
                Encode.tryInclude "orcid" Encode.string oa.ORCID 
                Encode.tryInclude "firstName" Encode.string oa.FirstName
                Encode.tryInclude "lastName" Encode.string oa.LastName
                Encode.tryInclude "midInitials" Encode.string oa.MidInitials
                Encode.tryInclude "email" Encode.string oa.EMail
                Encode.tryInclude "phone" Encode.string oa.Phone
                Encode.tryInclude "fax" Encode.string oa.Fax
                Encode.tryInclude "address" Encode.string oa.Address
                Encode.tryInclude "affiliation" Affiliation.encoder oa.Affiliation
                Encode.tryIncludeSeq "roles" OntologyAnnotation.ROCrate.encoderDefinedTerm oa.Roles
                Encode.tryIncludeSeq "comments" Comment.ROCrate.encoderDisambiguatingDescription oa.Comments
                "@context", ROCrateContext.Person.context_jsonvalue |> Some
            ]
            |> Encode.choose
            |> Encode.object


        let decoder: Decoder<Person> =
            Decode.object (fun get ->
                Person(
                    ?orcid=get.Optional.Field "orcid" Decode.string, //is set afterwards with "Person.setOrcidFromComments"
                    ?lastName=get.Optional.Field "lastName" Decode.string,
                    ?firstName=get.Optional.Field "firstName" Decode.string,
                    ?midInitials=get.Optional.Field "midInitials" Decode.string,
                    ?email=get.Optional.Field "email" Decode.string,
                    ?phone=get.Optional.Field "phone" Decode.string,
                    ?fax=get.Optional.Field "fax" Decode.string,
                    ?address=get.Optional.Field "address" Decode.string,
                    ?affiliation=get.Optional.Field "affiliation" Affiliation.decoder,
                    ?roles=get.Optional.Field "roles" (Decode.resizeArray OntologyAnnotation.ROCrate.decoderDefinedTerm),
                    ?comments=get.Optional.Field "comments" (Decode.resizeArray Comment.ROCrate.decoderDisambiguatingDescription)
                )
            )

        /// <summary>
        /// This is only used for ro-crate creation. In ISA publication authors are only a string. ro-crate requires person object.
        /// Therefore, we try to split the string by common separators and create a minimal person object for ro-crate.
        /// </summary>
        /// <param name="authorList"></param>
        let encodeAuthorListString (authorList: string) =
            let tab = "\t"
            let semi = ";"
            let comma = ","
            let separator =
                if authorList.Contains(tab) then tab
                elif authorList.Contains(semi) then semi
                else comma
            let names = authorList.Split([|separator|], System.StringSplitOptions.None) |> Array.map (fun s -> s.Trim())
            let encodeSingle (name:string) =
                [
                    "@type", Encode.string "Person" |> Some
                    Encode.tryInclude "name" Encode.string (Some name)
                    "@context", ROCrateContext.Person.contextMinimal_jsonValue  |> Some
                ]
                |> Encode.choose
                |> Encode.object
            Encode.array (names |> Array.map encodeSingle)

        /// <summary>
        /// This is only used for ro-crate creation. In ISA publication authors are only a string. ro-crate requires person object.
        /// Therefore, we try to split the string by common separators and create a minimal person object for ro-crate.
        /// </summary>
        /// <param name="authorList"></param>
        let decodeAuthorListString: Decoder<string> =
            let decodeSingle: Decoder<string> =
                Decode.object (fun get ->
                    get.Optional.Field "name" Decode.string |> Option.defaultValue ""
                )
            Decode.array decodeSingle
            |> Decode.map (fun v ->
                let cs = v |> String.concat ", "
                cs
            )

    module ISAJson =

        let allowedFields = ["@id";"firstName";"lastName";"midInitials";"email";"phone";"fax";"address";"affiliation";"roles";"comments";"@type"; "@context"]

        let encoder (idMap : IDTable.IDTableWrite option) (person : Person) = 
            let f (person : Person) =
                let person = Person.setCommentFromORCID person
                [
                    Encode.tryInclude "@id" Encode.string (person |> ROCrate.genID |> Some)
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
            | Some idMap -> IDTable.encode ROCrate.genID f person idMap


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

[<AutoOpen>]
module PersonExtensions =

    type Person with
       
        static member fromJsonString (s:string)  = 
            Decode.fromJsonString Person.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:Person) ->
                Person.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)                  

        member this.toJsonString(?spaces) =
            Person.toJsonString(?spaces=spaces) this

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString Person.ROCrate.decoder s

        /// exports in json-ld format
        static member toROCrateJsonString(?spaces) =
            fun (obj:Person) ->
                Person.ROCrate.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toROCrateJsonString(?spaces) =
            Person.toROCrateJsonString(?spaces=spaces) this

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Person.ISAJson.decoder s

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (obj:Person) ->
                Person.ISAJson.encoder idMap obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toISAJsonString(?spaces, ?useIDReferencing) =
            Person.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this