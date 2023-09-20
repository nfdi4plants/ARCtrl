module ARCtrl.Contracts.Tests

open TestingUtils

open ARCtrl
open ARCtrl.Contract
open FsSpreadsheet

let tests_tryFromContract = testList "tryFromContract" [
    testCase "Exists" <| fun _ ->
        let fswb = new FsWorkbook()
        let contracts = [|
            Contract.create(
                READ, 
                "isa.investigation.xlsx", 
                DTOType.ISA_Investigation, 
                DTO.Spreadsheet fswb
            )
        |]
        let investigation = contracts |> Array.choose ArcInvestigation.tryFromContract
        Expect.hasLength investigation 1 ""
]

let main = testList "Contracts" [
    tests_tryFromContract
]