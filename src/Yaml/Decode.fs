namespace ARCtrl.Yaml

module Decode =

    open YAMLicious
    open YAMLicious.YAMLiciousTypes
    open YAMLicious.Reader

    let inline fromYamlString (decoder : YAMLElement -> 'a) (s : string) : 'a = 
        read s |> decoder