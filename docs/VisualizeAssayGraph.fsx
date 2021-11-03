// #r "nuget: Newtonsoft.Json"
// #r "nuget: DynamicObj"
// #r @"C:\Users\Lukas\Downloads\Cyjs.NET-main\bin\Cyjs.NET\netstandard2.1\Cyjs.NET.dll"
#r "nuget: Cyjs.NET"
#r "nuget: System.Text.Json"
#r "nuget: FSharpSpreadsheetML"

#r "nuget: FSharp.SystemTextJson"


#r @"D:\Repos\ISADotNet\bin\ISADotNet.XLSX\netstandard2.0\ISADotNet.dll"
#r @"D:\Repos\ISADotNet\bin\ISADotNet.XLSX\netstandard2.0\ISADotNet.XLSX.dll"
#r @"D:\Repos\ISADotNet\bin\ISADotNet.Viz\netstandard2.0\ISADotNet.Viz.dll"

open ISADotNet
open ISADotNet.API
open ISADotNet.XLSX
open ISADotNet.Viz

open Cyjs.NET
open Elements



open Cyjs.NET
open Elements

let source = __SOURCE_DIRECTORY__

let _,_,_,assay = AssayFile.Assay.fromFile (source + @"\content\files\AssayFile.xlsx")


DAG.fromProcessSequence (assay.ProcessSequence.Value,Schema = Schema.SwateGreen)
|> DAG.show