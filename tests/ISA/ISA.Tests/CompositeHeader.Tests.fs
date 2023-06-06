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
            testCase "Characteristics Value" (fun () ->
                let headerString = "Characteristics Value [species]"
                match headerString.Trim() with
                // Is term column
                | Regex.Aux.Regex Regex.Pattern.TermColumnPattern r ->
                    Expect.isTrue false "This should happen"
                | _ -> Expect.isTrue true ""
                let actual = CompositeHeader.OfHeaderString headerString
                let expected = Characteristics <| OntologyAnnotation.fromString("species")
                Expect.equal actual expected ""
            )
        ]
    ]

let main = 
    testList "CompositeHeader" [
        tests_iotype
        tests_compositeHeader
    ]

