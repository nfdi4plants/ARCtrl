module Tests.YAMLParameterFile

open ARCtrl.CWL
open DynamicObj
open YAMLicious
open TestingUtils

let decodeYAMLParameterFile =
    TestObjects.CWL.YAMLParameterFile.yamlParameterFileContent
    |> Decode.readYaml
    |> DecodeParameters.cwlparameterReferenceArrayDecoder

let testYAMLParameterFile =
    testList "Decode" [
        testCase "Length" <| fun _ -> Expect.equal 5  decodeYAMLParameterFile.Count ""
        testCase "exampleKeyInt" <| fun _ ->
            let expected =
                CWLParameterReference(
                    key = "exampleKey",
                    values = ResizeArray [| "1234" |]
                )
            Expect.equal expected.Key decodeYAMLParameterFile.[0].Key ""
            Expect.sequenceEqual expected.Values decodeYAMLParameterFile.[0].Values ""
        testCase "exampleKeyString" <| fun _ ->
            let expected =             
                CWLParameterReference(
                    key = "exampleKeyString",
                    values = ResizeArray [| "abcdefg" |]
                )
            Expect.equal expected.Key decodeYAMLParameterFile.[1].Key ""
            Expect.sequenceEqual expected.Values decodeYAMLParameterFile.[1].Values ""
        testCase "exampleFile" <| fun _ ->
            let expected =
                CWLParameterReference(
                    key = "exampleFile",
                    values = ResizeArray [| "../examplePath" |]
                )
            Expect.equal expected.Key decodeYAMLParameterFile.[2].Key ""
            Expect.sequenceEqual expected.Values decodeYAMLParameterFile.[2].Values ""
        testCase "exampleDir" <| fun _ ->
            let expected =
                CWLParameterReference(
                    key = "exampleDir",
                    values = ResizeArray [| "../examplePathDir" |]
                )
            Expect.equal expected.Key decodeYAMLParameterFile.[3].Key ""
            Expect.sequenceEqual expected.Values decodeYAMLParameterFile.[3].Values ""
        testCase "exampleList" <| fun _ ->
            let expected =
                CWLParameterReference(
                    key = "exampleList",
                    values = ResizeArray [| "foo.txt"; "bar.dat"; "baz.txt" |]
                )
            Expect.equal expected.Key decodeYAMLParameterFile.[4].Key ""
            Expect.sequenceEqual expected.Values decodeYAMLParameterFile.[4].Values ""
        testCase "nested File arrays decode as structured values" <| fun _ ->
            let decoded = DecodeParameters.decodeYAMLParameterFile TestObjects.CWL.YAMLParameterFile.Structured.nestedFileArray
            let reference = Expect.wantExactlyOne decoded "Expected one parameter reference."
            Expect.equal reference.Key "sampleRecordFiles" "Key should decode."
            match reference.Value with
            | Some (CWLParameterValue.Array outer) ->
                Expect.equal outer.Count 2 "Outer array should contain one entry per sample."
                for item in outer do
                    match item with
                    | CWLParameterValue.Array inner ->
                        let file = Expect.wantExactlyOne inner "Each sample entry should contain one file in this fixture."
                        match file with
                        | CWLParameterValue.File file ->
                            Expect.isSome file.Path "File path should be preserved."
                            Expect.equal file.Format (Some (SchemaSaladString.Literal "edam:format_1930")) "File format metadata should be preserved."
                        | other -> failwithf "Expected file value but got %A" other
                    | other -> failwithf "Expected nested array but got %A" other
            | other -> failwithf "Expected structured array value but got %A" other
            Expect.sequenceEqual reference.Values (ResizeArray [|
                "../../assays/RNASeq/dataset/DB_097.fastq.gz"
                "../../assays/RNASeq/dataset/DB_163.fastq.gz"
            |]) "Legacy Values should flatten leaf file paths."
        testCase "array of records decodes without dropping nested fields" <| fun _ ->
            let decoded = DecodeParameters.decodeYAMLParameterFile TestObjects.CWL.YAMLParameterFile.Structured.arrayOfRecords
            let reference = Expect.wantExactlyOne decoded "Expected one parameter reference."
            match reference.Value with
            | Some (CWLParameterValue.Array records) ->
                Expect.equal records.Count 2 "Record array length should be preserved."
                match records.[0] with
                | CWLParameterValue.Record fields ->
                    Expect.isTrue (fields |> Seq.exists (fun f -> f.Name = "name")) "Record field `name` should exist."
                    Expect.isTrue (fields |> Seq.exists (fun f -> f.Name = "reads")) "Record field `reads` should exist."
                | other -> failwithf "Expected record value but got %A" other
            | other -> failwithf "Expected structured array value but got %A" other
        testCase "empty arrays decode as present empty arrays" <| fun _ ->
            let decoded = DecodeParameters.decodeYAMLParameterFile TestObjects.CWL.YAMLParameterFile.Structured.emptyArray
            let reference = Expect.wantExactlyOne decoded "Expected one parameter reference."
            match reference.Value with
            | Some (CWLParameterValue.Array values) -> Expect.isEmpty values "Empty array should be preserved."
            | other -> failwithf "Expected present empty array but got %A" other
        testCase "nested File arrays roundtrip through YAML" <| fun _ ->
            let decoded = DecodeParameters.decodeYAMLParameterFile TestObjects.CWL.YAMLParameterFile.Structured.nestedFileArray
            let encoded = Encode.encodeYAMLParameterFile decoded
            let decodedAgain = DecodeParameters.decodeYAMLParameterFile encoded
            Expect.sequenceEqual decodedAgain decoded "Structured parameter values should roundtrip through YAML."
        testCase "file object fields decode without field order assumptions and keep extension fields on the file" <| fun _ ->
            let yaml = """exampleFile:
  path: ../examplePath
  arc:file note: keep file note
  class: File"""
            let decoded = DecodeParameters.decodeYAMLParameterFile yaml
            let reference = decoded.[0]
            Expect.equal reference.Key "exampleFile" ""
            Expect.sequenceEqual reference.Values (ResizeArray [| "../examplePath" |]) "Path should decode even when it appears before class."
            match reference.Value with
            | Some (CWLParameterValue.File file) ->
                Expect.equal file.Path (Some (SchemaSaladString.Literal "../examplePath")) "The file keeps its path."
                Expect.equal (DynObj.tryGetTypedPropertyValue<string> "arc:file note" file) (Some "keep file note") "Unknown fields of the file object stay on the file."
                Expect.isNone (DynObj.tryGetTypedPropertyValue<string> "class" file) "Known class field should not be overflow."
            | other -> failwithf "Expected a file value but got %A" other
        testCase "an object with the fields type and value is a record" <| fun _ ->
            let yaml = "sample:\n  type: tumor\n  value: 3\n"
            let reference = Expect.wantExactlyOne (DecodeParameters.decodeYAMLParameterFile yaml) "Expected one parameter reference."
            let expected =
                CWLParameterValue.Record (ResizeArray [
                    CWLParameterRecordField("type", CWLParameterValue.String "tumor")
                    CWLParameterRecordField("value", CWLParameterValue.Int 3L)
                ])
            Expect.equal reference.Value (Some expected) "The two fields are the fields of the record, whatever the process input declares."
            Expect.equal (Encode.encodeYAMLParameterFile (ResizeArray [ reference ])) yaml "The record is written as it was read."
        testCase "an object whose type field is no CWL type is a record too" <| fun _ ->
            let yaml = """sample:
  type: tumor tissue
  id: s1
"""
            let reference = Expect.wantExactlyOne (DecodeParameters.decodeYAMLParameterFile yaml) "Expected one parameter reference."
            let expected =
                CWLParameterValue.Record (ResizeArray [
                    CWLParameterRecordField("type", CWLParameterValue.String "tumor tissue")
                    CWLParameterRecordField("id", CWLParameterValue.String "s1")
                ])
            Expect.equal reference.Value (Some expected) "The text tumor tissue is a string value, not a type."
        testCase "a file value is written as a File object and nothing else" <| fun _ ->
            let reference = CWLParameterReference(key = "reads", value = CWLParameterValue.File (FileInstance(path = SchemaSaladString.Literal "a.txt")))
            let encoded = Encode.encodeYAMLParameterFile (ResizeArray [ reference ])
            Expect.equal encoded "reads:\n  class: File\n  path: a.txt\n" "The job file has the class and the path, and no type."
            Expect.sequenceEqual (DecodeParameters.decodeYAMLParameterFile encoded) (ResizeArray [ reference ]) "The File object reads back as the same value."
        testCase "quoted scalars decode as strings and plain scalars keep their type" <| fun _ ->
            let yaml = TestObjects.CWL.YAMLParameterFile.CoreSchemaScalars.quotedAndPlainScalars
            let decoded = DecodeParameters.decodeYAMLParameterFile yaml
            let valueOf key = (decoded |> Seq.find (fun r -> r.Key = key)).Value
            Expect.equal (valueOf "quotedBool") (Some (CWLParameterValue.String "true")) "A quoted true is a string."
            Expect.equal (valueOf "plainBool") (Some (CWLParameterValue.Boolean true)) "A plain true is a boolean."
            Expect.equal (valueOf "quotedInt") (Some (CWLParameterValue.String "12")) "A quoted number is a string."
            Expect.equal (valueOf "plainInt") (Some (CWLParameterValue.Int 12L)) "A plain number is an int."
            Expect.equal (valueOf "quotedNull") (Some (CWLParameterValue.String "null")) "A quoted null is a string."
            Expect.equal (valueOf "plainNull") (Some CWLParameterValue.Null) "A plain null is null."
            Expect.equal (valueOf "quotedFloat") (Some (CWLParameterValue.String "1.5")) "A quoted float is a string."
            Expect.equal (valueOf "plainFloat") (Some (CWLParameterValue.Float 1.5)) "A plain float is a float."
            Expect.equal (valueOf "plainInfinity") (Some (CWLParameterValue.Float System.Double.PositiveInfinity)) "The core schema spells infinity .inf."
            Expect.equal (valueOf "capitalBool") (Some (CWLParameterValue.Boolean true)) "The core schema reads True as a boolean."
            Expect.equal (valueOf "upperBool") (Some (CWLParameterValue.Boolean false)) "The core schema reads FALSE as a boolean."
            Expect.equal (valueOf "tilde") (Some CWLParameterValue.Null) "The core schema reads ~ as null."
            Expect.equal (valueOf "capitalNull") (Some CWLParameterValue.Null) "The core schema reads Null as null."
            Expect.equal (valueOf "nanWord") (Some (CWLParameterValue.String "NaN")) "YAML 1.2 reads NaN as a string."
            Expect.equal (valueOf "infinityWord") (Some (CWLParameterValue.String "Infinity")) "YAML 1.2 reads Infinity as a string."
        testCase "strings that look like another type roundtrip as strings" <| fun _ ->
            let strings = [ "true"; "False"; "null"; "~"; "12"; "-3"; "1.5"; "1e3"; "0x1F"; ".inf"; "plain text" ]
            let references =
                strings
                |> List.map (fun s -> CWLParameterReference(key = "k" + s, value = CWLParameterValue.String s))
                |> ResizeArray
            let encoded = Encode.encodeYAMLParameterFile references
            let decoded = DecodeParameters.decodeYAMLParameterFile encoded
            Expect.sequenceEqual decoded references "String parameter values must not turn into bool, number or null."
            Expect.stringContains encoded ": \"true\"" "The string true should be written with quotes."
            Expect.stringContains encoded ": plain text" "A string that cannot be mistaken for another type stays plain."
        testCase "plain spellings of null and booleans keep their meaning through a roundtrip" <| fun _ ->
            let yaml = """capitalBool: True
upperBool: FALSE
tilde: ~
capitalNull: Null
wholeFloat: 2.0
infinity: .inf
"""
            let decoded = DecodeParameters.decodeYAMLParameterFile yaml
            let reloaded = DecodeParameters.decodeYAMLParameterFile (Encode.encodeYAMLParameterFile decoded)
            Expect.sequenceEqual reloaded decoded "The values should decode to the same types after a roundtrip."
            let encoded = Encode.encodeYAMLParameterFile decoded
            Expect.isFalse (encoded.Contains("\"")) "None of these values is a string, so nothing may be quoted."
            Expect.stringContains encoded "wholeFloat: 2.0" "A whole number written as a float must not turn into an int."
            Expect.stringContains encoded "infinity: .inf" "The core schema spells infinity .inf, and Infinity would read back as a string."
        testCase "compact nested sequences decode as arrays of arrays" <| fun _ ->
            let yaml = """matrix:
  - - 1
    - 2
  - - 3
next: value
"""
            let decoded = DecodeParameters.decodeYAMLParameterFile yaml
            let valueOf key = (decoded |> Seq.find (fun r -> r.Key = key)).Value
            let ints xs = CWLParameterValue.Array (ResizeArray (xs |> List.map CWLParameterValue.Int))
            Expect.equal decoded.Count 2 "The key after the nested sequences is a separate parameter."
            Expect.equal (valueOf "matrix") (Some (CWLParameterValue.Array (ResizeArray [ ints [ 1L; 2L ]; ints [ 3L ] ]))) "Each inner sequence should be its own array."
            Expect.equal (valueOf "next") (Some (CWLParameterValue.String "value")) "The following key keeps its value."
        testCase "a $namespaces block survives a roundtrip" <| fun _ ->
            let yaml = """isSingle: true
$namespaces:
  edam: https://edamontology.org/
"""
            let decoded = DecodeParameters.decodeYAMLParameterFile yaml
            let encoded = Encode.encodeYAMLParameterFile decoded
            Expect.stringContains encoded "$namespaces:" "The namespaces key should be written."
            Expect.stringContains encoded "edam: https://edamontology.org/" "The prefix should be written below the key."
            Expect.sequenceEqual (DecodeParameters.decodeYAMLParameterFile encoded) decoded "The parameter file should decode to the same references."
        testCase "a key without a value holds null" <| fun _ ->
            let yaml = """present: 1
empty:
nested:
  inner:
items:
  - 1
  -
  - x
"""
            let decoded = DecodeParameters.decodeYAMLParameterFile yaml
            let valueOf key = (decoded |> Seq.find (fun r -> r.Key = key)).Value
            Expect.equal (valueOf "empty") (Some CWLParameterValue.Null) "A key without a value is null, not the text Nil."
            match valueOf "nested" with
            | Some (CWLParameterValue.Record fields) ->
                let inner = Expect.wantExactlyOne fields "The nested mapping has one key."
                Expect.equal inner.Value CWLParameterValue.Null "The nested key without a value is null too."
            | other -> failwithf "Expected a record but got %A" other
            let expectedItems = CWLParameterValue.Array (ResizeArray [ CWLParameterValue.Int 1L; CWLParameterValue.Null; CWLParameterValue.String "x" ])
            Expect.equal (valueOf "items") (Some expectedItems) "A sequence item without a value is null, not an empty mapping."
            let encoded = Encode.encodeYAMLParameterFile decoded
            Expect.isFalse (encoded.Contains("Nil")) "No node name may leak into the output."
            Expect.isFalse (encoded.Contains("{}")) "The empty item must not turn into an empty mapping."
            let valuesOf (references: ResizeArray<CWLParameterReference>) = references |> Seq.map (fun r -> r.Key, r.Value) |> Seq.toList
            Expect.equal (valuesOf (DecodeParameters.decodeYAMLParameterFile encoded)) (valuesOf decoded) "The keys and values should not change."
        testCase "a comment after a flow sequence does not hide the sequence" <| fun _ ->
            let yaml = """xs: [a, b] # tail
files:
  - class: File
    path: a.txt
  # comment after the last item
"""
            let decoded = DecodeParameters.decodeYAMLParameterFile yaml
            let valueOf key = (decoded |> Seq.find (fun r -> r.Key = key)).Value
            Expect.equal (valueOf "xs") (Some (CWLParameterValue.Array (ResizeArray [ CWLParameterValue.String "a"; CWLParameterValue.String "b" ]))) "The flow sequence should decode as an array."
            match valueOf "files" with
            | Some (CWLParameterValue.Array items) ->
                Expect.equal items.Count 1 "The trailing comment must not add an item."
            | other -> failwithf "Expected an array of files but got %A" other
    ]

let testScalarsOfJobFiles =
    let valueOf (text: string) =
        let reference = Expect.wantExactlyOne (DecodeParameters.decodeYAMLParameterFile ("x: " + text + "\n")) "Expected one parameter reference."
        Expect.wantSome reference.Value (sprintf "%s should hold a value" text)
    testList "Plain scalars of a job file" [
        testCase "a plain scalar of a job file has the type that the core schema gives it" <| fun _ ->
            let cases =
                [ ".5e3", CWLParameterValue.Float 500.0
                  "+.5e3", CWLParameterValue.Float 500.0
                  ".5e+3", CWLParameterValue.Float 500.0
                  ".5", CWLParameterValue.Float 0.5
                  "5.", CWLParameterValue.Float 5.0
                  "1e3", CWLParameterValue.Float 1000.0
                  "1.e3", CWLParameterValue.Float 1000.0
                  "-.5E-3", CWLParameterValue.Float -0.0005
                  ".inf", CWLParameterValue.Float infinity
                  "-.Inf", CWLParameterValue.Float -infinity
                  "0o17", CWLParameterValue.Int 15L
                  "017", CWLParameterValue.Int 17L
                  "0x1F", CWLParameterValue.Int 31L
                  "-0", CWLParameterValue.Int 0L
                  "+1", CWLParameterValue.Int 1L
                  "-017", CWLParameterValue.Int -17L
                  "true", CWLParameterValue.Boolean true
                  "True", CWLParameterValue.Boolean true
                  "FALSE", CWLParameterValue.Boolean false
                  "~", CWLParameterValue.Null
                  "null", CWLParameterValue.Null
                  "NULL", CWLParameterValue.Null
                  "1_000", CWLParameterValue.String "1_000"
                  "1__0", CWLParameterValue.String "1__0"
                  "1_0.5", CWLParameterValue.String "1_0.5"
                  "0b101", CWLParameterValue.String "0b101"
                  "=", CWLParameterValue.String "="
                  "<<", CWLParameterValue.String "<<"
                  "._", CWLParameterValue.String "._"
                  "yes", CWLParameterValue.String "yes"
                  "on", CWLParameterValue.String "on"
                  "2001-12-14", CWLParameterValue.String "2001-12-14"
                  "0X1F", CWLParameterValue.String "0X1F"
                  "0o8", CWLParameterValue.String "0o8"
                  "1:30", CWLParameterValue.String "1:30"
                  "_1", CWLParameterValue.String "_1"
                  "1e", CWLParameterValue.String "1e"
                  "1.5.5", CWLParameterValue.String "1.5.5"
                  "\"1_000\"", CWLParameterValue.String "1_000"
                  "'.5e+3'", CWLParameterValue.String ".5e+3" ]
            for (text, expected) in cases do
                Expect.equal (valueOf text) expected (sprintf "The scalar %s should have the type that the core schema gives it." text)
            match valueOf ".NaN" with
            | CWLParameterValue.Float parsed -> Expect.isTrue (System.Double.IsNaN parsed) ".NaN is the float that is not a number."
            | other -> failwithf "Expected a float but got %A" other
        testCase "a string that cwltool would read as another type is written in quotes and reads back as the string" <| fun _ ->
            let quoted =
                [ "1_000"; "0b101"; "1_0.5"; "1__0"; "+1_0"; "1_"; "1e3"; "5."; ".5e+3"; "0o17"; "017"; "true"; "~"; "="; "<<"; "yes"; "No"; "on"; "OFF"; "y"; "n"
                  "1:30"; "0b_"; "0x_"; "-_"; "._"; "-._"; "._e+5"; ".__"; ".5e3" ]
            for text in quoted do
                let reference = CWLParameterReference(key = "x", value = CWLParameterValue.String text)
                let written = Encode.encodeYAMLParameterFile (ResizeArray [ reference ])
                Expect.stringContains written ("\"" + text + "\"") (sprintf "The string %s is written in quotes.\n%s" text written)
                Expect.equal (valueOf ("\"" + text + "\"")) (CWLParameterValue.String text) (sprintf "The quoted %s is a string." text)
                let decoded = Expect.wantExactlyOne (DecodeParameters.decodeYAMLParameterFile written) "Expected one parameter reference."
                Expect.equal decoded.Value (Some (CWLParameterValue.String text)) (sprintf "The written %s should read back as the string." text)
            for text in [ "0X1F"; "abc"; "2001-12-14"; "YeS"; "_1"; "_"; "NaN"; "Infinity" ] do
                let written = Encode.encodeYAMLParameterFile (ResizeArray [ CWLParameterReference(key = "x", value = CWLParameterValue.String text) ])
                Expect.equal written ("x: " + text + "\n") (sprintf "The string %s stays plain, because no reader takes it for another type." text)
        testCase "an edited doc or label that cwltool would read as another type is written in quotes" <| fun _ ->
            let tool = Decode.decodeCommandLineTool "cwlVersion: v1.2\nclass: CommandLineTool\nbaseCommand: echo\nlabel: first\ninputs: {}\noutputs: {}\n"
            for text in [ "1_000"; "0b101"; ".5e3"; "yes" ] do
                tool.Label <- Some (SchemaSaladString.Literal text)
                let written = Encode.encodeToolDescription tool
                Expect.stringContains written ("label: \"" + text + "\"") (sprintf "The label %s is written in quotes." text)
                Expect.equal (Decode.decodeCommandLineTool written).Label tool.Label "The label reads back as the string."
    ]

let testProhibitedYamlFeaturesInJobFiles =
    let errorOf (name: string) (text: string) : DecodeError =
        match (try DecodeParameters.decodeYAMLParameterFile text |> ignore; None with YamlDecodeException error -> Some error) with
        | Some error ->
            Expect.isSome error.Span (sprintf "%s: the error should have the place in the text" name)
            error
        | None -> failwith (sprintf "%s must fail" name)
    testList "Anchors, aliases, explicit tags, directives and merge keys in a job file" [
        testCase "an anchor, an explicit tag and a directive are decode errors with the place, as in a CWL document" <| fun _ ->
            for (name, text, path, message) in
                [ "an anchor", "x: &v a\ny: b\n", "x", "The anchor &v is not allowed"
                  "an anchor and its alias", "x: &v a\ny: *v\n", "x", "The anchor &v is not allowed"
                  "an anchor in a list", "x: [a, &v b]\n", "x[1]", "The anchor &v is not allowed"
                  "an anchor in a record", "x:\n  y: &m {z: 1}\n", "x.y", "The anchor &m is not allowed"
                  "an explicit tag", "x: !!str 5\n", "x", "The tag !!str is not allowed"
                  "a tag of a File", "x: !custom {class: File, path: a.txt}\n", "x", "The tag !custom is not allowed"
                  "a %YAML directive", "%YAML 1.2\n---\nx: 1\n", "", "The directive %YAML 1.2 is not allowed"
                  "a %TAG directive", "%TAG ! tag:x,2000:\n---\nx: 1\n", "", "The directive %TAG ! tag:x,2000: is not allowed" ] do
                let error = errorOf name text
                Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
                Expect.stringContains error.Message message (sprintf "%s: the error should say what is wrong" name)
                Expect.stringContains error.Message "Schema Salad v1.2 prohibits" (sprintf "%s: the error should name the rule" name)
        testCase "an alias without an anchor fails in the reader, so its error stands at the root" <| fun _ ->
            let error = errorOf "an alias without an anchor" "x: *nothere\n"
            Expect.equal (DecodeError.pathText error) "" "The error stands at the root."
            Expect.stringContains error.Message "*nothere" "The error names the alias."
            Expect.isFalse (error.Message.StartsWith "Line") "The place is in the span."
        testCase "a << key is an ordinary key of the job file, and a quoted one is too" <| fun _ ->
            for key in [ "<<"; "\"<<\"" ] do
                let references = DecodeParameters.decodeYAMLParameterFile (key + ": {a: 1}\ny: <<\n")
                Expect.equal references.[0].Key "<<" (sprintf "%s: the key stays." key)
                Expect.equal references.[0].Value (Some (CWLParameterValue.Record (ResizeArray [ CWLParameterRecordField("a", CWLParameterValue.Int 1L) ]))) (sprintf "%s: the value is a record." key)
                Expect.equal references.[1].Value (Some (CWLParameterValue.String "<<")) (sprintf "%s: the value << is a text." key)
                let written = Encode.encodeYAMLParameterFile references
                Expect.stringContains written "\"<<\":" (sprintf "%s: the key is written in quotes.\n%s" key written)
                Expect.sequenceEqual (DecodeParameters.decodeYAMLParameterFile written) references (sprintf "%s: the written file should decode to the same references." key)
        testCase "a key that the core schema reads as null, a bool or a number is a decode error at the key, for a parameter and for a field of a record" <| fun _ ->
            // Schema Salad restricts a job file to the JSON-compatible subset of YAML, where every key is a string.
            let keys = [ "1"; "0x1"; "-2"; "1.5"; ".nan"; "-.inf"; "12345678901234567890"; "true"; "False"; "null"; "~" ]
            for key in keys do
                for (place, text, path, line, column) in
                    [ "a parameter", "x: 0\n" + key + ": a\n", "", 2, 1
                      "a field of a record", "x:\n  " + key + ": a\n", "x", 2, 3
                      "a field of a record in a record", "x:\n  y: {z: 0, " + key + ": a}\n", "x.y", 2, 13
                      "a field of a record in a list", "x:\n  - " + key + ": a\n", "x[0]", 2, 5 ] do
                    let error = errorOf (place + " with the key " + key) text
                    Expect.equal (DecodeError.pathText error) path (sprintf "%s with the key %s: the error should show the mapping of the key" place key)
                    Expect.equal (error.Span |> Option.map (fun span -> span.Start.Line, span.Start.Column)) (Some (line, column)) (sprintf "%s with the key %s: the error should stand at the key" place key)
                    Expect.stringContains error.Message "Expected a string, found " (sprintf "%s with the key %s: the error should say that the key is no string" place key)
            let empty = errorOf "an empty key" "x: {z: 0, : a}\n"
            Expect.equal (DecodeError.pathText empty) "x" "An empty key is null, and the error shows the mapping of the key."
            Expect.stringContains empty.Message "Expected a string, found null." "The error should say that the empty key is null."
        testCase "a key that is a string names a parameter or a field, also when it looks like another type, and the written file reads back equal" <| fun _ ->
            let keys = [ "\"1\"", "1"; "'true'", "true"; "\"null\"", "null"; "\"~\"", "~"; "\"1.5\"", "1.5"; "\"\"", ""; "yes", "yes"; "1_000", "1_000"; "<<", "<<"; "a b", "a b" ]
            let text = keys |> List.map (fun (spelling, _) -> spelling + ": {" + spelling + ": a}") |> String.concat "\n"
            let references = DecodeParameters.decodeYAMLParameterFile (text + "\n")
            for (reference, (spelling, name)) in Seq.zip references keys do
                Expect.equal reference.Key name (sprintf "The key %s names the parameter %s." spelling name)
                Expect.equal reference.Value (Some (CWLParameterValue.Record (ResizeArray [ CWLParameterRecordField(name, CWLParameterValue.String "a") ]))) (sprintf "The key %s names the field %s." spelling name)
            let written = Encode.encodeYAMLParameterFile references
            let readBack = DecodeParameters.decodeYAMLParameterFile written
            Expect.sequenceEqual readBack references (sprintf "The written file should decode to the same references.\n%s" written)
            Expect.equal (Encode.encodeYAMLParameterFile readBack) written "Writing again should give the same text."
        testCase "the error of a value shows the parameter, the field of a record and the index of a list" <| fun _ ->
            let beyond = "100000000000000000000000000000"
            for (name, text, path) in
                [ "a parameter", "x: " + beyond + "\n", "x"
                  "an item of a list", "x: [1, " + beyond + "]\n", "x[1]"
                  "a field of a record", "x:\n  a:\n    b: " + beyond + "\n", "x.a.b"
                  "an item of a list in a record", "x:\n  a: [" + beyond + "]\n", "x.a[0]" ] do
                let error = errorOf name text
                Expect.equal (DecodeError.pathText error) path (sprintf "%s: the error should show the place" name)
                Expect.stringContains error.Message "beyond 64 bits" (sprintf "%s: the error should say what is wrong" name)
    ]

let main =
    testList "YAMLParameterFile" [
        testYAMLParameterFile
        testScalarsOfJobFiles
        testProhibitedYamlFeaturesInJobFiles
    ]
