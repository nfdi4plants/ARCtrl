namespace ISADotNet

open System.Text.Json.Serialization

type Person = 
    {   
        [<JsonPropertyName(@"@id")>]
        ID : string
        [<JsonPropertyName(@"lastName")>]
        LastName : string
        [<JsonPropertyName(@"firstName")>]
        FirstName : string
        [<JsonPropertyName(@"midInitials")>]
        MidInitials : string
        [<JsonPropertyName(@"email")>]
        EMail : string
        [<JsonPropertyName(@"phone")>]
        Phone : string
        [<JsonPropertyName(@"fax")>]
        Fax : string
        [<JsonPropertyName(@"address")>]
        Address : string
        [<JsonPropertyName(@"affiliation")>]
        Affiliation : string
        [<JsonPropertyName(@"roles")>]
        Roles : OntologyAnnotation list
        [<JsonPropertyName(@"comments")>]
        Comments : Comment list  
    }

    static member create id lastName firstName midInitials email phone fax address affiliation roles comments : Person =
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


    static member LastNameTab = "Last Name"
    static member FirstNameTab = "First Name"
    static member MidInitialsTab = "Mid Initials"
    static member EmailTab = "Email"
    static member PhoneTab = "Phone"
    static member FaxTab = "Fax"
    static member AddressTab = "Address"
    static member AffiliationTab = "Affiliation"
    static member RolesTab = "Roles"
    static member RolesTermAccessionNumberTab = "Roles Term Accession Number"
    static member RolesTermSourceREFTab = "Roles Term Source REF"
