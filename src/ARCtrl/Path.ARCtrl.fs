module Path.ARCtrl

open ISA
open FileSystem

// Files
let [<Literal>] GitKeepFileName = ".gitkeep" 
let [<Literal>] READMEFileName = "README.md"

// Folder
let [<Literal>] AssayProtocolsFolderName = "protocols"
let [<Literal>] AssayDatasetFolderName = "dataset"

//let assaySubFolderNames = [|assayDatasetFolderName;assayProtocolsFolderName|]

//let combineAssayFolderPath (assay : ISA.ArcAssay) = 
//    //Assay.getIdentifier assay
//    //|> Path.combine assaysFolderName
//    raise (System.NotImplementedException())

//let combineAssayProtocolsFolderPath (assay : ISA.ArcAssay) = 
//    let assayFolder = combineAssayFolderPath assay
//    Path.combine assayFolder assayProtocolsFolderName

//let combineAssayDatasetFolderPath (assay : ISA.ArcAssay) = 
//    let assayFolder = combineAssayFolderPath assay
//    Path.combine assayFolder assayDatasetFolderName

//let combineAssaySubfolderPaths (assay : ISA.ArcAssay) = 
//    let assayFolder = combineAssayFolderPath assay
//    assaySubFolderNames
//    |> Array.map (fun subfolderName -> Path.combine assayFolder subfolderName)

