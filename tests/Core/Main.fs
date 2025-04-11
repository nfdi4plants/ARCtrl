module ARCtrl.Core.Tests

open Fable.Pyxpecto

let all = testSequenced <| testList "Core" [
    UriHelper.Tests.main
    DataModel.Tests.main
    Comment.Tests.main
    OntologyAnnotation.Tests.main
    Regex.Tests.main
    Person.Tests.main
    CompositeHeader.Tests.main
    Data.Tests.main
    CompositeCell.Tests.main
    CompositeColumn.Tests.main
    ArcTables.Tests.main
    ArcTable.Tests.main
    ArcAssay.Tests.main
    ArcStudy.Tests.main
    ArcWorkflow.Tests.main
    ArcRun.Tests.main
    ArcInvestigation.Tests.main
    Template.Tests.main
    ArcJsonConversion.Tests.main
    Identifier.Tests.main
    Fable.Tests.main
]

#if !TESTS_ALL
[<EntryPoint>]
#endif
let main argv = Pyxpecto.runTests [||] all