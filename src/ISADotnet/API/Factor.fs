namespace ISADotNet

open Update

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