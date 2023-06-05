namespace ISA

[<CustomEquality; NoComparison>]
type OntologyAnnotation =
    {
        ID : URI option
        Name : AnnotationValue option
        TermSourceREF : string option
        TermAccessionNumber : URI option
        Comments : Comment list option
    }

    static member make id name termSourceREF termAccessionNumber comments= 
        {
            ID = id
            Name = name 
            TermSourceREF = termSourceREF
            TermAccessionNumber = termAccessionNumber  
            Comments = comments
        }

    static member create(?Id,?Name,?TermSourceREF,?TermAccessionNumber,?Comments) : OntologyAnnotation =
        OntologyAnnotation.make Id Name TermSourceREF TermAccessionNumber Comments

    static member empty =
        OntologyAnnotation.create()

    /// Returns the name of the ontology as string
    // TODO: Why is this called Text, while everything else is called string?
    member this.NameText =
        this.Name
        |> Option.map (fun av ->
            match av with
            | AnnotationValue.Text s  -> s
            | AnnotationValue.Float f -> string f
            | AnnotationValue.Int i   -> string i
        )
        |> Option.defaultValue ""

    /// Returns the name of the ontology as string
    member this.TryNameText =
        this.Name
        |> Option.map (fun av ->
            match av with
            | AnnotationValue.Text s  -> s
            | AnnotationValue.Float f -> string f
            | AnnotationValue.Int i   -> string i
        )

    /// Returns the term source of the ontology as string
    member this.TermSourceREFString =       
        this.TermSourceREF
        |> Option.defaultValue ""

    /// Returns the term accession number of the ontology as string
    member this.TermAccessionString =       
        this.TermAccessionNumber
        |> Option.defaultValue ""

    /// Create a path in form of `http://purl.obolibrary.org/obo/MS_1000121` from it's Term Accession Source `MS` and Term Accession Number `1000121`. 
    static member createUriAnnotation (termSourceRef : string) (termAccessionNumber : string) =
        $"{Url.OntobeeOboPurl}{termSourceRef}_{termAccessionNumber}"

    /// Create a ISAJson Ontology Annotation value from ISATab string entries, will try to reduce `termAccessionNumber` with regex matching.
    ///
    /// Exmp. 1: http://purl.obolibrary.org/obo/GO_000001 --> GO:000001
    static member fromString (term:string, ?termSourceRef:string, ?termAccessionString:string, ?comments : Comment list) =

        let termAccession = 
            if termAccessionString.IsSome then
                let termAccessionString = termAccessionString.Value
                let regexResult = Regex.tryGetTermAccessionString termAccessionString
                regexResult 
                |> Option.defaultValue termAccessionString
                |> Some 
            else
                None

        OntologyAnnotation.make 
            None 
            (Some term |> Option.map AnnotationValue.fromString)
            (termSourceRef)
            (termAccession)
            (comments)

    /// Will always be created without `OntologyAnnotion.Name`
    static member fromTermAccession (termAccession : string) =
        termAccession
        |> Regex.tryParseTermAccession
        |> Option.get 
        |> fun r ->
            let accession = r.IdSpace + ":" + r.LocalId
            OntologyAnnotation.fromString ("", r.IdSpace, accession)

    /// Parses any value in `TermAccessionString` to term accession format "idspace:localid". Exmp.: "MS:000001".
    ///
    /// If `TermAccessionString` cannot be parsed to this format, returns empty string!
    member this.TermAccessionShort = 
        match Regex.tryGetTermAccessionString this.TermAccessionString with
        | Some s -> s
        | None -> ""

    member this.TermAccessionOntobeeUrl = 
        match this.TermSourceREF, this.TermAccessionNumber with
        | Some tsr, Some tan ->
            OntologyAnnotation.createUriAnnotation tsr tan
        | None, Some tan ->
            match Regex.tryParseTermAccession tan with 
            | Some termAccession -> OntologyAnnotation.createUriAnnotation termAccession.IdSpace termAccession.LocalId
            | None -> ""
        | _ -> ""

    /// <summary>
    /// Get a ISATab string entries from an ISAJson Ontology Annotation object (name,source,accession)
    ///
    /// `asOntobeePurlUrl`: option to return term accession in Ontobee purl-url format (`http://purl.obolibrary.org/obo/MS_1000121`)
    /// </summary>
    static member toString (oa : OntologyAnnotation, ?asOntobeePurlUrl: bool) =
        let asOntobeePurlUrl = Option.defaultValue false asOntobeePurlUrl
        {|
            TermName = oa.Name |> Option.map AnnotationValue.toString |> Option.defaultValue ""
            TermSourceREF = oa.TermSourceREF |> Option.defaultValue ""
            TermAccessionNumber = 
                if asOntobeePurlUrl then
                    oa.TermAccessionOntobeeUrl
                else
                    oa.TermAccessionNumber |> Option.defaultValue ""
        |}

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            "OA " + this.NameText

    override this.Equals other =
        match other with
        | :? OntologyAnnotation as oa -> (this :> System.IEquatable<_>).Equals oa
        | :? string as s ->           
            this.NameText = s
            || 
            this.TermAccessionShort = s
            ||
            this.TermAccessionOntobeeUrl = s
        | _ -> false

    override this.GetHashCode () = (this.NameText+this.TermAccessionShort).GetHashCode()

    interface System.IEquatable<OntologyAnnotation> with
        member this.Equals other =
            if this.TermAccessionNumber.IsSome && other.TermAccessionNumber.IsSome then
                other.TermAccessionShort = this.TermAccessionShort
                ||
                other.TermAccessionOntobeeUrl = this.TermAccessionOntobeeUrl
            elif this.Name.IsSome && other.Name.IsSome then
                other.NameText = this.NameText
            elif this.TermAccessionNumber.IsNone && other.TermAccessionNumber.IsNone && this.Name.IsNone && other.Name.IsNone then
                true
            else 
                false