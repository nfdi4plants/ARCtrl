namespace ISADotNet

open Update

module Person =  

    open ISADotNet

    /// If a person for which the predicate returns true exists in the investigation, gets it
    let tryGetBy (predicate : Person -> bool) (investigation:Investigation) =
        investigation.Contacts
        |> List.tryFind (predicate) 

    /// If a person with the given full name exists in the investigation, returns it
    let tryGetByFullName (firstName : string) (midInitials : string) (lastName : string) (investigation:Investigation) =
        tryGetBy (fun p -> p.FirstName = firstName && p.MidInitials = midInitials && p.LastName = lastName) investigation

    /// Returns true, if a person for which the predicate returns true exists in the investigation
    let exists (predicate : Person -> bool) (investigation:Investigation) =
        investigation.Contacts
        |> List.exists (predicate) 

    /// Returns true, if the given person exists in the investigation
    let contains (person : Person) (investigation:Investigation) =
        exists ((=) person) investigation

    /// If an person with the given identfier exists in the investigation exists, returns it
    let existsByFullName (firstName : string) (midInitials : string) (lastName : string) (investigation:Investigation) =
        exists (fun p -> p.FirstName = firstName && p.MidInitials = midInitials && p.LastName = lastName) investigation

    /// adds the given person to the investigation  
    let add (person : Person) (investigation:Investigation) =
        {investigation with Contacts = List.append investigation.Contacts [person]}

    /// If an person exists in the investigation for which the predicate returns true, updates it with the given person
    let updateBy (predicate : Person -> bool) (updateOption:UpdateOptions) (person : Person) (investigation:Investigation) =
        if exists predicate investigation 
        then
            { investigation with 
                Contacts = 
                    investigation.Contacts
                    |> List.map (fun p -> if predicate p then updateOption.updateRecordType p person else p) 
            }
        else 
            investigation

    /// If a person with the same name as the given person exists in the investigation exists, updates it with the given person
    let updateByFullName (updateOption:UpdateOptions) (person : Person) (investigation:Investigation) =
        updateBy (fun p -> p.FirstName = person.FirstName && p.MidInitials = person.MidInitials && p.LastName = person.LastName) updateOption person investigation

    /// If a person for which the predicate returns true exists in the investigation, removes it from the investigation
    let removeBy (predicate : Person -> bool) (investigation:Investigation) =
        if exists predicate investigation then
            {investigation with Contacts = List.filter (predicate >> not) investigation.Contacts}
        else 
            investigation

    /// If the given person exists in the investigation, removes it from the investigation
    let remove (person : Person) (investigation:Investigation) =
        removeBy ((=) person) investigation
    
    /// If a person with the given full name exists in the investigation, removes it from the investigation
    let removeByFullName (firstName : string) (midInitials : string) (lastName : string) (investigation:Investigation) =
        removeBy (fun p -> p.FirstName = firstName && p.MidInitials = midInitials && p.LastName = lastName) investigation

    module Design =  

        /// If a ontology annotation for which the predicate returns true exists in the Study.StudyDesignDescriptors, gets it
        let tryGetBy (predicate : OntologyAnnotation -> bool) (study:Study) =
            study.StudyDesignDescriptors
            |> List.tryFind (predicate) 

        /// If an ontology annotation with the given name (AnnotationValue) exists in the Study.StudyDesignDescriptors, returns it
        let tryGetByName (name : AnnotationValue) (study:Study) =
            tryGetBy (fun d -> d.Name = name) study

        /// Returns true, if a ontology annotation for which the predicate returns true exists in the Study.StudyDesignDescriptors
        let exists (predicate : OntologyAnnotation -> bool) (study:Study) =
            study.StudyDesignDescriptors
            |> List.exists (predicate) 

        /// Returns true, if the given ontology annotation exists in the Study.StudyDesignDescriptors
        let contains (design : OntologyAnnotation) (study:Study) =
            exists ((=) design) study

        /// Returns true, if a ontology annotation with the given name (AnnotationValue) exists in the Study.StudyDesignDescriptors
        let existsByName (name : AnnotationValue) (study:Study) =
            exists (fun d -> d.Name = name) study

        /// Adds the given ontology annotation to the Study.StudyDesignDescriptors
        let add (design : OntologyAnnotation) (study:Study) =
            {study with StudyDesignDescriptors = List.append study.StudyDesignDescriptors [design]}

        /// If a ontology annotation exists in the Study.StudyDesignDescriptors for which the predicate returns true, updates it with the given ontology annotation
        let updateBy (predicate : OntologyAnnotation -> bool) (updateOption:UpdateOptions) (design : OntologyAnnotation) (study:Study) =
            if exists predicate study then
                {study 
                    with StudyDesignDescriptors = 
                         study.StudyDesignDescriptors
                         |> List.map (fun d -> if predicate d then updateOption.updateRecordType d design else d)
                }
            else 
                study

        /// If an ontology annotation with the same name as the given ontology annotation exists in the Study.StudyDesignDescriptors, updates it with the given ontology annotation.
        let updateByName (updateOption:UpdateOptions) (design : OntologyAnnotation) (study:Study) =
            updateBy (fun f -> f.Name = design.Name) updateOption design study

        /// If a ontology annotation for which the predicate returns true exists in the Study.StudyDesignDescriptors, removes it from the study
        let removeBy (predicate : OntologyAnnotation -> bool) (study:Study) =
            if exists predicate study then
                {study with StudyDesignDescriptors = List.filter (predicate >> not) study.StudyDesignDescriptors}
            else 
                study

        /// If the given ontology annotation exists in the Study.StudyDesignDescriptors, removes it from the study
        let remove (design : OntologyAnnotation) (study:Study) =
            removeBy ((=) design) study

        /// If a ontology annotation with the name (AnnotationValue) exists in the Study.StudyDesignDescriptors, removes it from the study
        let removeByName (name : AnnotationValue) (study : Study) = 
            removeBy (fun d -> d.Name = name) study