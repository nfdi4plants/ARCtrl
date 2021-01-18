namespace ISADotNet

open Update

module Assay =  
  
    /// If an assay for which the predicate returns true exists in the study, gets it
    let tryGetBy (predicate : Assay -> bool) (study:Study) =
        study.Assays
        |> List.tryFind (predicate) 

    /// If an assay with the given identfier exists in the study, returns it
    let tryGetByFileName (fileName : string) (study:Study) =
        tryGetBy (fun a -> a.FileName = fileName) study

    /// Returns true, if an assay for which the predicate returns true exists in the study
    let exists (predicate : Assay -> bool) (study:Study) =
        study.Assays
        |> List.exists (predicate) 

    /// Returns true, if the assay exists in the study
    let contains (assay : Assay) (study:Study) =
        exists ((=) assay) study

    /// If an assay with the given identfier exists in the study, returns it
    let existsByFileName (fileName : string) (study:Study) =
        exists (fun a -> a.FileName = fileName) study

    /// Adds the given assay to the study  
    let add (assay : Assay) (study:Study) =
        {study with Assays = List.append study.Assays [assay]}

    /// If an assay exists in the study for which the predicate returns true, updates it with the given assay values
    let updateBy (predicate : Assay -> bool) (updateOption:UpdateOptions) (assay : Assay) (study:Study) =
        if exists predicate study then
            {study 
                with Assays = 
                     study.Assays
                     |> List.map (fun a -> if predicate a then updateOption.updateRecordType a assay else a) 
            }
        else 
            study

    /// If an assay with the same fileName as the given assay exists in the study exists, updates it with the given assay values
    let updateByFileName (updateOption:UpdateOptions) (assay : Assay) (study:Study) =
        updateBy (fun a -> a.FileName = assay.FileName) updateOption assay study

    /// If an assay for which the predicate returns true exists in the study, removes it from the study
    let removeBy (predicate : Assay -> bool) (study:Study) =
        if exists predicate study then
            {study with Assays = List.filter (predicate >> not) study.Assays}
        else 
            study

    /// If the given assay exists in the study, removes it from the study
    let remove (assay : Assay) (study:Study) =
        removeBy ((=) assay) study

    /// If an assay with the given fileName exists in the study, removes it from the study
    let removeByFileName (fileName : string) (study : Study) = 
        removeBy (fun a -> a.FileName = fileName) study

    /// TODO Filename = identfier ausformulieren
    /// If no assay with the same identifier as the given assay exists in the study, adds the given assay to the study
    let tryAdd (assay : Assay) (study:Study) =
        if study.Assays |> List.exists (fun studyAssay -> studyAssay.FileName = assay.FileName) then
            None
        else 
            Some {study with Assays = List.append study.Assays [assay]}

    /// If an assay with the given identfier exists in the study exists, removes it from the study
    let tryRemove (assayIdentfier : string) (study:Study) =
        if study.Assays |> List.exists (fun assay -> assay.FileName = assayIdentfier) then
            Some {study with Assays = List.filter (fun assay -> assay.FileName <> assayIdentfier) study.Assays }
        else 
            None

    /// If an assay with the same identifier as the given assay exists in the study, overwrites its values with values in the given study
    let tryUpdate (assay : Assay) (study:Study) =
        if study.Assays |> List.exists (fun studyAssay -> studyAssay.FileName = assay.FileName) then
            Some {study with Assays = 
                                study.Assays 
                                |> List.map (fun studyAssay -> if studyAssay.FileName = assay.FileName then assay else studyAssay) 
            }
        else 
            None