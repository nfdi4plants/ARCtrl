module ARC.Path 

open ISA
open FileSystem

// ->
//namespace ARC
//
// module Path =
// 

let [<Literal>] gitKeepFileName = ".gitkeep" 

let [<Literal>] assaysFolderName = "assays"
let [<Literal>] assayProtocolsFolderName = "protocols"
let [<Literal>] assayDatasetFolderName = "dataset"
let [<Literal>] assayFileName = "isa.assay.xlsx"
let [<Literal>] assayReadmeFileName = "Readme.md"

let assaySubFolderNames = [|assayDatasetFolderName;assayProtocolsFolderName|]


//let combineAssayFolderPath (assay : ISA.Assay) = 
//    Assay.getIdentifier assay
//    |> Path.combine assaysFolderName

//let combineAssayProtocolsFolderPath (assay : ISA.Assay) = 
//    let assayFolder = combineAssayFolderPath assay
//    Path.combine assayFolder assayProtocolsFolderName

//let combineAssayDatasetFolderPath (assay : ISA.Assay) = 
//    let assayFolder = combineAssayFolderPath assay
//    Path.combine assayFolder assayDatasetFolderName

//let combineAssaySubfolderPaths (assay : ISA.Assay) = 
//    let assayFolder = combineAssayFolderPath assay
//    assaySubFolderNames
//    |> Array.map (fun subfolderName -> Path.combine assayFolder subfolderName)

