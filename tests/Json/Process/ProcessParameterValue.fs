module Tests.Process.ProcessParameterValue

open ARCtrl
open ARCtrl.Process
open ARCtrl.Json
open TestingUtils
open TestObjects.Json

let private tests_roCrate =
    testList "RO-Crate" [
        testCase "ReadWriteIntegerValue" (fun () -> 
            let pp = ProtocolParameter.create(ParameterName = OntologyAnnotation("temperature","NCIT","http://purl.obolibrary.org/obo/NCIT_0000029"))
            let value = Value.Int 25
            let unit = OntologyAnnotation("degree Celsius","UO","http://purl.obolibrary.org/obo/UO_0000185")
            let ppv = ProcessParameterValue.create(pp,value,unit)

            let roCrate = ProcessParameterValue.toROCrateJsonString() ppv
            let ppv2 = ProcessParameterValue.fromROCrateJsonString roCrate

            Expect.equal ppv ppv2 "RO-Crate roundtrip failed"
        )
        testCase "ReadWriteOntologyValue" (fun () -> 
            let pp = ProtocolParameter.create(ParameterName = OntologyAnnotation("organism","NCIT","http://purl.obolibrary.org/obo/NCIT_0000029"))
            let value = Value.Ontology (OntologyAnnotation(
                "chlamydomonas reinhardtii",
                 "NCIT",
                 "http://purl.obolibrary.org/obo/NCIT_0000030"
            ))
            let ppv = ProcessParameterValue.create(pp,value)

            let roCrate = ProcessParameterValue.toROCrateJsonString() ppv
            let ppv2 = ProcessParameterValue.fromROCrateJsonString roCrate

            Expect.equal ppv ppv2 "RO-Crate roundtrip failed"
        )
    ]

let main = testList "ProcessParameterValue" [
    tests_roCrate
]