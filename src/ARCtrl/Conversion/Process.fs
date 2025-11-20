namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.FileSystem
open System.Collections.Generic
//open ColumnIndex

open ColumnIndex
open ARCtrl.Helper.Regex.ActivePatterns


/// Functions for parsing ArcTables to ISA json Processes and vice versa
type ProcessConversion = 

    static member tryGetProtocolType (pv : LDNode, ?graph : LDGraph, ?context : LDContext) =
        match LDLabProtocol.tryGetIntendedUseAsDefinedTerm(pv,?graph = graph, ?context = context) with
        | Some dt ->
            Some (BaseTypes.decomposeDefinedTerm(dt, ?context = context))
        | None ->
            match LDLabProtocol.tryGetIntendedUseAsString(pv, ?context = context) with
            | Some s -> Some (OntologyAnnotation.create(name = s))
            | None -> None

    static member composeProcessName (processNameRoot : string) (i : int) =
        $"{processNameRoot}_{i}"

    static member decomposeProcessName (name : string) =
        let pattern = """(?<name>.+)_(?<num>\d+)"""

        match name with 
        | Regex pattern r ->
            (r.Groups.Item "name").Value, Some ((r.Groups.Item "num").Value |> int)
        | _ ->
            name, None
    // Explanation of the Getter logic:
    // The getter logic is used to treat every value of the table only once
    // First, the headers are checked for what getter applies to the respective column. E.g. a ProtocolType getter will only return a function for parsing protocolType cells if the header depicts a protocolType.
    // The appropriate getters are then applied in the context of the processGetter, parsing the cells of the matrix

    /// If the given headers depict a component, returns a function for parsing the values of the matrix to the values of this component
    static member tryComponentGetter (generalI : int) (valueI : int) (valueHeader : CompositeHeader) =
        match valueHeader with
        | CompositeHeader.Component oa ->
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader valueHeader None
                let c = BaseTypes.composeComponent valueHeader cell
                c.SetColumnIndex valueI
                c
            |> Some
        | _ -> None    
            
    /// If the given headers depict a parameter, returns a function for parsing the values of the matrix to the values of this type
    static member tryParameterGetter (generalI : int) (valueI : int) (valueHeader : CompositeHeader) =
        match valueHeader with
        | CompositeHeader.Parameter oa ->
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader valueHeader None
                let p = BaseTypes.composeParameterValue valueHeader cell
                p.SetColumnIndex valueI
                p
            |> Some
        | _ -> None 

    /// If the given headers depict a factor, returns a function for parsing the values of the matrix to the values of this type
    static member tryFactorGetter (generalI : int) (valueI : int) (valueHeader : CompositeHeader) =
        match valueHeader with
        | CompositeHeader.Factor oa ->
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader valueHeader None
                let f = BaseTypes.composeFactorValue valueHeader cell
                f.SetColumnIndex valueI
                f
            |> Some
        | _ -> None 

    /// If the given headers depict a protocolType, returns a function for parsing the values of the matrix to the values of this type
    static member tryCharacteristicGetter (generalI : int) (valueI : int) (valueHeader : CompositeHeader) =
        match valueHeader with
        | CompositeHeader.Characteristic oa ->        
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader valueHeader None
                let c = BaseTypes.composeCharacteristicValue valueHeader cell
                c.SetColumnIndex valueI
                c
            |> Some
        | _ -> None 

    /// If the given headers depict a protocolType, returns a function for parsing the values of the matrix to the values of this type
    static member tryGetProtocolTypeGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.ProtocolType ->
            fun (table : ArcTable) i ->
                let oa =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell.AsTerm
                    | None -> OntologyAnnotation()
                BaseTypes.composeDefinedTerm oa
            |> Some
        | _ -> None 


    /// If the given headers depict a protocolREF, returns a function for parsing the values of the matrix to the values of this type
    static member tryGetProtocolREFGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.ProtocolREF ->
            fun (table : ArcTable) i ->
                match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                | Some cell -> cell.AsFreeText
                | None -> ""
            |> Some
        | _ -> None

    /// If the given headers depict a protocolDescription, returns a function for parsing the values of the matrix to the values of this type
    static member tryGetProtocolDescriptionGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.ProtocolDescription ->
            fun (table : ArcTable) i ->
                match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                | Some cell -> cell.AsFreeText
                | None -> ""
            |> Some
        | _ -> None
   
    /// If the given headers depict a protocolURI, returns a function for parsing the values of the matrix to the values of this type
    static member tryGetProtocolURIGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.ProtocolUri ->
            fun (table : ArcTable) i ->
                match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                | Some cell -> cell.AsFreeText
                | None -> ""
            |> Some
        | _ -> None

    /// If the given headers depict a protocolVersion, returns a function for parsing the values of the matrix to the values of this type
    static member tryGetProtocolVersionGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.ProtocolVersion ->
            fun (table : ArcTable) i ->
                match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                | Some cell -> cell.AsFreeText
                | None -> ""
            |> Some
        | _ -> None

    /// If the given headers depict an input, returns a function for parsing the values of the matrix to the values of this type
    static member tryGetInputGetter (generalI : int) (header : CompositeHeader) (fs : FileSystem option) =
        match header with
        | CompositeHeader.Input io ->
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader header None
                BaseTypes.composeProcessInput header cell fs
            |> Some
        | _ -> None

    /// If the given headers depict an output, returns a function for parsing the values of the matrix to the values of this type
    static member tryGetOutputGetter (generalI : int) (header : CompositeHeader) (fs : FileSystem option) =
        match header with
        | CompositeHeader.Output io ->
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader header None
                BaseTypes.composeProcessOutput header cell fs
            |> Some
        | _ -> None

    /// If the given headers depict a comment, returns a function for parsing the values of the matrix to the values of this type
    static member tryGetCommentGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.Comment c ->
            fun (table : ArcTable) i ->
                let comment =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> Comment(c,cell.AsFreeText)
                    | None -> Comment(c)
                comment.ToString()
            |> Some
        | _ -> None

    static member tryGetPerformerGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.Performer ->
            fun (table : ArcTable) i ->
                let performer =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell.AsFreeText
                    | None -> ""
                let person = LDPerson.create(performer)
                person
            |> Some
        | _ -> None

    /// Given the header sequence of an ArcTable, returns a function for parsing each row of the table to a process
    static member getProcessGetter (assayName: string option) (studyName : string option) (processNameRoot : string) (headers : CompositeHeader seq) (fs : FileSystem option) =
    
        let headers = 
            headers
            |> Seq.indexed

        let valueHeaders =
            headers
            |> Seq.filter (snd >> fun h -> h.IsCvParamColumn)
            |> Seq.indexed
            |> Seq.toList

        let charGetters =
            valueHeaders 
            |> List.choose (fun (valueI,(generalI,header)) -> ProcessConversion.tryCharacteristicGetter generalI valueI header)

        let factorValueGetters =
            valueHeaders
            |> List.choose (fun (valueI,(generalI,header)) -> ProcessConversion.tryFactorGetter generalI valueI header)

        let parameterValueGetters =
            valueHeaders
            |> List.choose (fun (valueI,(generalI,header)) -> ProcessConversion.tryParameterGetter generalI valueI header)

        let componentGetters =
            valueHeaders
            |> List.choose (fun (valueI,(generalI,header)) -> ProcessConversion.tryComponentGetter generalI valueI header)

        let protocolTypeGetter = 
            headers
            |> Seq.tryPick (fun (generalI,header) -> ProcessConversion.tryGetProtocolTypeGetter generalI header)

        let protocolREFGetter = 
            headers
            |> Seq.tryPick (fun (generalI,header) -> ProcessConversion.tryGetProtocolREFGetter generalI header)

        let protocolDescriptionGetter = 
            headers
            |> Seq.tryPick (fun (generalI,header) -> ProcessConversion.tryGetProtocolDescriptionGetter generalI header)

        let protocolURIGetter = 
            headers
            |> Seq.tryPick (fun (generalI,header) -> ProcessConversion.tryGetProtocolURIGetter generalI header)

        let protocolVersionGetter =
            headers
            |> Seq.tryPick (fun (generalI,header) -> ProcessConversion.tryGetProtocolVersionGetter generalI header)

        let performerGetter = 
            headers
            |> Seq.tryPick (fun (generalI,header) -> ProcessConversion.tryGetPerformerGetter generalI header)

        let commentGetters = 
            headers
            |> Seq.choose (fun (generalI,header) -> ProcessConversion.tryGetCommentGetter generalI header)
            |> Seq.toList

        let inputGetter =
            match headers |> Seq.tryPick (fun (generalI,header) -> ProcessConversion.tryGetInputGetter generalI header fs) with
            | Some inputGetter ->
                fun (table : ArcTable) i ->
                    let chars = charGetters |> Seq.map (fun f -> f table i) |> ResizeArray
                    let input = inputGetter table i

                    if chars.Count > 0 then
                        LDSample.setAdditionalProperties(input,chars)
                    input
                    |> ResizeArray.singleton
            | None when charGetters.Length <> 0 ->
                fun (table : ArcTable) i ->
                    let chars = charGetters |> Seq.map (fun f -> f table i) |> ResizeArray
                    LDSample.createSample(name = $"{processNameRoot}_Input_{i}", additionalProperties = chars)
                    |> ResizeArray.singleton
            | None ->
                fun (table : ArcTable) i ->
                    ResizeArray []
            
        // This is a little more complex, as data and material objects can't contain factors. So in the case where the output of the table is a data object but factors exist. An additional sample object with the same name is created to contain the factors.
        let outputGetter =
            match headers |> Seq.tryPick (fun (generalI,header) -> ProcessConversion.tryGetOutputGetter generalI header fs) with
            | Some outputGetter ->
                fun (table : ArcTable) i ->
                    let factors = factorValueGetters |> Seq.map (fun f -> f table i) |> ResizeArray
                    let output = outputGetter table i
                    if factors.Count > 0 then
                        LDSample.setAdditionalProperties(output,factors)
                    output
                    |> ResizeArray.singleton
            | None when factorValueGetters.Length <> 0 ->
                fun (table : ArcTable) i ->
                    let factors = factorValueGetters |> Seq.map (fun f -> f table i) |> ResizeArray
                    LDSample.createSample(name = $"{processNameRoot}_Output_{i}", additionalProperties = factors)
                    |> ResizeArray.singleton
            | None ->
                fun (table : ArcTable) i ->
                    ResizeArray []
        fun (table : ArcTable) i ->

            let rowCount = table.RowCount
            let pn =
                if rowCount = 1 then processNameRoot
                else ProcessConversion.composeProcessName processNameRoot i

            let paramvalues = parameterValueGetters |> List.map (fun f -> f table i) |> Option.fromValueWithDefault [] |> Option.map ResizeArray
            //let parameters = paramvalues |> Option.map (List.map (fun pv -> pv.Category.Value))

            let comments = commentGetters |> List.map (fun f -> f table i) |> Option.fromValueWithDefault [] |> Option.map ResizeArray

            let components = componentGetters |> List.map (fun f -> f table i) |> Option.fromValueWithDefault [] |> Option.map ResizeArray

            let id = LDLabProcess.genId(processNameRoot,?assayName = assayName, ?studyName = studyName) + $"_{i}"
            
            let protocol : LDNode option =
                let name = (protocolREFGetter |> Option.map (fun f -> f table i))
                let protocolId = LDLabProtocol.genId(?name = name, processName = processNameRoot)
                LDLabProtocol.create(
                    id = protocolId,
                    ?name = name,
                    ?description = (protocolDescriptionGetter |> Option.map (fun f -> f table i)),
                    ?intendedUse = (protocolTypeGetter |> Option.map (fun f -> f table i)),
                    //?comments = comments,
                    ?url = (protocolURIGetter |> Option.map (fun f -> f table i)),
                    ?version = (protocolVersionGetter |> Option.map (fun f -> f table i)),
                    ?labEquipments = components
                )
                |> Some

            let input,output = inputGetter table i, outputGetter table i

            let agent = performerGetter |> Option.map (fun f -> f table i)

            LDLabProcess.create(
                name = pn,
                objects = input,
                results = output,
                id = id,
                ?agent = agent,
                ?executesLabProtocol = protocol,
                ?parameterValues = paramvalues,
                ?disambiguatingDescriptions = comments

                )

    /// Groups processes by their name, or by the name of the protocol they execute
    ///
    /// Process names are taken from the Worksheet name and numbered: SheetName_1, SheetName_2, etc.
    /// 
    /// This function decomposes this name into a root name and a number, and groups processes by root name.
    static member groupProcesses (processes : LDNode list, ?graph : LDGraph, ?context : LDContext) = 
        processes
        |> List.groupBy (fun p ->
            match LDLabProcess.tryGetNameAsString (p, ?context = context), LDLabProcess.tryGetExecutesLabProtocol(p,?graph = graph, ?context = context) with
            | Some name, _ when ProcessConversion.decomposeProcessName name |> snd |> Option.isSome ->
                ProcessConversion.decomposeProcessName name |> fst
            | _, Some protocol when LDLabProtocol.tryGetNameAsString (protocol, ?context = context) |> Option.isSome ->
                LDLabProtocol.tryGetNameAsString (protocol, ?context = context) |> Option.defaultValue ""
            // Removed this case in order to fix https://github.com/nfdi4plants/ARCtrl/issues/512
            // The problem is, that through this case table-names with underscores clash, e.g. "Table_MS" and "Table_RNASeq" would be grouped together.
            //| Some name, _ when name.Contains "_" ->
            //    let lastUnderScoreIndex = name.LastIndexOf '_'
            //    name.Remove lastUnderScoreIndex
            | Some name, _ ->
                name
            | _, Some protocol  ->
                protocol.Id
            | _ ->
                Identifier.createMissingIdentifier()  
        )

    /// Merges processes with the same name, protocol and param values
    //let mergeIdenticalProcesses (processes : list<Process>) =
    //    processes
    //    |> List.groupBy (fun x -> 
    //        if x.Name.IsSome && (x.Name.Value |> Process.decomposeName |> snd).IsSome then
    //            (x.Name.Value |> Process.decomposeName |> fst), HashCodes.boxHashOption x.ExecutesProtocol, x.ParameterValues |> Option.map HashCodes.boxHashSeq, x.Comments |> Option.map HashCodes.boxHashSeq
    //        elif x.ExecutesProtocol.IsSome && x.ExecutesProtocol.Value.Name.IsSome then
    //            x.ExecutesProtocol.Value.Name.Value, HashCodes.boxHashOption x.ExecutesProtocol, x.ParameterValues |> Option.map HashCodes.boxHashSeq, x.Comments |> Option.map HashCodes.boxHashSeq
    //        else
    //            Identifier.createMissingIdentifier(), HashCodes.boxHashOption x.ExecutesProtocol, x.ParameterValues |> Option.map HashCodes.boxHashSeq, x.Comments |> Option.map HashCodes.boxHashSeq
    //    )
    //    |> fun l ->
    //        l
    //        |> List.mapi (fun i ((n,_,_,_),processes) -> 
    //            let pVs = processes.[0].ParameterValues
    //            let inputs = processes |> List.collect (fun p -> p.Inputs |> Option.defaultValue []) |> Option.fromValueWithDefault []
    //            let outputs = processes |> List.collect (fun p -> p.Outputs |> Option.defaultValue []) |> Option.fromValueWithDefault []
    //            let n = if l.Length > 1 then Process.composeName n i else n
    //            Process.create(Name = n,?ExecutesProtocol = processes.[0].ExecutesProtocol,?ParameterValues = pVs,?Inputs = inputs,?Outputs = outputs,?Comments = processes.[0].Comments)
    //        )


    // Transform a isa json process into a isa tab row, where each row is a header+value list
    static member processToRows (p : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let pvs =
            LDLabProcess.getParameterValues(p, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun ppv -> BaseTypes.decomposeParameterValue(ppv, ?context = context), ColumnIndex.tryGetIndex ppv)
        // Get the component
        let components = 
            match LDLabProcess.tryGetExecutesLabProtocol(p, ?graph = graph, ?context = context) with
            | Some prot ->
                LDLabProtocol.getComponents(prot, ?graph = graph, ?context = context)
                |> ResizeArray.map (fun ppv -> BaseTypes.decomposeComponent(ppv, ?context = context), ColumnIndex.tryGetIndex ppv)
            | None -> ResizeArray []
        // Get the values of the protocol
        let protVals = 
            match LDLabProcess.tryGetExecutesLabProtocol(p, ?graph = graph, ?context = context) with
            | Some prot ->
                [
                    match LDLabProtocol.tryGetNameAsString (prot, ?context = context) with | Some name -> yield (CompositeHeader.ProtocolREF, CompositeCell.FreeText name) | None -> ()
                    match LDLabProtocol.tryGetDescriptionAsString (prot, ?context = context) with | Some desc -> yield (CompositeHeader.ProtocolDescription, CompositeCell.FreeText desc) | None -> ()
                    match LDLabProtocol.tryGetUrl (prot, ?context = context) with | Some uri -> yield (CompositeHeader.ProtocolUri, CompositeCell.FreeText uri) | None -> ()
                    match LDLabProtocol.tryGetVersionAsString(prot, ?context = context) with | Some version -> yield (CompositeHeader.ProtocolVersion, CompositeCell.FreeText version) | None -> ()
                    match ProcessConversion.tryGetProtocolType(prot, ?graph = graph, ?context = context) with
                    | Some intendedUse -> yield (CompositeHeader.ProtocolType, CompositeCell.Term intendedUse)
                    | None -> ()
                ]
            | None -> []
        let comments = 
            LDLabProcess.getDisambiguatingDescriptionsAsString(p, ?context = context)
            |> ResizeArray.map (fun c ->
                let c = Comment.fromString c
                CompositeHeader.Comment (Option.defaultValue "" c.Name),
                CompositeCell.FreeText (Option.defaultValue "" c.Value)
            )

        let inputs = LDLabProcess.getObjects(p, ?graph = graph, ?context = context)
        let outputs = LDLabProcess.getResults(p, ?graph = graph, ?context = context)

        let inputs,outputs =
            if inputs.Count = 0 && outputs.Count <> 0 then
                ResizeArray.create outputs.Count None,
                ResizeArray.map Option.Some outputs
            elif inputs.Count <> 0 && outputs.Count = 0 then
                 ResizeArray.map Option.Some inputs,
                 ResizeArray.create inputs.Count None
            else               
                ResizeArray.map Option.Some inputs,
                ResizeArray.map Option.Some outputs


        if inputs.Count = 0 && outputs.Count = 0 then
            let vals =
                [
                    yield! components
                    yield! pvs
                ]
                |> List.sortBy (snd >> Option.defaultValue 10000)
                |> List.map fst
            [
                yield! protVals
                yield! vals
                yield! comments
            ]
            |> ResizeArray.singleton
        else
        // zip the inputs and outpus so they are aligned as rows
            outputs
            |> ResizeArray.zip inputs
            // This grouping here and the picking of the "inputForCharas" etc is done, so there can be rows where data do have characteristics, which is not possible in isa json
            |> ResizeArray.map (fun (i,o) ->
                let chars =
                    match i with
                    | Some i -> 
                        LDSample.getCharacteristics(i, ?graph = graph, ?context = context)
                        |> ResizeArray.map (fun cv -> BaseTypes.decomposeCharacteristicValue(cv, ?context = context), ColumnIndex.tryGetIndex cv)
                    | None -> ResizeArray []            
                let factors =
                    match o with
                    | Some o -> 
                        LDSample.getFactors(o, ?graph = graph, ?context = context)
                        |> ResizeArray.map (fun fv -> BaseTypes.decomposeFactorValue(fv, ?context = context), ColumnIndex.tryGetIndex fv)
                    | None -> ResizeArray []


                let vals =
                    [
                        yield! chars
                        yield! components
                        yield! pvs
                        yield! factors
                    ]
                    |> List.sortBy (snd >> Option.defaultValue 10000)
                    |> List.map fst
                [
                    if i.IsSome then yield BaseTypes.decomposeProcessInput(i.Value, ?context = context)
                    yield! protVals
                    yield! vals
                    yield! comments
                    if o.IsSome then yield BaseTypes.decomposeProcessOutput(o.Value, ?context = context)
                ]
            )
