#r "nuget: FsSpreadsheet, 2.0.1"
#r "nuget: FsSpreadsheet.ExcelIO, 1.2.0-preview"
#r "nuget: Fable.Core, 4.0.0"
#r "nuget: ISADotNet.XLSX"

#I @"C:\Users\HLWei\source\repos\ARC_tools\arcAPI\src\ARC\bin\Debug\netstandard2.0"
#r "ARC.dll"
#r "CWL.dll"
#r "FileSystem.dll"
#r "ISA.dll"

open ARC
open FileSystem
open ISA
open System.IO

let arcPath = Path.Combine(__SOURCE_DIRECTORY__, "testArc")

// Function to recursively read file system and return a FileSystemTree
let rec readFileSystem (path : string) : FileSystemTree =
    let folderName = DirectoryInfo(path).Name
    let fileName file = FileInfo(file).Name
    let files = Directory.GetFiles(path)
    let folders = Directory.GetDirectories(path)
    let children = 
        folders
        |> Array.map (fun folder -> readFileSystem folder)
        |> Array.append (files |> Array.map (fun file -> fileName file |> FileSystemTree.createFile))
    FileSystemTree.createFolder(folderName, children)

open FsSpreadsheet.ExcelIO
open FsSpreadsheet

let readArc (path : string) : ARC =
    let fs = readFileSystem arcPath
    //let arc = ARC.create(Investigation.create(), (), FileSystem.create(fs,[||]))

    let isaItems = 
        ARC.getContractsForFileTree fs
        //ARC.getISAContracts arc
        |> Array.map (fun isaContract -> 
            let workbook = 
                FsWorkbook.fromXlsxFile isaContract.Path
            {isaContract with
                Data = Some(Spreadsheet(workbook))
            }
        )

    let arcWithIsa = ARC.fillContracts isaItems fs


    //let assay = Assay.create()
    //let arcWithAssay = ARC.addAssay assay "test" arcWithIsa

    //let outContracts = ARC.getContracts arcWithAssay

    //outContracts
    //|> Array.map (fun c ->
    //    ()
    //) |> ignore
    
    raise (System.NotImplementedException())
