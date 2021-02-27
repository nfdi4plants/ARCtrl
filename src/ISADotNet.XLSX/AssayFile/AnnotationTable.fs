namespace ISADotNet.XLSX.AssayFile

open ISADotNet.XLSX
open ISADotNet

module AnnotationTable = 
    


    let splitBySamples (sheetName:string) (headers : seq<string>) =
        let isSample header = AnnotationColumn.tryParseSampleName header |> Option.isSome 
        let isSource header = AnnotationColumn.tryParseSourceName header |> Option.isSome 
        
        match Seq.filter isSource headers |> Seq.length, Seq.filter isSample headers |> Seq.length with
        | 1,1  | 0,1 -> headers |> Seq.singleton
        | 0,2 when Seq.head headers |> isSample && Seq.last headers |> isSample -> headers |> Seq.singleton
        | _ -> Seq.groupWhen false (fun header -> isSample header || isSource header) headers

    let splitByNamedProtocols (namedProtocols : (Protocol * seq<string>) seq) (headers : seq<string>) =
        let isSample (header:string) = header.Contains "Sample" || header.Contains "Source"
    
        let rec loop (protocolOverlaps : (Protocol option * seq<string>) list) (namedProtocols : (Protocol * Set<string>) list) (remainingHeaders : Set<string>) =
            match namedProtocols with
            | _ when remainingHeaders.IsEmpty -> 
                protocolOverlaps
            | (p,hs)::l ->
                if Set.isSubset hs remainingHeaders then
                    loop ((Some p,Set.toSeq hs)::protocolOverlaps) l (Set.difference remainingHeaders hs)
                else 
                    loop protocolOverlaps l remainingHeaders
            | [] ->
                (None ,remainingHeaders |> Set.toSeq)::protocolOverlaps
        
        let sampleColumns,otherColumns = headers |> Seq.filter (isSample) |> Seq.toList,headers |> Seq.filter (isSample>>not)
    
        let protocolOverlaps = loop [] (namedProtocols |> Seq.map (fun (p,hs) -> p,hs |> Set.ofSeq) |> List.ofSeq) (otherColumns |> Set.ofSeq)
        
        match sampleColumns with
        | [] -> protocolOverlaps
        | [s] -> protocolOverlaps |> List.map (fun (p,hs) -> p,Seq.append [s] hs)
        | [s1;s2] -> protocolOverlaps |> List.map (fun (p,hs) -> p,Seq.append (Seq.append [s1] hs) [s2])
        | s -> protocolOverlaps |> List.map (fun (p,hs) -> p,Seq.append hs s)

    let indexProtocolsBySheetName (sheetName:string) (protocols : (Protocol * seq<string>) seq) =
        let unnamedProtocolCount = protocols |> Seq.filter (fun (p,_) -> p.Name.IsNone) |> Seq.length
        match unnamedProtocolCount with
        | 0 -> protocols
        | 1 -> 
            protocols 
            |> Seq.map (fun (p,hs) -> 
                if p.Name.IsNone then
                    {p with Name = Some sheetName},hs
                else p,hs
            )
        | _ -> 
            let mutable i = 0 
            protocols 
            |> Seq.map (fun (p,hs) -> 
                if p.Name.IsNone then
                    let name = sprintf "%s_%i" sheetName i
                    i <- i + 1
                    {p with Name = Some sheetName},hs
                else p,hs
            )

    let getProcessGetter protocolMetaData (nodes : seq<seq<string>>) =
    
        let characteristics,characteristicValueGetters =
            nodes |> Seq.choose AnnotationNode.tryGetCharacteristicGetterFunction
            |> Seq.fold (fun (cl,cvl) (c,cv) -> c.Value :: cl, cv :: cvl) ([],[])
            |> fun (l1,l2) -> List.rev l1, List.rev l2
        let factors,factorValueGetters =
            nodes |> Seq.choose AnnotationNode.tryGetFactorGetterFunction
            |> Seq.fold (fun (fl,fvl) (f,fv) -> f.Value :: fl, fv :: fvl) ([],[])
            |> fun (l1,l2) -> List.rev l1, List.rev l2
        let parameters,parameterValueGetters =
            nodes |> Seq.choose AnnotationNode.tryGetParameterGetterFunction
            |> Seq.fold (fun (pl,pvl) (p,pv) -> p.Value :: pl, pv :: pvl) ([],[])
            |> fun (l1,l2) -> List.rev l1, List.rev l2
    
        let inputGetter,outputGetter =
            match nodes |> Seq.tryPick AnnotationNode.tryGetSourceNameGetter with
            | Some inputNameGetter ->
                let outputNameGetter = nodes |> Seq.tryPick AnnotationNode.tryGetSampleNameGetter
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
                let inputNameGetter = nodes |> Seq.head |> AnnotationNode.tryGetSampleNameGetter
                let outputNameGetter = nodes |> Seq.last |> AnnotationNode.tryGetSampleNameGetter
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
    
    