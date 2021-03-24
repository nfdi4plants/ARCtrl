// #r "nuget: Newtonsoft.Json"
// #r "nuget: DynamicObj"
// #r @"C:\Users\Lukas\Downloads\Cyjs.NET-main\bin\Cyjs.NET\netstandard2.1\Cyjs.NET.dll"
#r "nuget: Cyjs.NET"
#r "nuget: ISADotNet.XLSX"

open ISADotNet
open ISADotNet.API
open ISADotNet.XLSX

open Cyjs.NET
open Elements

let source = __SOURCE_DIRECTORY__

let _,_,_,assay = AssayFile.AssayFile.fromFile (source + @"\content\files\AssayFile.xlsx")



let myFirstGraph = 
    CyGraph.initEmpty ()


let edges = 
    assay.ProcessSequence.Value 
    |> List.collect (fun p ->
        p.Outputs.Value
        |> List.zip p.Inputs.Value
        |> List.map (fun (i,o) -> ProcessInput.getName i |> Option.get,ProcessOutput.getName o |> Option.get,p.Name.Value)
    )
    



let cyNodes = 
    edges
    |> List.collect (fun (i,o,e) -> [i;o])
    |> List.distinct
    |> List.map (fun n -> node n [CyParam.label n])

let cyEgdes = 
    edges
    |> List.mapi (fun index (i,o,e) -> edge (string index) i o [CyParam.label e])

let myGraph = 
    CyGraph.initEmpty ()
    |> CyGraph.withElements cyNodes
    |> CyGraph.withElements cyEgdes
    |> CyGraph.withStyle "node"     
            [
                CyParam.content =. CyParam.label
                CyParam.color "#A00975"
            ]
    |> CyGraph.withStyle "edge"     
            [
                CyParam.content =. CyParam.label
                CyParam.color "#FF0000"
            ]
    |> CyGraph.withLayout (Layout.initBreadthfirst id) 
    |> CyGraph.withSize(800, 400) 

Layout.initBreadthfirst

myGraph
|> CyGraph.show        