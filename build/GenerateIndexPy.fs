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
        ".ISA.ISA.JsonTypes.comment", "Comment"
        ".ISA.ISA.JsonTypes.ontology_annotation","OntologyAnnotation"; 
        ".ISA.ISA.JsonTypes.person", "Person";
        ".ISA.ISA.JsonTypes.publication", "Publication";
        ".ISA.ISA.ArcTypes.composite_header", "IOType"
        ".ISA.ISA.ArcTypes.composite_header", "CompositeHeader"; 
        ".ISA.ISA.ArcTypes.composite_cell", "CompositeCell"
        ".ISA.ISA.ArcTypes.composite_column", "CompositeColumn"
        ".ISA.ISA.ArcTypes.arc_table", "ArcTable"
        ".ISA.ISA.ArcTypes.arc_types", "ArcAssay"; 
        ".ISA.ISA.ArcTypes.arc_types", "ArcStudy"; 
        ".ISA.ISA.ArcTypes.arc_types", "ArcInvestigation"; 
        ".Templates.template", "Template"
        ".Templates.templates", "Templates"
        ".Templates.template", "Organisation"
        ".arc","ARC"
    |]

let ARCtrl_generate (rootPath: string) = 
    generateIndexFileContent classes
    |> Array.reduce (fun a b -> a + "\n" + b)
    |> writePyIndexfile rootPath