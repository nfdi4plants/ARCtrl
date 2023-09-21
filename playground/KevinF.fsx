#r "nuget: Fable.Core, 4.0.0"
#r "nuget: FsSpreadsheet, 2.0.2"
#r "nuget: FsSpreadsheet.ExcelIO, 2.0.2"
#r "nuget: Thoth.Json.Net, 11.0.0"
#r "nuget: Fable.SimpleHttp, 3.5.0"
#r "nuget: Fable.Promise, 3.2.0"
#r "nuget: Fable.Fetch, 2.6.0"
#I @"../src\ARCtrl/bin\Debug\netstandard2.0"
#r "ARCtrl.ISA.dll"
#r "ARCtrl.Contract.dll"
#r "ARCtrl.FileSystem.dll"
#r "ARCtrl.ISA.Spreadsheet.dll"
#r "ARCtrl.CWL.dll"
#r "ARCtrl.dll"

open Thoth.Json.Net

open ARCtrl
open ARCtrl.ISA
open ARCtrl.Templates
open ARCtrl.Templates.Json

let path = @"C:\Users\Kevin\source\repos\ARCtrl\playground"

let baseUrl = @"https://github.com/nfdi4plants/Swate-templates/releases/download/latest/templates.json"

let decoder : Thoth.Json.Net.Decoder<Map<string,Template>> = Thoth.Json.Net.Decode.dict Json.Template.decode
    
let dict (json: string) = 
    json
    |> Thoth.Json.Net.Decode.fromString decoder

let GetTemplates() =
    ARCtrl.WebRequest.downloadFile baseUrl (fun json ->
        let mapResult = dict json
        match mapResult with
        | Ok map -> printfn "%A" map.Count
        | Error exn -> failwith "Unable to parse json to template []"
    )
    |> Async.RunSynchronously

GetTemplates()