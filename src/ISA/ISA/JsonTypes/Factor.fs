namespace ISA

type Factor = 
    {
        ID : URI option
        Name : string option
        FactorType : OntologyAnnotation option
        Comments : Comment list option
    }

    static member make id name factorType comments =
        {
            ID      = id
            Name    = name
            FactorType = factorType
            Comments = comments         
        }

    static member create(?Id,?Name,?FactorType,?Comments) : Factor =
        Factor.make Id Name FactorType Comments

    static member empty =
        Factor.create()

    /// Create a ISAJson Factor from ISATab string entries
    static member fromString (name : string) (term:string) (source:string) (accession:string) =
        let oa = OntologyAnnotation.fromString (term, source, accession)
        Factor.make None (Option.fromValueWithDefault "" name) (Option.fromValueWithDefault OntologyAnnotation.empty oa) None

    /// Create a ISAJson Ontology Annotation value from ISATab string entries
    static member fromStringWithComments (name:string) (term:string) (source:string) (accession:string) (comments : Comment list) =
        let oa = OntologyAnnotation.fromStringWithComments term source accession comments
        Factor.make None (Option.fromValueWithDefault "" name) (Option.fromValueWithDefault OntologyAnnotation.empty oa) None

    /// Get ISATab string entries from an ISAJson Factor object
    static member toString (factor : Factor) =
        factor.FactorType |> Option.map OntologyAnnotation.toString |> Option.defaultValue {|TermName = ""; TermAccessionNumber = ""; TermSourceREF = ""|}

    member this.NameText =
        this.Name
        |> Option.defaultValue ""

    member this.MapCategory(f : OntologyAnnotation -> OntologyAnnotation) =
        {this with FactorType = Option.map f this.FactorType}

    member this.SetCategory(c : OntologyAnnotation) =
        {this with FactorType = Some c}

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            "OA " + this.NameText