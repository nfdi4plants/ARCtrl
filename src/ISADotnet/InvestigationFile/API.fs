namespace ISADotNet.InvestigationFile

/// Contains functions for manipulating ISA Investigation file items
module API =

    module TermSource = 

        /// If a termsource for which the predicate returns true exists in the investigation, gets it
        let tryGetBy (predicate : TermSource -> bool) (investigation:Investigation) =
            investigation.OntologySourceReference
            |> List.tryFind (predicate) 

        /// If an termSource with the given name exists in the investigation, returns it
        let tryGetByName (name : string) (investigation:Investigation) =
            tryGetBy (fun t -> t.Name = name)  investigation

        /// Returns true, if a termsource for which the predicate returns true exists in the investigation
        let exists (predicate : TermSource -> bool) (investigation:Investigation) =
            investigation.OntologySourceReference
            |> List.exists (predicate) 

        /// If an termSource with the given name exists in the investigation , returns it
        let existsByName (name : string) (investigation:Investigation) =
            exists (fun t -> t.Name = name) investigation

        /// Returns true, if the investigation contains the given termSource
        let contains (termSource : TermSource) (investigation:Investigation) =
            exists ((=) termSource) investigation

        /// Adds the given termSource to the investigation  
        let add (termSource : TermSource) (investigation:Investigation) =
            {investigation with OntologySourceReference = List.append investigation.OntologySourceReference [termSource]}

        /// If an termSource exists in the investigation for which the predicate returns true, updates it with the given termSource values, 
        let updateBy (predicate : TermSource -> bool) (termSource : TermSource) (investigation:Investigation) =
            if exists predicate investigation then
                {investigation 
                    with OntologySourceReference = 
                         investigation.OntologySourceReference
                         |> List.map (fun t -> if predicate t then {termSource with Comments = t.Comments} else t) 
                }
            else 
                investigation

        /// If an termSource with the same name as the given name exists in the investigation, updates it with the given termSource
        let updateByName (termSource : TermSource) (investigation:Investigation) =
            updateBy (fun t -> t.Name = termSource.Name) termSource investigation

        /// If a termsource for which the predicate returns true exists in the investigation, removes it from the investigation
        let removeBy (predicate : TermSource -> bool) (investigation:Investigation) =
            if exists predicate investigation then
                {investigation with OntologySourceReference = List.filter (predicate >> not) investigation.OntologySourceReference}
            else 
                investigation

        /// If the given termSource exists in the investigation, removes it from the investigation
        let remove (termSource : TermSource) (investigation:Investigation) =
            removeBy ((=) termSource) investigation

        /// If a termSource with the given name exists in the investigation, removes it from the investigation
        let removeByName (name : string) (investigation : Investigation) = 
            removeBy (fun t -> t.Name = name)  investigation

    /// Investigation Contacts
    module Person =  

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
        let updateBy (predicate : Person -> bool) (person : Person) (investigation:Investigation) =
            if exists predicate investigation then
                {investigation 
                    with Contacts = 
                         investigation.Contacts
                         |> List.map (fun p -> if predicate p then {person with Comments = p.Comments} else p) 
                }
            else 
                investigation

        /// If a person with the same name as the given person exists in the investigation exists, updates it with the given person
        let updateByFullName (person : Person) (investigation:Investigation) =
            updateBy (fun p -> p.FirstName = person.FirstName && p.MidInitials = person.MidInitials && p.LastName = person.LastName) person investigation

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

    /// Investigation Publications
    module Publication =  
  
        /// If a publication for which the predicate returns true exists in the investigation, gets it
        let tryGetBy (predicate : Publication -> bool) (investigation:Investigation) =
            investigation.Publications
            |> List.tryFind (predicate) 

        /// If an publication with the given doi exists in the investigation, returns it
        let tryGetByDOI (doi : string) (investigation:Investigation) =
            tryGetBy (fun p -> p.DOI = doi) investigation
    
        /// If an publication with the given pubmedID exists in the investigation, returns it
        let tryGetByPubMedID (pubMedID : string) (investigation:Investigation) =
            tryGetBy (fun p -> p.PubMedID = pubMedID) investigation

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

        /// Adds the given publication to the investigation  
        let add (publication : Publication) (investigation:Investigation) =
            {investigation with Publications = List.append investigation.Publications [publication]}

        /// If an publication exists in the investigation for which the predicate returns true, updates it with the given publication
        let updateBy (predicate : Publication -> bool) (publication : Publication) (investigation:Investigation) =
            if exists predicate investigation then
                {investigation 
                    with Publications = 
                         investigation.Publications
                         |> List.map (fun p -> if predicate p then {publication with Comments = p.Comments} else p) 
                }
            else 
                investigation

        /// If an publication with the same doi as the given publication exists in the investigation, updates it with the given publication
        let updateByDoi (publication : Publication) (investigation:Investigation) =
            updateBy (fun p -> p.DOI = publication.DOI) publication investigation

        /// If an publication with the same pubmedID as the given publication exists in the investigation, updates it with the given publication
        let updateByPubMedID (publication : Publication) (investigation:Investigation) =
            updateBy (fun p -> p.PubMedID = publication.PubMedID) publication investigation

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

        /// If a study for which the predicate returns true exists in the investigation, gets it
        let tryGetBy (predicate : Study -> bool) (investigation:Investigation) =
            investigation.Studies
            |> List.tryFind (predicate) 

        let tryGet (study : Study) (investigation:Investigation) =
            tryGetBy ((=) study) investigation

        /// If an study with the given identfier exists in the investigation, returns it
        let tryGetByIdentifier (identifier : string) (investigation:Investigation) =
            tryGetBy (fun s -> s.Info.Identifier = identifier)  investigation

        /// Returns true, if a study for which the predicate returns true exists in the investigation
        let exists (predicate : Study -> bool) (investigation:Investigation) =
            investigation.Studies
            |> List.exists (predicate) 

        /// Returns true, if the given study exists in the investigation
        let contains (study : Study) (investigation:Investigation) =
            exists ((=) study) investigation

        /// If an study with the given identfier exists in the investigation , returns it
        let existsByIdentifier (identifier : string) (investigation:Investigation) =
            exists (fun s -> s.Info.Identifier = identifier) investigation

        /// Adds the given study to the investigation  
        let add (study : Study) (investigation:Investigation) =
            {investigation with Studies = List.append investigation.Studies [study]}

        /// If an study exists in the investigation for which the predicate returns true, updates it with the given study
        let updateBy (predicate : Study -> bool) (study : Study) (investigation:Investigation) =
            if exists predicate investigation then
                {investigation with Studies = List.map (fun a -> if predicate a then study else a) investigation.Studies}
            else 
                investigation

        /// If an study with the same identifier as the given study exists in the investigation, updates it with the given study
        let updateByIdentifier (study : Study) (investigation:Investigation) =
            updateBy (fun s -> s.Info.Identifier = study.Info.Identifier) study investigation

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
            removeBy (fun s -> s.Info.Identifier = identifier)  investigation

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
            let updateBy (predicate : Assay -> bool) (assay : Assay) (study:Study) =
                if exists predicate study then
                    {study 
                        with Assays = 
                             study.Assays
                             |> List.map (fun a -> if predicate a then {assay with Comments = a.Comments} else a) 
                    }
                else 
                    study

            /// If an assay with the same fileName as the given assay exists in the study exists, updates it with the given assay values
            let updateByFileName (assay : Assay) (study:Study) =
                updateBy (fun a -> a.FileName = assay.FileName) assay study

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

            ///// TODO Filename = identfier ausformulieren
            ///// If no assay with the same identifier as the given assay exists in the study, adds the given assay to the study
            //let tryAdd (assay : Assay) (study:Study) =
            //    if study.Assays |> List.exists (fun studyAssay -> studyAssay.FileName = assay.FileName) then
            //        None
            //    else 
            //        Some {study with Assays = List.append study.Assays [assay]}

            ///// If an assay with the given identfier exists in the study exists, removes it from the study
            //let tryRemove (assayIdentfier : string) (study:Study) =
            //    if study.Assays |> List.exists (fun assay -> assay.FileName = assayIdentfier) then
            //        Some {study with Assays = List.filter (fun assay -> assay.FileName <> assaAssayyIdentfier) study.Assays }
            //    else 
            //        None

            ///// If an assay with the same identifier as the given assay exists in the study, overwrites its values with values in the given study
            //let tryUpdate (assay : Assay) (study:Study) =
            //    if study.Assays |> List.exists (fun studyAssay -> studyAssay.FileName = assay.FileName) then
            //        Some {study with Assays = 
            //                            study.Assays 
            //                            |> List.map (fun studyAssay -> if studyAssay.FileName = assay.FileName then assay else studyAssay) 
            //        }
            //    else 
            //        None

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
            let updateBy (predicate : Protocol -> bool) (protocol : Protocol) (study:Study) =
                if exists predicate study then
                    {study 
                        with Protocols = 
                             List.map (fun p -> if predicate p then {protocol with Comments = p.Comments} else p) study.Protocols}
                else 
                    study

            /// If a protocol with the same name as the given protocol exists in the study exists, updates it with the given protocol
            let updateByName (protocol : Protocol) (study:Study) =
                updateBy (fun p -> p.Name = protocol.Name) protocol study

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
            let updateBy (predicate : Factor -> bool) (factor : Factor) (study:Study) =
                if exists predicate study then
                    {study 
                        with Factors = 
                             List.map (fun f -> if predicate f then {factor with Comments = f.Comments} else f) study.Factors}
                else 
                    study

            /// If a factor with the same name as the given factor exists in the study exists, update its values with the given factor
            let updateByName (factor : Factor) (study:Study) =
                updateBy (fun f -> f.Name = factor.Name) factor study

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
            let updateBy (predicate : Person -> bool) (person : Person) (study:Study) =
                if exists predicate study then
                    {study 
                        with Contacts = 
                             study.Contacts
                             |> List.map (fun p -> if predicate p then {person with Comments = p.Comments} else p) 
                    }
                else 
                    study

            /// If a person with the same name as the given person exists in the study exists, updates it with the given person
            let updateByFullName (person : Person) (study:Study) =
                updateBy (fun p -> p.FirstName = person.FirstName && p.MidInitials = person.MidInitials && p.LastName = person.LastName) person study

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

            /// If a design for which the predicate returns true exists in the study, gets it
            let tryGetBy (predicate : Design -> bool) (study:Study) =
                study.DesignDescriptors
                |> List.tryFind (predicate) 

            /// If an design with the given design type exists in the study, returns it
            let tryGetByDesignType (designType : string) (study:Study) =
                tryGetBy (fun d -> d.DesignType = designType) study

            /// Returns true, if a design for which the predicate returns true exists in the study
            let exists (predicate : Design -> bool) (study:Study) =
                study.DesignDescriptors
                |> List.exists (predicate) 

            /// Returns true, if the given design exists in the study
            let contains (design : Design) (study:Study) =
                exists ((=) design) study

            /// Returns true, if a design with the given design type exists in the study
            let existsByDesignType (designType : string) (study:Study) =
                exists (fun d -> d.DesignType = designType) study

            /// Adds the given design to the study  
            let add (design : Design) (study:Study) =
                {study with DesignDescriptors = List.append study.DesignDescriptors [design]}

            /// If a design exists in the study for which the predicate returns true, updates it with the given design
            let updateBy (predicate : Design -> bool) (design : Design) (study:Study) =
                if exists predicate study then
                    {study 
                        with DesignDescriptors = 
                             study.DesignDescriptors
                             |> List.map (fun d -> if predicate d then {design with Comments = d.Comments} else d)
                    }
                else 
                    study

            /// If an design with the same designtype as the given design exists in the study exists, updates it with the given design
            let updateByDesignType (design : Design) (study:Study) =
                updateBy (fun f -> f.DesignType = design.DesignType) design study

            /// If a design for which the predicate returns true exists in the study, removes it from the study
            let removeBy (predicate : Design -> bool) (study:Study) =
                if exists predicate study then
                    {study with DesignDescriptors = List.filter (predicate >> not) study.DesignDescriptors}
                else 
                    study

            /// If the given design exists in the study, removes it from the study
            let remove (design : Design) (study:Study) =
                removeBy ((=) design) study

            /// If a design with the given full name exists in the study, removes it from the study
            let removeByDesignType (designType : string) (study : Study) = 
                removeBy (fun d -> d.DesignType = designType) study

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
            let updateBy (predicate : Publication -> bool) (publication : Publication) (study:Study) =
                if exists predicate study then
                    {study 
                        with Publications = 
                             study.Publications
                             |> List.map (fun p -> if predicate p then {publication with Comments = p.Comments} else p) 
                    }
                else 
                    study

            /// If an publication with the same doi as the given publication exists in the study, updates it with the given publication
            let updateByDoi (publication : Publication) (study:Study) =
                updateBy (fun p -> p.DOI = publication.DOI) publication study

            /// If an publication with the same pubmedID as the given publication exists in the study, updates it with the given publication
            let updateByPubMedID (publication : Publication) (study:Study) =
                updateBy (fun p -> p.PubMedID = publication.PubMedID) publication study

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


