namespace ISADotNet.XLSX.AssayFile

open ISADotNet.XLSX
open ISADotNet

/// Functions for parsing an annotation table to the described processes
module AnnotationTable = 
    
    /// Splits the headers of an annotation table into parts, so that each part has at most one input and one output column (Source Name, Sample Name)
    let splitBySamples (headers : seq<string>) =
        let isSample header = AnnotationColumn.tryParseSampleName header |> Option.isSome 
        let isSource header = AnnotationColumn.tryParseSourceName header |> Option.isSome 
        
        match Seq.filter isSource headers |> Seq.length, Seq.filter isSample headers |> Seq.length with
        | 1,1  -> 
            Seq.filter isSample headers
            |> Seq.append (Seq.filter (fun s -> (isSample s || isSource s) |> not) headers) 
            |> Seq.append (Seq.filter isSource headers)
            |> Seq.singleton
        | 0,1 -> Seq.append (Seq.filter (isSample>>not) headers) (Seq.filter isSample headers) |> Seq.singleton
        | 0,2 when Seq.head headers |> isSample && Seq.last headers |> isSample -> headers |> Seq.singleton
        | _ -> Seq.groupWhen true (fun header -> isSample header || isSource header) headers

    /// Splits the parts into protocols according to the headers given together with the named protocols. Assins the input and output column to each resulting protocol
    let splitByNamedProtocols (namedProtocols : (Protocol * seq<string>) seq) (headers : seq<string>) =
        let sortAgainst =
            let m = headers |> Seq.mapi (fun i x -> x,i) |> Map.ofSeq
            fun hs -> hs |> Seq.sortBy (fun v -> m.[v])
        let isSample (header:string) = AnnotationColumn.isSample header || AnnotationColumn.isSource header

        let rec loop (protocolOverlaps : (Protocol * seq<string>) list) (namedProtocols : (Protocol * Set<string>) list) (remainingHeaders : Set<string>) =
            match namedProtocols with
            | _ when remainingHeaders.IsEmpty -> 
                protocolOverlaps
            | (p,hs)::l ->
                if Set.isSubset hs remainingHeaders then
                    loop ((p,Set.toSeq hs)::protocolOverlaps) l (Set.difference remainingHeaders hs)
                else 
                    loop protocolOverlaps l remainingHeaders
            | [] ->
                (Protocol.empty ,remainingHeaders |> Set.toSeq)::protocolOverlaps
        
        let sampleColumns,otherColumns = headers |> Seq.filter (isSample) |> Seq.toList,headers |> Seq.filter (isSample>>not)
    
        let protocolOverlaps = 
            loop [] (namedProtocols |> Seq.map (fun (p,hs) -> p,hs |> Set.ofSeq) |> List.ofSeq) (otherColumns |> Set.ofSeq)
            |> Seq.map (fun (p,hs) -> p, sortAgainst hs)
        
        match sampleColumns with
        | [] ->         protocolOverlaps 
        | [s] ->        protocolOverlaps |> Seq.map (fun (p,hs) -> p,Seq.append [s] hs)
        | [s1;s2] ->    protocolOverlaps |> Seq.map (fun (p,hs) -> p,Seq.append (Seq.append [s1] hs) [s2])
        | s ->          protocolOverlaps |> Seq.map (fun (p,hs) -> p,Seq.append hs s)

    /// Name unnamed protocols with the given sheetName. If there is more than one unnamed protocol, additionally add an index
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
                    {p with Name = Some name},hs
                else p,hs
            )

    /// Returns the protocol described by the headers and a function for parsing the values of the matrix to the processes of this protocol
    let getProcessGetter protocolMetaData (nodes : seq<seq<string>>) =
    
        let characteristics,characteristicValueGetters =
            nodes |> Seq.choose AnnotationNode.tryGetCharacteristicGetter
            |> Seq.fold (fun (cl,cvl) (c,cv) -> c.Value :: cl, cv :: cvl) ([],[])
            |> fun (l1,l2) -> List.rev l1, List.rev l2
        let factors,factorValueGetters =
            nodes |> Seq.choose AnnotationNode.tryGetFactorGetter
            |> Seq.fold (fun (fl,fvl) (f,fv) -> f.Value :: fl, fv :: fvl) ([],[])
            |> fun (l1,l2) -> List.rev l1, List.rev l2
        let parameters,parameterValueGetters =
            nodes |> Seq.choose AnnotationNode.tryGetParameterGetter
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
                (fun matrix i -> inputGetter matrix i |> Source |> Some),outputGetter
            | None ->
                let inputNameGetter = nodes |> Seq.head |> AnnotationNode.tryGetSampleNameGetter
                let outputNameGetter = nodes |> Seq.last |> AnnotationNode.tryGetSampleNameGetter
                let inputGetter = 

                    fun matrix i ->      
                        inputNameGetter
                        |> Option.map (fun ing ->
                            Sample.create
                                None
                                (ing matrix i)
                                (characteristicValueGetters |> List.map (fun f -> f matrix i) |> API.Option.fromValueWithDefault [])
                                None
                                None
                            |> ProcessInput.Sample
                    )   

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
                (inputGetter matrix i |> Option.map List.singleton)
                (outputGetter matrix i |> List.singleton |> Some)
                None

    /// Merges processes with the same parameter values, grouping the input and output files
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

    /// Name processes by the protocol they execute. If more than one process executes the same protocol, additionally add an index
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

    /// Create a sample from a source
    let sampleOfSource (s:Source) =
        Sample.create s.ID s.Name s.Characteristics None None

    /// Updates the sample information in the given processes with the information of the samples in the given referenceProcesses.
    ///
    /// If the processes contain a source with the same name as a sample in the referenceProcesses. Additionally transforms it to a sample
    let updateSamplesByReference (referenceProcesses : Process seq) (processes : Process seq) = 
        let samples = 
            referenceProcesses
            |> Seq.collect (fun p -> 
                printfn "%O" p.Name 
                let inputs =
                    p.Inputs 
                    |> Option.defaultValue [] 
                    |> Seq.choose (function | ProcessInput.Sample x -> Some(x.Name, x) | _ -> None)
                let outputs =
                    p.Outputs 
                    |> Option.defaultValue [] 
                    |> Seq.choose (function | ProcessOutput.Sample x -> Some(x.Name, x) | _ -> None)
                Seq.append inputs outputs
                )
            |> Seq.filter (fun (name,s) -> name <> None && name <> (Some ""))
            |> Seq.groupBy fst
            |> Seq.map (fun (name,s) -> name, s |> Seq.map snd |> Seq.reduce API.Update.UpdateByExisting.updateRecordType)
            |> Map.ofSeq
    
        let updateInput (i:ProcessInput) =
            match i with 
            | ProcessInput.Source x ->      match Map.tryFind x.Name samples with   | Some sample -> ProcessInput.Sample (API.Update.UpdateByExisting.updateRecordType (sampleOfSource x) sample) | None -> i
            | ProcessInput.Data x ->        match Map.tryFind x.Name samples with   | Some sample -> ProcessInput.Sample sample | None -> i
            | ProcessInput.Sample x ->      match Map.tryFind x.Name samples with   | Some sample -> ProcessInput.Sample (API.Update.UpdateByExisting.updateRecordType x sample) | None -> i
            | ProcessInput.Material x ->    match Map.tryFind x.Name samples with   | Some sample -> ProcessInput.Sample sample | None -> i
        let updateOutput (o:ProcessOutput) =                         
            match o with                                             
            | ProcessOutput.Data x ->       match Map.tryFind x.Name samples with   | Some sample -> ProcessOutput.Sample sample | None -> o
            | ProcessOutput.Sample x ->     match Map.tryFind x.Name samples with   | Some sample -> ProcessOutput.Sample (API.Update.UpdateByExisting.updateRecordType x sample) | None -> o
            | ProcessOutput.Material x ->   match Map.tryFind x.Name samples with   | Some sample -> ProcessOutput.Sample sample | None -> o
        processes
        |> Seq.map (fun p -> 
           {p with
                    Inputs = p.Inputs |> Option.map (List.map updateInput)
                    Outputs = p.Outputs |> Option.map (List.map updateOutput)
           }
        )
    