namespace ISADotNet.API

open ISADotNet
open Update

/// Investigation Publications
module Publication =  
  
    open ISADotNet

    ///// Adds the given publication to the investigation  
    //let add (publication : Publication) (investigation:Investigation) =
    //    {investigation with Publications = List.append investigation.Publications [publication]}

    ///// Returns true, if a publication for which the predicate returns true exists in the investigation
    //let exists (predicate : Publication -> bool) (investigation:Investigation) =
    //    investigation.Publications
    //    |> List.exists (predicate) 

    ///// Returns true, if the publication exists in the investigation
    //let contains (publication : Publication) (investigation:Investigation) =
    //    exists ((=) publication) investigation

    /// Returns true, if a publication with the given doi exists in the investigation
    let existsByDoi (doi : string) (publications : Publication list) =
        List.exists (fun p -> p.DOI = doi) publications

    /// Returns true, if a publication with the given pubmedID exists in the investigation
    let existsByPubMedID (pubMedID : string) (publications : Publication list) =
        List.exists (fun p -> p.PubMedID = pubMedID) publications

    /// Updates all publications for which the predicate returns true with the given publication values
    let updateBy (predicate : Publication -> bool) (updateOption : UpdateOptions) (publication : Publication) (publications : Publication list) =
        if List.exists predicate publications then
            publications
            |> List.map (fun p -> if predicate p then updateOption.updateRecordType p publication else p) 
        else 
            publications

    /// Updates all protocols with the same DOI as the given publication with its values
    let updateByDOI (updateOption : UpdateOptions) (publication : Publication) (publications : Publication list) =
        updateBy (fun p -> p.DOI = publication.DOI) updateOption publication publications

    /// Updates all protocols with the same pubMedID as the given publication with its values
    let updateByPubMedID (updateOption : UpdateOptions) (publication : Publication) (publications : Publication list) =
        updateBy (fun p -> p.PubMedID = publication.PubMedID) updateOption publication publications

    ///// If a publication for which the predicate returns true exists in the investigation, removes it from the investigation
    //let removeBy (predicate : Publication -> bool) (investigation:Investigation) =
    //    if exists predicate investigation then
    //        {investigation with Publications = List.filter (predicate >> not) investigation.Publications}
    //    else 
    //        investigation

    ///// If the given publication exists in the investigation, removes it from the investigation
    //let remove (publication : Publication) (investigation:Investigation) =
    //    removeBy ((=) publication) investigation

    /// If a publication with the given doi exists in the investigation, removes it from the investigation
    let removeByDoi (doi : string) (publications : Publication list) = 
        List.filter (fun p -> p.DOI = doi) publications

    /// If a publication with the given pubMedID exists in the investigation, removes it
    let removeByPubMedID (pubMedID : string) (publications : Publication list) = 
        List.filter (fun p -> p.PubMedID = pubMedID) publications

    /// Status

    /// Returns publication status of a publication
    let getStatus (publication : Publication) =
        publication.Status

    /// Applies function f on publication status of a publication
    let mapStatus (f : OntologyAnnotation -> OntologyAnnotation) (publication : Publication) =
        { publication with 
            Status = f publication.Status}

    /// Replaces publication status of a publication by given publication status
    let setStatus (publication : Publication) (status : OntologyAnnotation) =
        { publication with
            Status = status }

    // Comments
    
    /// Returns comments of a protocol
    let getComments (publication : Publication) =
        publication.Comments
    
    /// Applies function f on comments of a protocol
    let mapComments (f : Comment list -> Comment list) (publication : Publication) =
        { publication with 
            Comments = f publication.Comments}
    
    /// Replaces comments of a protocol by given comment list
    let setComments (publication : Publication) (comments : Comment list) =
        { publication with
            Comments = comments }