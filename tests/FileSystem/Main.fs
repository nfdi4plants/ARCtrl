module ARCtrl.FileSystem.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "FileSystem" [
    ARCtrl.FileSystemTree.Tests.main
]

[<EntryPoint>]
let main argv = Pyxpecto.runTests [||] all
