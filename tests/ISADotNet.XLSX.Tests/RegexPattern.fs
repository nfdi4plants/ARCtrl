module RegexPatternTests

open Expecto
open TestingUtils

open ISADotNet.XLSX.AssayFile.AnnotationColumn.RegexPattern
open System.Text.RegularExpressions

//let kindPattern = @"[^\[(]*(?= [\[\(])"
//let namePattern = @"(?<= \[).*(?=[\]])"
//let ontologySourcePattern = @"(?<=\()\S+:[^;)#]*(?=[\)\#])"
//let numberPattern = @"(?<=#)\d+(?=[\)\]])"

/// Patterns found at ISADotNet.XLSX.AssayFile, AnnotationColumn.RegexPattern

//(?<=\()
//(?=[\)\#])

module TestCases =
    [<Literal>]
    let case1 = "Source Name"
    [<Literal>]
    let case2 = "Sample Name"
    [<Literal>]
    let case3 = "Characteristics [Sample type]"
    [<Literal>]
    let case5 = "Factor [Sample type#2]"
    [<Literal>]
    let case6 = "Parameter [biological replicate#2]"
    [<Literal>]
    let case7 = "Data File Name"
    [<Literal>]
    let case8 = "Term Source REF (NFDI4PSO:0000064)"
    [<Literal>]
    let case9 = "Term Source REF (NFDI4PSO:0000064#2)"
    [<Literal>]
    let case10 = "Term Accession Number (MS:1001809)"
    [<Literal>]
    let case11 = "Term Accession Number (MS:1001809#2)"
    [<Literal>]
    let case12 = "Unit"
    [<Literal>]
    let case13 = "Unit (#3)"
    [<Literal>]
    let case14 = "Term Accession Number ()" 
    [<Literal>]
    let case15 = "Parameter [hello world]"
    [<Literal>]
    let case16 = "Parameter [hello (world)]"
    [<Literal>]
    let case17 = "Parameter [hello[world]]"
    [<Literal>]
    let case18 = "Parameter [hello ]]"
    [<Literal>]
    let case19 = "http://purl.obolibrary.org/obo/NFDI4PSO_0000064"
    [<Literal>]
    let case20 = "Term Accession Number (#2)"

[<Tests>]
let testRegexPatternMatching = 
    testList "Regex pattern tests" [
        test "kindPattern 'Source Name'" {
            let regExMatch = Regex.Match(TestCases.case1, columnTypePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "Source Name" ""
        }

        test "kindPattern 'Characteristic [Sample type]'" {
            let regExMatch = Regex.Match(TestCases.case3, columnTypePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "Characteristics" ""
        }

        test "kindPattern 'Term Source REF (NFDI4PSO:0000064)'" {
            let regExMatch = Regex.Match(TestCases.case8, columnTypePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "Term Source REF" ""
        }

        test "kindPattern 'Term Accession Number (MS:1001809#2)'" {
            let regExMatch = Regex.Match(TestCases.case11, columnTypePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "Term Accession Number" ""
        }

        test "kindPattern 'Unit (#3)'" {
            let regExMatch = Regex.Match(TestCases.case13, columnTypePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "Unit" ""
        }

        test "kindPattern 'Term Accession Number ()'" {
            let regExMatch = Regex.Match(TestCases.case14, columnTypePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "Term Accession Number" ""
        }

        test "namePattern 'Source Name'" {
            let regExMatch = Regex.Match(TestCases.case1, namePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "" ""
        }

        test "namePattern 'Characteristic [Sample type]'" {
            let regExMatch = Regex.Match(TestCases.case3, namePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "Sample type" ""
        }

        test "namePattern 'Term Source REF (NFDI4PSO:0000064)'" {
            let regExMatch = Regex.Match(TestCases.case8, namePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "" ""
        }

        test "namePattern 'Term Accession Number (MS:1001809#2)'" {
            let regExMatch = Regex.Match(TestCases.case11, namePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "" ""
        }

        test "namePattern 'Unit (#3)'" {
            let regExMatch = Regex.Match(TestCases.case13, namePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "" ""
        }

        test "namePattern 'Term Accession Number ()'" {
            let regExMatch = Regex.Match(TestCases.case14, namePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "" ""
        }

        test "namePattern 'Parameter [hello world]'" {
            let regExMatch = Regex.Match(TestCases.case15, namePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "hello world" ""
        }

        test "namePattern 'Parameter [hello (world)]'" {
            let regExMatch = Regex.Match(TestCases.case16, namePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "hello (world)" ""
        }

        test "namePattern 'Parameter [hello[world]]'" {
            let regExMatch = Regex.Match(TestCases.case17, namePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "hello[world]" ""
        }

        test "namePattern 'Parameter [hello ]]'" {
            let regExMatch = Regex.Match(TestCases.case18, namePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "hello ]" ""
        }

        test "ontologySourcePattern 'Source Name'" {
            let regExMatch = Regex.Match(TestCases.case1, ontologySourcePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "" ""
        }

        test "ontologySourcePattern 'Characteristic [Sample type]'" {
            let regExMatch = Regex.Match(TestCases.case3, ontologySourcePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "" ""
        }

        test "ontologySourcePattern 'Term Source REF (NFDI4PSO:0000064)'" {
            let regExMatch = Regex.Match(TestCases.case8, ontologySourcePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "NFDI4PSO:0000064" ""
        }

        test "ontologySourcePattern 'Term Accession Number (MS:1001809#2)'" {
            let regExMatch = Regex.Match(TestCases.case11, ontologySourcePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "MS:1001809" ""
        }

        test "ontologySourcePattern 'Unit (#3)'" {
            let regExMatch = Regex.Match(TestCases.case13, ontologySourcePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "" ""
        }

        test "ontologySourcePattern 'Term Accession Number ()'" {
            let regExMatch = Regex.Match(TestCases.case14, ontologySourcePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "" ""
        }

        test "ontologySourcePattern get from purl url." {
            let regExMatch = Regex.Match(TestCases.case19, ontologySourcePattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "NFDI4PSO_0000064" ""
        }

        test "numberPattern 'Source Name'" {
            let regExMatch = Regex.Match(TestCases.case1, numberPattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "" ""
        }

        test "numberPattern 'Characteristic [Sample type]'" {
            let regExMatch = Regex.Match(TestCases.case3, numberPattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "" ""
        }

        test "numberPattern 'Term Source REF (NFDI4PSO:0000064)'" {
            let regExMatch = Regex.Match(TestCases.case8, numberPattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "" ""
        }

        test "numberPattern 'Term Accession Number (MS:1001809#2)'" {
            let regExMatch = Regex.Match(TestCases.case11, numberPattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "2" ""
        }

        test "numberPattern 'Unit (#3)'" {
            let regExMatch = Regex.Match(TestCases.case13, numberPattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "3" ""
        }

        test "numberPattern 'Term Accession Number ()'" {
            let regExMatch = Regex.Match(TestCases.case14, numberPattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "" ""
        }

        test "numberPattern 'Term Accession Number (#2)'" {
            let regExMatch = Regex.Match(TestCases.case20, numberPattern)
            let regexValue = regExMatch.Value.Trim()
            Expect.equal regexValue "2" ""
        }

    ]
