namespace ARCtrl.Yaml

module Decode =

    open YAMLicious

    let inline fromYamlString (decoder : YamlNode -> 'a) (s : string) : 'a = 
        Reader.readRoot s |> decoder