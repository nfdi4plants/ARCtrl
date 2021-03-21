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
        List.tryFind (fun (t : OntologySourceReference) -> t.Name = Some name) ontologies

    ///// Returns true, if a ontology source reference for which the predicate returns true exists in the investigation
    //let exists (predicate : OntologySourceReference -> bool) (investigation:Investigation) =
    //    investigation.OntologySourceReferences
    //    |> List.exists (predicate) 

    /// If an ontology source reference with the given name exists in the list, returns true
    let existsByName (name : string) (ontologies : OntologySourceReference list) =
        List.exists (fun (t : OntologySourceReference) -> t.Name = Some name) ontologies

    ///// Returns true, if the investigation contains the given ontology source reference
    //let contains (ontologySourceReference : OntologySourceReference) (investigation:Investigation) =
    //    exists ((=) ontologySourceReference) investigation

    /// Adds the given ontology source reference to the investigation  
    let add (ontologySourceReference : OntologySourceReference) (ontologies : OntologySourceReference list) =
        List.append ontologies [ontologySourceReference]

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


    /// If a ontology source reference with the given name exists in the list, removes it
    let removeByName (name : string) (ontologies : OntologySourceReference list) = 
        List.filter (fun (t : OntologySourceReference) -> t.Name = Some name |> not) ontologies

    /// Returns comments of ontology source ref
    let getComments (ontology : OntologySourceReference) =
        ontology.Comments

    /// Applies function f on comments in ontology source ref
    let mapComments (f : Comment list -> Comment list) (ontology : OntologySourceReference) =
        { ontology with 
            Comments = Option.mapDefault [] f ontology.Comments}

    /// Replaces comments in ontology source ref by given comment list
    let setComments (ontology : OntologySourceReference) (comments : Comment list) =
        { ontology with
            Comments = Some comments }

module OntologyAnnotation =  

    /// Returns true if the given name matches the name of the ontology annotation
    let nameEqualsString (name : string) (oa : OntologyAnnotation) =
        match oa.Name with
        | Some (AnnotationValue.Text s) when s  = name -> true
        | None when name = "" -> true
        | Some (AnnotationValue.Float f) when (string f)  = name -> true
        | Some (AnnotationValue.Int i) when (string i)  = name -> true
        | _ -> false

    /// If an ontology annotation with the given annotation value exists in the list, returns it
    let tryGetByName (name : AnnotationValue) (annotations : OntologyAnnotation list) =
        List.tryFind (fun (d:OntologyAnnotation) -> d.Name = Some name) annotations

    ///// Returns true, if a ontology annotation for which the predicate returns true exists in the Study.StudyDesignDescriptors
    //let exists (predicate : OntologyAnnotation -> bool) (study:Study) =
    //    study.StudyDesignDescriptors
    //    |> List.exists (predicate) 

    ///// Returns true, if the given ontology annotation exists in the Study.StudyDesignDescriptors
    //let contains (design : OntologyAnnotation) (study:Study) =
    //    exists ((=) design) study

    /// If a ontology annotation with the given annotation value exists in the list, returns true
    let existsByName (name : AnnotationValue) (annotations : OntologyAnnotation list) =
        List.exists (fun (d:OntologyAnnotation) -> d.Name = Some name) annotations

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

    /// If a ontology annotation with the annotation value exists in the list, removes it
    let removeByName (name : AnnotationValue) (annotations : OntologyAnnotation list) = 
        List.filter (fun (d:OntologyAnnotation) -> d.Name = Some name |> not) annotations

    // Comments
    
    /// Returns comments of a ontology annotation
    let getComments (annotation : OntologyAnnotation) =
        annotation.Comments
    
    /// Applies function f on comments of a ontology annotation
    let mapComments (f : Comment list -> Comment list) (annotation : OntologyAnnotation) =
        { annotation with 
            Comments = Option.mapDefault [] f annotation.Comments}
    
    /// Replaces comments of a ontology annotation by given comment list
    let setComments (annotation : OntologyAnnotation) (comments : Comment list) =
        { annotation with
            Comments = Some comments }