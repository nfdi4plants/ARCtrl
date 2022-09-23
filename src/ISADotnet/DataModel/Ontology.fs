namespace ISADotNet

open System.Text.Json.Serialization
open System.Text.RegularExpressions

[<AnyOf>]
type AnnotationValue = 
    | [<SerializationOrder(2)>]Text of string
    | [<SerializationOrder(1)>]Float of float
    | [<SerializationOrder(0)>]Int of int

    static member empty = Text ""

    /// Create a ISAJson Annotation value from a ISATab string entry
    static member fromString (s : string) = 
        try s |> int |> AnnotationValue.Int
        with | _ -> 
            try s |> float |> AnnotationValue.Float
            with
            | _ -> AnnotationValue.Text s

    /// Get a ISATab string Annotation Name from a ISAJson object
    static member toString (v : AnnotationValue) = 
        match v with
        | Text s -> s
        | Int i -> string i
        | Float f -> string f

[<CustomEquality; NoComparison>]
type OntologyAnnotation =
    {
        [<JsonPropertyName("@id")>]
        ID : URI option
        [<JsonPropertyName("annotationValue")>]
        Name : AnnotationValue option
        [<JsonPropertyName("termSource")>]
        TermSourceREF : string option
        [<JsonPropertyName("termAccession")>]
        TermAccessionNumber : URI option
        [<JsonPropertyName("comments")>]
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
    [<System.Obsolete("This function is deprecated. Use the member \"GetName\" instead.")>]
    member this.NameAsString =
        this.Name
        |> Option.map (fun oa ->
            match oa with
            | AnnotationValue.Text s  -> s
            | AnnotationValue.Float f -> string f
            | AnnotationValue.Int i   -> string i
        )
        |> Option.defaultValue ""

    /// Returns the name of the ontology with the number as string (e.g. "temperature #2")
    [<System.Obsolete("This function is deprecated. Use the member \"GetNameWithNumber\" instead.")>]
    member this.NameAsStringWithNumber =       
        let number = this.Comments |> Option.bind (List.tryPick (fun c -> if c.Name = Some "Number" then c.Value else None))
        let name = this.NameAsString
        match number with
        | Some n -> name + " #" + n
        | None -> name

    /// Returns the name of the ontology as string
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
    [<System.Obsolete("This function is deprecated. Use the member \"NameText\" instead.")>]
    member this.GetName = this.NameText

    /// Returns number of Ontology annotation
    [<System.Obsolete("This function is deprecated. Numbering support will soon be dropped")>]
    member this.Number =       
        this.Comments |> Option.bind (List.tryPick (fun c -> if c.Name = Some "Number" then c.Value else None))       

    /// Returns the name of the ontology with the number as string (e.g. "temperature #2")
    [<System.Obsolete("This function is deprecated. Numbering support will soon be dropped")>]
    member this.GetNameWithNumber =       
        let name = this.GetName
        match this.Number with
        | Some n -> name + " #" + n
        | None -> name

    /// Returns the term source of the ontology as string
    member this.TermSourceREFString =       
        this.TermSourceREF
        |> Option.defaultValue ""

    /// Returns the term accession number of the ontology as string
    member this.TermAccessionString =       
        this.TermAccessionNumber
        |> Option.defaultValue ""

    static member shortAnnotationRegex = "(?<ref>\\w*):(?<num>\\w*)"
    static member ontologyTermURIRegex = ".*/(?<ref>\\w*)_(?<num>\\w*)"
    
    /// Tries to split a term Path in form of `http://purl.obolibrary.org/obo/MS_1000121` into it's Term Accession Source `MS` and Term Accession Number `1000121`. 
    static member trySplitUri (termAccessionPath : string) =
        let r = Regex.Match(termAccessionPath,OntologyAnnotation.ontologyTermURIRegex)
        if r.Success then 
            Some (r.Groups.Item("ref").Value,r.Groups.Item("num").Value)
        else 
            None

    /// Tries to split a term id in form of `MS:1000121` into it's Term Accession Source `MS` and Term Accession Number `1000121`. 
    static member trySplitShortAnnotation (termAccessionPath : string) =
        let r = Regex.Match(termAccessionPath,OntologyAnnotation.shortAnnotationRegex)
        if r.Success then 
            Some (r.Groups.Item("ref").Value,r.Groups.Item("num").Value)
        else 
            None

    /// Create a path in form of `http://purl.obolibrary.org/obo/MS_1000121`from it's Term Accession Source `MS` and Term Accession Number `1000121`. 
    static member createUriAnnotation (termSourceRef : string) (termAccessionNumber : string) =
        let r = Regex.Match(termAccessionNumber,OntologyAnnotation.ontologyTermURIRegex)
        let r2 = Regex.Match(termAccessionNumber,OntologyAnnotation.shortAnnotationRegex)

        if r.Success then
            let termSourceRef = r.Groups.Item("ref").Value
            let termAccessionNumber = r.Groups.Item("num").Value
            $"http://purl.obolibrary.org/obo/{termSourceRef}_{termAccessionNumber}"
        elif r2.Success then
            let termSourceRef = r.Groups.Item("ref").Value
            let termAccessionNumber = r.Groups.Item("num").Value
            $"http://purl.obolibrary.org/obo/{termSourceRef}_{termAccessionNumber}"
        else
            $"http://purl.obolibrary.org/obo/{termSourceRef}_{termAccessionNumber}"

    /// Creates an annotation of format `TermSourceRef:TermAccessionNumber` (e.g: `MS:1000690`)
    /// 
    /// If termAccessionNumber is given in full URI form `http://purl.obolibrary.org/obo/MS_1000121`, takes last part of it. 
    static member createShortAnnotation (termSourceRef : string) (termAccessionNumber : string) =
        let r = Regex.Match(termAccessionNumber,OntologyAnnotation.ontologyTermURIRegex)
        let r2 = Regex.Match(termAccessionNumber,OntologyAnnotation.shortAnnotationRegex)
        
        if r.Success then
            let termSourceRef = r.Groups.Item("ref").Value
            let termAccessionNumber = r.Groups.Item("num").Value
            $"{termSourceRef}:{termAccessionNumber}"
        elif r2.Success then
            let termSourceRef = r.Groups.Item("ref").Value
            let termAccessionNumber = r.Groups.Item("num").Value
            $"{termSourceRef}:{termAccessionNumber}"
        else
            $"{termSourceRef}:{termAccessionNumber}"
        
    /// Splits the Annotation of format `TermSourceRef:TermAccessionNumber` (e.g: `MS:1000690`) into a tuple of TermSourceRef*TermAccessionNumber 
    static member splitAnnotation (a : string) = 
        a.Split [|';';'_';':'|]
        |> fun a -> a.[0],a.[1]

    /// Create a ISAJson Ontology Annotation value from ISATab string entries
    static member fromString (term:string) (source:string) (accessionNumber:string) =
       
        OntologyAnnotation.make 
            None 
            (Option.fromValueWithDefault "" term |> Option.map AnnotationValue.fromString)
            (Option.fromValueWithDefault "" source)
            (Option.fromValueWithDefault "" accessionNumber)
            None

    /// Create a ISAJson Ontology Annotation value from ISATab string entries
    static member fromStringWithComments (term:string) (source:string) (accessionNumber:string) (comments : Comment list) =
        
        OntologyAnnotation.make 
            None 
            (Option.fromValueWithDefault "" term |> Option.map AnnotationValue.fromString)
            (Option.fromValueWithDefault "" source)
            (Option.fromValueWithDefault "" accessionNumber |> Option.map URI.fromString)
            (Option.fromValueWithDefault [] comments)

    static member fromAnnotationId (id : string) =
        id 
        |> OntologyAnnotation.splitAnnotation
        |> fun (source,num) ->
            OntologyAnnotation.fromString "" source id

    /// Create a ISAJson Ontology Annotation value from string entries, where the term name can contain a # separated number. e.g: "temperature unit #2"
    [<System.Obsolete("This function is deprecated. Numbering support will soon be dropped")>]
    static member fromStringWithNumber (term:string) (source:string) (accession:string) =
        let t,number = 
            let lastIndex = term.LastIndexOf '#'
            term.Remove(lastIndex).Trim(), term.Substring(lastIndex+1).Trim()
        let numberComment = Option.fromValueWithDefault "" number |> Option.map ((fun n -> Comment.create(Name = "Number", Value = n)) >> List.singleton)
        OntologyAnnotation.fromStringWithComments t source accession (numberComment |> Option.defaultValue [])      

    /// Create a ISAJson Ontology Annotation value from string entries, where the term name can contain a # separated number. e.g: "temperature unit #2"
    [<System.Obsolete("This function is deprecated. Numbering support will soon be dropped")>]
    static member fromStringWithNumberAndComments (term:string) (source:string) (accession:string) (comments : Comment list) =
        let t,number = 
            let lastIndex = term.LastIndexOf '#'
            term.Remove(lastIndex).Trim(), term.Substring(lastIndex+1).Trim()
        let numberComment = Option.fromValueWithDefault "" number |> Option.map ((fun n -> Comment.create(Name = "Number", Value = n)) >> List.singleton)
        OntologyAnnotation.fromStringWithComments term source accession ((numberComment |> Option.defaultValue []) @ comments)   

    member this.ShortAnnotationString = 
        match this.TermAccessionNumber with
        | Some t ->
            match OntologyAnnotation.trySplitUri t with 
            | Some (s,t) -> OntologyAnnotation.createShortAnnotation s t
            | None -> 
                if System.Text.RegularExpressions.Regex.Match(t,OntologyAnnotation.shortAnnotationRegex).Success then t
                else ""
        | None -> ""

    member this.URLAnnotationString = 
        match this.TermAccessionNumber with
        | Some t ->
            match OntologyAnnotation.trySplitUri t with 
            | Some _ -> t
            | None -> 
                let r = System.Text.RegularExpressions.Regex.Match(t,OntologyAnnotation.shortAnnotationRegex)
                if r.Success then t
                else ""
        | None -> ""


    /// Get a ISATab string entries from an ISAJson Ontology Annotation object (name,source,accession)
    static member toString (oa : OntologyAnnotation) =
        oa.Name |> Option.map AnnotationValue.toString |> Option.defaultValue "",
        oa.TermSourceREF |> Option.defaultValue "",
        oa.TermAccessionNumber |> Option.defaultValue ""

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            "OA " + this.NameText

    override this.Equals other =
        match other with
        | :? OntologyAnnotation as oa -> (this :> System.IEquatable<_>).Equals oa
        | _ -> false

    override this.GetHashCode () = (this.NameText+this.ShortAnnotationString).GetHashCode()

    interface System.IEquatable<OntologyAnnotation> with
        member this.Equals other =
            if this.TermAccessionNumber.IsSome && other.TermAccessionNumber.IsSome then
                other.ShortAnnotationString = this.ShortAnnotationString
            elif this.Name.IsSome && other.Name.IsSome then
                other.NameText = this.NameText
            elif this.TermAccessionNumber.IsNone && other.TermAccessionNumber.IsNone && this.Name.IsNone && other.Name.IsNone then
                true
            else 
                false

type OntologySourceReference =
    {
        [<JsonPropertyName("description")>]
        Description : string option
        [<JsonPropertyName("file")>]
        File : string option
        [<JsonPropertyName("name")>]
        Name : string option
        [<JsonPropertyName("version")>]
        Version : string option
        [<JsonPropertyName("comments")>]
        Comments : Comment list option
    }

    static member make description file name version comments  =
        {

            Description = description
            File        = file
            Name        = name
            Version     = version
            Comments    = comments
        }

    static member create(?Description,?File,?Name,?Version,?Comments) : OntologySourceReference =
        OntologySourceReference.make Description File Name Version Comments

    static member empty =
        OntologySourceReference.create()