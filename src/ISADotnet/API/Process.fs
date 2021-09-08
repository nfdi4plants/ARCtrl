namespace ISADotNet.API

open ISADotNet

module ProcessParameterValue =

    /// Returns the name of the paramater value as string if it exists
    let tryGetNameAsString (pv : ProcessParameterValue) =
        pv.Category
        |> Option.bind (ProtocolParameter.tryGetNameAsString)

    /// Returns the name of the paramater value as string
    let getNameAsString (pv : ProcessParameterValue) =
        tryGetNameAsString pv
        |> Option.defaultValue ""

    /// Returns true if the given name matches the name of the parameter value
    let nameEqualsString (name : string) (pv : ProcessParameterValue) =
        match pv.Category with
        | Some oa -> ProtocolParameter.nameEqualsString name oa
        | None -> false

    /// Returns the value of the parameter value as string if it exists (with unit)
    let tryGetValueAsString (pv : ProcessParameterValue) =
        let unit = pv.Unit |> Option.bind (OntologyAnnotation.tryGetNameAsString)
        pv.Value
        |> Option.map (fun v ->
            let s = v |> Value.toString
            match unit with
            | Some u -> s + " " + u
            | None -> s
        )

    /// Returns the value of the parameter value as string (with unit)
    let getValueAsString (pv : ProcessParameterValue) =
        tryGetValueAsString pv
        |> Option.defaultValue ""

/// Functions for handling the ProcessInput Type
module ProcessInput =

    /// Returns name of processInput
    let getName (pi : ProcessInput) =
        match pi with
        | ProcessInput.Sample s     -> s.Name
        | ProcessInput.Source s     -> s.Name
        | ProcessInput.Material m   -> m.Name
        | ProcessInput.Data d       -> d.Name

    /// Returns true, if given name equals name of processInput
    let nameEquals (name : string) (pi : ProcessInput) =
        match pi with
        | ProcessInput.Sample s     -> s.Name = (Some name)
        | ProcessInput.Source s     -> s.Name = (Some name)
        | ProcessInput.Material m   -> m.Name = (Some name)
        | ProcessInput.Data d       -> d.Name = (Some name)

    /// Returns true, if Process Input is Sample
    let isSample (pi : ProcessInput) =
        match pi with
        | ProcessInput.Sample _ -> true
        | _ -> false

    /// Returns true, if Process Input is Source
    let isSource (pi : ProcessInput) =
        match pi with
        | ProcessInput.Source _ -> true
        | _ -> false

    /// Returns true, if Process Input is Data
    let isData (pi : ProcessInput) =
        match pi with
        | ProcessInput.Data _ -> true
        | _ -> false

    /// Returns true, if Process Input is Material
    let isMaterial (pi : ProcessInput) =
        match pi with
        | ProcessInput.Material _ -> true
        | _ -> false

    /// If given process input is a sample, returns it, else returns None
    let trySample (pi : ProcessInput) =
        match pi with
        | ProcessInput.Sample s -> Some s
        | _ -> None

    /// If given process input is a source, returns it, else returns None
    let trySource (pi : ProcessInput) =
        match pi with
        | ProcessInput.Source s -> Some s
        | _ -> None

    /// If given process input is a data, returns it, else returns None
    let tryData (pi : ProcessInput) =
        match pi with
        | ProcessInput.Data d -> Some d
        | _ -> None

    /// If given process input is a material, returns it, else returns None
    let tryMaterial (pi : ProcessInput) =
        match pi with
        | ProcessInput.Material m -> Some m
        | _ -> None

    /// If given process input contains characteristics, returns them
    let tryGetCharacteristics (pi : ProcessInput) =
        match pi with
        | ProcessInput.Sample s     -> s.Characteristics
        | ProcessInput.Source s     -> s.Characteristics
        | ProcessInput.Material m   -> m.Characteristics
        | ProcessInput.Data _       -> None

/// Functions for handling the ProcessOutput Type
module ProcessOutput =

    /// Returns name of processOutput
    let getName (po : ProcessOutput) =
        match po with
        | ProcessOutput.Sample s     -> s.Name
        | ProcessOutput.Material m   -> m.Name
        | ProcessOutput.Data d       -> d.Name

    /// Returns true, if given name equals name of processOutput
    let nameEquals (name : string) (po : ProcessOutput) =
        match po with
        | ProcessOutput.Sample s     -> s.Name = (Some name)
        | ProcessOutput.Material m   -> m.Name = (Some name)
        | ProcessOutput.Data d       -> d.Name = (Some name)

    /// Returns true, if Process Output is Sample
    let isSample (po : ProcessOutput) =
        match po with
        | ProcessOutput.Sample _ -> true
        | _ -> false

    /// Returns true, if Process Output is Data
    let isData (po : ProcessOutput) =
        match po with
        | ProcessOutput.Data _ -> true
        | _ -> false

    /// Returns true, if Process Output is Material
    let isMaterial (po : ProcessOutput) =
        match po with
        | ProcessOutput.Material _ -> true
        | _ -> false

    /// If given process output is a sample, returns it, else returns None
    let trySample (po : ProcessOutput) =
        match po with
        | ProcessOutput.Sample s -> Some s
        | _ -> None

    /// If given process output is a data, returns it, else returns None
    let tryData (po : ProcessOutput) =
        match po with
        | ProcessOutput.Data d -> Some d
        | _ -> None

    /// If given process output is a material, returns it, else returns None
    let tryMaterial (po : ProcessOutput) =
        match po with
        | ProcessOutput.Material m -> Some m
        | _ -> None


    /// If given process output contains characteristics, returns them
    let tryGetCharacteristics (po : ProcessOutput) =
        match po with
        | ProcessOutput.Sample s     -> s.Characteristics
        | ProcessOutput.Material m   -> m.Characteristics
        | ProcessOutput.Data _       -> None

    /// If given process output contains factors, returns them
    let tryGetFactorValues (po : ProcessOutput) =
        match po with
        | ProcessOutput.Sample s     -> s.FactorValues
        | ProcessOutput.Material _   -> None
        | ProcessOutput.Data _       -> None

/// Functions for handling ISA Processes
module Process =

    /// Returns the name of the protocol the given process executes
    let tryGetProtocolName (p: Process) =
        p.ExecutesProtocol
        |> Option.bind (fun p -> p.Name)

    /// Returns the name of the protocol the given process executes
    let getProtocolName (p: Process) =
        p.ExecutesProtocol
        |> Option.bind (fun p -> p.Name)
        |> Option.get 

    /// Returns the parameters describing the process
    let getParameters (p: Process) =
        match p.ParameterValues with
        | Some paramValues ->
            paramValues
            |> List.choose (fun pv -> pv.Category)
        | None -> []

    /// If the process implements the given parameter, return the list of input files together with their according parameter values of this parameter
    let tryGetInputsWithParameterBy (predicate : ProtocolParameter -> bool) (p : Process) =
        match p.ParameterValues with
        | Some paramValues ->
            match paramValues |> List.tryFind (fun pv -> Option.defaultValue ProtocolParameter.empty pv.Category |> predicate ) with
            | Some paramValue ->
                p.Inputs
                |> Option.map (List.map (fun i -> i,paramValue))
            | None -> None
        | None -> None

    
    /// If the process implements the given parameter, return the list of output files together with their according parameter values of this parameter
    let tryGetOutputsWithParameterBy (predicate : ProtocolParameter -> bool) (p : Process) =
        match p.ParameterValues with
        | Some paramValues ->
            match paramValues |> List.tryFind (fun pv -> Option.defaultValue ProtocolParameter.empty pv.Category |> predicate ) with
            | Some paramValue ->
                p.Outputs
                |> Option.map (List.map (fun i -> i,paramValue))
            | None -> None
        | None -> None

    /// Returns the characteristics of the samples of the process
    let getCharacteristics (p : Process) =
        let materialAttributesOfValues (mvs : (MaterialAttributeValue list) Option) = 
            mvs |> Option.defaultValue [] |> List.choose (fun mv -> mv.Category)
        p.Inputs |> Option.defaultValue [] |> List.collect (ProcessInput.tryGetCharacteristics >> materialAttributesOfValues)
        |> List.append (p.Outputs |> Option.defaultValue [] |> List.collect (ProcessOutput.tryGetCharacteristics >> materialAttributesOfValues))
        |> List.distinct

    /// If the process implements the given characteristic, return the list of input files together with their according characteristic values of this characteristic
    let tryGetInputsWithCharacteristicBy (predicate : MaterialAttribute -> bool) (p : Process) =
        match p.Inputs with
        | Some is ->
            is
            |> List.choose (fun i ->
                ProcessInput.tryGetCharacteristics i
                |> Option.defaultValue []
                |> List.tryPick (fun mv -> 
                    match mv.Category with
                    | Some m when predicate m -> Some (i,mv)
                    | _ -> None

                )
            )
            |> Option.fromValueWithDefault []
        | None -> None

    /// If the process implements the given characteristic, return the list of output files together with their according characteristic values of this characteristic
    let tryGetOutputsWithCharacteristicBy (predicate : MaterialAttribute -> bool) (p : Process) =
        match p.Outputs with
        | Some os ->
            os
            |> List.choose (fun o ->
                ProcessOutput.tryGetCharacteristics o
                |> Option.defaultValue []
                |> List.tryPick (fun mv -> 
                    match mv.Category with
                    | Some m when predicate m -> Some (o,mv)
                    | _ -> None

                )
            )
            |> Option.fromValueWithDefault []
        | None -> None

    /// Returns the factors of the samples of the process
    let getFactors (p : Process) =
        let factorsOfValues (fvs : (FactorValue list) Option) = 
            fvs |> Option.defaultValue [] |> List.choose (fun fv -> fv.Category)
        p.Outputs |> Option.defaultValue [] |> List.collect (ProcessOutput.tryGetFactorValues >> factorsOfValues)
        |> List.distinct

    /// If the process implements the given factor, return the list of output files together with their according factor values of this factor
    let tryGetOutputsWithFactorBy (predicate : Factor -> bool) (p : Process) =
        match p.Outputs with
        | Some os ->
            os
            |> List.choose (fun o ->
                ProcessOutput.tryGetFactorValues o
                |> Option.defaultValue []
                |> List.tryPick (fun mv -> 
                    match mv.Category with
                    | Some m when predicate m -> Some (o,mv)
                    | _ -> None

                )
            )
            |> Option.fromValueWithDefault []
        | None -> None

    //let tryGetCharacteristicValuesOfInputBy (predicate : ProcessInput -> bool) (p : Process) =
    //    match p.Inputs with
    //    | Some is -> 
    //        is
    //        |> List.choose
    //    | None -> None


module ProcessSequence = 

    /// Returns the names of the protocols the given processes impelement
    let getProtocolNames (processSequence : Process list) =
        processSequence
        |> List.choose (fun p ->
            p.ExecutesProtocol
            |> Option.bind (fun protocol ->
                protocol.Name
            )        
        )
        |> List.distinct

    /// Returns a list of the processes, containing only the ones executing a protocol for which the predicate returns true
    let filterByProtocolBy (predicate : Protocol -> bool) (processSequence : Process list) =
        processSequence
        |> List.filter (fun p ->
            match p.ExecutesProtocol with
            | Some protocol when predicate protocol -> true
            | _ -> false
        )

    /// Returns a list of the processes, containing only the ones executing a protocol with the given name
    let filterByProtocolName (protocolName : string) (processSequence : Process list) =
        filterByProtocolBy (fun (p:Protocol) -> p.Name = Some protocolName) processSequence

    /// If the processes contain a process implementing the given parameter, return the list of input files together with their according parameter values of this parameter
    let getInputsWithParameterBy (predicate:ProtocolParameter -> bool) (processSequence : Process list) =
        processSequence
        |> List.choose (Process.tryGetInputsWithParameterBy predicate)
        |> List.concat
        
    /// If the processes contain a process implementing the given parameter, return the list of output files together with their according parameter values of this parameter
    let getOutputsWithParameterBy (predicate:ProtocolParameter -> bool) (processSequence : Process list) =
        processSequence
        |> List.choose (Process.tryGetOutputsWithParameterBy predicate)
        |> List.concat

    /// Returns the parameters implemented by the processes contained in these processes
    let getParameters (processSequence : Process list) =
        processSequence
        |> List.collect Process.getParameters
        |> List.distinct
    
    /// If the processes contain a process implementing the given parameter, return the list of input files together with their according parameter values of this parameter
    let getInputsWithCharacteristicBy (predicate:MaterialAttribute -> bool) (processSequence : Process list) =
        processSequence
        |> List.choose (Process.tryGetInputsWithCharacteristicBy predicate)
        |> List.concat
        
    /// If the processes contain a process implementing the given parameter, return the list of output files together with their according parameter values of this parameter
    let getOutputsWithCharacteristicBy (predicate:MaterialAttribute -> bool) (processSequence : Process list) =
        processSequence
        |> List.choose (Process.tryGetOutputsWithCharacteristicBy predicate)
        |> List.concat

    /// If the processes contain a process implementing the given factor, return the list of output files together with their according factor values of this factor
    let getOutputsWithFactorBy (predicate:Factor -> bool) (processSequence : Process list) =
        processSequence
        |> List.choose (Process.tryGetOutputsWithFactorBy predicate)
        |> List.concat

    /// Returns the factors implemented by the processes contained in these processes
    let getFactors (processSequence : Process list) =
        processSequence
        |> List.collect Process.getFactors
        |> List.distinct
