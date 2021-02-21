namespace ISADotNet.XLSX.AssayFile

open ISADotNet

module Seq =

    /// Iterates over elements of the input sequence and groups adjacent elements.
    /// A new group is started when the specified predicate holds about the element
    /// of the sequence (and at the beginning of the iteration).
    ///
    /// For example: 
    ///    Seq.groupWhen isOdd [3;3;2;4;1;2] = seq [[3]; [3; 2; 4]; [1; 2]]
    let groupWhen f (input:seq<'a>) =
        use en = input.GetEnumerator()

        let rec loop cont =
            if en.MoveNext() then
                if (f en.Current) then
                    let temp = en.Current
                    loop (fun y -> 
                        cont 
                            (   match y with
                                | h::t -> []::(temp::h)::t
                                //| h::t -> [temp]::(h)::t
                                | [] -> [[temp]]
                            )
                         )
                else
                    let temp = en.Current                    
                    loop (fun y -> 
                        cont 
                            (   match y with
                                | h::t -> (temp::h)::t
                                | []   -> [[temp]]
                            )
                         )
            else
                cont []
        // Remove when first element is empty due to "[]::(temp::h)::t"
        let tmp:seq<seq<'a>> = 
            match (loop id) with
            | h::t -> match h with
                      | [] -> t
                      | _  -> h::t
            | [] -> []
            |> Seq.cast

        tmp

module AnnotationTable = 
    
    let splitIntoColumns (headers : seq<string>) =
        headers
        |> Seq.groupWhen (fun header -> 
            match (AnnotationColumn.SwateHeader.fromStringHeader header).Kind with
            | "Unit"                    -> false
            | "Term Source REF"         -> false
            | "Term Accession Number"   -> false
            | _ -> true
        )

    let getProcessGetter protocolMetaData (columnGroup : seq<seq<string>>) =
    
        let characteristics,characteristicValueGetters =
            columnGroup |> Seq.choose AnnotationColumn.tryGetCharacteristicGetterFunction
            |> Seq.fold (fun (cl,cvl) (c,cv) -> c.Value :: cl, cv :: cvl) ([],[])
            |> fun (l1,l2) -> List.rev l1, List.rev l2
        let factors,factorValueGetters =
            columnGroup |> Seq.choose AnnotationColumn.tryGetFactorGetterFunction
            |> Seq.fold (fun (fl,fvl) (f,fv) -> f.Value :: fl, fv :: fvl) ([],[])
            |> fun (l1,l2) -> List.rev l1, List.rev l2
        let parameters,parameterValueGetters =
            columnGroup |> Seq.choose AnnotationColumn.tryGetParameterGetterFunction
            |> Seq.fold (fun (pl,pvl) (p,pv) -> p.Value :: pl, pv :: pvl) ([],[])
            |> fun (l1,l2) -> List.rev l1, List.rev l2
    
        let inputGetter,outputGetter =
            match columnGroup |> Seq.tryPick AnnotationColumn.tryGetSourceNameGetter with
            | Some inputNameGetter ->
                let outputNameGetter = columnGroup |> Seq.tryPick AnnotationColumn.tryGetSampleNameGetter
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
                let inputNameGetter = columnGroup |> Seq.head |> AnnotationColumn.tryGetSampleNameGetter
                let outputNameGetter = columnGroup |> Seq.last |> AnnotationColumn.tryGetSampleNameGetter
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
        |> Seq.map (fun (protocol,processGroup) ->
            processGroup
            |> Seq.mapi (fun i p -> 
                {p with Name = 
                        protocol.Value.Name |> Option.map (fun s -> sprintf "%s_%i" s i)
                }
            )
        )
        |> Seq.concat
    