module ARCtrl.ROCrate.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "ROCrate" [
    Tests.LDObject.main
    Tests.Dataset.main
    Tests.Investigation.main
    Tests.Study.main
    Tests.Assay.main
    Tests.LabProcess.main
    Tests.LabProtocol.main
    Tests.Sample.main
    Tests.Data.main
    Tests.PropertyValue.main
    Tests.Person.main
    Tests.ScholarlyArticle.main
]

#if !TESTS_ALL
[<EntryPoint>]
#endif
let main argv = Pyxpecto.runTests [||] all
