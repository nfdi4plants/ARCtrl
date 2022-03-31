namespace ISADotNet

open System.Text.Json.Serialization

type Factor = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"factorName")>]
        Name : string option
        [<JsonPropertyName(@"factorType")>]
        FactorType : OntologyAnnotation option
        [<JsonPropertyName(@"comments")>]
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
        let oa = OntologyAnnotation.fromString term source accession
        Factor.make None (Option.fromValueWithDefault "" name) (Option.fromValueWithDefault OntologyAnnotation.empty oa) None

    /// Create a ISAJson Ontology Annotation value from string entries, where the term name can contain a # separated number. e.g: "temperature unit #2"
    static member fromStringWithNumber (name:string) (term:string) (source:string) (accession:string) =
        let oa = OntologyAnnotation.fromStringWithNumber term source accession
        Factor.make None (Option.fromValueWithDefault "" name) (Option.fromValueWithDefault OntologyAnnotation.empty oa) None

    /// Create a ISAJson Ontology Annotation value from ISATab string entries
    static member fromStringWithComments (name:string) (term:string) (source:string) (accession:string) (comments : Comment list) =
        let oa = OntologyAnnotation.fromStringWithComments term source accession comments
        Factor.make None (Option.fromValueWithDefault "" name) (Option.fromValueWithDefault OntologyAnnotation.empty oa) None

    /// Create a ISAJson Ontology Annotation value from ISATab string entries
    static member fromStringWithNumberAndComments (name:string) (term:string) (source:string) (accession:string) (comments : Comment list) =
        let oa = OntologyAnnotation.fromStringWithNumberAndComments term source accession comments
        Factor.make None (Option.fromValueWithDefault "" name) (Option.fromValueWithDefault OntologyAnnotation.empty oa) None

    /// Get ISATab string entries from an ISAJson Factor object
    static member toString (factor : Factor) =
        factor.FactorType |> Option.map OntologyAnnotation.toString |> Option.defaultValue ("","","")  

    /// Returns the name of the factor as string
    [<System.Obsolete("This function is deprecated. Use the member \"GetName\" instead.")>]
    member this.NameAsString =
        this.Name
        |> Option.defaultValue ""

    /// Returns the name of the factor with the number as string (e.g. "temperature #2")
    [<System.Obsolete("This function is deprecated. Use the member \"GetNameWithNumber\" instead.")>]
    member this.NameAsStringWithNumber =       
        this.FactorType
        |> Option.map (fun oa -> oa.GetNameWithNumber)
        |> Option.defaultValue ""

    /// Returns the name of the factor as string
    member this.GetName =
        this.Name
        |> Option.defaultValue ""

    /// Returns the name of the factor with the number as string (e.g. "temperature #2")
    member this.GetNameWithNumber =     
        this.FactorType
        |> Option.map (fun oa -> oa.GetName)
        |> Option.defaultValue ""

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            "OA " + this.GetNameWithNumber

[<AnyOf>]
type Value =
    | [<SerializationOrder(0)>] Ontology of OntologyAnnotation
    | [<SerializationOrder(1)>] Int of int
    | [<SerializationOrder(2)>] Float of float
    | [<SerializationOrder(3)>] Name of string

    static member fromOptions (value : string Option) (termSource: string Option) (termAccesssion: string Option) =
        match value, termSource, termAccesssion with
        | Some value, None, None ->
            try Value.Int (int value)
            with
            | _ -> 
                try Value.Float (float value)
                with
                | _ -> Value.Name value
            |> Some
        | None, None, None -> 
            None
        | _ -> 
            OntologyAnnotation.fromString (Option.defaultValue "" value) (Option.defaultValue "" termAccesssion) (Option.defaultValue "" termSource)
            |> Value.Ontology
            |> Some

    static member toOptions (value : Value) =
        match value with
        | Ontology oa -> oa.Name |> Option.map AnnotationValue.toString,oa.TermAccessionNumber,oa.TermSourceREF
        | Int i -> string i |> Some, None, None
        | Float f -> string f |> Some, None, None
        | Name s -> s |> Some, None, None

    member this.AsString =         
        match this with
        | Value.Ontology oa  -> oa.GetName
        | Value.Float f -> string f
        | Value.Int i   -> string i
        | Value.Name s  -> s

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            match this with
            | Ontology oa   -> oa.GetName
            | Int i         -> sprintf "%i" i
            | Float f       -> sprintf "%f" f        
            | Name n        -> n

type FactorValue =
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"category")>]
        Category : Factor option
        [<JsonPropertyName(@"value")>]
        Value : Value option
        [<JsonPropertyName(@"unit")>]
        Unit : OntologyAnnotation option
    }

    static member make id category value unit =
        {
            ID      = id
            Category = category
            Value = value
            Unit = unit         
        }

    static member create(?Id,?Category,?Value,?Unit) : FactorValue =
        FactorValue.make Id Category Value Unit

    static member empty =
        FactorValue.create()

    member this.GetValue =
        this.Value
        |> Option.map (fun oa ->
            match oa with
            | Value.Ontology oa  -> oa.GetName
            | Value.Float f -> string f
            | Value.Int i   -> string i
            | Value.Name s  -> s
        )
        |> Option.defaultValue ""

    member this.GetValueWithUnit =
        let unit = 
            this.Unit |> Option.map (fun oa -> oa.GetName)
        let v = this.GetValue
        match unit with
        | Some u    -> sprintf "%s %s" v u
        | None      -> v

    /// Returns the name of the category as string
    member this.GetName =
        this.Category
        |> Option.map (fun factor -> factor.GetName)
        |> Option.defaultValue ""

    /// Returns the name of the category with the number as string (e.g. "temperature #2")
    member this.GetNameWithNumber =       
        this.Category
        |> Option.map (fun oa -> oa.GetNameWithNumber)
        |> Option.defaultValue ""

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            let category = this.Category |> Option.map (fun f -> f.GetName)
            let unit = this.Unit |> Option.map (fun oa -> oa.GetName)
            let value = 
                this.Value
                |> Option.map (fun v ->
                    let s = (v :> IISAPrintable).PrintCompact()
                    match unit with
                    | Some u -> s + " " + u
                    | None -> s
                )
            match category,value with
            | Some category, Some value -> category + ":" + value
            | Some category, None -> category + ":" + "No Value"
            | None, Some value -> value
            | None, None -> ""
