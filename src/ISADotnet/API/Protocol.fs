namespace ISADotNet

open Update

module Protocol =  
  
    ///// If a protocol for which the predicate returns true exists in the study, gets it
    //let tryGetBy (predicate : Protocol -> bool) (study:Study) =
    //    study.Protocols
    //    |> List.tryFind (predicate) 

    /// If a protocol with the given identfier exists in the list, returns it
    let tryGetByName (name : string) (protocols : Protocol list) =
        List.tryFind (fun (p:Protocol) -> p.Name = name) protocols

    ///// Returns true, if a protocol for which the predicate returns true exists in the study
    //let exists (predicate : Protocol -> bool) (study:Study) =
    //    study.Protocols
    //    |> List.exists (predicate) 

    ///// Returns true, if the protocol exists in the study
    //let contains (protocol : Protocol) (study:Study) =
    //    exists ((=) protocol) study

    /// If a protocol with the given name exists in the list exists, returns true
    let existsByName (name : string) (protocols : Protocol list) =
        List.exists (fun (p:Protocol) -> p.Name = name) protocols

    ///// Adds the given protocol to the study  
    //let add (protocol : Protocol) (study:Study) =
    //    {study with Protocols = List.append study.Protocols [protocol]}

    /// Updates all protocols for which the predicate returns true with the given protocol values
    let updateBy (predicate : Protocol -> bool) (updateOption : UpdateOptions) (protocol : Protocol) (protocols : Protocol list) =
        if List.exists predicate protocols then
            List.map (fun p -> if predicate p then updateOption.updateRecordType p protocol else p) protocols
        else 
            protocols

    /// Updates all protocols with the same name as the given protocol with its values
    let updateByName (updateOption:UpdateOptions) (protocol : Protocol) (protocols : Protocol list) =
        updateBy (fun p -> p.Name = protocol.Name) updateOption protocol protocols

    ///// If a protocol for which the predicate returns true exists in the study, removes it from the study
    //let removeBy (predicate : Protocol -> bool) (study:Study) =
    //    if exists predicate study then
    //        {study with Protocols = List.filter (predicate >> not) study.Protocols}
    //    else 
    //        study

    ///// If the given protocol exists in the study, removes it from the study
    //let remove (protocol : Protocol) (study:Study) =
    //    removeBy ((=) protocol) study

    /// If a protocol with the given name exists in the list, removes it
    let removeByName (name : string) (protocols : Protocol list) = 
        List.filter (fun (p:Protocol) -> p.Name = name) protocols

    // Comments

    /// Returns comments of a protocol
    let getComments (protocol : Protocol) =
        protocol.Comments

    /// Applies function f on comments of a protocol
    let mapComments (f : Comment list -> Comment list) (protocol : Protocol) =
        { protocol with 
            Comments = f protocol.Comments}

    /// Replaces comments of a protocol by given comment list
    let setComments (protocol : Protocol) (comments : Comment list) =
        { protocol with
            Comments = comments }

    // Protocol Type

    /// Returns protocol type of a protocol
    let getProtocolType (protocol : Protocol) =
        protocol.ProtocolType

    /// Applies function f on protocol type of a protocol
    let mapProtocolType (f : OntologyAnnotation -> OntologyAnnotation) (protocol : Protocol) =
        { protocol with 
            ProtocolType = f protocol.ProtocolType}

    /// Replaces protocol type of a protocol by given protocol type
    let setProtocolType (protocol : Protocol) (protocolType : OntologyAnnotation) =
        { protocol with
            ProtocolType = protocolType }

    // Components

    /// Returns components of a protocol
    let getComponents (protocol : Protocol) =
        protocol.Components

    /// Applies function f on components of a protocol
    let mapComponents (f : Component list -> Component list) (protocol : Protocol) =
        { protocol with 
            Components = f protocol.Components}

    /// Replaces components of a protocol by given component list
    let setComponents (protocol : Protocol) (components : Component list) =
        { protocol with
            Components = components }

    // Protocol Parameters
    
    /// Returns protocol parameters of a protocol
    let getParameters (protocol : Protocol) =
        protocol.Parameters
    
    /// Applies function f on protocol parameters of a protocol
    let mapParameters (f : ProtocolParameter list -> ProtocolParameter list) (protocol : Protocol) =
        { protocol with 
            Parameters = f protocol.Parameters}
    
    /// Replaces protocol parameters of a protocol by given protocol parameter list
    let setParameters (protocol : Protocol) (parameters : ProtocolParameter list) =
        { protocol with
            Parameters = parameters }