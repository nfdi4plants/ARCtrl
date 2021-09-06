namespace ISADotNet

open System.Text.Json.Serialization

type Person = 
    {   
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"lastName")>]
        LastName : string option
        [<JsonPropertyName(@"firstName")>]
        FirstName : string option
        [<JsonPropertyName(@"midInitials")>]
        MidInitials : string option
        [<JsonPropertyName(@"email")>]
        EMail : EMail option
        [<JsonPropertyName(@"phone")>]
        Phone : string option
        [<JsonPropertyName(@"fax")>]
        Fax : string option
        [<JsonPropertyName(@"address")>]
        Address : string option
        [<JsonPropertyName(@"affiliation")>]
        Affiliation : string option
        [<JsonPropertyName(@"roles")>]
        Roles : OntologyAnnotation list option
        [<JsonPropertyName(@"comments")>] 
        Comments : Comment list option  
    }

    static member create (?Id,?LastName,?FirstName,?MidInitials,?Email,?Phone,?Fax,?Address,?Affiliation,?Roles,?Comments) : Person =
        {
            ID          = Id
            LastName    = LastName
            FirstName   = FirstName
            MidInitials = MidInitials
            EMail       = Email
            Phone       = Phone
            Fax         = Fax
            Address     = Address
            Affiliation = Affiliation
            Roles       = Roles
            Comments    = Comments
        }

    static member empty =
        Person.create ()
