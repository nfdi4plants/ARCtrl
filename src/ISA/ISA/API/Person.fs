namespace ISA.API

open ISA
open Update

module Person =  

    open ISA

    ///// If a person for which the predicate returns true exists in the investigation, gets it
    //let tryGetBy (predicate : Person -> bool) (investigation:Investigation) =
    //    investigation.Contacts
    //    |> List.tryFind (predicate) 

    /// If a person with the given FirstName, MidInitials and LastName exists in the list, returns it
    let tryGetByFullName (firstName : string) (midInitials : string) (lastName : string) (persons : Person list) =
        List.tryFind (fun p -> 
            if midInitials = "" then 
                p.FirstName = Some firstName && p.LastName = Some lastName
            else 

                p.FirstName = Some firstName && p.MidInitials = Some midInitials && p.LastName = Some lastName
        ) persons

    ///// Returns true, if a person for which the predicate returns true exists in the investigation
    //let exists (predicate : Person -> bool) (investigation:Investigation) =
    //    investigation.Contacts
    //    |> List.exists (predicate) 

    ///// Returns true, if the given person exists in the investigation
    //let contains (person : Person) (investigation:Investigation) =
    //    exists ((=) person) investigation

    /// If an person with the given FirstName, MidInitials and LastName exists in the list, returns true
    let existsByFullName (firstName : string) (midInitials : string) (lastName : string) (persons : Person list) =
        List.exists (fun p -> 
            if midInitials = "" then 
                p.FirstName = Some firstName && p.LastName = Some lastName
            else 

                p.FirstName = Some firstName && p.MidInitials = Some midInitials && p.LastName = Some lastName
        ) persons

    /// adds the given person to the persons  
    let add (persons : Person list) (person : Person) =
        List.append persons [person]

    /// Updates all persons for which the predicate returns true with the given person values
    let updateBy (predicate : Person -> bool) (updateOption:UpdateOptions) (person : Person) (persons : Person list) =
        if List.exists predicate persons 
        then
            persons
            |> List.map (fun p -> if predicate p then updateOption.updateRecordType p person else p) 
        else 
            persons

    /// Updates all persons with the same FirstName, MidInitials and LastName as the given person with its values
    let updateByFullName (updateOption:UpdateOptions) (person : Person) (persons : Person list) =
        updateBy (fun p -> p.FirstName = person.FirstName && p.MidInitials = person.MidInitials && p.LastName = person.LastName) updateOption person persons
    
    /// If a person with the given FirstName, MidInitials and LastName exists in the list, removes it
    let removeByFullName (firstName : string) (midInitials : string) (lastName : string) (persons : Person list) =
        List.filter (fun p ->
            if midInitials = "" then
                (p.FirstName = Some firstName && p.LastName = Some lastName)
                |> not
            else
                (p.FirstName = Some firstName && p.MidInitials = Some midInitials && p.LastName = Some lastName)
                |> not
        ) persons

    // Roles
    
    /// Returns roles of a person
    let getRoles (person : Person) =
        person.Roles
    
    /// Applies function f on roles of a person
    let mapRoles (f : OntologyAnnotation list -> OntologyAnnotation list) (person : Person) =
        { person with 
            Roles = Option.mapDefault [] f person.Roles}
    
    /// Replaces roles of a person with the given roles
    let setRoles (person : Person) (roles : OntologyAnnotation list) =
        { person with
            Roles = Some roles }

    // Comments
    
    /// Returns comments of a person
    let getComments (person : Person) =
        person.Comments
    
    /// Applies function f on comments of a person
    let mapComments (f : Comment list -> Comment list) (person : Person) =
        { person with 
            Comments = Option.mapDefault [] f person.Comments}
    
    /// Replaces comments of a person by given comment list
    let setComments (person : Person) (comments : Comment list) =
        { person with
            Comments = Some comments }
