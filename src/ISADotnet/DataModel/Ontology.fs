namespace ISADotNet

open System.Text.Json.Serialization

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

    static member create id name termAccessionNumber termSourceREF comments= 
        {
            ID = id
            Name = name 
            TermSourceREF = termSourceREF
            TermAccessionNumber = termAccessionNumber  
            Comments = comments
        }

    static member empty =
        OntologyAnnotation.create None None None None None

    static member Create(?Id,?Name,?TermAccessionNumber,?TermSourceREF,?Comments) =
        OntologyAnnotation.create Id Name TermAccessionNumber TermSourceREF Comments

    /// Returns the name of the ontology as string
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
    member this.NameAsStringWithNumber =       
        let number = this.Comments |> Option.bind (List.tryPick (fun c -> if c.Name = Some "Number" then c.Value else None))
        let name = this.NameAsString
        match number with
        | Some n -> name + " #" + n
        | None -> name


    /// Create a ISAJson Ontology Annotation value from ISATab string entries
    static member fromString (term:string) (accession:string) (source:string) =
        OntologyAnnotation.create 
            None 
            (Option.fromValueWithDefault "" term |> Option.map AnnotationValue.fromString)
            (Option.fromValueWithDefault "" accession |> Option.map URI.fromString)
            (Option.fromValueWithDefault "" source)
            None

    /// Get a ISATab string entries from an ISAJson Ontology Annotation object (name,accession,source)
    static member toString (oa : OntologyAnnotation) =
        oa.Name |> Option.map AnnotationValue.toString |> Option.defaultValue "",
        oa.TermAccessionNumber |> Option.map URI.toString |> Option.defaultValue "",
        oa.TermSourceREF |> Option.defaultValue ""


    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            "OA " + this.NameAsStringWithNumber


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

    static member create description file name version comments  =
        {

            Description = description
            File        = file
            Name        = name
            Version     = version
            Comments    = comments
        }

    static member empty =
        OntologySourceReference.create None None None None None

    static member Create(?Description,?File,?Name,?Version,?Comments) =
        OntologySourceReference.create Description File Name Version Comments