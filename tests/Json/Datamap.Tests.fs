module Tests.Datamap

open ARCtrl
open ARCtrl.Json
open TestingUtils

module Helper =
    let create_empty() = Datamap.init()
    let create_Datacontext (i:int) =
        DataContext(
            $"id_string_{i}",
            "My Name",
            DataFile.DerivedDataFile,
            "My Format",
            "My Selector Format",
            OntologyAnnotation("Explication", "MS", "MS:123456"),
            OntologyAnnotation("Unit", "MS", "MS:123456"),
            OntologyAnnotation("ObjectType", "MS", "MS:123456"),
            "My Label",
            "My Description",
            "Kevin F",
            (ResizeArray [Comment.create("Hello", "World")])
        )

    let create_filled() = 
        Datamap(ResizeArray [
            for i in 1 .. 3 do
                create_Datacontext i
        ])

    let compare =
        fun (d1: Datamap) (d2: Datamap) ->
            let d1DataContexts =
                d1.DataContexts
                |> Array.ofSeq
            let d2DataContexts =
                d2.DataContexts
                |> Array.ofSeq

            Expect.equal d1DataContexts.Length d2DataContexts.Length "Datamap Length"

            for i in 0..d1DataContexts.Length - 1 do

                Expect.equal d1DataContexts.[i] d2DataContexts.[i] $"Datamap is not equal on {i}"

            Expect.sequenceEqual d1.DataContexts d2.DataContexts "DataContexts"
        |> Some

open Helper

let private test_datamapJsonTesting =
    createBaseJsonTests
        "datamap-json"
        create_filled
        Datamap.toJsonString
        Datamap.fromJsonString
        None
        compare

let main = testList "Datamap" [
    test_datamapJsonTesting
]
