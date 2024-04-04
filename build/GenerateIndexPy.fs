module GenerateIndexPy

open System
open System.IO
open System.Text.RegularExpressions


open System.Text

let createImportStatement path (classes : string []) =
    let classes = classes |> Array.reduce (fun acc x -> acc + ", " + x)
    sprintf "from %s import %s" path classes

let writePyIndexfile (path: string) (content: string) =
    let filePath = Path.Combine(path, "arctrl.py")
    File.WriteAllText(filePath, content)

let generateIndexFileContent (classes : (string*string) []) =
    classes
    |> Array.groupBy fst
    |> Array.map (fun (p,a) -> createImportStatement p (a |> Array.map snd))

let classes =
    [|
        "__future__", "annotations"
        "collections.abc", "Callable"
        "typing", "Any"
        ".Core.comment", "Comment"
        ".Core.ontology_annotation","OntologyAnnotation"; 
        ".Core.person", "Person";
        ".Core.publication", "Publication";
        ".Core.Table.composite_header", "IOType"
        ".Core.Table.composite_header", "CompositeHeader"; 
        ".Core.Table.composite_cell", "CompositeCell"
        ".Core.Table.composite_column", "CompositeColumn"
        ".Core.Table.arc_table", "ArcTable"
        ".Core.arc_types", "ArcAssay"; 
        ".Core.arc_types", "ArcStudy"; 
        ".Core.arc_types", "ArcInvestigation"; 
        ".Core.template", "Template"
        ".json", "JsonController"
        ".xlsx", "XlsxController"
        ".arc","ARC"
    |]

let ARCtrl_generate (rootPath: string) = 
    generateIndexFileContent classes
    |> Array.reduce (fun a b -> a + "\n" + b)
    |> writePyIndexfile rootPath