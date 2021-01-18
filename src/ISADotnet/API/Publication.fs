namespace ISADotNet

open Update

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