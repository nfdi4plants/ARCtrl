namespace ISADotNet.API

open ISADotNet
open Update

module Study =

    ///// If a study for which the predicate returns true exists in the investigation, gets it
    //let tryGetBy (predicate : Study -> bool) (investigation:Investigation) =
    //    investigation.Studies
    //    |> List.tryFind (predicate) 

    //let tryGet (study : Study) (investigation:Investigation) =
    //    tryGetBy ((=) study) investigation

    /// If an study with the given identfier exists in the list, returns it
    let tryGetByIdentifier (identifier : string) (studies : Study list) =
        List.tryFind (fun (s:Study) -> s.Identifier = Some identifier) studies

    ///// Returns true, if a study for which the predicate returns true exists in the investigation
    //let exists (predicate : Study -> bool) (investigation:Investigation) =
    //    investigation.Studies
    //    |> List.exists (predicate) 

    ///// Returns true, if the given study exists in the investigation
    //let contains (study : Study) (investigation:Investigation) =
    //    exists ((=) study) investigation

    /// If an study with the given identfier exists in the list, returns true
    let existsByIdentifier (identifier : string) (studies : Study list) =
        List.exists (fun (s:Study) -> s.Identifier = Some identifier) studies

    /// Adds the given study to the studies  
    let add (studies : Study list) (study : Study) =
        List.append studies [study]

    /// Updates all studies for which the predicate returns true with the given study values
    let updateBy (predicate : Study -> bool) (updateOption:UpdateOptions) (study : Study) (studies : Study list) =
        if List.exists predicate studies then
            List.map (fun a -> if predicate a then updateOption.updateRecordType a study else a) studies
        else 
            studies

    /// Updates all studies with the same identifier as the given study with its values
    let updateByIdentifier (updateOption:UpdateOptions) (study : Study) (studies : Study list) =
        updateBy (fun (s:Study) -> s.Identifier = study.Identifier) updateOption study studies

    /// If a study with the given identifier exists in the list, removes it
    let removeByIdentifier (identifier : string) (studies : Study list) = 
        List.filter (fun (s:Study) -> s.Identifier = Some identifier |> not) studies
    

    /// Returns assays of a study
    let getAssays (study : Study) =
        study.Assays

    /// Applies function f to the assays of a study
    let mapAssays (f : Assay list -> Assay list) (study : Study) =
        { study with 
            Assays = Option.mapDefault [] f study.Assays }

    /// Replaces study assays with the given assay list
    let setAssays (study : Study) (assays : Assay list) =
        { study with
            Assays = Some assays }
    
    /// Returns factors of a study
    let getFactors (study : Study) =
        study.Factors

    /// Applies function f to the factors of a study
    let mapFactors (f : Factor list -> Factor list) (study : Study) =
        { study with 
            Factors = Option.mapDefault [] f study.Factors }

    /// Replaces study factors with the given assay list
    let setFactors (study : Study) (factors : Factor list) =
        { study with
            Factors = Some factors }

    /// Returns protocols of a study
    let getProtocols (study : Study) =
        study.Protocols

    /// Applies function f to the protocols of a study
    let mapProtocols (f : Protocol list -> Protocol list) (study : Study) =
        { study with 
            Protocols = Option.mapDefault [] f study.Protocols }

    /// Replaces study protocols with the given assay list
    let setProtocols (study : Study) (protocols : Protocol list) =
        { study with
            Protocols = Some protocols }

    /// Returns all contacts of a study
    let getContacts (study : Study) =
        study.Contacts

    /// Applies function f to contacts of a study
    let mapContacts (f : Person list -> Person list) (study : Study) =
        { study with 
            Contacts = Option.mapDefault [] f study.Contacts }

    /// Replaces contacts of a study with the given person list
    let setContacts (study : Study) (persons : Person list) =
        { study with
            Contacts = Some persons }

    /// Returns publications of a study
    let getPublications (study : Study) =
        study.Publications

    /// Applies function f to publications of the study
    let mapPublications (f : Publication list -> Publication list) (study : Study) =
        { study with 
            Publications = Option.mapDefault [] f study.Publications }

    /// Replaces publications of a study with the given publication list
    let setPublications (study : Study) (publications : Publication list) =
        { study with
            Publications = Some publications }

    /// Returns study design descriptors of a study
    let getDescriptors (study : Study) =
        study.StudyDesignDescriptors

    /// Applies function f to to study design descriptors of a study
    let mapDescriptors (f : OntologyAnnotation list -> OntologyAnnotation list) (study : Study) =
        { study with 
            StudyDesignDescriptors = Option.mapDefault [] f study.StudyDesignDescriptors }

    /// Replaces study design descriptors with the given ontology annotation list
    let setDescriptors (study : Study) (descriptors : OntologyAnnotation list) =
        { study with
            StudyDesignDescriptors = Some descriptors }

