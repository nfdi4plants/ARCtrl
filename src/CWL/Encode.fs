namespace ARCtrl.CWL

module CWLYamlEncoder =

    open ARCtrl.CWL
    open YAMLicious.YAMLiciousTypes
    open YAMLicious.Writer

    let yamlStr s = YAMLContent.create s

    let rec encodeCWLType (t: CWLType) : YAMLElement =
        match t with
        | CWLType.String -> YAMLElement.Value (yamlStr "string")
        | CWLType.Int -> YAMLElement.Value (yamlStr "int")
        | CWLType.Long -> YAMLElement.Value (yamlStr "long")
        | CWLType.Float -> YAMLElement.Value (yamlStr "float")
        | CWLType.Double -> YAMLElement.Value (yamlStr "double")
        | CWLType.Boolean -> YAMLElement.Value (yamlStr "boolean")
        | CWLType.Null -> YAMLElement.Value (yamlStr "null")
        | CWLType.File _ -> YAMLElement.Value (yamlStr "File")
        | CWLType.Directory _ -> YAMLElement.Value (yamlStr "Directory")
        | CWLType.Stdout -> YAMLElement.Value (yamlStr "stdout")
        | CWLType.Dirent _ -> YAMLElement.Value (yamlStr "Dirent")
        | CWLType.Array inner ->
            YAMLElement.Object [
                YAMLElement.Mapping(yamlStr "type", YAMLElement.Value (yamlStr "array"));
                YAMLElement.Mapping(yamlStr "items", encodeCWLType inner)
            ]

    let encodeInput (i: CWLInput) : YAMLElement =
        let elements =
            [
                match i.Type_ with
                | Some t -> YAMLElement.Mapping(yamlStr "type", encodeCWLType t)
                | None -> ()
                match i.InputBinding with
                | Some b ->
                    let bindingFields =
                        [ match b.Position with Some p -> YAMLElement.Mapping(yamlStr "position", YAMLElement.Value (yamlStr (string p))) | None -> () ]
                    YAMLElement.Mapping(yamlStr "inputBinding", YAMLElement.Object bindingFields)
                | None -> ()
            ]
        YAMLElement.Mapping(yamlStr i.Name, YAMLElement.Object elements)

    let encodeOutput (o: CWLOutput) : YAMLElement =
        let elements =
            [
                match o.Type_ with
                | Some t -> YAMLElement.Mapping(yamlStr "type", encodeCWLType t)
                | None -> ()
                match o.OutputBinding with
                | Some b ->
                    match b.Glob with
                    | Some g -> YAMLElement.Mapping(yamlStr "outputBinding", YAMLElement.Object [YAMLElement.Mapping(yamlStr "glob", YAMLElement.Value (yamlStr g))])
                    | None -> ()
                | None -> ()
                match o.OutputSource with
                | Some src -> YAMLElement.Mapping(yamlStr "outputSource", YAMLElement.Value (yamlStr src))
                | None -> ()
            ]
        YAMLElement.Mapping(yamlStr o.Name, YAMLElement.Object elements)

    let encodeRequirement (r: Requirement) : YAMLElement option =
        let classField name = YAMLElement.Mapping(yamlStr "class", YAMLElement.Value (yamlStr name))
        match r with
        | Requirement.InlineJavascriptRequirement -> Some (YAMLElement.Object [classField "InlineJavascriptRequirement"])
        | Requirement.SchemaDefRequirement _ -> Some (YAMLElement.Object [classField "SchemaDefRequirement"])
        | Requirement.DockerRequirement d ->
            Some (YAMLElement.Object ([classField "DockerRequirement"] @
                (match d.DockerPull with Some v -> [YAMLElement.Mapping(yamlStr "dockerPull", YAMLElement.Value (yamlStr v))] | None -> []) @
                (match d.DockerImageId with Some v -> [YAMLElement.Mapping(yamlStr "dockerImageId", YAMLElement.Value (yamlStr v))] | None -> []) @
                (match d.DockerFile with Some m -> m |> Map.toList |> List.map (fun (k,v) -> YAMLElement.Mapping(yamlStr k, YAMLElement.Value (yamlStr v))) | None -> [])))
        | Requirement.SoftwareRequirement pkgs ->
            let pkgList =
                pkgs
                |> Seq.map (fun pkg ->
                    let fields = [YAMLElement.Mapping(yamlStr "package", YAMLElement.Value (yamlStr pkg.Package))] @
                                 (match pkg.Version with Some v -> [YAMLElement.Mapping(yamlStr "version", YAMLElement.Value (yamlStr (String.concat "," v)))] | None -> []) @
                                 (match pkg.Specs with Some s -> [YAMLElement.Mapping(yamlStr "specs", YAMLElement.Value (yamlStr (String.concat "," s)))] | None -> [])
                    YAMLElement.Object fields)
                |> List.ofSeq
            Some (YAMLElement.Object [classField "SoftwareRequirement"; YAMLElement.Mapping(yamlStr "packages", YAMLElement.Sequence pkgList)])
        | Requirement.InitialWorkDirRequirement items ->
            Some (YAMLElement.Object [classField "InitialWorkDirRequirement"; YAMLElement.Mapping(yamlStr "listing", YAMLElement.Sequence (items |> Seq.map encodeCWLType |> List.ofSeq))])
        | Requirement.EnvVarRequirement envs ->
            let entries =
                envs
                |> Seq.map (fun e -> YAMLElement.Object [YAMLElement.Mapping(yamlStr "envName", YAMLElement.Value (yamlStr e.EnvName)); YAMLElement.Mapping(yamlStr "envYAMLElement.Value", YAMLElement.Value (yamlStr e.EnvValue))])
                |> List.ofSeq
            Some (YAMLElement.Object [classField "EnvVarRequirement"; YAMLElement.Mapping(yamlStr "envDef", YAMLElement.Sequence entries)])
        | Requirement.ShellCommandRequirement -> Some (YAMLElement.Object [classField "ShellCommandRequirement"])
        | Requirement.ResourceRequirement _ -> Some (YAMLElement.Object [classField "ResourceRequirement"]) // TODO: Detailed fields
        | Requirement.WorkReuseRequirement -> Some (YAMLElement.Object [classField "WorkReuseRequirement"])
        | Requirement.NetworkAccessRequirement -> Some (YAMLElement.Object [classField "NetworkAccess"])
        | Requirement.InplaceUpdateRequirement -> Some (YAMLElement.Object [classField "InplaceUpdateRequirement"])
        | Requirement.ToolTimeLimitRequirement limit -> Some (YAMLElement.Object [classField "ToolTimeLimitRequirement"; YAMLElement.Mapping(yamlStr "timelimit", YAMLElement.Value (yamlStr (string limit)))])
        | Requirement.SubworkflowFeatureRequirement -> Some (YAMLElement.Object [classField "SubworkflowFeatureRequirement"])
        | Requirement.ScatterFeatureRequirement -> Some (YAMLElement.Object [classField "ScatterFeatureRequirement"])
        | Requirement.MultipleInputFeatureRequirement -> Some (YAMLElement.Object [classField "MultipleInputFeatureRequirement"])
        | Requirement.StepInputExpressionRequirement -> Some (YAMLElement.Object [classField "StepInputExpressionRequirement"])

    let encodeTool (tool: CWLToolDescription) : YAMLElement =
        let inputs =
            tool.Inputs
            |> Option.defaultValue (ResizeArray())
            |> Seq.map encodeInput
            |> List.ofSeq

        let outputs =
            tool.Outputs
            |> Seq.map encodeOutput
            |> List.ofSeq

        let requirements =
            tool.Requirements
            |> Option.defaultValue (ResizeArray())
            |> Seq.choose encodeRequirement
            |> fun reqs -> YAMLElement.Mapping(yamlStr "requirements", YAMLElement.Sequence (List.ofSeq reqs))

        let hints =
            tool.Hints
            |> Option.defaultValue (ResizeArray())
            |> Seq.choose encodeRequirement
            |> fun hs -> YAMLElement.Mapping(yamlStr "hints", YAMLElement.Sequence (List.ofSeq hs))

        let baseCommand =
            tool.BaseCommand
            |> Option.defaultValue (ResizeArray())
            |> Seq.map (fun s -> YAMLElement.Value (yamlStr s))
            |> List.ofSeq
            |> fun cmd -> YAMLElement.Mapping(yamlStr "baseCommand", YAMLElement.Sequence cmd)

        YAMLElement.Object [
            YAMLElement.Mapping(yamlStr "cwlVersion", YAMLElement.Value (yamlStr tool.CWLVersion))
            YAMLElement.Mapping(yamlStr "class", YAMLElement.Value (yamlStr "CommandLineTool"))
            baseCommand
            requirements
            hints
            YAMLElement.Mapping(yamlStr "inputs", YAMLElement.Object inputs)
            YAMLElement.Mapping(yamlStr "outputs", YAMLElement.Object outputs)
        ]

    let encodeWorkflow (wf: CWLWorkflowDescription) : YAMLElement =
        let inputs =
            wf.Inputs
            |> Seq.map encodeInput
            |> List.ofSeq

        let outputs =
            wf.Outputs
            |> Seq.map encodeOutput
            |> List.ofSeq

        let requirements =
            wf.Requirements
            |> Option.defaultValue (ResizeArray())
            |> Seq.choose encodeRequirement
            |> fun reqs -> YAMLElement.Mapping(yamlStr "requirements", YAMLElement.Sequence (List.ofSeq reqs))

        //let steps =
        //    wf.Steps
        //    |> Seq.map (fun (KeyValue(name, step)) ->
        //        let inputs =
        //            step.In
        //            |> Option.defaultValue Map.empty
        //            |> Seq.map (fun (KeyValue(k, v)) -> Mapping(yamlStr k, Value (yamlStr v)))
        //            |> List.ofSeq

        //        let outputs =
        //            step.Out
        //            |> Option.defaultValue []
        //            |> List.map (fun s -> YAMLElement.Value (yamlStr s))

        //        YAMLElement.Mapping(yamlStr name,
        //            YAMLElement.Object [
        //                Mapping(yamlStr "run", Value (yamlStr step.Run))
        //                Mapping(yamlStr "in", Object inputs)
        //                Mapping(yamlStr "out", Sequence outputs)
        //            ])
        //    )
        //    |> List.ofSeq

        YAMLElement.Object [
            YAMLElement.Mapping(yamlStr "cwlVersion", YAMLElement.Value (yamlStr wf.CWLVersion))
            YAMLElement.Mapping(yamlStr "class", YAMLElement.Value (yamlStr "Workflow"))
            requirements
            YAMLElement.Mapping(yamlStr "inputs", YAMLElement.Object inputs)
            YAMLElement.Mapping(yamlStr "outputs", YAMLElement.Object outputs)
            //YAMLElement.Mapping(yamlStr "steps", YAMLElement.Object steps)
        ]