namespace ISADotNet.QueryModel
open ISADotNet
module Person = 

    open Errors

    let id (p : Person) =
        match p.ID with
        | Some v -> Ok v
        | None -> Error (PersonHasNoIDException)
    let lastName (p : Person) =
        match p.LastName with
        | Some v -> Ok v
        | None -> Error (PersonHasNoLastNameException)
    let firstName (p : Person) =
        match p.FirstName with
        | Some v -> Ok v
        | None -> Error (PersonHasNoFirstNameException)
    let midInitials (p : Person) =
        match p.MidInitials with
        | Some v -> Ok v
        | None -> Error (PersonHasNoMidInitialsException)
    let email (p : Person) =
        match p.EMail with
        | Some v -> Ok v
        | None -> Error (PersonHasNoEMailException)
    let phone (p : Person) =
        match p.Phone with
        | Some v -> Ok v
        | None -> Error (PersonHasNoPhoneException)
    let fax (p : Person) =
        match p.Fax with
        | Some v -> Ok v
        | None -> Error (PersonHasNoFaxException)
    let address (p : Person) =
        match p.Address with
        | Some v -> Ok v
        | None -> Error (PersonHasNoAddressException)
    let affiliation (p : Person) =
        match p.Affiliation with
        | Some v -> Ok v
        | None -> Error (PersonHasNoAffiliationException)
    let roles (p : Person) =
        match p.Roles with
        | Some v -> Ok v
        | None -> Error (PersonHasNoRolesException)
    let comments (p : Person) =
        match p.Comments with
        | Some v -> Ok v
        | None -> Error (PersonHasNoCommentsException) 