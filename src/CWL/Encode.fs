namespace ARCtrl.CWL

open System
open DynamicObj
open YAMLicious
open YAMLicious.YAMLiciousTypes
open YAMLicious.Writer

/// Encoding helpers and functions mirroring the logic in Decode.fs.
/// Each decode path gets a corresponding encode path that produces
/// YAML key ordering and scalar formatting matching existing fixtures.
module Encode =
    // ------------------------------
    // Generic YAML node constructors
    // ------------------------------
    let private yScalar (s:string) =
        YAMLElement.Value { Value = s; Comment = None }

    let private yInt (i:int) = YAMLElement.Value { Value = string i; Comment = None }
    let private yFloat (f:float) =
        YAMLElement.Value { Value = f.ToString(System.Globalization.CultureInfo.InvariantCulture); Comment = None }
    let private yBool (b:bool) =
        YAMLElement.Value { Value = (if b then "true" else "false"); Comment = None }

    let private ySeq (elements: YAMLElement list) =
        YAMLElement.Sequence elements

    let private yMap (pairs: (string * YAMLElement) list) =
        // Represent a mapping as an Object containing Mapping nodes preserving order.
        // Avoid wrapping scalar/sequence values inside an extra Object layer so YAMLicious prints 'key: value'.
        let normalize = function
            | YAMLElement.Object [single] -> single // unwrap single wrapped value (legacy helper usage)
            | other -> other
        pairs
        |> List.map (fun (k,v) -> YAMLElement.Mapping ({ Value = k; Comment = None }, normalize v))
        |> YAMLElement.Object

    // ------------------------------
    // Helper append functions for ordered construction
    // ------------------------------
    let inline private appendOpt name (encoder:'a -> YAMLElement) (value:'a option) acc =
        match value with
        | Some v -> acc @ [name, encoder v]
        | None -> acc

    let inline private appendList name (encoder:'a -> YAMLElement) (values: ResizeArray<'a>) acc =
        if isNull (box values) || values.Count = 0 then acc
        else acc @ [ name, (values |> Seq.map encoder |> List.ofSeq |> ySeq) ]

    // ------------------------------
    // CWLType encoder
    // ------------------------------
    let rec encodeCWLType (t:CWLType) : YAMLElement =
        match t with
        | File _ -> yScalar "File"
        | Directory _ -> yScalar "Directory"
        | Dirent d ->
            [ "entry", yScalar d.Entry ]
            |> appendOpt "entryname" yScalar d.Entryname
            |> appendOpt "writable" (fun b -> yBool b) d.Writable
            |> yMap
        | String -> yScalar "string"
        | Int -> yScalar "int"
        | Long -> yScalar "long"
        | Float -> yScalar "float"
        | Double -> yScalar "double"
        | Boolean -> yScalar "boolean"
        | Stdout -> yScalar "stdout"
        | Null -> yScalar "null"
        | Array inner ->
            let shortForm =
                match inner with
                | File _ -> Some "File[]"
                | Directory _ -> Some "Directory[]"
                | Dirent _ -> Some "Dirent[]"
                | String -> Some "string[]"
                | Int -> Some "int[]"
                | Long -> Some "long[]"
                | Float -> Some "float[]"
                | Double -> Some "double[]"
                | Boolean -> Some "boolean[]"
                | _ -> None
            match shortForm with
            | Some s -> yScalar s
            | None -> [ "type", yScalar "array"; "items", encodeCWLType inner ] |> yMap

    // ------------------------------
    // Binding & Port encoders
    // ------------------------------

    let encodeOutputBinding (ob:OutputBinding) : YAMLElement =
        [ ob.Glob |> Option.map (fun g -> "glob", yScalar g) ]
        |> List.choose id
        |> yMap

    let encodeCWLOutput (o:CWLOutput) : (string * YAMLElement) =
        let pairs =
            []
            |> appendOpt "type" (fun t -> encodeCWLType t) o.Type_
            |> appendOpt "outputBinding" encodeOutputBinding o.OutputBinding
            |> appendOpt "outputSource" yScalar o.OutputSource
        match pairs with
        | [ ("type", t) ] -> o.Name, t // only type specified
        | _ ->
            // Always extended form when additional fields like outputSource/outputBinding present
            o.Name, (yMap pairs)

    let encodeInputBinding (ib:InputBinding) : YAMLElement =
        []
        |> appendOpt "prefix" yScalar ib.Prefix
        |> appendOpt "position" (fun p -> yInt p) ib.Position
        |> appendOpt "itemSeparator" yScalar ib.ItemSeparator
        |> appendOpt "separate" yBool ib.Separate
        |> yMap

    let encodeCWLInput (i:CWLInput) : (string * YAMLElement) =
        let pairs =
            []
            |> appendOpt "type" encodeCWLType i.Type_
            |> appendOpt "inputBinding" encodeInputBinding i.InputBinding
            |> appendOpt "optional" yBool i.Optional
        match pairs with
        | [ ("type", t) ] -> i.Name, t
        | _ -> i.Name, yMap pairs

    // ------------------------------
    // Requirement encoder (always extended style)
    // ------------------------------

    let encodeSchemaDefRequirementType (s:SchemaDefRequirementType) : YAMLElement =
        s.GetProperties(false)
        |> Seq.choose (fun kvp ->
            match kvp.Value with
            | :? string as str -> Some (kvp.Key, yScalar str)
            | _ -> None)
        |> Seq.toList
        |> yMap

    let encodeRequirement (r:Requirement) : YAMLElement =
        match r with
        | InlineJavascriptRequirement -> [ "class", yScalar "InlineJavascriptRequirement" ] |> yMap
        | SchemaDefRequirement types ->
            [ "class", yScalar "SchemaDefRequirement";
              "types", (types |> Seq.map encodeSchemaDefRequirementType |> List.ofSeq |> ySeq) ] |> yMap
        | DockerRequirement dr ->
            [ "class", yScalar "DockerRequirement" ]
            |> appendOpt "dockerPull" yScalar dr.DockerPull
            |> appendOpt "dockerFile" (fun m ->
                m |> Map.toList |> List.map (fun (k,v) -> k, yScalar v) |> yMap) dr.DockerFile
            |> appendOpt "dockerImageId" yScalar dr.DockerImageId
            |> yMap
        | SoftwareRequirement pkgs ->
            let encodePkg (p:SoftwarePackage) =
                []
                |> fun acc -> acc @ [ "package", yScalar p.Package ]
                |> appendOpt "version" (fun vs -> vs |> Seq.map yScalar |> List.ofSeq |> ySeq) p.Version
                |> appendOpt "specs" (fun vs -> vs |> Seq.map yScalar |> List.ofSeq |> ySeq) p.Specs
                |> yMap
            [ "class", yScalar "SoftwareRequirement";
              "packages", (pkgs |> Seq.map encodePkg |> List.ofSeq |> ySeq) ] |> yMap
        | InitialWorkDirRequirement listing ->
            let encodeDirent = function
                | Dirent d ->
                    let entryElement =
                        if d.Entry.Contains(": ") then
                            let parts = d.Entry.Split([|": "|], 2, StringSplitOptions.None)
                            if parts.Length = 2 then
                                yMap [ parts.[0], yScalar parts.[1] ]
                            else yScalar d.Entry
                        else yScalar d.Entry
                    [ ]
                    |> appendOpt "entryname" yScalar d.Entryname
                    |> fun acc -> acc @ [ "entry", entryElement ]
                    |> appendOpt "writable" yBool d.Writable
                    |> yMap
                | other -> encodeCWLType other // fallback
            [ "class", yScalar "InitialWorkDirRequirement";
              "listing", (listing |> Seq.map encodeDirent |> List.ofSeq |> ySeq) ] |> yMap
        | EnvVarRequirement envs ->
            let encodeEnv (e:EnvironmentDef) =
                let v = if e.EnvValue = "true" || e.EnvValue = "false" then "\"" + e.EnvValue + "\"" else e.EnvValue
                [ "envName", yScalar e.EnvName; "envValue", yScalar v ] |> yMap
            [ "class", yScalar "EnvVarRequirement";
              "envDef", (envs |> Seq.map encodeEnv |> List.ofSeq |> ySeq) ] |> yMap
        | ShellCommandRequirement -> [ "class", yScalar "ShellCommandRequirement" ] |> yMap
        | ResourceRequirement rr ->
            let dynamicPairs =
                rr.GetProperties(false)
                |> Seq.choose (fun kvp ->
                    match kvp.Value with
                    | :? int as i -> Some (kvp.Key, yInt i)
                    | :? float as f -> Some (kvp.Key, yFloat f)
                    | :? string as s -> Some (kvp.Key, yScalar s)
                    | :? bool as b -> Some (kvp.Key, yBool b)
                    | _ -> None)
                |> Seq.toList
            [ "class", yScalar "ResourceRequirement" ] @ dynamicPairs |> yMap
        | WorkReuseRequirement -> [ "class", yScalar "WorkReuse" ] |> yMap
        | NetworkAccessRequirement -> [ "class", yScalar "NetworkAccess"; "networkAccess", yBool true ] |> yMap
        | InplaceUpdateRequirement -> [ "class", yScalar "InplaceUpdateRequirement" ] |> yMap
        | ToolTimeLimitRequirement tl -> [ "class", yScalar "ToolTimeLimit"; "timelimit", yFloat tl ] |> yMap
        | SubworkflowFeatureRequirement -> [ "class", yScalar "SubworkflowFeatureRequirement" ] |> yMap
        | ScatterFeatureRequirement -> [ "class", yScalar "ScatterFeatureRequirement" ] |> yMap
        | MultipleInputFeatureRequirement -> [ "class", yScalar "MultipleInputFeatureRequirement" ] |> yMap
        | StepInputExpressionRequirement -> [ "class", yScalar "StepInputExpressionRequirement" ] |> yMap

    // ------------------------------
    // Workflow step encoders
    // ------------------------------
    let encodeStepInput (si:StepInput) : (string * YAMLElement) =
        let pairs =
            []
            |> appendOpt "source" yScalar si.Source
            |> appendOpt "default" yScalar si.DefaultValue
            |> appendOpt "valueFrom" yScalar si.ValueFrom
        match pairs with
        | [ ("source", s) ] when si.DefaultValue.IsNone && si.ValueFrom.IsNone -> si.Id, s
        | _ -> si.Id, yMap pairs

    let encodeStepInputs (inputs:ResizeArray<StepInput>) : YAMLElement =
        inputs
        |> Seq.map encodeStepInput
        |> Seq.toList
        |> yMap

    let encodeStepOutput (so:StepOutput) : YAMLElement =
        so.Id |> Seq.map yScalar |> List.ofSeq |> ySeq

    let encodeWorkflowStep (ws:WorkflowStep) : (string * YAMLElement) =
        let basePairs =
            [ "run", yScalar ws.Run
              "in", encodeStepInputs ws.In
              "out", encodeStepOutput ws.Out ]
        let withReq =
            match ws.Requirements with
            | Some r when r.Count > 0 -> basePairs @ [ "requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> ySeq) ]
            | _ -> basePairs
        let withHints =
            match ws.Hints with
            | Some h when h.Count > 0 -> withReq @ [ "hints", (h |> Seq.map encodeRequirement |> List.ofSeq |> ySeq) ]
            | _ -> withReq
        ws.Id, yMap withHints

    // ------------------------------
    // Top-level encoders
    // ------------------------------

    let private writeYaml (element:YAMLElement) =
        // Use whitespace=2 to match fixtures (assumed)
        YAMLicious.Writer.write element (Some (fun c -> { c with Whitespace = 2 }))

    let encodeToolDescription (td:CWLToolDescription) : string =
        // Build each top-level section separately to control blank line placement like fixtures
        let section (pairs:(string*YAMLElement) list) =
            pairs |> yMap |> writeYaml |> fun s -> s.Replace("\r\n","\n").TrimEnd('\n').Split('\n') |> Array.toList
        let baseLines = section ["cwlVersion", yScalar td.CWLVersion; "class", yScalar "CommandLineTool"]
        let hintsLines = td.Hints |> Option.map (fun h -> section ["hints", (h |> Seq.map encodeRequirement |> List.ofSeq |> ySeq)])
        let reqLines = td.Requirements |> Option.map (fun r -> section ["requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> ySeq)])
        let baseCommandLines = td.BaseCommand |> Option.map (fun bc -> section ["baseCommand", (bc |> Seq.map yScalar |> List.ofSeq |> ySeq)])
        let inputsLines = td.Inputs |> Option.map (fun i -> section ["inputs", (i |> Seq.map encodeCWLInput |> Seq.toList |> yMap)])
        let outputsLines = section ["outputs", (td.Outputs |> Seq.map encodeCWLOutput |> Seq.toList |> yMap)]
        let metadataLines =
            td.Metadata
            |> Option.map (fun md ->
                md.GetProperties(false)
                |> Seq.map (fun kvp -> kvp.Key, match kvp.Value with | :? string as s -> yScalar s | _ -> yScalar (string kvp.Value))
                |> Seq.toList |> yMap |> writeYaml |> fun s -> s.Replace("\r\n","\n").TrimEnd('\n').Split('\n') |> Array.toList)

        let resultLines = [
            yield! baseLines
            yield ""
            match hintsLines with | Some l -> yield! l; yield "" | None -> ()
            match reqLines with | Some l -> yield! l; yield "" | None -> ()
            match baseCommandLines with | Some l -> yield! l; yield "" | None -> ()
            match inputsLines with | Some l -> yield! l; yield "" | None -> ()
            yield! outputsLines
            match metadataLines with | Some l when l.Length>0 -> yield ""; yield! l | _ -> ()
        ]
        let output = String.Join("\r\n", resultLines)
        // Post-process to collapse sequence items that only contain mappings so that '- class:' appears on same line
        let lines = output.Split([|"\r\n"|], StringSplitOptions.None) |> Array.toList
        let rec merge (acc:string list) (remaining:string list) =
            match remaining with
            | a::b::rest when a.Trim() = "-" && b.TrimStart().Contains(":") ->
                // Merge dash with first mapping key line
                let merged = a + " " + b.Trim()
                merge (merged::acc) rest
            | l::rest -> merge (l::acc) rest
            | [] -> List.rev acc
        merge [] lines |> String.concat "\r\n"

    let encodeWorkflowDescription (wd:CWLWorkflowDescription) : string =
        let section (pairs:(string*YAMLElement) list) =
            pairs |> yMap |> writeYaml |> fun s -> s.Replace("\r\n","\n").TrimEnd('\n').Split('\n') |> Array.toList
        let baseLines = section ["cwlVersion", yScalar wd.CWLVersion; "class", yScalar "Workflow"]
        let reqLines = wd.Requirements |> Option.map (fun r -> section ["requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> ySeq)])
        let inputsLines = section ["inputs", (wd.Inputs |> Seq.map encodeCWLInput |> Seq.toList |> yMap)]
        let stepsLines = section ["steps", (wd.Steps |> Seq.map encodeWorkflowStep |> Seq.toList |> yMap)]
        let outputsLines = section ["outputs", (wd.Outputs |> Seq.map encodeCWLOutput |> Seq.toList |> yMap)]
        let metadataLines =
            wd.Metadata
            |> Option.map (fun md ->
                md.GetProperties(false)
                |> Seq.map (fun kvp -> kvp.Key, match kvp.Value with | :? string as s -> yScalar s | _ -> yScalar (string kvp.Value))
                |> Seq.toList |> yMap |> writeYaml |> fun s -> s.Replace("\r\n","\n").TrimEnd('\n').Split('\n') |> Array.toList)

        let resultLines = [
            yield! baseLines
            yield ""
            match reqLines with | Some l -> yield! l; yield "" | None -> ()
            yield! inputsLines; yield ""
            yield! stepsLines; yield ""
            yield! outputsLines
            match metadataLines with | Some l when l.Length>0 -> yield ""; yield! l | _ -> ()
        ]
        let output = String.Join("\r\n", resultLines)
        let lines = output.Split([|"\r\n"|], StringSplitOptions.None) |> Array.toList
        let rec merge (acc:string list) (remaining:string list) =
            match remaining with
            | a::b::rest when a.Trim() = "-" && b.TrimStart().Contains(":") ->
                let merged = a + " " + b.Trim()
                merge (merged::acc) rest
            | l::rest -> merge (l::acc) rest
            | [] -> List.rev acc
        merge [] lines |> String.concat "\r\n"


