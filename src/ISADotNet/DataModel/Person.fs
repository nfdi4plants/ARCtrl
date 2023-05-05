namespace ISADotNet


type Person = 
    {   
        ID : URI option
        LastName : string option
        FirstName : string option
        MidInitials : string option
        EMail : EMail option
        Phone : string option
        Fax : string option
        Address : string option
        Affiliation : string option
        Roles : OntologyAnnotation list option
        Comments : Comment list option  
    }

    static member make id lastName firstName midInitials email phone fax address affiliation roles comments : Person =
        {
            ID = id
            LastName = lastName
            FirstName = firstName
            MidInitials = midInitials
            EMail = email
            Phone = phone
            Fax = fax
            Address = address
            Affiliation = affiliation
            Roles = roles
            Comments = comments
        }

    static member create (?Id,?LastName,?FirstName,?MidInitials,?Email,?Phone,?Fax,?Address,?Affiliation,?Roles,?Comments) : Person =
        Person.make Id LastName FirstName MidInitials Email Phone Fax Address Affiliation Roles Comments

    static member empty =
        Person.create ()
