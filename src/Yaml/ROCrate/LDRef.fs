namespace ARCtrl.Yaml.ROCrate

open ARCtrl.ROCrate
open YAMLicious.YAMLiciousTypes

module LDRef =

    let decoder (value: YAMLElement) : LDRef =
        let id = Helpers.requireField "@id" value |> Helpers.decodeString
        LDRef(id)

    let encoder (r: LDRef) =
        [
            "\"@id\"", Helpers.yamlValue r.Id
        ]
        |> Helpers.yamlMap
