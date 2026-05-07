namespace ARCtrl.Yaml.ROCrate

open ARCtrl.ROCrate
open YAMLicious.YAMLiciousTypes

module LDValue =

    let genericDecoder (value: YAMLElement) : obj =
        match Helpers.unwrapSingleObject value with
        | YAMLElement.Value v -> Helpers.parseScalarToObj v.Value
        | _ -> failwithf "Expected scalar value for @value but got %A" value

    let decoder (value: YAMLElement) : LDValue =
        let valueField = Helpers.requireField "@value" value |> genericDecoder
        let valueType = Helpers.tryGetField "@type" value |> Option.map Helpers.decodeString
        LDValue(valueField, ?valueType = valueType)

    let genericEncoder (value : obj) =
        Helpers.objToScalarYaml value

    let encoder (v: LDValue) =
        [
            "\"@value\"", genericEncoder v.Value
            "\"@type\"", Helpers.yamlValue v.ValueType
        ]
        |> Helpers.yamlMap
