namespace ISADotNet

open System.Text.Json.Serialization

type MaterialAttribute = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"characteristicType")>]
        CharacteristicType : OntologyAnnotation option
    
    }
        
    static member create (?Id,?CharacteristicType) : MaterialAttribute =
        {
            ID                  = Id
            CharacteristicType  = CharacteristicType     
        }

    static member empty =
        MaterialAttribute.create()


    /// Create a ISAJson MaterialAttribute from ISATab string entries
    static member fromString (term:string) (accession:string) (source:string) =
        let oa = OntologyAnnotation.fromString term accession source
        MaterialAttribute.create (CharacteristicType = oa)

    /// Get ISATab string entries from an ISAJson MaterialAttribute object
    static member toString (ma : MaterialAttribute) =
        ma.CharacteristicType |> Option.map OntologyAnnotation.toString |> Option.defaultValue ("","","")    

    /// Returns the name of the characteristic as string
    [<System.Obsolete("This function is deprecated. Use the member \"GetName\" instead.")>]
    member this.NameAsString =
        this.CharacteristicType
        |> Option.map (fun oa -> oa.NameAsString)
        |> Option.defaultValue ""

    /// Returns the name of the characteristic with the number as string (e.g. "temperature #2")
    [<System.Obsolete("This function is deprecated. Use the member \"GetNameWithNumber\" instead.")>]
    member this.NameAsStringWithNumber =       
        this.CharacteristicType
        |> Option.map (fun oa -> oa.NameAsStringWithNumber)
        |> Option.defaultValue ""

    /// Returns the name of the characteristic as string
    member this.GetName =
        this.CharacteristicType
        |> Option.map (fun oa -> oa.GetName)
        |> Option.defaultValue ""

    /// Returns the name of the characteristic with the number as string (e.g. "temperature #2")
    member this.GetNameWithNumber =       
        this.CharacteristicType
        |> Option.map (fun oa -> oa.GetNameWithNumber)
        |> Option.defaultValue ""


    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            "OA " + this.GetNameWithNumber

type MaterialAttributeValue = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"category")>]
        Category : MaterialAttribute option
        [<JsonPropertyName(@"value")>]
        Value : Value option
        [<JsonPropertyName(@"unit")>]
        Unit : OntologyAnnotation option
    
    }

    static member create(?Id,?Category,?Value,?Unit) : MaterialAttributeValue =
        {
            ID          = Id
            Category    = Category
            Value       = Value
            Unit        = Unit         
        }

    static member empty =
        MaterialAttributeValue.create()

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
        | Some u    -> $"{v} {u}"
        | None      -> v

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

[<StringEnumAttribute>]
type MaterialType =
    | [<StringEnumValue("Extract Name")>]           ExtractName // "Extract Name"
    | [<StringEnumValue("Labeled Extract Name")>]   LabeledExtractName // "Labeled Extract Name"

    static member create t =
        if t = "Extract Name" then ExtractName
        elif t = "Labeled Extract Name" then LabeledExtractName
        else failwith "No other value than \"Extract Name\" or \"Labeled Extract Name\" allowed for materialtype"

    /// Returns the type of the MaterialType
    member this.AsString =
        match this with
        | ExtractName -> "Extract"
        | LabeledExtractName -> "Labeled Extract"

type Material = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"name")>]
        Name : string option
        [<JsonPropertyName(@"type")>]
        MaterialType : MaterialType option
        [<JsonPropertyName(@"characteristics")>]
        Characteristics : MaterialAttributeValue list option
        [<JsonPropertyName(@"derivesFrom")>]
        DerivesFrom : OntologyAnnotation option   
    }

    static member create(?Id,?Name,?MaterialType,?Characteristics,?DerivesFrom) : Material = 
        {
            ID              = Id
            Name            = Name
            MaterialType    = MaterialType
            Characteristics = Characteristics     
            DerivesFrom     = DerivesFrom       
        }

    static member empty =
        Material.create()

    [<System.Obsolete("This function is deprecated. Use the member \"GetNameWithNumber\" instead.")>]
    member this.NameAsString =
        this.Name
        |> Option.defaultValue ""

    member this.GetName =
        this.Name
        |> Option.defaultValue ""

    interface IISAPrintable with
        member this.Print() = 
            this.ToString()
        member this.PrintCompact() =
            let chars = this.Characteristics |> Option.defaultValue [] |> List.length
            match this.MaterialType with
            | Some t ->
                sprintf "%s [%s; %i characteristics]" this.GetName t.AsString chars
            | None -> sprintf "%s [%i characteristics]" this.GetName chars