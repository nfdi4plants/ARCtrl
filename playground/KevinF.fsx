#r "nuget: Fable.Core, 4.0.0"
#r "nuget: FsSpreadsheet, 2.0.2"
#r "nuget: FsSpreadsheet.ExcelIO, 2.0.2"
#r "nuget: Thoth.Json.Net, 11.0.0"
#I @"../src\ARCtrl/bin\Debug\netstandard2.0"
#r "ARCtrl.ISA.dll"
#r "ARCtrl.Contract.dll"
#r "ARCtrl.FileSystem.dll"
#r "ARCtrl.ISA.Spreadsheet.dll"
#r "ARCtrl.CWL.dll"
#r "ARCtrl.dll"

open ARCtrl
open ARCtrl.ISA
open ARCtrl.Templates
open ARCtrl.Templates.Json

let path = @"C:\Users\Kevin\source\repos\ARCtrl\playground"

let template: Template = 
    let table = ArcTable.init("My Table")
    table.AddColumn(CompositeHeader.Input IOType.Source, [|for i in 0 .. 9 do yield CompositeCell.createFreeText($"Source {i}")|])
    table.AddColumn(CompositeHeader.Output IOType.RawDataFile, [|for i in 0 .. 9 do yield CompositeCell.createFreeText($"Output {i}")|])
    let o = Template.init("MyTemplate")
    o.Table <- table
    o.Authors <- [|ARCtrl.ISA.Person.create(FirstName="John", LastName="Doe"); ARCtrl.ISA.Person.create(FirstName="Jane", LastName="Doe");|]
    o.EndpointRepositories <- [|ARCtrl.ISA.OntologyAnnotation.fromString "Test"; ARCtrl.ISA.OntologyAnnotation.fromString "Testing second"|]
    o

let json = template.ToJson()

let templateReResult = Template.ofJson(json)

match templateReResult with
| Ok templateRe -> 
    printfn "Can read in!"
    template = templateRe
| Error exn -> failwithf "Cannot read in: %s" exn

let writeFile() = System.IO.File.WriteAllText(path + @"/UNITTEST_Template.json", json)

writeFile()