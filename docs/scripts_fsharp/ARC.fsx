#r "nuget: FsSpreadsheet.ExcelIO, 4.1.0"
#r "nuget: ARCtrl, 1.0.0-alpha9"
#load "Contracts.fsx"

// # Create

open ARCtrl

/// Init a new empty ARC
let arc = ARC()

arc.FileSystem

// # Write

open ARCtrl.Contract
open FsSpreadsheet
open FsSpreadsheet.ExcelIO

let arcRootPath = @"C:\Users\Kevin\Desktop\NewTestARCNET"

let write (arcPath: string) (arc:ARC) =
    arc.GetWriteContracts()
    // `Contracts.fulfillWriteContract` from Contracts.fsx docs
    |> Array.iter (Contracts.fulfillWriteContract arcPath)

write arcRootPath arc

// # Read
open System.IO

// put it all together
let readARC(basePath: string) =
    // `Contracts.getAllFilePaths` from Contracts.fsx docs
    let allFilePaths = Contracts.getAllFilePaths basePath
    // Initiates an ARC from FileSystem but no ISA info.
    let arcRead = ARC.fromFilePaths allFilePaths
    // Read contracts will tell us what we need to read from disc.
    let readContracts = arcRead.GetReadContracts()
    let fulfilledContracts = 
        readContracts 
        // `Contracts.fulfillReadContract` from Contracts.fsx docs
        |> Array.map (Contracts.fulfillReadContract basePath) 
    arcRead.SetISAFromContracts(fulfilledContracts)
    arcRead 

// execution

readARC arcRootPath