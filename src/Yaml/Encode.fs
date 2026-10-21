namespace ARCtrl.Yaml

module Encode =

    open YAMLicious

    let DefaultWhitespace = 2

    let defaultWhitespace spaces = defaultArg spaces DefaultWhitespace

    let inline toYamlString whitespace (node : YamlNode) = 
        Writer.writeNode { WriterOptions.defaults with Indent = whitespace } node