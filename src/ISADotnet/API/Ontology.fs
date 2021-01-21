namespace ISADotNet.API

open ISADotNet
open Update

module OntologySourceReference =

    ///// If a ontology source reference for which the predicate returns true exists in the investigation, gets it
    //let tryGetBy (predicate : OntologySourceReference -> bool) (investigation:Investigation) =
    //    investigation.OntologySourceReferences
    //    |> List.tryFind (predicate) 

    /// If an ontology source reference with the given name exists in the list, returns it
    let tryGetByName (name : string) (ontologies : OntologySourceReference list) =
        List.tryFind (fun (t : OntologySourceReference) -> t.Name = name) ontologies

    ///// Returns true, if a ontology source reference for which the predicate returns true exists in the investigation
    //let exists (predicate : OntologySourceReference -> bool) (investigation:Investigation) =
    //    investigation.OntologySourceReferences
    //    |> List.exists (predicate) 

    /// If an ontology source reference with the given name exists in the list, returns true
    let existsByName (name : string) (ontologies : OntologySourceReference list) =
        List.exists (fun (t : OntologySourceReference) -> t.Name = name) ontologies

    ///// Returns true, if the investigation contains the given ontology source reference
    //let contains (ontologySourceReference : OntologySourceReference) (investigation:Investigation) =
    //    exists ((=) ontologySourceReference) investigation

    /// Adds the given ontology source reference to the investigation  
    let add (ontologySourceReference : OntologySourceReference) (investigation:Investigation) =
        {investigation with OntologySourceReferences = List.append investigation.OntologySourceReferences [ontologySourceReference]}

    /// Updates all ontology source references for which the predicate returns true with the given ontology source reference values
    let updateBy (predicate : OntologySourceReference -> bool) (updateOption : UpdateOptions) (ontologySourceReference : OntologySourceReference) (ontologies : OntologySourceReference list) =
        if List.exists predicate ontologies then
            ontologies
            |> List.map (fun t -> if predicate t then updateOption.updateRecordType t ontologySourceReference else t) 
        else 
            ontologies

    /// If an ontology source reference with the same name as the given name exists in the investigation, updates it with the given ontology source reference
    let updateByName (updateOption:UpdateOptions) (ontologySourceReference : OntologySourceReference) (ontologies:OntologySourceReference list) =
        updateBy (fun t -> t.Name = ontologySourceReference.Name) updateOption ontologySourceReference ontologies

    ///// If a ontology source reference for which the predicate returns true exists in the investigation, removes it from the investigation
    //let removeBy (predicate : OntologySourceReference -> bool) (investigation:Investigation) =
    //    if exists predicate investigation then
    //        {investigation with OntologySourceReferences = List.filter (predicate >> not) investigation.OntologySourceReferences}
    //    else 
    //        investigation

    ///// If the given ontology source reference exists in the investigation, removes it from the investigation
    //let remove (ontologySourceReference : OntologySourceReference) (investigation:Investigation) =
    //    removeBy ((=) ontologySourceReference) investigation

    /// If a ontology source reference with the given name exists in the list, removes it
    let removeByName (name : string) (ontologies : OntologySourceReference list) = 
        List.filter (fun (t : OntologySourceReference) -> t.Name = name) ontologies

    /// Returns comments of ontology source ref
    let getComments (ontology : OntologySourceReference) =
        ontology.Comments

    /// Applies function f on comments in ontology source ref
    let mapComments (f : Comment list -> Comment list) (ontology : OntologySourceReference) =
        { ontology with 
            Comments = f ontology.Comments}

    /// Replaces comments in ontology source ref by given comment list
    let setComments (ontology : OntologySourceReference) (comments : Comment list) =
        { ontology with
            Comments = comments }

module OntologyAnnotation =  

    ///// If a ontology annotation for which the predicate returns true exists in the Study.StudyDesignDescriptors, gets it
    //let tryGetBy (predicate : OntologyAnnotation -> bool) (study:Study) =
    //    study.StudyDesignDescriptors
    //    |> List.tryFind (predicate) 

    /// If an ontology annotation with the given annotation value exists in the list, returns it
    let tryGetByName (name : AnnotationValue) (annotations : OntologyAnnotation list) =
        List.tryFind (fun (d:OntologyAnnotation) -> d.Name = name) annotations

    ///// Returns true, if a ontology annotation for which the predicate returns true exists in the Study.StudyDesignDescriptors
    //let exists (predicate : OntologyAnnotation -> bool) (study:Study) =
    //    study.StudyDesignDescriptors
    //    |> List.exists (predicate) 

    ///// Returns true, if the given ontology annotation exists in the Study.StudyDesignDescriptors
    //let contains (design : OntologyAnnotation) (study:Study) =
    //    exists ((=) design) study

    /// If a ontology annotation with the given annotation value exists in the list, returns true
    let existsByName (name : AnnotationValue) (annotations : OntologyAnnotation list) =
        List.exists (fun (d:OntologyAnnotation) -> d.Name = name) annotations

    /// Adds the given ontology annotation to the Study.StudyDesignDescriptors
    let add (onotolgyAnnotations: OntologyAnnotation list) (onotolgyAnnotation : OntologyAnnotation) =
        List.append onotolgyAnnotations [onotolgyAnnotation]

    /// Updates all ontology annotations for which the predicate returns true with the given ontology annotations values
    let updateBy (predicate : OntologyAnnotation -> bool) (updateOption : UpdateOptions) (design : OntologyAnnotation) (annotations : OntologyAnnotation list) =
        if List.exists predicate annotations then
            annotations
            |> List.map (fun d -> if predicate d then updateOption.updateRecordType d design else d)
        else 
            annotations

    /// If an ontology annotation with the same annotation value as the given annotation value exists in the list, updates it with the given ontology annotation
    let updateByName (updateOption:UpdateOptions) (design : OntologyAnnotation) (annotations : OntologyAnnotation list) =
        updateBy (fun f -> f.Name = design.Name) updateOption design annotations

    ///// If a ontology annotation for which the predicate returns true exists in the Study.StudyDesignDescriptors, removes it from the study
    //let removeBy (predicate : OntologyAnnotation -> bool) (study:Study) =
    //    if exists predicate study then
    //        {study with StudyDesignDescriptors = List.filter (predicate >> not) study.StudyDesignDescriptors}
    //    else 
    //        study

    ///// If the given ontology annotation exists in the Study.StudyDesignDescriptors, removes it from the study
    //let remove (design : OntologyAnnotation) (study:Study) =
    //    removeBy ((=) design) study

    /// If a ontology annotation with the annotation value exists in the list, removes it
    let removeByName (name : AnnotationValue) (annotations : OntologyAnnotation list) = 
        List.filter (fun (d:OntologyAnnotation) -> d.Name = name) annotations

    // Comments
    
    /// Returns comments of a ontology annotation
    let getComments (annotation : OntologyAnnotation) =
        annotation.Comments
    
    /// Applies function f on comments of a ontology annotation
    let mapComments (f : Comment list -> Comment list) (annotation : OntologyAnnotation) =
        { annotation with 
            Comments = f annotation.Comments}
    
    /// Replaces comments of a ontology annotation by given comment list
    let setComments (annotation : OntologyAnnotation) (comments : Comment list) =
        { annotation with
            Comments = comments }