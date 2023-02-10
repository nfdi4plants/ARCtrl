namespace ISADotNet.API

open ISADotNet
open Update

module StudyMaterials =  

    let getMaterials (am : StudyMaterials) =
        am.OtherMaterials |> Option.defaultValue []
        
    let getSamples (am : StudyMaterials) =
        am.Samples |> Option.defaultValue []

    let getSources (am : StudyMaterials) =
        am.Sources |> Option.defaultValue []

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
        study.Assays |> Option.defaultValue []

    /// Applies function f to the assays of a study
    let mapAssays (f : Assay list -> Assay list) (study : Study) =
        { study with 
            Assays = Option.mapDefault [] f study.Assays }

    /// Replaces study assays with the given assay list
    let setAssays (study : Study) (assays : Assay list) =
        { study with
            Assays = Some assays }
   

    /// Applies function f to the factors of a study
    let mapFactors (f : Factor list -> Factor list) (study : Study) =
        { study with 
            Factors = Option.mapDefault [] f study.Factors }

    /// Replaces study factors with the given assay list
    let setFactors (study : Study) (factors : Factor list) =
        { study with
            Factors = Some factors }




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
        study.Contacts |> Option.defaultValue []

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
        study.Publications |> Option.defaultValue []

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
        study.StudyDesignDescriptors |> Option.defaultValue []

    /// Applies function f to to study design descriptors of a study
    let mapDescriptors (f : OntologyAnnotation list -> OntologyAnnotation list) (study : Study) =
        { study with 
            StudyDesignDescriptors = Option.mapDefault [] f study.StudyDesignDescriptors }

    /// Replaces study design descriptors with the given ontology annotation list
    let setDescriptors (study : Study) (descriptors : OntologyAnnotation list) =
        { study with
            StudyDesignDescriptors = Some descriptors }

    /// Returns processSequence of study
    let getProcesses  (study : Study) =
        study.ProcessSequence |> Option.defaultValue []

    /// Returns protocols of a study
    let getProtocols (study : Study) =
        let processSequenceProtocols = 
            getProcesses study
            |> ProcessSequence.getProtocols
        let assaysProtocols = 
            getAssays study
            |> List.collect Assay.getProtocols            
        let studyProtocols = 
            study.Protocols
            |> Option.defaultValue []
        Update.mergeUpdateLists UpdateByExistingAppendLists (fun (p : Protocol) -> p.Name) assaysProtocols processSequenceProtocols
        |> Update.mergeUpdateLists UpdateByExistingAppendLists (fun (p : Protocol) -> p.Name) studyProtocols
    
    /// Returns Characteristics of the study
    let getCharacteristics (study : Study) =
        let processSequenceCharacteristics = 
            getProcesses study
            |> ProcessSequence.getCharacteristics
        let assaysCharacteristics = 
            getAssays study
            |> List.collect Assay.getCharacteristics            
        let studyCharacteristics = 
            study.CharacteristicCategories
            |> Option.defaultValue []
        processSequenceCharacteristics @ assaysCharacteristics @ studyCharacteristics 
        |> List.distinct
        //Update.mergeUpdateLists UpdateByExistingAppendLists (fun (f : MaterialAttribute) -> f.CharacteristicType |> Option.defaultValue OntologyAnnotation.empty) assaysCharacteristics processSequenceCharacteristics
        //|> Update.mergeUpdateLists UpdateByExistingAppendLists (fun (f : MaterialAttribute) -> f.CharacteristicType |> Option.defaultValue OntologyAnnotation.empty) studyCharacteristics

    /// Returns factors of the study
    let getFactors (study : Study) =
        let processSequenceFactors = 
            getProcesses study
            |> ProcessSequence.getFactors
        let assaysFactors = 
            getAssays study
            |> List.collect Assay.getFactors            
        let studyFactors = 
            study.Factors
            |> Option.defaultValue []
        processSequenceFactors @ assaysFactors @ studyFactors 
        |> List.distinct
        //Update.mergeUpdateLists UpdateByExistingAppendLists (fun (f : Factor) -> f.FactorType |> Option.defaultValue OntologyAnnotation.empty) assaysFactors processSequenceFactors
        //|> Update.mergeUpdateLists UpdateByExistingAppendLists (fun (f : Factor) -> f.FactorType |> Option.defaultValue OntologyAnnotation.empty) studyFactors

    /// Returns unit categories of the study
    let getUnitCategories (study : Study) =
        let processSequenceUnits = 
            getProcesses study
            |> ProcessSequence.getUnits
        let assaysUnits = 
            getAssays study
            |> List.collect Assay.getUnitCategories            
        let studyUnits = 
            study.UnitCategories
            |> Option.defaultValue []
        processSequenceUnits @ assaysUnits @ studyUnits 
        |> List.distinct
        //Update.mergeUpdateLists UpdateByExistingAppendLists (fun (d : OntologyAnnotation) -> d.Name) assaysUnits processSequenceUnits
        //|> Update.mergeUpdateLists UpdateByExistingAppendLists (fun (d : OntologyAnnotation) -> d.Name) studyUnits

    /// Returns sources of the study
    let getSources (study : Study) =
        let processSequenceSources = 
            getProcesses study
            |> ProcessSequence.getSources
        let assaysSources = 
            getAssays study
            |> List.collect Assay.getSources   
        let studySources = 
            match study.Materials with
            | Some mat -> mat.Sources |> Option.defaultValue []
            | None -> []
        Update.mergeUpdateLists UpdateByExistingAppendLists (fun (s : Source) -> s.Name) assaysSources processSequenceSources
        |> Update.mergeUpdateLists UpdateByExistingAppendLists (fun (s : Source) -> s.Name) studySources

    /// Returns sources of the study
    let getSamples (study : Study) =
        let processSequenceSamples = 
            getProcesses study
            |> ProcessSequence.getSamples
        let assaysSamples = 
            getAssays study
            |> List.collect Assay.getSamples   
        let studySamples = 
            match study.Materials with
            | Some mat -> mat.Samples |> Option.defaultValue []
            | None -> []
        Update.mergeUpdateLists UpdateByExistingAppendLists (fun (s : Sample) -> s.Name) assaysSamples processSequenceSamples
        |> Update.mergeUpdateLists UpdateByExistingAppendLists (fun (s : Sample) -> s.Name) studySamples

    /// Returns materials of the study
    let getMaterials (study : Study) =
        let processSequenceMaterials = 
            getProcesses study
            |> ProcessSequence.getMaterials
        let assaysMaterials = 
            getAssays study
            |> List.collect (Assay.getMaterials >> AssayMaterials.getMaterials)           
        let studyMaterials = 
            match study.Materials with
            | Some mat -> mat.OtherMaterials |> Option.defaultValue []
            | None -> []
        let materials = 
            Update.mergeUpdateLists UpdateByExistingAppendLists (fun (s : Material) -> s.Name) assaysMaterials processSequenceMaterials
            |> Update.mergeUpdateLists UpdateByExistingAppendLists (fun (s : Material) -> s.Name) studyMaterials
        let sources = getSources study
        let samples = getSamples study
        StudyMaterials.make (Option.fromValueWithDefault [] sources)
                            (Option.fromValueWithDefault [] samples)
                            (Option.fromValueWithDefault [] materials)

    let update (study : Study) =
        try
            {study with 
                        Materials  = getMaterials study |> Option.fromValueWithDefault StudyMaterials.empty
                        Assays = study.Assays |> Option.map (List.map Assay.update)
                        Protocols = getProtocols study |> Option.fromValueWithDefault []
                        Factors = getFactors study |> Option.fromValueWithDefault []
                        CharacteristicCategories = getCharacteristics study |> Option.fromValueWithDefault []
                        UnitCategories = getUnitCategories study |> Option.fromValueWithDefault []
            }
        with
        | err -> failwithf $"Could not update study {study.Identifier}: \n{err.Message}"