namespace ARCtrl.Contract

open ARCtrl.FileSystem
open ARCtrl.ArcPathHelper
open ARCtrl.Spreadsheet
open ARCtrl
open ARCtrl.Helper
open FsSpreadsheet


[<AutoOpen>]
module InvestigationContractExtensions = 

    let (|InvestigationPath|_|) (input) =
        match input with
        | [|InvestigationFileName|] -> 
            let path = ARCtrl.ArcPathHelper.combineMany input
            Some path
        | _ -> None

    type ArcInvestigation with

        member this.ToCreateContract () =
            let converter = ArcInvestigation.toFsWorkbook
            let path = InvestigationFileName
            let c = Contract.createCreate(path, DTOType.ISA_Investigation, DTO.Spreadsheet (this |> converter))
            c

        member this.ToUpdateContract () =
            let converter = ArcInvestigation.toFsWorkbook
            let path = InvestigationFileName
            let c = Contract.createUpdate(path, DTOType.ISA_Investigation, DTO.Spreadsheet (this |> converter))
            c

        //member this.ToDeleteContract () =
        //    let path = InvestigationFileName
        //    let c = Contract.createDelete(path)
        //    c
        
        //static member toDeleteContract () : Contract =
        //    let path = InvestigationFileName
        //    let c = Contract.createDelete(path)
        //    c

        static member toCreateContract (inv: ArcInvestigation) : Contract =
            inv.ToCreateContract()

        static member toUpdateContract (inv: ArcInvestigation) : Contract =
            inv.ToUpdateContract()

        static member tryFromReadContract (c:Contract) =
            match c with
            | {Operation = READ; DTOType = Some DTOType.ISA_Investigation; DTO = Some (DTO.Spreadsheet fsworkbook)} ->
                fsworkbook :?> FsWorkbook
                |> ArcInvestigation.fromFsWorkbook
                |> Some 
            | _ -> None
      