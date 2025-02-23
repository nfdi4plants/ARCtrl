module ARCtrl.ARC.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "ARCtrl" [
    ARCtrl.ROCrateConversion.Tests.main
    ARCtrl.CrossAsync.Tests.main
    ARCtrl.FileSystemHelper.Tests.main
    ARCtrl.ContractIO.Tests.main
    ARCtrl.Contracts.Tests.main
    ARCtrl.WebRequest.Tests.main
    ARCtrl.SemVer.Tests.main
    ARCtrl.Template.Tests.main
    ARCtrl.ValidationPackagesConfig.Tests.main
    ARCtrl.Tests.main
]

#if !TESTS_ALL
[<EntryPoint>]
#endif
let main argv = Pyxpecto.runTests [||] all