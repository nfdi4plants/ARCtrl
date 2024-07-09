namespace ARCtrl.Yaml

module Decode =

    // yamlicious equivalents for this?

    //let helpers = 
    //    #if FABLE_COMPILER_PYTHON
    //    Thoth.Json.Python.Decode.helpers
    //    #endif
    //    #if FABLE_COMPILER_JAVASCRIPT
    //    Thoth.Json.JavaScript.Decode.helpers
    //    #endif
    //    #if !FABLE_COMPILER
    //    Thoth.Json.Newtonsoft.Decode.helpers
    //    #endif

    //let inline fromJsonString (decoder : Decoder<'a>) (s : string) : 'a = 
    //    #if FABLE_COMPILER_PYTHON
    //    match Thoth.Json.Python.Decode.fromString decoder s with
    //    #endif
    //    #if FABLE_COMPILER_JAVASCRIPT
    //    match Thoth.Json.JavaScript.Decode.fromString decoder s with
    //    #endif
    //    #if !FABLE_COMPILER
    //    match Thoth.Json.Newtonsoft.Decode.fromString decoder s with
    //    #endif
    //    | Ok a -> a
    //    | Error e -> failwith (sprintf "Error decoding string: %O" e)

    open YAMLicious
    open YAMLicious.YAMLiciousTypes
    open YAMLicious.Reader

    let inline fromYamlString (decoder : YAMLElement -> 'a) (s : string) : 'a = 
        read s |> decoder