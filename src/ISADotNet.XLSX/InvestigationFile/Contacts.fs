namespace ISADotNet.XSLX

open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open Comment
open Remark
open System.Collections.Generic

module Contacts = 

    let lastNameLabel = "Last Name"
    let firstNameLabel = "First Name"
    let midInitialsLabel = "Mid Initials"
    let emailLabel = "Email"
    let phoneLabel = "Phone"
    let faxLabel = "Fax"
    let addressLabel = "Address"
    let affiliationLabel = "Affiliation"
    let rolesLabel = "Roles"
    let rolesTermAccessionNumberLabel = "Roles Term Accession Number"
    let rolesTermSourceREFLabel = "Roles Term Source REF"


    let createPerson lastName firstName midInitials email phone fax address affiliation role rolesTermAccessionNumber rolesTermSourceREF comments =
        let roles = OntologyAnnotation.fromAggregatedStrings ';' role rolesTermAccessionNumber rolesTermSourceREF
        Person.create null lastName firstName midInitials email phone fax address affiliation roles comments

    let readPersons (prefix : string) lineNumber (en:IEnumerator<Row>) =
        let rec loop 
            lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
            comments remarks lineNumber = 

            let create () = 
                let length =
                    [|lastNames;firstNames;midInitialss;emails;phones;faxs;addresss;affiliations;roless;rolesTermAccessionNumbers;rolesTermSourceREFs|]
                    |> Array.map Array.length
                    |> Array.max

                List.init length (fun i ->
                    let roles = 
                        splitAndCreateOntologies
                            (Array.tryItemDefault i "" roless)
                            (Array.tryItemDefault i "" rolesTermAccessionNumbers)
                            (Array.tryItemDefault i "" rolesTermSourceREFs)
                    let comments = 
                        List.map (fun (key,values) -> 
                            Comment.create "" key (Array.tryItemDefault i "" values)
                        ) comments
                    Person.create
                        ""
                        (Array.tryItemDefault i "" lastNames)
                        (Array.tryItemDefault i "" firstNames)
                        (Array.tryItemDefault i "" midInitialss)
                        (Array.tryItemDefault i "" emails)
                        (Array.tryItemDefault i "" phones)
                        (Array.tryItemDefault i "" faxs)
                        (Array.tryItemDefault i "" addresss)
                        (Array.tryItemDefault i "" affiliations)
                        roles
                        comments
                    |> REF.Item
                )
            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v) |> Array.ofIndexedSeq
                match Array.tryItem 0 row , Array.trySkip 1 row with

                | Comment k, Some v -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        ((k,v) :: comments) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments (Remark.create lineNumber k :: remarks) (lineNumber + 1)

                | Some k, Some lastNames when k = prefix + " " + Person.LastNameLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some firstNames when k = prefix + " " + Person.FirstNameLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some midInitialss when k = prefix + " " + Person.MidInitialsLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some emails when k = prefix + " " + Person.EmailLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some phones when k = prefix + " " + Person.PhoneLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some faxs when k = prefix + " " + Person.FaxLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some addresss when k = prefix + " " + Person.AddressLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some affiliations when k = prefix + " " + Person.AffiliationLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some roless when k = prefix + " " + Person.RolesLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some rolesTermAccessionNumbers when k = prefix + " " + Person.RolesTermAccessionNumberLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some rolesTermSourceREFs when k = prefix + " " + Person.RolesTermSourceREFLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,create ()
                | _ -> None, lineNumber,remarks,create ()
            else
                None,lineNumber,remarks,create ()
        loop [||] [||] [||] [||] [||] [||] [||] [||] [||] [||] [||] [] [] lineNumber

    
    let writePersons prefix (persons : Person list) =
        let commentKeys = persons |> List.collect (fun person -> person.Comments |> List.map (fun c -> c.Name)) |> List.distinct

        seq {
            let roleNames,roleAccessions,roleSourceRefs = persons |> List.map (fun p -> mergeOntologies p.Roles) |> List.unzip3
            yield   ( Row.ofValues None 0u (prefix + " " + Person.LastNameLabel                      :: (persons |> List.map (fun person -> person.LastName))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.FirstNameLabel                      :: (persons |> List.map (fun person -> person.FirstName))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.MidInitialsLabel                      :: (persons |> List.map (fun person -> person.MidInitials))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.EmailLabel                      :: (persons |> List.map (fun person -> person.EMail))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.PhoneLabel                      :: (persons |> List.map (fun person -> person.Phone))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.FaxLabel                      :: (persons |> List.map (fun person -> person.Fax))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.AddressLabel                      :: (persons |> List.map (fun person -> person.Address))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.AffiliationLabel                      :: (persons |> List.map (fun person -> person.Affiliation))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.RolesLabel                      :: roleNames))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.RolesTermAccessionNumberLabel   :: roleAccessions))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.RolesTermSourceREFLabel         :: roleSourceRefs))

            for key in commentKeys do
                let values = 
                    persons |> List.map (fun person -> 
                        List.tryPickDefault (fun (c : Comment) -> if c.Name = key then Some c.Value else None) "" person.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }