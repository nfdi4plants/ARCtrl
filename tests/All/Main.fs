module Main.Tests

open Fable.Pyxpecto

let all = testList "All" [
    
    ARCtrl.Core.Tests.all
    ARCtrl.Json.Tests.all
    ARCtrl.Spreadsheet.Tests.all
    ARCtrl.FileSystem.Tests.all
    ARCtrl.ARC.Tests.all
    ARCtrl.Yaml.Tests.all
    ARCtrl.ValidationPackages.Tests.all
    ARCtrl.Contract.Tests.all
    ARCtrl.ROCrate.Tests.all
    
]

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all