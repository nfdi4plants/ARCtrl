namespace ARCtrl

open ARCtrl.Helper
open Fable.Core

[<AttachMembers>]
type Person(?orcid, ?lastName, ?firstName, ?midInitials, ?email, ?phone, ?fax, ?address, ?affiliation, ?roles, ?comments) =

    let mutable _orcid  : string option = orcid
    let mutable _lastName : string option = lastName
    let mutable _firstName : string option = firstName
    let mutable _midInitials : string option  = midInitials
    let mutable _email : string option = email
    let mutable _phone : string option = phone
    let mutable _fax : string option = fax
    let mutable _address : string option  = address
    let mutable _affiliation : string option = affiliation
    let mutable _roles : ResizeArray<OntologyAnnotation> = roles |> Option.defaultValue (ResizeArray())
    let mutable _comments : ResizeArray<Comment> = comments |> Option.defaultValue (ResizeArray())

    member this.ORCID
        with get() = _orcid
        and set(orcid) = _orcid <- orcid

    member this.FirstName 
        with get() = _firstName
        and set(firstName) = _firstName <- firstName

    member this.LastName
        with get() = _lastName
        and set(lastName) = _lastName <- lastName

    member this.MidInitials
        with get() = _midInitials
        and set(midInitials) = _midInitials <- midInitials

    member this.Address
        with get() = _address
        and set(address) = _address <- address

    member this.Affiliation
        with get() = _affiliation
        and set(affiliation) = _affiliation <- affiliation

    member this.EMail
        with get() = _email
        and set(email) = _email <- email

    member this.Phone
        with get() = _phone
        and set(phone) = _phone <- phone

    member this.Fax
        with get() = _fax
        and set(fax) = _fax <- fax

    member this.Roles
        with get() = _roles
        and set(roles) = _roles <- roles

    member this.Comments
        with get() = _comments
        and set(comments) = _comments <- comments


    static member make orcid lastName firstName midInitials email phone fax address affiliation roles comments : Person =
        Person(
            ?orcid=orcid,
            ?lastName=lastName, 
            ?firstName=firstName, 
            ?midInitials=midInitials,
            ?email=email,
            ?phone=phone,
            ?fax=fax,
            ?address=address,
            ?affiliation=affiliation,
            roles=roles,
            comments=comments
        )

    static member create (?orcid,?lastName,?firstName,?midInitials,?email,?phone,?fax,?address,?affiliation,?roles,?comments) : Person =
        Person.make orcid lastName firstName midInitials email phone fax address affiliation (Option.defaultValue (ResizeArray()) roles) (Option.defaultValue (ResizeArray()) comments)

    static member empty =
        Person.create ()

    static member tryGetByFullName (firstName : string) (midInitials : string) (lastName : string) (persons : Person []) =
        persons
        |> Array.tryFind (fun p -> 
            if midInitials = "" then 
                p.FirstName = Some firstName && p.LastName = Some lastName
            else 

                p.FirstName = Some firstName && p.MidInitials = Some midInitials && p.LastName = Some lastName
        ) 

    ///// Returns true, if a person for which the predicate returns true exists in the investigation
    //static member exists (predicate : Person -> bool) (investigation:Investigation) =
    //    investigation.Contacts
    //    |> List.exists (predicate) 

    ///// Returns true, if the given person exists in the investigation
    //static member contains (person : Person) (investigation:Investigation) =
    //    exists ((=) person) investigation

    /// If an person with the given FirstName, MidInitials and LastName exists in the list, returns true
    static member existsByFullName (firstName : string) (midInitials : string) (lastName : string) (persons : Person []) =
        Array.exists (fun (p : Person) -> 
            if midInitials = "" then 
                p.FirstName = Some firstName && p.LastName = Some lastName
            else 

                p.FirstName = Some firstName && p.MidInitials = Some midInitials && p.LastName = Some lastName
        ) persons

    /// adds the given person to the persons  
    static member add (persons : Person list) (person : Person) =
        List.append persons [person]
    
    /// If a person with the given FirstName, MidInitials and LastName exists in the list, removes it
    static member removeByFullName (firstName : string) (midInitials : string) (lastName : string) (persons : Person []) =
        Array.filter (fun (p : Person) ->
            if midInitials = "" then
                (p.FirstName = Some firstName && p.LastName = Some lastName)
                |> not
            else
                (p.FirstName = Some firstName && p.MidInitials = Some midInitials && p.LastName = Some lastName)
                |> not
        ) persons



    member this.Copy() : Person =
        let nextComments = this.Comments |> ResizeArray.map (fun c -> c.Copy())
        let nextRoles = this.Roles |> ResizeArray.map (fun c -> c.Copy())
        Person.make 
            this.ORCID
            this.LastName 
            this.FirstName 
            this.MidInitials 
            this.EMail 
            this.Phone 
            this.Fax 
            this.Address 
            this.Affiliation 
            nextRoles
            nextComments

    override this.GetHashCode() : int = 
        [|
            HashCodes.boxHashOption this.ORCID
            HashCodes.boxHashOption this.LastName
            HashCodes.boxHashOption this.FirstName
            HashCodes.boxHashOption this.MidInitials
            HashCodes.boxHashOption this.EMail
            HashCodes.boxHashOption this.Phone
            HashCodes.boxHashOption this.Fax
            HashCodes.boxHashOption this.Address
            HashCodes.boxHashOption this.Affiliation
            HashCodes.boxHashSeq this.Roles
            HashCodes.boxHashSeq this.Comments
        |]
        |> HashCodes.boxHashArray
        |> fun x -> x :?> int

    override this.Equals(obj) : bool =
        HashCodes.hash this = HashCodes.hash obj

    // Use this for better print debugging and better unit test output
    override this.ToString() =
        let sb = System.Text.StringBuilder()
        sb.Append("Person {\n\t") |> ignore
        [
            "FirstName", this.FirstName
            "LastName", this.LastName
            "MidInitials", this.MidInitials
            "EMail", this.EMail
            "Phone", this.Phone
            "Address", this.Address
            "Affiliation", this.Affiliation
            "Fax", this.Fax
            "ORCID", this.ORCID
            "Roles", 
                if Seq.length this.Roles > 0 then 
                    this.Roles
                    |> sprintf "%A"
                    |> Some
                else
                    None
            "Comments", 
                if Seq.length this.Comments > 0 then 
                    this.Comments
                    |> sprintf "%A"
                    |> Some
                else
                    None
        ] 
        |> List.choose (fun (s,opt) -> opt |> Option.map (fun o -> s,o))
        |> List.map (fun (s,v) -> sprintf "%s = %A" s v)
        |> String.concat ",\n\t"
        |> sb.Append
        |> ignore
        sb.Append("\n}") |> ignore
        sb.ToString()