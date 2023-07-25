module Contracts.ArcAssay

open Contract

let tryFromContract (c:Contract) =
    match c with
    | {Operation = READ; DTOType = Some DTOType.ISA_Assay; DTO = Some (DTO.Spreadsheet fsworkbook)} ->
        Some fsworkbook
    | _ -> None

open ARCtrl.Path

let (|AssayPath|_|) (input) =
    match input with
    | [|AssaysFolderName; anyAssayName; AssayFileName|] -> 
        let path = FileSystem.Path.combineMany input
        Some path
    | _ -> None
