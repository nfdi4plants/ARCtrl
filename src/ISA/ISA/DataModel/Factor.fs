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
        let oa = OntologyAnnotation.fromString term source accession
        Factor.make None (Option.fromValueWithDefault "" name) (Option.fromValueWithDefault OntologyAnnotation.empty oa) None

    /// Create a ISAJson Ontology Annotation value from ISATab string entries
    static member fromStringWithComments (name:string) (term:string) (source:string) (accession:string) (comments : Comment list) =
        let oa = OntologyAnnotation.fromStringWithComments term source accession comments
        Factor.make None (Option.fromValueWithDefault "" name) (Option.fromValueWithDefault OntologyAnnotation.empty oa) None

    /// Get ISATab string entries from an ISAJson Factor object
    static member toString (factor : Factor) =
        factor.FactorType |> Option.map OntologyAnnotation.toString |> Option.defaultValue ("","","")  

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

type Value =
    | Ontology of OntologyAnnotation
    | Int of int
    | Float of float
    | Name of string

    static member fromString (value : string) =
        try Value.Int (int value)
        with
        | _ -> 
            try Value.Float (float value)
            with
            | _ -> Value.Name value

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
            OntologyAnnotation.fromString (Option.defaultValue "" value) (Option.defaultValue "" termSource) (Option.defaultValue "" termAccesssion)
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
        | Value.Ontology oa  -> oa.NameText
        | Value.Float f -> string f
        | Value.Int i   -> string i
        | Value.Name s  -> s

    member this.AsName() =         
        match this with
        | Value.Name s  -> s
        | _ -> failwith $"Value {this} is not of case name"

    member this.AsInt() =         
        match this with           
        | Value.Int i   -> i
        | _ -> failwith $"Value {this} is not of case int"

    member this.AsFloat() = 
        match this with
        | Value.Float f -> f
        | _ -> failwith $"Value {this} is not of case float"

    member this.AsOntology() =         
        match this with
        | Value.Ontology oa  -> oa
        | _ -> failwith $"Value {this} is not of case ontology"

    member this.IsAnOntology = 
        match this with
        | Ontology oa -> true
        | _ -> false

    member this.IsNumerical = 
        match this with
        | Int _ | Float _ -> true
        | _ -> false

    member this.IsAnInt = 
        match this with
        | Int _ -> true
        | _ -> false

    member this.IsAFloat = 
        match this with
        | Float _ -> true
        | _ -> false

    member this.IsAText = 
        match this with
        | Name _ -> true
        | _ -> false

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            match this with
            | Ontology oa   -> oa.NameText
            | Int i         -> sprintf "%i" i
            | Float f       -> sprintf "%f" f        
            | Name n        -> n

    
type FactorValue =
    {
        ID : URI option
        Category : Factor option
        Value : Value option
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

    member this.ValueText =
        this.Value
        |> Option.map (fun oa ->
            match oa with
            | Value.Ontology oa  -> oa.NameText
            | Value.Float f -> string f
            | Value.Int i   -> string i
            | Value.Name s  -> s
        )
        |> Option.defaultValue ""

    member this.ValueWithUnitText =
        let unit = 
            this.Unit |> Option.map (fun oa -> oa.NameText)
        let v = this.ValueText
        match unit with
        | Some u    -> sprintf "%s %s" v u
        | None      -> v

    member this.NameText =
        this.Category
        |> Option.map (fun factor -> factor.NameText)
        |> Option.defaultValue ""

    member this.MapCategory(f : OntologyAnnotation -> OntologyAnnotation) =
        {this with Category = this.Category |> Option.map (fun p -> p.MapCategory f) }

    member this.SetCategory(c : OntologyAnnotation) =
        {this with Category = 
                            match this.Category with
                            | Some p -> Some (p.SetCategory c)
                            | None -> Some (Factor.create(FactorType = c))
        }

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            let category = this.Category |> Option.map (fun f -> f.NameText)
            let unit = this.Unit |> Option.map (fun oa -> oa.NameText)
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
