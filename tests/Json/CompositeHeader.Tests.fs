module Tests.CompositeHeader

open ARCtrl
open ARCtrl.Json
open TestingUtils

let private tests_extended = testList "extended" [
    let header_parameter = CompositeHeader.Parameter (OntologyAnnotation("My Name", "MY", "MY:2"))
    let header_parameter_jsonString =  """{"headertype":"Parameter","values":[{"annotationValue":"My Name","termSource":"MY","termAccession":"MY:2"}]}""" 
    let header_input_source = CompositeHeader.Input IOType.Source
    let header_input_source_jsonString =  """{"headertype":"Input","values":["Source Name"]}""" 
    let header_protocolref = CompositeHeader.ProtocolREF
    let header_protocolref_jsonString =  """{"headertype":"ProtocolREF","values":[]}""" 
    testList "encoder" [
    testCase "Parameter" <| fun _ ->
        let actual = Encode.toJsonString  0 <| CompositeHeader.encoder header_parameter
        let expected = header_parameter_jsonString
        Expect.equal actual expected ""
    testCase "Input Source" <| fun _ ->
        let actual = Encode.toJsonString  0 <| CompositeHeader.encoder header_input_source
        let expected = header_input_source_jsonString
        Expect.equal actual expected ""
    testCase "ProtocolREF" <| fun _ ->
        let actual = Encode.toJsonString  0 <| CompositeHeader.encoder header_protocolref
        let expected = header_protocolref_jsonString
        Expect.equal actual expected ""
    ]
    testList "decoder" [
    testCase "Parameter" <| fun _ ->
        let actual = Decode.fromJsonString CompositeHeader.decoder header_parameter_jsonString
        let expected = header_parameter
        Expect.equal actual expected ""
    testCase "Input Source" <| fun _ ->
        let actual = Decode.fromJsonString CompositeHeader.decoder header_input_source_jsonString
        let expected = header_input_source
        Expect.equal actual expected ""
    testCase "ProtocolREF" <| fun _ ->
        let actual = Decode.fromJsonString CompositeHeader.decoder header_protocolref_jsonString
        let expected = header_protocolref
        Expect.equal actual expected ""
    ]
]

let main = testList "CompositeHeader" [
    tests_extended
]