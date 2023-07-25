module Contracts.ArcAssay

open Contract

let tryFromContract (c:Contract) =
    match c with
    | {Operation = READ; DTOType = Some DTOType.ISA_Study; DTO = Some (DTO.Spreadsheet fsworkbook)} ->
        Some fsworkbook
    | _ -> None

