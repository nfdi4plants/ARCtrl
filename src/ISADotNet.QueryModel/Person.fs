namespace ISADotNet.QueryModel
open ISADotNet
module Person = 

    open Errors

    let id (p : Person) =
        match p.ID with
        | Some v -> v
        | None -> raise PersonHasNoIDException
    let lastName (p : Person) =
        match p.LastName with
        | Some v -> v
        | None -> raise PersonHasNoLastNameException
    let firstName (p : Person) =
        match p.FirstName with
        | Some v -> v
        | None -> raise PersonHasNoFirstNameException
    let midInitials (p : Person) =
        match p.MidInitials with
        | Some v -> Ok v
        | None -> raise PersonHasNoMidInitialsException
    let email (p : Person) =
        match p.EMail with
        | Some v -> Ok v
        | None -> raise PersonHasNoEMailException
    let phone (p : Person) =
        match p.Phone with
        | Some v -> Ok v
        | None -> raise PersonHasNoPhoneException
    let fax (p : Person) =
        match p.Fax with
        | Some v -> Ok v
        | None -> raise PersonHasNoFaxException
    let address (p : Person) =
        match p.Address with
        | Some v -> Ok v
        | None -> raise PersonHasNoAddressException
    let affiliation (p : Person) =
        match p.Affiliation with
        | Some v -> Ok v
        | None -> raise PersonHasNoAffiliationException
    let roles (p : Person) =
        match p.Roles with
        | Some v -> Ok v
        | None -> raise PersonHasNoRolesException
    let comments (p : Person) =
        match p.Comments with
        | Some v -> Ok v
        | None -> raise PersonHasNoCommentsException