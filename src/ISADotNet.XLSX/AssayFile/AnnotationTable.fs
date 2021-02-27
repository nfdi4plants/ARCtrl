namespace ISADotNet.XLSX.AssayFile

open ISADotNet

module AnnotationTable = 
   

    let splitIntoProtocols (sheetName:string) (namedProtocols : (Protocol * seq<string>) seq) (headers : seq<string>) =
        seq {
            Protocol.create None (Some sheetName) None None None None None None None, headers
        }


    let getProcessGetter protocolMetaData (columnGroup : seq<seq<string>>) =
    
        let characteristics,characteristicValueGetters =
            columnGroup |> Seq.choose AnnotationNode.tryGetCharacteristicGetterFunction
            |> Seq.fold (fun (cl,cvl) (c,cv) -> c.Value :: cl, cv :: cvl) ([],[])
            |> fun (l1,l2) -> List.rev l1, List.rev l2
        let factors,factorValueGetters =
            columnGroup |> Seq.choose AnnotationNode.tryGetFactorGetterFunction
            |> Seq.fold (fun (fl,fvl) (f,fv) -> f.Value :: fl, fv :: fvl) ([],[])
            |> fun (l1,l2) -> List.rev l1, List.rev l2
        let parameters,parameterValueGetters =
            columnGroup |> Seq.choose AnnotationNode.tryGetParameterGetterFunction
            |> Seq.fold (fun (pl,pvl) (p,pv) -> p.Value :: pl, pv :: pvl) ([],[])
            |> fun (l1,l2) -> List.rev l1, List.rev l2
    
        let inputGetter,outputGetter =
            match columnGroup |> Seq.tryPick AnnotationNode.tryGetSourceNameGetter with
            | Some inputNameGetter ->
                let outputNameGetter = columnGroup |> Seq.tryPick AnnotationNode.tryGetSampleNameGetter
                let inputGetter = 
                    fun matrix i -> 
                        Source.create
                            None
                            (inputNameGetter matrix i)
                            (characteristicValueGetters |> List.map (fun f -> f matrix i) |> API.Option.fromValueWithDefault [])
                let outputGetter =
                    fun matrix i -> 
                        Sample.create
                            None
                            (outputNameGetter |> Option.bind (fun o -> o matrix i))
                            (characteristicValueGetters |> List.map (fun f -> f matrix i) |> API.Option.fromValueWithDefault [])
                            (factorValueGetters |> List.map (fun f -> f matrix i) |> API.Option.fromValueWithDefault [])
                            (inputGetter matrix i |> List.singleton |> Some)
                         |> Sample
                (fun matrix i -> inputGetter matrix i |> Source),outputGetter
            | None ->
                let inputNameGetter = columnGroup |> Seq.head |> AnnotationNode.tryGetSampleNameGetter
                let outputNameGetter = columnGroup |> Seq.last |> AnnotationNode.tryGetSampleNameGetter
                let inputGetter = 
                    fun matrix i -> 
                        Sample.create
                            None
                            (inputNameGetter |> Option.bind (fun o -> o matrix i))
                            (characteristicValueGetters |> List.map (fun f -> f matrix i) |> API.Option.fromValueWithDefault [])
                            None
                            None
                        |> ProcessInput.Sample
                let outputGetter =
                    fun matrix i -> 
                        Sample.create
                            None
                            (outputNameGetter |> Option.bind (fun o -> o matrix i))
                            (characteristicValueGetters |> List.map (fun f -> f matrix i) |> API.Option.fromValueWithDefault [])
                            (factorValueGetters |> List.map (fun f -> f matrix i) |> API.Option.fromValueWithDefault [])
                            None
                        |> Sample
                inputGetter,outputGetter
    
        let protocol = {protocolMetaData with Parameters = API.Option.fromValueWithDefault [] parameters}
    
        characteristics,
        factors,
        protocol,
        fun (matrix : System.Collections.Generic.Dictionary<(string * int),string>) i ->
            Process.create 
                None 
                None 
                (Some protocol) 
                (parameterValueGetters |> List.map (fun f -> f matrix i) |> API.Option.fromValueWithDefault [])
                None
                None
                None
                None          
                (inputGetter matrix i |> List.singleton |> Some)
                (outputGetter matrix i |> List.singleton |> Some)
                None

    let mergeIdenticalProcesses (processes : seq<Process>) =
        processes
        |> Seq.groupBy (fun p -> p.ExecutesProtocol,p.ParameterValues)
        |> Seq.map (fun (_,processGroup) ->
            processGroup
            |> Seq.reduce (fun p1 p2 ->
                let mergedInputs = List.append (p1.Inputs |> Option.defaultValue []) (p2.Inputs |> Option.defaultValue []) |> API.Option.fromValueWithDefault []
                let mergedOutputs = List.append (p1.Outputs |> Option.defaultValue []) (p2.Outputs |> Option.defaultValue []) |> API.Option.fromValueWithDefault []
                {p1 with Inputs = mergedInputs; Outputs = mergedOutputs}
            )
        )

    let indexRelatedProcessesByProtocolName (processes : seq<Process>) =
        processes
        |> Seq.groupBy (fun p -> p.ExecutesProtocol)
        |> Seq.collect (fun (protocol,processGroup) ->
            processGroup
            |> Seq.mapi (fun i p -> 
                {p with Name = 
                        protocol.Value.Name |> Option.map (fun s -> sprintf "%s_%i" s i)
                }
            )
        )
    
    