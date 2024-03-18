#r "nuget: FsSpreadsheet.Net"
#r "nuget: ARCtrl"
#load "Contracts.fsx"

// # Create

open ARCtrl

/// Init a new empty ARC
let arc = ARC()

arc.FileSystem

// # Write

open ARCtrl.Contract
open FsSpreadsheet
open FsSpreadsheet.Net

let arcRootPath = @"C:\Users\Kevin\Desktop\NewTestARCNET"

let write (arcPath: string) (arc:ARC) =
    arc.GetWriteContracts()
    // `Contracts.fulfillWriteContract` from Contracts.fsx docs
    |> Array.iter (Contracts.fulfillWriteContract arcPath)

write arcRootPath arc

// # Read
open System.IO

// Setup

let normalizePathSeparators (str:string) = str.Replace("\\","/")

let getAllFilePaths (basePath: string) =
    let options = EnumerationOptions()
    options.RecurseSubdirectories <- true
    Directory.EnumerateFiles(basePath, "*", options)
    |> Seq.map (fun fp ->
        Path.GetRelativePath(basePath, fp)
        |> normalizePathSeparators
    )
    |> Array.ofSeq

// put it all together
let readARC(basePath: string) =
    // Get all file paths for a given ARC folder
    let allFilePaths = getAllFilePaths basePath
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

// Execution

readARC arcRootPath