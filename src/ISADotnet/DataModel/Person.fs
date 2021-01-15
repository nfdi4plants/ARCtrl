namespace ISADotNet

open System.Text.Json.Serialization

type Person = 
    {   
        [<JsonPropertyName(@"@id")>]
        ID : URI
        [<JsonPropertyName(@"lastName")>]
        LastName : string
        [<JsonPropertyName(@"firstName")>]
        FirstName : string
        [<JsonPropertyName(@"midInitials")>]
        MidInitials : string
        [<JsonPropertyName(@"email")>]
        EMail : EMail
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
