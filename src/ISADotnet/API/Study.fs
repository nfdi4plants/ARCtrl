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
        List.tryFind (fun (s:Study) -> s.Identifier = identifier) studies

    ///// Returns true, if a study for which the predicate returns true exists in the investigation
    //let exists (predicate : Study -> bool) (investigation:Investigation) =
    //    investigation.Studies
    //    |> List.exists (predicate) 

    ///// Returns true, if the given study exists in the investigation
    //let contains (study : Study) (investigation:Investigation) =
    //    exists ((=) study) investigation

    /// If an study with the given identfier exists in the list, returns true
    let existsByIdentifier (identifier : string) (studies : Study list) =
        List.exists (fun (s:Study) -> s.Identifier = identifier) studies

    ///// Adds the given study to the investigation  
    //let add (study : Study) (investigation:Investigation) =
    //    {investigation with Studies = List.append investigation.Studies [study]}

    /// Updates all studies for which the predicate returns true with the given study values
    let updateBy (predicate : Study -> bool) (updateOption:UpdateOptions) (study : Study) (studies : Study list) =
        if List.exists predicate studies then
            List.map (fun a -> if predicate a then updateOption.updateRecordType a study else a) studies
        else 
            studies

    /// Updates all studies with the same identifier as the given study with its values
    let updateByIdentifier (updateOption:UpdateOptions) (study : Study) (studies : Study list) =
        updateBy (fun (s:Study) -> s.Identifier = study.Identifier) updateOption study studies

    ///// If a study for which the predicate returns true exists in the investigation, removes it
    //let removeBy (predicate : Study -> bool) (investigation:Investigation) =
    //    if exists predicate investigation then
    //        {investigation with Studies = List.filter (predicate >> not) investigation.Studies}
    //    else 
    //        investigation

    ///// If the given study exists in the investigation, removes it
    //let remove (study : Study) (investigation:Investigation) =
    //    removeBy ((=) study) investigation

    /// If a study with the given identifier exists in the list, removes it
    let removeByIdentifier (identifier : string) (studies : Study list) = 
        List.filter (fun (s:Study) -> s.Identifier = identifier) studies
    
    /// Returns all contacts of a study
    let getContacts (study : Study) =
        study.Contacts

    /// Applies function f to contacts of a study
    let mapContacts (f : Person list -> Person list) (study : Study) =
        { study with 
            Contacts = f study.Contacts }

    /// Replaces contacts of a study with the given person list
    let setContacts (study : Study) (persons : Person list) =
        { study with
            Contacts = persons }

    /// Returns publications of a study
    let getPublications (study : Study) =
        study.Publications

    /// Applies function f to publications of the study
    let mapPublications (f : Publication list -> Publication list) (study : Study) =
        { study with 
            Publications = f study.Publications }

    /// Replaces publications of a study with the given publication list
    let setPublications (study : Study) (publications : Publication list) =
        { study with
            Publications = publications }

    /// Returns study design descriptors of a study
    let getDescriptors (study : Study) =
        study.StudyDesignDescriptors

    /// Applies function f to to study design descriptors of a study
    let mapDescriptors (f : OntologyAnnotation list -> OntologyAnnotation list) (study : Study) =
        { study with 
            StudyDesignDescriptors = f study.StudyDesignDescriptors }

    /// Replaces study design descriptors with the given ontology annotation list
    let setDescriptors (study : Study) (descriptors : OntologyAnnotation list) =
        { study with
            StudyDesignDescriptors = descriptors }

    //module Person = 
    
    //    /// If a person for which the predicate returns true exists in the study, gets it
    //    let tryGetBy (predicate : Person -> bool) (study:Study) =
    //        study.Contacts
    //        |> List.tryFind (predicate) 
    
    //    /// If a person with the given full name exists in the study, returns it
    //    let tryGetByFullName (firstName : string) (midInitials : string) (lastName : string) (study:Study) =
    //        tryGetBy (fun p -> p.FirstName = firstName && p.MidInitials = midInitials && p.LastName = lastName) study
    
    //    /// Returns true, if a person for which the predicate returns true exists in the study
    //    let exists (predicate : Person -> bool) (study:Study) =
    //        study.Contacts
    //        |> List.exists (predicate) 
    
    //    /// Returns true, if the given person exists in the study
    //    let contains (person : Person) (study:Study) =
    //        exists ((=) person) study
    
    //    /// If an person with the given identfier exists in the study exists, returns it
    //    let existsByFullName (firstName : string) (midInitials : string) (lastName : string) (study:Study) =
    //        exists (fun p -> p.FirstName = firstName && p.MidInitials = midInitials && p.LastName = lastName) study
    
    //    /// adds the given person to the study  
    //    let add (person : Person) (study:Study) =
    //        {study with Contacts = List.append study.Contacts [person]}
    
    //    /// If an person exists in the study for which the predicate returns true, updates it with the given person
    //    let updateBy (predicate : Person -> bool) (updateOption:UpdateOptions) (person : Person) (study:Study) =
    //        if exists predicate study then
    //            {study 
    //                with Contacts = 
    //                        study.Contacts
    //                        |> List.map (fun p -> if predicate p then updateOption.updateRecordType p person else p) 
    //            }
    //        else 
    //            study
    
    //    /// If a person with the same name as the given person exists in the study exists, updates it with the given person
    //    let updateByFullName (updateOption:UpdateOptions) (person : Person) (study:Study) =
    //        updateBy (fun p -> p.FirstName = person.FirstName && p.MidInitials = person.MidInitials && p.LastName = person.LastName) updateOption person study
    
    //    /// If a person for which the predicate returns true exists in the study, removes it from the study
    //    let removeBy (predicate : Person -> bool) (study:Study) =
    //        if exists predicate study then
    //            {study with Contacts = List.filter (predicate >> not) study.Contacts}
    //        else 
    //            study
    
    //    /// If the given person exists in the study, removes it from the study
    //    let remove (person : Person) (study:Study) =
    //        removeBy ((=) person) study
    
    //    /// If a person with the given full name exists in the study, removes it from the study
    //    let removeByFullName (firstName : string) (midInitials : string) (lastName : string) (study:Study) =
    //        removeBy (fun p -> p.FirstName = firstName && p.MidInitials = midInitials && p.LastName = lastName) study

    
    //module Publication =  
  
    //    /// If a publication for which the predicate returns true exists in the study, gets it
    //    let tryGetBy (predicate : Publication -> bool) (study:Study) =
    //        study.Publications
    //        |> List.tryFind (predicate) 

    //    /// If an publication with the given doi exists in the study, returns it
    //    let tryGetByDOI (doi : string) (study:Study) =
    //        tryGetBy (fun p -> p.DOI = doi) study
    
    //    /// If an publication with the given pubmedID exists in the study, returns it
    //    let tryGetByPubMedID (pubMedID : string) (study:Study) =
    //        tryGetBy (fun p -> p.PubMedID = pubMedID) study

    //    /// Returns true, if a publication for which the predicate returns true exists in the study
    //    let exists (predicate : Publication -> bool) (study:Study) =
    //        study.Publications
    //        |> List.exists (predicate) 

    //    /// Returns true, if the publication exists in the study
    //    let contains (publication : Publication) (study:Study) =
    //        exists ((=) publication) study

    //    /// Returns true, if a publication with the given doi exists in the study
    //    let existsByDoi (doi : string) (study:Study) =
    //        exists (fun p -> p.DOI = doi) study
    
    //    /// Returns true, if a publication with the given pubmedID exists in the study
    //    let existsByPubMedID (pubMedID : string) (study:Study) =
    //        exists (fun p -> p.PubMedID = pubMedID) study

    //    /// Adds the given publication to the study  
    //    let add (publication : Publication) (study:Study) =
    //        {study with Publications = List.append study.Publications [publication]}

    //    /// If an publication exists in the study for which the predicate returns true, updates it with the given publication
    //    let updateBy (predicate : Publication -> bool) (updateOption:UpdateOptions) (publication : Publication) (study:Study) =
    //        if exists predicate study then
    //            {study 
    //                with Publications = 
    //                     study.Publications
    //                     |> List.map (fun p -> if predicate p then updateOption.updateRecordType p publication else p) 
    //            }
    //        else 
    //            study

    //    /// If an publication with the same doi as the given publication exists in the study, updates it with the given publication
    //    let updateByDoi (updateOption:UpdateOptions) (publication : Publication) (study:Study) =
    //        updateBy (fun p -> p.DOI = publication.DOI) updateOption publication study

    //    /// If an publication with the same pubmedID as the given publication exists in the study, updates it with the given publication
    //    let updateByPubMedID (updateOption:UpdateOptions) (publication : Publication) (study:Study) =
    //        updateBy (fun p -> p.PubMedID = publication.PubMedID) updateOption publication study

    //    /// If a publication for which the predicate returns true exists in the study, removes it from the study
    //    let removeBy (predicate : Publication -> bool) (study:Study) =
    //        if exists predicate study then
    //            {study with Publications = List.filter (predicate >> not) study.Publications}
    //        else 
    //            study

    //    /// If the given publication exists in the study, removes it from the study
    //    let remove (publication : Publication) (study:Study) =
    //        removeBy ((=) publication) study

    //    /// If a publication with the given doi exists in the study, removes it from the study
    //    let removeByDoi (doi : string) (study : Study) = 
    //        removeBy (fun p -> p.DOI = doi) study

    //    /// If a publication with the given pubMedID exists in the study, removes it from the study
    //    let removeByPubMedID (pubMedID : string) (study : Study) = 
    //        removeBy (fun p -> p.PubMedID = pubMedID) study