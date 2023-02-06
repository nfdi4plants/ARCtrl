namespace rec ISADotNet.QueryModel

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
                if x.Name.IsSome && (x.Name.Value |> Process.decomposeName |> snd).IsSome then
                    (x.Name.Value |> Process.decomposeName |> fst)
                elif x.ExecutesProtocol.IsSome && x.ExecutesProtocol.Value.Name.IsSome then
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

    static member merge (pss : #IEnumerable<#IEnumerable<QSheet>>) =
        let l = 
            Seq.concat pss
            |> Seq.toList
        QProcessSequence.updateNodesAgainst l l
        |> QProcessSequence

    member this.TryGetChildProtocolOf(parentProtocolType : OntologyAnnotation) =
        this.Sheets
        |> List.collect (fun s -> s.Protocols)
        |> List.choose (fun p -> if p.IsChildProtocolTypeOf(parentProtocolType) then Some p else None)
        |> Option.fromValueWithDefault []

    member this.TryGetChildProtocolOf(parentProtocolType : OntologyAnnotation, obo : Obo.OboOntology) =
        this.Sheets
        |> List.collect (fun s -> s.Protocols)
        |> List.choose (fun p -> if p.IsChildProtocolTypeOf(parentProtocolType, obo) then Some p else None)
        |> Option.fromValueWithDefault []

    /// Returns the list of all nodes (sources, samples, data) in the ProcessSequence
    static member getNodes (ps : #QProcessSequence) =
        ps.Protocols 
        |> List.collect (fun p -> 
            p.Rows 
            |> List.collect (fun r -> 
                [
                    QNode(r.Input,r.InputType.Value,ps)
                    QNode(r.Output,r.OutputType.Value,ps)
                ]
            )
        )
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
        let inputs = ps.Protocols |> List.collect (fun p -> p.Rows |> List.map (fun r -> r.Input,r.InputType.Value))
        let outputs =  ps.Protocols |> List.collect (fun p -> p.Rows |> List.map (fun r -> r.Output)) |> Set.ofList
        inputs
        |> List.choose (fun (iname,it) -> 
            if outputs.Contains iname then
                None
            else 
                QNode(iname,it,ps)
                |> Some
            )

    /// Returns the names of all final outputs of the processSequence, which point to no further nodes
    static member getFinalOutputs (ps : #QProcessSequence) =
        let inputs = ps.Protocols |> List.collect (fun p -> p.Rows |> List.map (fun r -> r.Input)) |> Set.ofList
        let outputs =  ps.Protocols |> List.collect (fun p -> p.Rows |> List.map (fun r -> r.Output, r.OutputType.Value))
        outputs
        |> List.choose (fun (oname,ot) -> 
            if inputs.Contains oname then
                None
            else 
                QNode(oname,ot,ps)
                |> Some
            )

    /// Returns the names of all nodes for which the predicate reutrns true
    static member getNodesBy (predicate : QueryModel.IOType -> bool) (ps : #QProcessSequence) =
        ps.Protocols 
        |> List.collect (fun p -> 
            p.Rows 
            |> List.collect (fun r -> 
                [                   
                    if predicate r.InputType.Value then QNode(r.Input, r.InputType.Value, ps); 
                    if predicate r.OutputType.Value then  QNode(r.Output, r.InputType.Value, ps)
                ])
        )
        |> List.distinct 

    /// Returns the names of all initial inputs final outputs of the processSequence, to which no processPoints, and for which the predicate returns true
    static member getRootInputsBy (predicate : QueryModel.IOType -> bool) (ps : #QProcessSequence) =
        let mappings = 
            ps.Protocols 
            |> List.collect (fun p -> 
                p.Rows 
                |> List.map (fun r -> QNode(r.Input,r.InputType.Value,ps), QNode(r.Output,r.OutputType.Value,ps))
                |> List.distinct
            ) 
            |> List.groupBy fst 
            |> List.map (fun (out,ins) -> out, ins |> List.map snd)
            |> Map.ofList

        let predicate (entity : QNode) =
            predicate entity.IOType

        let rec loop (searchEntities : QNode list) (foundEntities : QNode list) = 
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
                |> List.map (fun r -> QNode(r.Output,r.OutputType.Value,ps), QNode(r.Input,r.InputType.Value,ps))
                |> List.distinct
            ) 
            |> List.groupBy fst 
            |> List.map (fun (out,ins) -> out, ins |> List.map snd)
            |> Map.ofList  

        let predicate (entity : QNode) =
            predicate entity.IOType


        let rec loop (searchEntities : QNode list) (foundEntities : QNode list) = 
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
                        KeyValuePair((inp.Name,r.Output),v)
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
                        KeyValuePair((r.Input,out.Name),v)
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
                            KeyValuePair((inp.Name,out.Name),v)
                        )
                    )
                )
            )
        )
        |> IOValueCollection

    /// Returns the names of all nodes in the Process sequence
    member this.NodesOf(node : QNode) =
        QProcessSequence.getNodesOfBy (fun _ -> true) node.Name this

        /// Returns the names of all nodes in the Process sequence
    member this.NodesOf(node) =
        QProcessSequence.getNodesOfBy (fun _ -> true) node this

        /// Returns the names of all the input nodes in the Process sequence to which no output points, that are connected to the given node
    member this.FirstNodesOf(node : QNode) = 
        QProcessSequence.getRootInputsOfBy (fun _ -> true) node.Name this

    /// Returns the names of all the output nodes in the Process sequence that point to no input, that are connected to the given node
    member this.LastNodesOf(node : QNode) = 
        QProcessSequence.getFinalOutputsOfBy (fun _ -> true) node.Name this

    /// Returns the names of all the input nodes in the Process sequence to which no output points, that are connected to the given node
    member this.FirstNodesOf(node) = 
        QProcessSequence.getRootInputsOfBy (fun _ -> true) node this

    /// Returns the names of all the output nodes in the Process sequence that point to no input, that are connected to the given node
    member this.LastNodesOf(node) = 
        QProcessSequence.getFinalOutputsOfBy (fun _ -> true) node this

    /// Returns the names of all samples in the Process sequence, that are connected to the given node
    member this.SamplesOf(node : QNode) =
        QProcessSequence.getNodesOfBy (fun (io : IOType) -> io.isSample) node.Name this

        /// Returns the names of all samples in the Process sequence, that are connected to the given node
    member this.SamplesOf(node) =
        QProcessSequence.getNodesOfBy (fun (io : IOType) -> io.isSample) node this

    /// Returns the names of all the input samples in the Process sequence to which no output points, that are connected to the given node
    member this.FirstSamplesOf(node : QNode) = 
        QProcessSequence.getRootInputsOfBy (fun (io : IOType) -> io.isSample) node.Name this

    /// Returns the names of all the output samples in the Process sequence that point to no input, that are connected to the given node
    member this.LastSamplesOf(node : QNode) = 
        QProcessSequence.getFinalOutputsOfBy (fun (io : IOType) -> io.isSample) node.Name this

    /// Returns the names of all the input samples in the Process sequence to which no output points, that are connected to the given node
    member this.FirstSamplesOf(node) = 
        QProcessSequence.getRootInputsOfBy (fun (io : IOType) -> io.isSample) node this

    /// Returns the names of all the output samples in the Process sequence that point to no input, that are connected to the given node
    member this.LastSamplesOf(node) = 
        QProcessSequence.getFinalOutputsOfBy (fun (io : IOType) -> io.isSample) node this

    /// Returns the names of all sources in the Process sequence, that are connected to the given node
    member this.SourcesOf(node : QNode) =
        QProcessSequence.getNodesOfBy (fun (io : IOType) -> io.isSource) node.Name this

    /// Returns the names of all sources in the Process sequence, that are connected to the given node
    member this.SourcesOf(node) =
        QProcessSequence.getNodesOfBy (fun (io : IOType) -> io.isSource) node this

    /// Returns the names of all data in the Process sequence, that are connected to the given node
    member this.DataOf(node : QNode) =
        QProcessSequence.getNodesOfBy (fun (io : IOType) -> io.isData) node.Name this

    /// Returns the names of all data in the Process sequence, that are connected to the given node
    member this.DataOf(node) =
        QProcessSequence.getNodesOfBy (fun (io : IOType) -> io.isData) node this

    /// Returns the names of all the input data in the Process sequence to which no output points, that are connected to the given node
    member this.FirstDataOf(node : QNode) = 
        QProcessSequence.getRootInputsOfBy (fun (io : IOType) -> io.isData) node.Name this

    /// Returns the names of all the output data in the Process sequence that point to no input, that are connected to the given node
    member this.LastDataOf(node : QNode) = 
        QProcessSequence.getFinalOutputsOfBy (fun (io : IOType) -> io.isData) node.Name this

    /// Returns the names of all the input data in the Process sequence to which no output points, that are connected to the given node
    member this.FirstDataOf(node) = 
        QProcessSequence.getRootInputsOfBy (fun (io : IOType) -> io.isData) node this

    /// Returns the names of all the output data in the Process sequence that point to no input, that are connected to the given node
    member this.LastDataOf(node) = 
        QProcessSequence.getFinalOutputsOfBy (fun (io : IOType) -> io.isData) node this

    /// Returns the names of all raw data in the Process sequence, that are connected to the given node
    member this.RawDataOf(node : QNode) =
        QProcessSequence.getNodesOfBy (fun (io : IOType) -> io.isRawData) node.Name this

    /// Returns the names of all raw data in the Process sequence, that are connected to the given node
    member this.RawDataOf(node) =
        QProcessSequence.getNodesOfBy (fun (io : IOType) -> io.isRawData) node this
    
    /// Returns the names of all the input raw data in the Process sequence to which no output points, that are connected to the given node
    member this.FirstRawDataOf(node : QNode) = 
        QProcessSequence.getRootInputsOfBy (fun (io : IOType) -> io.isRawData) node.Name this

    /// Returns the names of all the output raw data in the Process sequence that point to no input, that are connected to the given node
    member this.LastRawDataOf(node : QNode) = 
        QProcessSequence.getFinalOutputsOfBy (fun (io : IOType) -> io.isRawData) node.Name this

    /// Returns the names of all the input raw data in the Process sequence to which no output points, that are connected to the given node
    member this.FirstRawDataOf(node) = 
        QProcessSequence.getRootInputsOfBy (fun (io : IOType) -> io.isRawData) node this

    /// Returns the names of all the output raw data in the Process sequence that point to no input, that are connected to the given node
    member this.LastRawDataOf(node) = 
        QProcessSequence.getFinalOutputsOfBy (fun (io : IOType) -> io.isRawData) node this

    /// Returns the names of all processed data in the Process sequence, that are connected to the given node
    member this.ProcessedDataOf(node : QNode) =
        QProcessSequence.getNodesOfBy (fun (io : IOType) -> io.isProcessedData) node.Name this

    /// Returns the names of all processed data in the Process sequence, that are connected to the given node
    member this.ProcessedDataOf(node) =
        QProcessSequence.getNodesOfBy (fun (io : IOType) -> io.isProcessedData) node this

    /// Returns the names of all the input processed data in the Process sequence to which no output points, that are connected to the given node
    member this.FirstProcessedDataOf(node : QNode) = 
        QProcessSequence.getRootInputsOfBy (fun (io : IOType) -> io.isProcessedData) node.Name this

    /// Returns the names of all the output processed data in the Process sequence that point to no input, that are connected to the given node
    member this.LastProcessedDataOf(node : QNode) = 
        QProcessSequence.getFinalOutputsOfBy (fun (io : IOType) -> io.isProcessedData) node.Name this

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
        |> List.collect (fun s -> s.Values.Values().WithCategory(ontology).Values)
        |> ValueCollection

    /// Returns all values in the process sequence whose header matches the given name
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.Values(name : string, ?ProtocolName) = 
        (QProcessSequence.onlyValuesOfProtocol this ProtocolName).Sheets
        |> List.collect (fun s -> s.Values.Values().WithName(name).Values)
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

    /// Returns all components in the process sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.Components(?ProtocolName) =
        (QProcessSequence.onlyValuesOfProtocol this ProtocolName).Values().Components()

    /// Returns all values in the process sequence, that are connected to the given node
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.ValuesOf(node : string, ?ProtocolName : string) =
        let ps = QProcessSequence.onlyValuesOfProtocol this ProtocolName
        (QProcessSequence.getPreviousValuesOf ps node).Values @ (QProcessSequence.getSucceedingValuesOf ps node).Values
        |> ValueCollection

    /// Returns all values in the process sequence, that are connected to the given node
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.ValuesOf(node : QNode, ?ProtocolName : string) =
        let ps = QProcessSequence.onlyValuesOfProtocol this ProtocolName
        (QProcessSequence.getPreviousValuesOf ps node.Name).Values @ (QProcessSequence.getSucceedingValuesOf ps node.Name).Values
        |> ValueCollection

    /// Returns all values in the process sequence, that are connected to the given node and come before it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.PreviousValuesOf(node : string, ?ProtocolName) =
        let ps = QProcessSequence.onlyValuesOfProtocol this ProtocolName
        QProcessSequence.getPreviousValuesOf ps node

    /// Returns all values in the process sequence, that are connected to the given node and come before it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.PreviousValuesOf(node : QNode, ?ProtocolName) =
        let ps = QProcessSequence.onlyValuesOfProtocol this ProtocolName
        QProcessSequence.getPreviousValuesOf ps node.Name      

    /// Returns all values in the process sequence, that are connected to the given node and come after it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.SucceedingValuesOf(node : string, ?ProtocolName) =
        let ps = QProcessSequence.onlyValuesOfProtocol this ProtocolName
        QProcessSequence.getSucceedingValuesOf ps node

    /// Returns all values in the process sequence, that are connected to the given node and come after it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.SucceedingValuesOf(node : QNode, ?ProtocolName) =
        let ps = QProcessSequence.onlyValuesOfProtocol this ProtocolName
        QProcessSequence.getSucceedingValuesOf ps node.Name

    /// Returns all characteristic values in the process sequence, that are connected to the given node
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.CharacteristicsOf(node : string, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).ValuesOf(node).Characteristics()

    /// Returns all characteristic values in the process sequence, that are connected to the given node
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.CharacteristicsOf(node : QNode, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).ValuesOf(node).Characteristics()

    /// Returns all characteristic values in the process sequence, that are connected to the given node and come before it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.PreviousCharacteristicsOf(node : string, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).PreviousValuesOf(node).Characteristics()

    /// Returns all characteristic values in the process sequence, that are connected to the given node and come before it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.PreviousCharacteristicsOf(node : QNode, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).PreviousValuesOf(node).Characteristics()

    /// Returns all characteristic values in the process sequence, that are connected to the given node and come after it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.SucceedingCharacteristicsOf(node : string, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).SucceedingValuesOf(node).Characteristics()

    /// Returns all characteristic values in the process sequence, that are connected to the given node and come after it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.SucceedingCharacteristicsOf(node : QNode, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).SucceedingValuesOf(node).Characteristics()

    /// Returns all parameter values in the process sequence, that are connected to the given node 
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.ParametersOf(node : string, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).ValuesOf(node).Parameters()

    /// Returns all parameter values in the process sequence, that are connected to the given node 
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.ParametersOf(node : QNode, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).ValuesOf(node).Parameters()

    /// Returns all parameter values in the process sequence, that are connected to the given node and come before it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.PreviousParametersOf(node : string, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).PreviousValuesOf(node).Parameters()

    /// Returns all parameter values in the process sequence, that are connected to the given node and come before it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.PreviousParametersOf(node : QNode, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).PreviousValuesOf(node).Parameters()

    /// Returns all parameter values in the process sequence, that are connected to the given node and come after it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.SucceedingParametersOf(node : string, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).SucceedingValuesOf(node).Parameters()

    /// Returns all parameter values in the process sequence, that are connected to the given node and come after it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.SucceedingParametersOf(node : QNode, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).SucceedingValuesOf(node).Parameters()

    /// Returns all factor values in the process sequence, that are connected to the given node 
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.FactorsOf(node : string, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).ValuesOf(node).Factors()

    /// Returns all factor values in the process sequence, that are connected to the given node 
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.FactorsOf(node : QNode, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).ValuesOf(node).Factors()

    /// Returns all factor values in the process sequence, that are connected to the given node and come before it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.PreviousFactorsOf(node : string, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).PreviousValuesOf(node).Factors()

    /// Returns all factor values in the process sequence, that are connected to the given node and come before it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.PreviousFactorsOf(node : QNode, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).PreviousValuesOf(node).Factors()

    /// Returns all factor values in the process sequence, that are connected to the given node 
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol and come after it in the sequence
    member this.SucceedingFactorsOf(node : string, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).SucceedingValuesOf(node).Factors()

    /// Returns all factor values in the process sequence, that are connected to the given node 
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol and come after it in the sequence
    member this.SucceedingFactorsOf(node : QNode, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).SucceedingValuesOf(node).Factors()

    /// Returns all components values in the process sequence, that are connected to the given node
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.ComponentsOf(node : string, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).ValuesOf(node).Components()

    /// Returns all components values in the process sequence, that are connected to the given node
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.ComponentsOf(node : QNode, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).ValuesOf(node).Components()

    /// Returns all components values in the process sequence, that are connected to the given node and come before it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.PreviousComponentsOf(node : string, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).PreviousValuesOf(node).Components()

    /// Returns all components values in the process sequence, that are connected to the given node and come before it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.PreviousComponentsOf(node : QNode, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).PreviousValuesOf(node).Components()

    /// Returns all components values in the process sequence, that are connected to the given node and come after it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.SucceedingComponentsOf(node : string, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).SucceedingValuesOf(node).Components()

    /// Returns all components values in the process sequence, that are connected to the given node and come after it in the sequence
    ///
    /// If a protocol name is given, returns only the values of the processes that implement this protocol
    member this.SucceedingComponentsOf(node : QNode, ?ProtocolName) =
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).SucceedingValuesOf(node).Components()

    member this.Contains(ontology : OntologyAnnotation, ?ProtocolName) = 
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).Values().Contains ontology

    member this.Contains(name : string, ?ProtocolName) = 
         (QProcessSequence.onlyValuesOfProtocol this ProtocolName).Values().Contains name

type QProcessSequence with

    /// Returns the names of all nodes in the Process sequence
    member this.Nodes =
        QProcessSequence.getNodes(this)

    /// Returns the names of all the input nodes in the Process sequence to which no output points
    member this.FirstNodes = 
        QProcessSequence.getRootInputs(this)

    /// Returns the names of all the output nodes in the Process sequence that point to no input
    member this.LastNodes = 
        QProcessSequence.getFinalOutputs(this)

    /// Returns the names of all samples in the Process sequence
    member this.Samples =
        QProcessSequence.getNodesBy (fun (io : IOType) -> io.isSample) this

    /// Returns the names of all the input samples in the Process sequence to which no output points
    member this.FirstSamples = 
        QProcessSequence.getRootInputsBy (fun (io : IOType) -> io.isSample) this

    /// Returns the names of all the output samples in the Process sequence that point to no input
    member this.LastSamples = 
        QProcessSequence.getFinalOutputsBy (fun (io : IOType) -> io.isSample) this

    /// Returns the names of all sources in the Process sequence
    member this.Sources =
        QProcessSequence.getNodesBy (fun (io : IOType) -> io.isSource) this

    /// Returns the names of all data in the Process sequence
    member this.Data =
        QProcessSequence.getNodesBy (fun (io : IOType) -> io.isData) this

    /// Returns the names of all the input data in the Process sequence to which no output points
    member this.FirstData = 
        QProcessSequence.getRootInputsBy (fun (io : IOType) -> io.isData) this

    /// Returns the names of all the output data in the Process sequence that point to no input
    member this.LastData = 
        QProcessSequence.getFinalOutputsBy (fun (io : IOType) -> io.isData) this

    /// Returns the names of all raw data in the Process sequence
    member this.RawData =
        QProcessSequence.getNodesBy (fun (io : IOType) -> io.isRawData) this

    /// Returns the names of all the input raw data in the Process sequence to which no output points
    member this.FirstRawData = 
        QProcessSequence.getRootInputsBy (fun (io : IOType) -> io.isRawData) this

    /// Returns the names of all the output raw data in the Process sequence that point to no input
    member this.LastRawData = 
        QProcessSequence.getFinalOutputsBy (fun (io : IOType) -> io.isRawData) this
    
    /// Returns the names of all processed data in the Process sequence
    member this.ProcessedData =
        QProcessSequence.getNodesBy (fun (io : IOType) -> io.isProcessedData) this

    /// Returns the names of all the input processed data in the Process sequence to which no output points
    member this.FirstProcessedData = 
        QProcessSequence.getRootInputsBy (fun (io : IOType) -> io.isProcessedData) this

    /// Returns the names of all the output processed data in the Process sequence that point to no input
    member this.LastProcessedData = 
        QProcessSequence.getFinalOutputsBy (fun (io : IOType) -> io.isProcessedData) this

/// One Node of an ISA Process Sequence (Source, Sample, Data)
type QNode(Name : string, IOType : IOType, ?ParentProcessSequence : QProcessSequence) =
    
    /// Returns the process sequence in which the node appears
    member this.ParentProcessSequence = ParentProcessSequence |> Option.defaultValue (QProcessSequence([]))

    /// Identifying name of the node
    member this.Name = Name

    /// Type of node (source, sample, data, raw data ...)
    member this.IOType : IOType = IOType

    interface System.IEquatable<QNode> with
        member this.Equals other = other.Name.Equals this.Name

    override this.Equals other =
        match other with
        | :? QNode as p -> (this :> System.IEquatable<_>).Equals p
        | _ -> false

    override this.GetHashCode () = this.Name.GetHashCode()

    interface System.IComparable with
        member this.CompareTo other =
            match other with
            | :? QNode as p -> (this :> System.IComparable<_>).CompareTo p
            | _ -> -1

    interface System.IComparable<QNode> with
        member this.CompareTo other = other.Name.CompareTo this.Name

    /// Returns true, if the node is a source
    member this.isSource = this.IOType.isSource

    /// Returns true, if the node is a sample
    member this.isSample = this.IOType.isSample
    
    /// Returns true, if the node is a data
    member this.isData = this.IOType.isData

    /// Returns true, if the node is a raw data
    member this.isRawData = this.IOType.isRawData
    
    /// Returns true, if the node is a processed data
    member this.isProcessedData = this.IOType.isProcessedData

    /// Returns true, if the node is a material
    member this.isMaterial = this.IOType.isMaterial


[<AutoOpen>]
module QNodeExtensions =

    type QNode with

        /// Returns all other nodes in the process sequence, that are connected to this node
        member this.Nodes = this.ParentProcessSequence.NodesOf(this)

        /// Returns all other nodes in the process sequence, that are connected to this node and have no more origin nodes pointing to them
        member this.FirstNodes = this.ParentProcessSequence.FirstNodesOf(this)

        /// Returns all other nodes in the process sequence, that are connected to this node and have no more sink nodes they point to
        member this.LastNodes = this.ParentProcessSequence.LastNodesOf(this)

        /// Returns all other samples in the process sequence, that are connected to this node
        member this.Samples = this.ParentProcessSequence.SamplesOf(this)

        /// Returns all other samples in the process sequence, that are connected to this node and have no more origin nodes pointing to them
        member this.FirstSamples = this.ParentProcessSequence.FirstSamplesOf(this)
        
        /// Returns all other samples in the process sequence, that are connected to this node and have no more sink nodes they point to
        member this.LastSamples = this.ParentProcessSequence.LastSamplesOf(this)

        /// Returns all other sources in the process sequence, that are connected to this node
        member this.Sources = this.ParentProcessSequence.SourcesOf(this)

        /// Returns all other data in the process sequence, that are connected to this node
        member this.Data = this.ParentProcessSequence.FirstDataOf(this)

        /// Returns all other data in the process sequence, that are connected to this node and have no more origin nodes pointing to them
        member this.FirstData = this.ParentProcessSequence.FirstDataOf(this)

        /// Returns all other data in the process sequence, that are connected to this node and have no more sink nodes they point to
        member this.LastData = this.ParentProcessSequence.LastNodesOf(this)

        /// Returns all other raw data in the process sequence, that are connected to this node
        member this.RawData = this.ParentProcessSequence.RawDataOf(this)

        /// Returns all other raw data in the process sequence, that are connected to this node and have no more origin nodes pointing to them
        member this.FirstRawData = this.ParentProcessSequence.FirstRawDataOf(this)

        /// Returns all other raw data in the process sequence, that are connected to this node and have no more sink nodes they point to
        member this.LastRawData = this.ParentProcessSequence.LastRawDataOf(this)

        /// Returns all other processed data in the process sequence, that are connected to this node
        member this.ProcessedData = this.ParentProcessSequence.ProcessedDataOf(this)

        /// Returns all other processed data in the process sequence, that are connected to this node and have no more sink nodes they point to
        member this.FirstProcessedData = this.ParentProcessSequence.FirstProcessedDataOf(this)

        /// Returns all other processed data in the process sequence, that are connected to this node and have no more sink nodes they point to
        member this.LastProcessedData = this.ParentProcessSequence.LastProcessedDataOf(this)

        /// Returns all values in the process sequence, that are connected to this given node
        member this.Values = this.ParentProcessSequence.ValuesOf(this)

        /// Returns all values in the process sequence, that are connected to this given node and come before it in the sequence
        member this.PreviousValues = this.ParentProcessSequence.PreviousValuesOf(this)

        /// Returns all values in the process sequence, that are connected to the given node and come after it in the sequence
        member this.SucceedingValues = this.ParentProcessSequence.SucceedingValuesOf(this)

        /// Returns all characteristic values in the process sequence, that are connected to the given node
        member this.Characteristics = this.ParentProcessSequence.CharacteristicsOf(this)

        /// Returns all characteristic values in the process sequence, that are connected to the given node and come before it in the sequence
        member this.PreviousCharacteristics = this.ParentProcessSequence.PreviousCharacteristicsOf(this)

        /// Returns all characteristic values in the process sequence, that are connected to the given node and come after it in the sequence
        member this.SucceedingCharacteristics = this.ParentProcessSequence.SucceedingCharacteristicsOf(this)

        /// Returns all parameter values in the process sequence, that are connected to the given node
        member this.Parameters = this.ParentProcessSequence.ParametersOf(this)

        /// Returns all parameter values in the process sequence, that are connected to the given node and come before it in the sequence
        member this.PreviousParameters = this.ParentProcessSequence.PreviousParametersOf(this)

        /// Returns all parameter values in the process sequence, that are connected to the given node and come after it in the sequence
        member this.SucceedingParameters = this.ParentProcessSequence.SucceedingParametersOf(this)

       /// Returns all factor values in the process sequence, that are connected to the given node
        member this.Factors = this.ParentProcessSequence.FactorsOf(this)

        /// Returns all factor values in the process sequence, that are connected to the given node and come before it in the sequence
        member this.PreviousFactors = this.ParentProcessSequence.PreviousFactorsOf(this)

        /// Returns all factor values in the process sequence, that are connected to the given node and come after it in the sequence
        member this.SucceedingFactors = this.ParentProcessSequence.SucceedingFactorsOf(this)

        /// Returns all component values in the process sequence, that are connected to the given node
        member this.Components = this.ParentProcessSequence.ComponentsOf(this)

        /// Returns all component values in the process sequence, that are connected to the given node and come before it in the sequence
        member this.PreviousComponents = this.ParentProcessSequence.PreviousComponentsOf(this)

        /// Returns all component values in the process sequence, that are connected to the given node and come after it in the sequence
        member this.SucceedingComponents = this.ParentProcessSequence.SucceedingComponentsOf(this)
