module CompositeHeader.Tests

open ISA

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif

let tests_iotype = 
    testList "IOType" [
        testCase "asInput" (fun () ->
            Expect.equal Source.asInput "Input [Source Name]" "Source"
            Expect.equal Sample.asInput "Input [Sample Name]" "Sample"
            Expect.equal RawDataFile.asInput "Input [Raw Data File]" "Raw Data File"
            Expect.equal DerivedDataFile.asInput "Input [Derived Data File]" "Derived Data File"
            Expect.equal ImageFile.asInput "Input [Image File]" "Image File"
            Expect.equal Material.asInput "Input [Material]" "Material"
            Expect.equal (IOType.FreeText "Test").asInput "Input [Test]" "FreeText Test"
        )
        testCase "asOutput" (fun () ->
            Expect.equal Source.asOutput "Output [Source Name]" "Source"
            Expect.equal Sample.asOutput "Output [Sample Name]" "Sample"
            Expect.equal RawDataFile.asOutput "Output [Raw Data File]" "Raw Data File"
            Expect.equal DerivedDataFile.asOutput "Output [Derived Data File]" "Derived Data File"
            Expect.equal ImageFile.asOutput "Output [Image File]" "Image File"
            Expect.equal Material.asOutput "Output [Material]" "Material"
            Expect.equal (IOType.FreeText "Test").asOutput "Output [Test]" "FreeText Test"
        )
        // This test ensures that new IOTypes are also added to `All` static member.
        testCase "All" (fun () -> 
            let count = Microsoft.FSharp.Reflection.FSharpType.GetUnionCases(typeof<IOType>) |> Array.length
            Expect.hasLength IOType.All (count-1) "Expect one less than all because we do not want to track `FreeText` case."
        )
    ]

let tests_compositeHeader =
    testList "CompositeHeader" [
        testList "ToString()" [
            testCase "Characteristic" (fun () -> 
                let header = Characteristics <| OntologyAnnotation.fromString("species", "MS", "MS:0000042")
                let actual = header.ToString()
                let expected = "Characteristics [species]"
                Expect.equal actual expected ""
            )
            testCase "Parameter" (fun () -> 
                let header = Parameter <| OntologyAnnotation.fromString("centrifugation time", "MS", "MS:0000042")
                let actual = header.ToString()
                let expected = "Parameter [centrifugation time]"
                Expect.equal actual expected ""
            )
            testCase "Factor" (fun () -> 
                let header = Factor <| OntologyAnnotation.fromString("growth temperature", "MS", "MS:0000042")
                let actual = header.ToString()
                let expected = "Factor [growth temperature]"
                Expect.equal actual expected ""
            )
            testCase "Component" (fun () -> 
                let header = Component <| OntologyAnnotation.fromString("instrument model", "MS", "MS:0000042")
                let actual = header.ToString()
                let expected = "Component [instrument model]"
                Expect.equal actual expected ""
            )
            testCase "ProtocolType" (fun () -> 
                let header = ProtocolType
                let actual = header.ToString()
                let expected = "Protocol Type"
                Expect.equal actual expected ""
            )
            testCase "Input Source" (fun () -> 
                let header = Input Source
                let actual = header.ToString()
                let expected = "Input [Source Name]"
                Expect.equal actual expected ""
            )
            testCase "Output ImageFile" (fun () -> 
                let header = Input ImageFile
                let actual = header.ToString()
                let expected = "Input [Image File]"
                Expect.equal actual expected ""
            )
        ]
        testList "OfHeaderString" [
            testList "TermColumns" [
                testCase "Characteristic" (fun () ->
                    let headerString = "Characteristic [species]"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Characteristics <| OntologyAnnotation.fromString("species")
                    Expect.equal actual expected ""
                )
                testCase "Characteristics" (fun () ->
                    let headerString = "Characteristics [species]"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Characteristics <| OntologyAnnotation.fromString("species")
                    Expect.equal actual expected ""
                )
                testCase "Characteristics Value" (fun () ->
                    let headerString = "Characteristics Value [species]"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Characteristics <| OntologyAnnotation.fromString("species")
                    Expect.equal actual expected ""
                )
                testCase "Parameter" (fun () ->
                    let headerString = "Parameter [centrifugation time]"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Parameter <| OntologyAnnotation.fromString("centrifugation time")
                    Expect.equal actual expected ""
                )
                testCase "Parameter Value" (fun () ->
                    let headerString = "Parameter Value [centrifugation time]"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Parameter <| OntologyAnnotation.fromString("centrifugation time")
                    Expect.equal actual expected ""
                )
                testCase "Factor" (fun () ->
                    let headerString = "Factor [temperature]"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Factor <| OntologyAnnotation.fromString("temperature")
                    Expect.equal actual expected ""
                )
                testCase "Factor Value" (fun () ->
                    let headerString = "Factor Value [temperature]"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Factor <| OntologyAnnotation.fromString("temperature")
                    Expect.equal actual expected ""
                )
                testCase "Component" (fun () ->
                    let headerString = "Component [temperature]"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Component <| OntologyAnnotation.fromString("temperature")
                    Expect.equal actual expected ""
                )
            ]
            testList "IoColumns" [
                testCase "Input [Source]" (fun () ->
                    let headerString = "Input [Source]"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Input Source
                    Expect.equal actual expected ""
                )
                testCase "Input [Source Name]" (fun () ->
                    let headerString = "Input [Source Name]"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Input Source
                    Expect.equal actual expected ""
                )
                testCase "Output [Source Name]" (fun () ->
                    let headerString = "Output [Source Name]"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Output Source
                    Expect.equal actual expected ""
                )
                testCase "Input loop" (fun () ->
                    let testForLoop (io:IOType) =      
                        let headerString = io.asInput
                        let actual = CompositeHeader.OfHeaderString headerString
                        let expected = Input io
                        Expect.equal actual expected $"""Faulty "to string -> back to IOType" loop for '{io}'"""
                    IOType.All
                    |> Array.iter testForLoop
                )
                testCase "Output loop" (fun () ->
                    let testForLoop (io:IOType) =      
                        let headerString = io.asOutput
                        let actual = CompositeHeader.OfHeaderString headerString
                        let expected = Output io
                        Expect.equal actual expected $"""Faulty "to string -> back to IOType" loop for '{io}'"""
                    IOType.All
                    |> Array.iter testForLoop
                )
                testCase "Input FreeText" (fun () ->
                    let headerString = "Input [Anything]"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Input (IOType.FreeText "Anything")
                    Expect.equal actual expected ""
                )
                testCase "Input loop FreeText" (fun () ->
                    let headerString = (IOType.FreeText "Anything").asInput
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Input (IOType.FreeText "Anything")
                    Expect.equal actual expected ""
                )
                testCase "Output FreeText" (fun () ->
                    let headerString = "Output [Anything]"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Output (IOType.FreeText "Anything")
                    Expect.equal actual expected ""
                )
                testCase "Output loop FreeText" (fun () ->
                    let headerString = (IOType.FreeText "Anything").asOutput
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Output (IOType.FreeText "Anything")
                    Expect.equal actual expected ""
                )
            ]
            testList "FeaturedColumns" [
                testCase "ProtocolType" (fun () ->
                    let headerString = "Protocol Type"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = ProtocolType
                    Expect.equal actual expected ""
                )
            ]
            testList "SingleColumns" [
                testCase "ProtocolREF" (fun () ->
                    let headerString = "Protocol REF"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = ProtocolREF
                    Expect.equal actual expected ""
                )
                testCase "ProtocolDescription" (fun () ->
                    let headerString = "Protocol Description"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = ProtocolDescription
                    Expect.equal actual expected ""
                )
                testCase "ProtocolUri" (fun () ->
                    let headerString = "Protocol Uri"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = ProtocolUri
                    Expect.equal actual expected ""
                )
                testCase "ProtocolVersion" (fun () ->
                    let headerString = "Protocol Version"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = ProtocolVersion
                    Expect.equal actual expected ""
                )
                testCase "Performer" (fun () ->
                    let headerString = "Performer"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Performer
                    Expect.equal actual expected ""
                )
                testCase "Date" (fun () ->
                    let headerString = "Date"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = Date
                    Expect.equal actual expected ""
                )
            ]
            testList "FreeText" [
                testCase "Anyone important header" (fun () ->
                    let headerString = "Anyone important header"
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = FreeText "Anyone important header"
                    Expect.equal actual expected ""
                )
                testCase "Anyone important header-loop" (fun () ->
                    let headerString = (FreeText "Anyone important header").ToString()
                    let actual = CompositeHeader.OfHeaderString headerString
                    let expected = FreeText "Anyone important header"
                    Expect.equal actual expected ""
                )
            ]
        ]
    ]

let main = 
    testList "CompositeHeader" [
        tests_iotype
        tests_compositeHeader
    ]