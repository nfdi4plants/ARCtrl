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

    static member empty =
        Person.create None None None None None None None None None None None
