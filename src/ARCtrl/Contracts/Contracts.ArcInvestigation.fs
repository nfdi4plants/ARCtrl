module Contracts.ArcInvestigation

open Contract

let tryFromContract (c:Contract) =
    match c with
    | {Operation = READ; DTOType = Some DTOType.ISA_Investigation; DTO = Some (DTO.Spreadsheet fsworkbook)} ->
        Some fsworkbook
    | _ -> None


open ARCtrl.Path

let (|InvestigationPath|_|) (input) =
    match input with
    | [|InvestigationFileName|] -> 
        let path = FileSystem.Path.combineMany input
        Some path
    | _ -> None
