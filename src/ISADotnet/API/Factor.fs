namespace ISADotNet.API

open ISADotNet
open Update

module Factor =  

    ///// If a factor for which the predicate returns true exists in the study, gets it
    //let tryGetBy (predicate : Factor -> bool) (study:Study) =
    //    study.Factors
    //    |> List.tryFind (predicate) 

    /// If a factor with the given name exists in the list, returns it
    let tryGetByName (name : string) (factors:Factor list) =
        List.tryFind (fun (f:Factor) -> f.Name = name) factors

    ///// Returns true, if a factor for which the predicate returns true exists in the study
    //let exists (predicate : Factor -> bool) (study:Study) =
    //    study.Factors
    //    |> List.exists (predicate) 

    ///// Returns true, if the factor exists in the study
    //let contains (factor : Factor) (study:Study) =
    //    exists ((=) factor) study

    /// If a factor with the given name exists in the list exists, returns true
    let existsByName (name : string) (factors:Factor list) =
        List.exists (fun (f:Factor) -> f.Name = name) factors

    ///// adds the given factor to the study  
    //let add (factor : Factor) (study:Study) =
    //    {study with Factors = List.append study.Factors [factor]}

    /// Updates all factors for which the predicate returns true with the given factor values
    let updateBy (predicate : Factor -> bool) (updateOption : UpdateOptions) (factor : Factor) (factors : Factor list) =
        if List.exists predicate factors then
            List.map (fun f -> if predicate f then updateOption.updateRecordType f factor else f) factors
        else 
            factors

    /// Updates all factors with the same name as the given factor with its values
    let updateByName (updateOption : UpdateOptions) (factor : Factor) (factors : Factor list) =
        updateBy (fun f -> f.Name = factor.Name) updateOption factor factors

    ///// If a factor for which the predicate returns true exists in the study, removes it from the study
    //let removeBy (predicate : Factor -> bool) (study:Study) =
    //    if exists predicate study then
    //        {study with Factors = List.filter (predicate >> not) study.Factors}
    //    else 
    //        study

    ///// If the given factor exists in the study, removes it from the study
    //let remove (factor : Factor) (study:Study) =
    //    removeBy ((=) factor) study

    /// If a factor with the given name exists in the list, removes it
    let removeByName (name : string) (factors : Factor list) = 
        List.filter (fun (f:Factor) -> f.Name = name) factors

    /// Returns comments of a factor
    let getComments (factor : Factor) =
        factor.Comments

    /// Applies function f on comments of a factor
    let mapComments (f : Comment list -> Comment list) (factor : Factor) =
        { factor with 
            Comments = f factor.Comments}

    /// Replaces comments of a factor by given comment list
    let setComments (factor : Factor) (comments : Comment list) =
        { factor with
            Comments = comments }

    /// Returns factor type of a factor
    let getFactorType (factor : Factor) =
        factor.FactorType

    /// Applies function f on factor type of a factor
    let mapFactorType (f : OntologyAnnotation -> OntologyAnnotation) (factor : Factor) =
        { factor with 
            FactorType = f factor.FactorType}

    /// Replaces factor type of a factor by given factor type
    let setFactorType (factor : Factor) (factorType : OntologyAnnotation) =
        { factor with
            FactorType = factorType }