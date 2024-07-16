namespace ARCtrl.Yaml

module Encode =

    open YAMLicious
    open YAMLicious.YAMLiciousTypes
    open YAMLicious.Writer

    let DefaultWhitespace = 2

    let defaultWhitespace spaces = defaultArg spaces DefaultWhitespace

    let inline toYamlString whitespace (element : YAMLElement) = 
        write element (Some (fun c -> {c with Whitespace = whitespace}))