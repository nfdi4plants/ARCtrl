namespace ISADotNet.InvestigationFile

open Update

/// Contains functions for manipulating ISA Investigation file items
[<System.Obsolete("This is deprecated and only left for control for now")>]
module API =

    module OntologySourceReference = 

        open ISADotNet

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

    /// Investigation Contacts
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
            if exists predicate investigation then
                {investigation 
                    with Contacts = 
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

        ///// If an publication with the given doi exists in the investigation, returns it
        //let tryGetByDOI (doi : string) (investigation:Investigation) =
        //    tryGetBy (fun p -> p.DOI = doi) investigation
    
        ///// If an publication with the given pubmedID exists in the investigation, returns it
        //let tryGetByPubMedID (pubMedID : URI) (investigation:Investigation) =
        //    tryGetBy (fun p -> p.PubMedID = pubMedID) investigation

    /// Investigation Publications
    module Publication =  
  
        open ISADotNet

        /// Adds the given publication to the investigation  
        let add (publication : Publication) (investigation:Investigation) =
            {investigation with Publications = List.append investigation.Publications [publication]}

        /// Returns true, if a publication for which the predicate returns true exists in the investigation
        let exists (predicate : Publication -> bool) (investigation:Investigation) =
            investigation.Publications
            |> List.exists (predicate) 

        /// Returns true, if the publication exists in the investigation
        let contains (publication : Publication) (investigation:Investigation) =
            exists ((=) publication) investigation

        /// Returns true, if a publication with the given doi exists in the investigation
        let existsByDoi (doi : string) (investigation:Investigation) =
            exists (fun p -> p.DOI = doi) investigation
    
        /// Returns true, if a publication with the given pubmedID exists in the investigation
        let existsByPubMedID (pubMedID : string) (investigation:Investigation) =
            exists (fun p -> p.PubMedID = pubMedID) investigation

        /// If an publication exists in the investigation for which the predicate returns true, updates it with the given publication
        let updateBy (predicate : Publication -> bool) (updateOption:UpdateOptions) (publication : Publication) (investigation:Investigation) =
            if exists predicate investigation then
                {investigation 
                    with Publications = 
                         investigation.Publications
                         |> List.map (fun p -> if predicate p then updateOption.updateRecordType p publication else p) 
                }
            else 
                investigation

        /// If an publication with the same pubmedID as the given publication exists in the investigation, updates it with the given publication
        let updateByPubMedID (updateOption:UpdateOptions) (publication : Publication) (investigation:Investigation) =
            updateBy (fun p -> p.PubMedID = publication.PubMedID) updateOption publication investigation

        /// If a publication for which the predicate returns true exists in the investigation, removes it from the investigation
        let removeBy (predicate : Publication -> bool) (investigation:Investigation) =
            if exists predicate investigation then
                {investigation with Publications = List.filter (predicate >> not) investigation.Publications}
            else 
                investigation

        /// If the given publication exists in the investigation, removes it from the investigation
        let remove (publication : Publication) (investigation:Investigation) =
            removeBy ((=) publication) investigation

        /// If a publication with the given doi exists in the investigation, removes it from the investigation
        let removeByDoi (doi : string) (investigation : Investigation) = 
            removeBy (fun p -> p.DOI = doi) investigation

        /// If a publication with the given pubMedID exists in the investigation, removes it from the investigation
        let removeByPubMedID (pubMedID : string) (investigation : Investigation) = 
            removeBy (fun p -> p.PubMedID = pubMedID) investigation

    module Study =

        open ISADotNet

        /// If a study for which the predicate returns true exists in the investigation, gets it
        let tryGetBy (predicate : Study -> bool) (investigation:Investigation) =
            investigation.Studies
            |> List.tryFind (predicate) 

        let tryGet (study : Study) (investigation:Investigation) =
            tryGetBy ((=) study) investigation

        /// If an study with the given identfier exists in the investigation, returns it
        let tryGetByIdentifier (identifier : string) (investigation:Investigation) =
            tryGetBy (fun s -> s.Identifier = identifier)  investigation

        /// Returns true, if a study for which the predicate returns true exists in the investigation
        let exists (predicate : Study -> bool) (investigation:Investigation) =
            investigation.Studies
            |> List.exists (predicate) 

        /// Returns true, if the given study exists in the investigation
        let contains (study : Study) (investigation:Investigation) =
            exists ((=) study) investigation

        /// If an study with the given identfier exists in the investigation , returns it
        let existsByIdentifier (identifier : string) (investigation:Investigation) =
            exists (fun s -> s.Identifier = identifier) investigation

        /// Adds the given study to the investigation  
        let add (study : Study) (investigation:Investigation) =
            {investigation with Studies = List.append investigation.Studies [study]}

        /// If an study exists in the investigation for which the predicate returns true, updates it with the given study
        let updateBy (predicate : Study -> bool) (updateOption:UpdateOptions) (study : Study) (investigation:Investigation) =
            if exists predicate investigation then
                {investigation with Studies = List.map (fun a -> if predicate a then updateOption.updateRecordType a study else a) investigation.Studies}
            else 
                investigation

        /// If an study with the same identifier as the given study exists in the investigation, updates it with the given study
        let updateByIdentifier (updateOption:UpdateOptions) (study : Study) (investigation:Investigation) =
            updateBy (fun s -> s.Identifier = study.Identifier) updateOption study investigation

        /// If a study for which the predicate returns true exists in the investigation, removes it
        let removeBy (predicate : Study -> bool) (investigation:Investigation) =
            if exists predicate investigation then
                {investigation with Studies = List.filter (predicate >> not) investigation.Studies}
            else 
                investigation

        /// If the given study exists in the investigation, removes it
        let remove (study : Study) (investigation:Investigation) =
            removeBy ((=) study) investigation

        /// If a study with the given identifier exists in the investigation, removes it
        let removeByIdentifier (identifier : string) (investigation : Investigation) = 
            removeBy (fun s -> s.Identifier = identifier)  investigation

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

        module Factor =  

            /// If a factor for which the predicate returns true exists in the study, gets it
            let tryGetBy (predicate : Factor -> bool) (study:Study) =
                study.Factors
                |> List.tryFind (predicate) 

            /// If a factor with the given name exists in the study, returns it
            let tryGetByName (name : string) (study:Study) =
                tryGetBy (fun f -> f.Name = name) study

            /// Returns true, if a factor for which the predicate returns true exists in the study
            let exists (predicate : Factor -> bool) (study:Study) =
                study.Factors
                |> List.exists (predicate) 

            /// Returns true, if the factor exists in the study
            let contains (factor : Factor) (study:Study) =
                exists ((=) factor) study

            /// If a factor with the given identfier exists in the study exists, returns it
            let existsByName (name : string) (study:Study) =
                exists (fun f -> f.Name = name) study

            /// adds the given factor to the study  
            let add (factor : Factor) (study:Study) =
                {study with Factors = List.append study.Factors [factor]}

            /// If a factor exists in the study for which the predicate returns true, updates it with the given factor
            let updateBy (predicate : Factor -> bool) (updateOption:UpdateOptions) (factor : Factor) (study:Study) =
                if exists predicate study then
                    {study 
                        with Factors = 
                             List.map (fun f -> if predicate f then updateOption.updateRecordType f factor else f) study.Factors}
                else 
                    study

            /// If a factor with the same name as the given factor exists in the study exists, update its values with the given factor
            let updateByName (updateOption:UpdateOptions) (factor : Factor) (study:Study) =
                updateBy (fun f -> f.Name = factor.Name) updateOption factor study

            /// If a factor for which the predicate returns true exists in the study, removes it from the study
            let removeBy (predicate : Factor -> bool) (study:Study) =
                if exists predicate study then
                    {study with Factors = List.filter (predicate >> not) study.Factors}
                else 
                    study

            /// If the given factor exists in the study, removes it from the study
            let remove (factor : Factor) (study:Study) =
                removeBy ((=) factor) study

            /// If a factor with the given name exists in the study, removes it from the study
            let removeByName (name : string) (study : Study) = 
                removeBy (fun f -> f.Name = name) study

        module Person =  

            /// If a person for which the predicate returns true exists in the study, gets it
            let tryGetBy (predicate : Person -> bool) (study:Study) =
                study.Contacts
                |> List.tryFind (predicate) 

            /// If a person with the given full name exists in the study, returns it
            let tryGetByFullName (firstName : string) (midInitials : string) (lastName : string) (study:Study) =
                tryGetBy (fun p -> p.FirstName = firstName && p.MidInitials = midInitials && p.LastName = lastName) study

            /// Returns true, if a person for which the predicate returns true exists in the study
            let exists (predicate : Person -> bool) (study:Study) =
                study.Contacts
                |> List.exists (predicate) 

            /// Returns true, if the given person exists in the study
            let contains (person : Person) (study:Study) =
                exists ((=) person) study

            /// If an person with the given identfier exists in the study exists, returns it
            let existsByFullName (firstName : string) (midInitials : string) (lastName : string) (study:Study) =
                exists (fun p -> p.FirstName = firstName && p.MidInitials = midInitials && p.LastName = lastName) study

            /// adds the given person to the study  
            let add (person : Person) (study:Study) =
                {study with Contacts = List.append study.Contacts [person]}

            /// If an person exists in the study for which the predicate returns true, updates it with the given person
            let updateBy (predicate : Person -> bool) (updateOption:UpdateOptions) (person : Person) (study:Study) =
                if exists predicate study then
                    {study 
                        with Contacts = 
                             study.Contacts
                             |> List.map (fun p -> if predicate p then updateOption.updateRecordType p person else p) 
                    }
                else 
                    study

            /// If a person with the same name as the given person exists in the study exists, updates it with the given person
            let updateByFullName (updateOption:UpdateOptions) (person : Person) (study:Study) =
                updateBy (fun p -> p.FirstName = person.FirstName && p.MidInitials = person.MidInitials && p.LastName = person.LastName) updateOption person study

            /// If a person for which the predicate returns true exists in the study, removes it from the study
            let removeBy (predicate : Person -> bool) (study:Study) =
                if exists predicate study then
                    {study with Contacts = List.filter (predicate >> not) study.Contacts}
                else 
                    study

            /// If the given person exists in the study, removes it from the study
            let remove (person : Person) (study:Study) =
                removeBy ((=) person) study

            /// If a person with the given full name exists in the study, removes it from the study
            let removeByFullName (firstName : string) (midInitials : string) (lastName : string) (study:Study) =
                removeBy (fun p -> p.FirstName = firstName && p.MidInitials = midInitials && p.LastName = lastName) study

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

        module Publication =  
  
            /// If a publication for which the predicate returns true exists in the study, gets it
            let tryGetBy (predicate : Publication -> bool) (study:Study) =
                study.Publications
                |> List.tryFind (predicate) 

            /// If an publication with the given doi exists in the study, returns it
            let tryGetByDOI (doi : string) (study:Study) =
                tryGetBy (fun p -> p.DOI = doi) study
        
            /// If an publication with the given pubmedID exists in the study, returns it
            let tryGetByPubMedID (pubMedID : string) (study:Study) =
                tryGetBy (fun p -> p.PubMedID = pubMedID) study

            /// Returns true, if a publication for which the predicate returns true exists in the study
            let exists (predicate : Publication -> bool) (study:Study) =
                study.Publications
                |> List.exists (predicate) 

            /// Returns true, if the publication exists in the study
            let contains (publication : Publication) (study:Study) =
                exists ((=) publication) study

            /// Returns true, if a publication with the given doi exists in the study
            let existsByDoi (doi : string) (study:Study) =
                exists (fun p -> p.DOI = doi) study
        
            /// Returns true, if a publication with the given pubmedID exists in the study
            let existsByPubMedID (pubMedID : string) (study:Study) =
                exists (fun p -> p.PubMedID = pubMedID) study

            /// Adds the given publication to the study  
            let add (publication : Publication) (study:Study) =
                {study with Publications = List.append study.Publications [publication]}

            /// If an publication exists in the study for which the predicate returns true, updates it with the given publication
            let updateBy (predicate : Publication -> bool) (updateOption:UpdateOptions) (publication : Publication) (study:Study) =
                if exists predicate study then
                    {study 
                        with Publications = 
                             study.Publications
                             |> List.map (fun p -> if predicate p then updateOption.updateRecordType p publication else p) 
                    }
                else 
                    study

            /// If an publication with the same doi as the given publication exists in the study, updates it with the given publication
            let updateByDoi (updateOption:UpdateOptions) (publication : Publication) (study:Study) =
                updateBy (fun p -> p.DOI = publication.DOI) updateOption publication study

            /// If an publication with the same pubmedID as the given publication exists in the study, updates it with the given publication
            let updateByPubMedID (updateOption:UpdateOptions) (publication : Publication) (study:Study) =
                updateBy (fun p -> p.PubMedID = publication.PubMedID) updateOption publication study

            /// If a publication for which the predicate returns true exists in the study, removes it from the study
            let removeBy (predicate : Publication -> bool) (study:Study) =
                if exists predicate study then
                    {study with Publications = List.filter (predicate >> not) study.Publications}
                else 
                    study

            /// If the given publication exists in the study, removes it from the study
            let remove (publication : Publication) (study:Study) =
                removeBy ((=) publication) study

            /// If a publication with the given doi exists in the study, removes it from the study
            let removeByDoi (doi : string) (study : Study) = 
                removeBy (fun p -> p.DOI = doi) study

            /// If a publication with the given pubMedID exists in the study, removes it from the study
            let removeByPubMedID (pubMedID : string) (study : Study) = 
                removeBy (fun p -> p.PubMedID = pubMedID) study


