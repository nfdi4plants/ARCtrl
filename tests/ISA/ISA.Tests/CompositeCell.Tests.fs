module CompositeCell.Tests

open ARCtrl.ISA

open TestingUtils

let private tests_cellConverter = 
    testList "CellConverter" [
        testCase "FreeText toFreeText" (fun () ->
            let currentCell : CompositeCell = CompositeCell.FreeText "path/to/input"
            let asNewCell : CompositeCell = currentCell.ToFreeTextCell()
            let expected = currentCell
            Expect.equal asNewCell expected ""
        )
        testCase "FreeText toTerm" (fun () ->
            let currentCell : CompositeCell = CompositeCell.FreeText "path/to/input"
            let asNewCell : CompositeCell = currentCell.ToTermCell()
            let expected = CompositeCell.Term <| OntologyAnnotation.fromString "path/to/input"
            Expect.equal asNewCell expected ""
        )
        testCase "FreeText toUnitized" (fun () ->
            let currentCell : CompositeCell = CompositeCell.FreeText "path/to/input"
            let asNewCell : CompositeCell = currentCell.ToUnitizedCell()
            let expected = CompositeCell.Unitized <| ("", OntologyAnnotation.fromString "path/to/input")
            Expect.equal asNewCell expected ""
        )
        testCase "Term toFreeText" (fun () ->
            let oa = OntologyAnnotation.fromString("instrument model", "MS", "MS:000000042")
            let currentCell : CompositeCell = CompositeCell.Term oa
            let asNewCell : CompositeCell = currentCell.ToFreeTextCell()
            let expected = CompositeCell.FreeText oa.NameText
            Expect.equal asNewCell expected ""
        )
        testCase "Term toTerm" (fun () ->
            let currentCell : CompositeCell = CompositeCell.Term <| OntologyAnnotation.fromString("instrument model", "MS", "MS:000000042")
            let asNewCell : CompositeCell = currentCell.ToTermCell()
            let expected = currentCell
            Expect.equal asNewCell expected ""
        )
        testCase "Term toUnitized" (fun () ->
            let oa = OntologyAnnotation.fromString("instrument model", "MS", "MS:000000042")
            let currentCell : CompositeCell = CompositeCell.Term oa
            let asNewCell : CompositeCell = currentCell.ToUnitizedCell()
            let expected = CompositeCell.Unitized <| ("", oa)
            Expect.equal asNewCell expected ""
        )
        testCase "Unitized toFreeText" (fun () ->
            let oa = OntologyAnnotation.fromString("degree celsius", "UO", "UO:000000042")
            let currentCell : CompositeCell = CompositeCell.Unitized <| ("42", oa)
            let asNewCell : CompositeCell = currentCell.ToFreeTextCell()
            let expected = CompositeCell.FreeText oa.NameText
            Expect.equal asNewCell expected ""
        )
        testCase "Unitized toTerm" (fun () ->
            let oa = OntologyAnnotation.fromString("degree celsius", "UO", "UO:000000042")
            let currentCell : CompositeCell = CompositeCell.Unitized <| ("42", oa)
            let asNewCell : CompositeCell = currentCell.ToTermCell()
            let expected = CompositeCell.Term <| oa
            Expect.equal asNewCell expected ""
        )
        testCase "Unitized toUnitized" (fun () ->
            let oa = OntologyAnnotation.fromString("degree celsius", "UO", "UO:000000042")
            let currentCell : CompositeCell = CompositeCell.Unitized <| ("42", oa)
            let asNewCell : CompositeCell = currentCell.ToUnitizedCell()
            let expected = currentCell
            Expect.equal asNewCell expected ""
        )
    ]

let private tests_create = 
    testList "createX" [
        testCase "createFreeText" (fun () ->
            let newCell : CompositeCell = CompositeCell.createFreeText("Any important value")
            let expected = CompositeCell.FreeText "Any important value"
            Expect.equal newCell expected ""
        )
        testCase "createTerm" (fun () ->
            let oa = OntologyAnnotation.fromString("degree celsius", "UO", "UO:000000042")
            let newCell : CompositeCell = CompositeCell.createTerm(oa)
            let expected = CompositeCell.Term oa
            Expect.equal newCell expected ""
        )
        testCase "createTermFromString" (fun () ->
            let newCell : CompositeCell = CompositeCell.createTermFromString("degree celsius", "UO", "UO:000000042")
            let expected = CompositeCell.Term <| OntologyAnnotation.fromString("degree celsius", "UO", "UO:000000042")
            Expect.equal newCell expected ""
        )
        testCase "createUnitized_1" (fun () ->
            let newCell : CompositeCell = CompositeCell.createUnitized("42")
            let expected = CompositeCell.Unitized ("42", OntologyAnnotation.empty)
            Expect.equal newCell expected ""
        )
        testCase "createUnitized_2" (fun () ->
            let oa = OntologyAnnotation.fromString("degree celsius", "UO", "UO:000000042")
            let newCell : CompositeCell = CompositeCell.createUnitized("42", oa)
            let expected = CompositeCell.Unitized("42", oa)
            Expect.equal newCell expected ""
        )
        testCase "createUnitizedFromString_1" (fun () ->
            let newCell : CompositeCell = CompositeCell.createUnitizedFromString("42")
            let expected = CompositeCell.Unitized("42",OntologyAnnotation.empty)
            Expect.equal newCell expected ""
        )
        testCase "createUnitizedFromString_2" (fun () ->
            let oa = OntologyAnnotation.fromString("degree celsius", "UO", "UO:000000042")
            let newCell : CompositeCell = CompositeCell.createUnitizedFromString("42", "degree celsius", "UO", "UO:000000042")
            let expected = CompositeCell.Unitized("42", oa)
            Expect.equal newCell expected ""
        )
    ]

let private tests_ToString = testList "ToString" [
    testCase "FreeText" <| fun _ ->
        let cc = CompositeCell.createFreeText "My Text"
        let actual = cc.ToString()
        let expected = "My Text"
        Expect.equal actual expected ""
    testCase "Term" <| fun _ ->
        let cc = CompositeCell.createTermFromString("instrument model", "MS", "MS:0000000")
        let actual = cc.ToString()
        let expected = "instrument model"
        Expect.equal actual expected ""
    testCase "Unitized" <| fun _ ->
        let cc = CompositeCell.createUnitizedFromString("20", "degree celcius", "UO", "UO:0000000")
        let actual = cc.ToString()
        let expected = "20 degree celcius"
        Expect.equal actual expected ""
]

let private tests_GetContent = testList "GetContent" [
    testCase "FreeText" <| fun _ ->
        let cell = CompositeCell.createFreeText "Any Text"
        let actual = cell.GetContent()
        Expect.equal actual [|"Any Text"|] ""
    testCase "Term" <| fun _ ->
        let cell = CompositeCell.createTerm (OntologyAnnotation.fromString "My OA Name")
        let actual = cell.GetContent()
        Expect.equal actual [|"My OA Name"; ""; ""|] ""
    testCase "Unitized" <| fun _ ->
        let cell = CompositeCell.createUnitized ("12", OntologyAnnotation.fromString "My Unit Name")
        let actual = cell.GetContent()
        Expect.equal actual [|"12"; "My Unit Name"; ""; ""|] ""
    testCase "Matching" <| fun _ ->
        let matchContent (content: string []) =
            match content with
            | [|ft|] -> "FreeText"
            | [|name; tsr; tan|] -> "Term"
            | [|v; name; tsr; tan|] -> "Unitized"
            | _ -> failwith "Not allowed"
        Expect.equal (matchContent [|"Some ft"|]) "FreeText" "FreeText"
        Expect.equal (matchContent [|""; ""; ""|]) "Term" "Term"
        Expect.equal (matchContent [|""; ""; ""; ""|]) "Unitized" "Unitized"
        Expect.throws (fun _ -> matchContent [|""; ""|] |> ignore) "Should throw"

]

let main = 
    testList "CompositeCell" [
        tests_cellConverter
        tests_create
        tests_ToString
        tests_GetContent
    ]