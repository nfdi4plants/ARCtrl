namespace ISADotNet

open Update

module Ontology =

    /// If a ontology source reference for which the predicate returns true exists in the investigation, gets it
    let tryGetBy (predicate : OntologySourceReference -> bool) (investigation:Investigation) =
        investigation.OntologySourceReferences
        |> List.tryFind (predicate) 

    /// If an ontology source reference with the given name exists in the investigation, returns it
    let tryGetByName (name : string) (investigation:Investigation) =
        tryGetBy (fun t -> t.Name = name)  investigation

    /// Returns true, if a ontology source reference for which the predicate returns true exists in the investigation
    let exists (predicate : OntologySourceReference -> bool) (investigation:Investigation) =
        investigation.OntologySourceReferences
        |> List.exists (predicate) 

    /// If an ontology source reference with the given name exists in the investigation , returns it
    let existsByName (name : string) (investigation:Investigation) =
        exists (fun t -> t.Name = name) investigation

    /// Returns true, if the investigation contains the given ontology source reference
    let contains (ontologySourceReference : OntologySourceReference) (investigation:Investigation) =
        exists ((=) ontologySourceReference) investigation

    /// Adds the given ontology source reference to the investigation  
    let add (ontologySourceReference : OntologySourceReference) (investigation:Investigation) =
        {investigation with OntologySourceReferences = List.append investigation.OntologySourceReferences [ontologySourceReference]}

    /// If an ontology source reference exists in the investigation for which the predicate returns true, updates it with the given ontology source reference values, 
    let updateBy (predicate : OntologySourceReference -> bool) (updateOption:UpdateOptions) (ontologySourceReference : OntologySourceReference) (investigation:Investigation) =
        if exists predicate investigation then
            {investigation 
                with OntologySourceReferences = 
                     investigation.OntologySourceReferences
                     |> List.map (fun t -> if predicate t then updateOption.updateRecordType t ontologySourceReference else t) 
            }
        else 
            investigation

    /// If an ontology source reference with the same name as the given name exists in the investigation, updates it with the given ontology source reference
    let updateByName (updateOption:UpdateOptions) (ontologySourceReference : OntologySourceReference) (investigation:Investigation) =
        updateBy (fun t -> t.Name = ontologySourceReference.Name) updateOption ontologySourceReference investigation

    /// If a ontology source reference for which the predicate returns true exists in the investigation, removes it from the investigation
    let removeBy (predicate : OntologySourceReference -> bool) (investigation:Investigation) =
        if exists predicate investigation then
            {investigation with OntologySourceReferences = List.filter (predicate >> not) investigation.OntologySourceReferences}
        else 
            investigation

    /// If the given ontology source reference exists in the investigation, removes it from the investigation
    let remove (ontologySourceReference : OntologySourceReference) (investigation:Investigation) =
        removeBy ((=) ontologySourceReference) investigation

    /// If a ontology source reference with the given name exists in the investigation, removes it from the investigation
    let removeByName (name : string) (investigation : Investigation) = 
        removeBy (fun t -> t.Name = name)  investigation