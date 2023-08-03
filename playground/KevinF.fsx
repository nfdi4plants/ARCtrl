#r "nuget: Fable.Core, 4.0.0"
#r "nuget: FsSpreadsheet, 2.0.2"
#r "nuget: FsSpreadsheet.ExcelIO, 2.0.2"
#I @"../src\ARCtrl/bin\Debug\netstandard2.0"
#r "ISA.dll"
#r "Contract.dll"
#r "FileSystem.dll"
#r "ISA.Spreadsheet.dll"
#r "CWL.dll"
#r "ARCtrl.dll"

//#i @"nuget: C:/Users/Kevin/source/repos/ISADotNet/pkg/"
//#r "nuget: ARC"

let [<Literal>] rootPath = @"C:\Users\Kevin\Desktop\TestARC"
open Contract
open ARCtrl
open ARCtrl.FileSystem

module ARC_IO =
    open FsSpreadsheet
    open FsSpreadsheet.ExcelIO
    let readFilePaths (arcPath: string) = 
        System.IO.Directory.EnumerateFiles(arcPath,"*",System.IO.SearchOption.AllDirectories)
        |> Array.ofSeq 
        |> Array.map (fun p -> System.IO.Path.GetRelativePath(rootPath, p))

    let fullfillREADContract (arcRoot: string) (c: Contract) =
        let p = System.IO.Path.Combine [|arcRoot; c.Path|]
        match c with
        | {Operation = READ} ->
            let dto = DTO.Spreadsheet <| FsWorkbook.fromXlsxFile(p)
            {c with DTO = Some dto}
        | _ -> failwith "Tried reading from non-READ contract."

    let fullfillREADContracts (arcRoot: string) (cArr: Contract []) =
        cArr |> Array.map (fullfillREADContract arcRoot)

    let initExistingARC (arcRootPath: string) =
        let arc = readFilePaths arcRootPath |> ARC.fromFilePaths
        let contracts = arc.getReadContracts() |> fullfillREADContracts arcRootPath
        arc.addISAFromContracts(contracts)

let myarc = ARC_IO.initExistingARC(rootPath)

let newArcPath = System.IO.Path.Combine(__SOURCE_DIRECTORY__, "TestArc") 

open ARCtrl.ISA
open FsSpreadsheet
open FsSpreadsheet.ExcelIO
open ARCtrl.ISA.Spreadsheet

module WriteContracts =

    let getInvestigationPath() = Path.combineMany [|ARCtrl.Path.InvestigationFileName|]

    let createInvestigation (investigation: ArcInvestigation) =
        let investigationFile = ArcInvestigation.toFsWorkbook (investigation)
        let investigationFilePath = getInvestigationPath ()
        let investigationContract = Contract.createCreate(investigationFilePath,DTOType.ISA_Investigation, DTO.Spreadsheet investigationFile)
        investigationContract


let writeNewContracts (investigation: ArcInvestigation) =
    let contracts = ResizeArray()
    // root items
    let investigationContract = WriteContracts.createInvestigation investigation
    contracts.Add(investigationContract)
    contracts


writeNewContracts arc