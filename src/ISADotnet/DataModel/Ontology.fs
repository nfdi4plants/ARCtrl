namespace ISADotNet

open System.Text.Json.Serialization

[<AnyOf>]
type AnnotationValue = 
    | [<SerializationOrder(2)>]Text of string
    | [<SerializationOrder(1)>]Float of float
    | [<SerializationOrder(0)>]Int of int

    static member empty = Text ""

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