module ARCtrl.FileSystem.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "FileSystem" [
    ARCtrl.FileSystemTree.Tests.main
]

#if !TESTS_ALL
[<EntryPoint>]
#endif
let main argv = Pyxpecto.runTests [||] all
