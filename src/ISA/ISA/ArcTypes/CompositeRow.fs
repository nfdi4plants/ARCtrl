module ISA.CompositeRow

let toProtocol (row : (CompositeHeader*CompositeCell) seq) =
    row
    |> Seq.fold (fun p hc ->
        match hc with
        | CompositeHeader.ProtocolType, CompositeCell.Term oa -> 
            Protocol.setProtocolType p oa
        | CompositeHeader.ProtocolVersion, CompositeCell.FreeText v -> Protocol.setVersion p v
        | CompositeHeader.ProtocolUri, CompositeCell.FreeText v -> Protocol.setUri p v
        | CompositeHeader.ProtocolDescription, CompositeCell.FreeText v -> Protocol.setDescription p v
        | CompositeHeader.ProtocolREF, CompositeCell.FreeText v -> Protocol.setName p v
        | CompositeHeader.Parameter oa, _ -> 
            let pp = ProtocolParameter.create(ParameterName = oa)
            Protocol.addParameter (pp) p
        | CompositeHeader.Component oa, CompositeCell.Unitized(v,unit) -> 
            let c = Component.create(ComponentType = oa, Value = Value.fromString v, Unit = unit)
            Protocol.addComponent c p        
        | CompositeHeader.Component oa, CompositeCell.Term t -> 
            let c = Component.create(ComponentType = oa, Value = Value.Ontology t)
            Protocol.addComponent c p     
        | _ -> p
    ) Protocol.empty

let toProcess name (row : (CompositeHeader*CompositeCell) seq) =
    let rec loop (protocol : Protocol) (chars : MaterialAttributeValue list) (paras : ProcessParameterValue list) (facts : FactorValue list) (input : ProcessInput) (output : ProcessOutput) rows =
        match rows with
        | [] ->
            let protocol = {protocol with Parameters = paras |> List.map (ProcessParameterValue.getCategory >> Option.get) |> Aux.Option.fromValueWithDefault []}
            let input = inpu
            let output = 
            let p = Process.create(Name = name, ExecutesProtocol = protocol,Inputs = [input],Outputs )
            p
    row
    |> Seq.fold (fun p hc ->
        match hc with
        | CompositeHeader.ProtocolType, CompositeCell.Term oa -> 
            Protocol.setProtocolType p oa
        | CompositeHeader.ProtocolVersion, CompositeCell.FreeText v -> Protocol.setVersion p v
        | CompositeHeader.ProtocolUri, CompositeCell.FreeText v -> Protocol.setUri p v
        | CompositeHeader.ProtocolDescription, CompositeCell.FreeText v -> Protocol.setDescription p v
        | CompositeHeader.ProtocolREF, CompositeCell.FreeText v -> Protocol.setName p v
        | CompositeHeader.Parameter oa, _ -> 
            let pp = ProtocolParameter.create(ParameterName = oa)
            Protocol.addParameter (pp) p
        | CompositeHeader.Component oa, CompositeCell.Unitized(v,unit) -> 
            let c = Component.create(ComponentType = oa, Value = Value.fromString v, Unit = unit)
            Protocol.addComponent c p        
        | CompositeHeader.Component oa, CompositeCell.Term t -> 
            let c = Component.create(ComponentType = oa, Value = Value.Ontology t)
            Protocol.addComponent c p     
        | _ -> p
    ) Protocol.empty