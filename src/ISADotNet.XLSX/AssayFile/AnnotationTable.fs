namespace ISADotNet.XLSX.AssayFile

open ISADotNet.XLSX
open ISADotNet

/// Functions for parsing an annotation table to the described processes
module AnnotationTable = 

    /// Returns the protocol described by the headers and a function for parsing the values of the matrix to the processes of this protocol
    let getProcessGetter protocolMetaData (nodes : seq<seq<string>>) =
    
        let valueNodes =
            nodes
            |> Seq.filter (AnnotationNode.isValueNode)
            |> Seq.indexed

        let characteristics,characteristicValueGetters =
            valueNodes |> Seq.choose (fun (i,n) -> AnnotationNode.tryGetCharacteristicGetter i n)
            |> Seq.fold (fun (cl,cvl) (c,cv) -> c.Value :: cl, cv :: cvl) ([],[])
            |> fun (l1,l2) -> List.rev l1, List.rev l2
        let factors,factorValueGetters =
            valueNodes |> Seq.choose (fun (i,n) -> AnnotationNode.tryGetFactorGetter i n)
            |> Seq.fold (fun (fl,fvl) (f,fv) -> f.Value :: fl, fv :: fvl) ([],[])
            |> fun (l1,l2) -> List.rev l1, List.rev l2
        let parameters,parameterValueGetters =
            valueNodes |> Seq.choose (fun (i,n) -> AnnotationNode.tryGetParameterGetter i n)
            |> Seq.fold (fun (pl,pvl) (p,pv) -> p.Value :: pl, pv :: pvl) ([],[])
            |> fun (l1,l2) -> List.rev l1, List.rev l2
    
        let dataFileGetter = nodes |> Seq.tryPick AnnotationNode.tryGetDataFileGetter

        let inputGetter,outputGetter =
            match nodes |> Seq.tryPick AnnotationNode.tryGetSourceNameGetter with
            | Some inputNameGetter ->
                let outputNameGetter = nodes |> Seq.tryPick AnnotationNode.tryGetSampleNameGetter
                let inputGetter = 
                    fun matrix i -> 
                        let source = 
                            Source.make
                                None
                                (inputNameGetter matrix i)
                                (characteristicValueGetters |> List.map (fun f -> f matrix i) |> Option.fromValueWithDefault [])
                        if dataFileGetter.IsSome then 
                            [source;source]
                        else 
                            [source]
                
                let outputGetter =
                    fun matrix i -> 
                        let data = dataFileGetter |> Option.map (fun f -> f matrix i)
                        let outputName = 
                            match outputNameGetter |> Option.bind (fun o -> o matrix i) with
                            | Some s -> Some s
                            | None -> 
                                match data with
                                | Some data -> data.Name
                                | None -> None
                        let sample =
                            Sample.make
                                None
                                outputName
                                None
                                (factorValueGetters |> List.map (fun f -> f matrix i) |> Option.fromValueWithDefault [])
                                (inputGetter matrix i |> List.distinct |> Some)
                        if data.IsSome then 
                            [ProcessOutput.Sample sample; ProcessOutput.Data data.Value]
                        else 
                            [ProcessOutput.Sample sample]                      
                (fun matrix i -> inputGetter matrix i |> List.map Source |> Some),outputGetter
            | None ->
                let inputNameGetter = nodes |> Seq.head |> AnnotationNode.tryGetSampleNameGetter
                let outputNameGetter = nodes |> Seq.last |> AnnotationNode.tryGetSampleNameGetter
                let inputGetter = 

                    fun matrix i ->      
                        let source = 
                            inputNameGetter
                            |> Option.map (fun ing ->
                                Sample.make
                                    None
                                    (ing matrix i)
                                    (characteristicValueGetters |> List.map (fun f -> f matrix i) |> Option.fromValueWithDefault [])
                                    None
                                    None
                                |> ProcessInput.Sample
                            )   
                        match source with
                        | Some source when dataFileGetter.IsSome -> Some [source;source]
                        | Some source -> Some  [source]
                        | None -> None
                            

                let outputGetter =
                    fun matrix i -> 
                        let data = dataFileGetter |> Option.map (fun f -> f matrix i)
                        let outputName = 
                            match outputNameGetter |> Option.bind (fun o -> o matrix i) with
                            | Some s -> Some s
                            | None -> 
                                match data with
                                | Some data -> data.Name
                                | None -> None
                        let sample =
                            Sample.make
                                None
                                outputName
                                None
                                (factorValueGetters |> List.map (fun f -> f matrix i) |> Option.fromValueWithDefault [])
                                None
                        if data.IsSome then 
                            [ProcessOutput.Sample sample; ProcessOutput.Data data.Value]
                        else 
                            [ProcessOutput.Sample sample]  
                inputGetter,outputGetter
    
        let protocol = {protocolMetaData with Parameters = Option.fromValueWithDefault [] parameters}
    
        characteristics,
        factors,
        protocol,
        fun (matrix : System.Collections.Generic.Dictionary<(int * string),string>) i ->
            Process.make 
                None 
                None 
                (Some protocol) 
                (parameterValueGetters |> List.map (fun f -> f matrix i) |> Option.fromValueWithDefault [])
                None
                None
                None
                None          
                (inputGetter matrix i)
                (outputGetter matrix i |> Some)
                None

    /// Merges processes with the same parameter values, grouping the input and output files
    let mergeIdenticalProcesses (processes : seq<Process>) =
        processes
        |> Seq.groupBy (fun p -> p.ExecutesProtocol,p.ParameterValues)
        |> Seq.map (fun (_,processGroup) ->
            processGroup
            |> Seq.reduce (fun p1 p2 ->
                let mergedInputs = List.append (p1.Inputs |> Option.defaultValue []) (p2.Inputs |> Option.defaultValue []) |> Option.fromValueWithDefault []
                let mergedOutputs = List.append (p1.Outputs |> Option.defaultValue []) (p2.Outputs |> Option.defaultValue []) |> Option.fromValueWithDefault []
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
        Sample.make s.ID s.Name s.Characteristics None None

    /// Create a sample from a source
    let sourceOfSample (s:Sample) =
        Source.make s.ID s.Name s.Characteristics

    /// Updates the sample information in the given processes with the information of the samples in the given referenceProcesses.
    ///
    /// If the processes contain a source with the same name as a sample in the referenceProcesses. Additionally transforms it to a sample
    let private updateSamplesBy (referenceProcesses : Process seq) (processes : Process seq) = 
        let samples = 
            referenceProcesses
            |> Seq.collect (fun p -> 
                let inputs =
                    p.Inputs 
                    |> Option.defaultValue [] 
                    |> Seq.choose (function | ProcessInput.Sample x -> Some(x.Name,true, x) | ProcessInput.Source x -> Some (x.Name,false,sampleOfSource x)| _ -> None)
                let outputs =
                    p.Outputs 
                    |> Option.defaultValue [] 
                    |> Seq.choose (function | ProcessOutput.Sample x -> Some(x.Name,true, x) | _ -> None)
                Seq.append inputs outputs
                |> Seq.distinct
                )
            |> Seq.filter (fun (name,_,samples) -> name <> None && name <> (Some ""))
            |> Seq.groupBy (fun (name,_,samples) -> name)
            |> Seq.map (fun (name,samples) -> 
                let aggregatedSample = 
                    samples 
                    |> Seq.map (fun (name,_,s) -> s) 
                    |> Seq.reduce (fun s1 s2 -> if s1 = s2 then s1 else API.Update.UpdateByExistingAppendLists.updateRecordType s1 s2)
                if Seq.exists (fun (name,isSample,s) -> isSample) samples then
                    name, ProcessInput.Sample aggregatedSample
                else name, ProcessInput.Source (sourceOfSample aggregatedSample)          
            )
            |> Map.ofSeq
    
        let updateInput (i:ProcessInput) =
            match i with 
            | ProcessInput.Source x ->      match Map.tryFind x.Name samples with   | Some s -> s | None -> ProcessInput.Source x
            | ProcessInput.Sample x ->      match Map.tryFind x.Name samples with   | Some s -> s | None -> ProcessInput.Sample x
            | ProcessInput.Data x ->        ProcessInput.Data x
            | ProcessInput.Material x ->    ProcessInput.Material x
        let updateOutput (o:ProcessOutput) =                         
            match o with                                             
            | ProcessOutput.Sample x ->     match Map.tryFind x.Name samples with   | Some (ProcessInput.Sample x) -> ProcessOutput.Sample x | _ -> ProcessOutput.Sample x
            | ProcessOutput.Data x ->       ProcessOutput.Data x
            | ProcessOutput.Material x ->   ProcessOutput.Material x
        processes
        |> Seq.map (fun p -> 
           {p with
                Inputs = p.Inputs |> Option.map (List.map updateInput)
                Outputs = p.Outputs |> Option.map (List.map updateOutput)
           }
        )

    /// Updates the sample information in the given processes with the information of the samples in the given referenceProcesses.
    ///
    /// If the processes contain a source with the same name as a sample in the referenceProcesses. Additionally transforms it to a sample
    let updateSamplesByReference (referenceProcesses : Process seq) (processes : Process seq) = 
        referenceProcesses
        |> Seq.append processes
        |> updateSamplesBy processes

    /// Updates the sample information in the given processes with the information of the samples in the given referenceProcesses.
    ///
    /// If the processes contain a source with the same name as a sample in the referenceProcesses. Additionally transforms it to a sample
    let updateSamplesByThemselves (processes : Process seq) =
        processes
        |> updateSamplesBy processes


module QRow =
    open FsSpreadsheet.DSL


    let renumberHeaders (headerss : (string list) list) = 
        
        let counts = System.Collections.Generic.Dictionary<string, int ref>()
        let renumberHeader num (h : string) = 
            counts.[h] <- ref num
            if h = "Unit" then
                $"Unit#{num}"
            elif h.EndsWith ")" then
                h.Replace(")",$"#{num})")
            elif h.EndsWith "]" then
                h.Replace("]",$"#{num}]")
            else h
        headerss
        |> List.map (fun headers ->
            let pickResult = 
                headers
                |> List.choose (fun h ->
                    match Dictionary.tryGetValue h counts with
                    | Some count -> 
                        count := !count + 1 
                        Some !count
                    | _ -> 
                        counts.[h] <- ref 1
                        None
                )
                |> function 
                    | [] -> None
                    | l -> List.max l |> Some
            match pickResult with
            | Some (count) -> 
                headers |> List.map (renumberHeader count)
            | _ -> 
                headers
        )

    let toHeaderRow (r : QueryModel.QRow) =
        try 
            row {
                "Source Name"
                for v in (r.Values |> List.map ISAValue.toHeaders |> renumberHeaders |> List.concat) do v
                IOType.toHeader r.OutputType.Value
            }
        with
        | err -> failwithf "Could not parse headers of row: \n%s" err.Message

    let toValueRow i (r : QueryModel.QRow) =
        try
            row {
                r.Input
                for v in (r.Values |> List.collect ISAValue.toValues) do v
                r.Output
            }
        with
        | err -> failwithf "Could not parse values of row %i: \n%s" (i+1) err.Message

module QSheet =

    open FsSpreadsheet.DSL

    let toSheet i (s : QueryModel.QSheet) =
        try 

            sheet s.SheetName {
                table $"annotationTable{i}" {
                    QRow.toHeaderRow s.[0]
                    for (i,r) in Seq.indexed s do QRow.toValueRow i r
                }
            }
        with
        | err -> failwithf "Could not parse sheet %s: \n%s" s.SheetName err.Message