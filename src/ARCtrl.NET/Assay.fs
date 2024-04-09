namespace ARCtrl.NET

open ARCtrl
open ARCtrl
open ARCtrl.Spreadsheet
open FsSpreadsheet
open FsSpreadsheet.Net
open System.IO 

module Assay = 

    module AssayFolder =
        
        /// Checks if an assay folder exists in the ARC.
        let exists (arc : string) (identifier : string) =
            
            Path.Combine([|arc;ARCtrl.Path.AssaysFolderName;identifier|])
            |> System.IO.Directory.Exists


    let readByFileName (arc : string) (assayFileName : string) =
        Path.Combine([|arc;assayFileName|]).Replace(@"\","/")
        |> FsWorkbook.fromXlsxFile
        |> ArcAssay.fromFsWorkbook


    let readByIdentifier (arc : string) (identifier : string) =
        ARCtrl.Helper.Identifier.Assay.fileNameFromIdentifier identifier
        |> readByFileName arc

    //let tryReadFromFolder (folderPath : string) =
    //    try 
    //        readFromFolder folderPath |> Some
    //    with | _ -> None

    //let tryReadByFileName (arc : string) (assayFileName : string) =
    //    try 
    //        readByFileName arc assayFileName |> Some
    //    with | _ -> None

    //let tryReadByName (arc : string) (assayName : string) =
    //    try 
    //        readByName arc assayName |> Some
    //    with | _ -> None

    //let writeToFolder (folderPath : string) (contacts : Person list) (assay : Assay) =
    //    let ap = Path.Combine (folderPath,assayFileName)
    //    AssayFile.Assay.toFile ap contacts assay        

    //let write (arc : string) (contacts : Person list) (assay : Assay) =
    //    if assay.FileName.IsNone then
    //        failwith "Cannot write assay to arc, as it has no filename"
    //    let ap = Path.Combine ([|arc;"assays";assay.FileName.Value|])
    //    AssayFile.Assay.toFile ap contacts assay  


    //let add (arc : string) (assay : ArcAssay) =
        
    //    if assay.FileName.IsNone then
    //        failwith "Given assay does not contain filename"

    //    let assayIdentifier = identifierFromFileName assay.FileName.Value

    //    if AssayFolder.exists arc assayIdentifier then
    //        printfn $"Assay folder with identifier {assayIdentifier} already exists."
    //    else
    //        subFolderPaths 
    //        |> List.iter (fun n ->
    //            let dp = Path.Combine([|arc;rootFolderName;assayIdentifier;n|])
    //            let dir = Directory.CreateDirectory(dp)
    //            File.Create(Path.Combine(dir.FullName, ".gitkeep")).Close()
    //        )

    //        let assayFilePath = Path.Combine([|arc;rootFolderName;assay.FileName.Value|])

    //        AssayFile.Assay.toFile assayFilePath [] assay


    //let init (arc : string) (assayName : string) =

    //    let assay = Assay.create(FileName = nameToFileName assayName)

    //    init arc assay

