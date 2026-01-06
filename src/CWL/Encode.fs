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
    // Basic boolean encoder with lowercase letters
    // ------------------------------
    let yBool (b:bool) =
        YAMLElement.Value { Value = (if b then "true" else "false"); Comment = None }

    // ------------------------------
    // Helper to build YAML mappings preserving order
    // ------------------------------
    let yMap (pairs: (string * YAMLElement) list) =
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

    let normalizeDocString (doc:string) =
        doc.Replace("\r\n","\n").TrimEnd('\n').TrimEnd('\r')

    let encodeLabel (label:string) : (string * YAMLElement) =
        "label", Encode.string label

    let encodeDoc (doc:string) : (string * YAMLElement) =
        "doc", Encode.string (normalizeDocString doc)

    let inline private appendOptPair pairOpt acc =
        match pairOpt with
        | Some pair -> acc @ [pair]
        | None -> acc

    // ------------------------------
    // CWLType encoder
    // ------------------------------
    let rec encodeCWLType (t:CWLType) : YAMLElement =
        match t with
        | File _ -> Encode.string "File"
        | Directory _ -> Encode.string "Directory"
        | Dirent d ->
            [ "entry", Encode.string d.Entry ]
            |> appendOpt "entryname" Encode.string d.Entryname
            |> appendOpt "writable" (fun b -> yBool b) d.Writable
            |> yMap
        | String -> Encode.string "string"
        | Int -> Encode.string "int"
        | Long -> Encode.string "long"
        | Float -> Encode.string "float"
        | Double -> Encode.string "double"
        | Boolean -> Encode.string "boolean"
        | Stdout -> Encode.string "stdout"
        | Null -> Encode.string "null"
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
            | Some s -> Encode.string s
            | None -> [ "type", Encode.string "array"; "items", encodeCWLType inner ] |> yMap

    // ------------------------------
    // Binding & Port encoders
    // ------------------------------

    let encodeOutputBinding (ob:OutputBinding) : YAMLElement =
        [ ob.Glob |> Option.map (fun g -> "glob", Encode.string g) ]
        |> List.choose id
        |> yMap

    let encodeCWLOutput (o:CWLOutput) : (string * YAMLElement) =
        let pairs =
            []
            |> appendOpt "type" (fun t -> encodeCWLType t) o.Type_
            |> appendOpt "outputBinding" encodeOutputBinding o.OutputBinding
            |> appendOpt "outputSource" Encode.string o.OutputSource
        match pairs with
        | [ ("type", t) ] -> o.Name, t // only type specified
        | _ ->
            // Always extended form when additional fields like outputSource/outputBinding present
            o.Name, (yMap pairs)

    let encodeInputBinding (ib:InputBinding) : YAMLElement =
        []
        |> appendOpt "prefix" Encode.string ib.Prefix
        |> appendOpt "position" (fun p -> Encode.int p) ib.Position
        |> appendOpt "itemSeparator" Encode.string ib.ItemSeparator
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
            | :? string as str -> Some (kvp.Key, Encode.string str)
            | _ -> None)
        |> Seq.toList
        |> yMap

    let encodeRequirement (r:Requirement) : YAMLElement =
        match r with
        | InlineJavascriptRequirement -> [ "class", Encode.string "InlineJavascriptRequirement" ] |> yMap
        | SchemaDefRequirement types ->
            [ "class", Encode.string "SchemaDefRequirement";
              "types", (types |> Seq.map encodeSchemaDefRequirementType |> List.ofSeq |> YAMLElement.Sequence) ] |> yMap
        | DockerRequirement dr ->
            [ "class", Encode.string "DockerRequirement" ]
            |> appendOpt "dockerPull" Encode.string dr.DockerPull
            |> appendOpt "dockerFile" (fun m ->
                m |> Map.toList |> List.map (fun (k,v) -> k, Encode.string v) |> yMap) dr.DockerFile
            |> appendOpt "dockerImageId" Encode.string dr.DockerImageId
            |> yMap
        | SoftwareRequirement pkgs ->
            let encodePkg (p:SoftwarePackage) =
                []
                |> fun acc -> acc @ [ "package", Encode.string p.Package ]
                |> appendOpt "version" (fun vs -> vs |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence) p.Version
                |> appendOpt "specs" (fun vs -> vs |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence) p.Specs
                |> yMap
            [ "class", Encode.string "SoftwareRequirement";
              "packages", (pkgs |> Seq.map encodePkg |> List.ofSeq |> YAMLElement.Sequence) ] |> yMap
        | InitialWorkDirRequirement listing ->
            let encodeDirent = function
                | Dirent d ->
                    let entryElement =
                        if d.Entry.Contains(": ") then
                            let parts = d.Entry.Split([|": "|], 2, StringSplitOptions.None)
                            if parts.Length = 2 then
                                yMap [ parts.[0], Encode.string parts.[1] ]
                            else Encode.string d.Entry
                        else Encode.string d.Entry
                    [ ]
                    |> appendOpt "entryname" Encode.string d.Entryname
                    |> fun acc -> acc @ [ "entry", entryElement ]
                    |> appendOpt "writable" yBool d.Writable
                    |> yMap
                | other -> encodeCWLType other // fallback
            [ "class", Encode.string "InitialWorkDirRequirement";
              "listing", (listing |> Seq.map encodeDirent |> List.ofSeq |> YAMLElement.Sequence) ] |> yMap
        | EnvVarRequirement envs ->
            let encodeEnv (e:EnvironmentDef) =
                let v = if e.EnvValue = "true" || e.EnvValue = "false" then "\"" + e.EnvValue + "\"" else e.EnvValue
                [ "envName", Encode.string e.EnvName; "envValue", Encode.string v ] |> yMap
            [ "class", Encode.string "EnvVarRequirement";
              "envDef", (envs |> Seq.map encodeEnv |> List.ofSeq |> YAMLElement.Sequence) ] |> yMap
        | ShellCommandRequirement -> [ "class", Encode.string "ShellCommandRequirement" ] |> yMap
        | ResourceRequirement rr ->
            let dynamicPairs =
                rr.GetProperties(false)
                |> Seq.choose (fun kvp ->
                    match kvp.Value with
                    | :? int as i -> Some (kvp.Key, Encode.int i)
                    | :? float as f -> Some (kvp.Key, Encode.float f)
                    | :? string as s -> Some (kvp.Key, Encode.string s)
                    | :? bool as b -> Some (kvp.Key, yBool b)
                    | _ -> None)
                |> Seq.toList
            [ "class", Encode.string "ResourceRequirement" ] @ dynamicPairs |> yMap
        | WorkReuseRequirement -> [ "class", Encode.string "WorkReuse" ] |> yMap
        | NetworkAccessRequirement -> [ "class", Encode.string "NetworkAccess"; "networkAccess", yBool true ] |> yMap
        | InplaceUpdateRequirement -> [ "class", Encode.string "InplaceUpdateRequirement" ] |> yMap
        | ToolTimeLimitRequirement tl -> [ "class", Encode.string "ToolTimeLimit"; "timelimit", Encode.float tl ] |> yMap
        | SubworkflowFeatureRequirement -> [ "class", Encode.string "SubworkflowFeatureRequirement" ] |> yMap
        | ScatterFeatureRequirement -> [ "class", Encode.string "ScatterFeatureRequirement" ] |> yMap
        | MultipleInputFeatureRequirement -> [ "class", Encode.string "MultipleInputFeatureRequirement" ] |> yMap
        | StepInputExpressionRequirement -> [ "class", Encode.string "StepInputExpressionRequirement" ] |> yMap

    // ------------------------------
    // Workflow step encoders
    // ------------------------------
    
    /// Encode a ResizeArray<string> as either a single string or a sequence
    let encodeSourceArray (sources:ResizeArray<string>) : YAMLElement =
        match sources.Count with
        | 1 -> Encode.string sources.[0]
        | _ -> 
            // Create sequence with each item as an Object[Value] to force block-style rendering
            sources 
            |> Seq.map (fun s -> YAMLElement.Object [YAMLElement.Value { Value = s; Comment = None }])
            |> List.ofSeq 
            |> YAMLElement.Sequence
    
    let encodeStepInput (si:StepInput) : (string * YAMLElement) =
        let pairs =
            []
            |> appendOpt "source" encodeSourceArray si.Source
            |> appendOpt "default" Encode.string si.DefaultValue
            |> appendOpt "valueFrom" Encode.string si.ValueFrom
            |> appendOpt "linkMerge" Encode.string si.LinkMerge
        match pairs with
        | [ ("source", s) ] when si.DefaultValue.IsNone && si.ValueFrom.IsNone && si.LinkMerge.IsNone -> si.Id, s
        | _ -> si.Id, yMap pairs

    let encodeStepInputs (inputs:ResizeArray<StepInput>) : YAMLElement =
        inputs
        |> Seq.map encodeStepInput
        |> Seq.toList
        |> yMap

    let encodeStepOutput (so:StepOutput) : YAMLElement =
        so.Id |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence

    let encodeWorkflowStep (ws:WorkflowStep) : (string * YAMLElement) =
        let basePairs =
            [ "run", Encode.string ws.Run
              "in", encodeStepInputs ws.In
              "out", encodeStepOutput ws.Out ]
        let withReq =
            match ws.Requirements with
            | Some r when r.Count > 0 -> basePairs @ [ "requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> basePairs
        let withHints =
            match ws.Hints with
            | Some h when h.Count > 0 -> withReq @ [ "hints", (h |> Seq.map encodeRequirement |> List.ofSeq |> YAMLElement.Sequence) ]
            | _ -> withReq
        ws.Id, yMap withHints

    // ------------------------------
    // Top-level encoders
    // ------------------------------

    let writeYaml (element:YAMLElement) =
        // Use whitespace=2 to match fixtures (assumed)
        YAMLicious.Writer.write element (Some (fun c -> { c with Whitespace = 2 }))

    let encodeToolDescription (td:CWLToolDescription) : string =
        // Build each top-level section separately to control blank line placement like fixtures
        let section (pairs:(string*YAMLElement) list) =
            pairs |> yMap |> writeYaml |> fun s -> s.Replace("\r\n","\n").TrimEnd('\n').Split('\n') |> Array.toList
        let basePairs =
            [ "cwlVersion", Encode.string td.CWLVersion; "class", Encode.string "CommandLineTool" ]
            |> appendOptPair (td.Label |> Option.map encodeLabel)
            |> appendOptPair (td.Doc |> Option.map encodeDoc)
        let baseLines = section basePairs
        let hintsLines = td.Hints |> Option.map (fun h -> section ["hints", (h |> Seq.map encodeRequirement |> List.ofSeq |> YAMLElement.Sequence)])
        let reqLines = td.Requirements |> Option.map (fun r -> section ["requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> YAMLElement.Sequence)])
        let baseCommandLines = td.BaseCommand |> Option.map (fun bc -> section ["baseCommand", (bc |> Seq.map Encode.string |> List.ofSeq |> YAMLElement.Sequence)])
        let inputsLines = td.Inputs |> Option.map (fun i -> section ["inputs", (i |> Seq.map encodeCWLInput |> Seq.toList |> yMap)])
        let outputsLines = section ["outputs", (td.Outputs |> Seq.map encodeCWLOutput |> Seq.toList |> yMap)]
        let metadataLines =
            td.Metadata
            |> Option.map (fun md ->
                md.GetProperties(false)
                |> Seq.map (fun kvp -> kvp.Key, match kvp.Value with | :? string as s -> Encode.string s | _ -> Encode.string (string kvp.Value))
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
        let basePairs =
            [ "cwlVersion", Encode.string wd.CWLVersion; "class", Encode.string "Workflow" ]
            |> appendOptPair (wd.Label |> Option.map encodeLabel)
            |> appendOptPair (wd.Doc |> Option.map encodeDoc)
        let baseLines = section basePairs
        let reqLines = wd.Requirements |> Option.map (fun r -> section ["requirements", (r |> Seq.map encodeRequirement |> List.ofSeq |> YAMLElement.Sequence)])
        let inputsLines = section ["inputs", (wd.Inputs |> Seq.map encodeCWLInput |> Seq.toList |> yMap)]
        let stepsLines = section ["steps", (wd.Steps |> Seq.map encodeWorkflowStep |> Seq.toList |> yMap)]
        let outputsLines = section ["outputs", (wd.Outputs |> Seq.map encodeCWLOutput |> Seq.toList |> yMap)]
        let metadataLines =
            wd.Metadata
            |> Option.map (fun md ->
                md.GetProperties(false)
                |> Seq.map (fun kvp -> kvp.Key, match kvp.Value with | :? string as s -> Encode.string s | _ -> Encode.string (string kvp.Value))
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


    let encodeProcessingUnit (pu : CWLProcessingUnit) :string =
        match pu with
        | CommandLineTool td -> encodeToolDescription td
        | Workflow wd -> encodeWorkflowDescription wd