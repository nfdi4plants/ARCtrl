#r "nuget: Fable.Core, 4.0.0"

//#I @"../src\ISA\ISA/bin\Debug\netstandard2.0"
#I @"../src\ARC/bin\Debug\netstandard2.0"
#r "ISA.dll"
#r "ARC.dll"
#r "FileSystem.dll"


open ISA
open ARC
open FileSystem

let [<Literal>] rootPath = @"C:\Users\Kevin\Desktop\TestARC"

open System
open System.IO

// https://github.com/nfdi4plants/arcIO.NET/blob/main/src/arcIO.NET/Assay.fs
let readFilePaths (arcPath: string) = 
    System.IO.Directory.EnumerateFiles(arcPath,"*",SearchOption.AllDirectories)

let filePaths = readFilePaths (rootPath) |> Array.ofSeq |> Array.map (fun p -> Path.GetRelativePath(rootPath, p))

module ARCPath =

    let [<Literal>] PathSeperator = '/'
    let [<Literal>] PathSeperatorWindows = '\\'
    let seperators = [|PathSeperator; PathSeperatorWindows|]

    let split(path: string) = 
        path.Split(seperators, enum<StringSplitOptions>(3))

    let combine (path1 : string) (path2 : string) : string = 
        let path1 = path1.TrimEnd(seperators)
        let path2 = path1.TrimStart(seperators)
        let combined = path1 + string PathSeperator + path2
        combined // should we trim any excessive path seperators?

    let combineMany (paths : string []) : string = 
        paths 
        |> Array.mapi (fun i p -> 
            if i = 0 then p.TrimEnd(seperators)
            elif i = (paths.Length-1) then p.TrimStart(seperators)
            else
                p.Trim(seperators)
        )
        |> String.concat(string PathSeperator)

let ofFilePaths (filePaths: string []) =
    let splitPaths = filePaths |> Array.map ARCPath.split
    let root = FileSystemTree.createFolder("root",[||])
    let rec loop (paths:string [] []) (parent: FileSystemTree) (ind:int) =
        if ind > 30 then 
            parent
        else
            let files = paths |> Array.filter (fun p -> p.Length = 1) |> Array.map (Array.head >> FileSystemTree.createFile)
            let folders = 
                paths 
                |> Array.filter (fun p -> p.Length > 1) 
                |> Array.groupBy(fun x -> Array.head x)
                |> Array.map (fun (folderName, children) -> 
                    let parent = FileSystemTree.createFolder(folderName, [||])
                    let children = children |> Array.map Array.tail
                    loop children parent (ind+1)
                )
            match parent with
            | Folder (name, _) -> FileSystemTree.createFolder(name, [|yield! files; yield! folders|])
            | x -> x
    loop splitPaths root 0

open FileSystem

let t = Folder("root",[|
    File "isa.investigation.xlsx"; 
    Folder(".arc", [|File ".gitkeep"|]);
    Folder(".git",[|
        File "config"; File "description"; File "HEAD";
        Folder("hooks",[|
            File "applypatch-msg.sample"; File "commit-msg.sample";
            File "fsmonitor-watchman.sample"; File "post-update.sample";
            File "pre-applypatch.sample"; File "pre-commit.sample";
            File "pre-merge-commit.sample"; File "pre-push.sample";
            File "pre-rebase.sample"; File "pre-receive.sample";
            File "prepare-commit-msg.sample";
            File "push-to-checkout.sample"; File "update.sample"
        |]);
        Folder ("info", [|File "exclude"|])
    |]);
    Folder("assays",[|
        File ".gitkeep";
        Folder("est",[|
            File "isa.assay.xlsx"; File "README.md";
            Folder ("dataset", [|File ".gitkeep"|]);
            Folder ("protocols", [|File ".gitkeep"|])
        |]);
        Folder
            ("TestAssay1",[|
            File "isa.assay.xlsx"; File "README.md";
            Folder ("dataset", [|File ".gitkeep"|]);
            Folder ("protocols", [|File ".gitkeep"|])
        |])
    |]);
    Folder("runs", [|File ".gitkeep"|]);
    Folder("studies",[|
        File ".gitkeep";
        Folder("est",[|
            File "isa.study.xlsx"; File "README.md";
            Folder ("protocols", [|File ".gitkeep"|]);
            Folder ("resources", [|File ".gitkeep"|])
        |]);
        Folder("MyStudy",[|
            File "isa.study.xlsx"; File "README.md";
            Folder ("protocols", [|File ".gitkeep"|]);
            Folder ("resources", [|File ".gitkeep"|])
        |]);
        Folder("TestAssay1",[|
            File "isa.study.xlsx"; File "README.md";
            Folder ("protocols", [|File ".gitkeep"|]);
            Folder ("resources", [|File ".gitkeep"|])
        |])
    |]);
    Folder ("workflows", [|File ".gitkeep"|])
|])

t = ofFilePaths (filePaths)