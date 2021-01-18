namespace ISADotNet

open Update

module Protocol =  
  
    /// If a protocol for which the predicate returns true exists in the study, gets it
    let tryGetBy (predicate : Protocol -> bool) (study:Study) =
        study.Protocols
        |> List.tryFind (predicate) 

    /// If a protocol with the given identfier exists in the study, returns it
    let tryGetByName (name : string) (study:Study) =
        tryGetBy (fun p -> p.Name = name) study

    /// Returns true, if a protocol for which the predicate returns true exists in the study
    let exists (predicate : Protocol -> bool) (study:Study) =
        study.Protocols
        |> List.exists (predicate) 

    /// Returns true, if the protocol exists in the study
    let contains (protocol : Protocol) (study:Study) =
        exists ((=) protocol) study

    /// If a protocol with the given name exists in the study exists, returns it
    let existsByName (name : string) (study:Study) =
        exists (fun p -> p.Name = name) study

    /// Adds the given protocol to the study  
    let add (protocol : Protocol) (study:Study) =
        {study with Protocols = List.append study.Protocols [protocol]}

    /// If a protocol exists in the study for which the predicate returns true, updates it with the given protocol
    let updateBy (predicate : Protocol -> bool) (updateOption:UpdateOptions) (protocol : Protocol) (study:Study) =
        if exists predicate study then
            {study 
                with Protocols = 
                     List.map (fun p -> if predicate p then updateOption.updateRecordType p protocol else p) study.Protocols}
        else 
            study

    /// If a protocol with the same name as the given protocol exists in the study exists, updates it with the given protocol
    let updateByName (updateOption:UpdateOptions) (protocol : Protocol) (study:Study) =
        updateBy (fun p -> p.Name = protocol.Name) updateOption protocol study

    /// If a protocol for which the predicate returns true exists in the study, removes it from the study
    let removeBy (predicate : Protocol -> bool) (study:Study) =
        if exists predicate study then
            {study with Protocols = List.filter (predicate >> not) study.Protocols}
        else 
            study

    /// If the given protocol exists in the study, removes it from the study
    let remove (protocol : Protocol) (study:Study) =
        removeBy ((=) protocol) study

    /// If a protocol with the given name exists in the study, removes it from the study
    let removeByName (name : string) (study : Study) = 
        removeBy (fun p -> p.Name = name) study