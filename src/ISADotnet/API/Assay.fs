namespace ISADotNet

open Update

module Assay =  
  
    ///// If an assay for which the predicate returns true exists in the study, gets it
    //let tryGetBy (predicate : Assay -> bool) (study:Study) =
    //    study.Assays
    //    |> List.tryFind (predicate) 

    /// If an assay with the given identfier exists in the list, returns it
    let tryGetByFileName (fileName : string) (assays : Assay list) =
        List.tryFind (fun (a:Assay) -> a.FileName = fileName) assays

    ///// Returns true, if an assay for which the predicate returns true exists in the study
    //let exists (predicate : Assay -> bool) (study:Study) =
    //    study.Assays
    //    |> List.exists (predicate) 

    ///// Returns true, if the assay exists in the study
    //let contains (assay : Assay) (study:Study) =
    //    exists ((=) assay) study

    /// If an assay with the given identfier exists in the list, returns true
    let existsByFileName (fileName : string) (assays : Assay list) =
        List.exists (fun (a:Assay) -> a.FileName = fileName) assays

    ///// Adds the given assay to the study  
    //let add (assay : Assay) (study:Study) =
    //    {study with Assays = List.append study.Assays [assay]}

    /// Updates all assays for which the predicate returns true with the given assays values
    let updateBy (predicate : Assay -> bool) (updateOption : UpdateOptions) (assay : Assay) (assays : Assay list) =
        if List.exists predicate assays then
            assays
            |> List.map (fun a -> if predicate a then updateOption.updateRecordType a assay else a) 
        else 
            assays

    /// If an assay with the same fileName as the given assay exists in the study exists, updates it with the given assay values
    let updateByFileName (updateOption:UpdateOptions) (assay : Assay) (assays : Assay list) =
        updateBy (fun a -> a.FileName = assay.FileName) updateOption assay assays

    ///// If an assay for which the predicate returns true exists in the study, removes it from the study
    //let removeBy (predicate : Assay -> bool) (study:Study) =
    //    if exists predicate study then
    //        {study with Assays = List.filter (predicate >> not) study.Assays}
    //    else 
    //        study

    ///// If the given assay exists in the study, removes it from the study
    //let remove (assay : Assay) (study:Study) =
    //    removeBy ((=) assay) study

    /// Updates all assays with the same name as the given assay with its values
    let removeByFileName (fileName : string) (assays : Assay list) = 
        List.filter (fun (a:Assay) -> a.FileName = fileName) assays

    /// TODO Filename = identfier ausformulieren
    /// If no assay with the same identifier as the given assay exists in the list, adds the given assay to the list
    let tryAdd (assay : Assay) (assays : Assay list) =
        if assays |> List.exists (fun studyAssay -> studyAssay.FileName = assay.FileName) then
            None
        else 
            Some (List.append assays [assay])

    /// If an assay with the given identfier exists in the list, removes it
    let tryRemove (assayIdentfier : string) (assays : Assay list) =
        if assays |> List.exists (fun assay -> assay.FileName = assayIdentfier) then
            Some (List.filter (fun (assay:Assay) -> assay.FileName <> assayIdentfier) assays)
        else 
            None

    /// If an assay with the same identifier as the given assay exists in the list, overwrites its values with values in the given assay
    let tryUpdate (assay : Assay) (assays : Assay list) =
        if assays |> List.exists (fun studyAssay -> studyAssay.FileName = assay.FileName) then
            Some (
                assays 
                |> List.map (fun studyAssay -> if studyAssay.FileName = assay.FileName then assay else studyAssay) 
            )
        else 
            None

    // Comment

    /// Returns comments of an assay
    let getComments (assay : Assay) =
        assay.Comments
    
    /// Applies function f on comments of an assay
    let mapComments (f : Comment list -> Comment list) (assay : Assay) =
        { assay with 
            Comments = f assay.Comments}
    
    /// Replaces comments of an assay by given comment list
    let setComments (assay : Assay) (comments : Comment list) =
        { assay with
            Comments = comments }

    // Data

    /// Returns data files of an assay
    let getData (assay : Assay) =
        assay.DataFiles
        
    /// Applies function f on data files of an assay
    let mapData (f : Data list -> Data list) (assay : Assay) =
        { assay with 
            DataFiles = f assay.DataFiles}
        
    /// Replaces data files of an assay by given data file list
    let setData (assay : Assay) (dataFiles : Data list) =
        { assay with
            DataFiles = dataFiles }

    // Unit Categories
    
    /// Returns unit categories of an assay
    let getUnitCategories (assay : Assay) =
        assay.UnitCategories
            
    /// Applies function f on unit categories of an assay
    let mapUnitCategories (f : OntologyAnnotation list -> OntologyAnnotation list) (assay : Assay) =
        { assay with 
            UnitCategories = f assay.UnitCategories}
            
    /// Replaces unit categories of an assay by given unit categorie list
    let setUnitCategories (assay : Assay) (unitCategories : OntologyAnnotation list) =
        { assay with
            UnitCategories = unitCategories }

    // Characteristic Categories
    
    /// Returns characteristic categories of an assay
    let getCharacteristics (assay : Assay) =
        assay.CharacteristicCategories
            
    /// Applies function f on characteristic categories of an assay
    let mapCharacteristics (f : MaterialAttribute list -> MaterialAttribute list) (assay : Assay) =
        { assay with 
            CharacteristicCategories = f assay.CharacteristicCategories}
            
    /// Replaces characteristic categories of an assay by given characteristic categorie list
    let setCharacteristics (assay : Assay) (characteristics : MaterialAttribute list) =
        { assay with
            CharacteristicCategories = characteristics }

    // Measurement Type

    /// Returns measurement type of an assay
    let getMeasurementType (assay : Assay) =
        assay.MeasurementType
            
    /// Applies function f on measurement type of an assay
    let mapMeasurementType (f : OntologyAnnotation -> OntologyAnnotation) (assay : Assay) =
        { assay with 
            MeasurementType = f assay.MeasurementType}
            
    /// Replaces measurement type of an assay by given measurement type
    let setMeasurementType (assay : Assay) (measurementType : OntologyAnnotation) =
        { assay with
            MeasurementType = measurementType }

    // Technology Type
    
    /// Returns technology type of an assay
    let getTechnologyType (assay : Assay) =
        assay.TechnologyType
                
    /// Applies function f on technology type of an assay
    let mapTechnologyType (f : OntologyAnnotation -> OntologyAnnotation) (assay : Assay) =
        { assay with 
            TechnologyType = f assay.TechnologyType}
                
    /// Replaces technology type of an assay by given technology type
    let setTechnologyType (assay : Assay) (technologyType : OntologyAnnotation) =
        { assay with
            TechnologyType = technologyType }

    // Processes

    /// Returns processes of an assay
    let getProcesses (assay : Assay) =
        assay.ProcessSequence
                
    /// Applies function f on processes of an assay
    let mapProcesses (f : Process list -> Process list) (assay : Assay) =
        { assay with 
            ProcessSequence = f assay.ProcessSequence}
                
    /// Replaces processes of an assay by given processe list
    let setProcesses (assay : Assay) (processes : Process list) =
        { assay with
            ProcessSequence = processes }

    // Materials 

    /// Returns materials of an assay
    let getMaterials (assay : Assay) =
        assay.Materials
                    
    /// Applies function f on materials of an assay
    let mapMaterials (f : AssayMaterials -> AssayMaterials) (assay : Assay) =
        { assay with 
            Materials = f assay.Materials}

    /// Replaces materials of an assay by given assay materials
    let setMaterials (assay : Assay) (materials : AssayMaterials) =
        { assay with
            Materials = materials }