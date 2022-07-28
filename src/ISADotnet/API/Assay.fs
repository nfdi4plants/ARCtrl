namespace ISADotNet.API

open ISADotNet
open Update

module Assay =  
  
    ///// If an assay for which the predicate returns true exists in the study, gets it
    //let tryGetBy (predicate : Assay -> bool) (study:Study) =
    //    study.Assays
    //    |> List.tryFind (predicate) 

    /// If an assay with the given identfier exists in the list, returns it
    let tryGetByFileName (fileName : string) (assays : Assay list) =
        List.tryFind (fun (a:Assay) -> a.FileName = Some fileName) assays

    ///// Returns true, if an assay for which the predicate returns true exists in the study
    //let exists (predicate : Assay -> bool) (study:Study) =
    //    study.Assays
    //    |> List.exists (predicate) 

    ///// Returns true, if the assay exists in the study
    //let contains (assay : Assay) (study:Study) =
    //    exists ((=) assay) study

    /// If an assay with the given identfier exists in the list, returns true
    let existsByFileName (fileName : string) (assays : Assay list) =
        List.exists (fun (a:Assay) -> a.FileName = Some fileName) assays

    /// Adds the given assay to the assays  
    let add (assays : Assay list) (assay : Assay) =
        List.append assays [assay]

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

    /// Updates all assays with the same name as the given assay with its values
    let removeByFileName (fileName : string) (assays : Assay list) = 
        List.filter (fun (a:Assay) -> a.FileName = Some fileName |> not) assays

    // Comment

    /// Returns comments of an assay
    let getComments (assay : Assay) =
        assay.Comments
    
    /// Applies function f on comments of an assay
    let mapComments (f : Comment list -> Comment list) (assay : Assay) =
        { assay with 
            Comments = Option.mapDefault [] f assay.Comments}
    
    /// Replaces comments of an assay by given comment list
    let setComments (assay : Assay) (comments : Comment list) =
        { assay with
            Comments = Some comments }

    // Data

    /// Returns data files of an assay
    let getData (assay : Assay) =
        assay.DataFiles
        
    /// Applies function f on data files of an assay
    let mapData (f : Data list -> Data list) (assay : Assay) =
        { assay with 
            DataFiles = Option.mapDefault [] f assay.DataFiles}
        
    /// Replaces data files of an assay by given data file list
    let setData (assay : Assay) (dataFiles : Data list) =
        { assay with
            DataFiles = Some dataFiles }

    // Unit Categories
    
    /// Returns unit categories of an assay
    let getUnitCategories (assay : Assay) =
        assay.UnitCategories
            
    /// Applies function f on unit categories of an assay
    let mapUnitCategories (f : OntologyAnnotation list -> OntologyAnnotation list) (assay : Assay) =
        { assay with 
            UnitCategories = Option.mapDefault [] f assay.UnitCategories}
            
    /// Replaces unit categories of an assay by given unit categorie list
    let setUnitCategories (assay : Assay) (unitCategories : OntologyAnnotation list) =
        { assay with
            UnitCategories = Some unitCategories }

    // Characteristic Categories
    
    /// Returns characteristic categories of an assay
    let getCharacteristics (assay : Assay) =
        assay.CharacteristicCategories
            
    /// Applies function f on characteristic categories of an assay
    let mapCharacteristics (f : MaterialAttribute list -> MaterialAttribute list) (assay : Assay) =
        { assay with 
            CharacteristicCategories = Option.mapDefault [] f assay.CharacteristicCategories}
            
    /// Replaces characteristic categories of an assay by given characteristic categorie list
    let setCharacteristics (assay : Assay) (characteristics : MaterialAttribute list) =
        { assay with
            CharacteristicCategories = Some characteristics }

    // Measurement Type

    /// Returns measurement type of an assay
    let getMeasurementType (assay : Assay) =
        assay.MeasurementType
            
    /// Applies function f on measurement type of an assay
    let mapMeasurementType (f : OntologyAnnotation -> OntologyAnnotation) (assay : Assay) =
        { assay with 
            MeasurementType = Option.map f assay.MeasurementType}
            
    /// Replaces measurement type of an assay by given measurement type
    let setMeasurementType (assay : Assay) (measurementType : OntologyAnnotation) =
        { assay with
            MeasurementType = Some measurementType }

    // Technology Type
    
    /// Returns technology type of an assay
    let getTechnologyType (assay : Assay) =
        assay.TechnologyType
                
    /// Applies function f on technology type of an assay
    let mapTechnologyType (f : OntologyAnnotation -> OntologyAnnotation) (assay : Assay) =
        { assay with 
            TechnologyType = Option.map f assay.TechnologyType}
                
    /// Replaces technology type of an assay by given technology type
    let setTechnologyType (assay : Assay) (technologyType : OntologyAnnotation) =
        { assay with
            TechnologyType = Some technologyType }

    // Processes

    /// Returns processes of an assay
    let getProcesses (assay : Assay) =
        assay.ProcessSequence
                
    /// Applies function f on processes of an assay
    let mapProcesses (f : Process list -> Process list) (assay : Assay) =
        { assay with 
            ProcessSequence = Option.mapDefault [] f assay.ProcessSequence}
                
    /// Replaces processes of an assay by given processe list
    let setProcesses (assay : Assay) (processes : Process list) =
        { assay with
            ProcessSequence = Some processes }

    // Materials 

    /// Returns materials of an assay
    let getMaterials (assay : Assay) =
        assay.Materials
                    
    /// Applies function f on materials of an assay
    let mapMaterials (f : AssayMaterials -> AssayMaterials) (assay : Assay) =
        { assay with 
            Materials = Option.map f assay.Materials}

    /// Replaces materials of an assay by given assay materials
    let setMaterials (assay : Assay) (materials : AssayMaterials) =
        { assay with
            Materials = Some materials }

    /// If the assay contains a process implementing the given parameter, return the list of input files together with their according parameter values of this parameter
    let getInputsWithParameterBy (predicate:ProtocolParameter -> bool) (assay : Assay) =
        assay.ProcessSequence
        |> Option.map (ProcessSequence.getInputsWithParameterBy predicate)
        
    /// If the assay contains a process implementing the given parameter, return the list of output files together with their according parameter values of this parameter
    let getOutputsWithParameterBy (predicate:ProtocolParameter -> bool) (assay : Assay) =
        assay.ProcessSequence
        |> Option.map (ProcessSequence.getOutputsWithParameterBy predicate)

    /// Returns the parameters implemented by the processes contained in this assay
    let getParameters (assay : Assay) =
        assay.ProcessSequence
        |> Option.map ProcessSequence.getParameters
    
    /// If the assay contains a process implementing the given parameter, return the list of input files together with their according parameter values of this parameter
    let getInputsWithCharacteristicBy (predicate:MaterialAttribute -> bool) (assay : Assay) =
        assay.ProcessSequence
        |> Option.map (ProcessSequence.getInputsWithCharacteristicBy predicate)
        
    /// If the assay contains a process implementing the given parameter, return the list of output files together with their according parameter values of this parameter
    let getOutputsWithCharacteristicBy (predicate:MaterialAttribute -> bool) (assay : Assay) =
        assay.ProcessSequence
        |> Option.map (ProcessSequence.getOutputsWithCharacteristicBy predicate)

    /// If the assay contains a process implementing the given factor, return the list of output files together with their according factor values of this factor
    let getOutputsWithFactorBy (predicate:Factor -> bool) (assay : Assay) =
        assay.ProcessSequence
        |> Option.map (ProcessSequence.getOutputsWithFactorBy predicate)

    /// Returns the factors implemented by the processes contained in this assay
    let getFactors (assay : Assay) =
        assay.ProcessSequence
        |> Option.map ProcessSequence.getFactors

    /// Returns the protocols implemented by the processes contained in this assay
    let getProtocols (assay : Assay) =
        assay.ProcessSequence
        |> Option.map ProcessSequence.getProtocols