module ARC.Path 

open ISA
open FileSystem

let [<Literal>] gitKeepFileName = ".gitkeep" 
let [<Literal>] assaysFolderName = "assays"
let [<Literal>] assayProtocolsFolderName = "protocols"
let [<Literal>] assayDatasetFolderName = "dataset"
let [<Literal>] AssayFileName = "isa.assay.xlsx"
let [<Literal>] StudyFileName = "isa.study.xlsx"
let [<Literal>] InvestigationFileName = "isa.investigation.xlsx"
let [<Literal>] assayReadmeFileName = "Readme.md"

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

