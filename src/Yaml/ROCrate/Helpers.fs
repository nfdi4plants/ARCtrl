namespace ARCtrl.Yaml.ROCrate

open System
open System.Globalization
open YAMLicious.YAMLiciousTypes

module Helpers =

    let normalizeKey (key: string) =
        key.Trim().Trim('"').Trim('\'')

    let ensureSingleYamlDocument (yaml: string) =
        let lines =
            yaml.Replace("\r\n", "\n").Split('\n')
            |> Array.map (fun l -> l.Trim())

        let nonEmpty = lines |> Array.filter (String.IsNullOrWhiteSpace >> not)
        let separators =
            lines
            |> Array.indexed
            |> Array.choose (fun (i, l) -> if l = "---" then Some i else None)

        if separators.Length > 1 then
            failwith "YAML-LD parser supports only a single YAML document in a stream."

        if separators.Length = 1 then
            let firstNonEmptyIndex =
                lines
                |> Array.tryFindIndex (String.IsNullOrWhiteSpace >> not)
                |> Option.defaultValue 0

            if separators.[0] <> firstNonEmptyIndex then
                failwith "YAML-LD parser supports only a single YAML document in a stream."

        if nonEmpty.Length = 0 then
            failwith "YAML-LD document is empty."

        yaml

    let unwrapSingleObject = function
        | YAMLElement.Object [single] ->
            match single with
            | YAMLElement.Mapping _ -> YAMLElement.Object [single]
            | _ -> single
        | other -> other

    let tryGetMappings = function
        | YAMLElement.Object elements ->
            elements
            |> List.choose (function
                | YAMLElement.Mapping (k, v) -> Some (normalizeKey k.Value, v)
                | _ -> None
            )
            |> Some
        | _ -> None

    let getMappings element =
        element
        |> unwrapSingleObject
        |> tryGetMappings
        |> Option.defaultValue []

    let tryGetField name element =
        getMappings element
        |> List.tryPick (fun (k, v) -> if normalizeKey k = name then Some v else None)

    let requireField name element =
        match tryGetField name element with
        | Some value -> value
        | None -> failwithf "Required field '%s' is missing." name

    let tryDecodeString = function
        | YAMLElement.Value v -> Some v.Value
        | YAMLElement.Object [YAMLElement.Value v] -> Some v.Value
        | _ -> None

    let decodeString element =
        match tryDecodeString element with
        | Some s -> s
        | None -> failwithf "Expected string-like YAML value but got %A" element

    let tryDecodeStringResizeArray element =
        match unwrapSingleObject element with
        | YAMLElement.Sequence elements ->
            elements
            |> List.map decodeString
            |> ResizeArray
            |> Some
        | _ ->
            tryDecodeString element
            |> Option.map (fun s -> ResizeArray [s])

    let decodeStringResizeArray element =
        match tryDecodeStringResizeArray element with
        | Some values -> values
        | None -> failwithf "Expected string or sequence of strings but got %A" element

    let tryDecodeSequence = function
        | YAMLElement.Sequence elements -> Some elements
        | YAMLElement.Object [YAMLElement.Sequence elements] -> Some elements
        | _ -> None

    let parseScalarToObj (value: string) : obj =
        let text = value.Trim()

        if text.Equals("null", StringComparison.OrdinalIgnoreCase) || text = "~" then
            null
        else
            let mutable boolValue = false
            let mutable intValue = 0
            let mutable decimalValue = 0M
            if Boolean.TryParse(text, &boolValue) then box boolValue
            elif Int32.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, &intValue) then box intValue
            elif Decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, &decimalValue) then box decimalValue
            else box value

    let yamlValue (s: string) =
        YAMLElement.Value (YAMLContent.create s)

    let yamlMap (pairs: (string * YAMLElement) list) =
        pairs
        |> List.map (fun (k,v) -> YAMLElement.Mapping (YAMLContent.create k, v))
        |> YAMLElement.Object

    let yamlSeq (items: YAMLElement list) =
        YAMLElement.Sequence items

    let objToScalarYaml (value: obj) =
        match value with
        | null -> yamlValue "null"
        | :? string as s -> yamlValue s
        | :? int as i -> yamlValue (string i)
        | :? bool as b -> yamlValue (if b then "true" else "false")
        | :? float as f -> yamlValue (f.ToString(CultureInfo.InvariantCulture))
        | :? decimal as d -> yamlValue (d.ToString(CultureInfo.InvariantCulture))
        | :? DateTime as d -> yamlValue (d.ToString("O", CultureInfo.InvariantCulture))
        | _ -> failwithf "Unsupported scalar type for YAML encoder: %s" (value.GetType().FullName)
