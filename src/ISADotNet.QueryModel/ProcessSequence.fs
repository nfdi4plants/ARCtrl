namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

open System.Collections.Generic
open System.Collections

/// Type representing a queryable collection of processes, which model the experimental graph
type QProcessSequence (sheets : QSheet list) =

    /// List of protocols or sheets (in ISATab) 
    member this.Sheets = sheets

    /// Update the entities in the sheets against the reference. The same entity (distinct by name) can be a source in some sheet and a sample in another. 
    /// In this case, the function will transform the source to a sample so it's the same everywhere
    static member internal updateNodesAgainst (reference : QSheet list) (sheets : QSheet list) =
        let mapping = 
            reference
            |> List.collect (fun s -> 
                s.Rows
                |> List.collect (fun r -> [r.Input, r.InputType.Value; r.Output, r.OutputType.Value])
            )
            |> List.groupBy fst
            |> List.map (fun (name,vs) -> name, vs |> List.map snd |> IOType.reduce)
            |> Map.ofList
        let updateRow row = 
            {row with 
                InputType = Some mapping.[row.Input]
                OutputType = Some mapping.[row.Output]
            }
        sheets
        |> List.map (fun sheet ->
            {sheet with Rows = sheet.Rows |> List.map updateRow}
        )

    /// Create a QProcessSequence object from a list of processes. The resulting sheets will be created by grouping all processes implementing the same protocol
    new (processSequence : Process list, ?ReferenceSheets : QSheet list) =
        let updateNodes (sheets : QSheet list) =
            match ReferenceSheets with
            | Some ref ->
                QProcessSequence.updateNodesAgainst (ref @ sheets) sheets
            | None ->
                QProcessSequence.updateNodesAgainst sheets sheets
        let sheets = 
            processSequence
            |> List.groupBy (fun x -> 
                if x.ExecutesProtocol.IsSome && x.ExecutesProtocol.Value.Name.IsSome then
                    x.ExecutesProtocol.Value.Name.Value 
                else
                    // Data Stewards use '_' as seperator to distinguish between protocol template types.
                    // Exmp. 1SPL01_plants, in these cases we need to find the last '_' char and remove from that index.
                    let lastUnderScoreIndex = x.Name.Value.LastIndexOf '_'
                    x.Name.Value.Remove lastUnderScoreIndex
            )
            |> List.map (fun (name,processes) -> QSheet.fromProcesses name processes)
            |> updateNodes
        QProcessSequence(sheets)

    /// Create a QProcessSequence object from an assay. The resulting sheets will be created by grouping all processes implementing the same protocol
    static member fromAssay (assay : Assay) =
        
        QProcessSequence(assay.ProcessSequence |> Option.defaultValue [])
      
    /// get the nth protocol or sheet (in ISATab logic) 
    member this.Protocol (i : int, ?EntityName) =
        let sheet = 
            this.Sheets 
            |> List.tryItem i
        match sheet with
        | Some s -> s
        | None -> failwith $"""{EntityName |> Option.defaultValue "ProcessSequence"} does not contain sheet with index {i} """

    /// get the protocol or sheet (in ISATab logic) with the given name
    member this.Protocol (sheetName, ?EntityName) =
        let sheet = 
            this.Sheets 
            |> List.tryFind (fun sheet -> sheet.SheetName = sheetName)
        match sheet with
        | Some s -> s
        | None -> failwith $"""{EntityName |> Option.defaultValue "ProcessSequence"} does not contain sheet with name "{sheetName}" """

    /// List of protocols or sheets (in ISATab) 
    member this.Protocols = this.Sheets

    /// Number of protocols or sheets (in ISATab) in the processSequence
    member this.ProtocolCount =
        this.Sheets 
        |> List.length

    /// Names of the protocols or sheets (in ISATab) in the processSequence
    member this.ProtocolNames =
        this.Sheets 
        |> List.map (fun sheet -> sheet.SheetName)
       
    interface IEnumerable<QSheet> with
        member this.GetEnumerator() = (Seq.ofList this.Sheets).GetEnumerator()

    interface IEnumerable with
        member this.GetEnumerator() = (this :> IEnumerable<QSheet>).GetEnumerator() :> IEnumerator

    /// Returns the list of all nodes (sources, samples, data) in the ProcessSequence
    static member getNodes (ps : #QProcessSequence) =
        ps.Protocols 
        |> List.collect (fun p -> p.Rows |> List.collect (fun r -> [r.Input;r.Output]))
        |> List.distinct     

    /// Returns a new process sequence, only with those rows that contain either an educt or a product entity of the given node (or entity)
    static member getSubTreeOf (node : string) (ps : #QProcessSequence) =
        let rec collectForwardNodes nodes =
            let newNodes = 
                ps.Sheets
                |> List.collect (fun sheet ->
                    sheet.Rows 
                    |> List.choose (fun r -> if List.contains r.Input nodes then Some r.Output else None)
                )
                |> List.append nodes 
                |> List.distinct
                
            if newNodes = nodes then nodes
            else collectForwardNodes newNodes

        let collectBackwardNodes nodes =
            let newNodes = 
                ps.Sheets
                |> List.collect (fun sheet ->
                    sheet.Rows 
                    |> List.choose (fun r -> if List.contains r.Output nodes then Some r.Input else None)
                )
                |> List.append nodes 
                |> List.distinct
                       
            if newNodes = nodes then nodes
            else collectForwardNodes newNodes

        let forwardNodes = collectForwardNodes [node]
        let backwardNodes = collectBackwardNodes [node]

        ps.Sheets
        |> List.map (fun sheet ->
            {sheet 
                with Rows = 
                        sheet.Rows
                        |> List.filter (fun r ->
                            List.contains r.Input forwardNodes 
                            || (List.contains r.Output backwardNodes)

                        )

            }
        )
        |> QProcessSequence

    /// Returns the names of all initial inputs final outputs of the processSequence, to which no processPoints
    static member getRootInputs (ps : #QProcessSequence) =
        let inputs = ps.Protocols |> List.collect (fun p -> p.Rows |> List.map (fun r -> r.Input))
        let outputs =  ps.Protocols |> List.collect (fun p -> p.Rows |> List.map (fun r -> r.Output)) |> Set.ofList
        inputs
        |> List.filter (fun i -> outputs.Contains i |> not)

    /// Returns the names of all final outputs of the processSequence, which point to no further nodes
    static member getFinalOutputs (ps : #QProcessSequence) =
        let inputs = ps.Protocols |> List.collect (fun p -> p.Rows |> List.map (fun r -> r.Input)) |> Set.ofList
        let outputs =  ps.Protocols |> List.collect (fun p -> p.Rows |> List.map (fun r -> r.Output))
        outputs
        |> List.filter (fun i -> inputs.Contains i |> not)

    /// Returns the names of all nodes for which the predicate reutrns true
    static member getNodesBy (predicate : QueryModel.IOType -> bool) (ps : #QProcessSequence) =
        ps.Protocols 
        |> List.collect (fun p -> 
            p.Rows 
            |> List.collect (fun r -> 
                [
                    if predicate r.InputType.Value then r.Input; 
                    if predicate r.OutputType.Value then  r.Output
                ])
        )
        |> List.distinct 

    /// Returns the names of all initial inputs final outputs of the processSequence, to which no processPoints, and for which the predicate returns true
    static member getRootInputsBy (predicate : QueryModel.IOType -> bool) (ps : #QProcessSequence) =
        let mappings = 
            ps.Protocols 
            |> List.collect (fun p -> 
                p.Rows 
                |> List.map (fun r -> r.Input, r.Output)
                |> List.distinct
            ) 
            |> List.groupBy fst 
            |> List.map (fun (out,ins) -> out, ins |> List.map snd)
            |> Map.ofList

        let typeMappings =
            ps.Protocols 
            |> List.collect (fun p -> 
                p.Rows 
                |> List.collect (fun r -> [r.Input, r.InputType; r.Output, r.OutputType])
            ) 
            |> Map.ofList       

        let predicate (entity : string) =
            match typeMappings.[entity] with
            | Some t -> predicate t
            | None -> false

        let rec loop (searchEntities : string list) (foundEntities : string list) = 
            if searchEntities.IsEmpty then foundEntities |> List.distinct
            else
                let targs = searchEntities |> List.filter predicate
                let nonTargs = searchEntities |> List.filter (predicate >> not)
                let nextSearchEntities = nonTargs |> List.collect (fun en -> Map.tryFind en mappings |> Option.defaultValue [])
                loop nextSearchEntities targs

        loop (QProcessSequence.getRootInputs ps) []

    /// Returns the names of all final outputs of the processSequence, which point to no further nodes, and for which the predicate returns true
    static member getFinalOutputsBy (predicate : QueryModel.IOType -> bool) (ps : #QProcessSequence) =
        let mappings = 
            ps.Protocols 
            |> List.collect (fun p -> 
                p.Rows 
                |> List.map (fun r -> r.Output, r.Input )
                |> List.distinct
            ) 
            |> List.groupBy fst 
            |> List.map (fun (out,ins) -> out, ins |> List.map snd)
            |> Map.ofList

        let typeMappings =
            ps.Protocols 
            |> List.collect (fun p -> 
                p.Rows 
                |> List.collect (fun r -> [r.Input, r.InputType; r.Output, r.OutputType])
            ) 
            |> Map.ofList       

        let predicate (entity : string) =
            match typeMappings.[entity] with
            | Some t -> predicate t
            | None -> false

        let rec loop (searchEntities : string list) (foundEntities : string list) = 
            if searchEntities.IsEmpty then foundEntities |> List.distinct
            else
                let targs = searchEntities |> List.filter predicate
                let nonTargs = searchEntities |> List.filter (predicate >> not)
                let nextSearchEntities = nonTargs |> List.collect (fun en -> Map.tryFind en mappings |> Option.defaultValue [])
                loop nextSearchEntities targs

        loop (QProcessSequence.getFinalOutputs ps) []

    /// Returns the names of all nodes processSequence, which are connected to the given node and for which the predicate returns true
    static member getNodesOfBy (predicate : QueryModel.IOType -> bool) (node : string) (ps : #QProcessSequence) =
        QProcessSequence.getSubTreeOf node ps
        |> QProcessSequence.getNodesBy predicate

    /// Returns the initial inputs final outputs of the assay, to which no processPoints, which are connected to the given node and for which the predicate returns true
    static member getRootInputsOfBy (predicate : QueryModel.IOType -> bool) (node : string) (ps : #QProcessSequence) =
        QProcessSequence.getSubTreeOf node ps
        |> QProcessSequence.getRootInputsBy predicate

    /// Returns the final outputs of the assay, which point to no further nodes, which are connected to the given node and for which the predicate returns true
    static member getFinalOutputsOfBy (predicate : QueryModel.IOType -> bool) (node : string) (ps : #QProcessSequence) =
        QProcessSequence.getSubTreeOf node ps
        |> QProcessSequence.getFinalOutputsBy predicate
       
    /// Returns the previous values of the given node
    static member getPreviousValuesOf (ps : #QProcessSequence) (node : string) =
        let mappings = 
            ps.Protocols 
            |> List.collect (fun p -> 
                p.Rows 
                |> List.map (fun r -> r.Output,r)
                |> List.distinct
            ) 

            |> Map.ofList
        let rec loop values lastState state = 
            if lastState = state then values 
            else
                let newState,newValues = 
                    state 
                    |> List.map (fun s -> 
                        mappings.TryFind s 
                        |> Option.map (fun r -> r.Input,r.Vals)
                        |> Option.defaultValue (s,[])
                    )
                    |> List.unzip
                    |> fun (s,vs) -> s, vs |> List.concat
                loop (newValues@values) state newState
        loop [] [] [node]  
        |> ValueCollection

    /// Returns the succeeding values of the given node
    static member getSucceedingValuesOf (ps : #QProcessSequence) (sample : string) =
        let mappings = 
            ps.Protocols 
            |> List.collect (fun p -> 
                p.Rows 
                |> List.map (fun r -> r.Input,r)
                |> List.distinct
            ) 

            |> Map.ofList
        let rec loop values lastState state = 
            if lastState = state then values 
            else
                let newState,newValues = 
                    state 
                    |> List.map (fun s -> 
                        mappings.TryFind s 
                        |> Option.map (fun r -> r.Output,r.Vals)
                        |> Option.defaultValue (s,[])
                    )
                    |> List.unzip
                    |> fun (s,vs) -> s, vs |> List.concat
                loop (values@newValues) state newState
        loop [] [] [sample]
        |> ValueCollection

    /// Returns a new ProcessSequence, with only the values from the processes that implement the given protocol
    static member onlyValuesOfProtocol (ps : #QProcessSequence) (protocolName : string option) =
        match protocolName with
        | Some pn ->
            ps.Sheets
            |> List.map (fun s -> 
                if s.SheetName = pn then 
                    s
                else 
                    {s with Rows = s.Rows |> List.map (fun r -> {r with Vals = []})}
            )
            |> QProcessSequence                    
        | None -> ps.Sheets |> QProcessSequence   

    /// Returns an IOValueCollection, where for each Value the closest inputs and outputs are used
    member this.Nearest = 
        this.Sheets
        |> List.collect (fun sheet -> sheet.Values |> Seq.toList)
        |> IOValueCollection
   
    /// Returns an IOValueCollection, where for each Value the global inputs and closest outputs are used
    member this.SinkNearest = 
        this.Sheets
        |> List.collect (fun sheet -> 
            sheet.Rows
            |> List.collect (fun r ->               
                
                QProcessSequence.getRootInputsOfBy (fun _ -> true) r.Input this
                |> List.distinct
                |> List.collect (fun inp -> 
                    r.Vals
                    |> List.map (fun v -> 
                        KeyValuePair((inp,r.Output),v)
                    )
                )
            )
        )
        |> IOValueCollection

    /// Returns an IOValueCollection, where for each Value the closest inputs and global outputs are used
    member this.SourceNearest = 
        this.Sheets
        |> List.collect (fun sheet -> 
            sheet.Rows
            |> List.collect (fun r ->               
                
                QProcessSequence.getFinalOutputsOfBy (fun _ -> true) r.Output this 
                |> List.distinct
                |> List.collect (fun out -> 
                    r.Vals
                    |> List.map (fun v -> 
                        KeyValuePair((r.Input,out),v)
                    )
                )
            )
        )
        |> IOValueCollection

    /// Returns an IOValueCollection, where for each Value the global inputs and outputs are used
    member this.Global =
        this.Sheets
        |> List.collect (fun sheet -> 
            sheet.Rows
            |> List.collect (fun r ->  
                let outs = QProcessSequence.getFinalOutputsOfBy (fun _ -> true) r.Output this |> List.distinct
                let inps = QProcessSequence.getRootInputsOfBy (fun _ -> true) r.Input this |> List.distinct
                outs
                |> List.collect (fun out -> 
                    inps
                    |> List.collect (fun inp ->
                        r.Vals
                        |> List.map (fun v -> 
                            KeyValuePair((inp,out),v)
                        )
                    )
                )
            )
        )
        |> IOValueCollection

    /// Returns the names of all nodes in the Process sequence
    member this.Nodes() =
        QProcessSequence.getNodes(this)

    /// Returns the names of all nodes in the Process sequence
    member this.NodesOf(node) =
        QProcessSequence.getNodesOfBy (fun _ -> true) node this

    /// Returns the names of all the input nodes in the Process sequence to which no output points
    member this.FirstNodes() = 
        QProcessSequence.getRootInputs(this)

    /// Returns the names of all the output nodes in the Process sequence that point to no input
    member this.LastNodes() = 
        QProcessSequence.getFinalOutputs(this)

    /// Returns the names of all the input nodes in the Process sequence to which no output points, that are connected to the given node
    member this.FirstNodesOf(node) = 
        QProcessSequence.getRootInputsOfBy (fun _ -> true) node this

    /// Returns the names of all the output nodes in the Process sequence that point to no input, that are connected to the given node
    member this.LastNodesOf(node) = 
        QProcessSequence.getFinalOutputsOfBy (fun _ -> true) node this

    /// Returns the names of all samples in the Process sequence
    member this.Samples() =
        QProcessSequence.getNodesBy (fun (io : IOType) -> io.isSample) this

    /// Returns the names of all samples in the Process sequence, that are connected to the given node
    member this.SamplesOf(node) =
        QProcessSequence.getNodesOfBy (fun (io : IOType) -> io.isSample) node this

    /// Returns the names of all the input samples in the Process sequence to which no output points
    member this.FirstSamples() = 
        QProcessSequence.getRootInputsBy (fun (io : IOType) -> io.isSample) this

    /// Returns the names of all the output samples in the Process sequence that point to no input
    member this.LastSamples() = 
        QProcessSequence.getFinalOutputsBy (fun (io : IOType) -> io.isSample) this

    /// Returns the names of all the input samples in the Process sequence to which no output points, that are connected to the given node
    member this.FirstSamplesOf(node) = 
        QProcessSequence.getRootInputsOfBy (fun (io : IOType) -> io.isSample) node this

    /// Returns the names of all the output samples in the Process sequence that point to no input, that are connected to the given node
    member this.LastSamplesOf(node) = 
        QProcessSequence.getFinalOutputsOfBy (fun (io : IOType) -> io.isSample) node this

    /// Returns the names of all sources in the Process sequence
    member this.Sources() =
        QProcessSequence.getNodesBy (fun (io : IOType) -> io.isSource) this

    /// Returns the names of all sources in the Process sequence, that are connected to the given node
    member this.SourcesOf(node) =
        QProcessSequence.getNodesOfBy (fun (io : IOType) -> io.isSource) node this

    /// Returns the names of all data in the Process sequence
    member this.Data() =
        QProcessSequence.getNodesBy (fun (io : IOType) -> io.isData) this

    /// Returns the names of all data in the Process sequence, that are connected to the given node
    member this.DataOf(node) =
        QProcessSequence.getNodesOfBy (fun (io : IOType) -> io.isData) node this

    /// Returns the names of all the input data in the Process sequence to which no output points
    member this.FirstData() = 
        QProcessSequence.getRootInputsBy (fun (io : IOType) -> io.isData) this

    /// Returns the names of all the output data in the Process sequence that point to no input
    member this.LastData() = 
        QProcessSequence.getFinalOutputsBy (fun (io : IOType) -> io.isData) this

    /// Returns the names of all the input data in the Process sequence to which no output points, that are connected to the given node
    member this.FirstDataOf(node) = 
        QProcessSequence.getRootInputsOfBy (fun (io : IOType) -> io.isData) node this

    /// Returns the names of all the output data in the Process sequence that point to no input, that are connected to the given node
    member this.LastDataOf(node) = 
        QProcessSequence.getFinalOutputsOfBy (fun (io : IOType) -> io.isData) node this

    /// Returns the names of all raw data in the Process sequence
    member this.RawData() =
        QProcessSequence.getNodesBy (fun (io : IOType) -> io.isRawData) this

    /// Returns the names of all raw data in the Process sequence, that are connected to the given node
    member this.RawDataOf(node) =
        QProcessSequence.getNodesOfBy (fun (io : IOType) -> io.isRawData) node this

    /// Returns the names of all the input raw data in the Process sequence to which no output points
    member this.FirstRawData() = 
        QProcessSequence.getRootInputsBy (fun (io : IOType) -> io.isRawData) this

    /// Returns the names of all the output raw data in the Process sequence that point to no input
    member this.LastRawData() = 
        QProcessSequence.getFinalOutputsBy (fun (io : IOType) -> io.isRawData) this
    
    /// Returns the names of all the input raw data in the Process sequence to which no output points, that are connected to the given node
    member this.FirstRawDataOf(node) = 
        QProcessSequence.getRootInputsOfBy (fun (io : IOType) -> io.isRawData) node this

    /// Returns the names of all the output raw data in the Process sequence that point to no input, that are connected to the given node
    member this.LastRawDataOf(node) = 
        QProcessSequence.getFinalOutputsOfBy (fun (io : IOType) -> io.isRawData) node this

    /// Returns the names of all processed data in the Process sequence
    member this.ProcessedData() =
        QProcessSequence.getNodesBy (fun (io : IOType) -> io.isProcessedData) this

    /// Returns the names of all processed data in the Process sequence, that are connected to the given node
    member this.ProcessedDataOf(node) =
        QProcessSequence.getNodesOfBy (fun (io : IOType) -> io.isProcessedData) node this

    /// Returns the names of all the input processed data in the Process sequence to which no output points
    member this.FirstProcessedData() = 
        QProcessSequence.getRootInputsBy (fun (io : IOType) -> io.isProcessedData) this

    /// Returns the names of all the output processed data in the Process sequence that point to no input
    member this.LastProcessedData() = 
        QProcessSequence.getFinalOutputsBy (fun (io : IOType) -> io.isProcessedData) this

    /// Returns the names of all the input processed data in the Process sequence to which no output points, that are connected to the given node
    member this.FirstProcessedDataOf(node) = 
        QProcessSequence.getRootInputsOfBy (fun (io : IOType) -> io.isProcessedData) node this

    /// Returns the names of all the output processed data in the Process sequence that point to no input, that are connected to the given node
    member this.LastProcessedDataOf(node) = 
        QProcessSequence.getFinalOutputsOfBy (fun (io : IOType) -> io.isProcessedData) node this

    /// Returns all values in the process sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.Values(?ProtocolName) = 
        (QProcessSequence.onlyValuesOfProtocol this ProtocolName).Sheets
        |> List.collect (fun s -> s.Values.Values().Values)
        |> ValueCollection

    /// Returns all values in the process sequence whose header matches the given category
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.Values(ontology : OntologyAnnotation, ?ProtocolName) = 
        (QProcessSequence.onlyValuesOfProtocol this ProtocolName).Sheets
        |> List.collect (fun s -> s.Values.Values().Filter(ontology).Values)
        |> ValueCollection

    /// Returns all values in the process sequence whose header matches the given name
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.Values(name : string, ?ProtocolName) = 
        (QProcessSequence.onlyValuesOfProtocol this ProtocolName).Sheets
        |> List.collect (fun s -> s.Values.Values().Filter(name).Values)
        |> ValueCollection

    /// Returns all factor values in the process sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.Factors(?ProtocolName) =
        (QProcessSequence.onlyValuesOfProtocol this ProtocolName).Values().Factors()

    /// Returns all parameter values in the process sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.Parameters(?ProtocolName) =
        (QProcessSequence.onlyValuesOfProtocol this ProtocolName).Values().Parameters()

    /// Returns all characteristic values in the process sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.Characteristics(?ProtocolName) =
        (QProcessSequence.onlyValuesOfProtocol this ProtocolName).Values().Characteristics()

    /// Returns all values in the process sequence, that are connected to the given node
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.ValuesOf(node, ?ProtocolName) =
        let ps = QProcessSequence.onlyValuesOfProtocol this ProtocolName
        (QProcessSequence.getPreviousValuesOf ps node).Values @ (QProcessSequence.getSucceedingValuesOf ps node).Values
        |> ValueCollection

    /// Returns all values in the process sequence, that are connected to the given node and come before it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.PreviousValuesOf(node, ?ProtocolName) =
        let ps = QProcessSequence.onlyValuesOfProtocol this ProtocolName
        QProcessSequence.getPreviousValuesOf ps node

    /// Returns all values in the process sequence, that are connected to the given node and come after it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.SucceedingValuesOf(node, ?ProtocolName) =
        let ps = QProcessSequence.onlyValuesOfProtocol this ProtocolName
        QProcessSequence.getSucceedingValuesOf ps node

    /// Returns all characteristic values in the process sequence, that are connected to the given node
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.CharacteristicsOf(node, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).ValuesOf(node).Characteristics()

    /// Returns all characteristic values in the process sequence, that are connected to the given node and come before it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.PreviousCharacteristicsOf(node, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).PreviousValuesOf(node).Characteristics()

    /// Returns all characteristic values in the process sequence, that are connected to the given node and come after it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.SucceedingCharacteristicsOf(node, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).SucceedingValuesOf(node).Characteristics()

    /// Returns all parameter values in the process sequence, that are connected to the given node 
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.ParametersOf(node, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).ValuesOf(node).Parameters()

    /// Returns all parameter values in the process sequence, that are connected to the given node and come before it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.PreviousParametersOf(node, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).PreviousValuesOf(node).Parameters()

    /// Returns all parameter values in the process sequence, that are connected to the given node and come after it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.SucceedingParametersOf(node, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).SucceedingValuesOf(node).Parameters()

    /// Returns all factor values in the process sequence, that are connected to the given node 
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.FactorsOf(node, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).ValuesOf(node).Factors()

    /// Returns all factor values in the process sequence, that are connected to the given node and come before it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.PreviousFactorsOf(node, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).PreviousValuesOf(node).Factors()

    /// Returns all factor values in the process sequence, that are connected to the given node 
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol and come after it in the sequence
    member this.SucceedingFactorsOf(node, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).SucceedingValuesOf(node).Factors()

    member this.Contains(ontology : OntologyAnnotation, ?ProtocolName) = 
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).Values().Contains ontology

    member this.Contains(name : string, ?ProtocolName) = 
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).Values().Contains name

