namespace ISADotNet

open System.Text.Json.Serialization

type MaterialAttribute = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"characteristicType")>]
        CharacteristicType : OntologyAnnotation option
    
    }
        
    static member make id characteristicType =
        {
            ID = id
            CharacteristicType = characteristicType     
        }

    static member create (?Id,?CharacteristicType) : MaterialAttribute =
        MaterialAttribute.make Id CharacteristicType

    static member empty =
        MaterialAttribute.create()

    /// Create a ISAJson MaterialAttribute from ISATab string entries
    static member fromString (term:string) (source:string) (accession:string) =
        let oa = OntologyAnnotation.fromString term source accession
        MaterialAttribute.make None (Option.fromValueWithDefault OntologyAnnotation.empty oa)

    /// Create a ISAJson MaterialAttribute from string entries, where the term name can contain a # separated number. e.g: "temperature unit #2"
    [<System.Obsolete("This function is deprecated. Numbering support will soon be dropped")>]
    static member fromStringWithNumber (term:string) (source:string) (accession:string) =
        let oa = OntologyAnnotation.fromStringWithNumber term source accession
        MaterialAttribute.make None (Option.fromValueWithDefault OntologyAnnotation.empty oa)

    /// Create a ISAJson MaterialAttribute from string entries
    static member fromStringWithComments (term:string) (source:string) (accession:string) (comments : Comment list) =
        let oa = OntologyAnnotation.fromStringWithComments term source accession comments
        MaterialAttribute.make None (Option.fromValueWithDefault OntologyAnnotation.empty oa)

    /// Create a ISAJson MaterialAttribute from string entries, where the term name can contain a # separated number. e.g: "temperature unit #2"
    [<System.Obsolete("This function is deprecated. Numbering support will soon be dropped")>]
    static member fromStringWithNumberAndComments (term:string) (source:string) (accession:string) (comments : Comment list) =
        let oa = OntologyAnnotation.fromStringWithNumberAndComments term source accession comments
        MaterialAttribute.make None (Option.fromValueWithDefault OntologyAnnotation.empty oa)

    /// Get ISATab string entries from an ISAJson MaterialAttribute object
    static member toString (ma : MaterialAttribute) =
        ma.CharacteristicType |> Option.map OntologyAnnotation.toString |> Option.defaultValue ("","","")    

    /// Returns the name of the characteristic as string
    [<System.Obsolete("This function is deprecated. Use the member \"NameText\" instead.")>]
    member this.NameAsString =
        this.CharacteristicType
        |> Option.map (fun oa -> oa.NameAsString)
        |> Option.defaultValue ""

    /// Returns the name of the characteristic with the number as string (e.g. "temperature #2")
    [<System.Obsolete("This function is deprecated. Numbering support will soon be dropped")>]
    member this.NameAsStringWithNumber =       
        this.CharacteristicType
        |> Option.map (fun oa -> oa.NameAsStringWithNumber)
        |> Option.defaultValue ""

    /// Returns the name of the characteristic as string
    [<System.Obsolete("This function is deprecated. Use the member \"NameText\" instead.")>]
    member this.GetName =
        this.CharacteristicType
        |> Option.map (fun oa -> oa.GetName)
        |> Option.defaultValue ""

    /// Returns number of Material attribute
    [<System.Obsolete("This function is deprecated. Numbering support will soon be dropped")>]
    member this.Number =       
        this.CharacteristicType
        |> Option.bind (fun oa -> oa.Number)
        |> Option.defaultValue ""

    /// Returns the name of the characteristic with the number as string (e.g. "temperature #2")
    [<System.Obsolete("This function is deprecated. Numbering support will soon be dropped")>]
    member this.GetNameWithNumber =       
        this.CharacteristicType
        |> Option.map (fun oa -> oa.GetNameWithNumber)
        |> Option.defaultValue ""

    /// Returns the name of the characteristic as string
    member this.NameText =
        this.CharacteristicType
        |> Option.map (fun oa -> oa.NameText)
        |> Option.defaultValue ""

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            "OA " + this.NameText

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

    static member make id category value unit : MaterialAttributeValue =
        {
            ID      = id
            Category = category
            Value = value
            Unit = unit         
        }


    static member create(?Id,?Category,?Value,?Unit) : MaterialAttributeValue =
        MaterialAttributeValue.make Id Category Value Unit

    static member empty =
        MaterialAttributeValue.create()

    /// Returns the name of the category as string
    [<System.Obsolete("This function is deprecated. Use the member \"NameText\" instead.")>]
    member this.GetName =
        this.Category
        |> Option.map (fun oa -> oa.GetName)
        |> Option.defaultValue ""

    /// Returns the name of the category as string
    member this.NameText =
        this.Category
        |> Option.map (fun oa -> oa.GetName)
        |> Option.defaultValue ""

    /// Returns the name of the category with the number as string (e.g. "temperature #2")
    [<System.Obsolete("This function is deprecated. Numbering support will soon be dropped")>]
    member this.GetNameWithNumber =       
        this.Category
        |> Option.map (fun oa -> oa.GetNameWithNumber)
        |> Option.defaultValue ""

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

    [<System.Obsolete("This function is deprecated. Use the member \"ValueText\" instead.")>]
    member this.GetValue = this.ValueText

    member this.ValueWithUnitText =
        let unit = 
            this.Unit |> Option.map (fun oa -> oa.NameText)
        let v = this.ValueText
        match unit with
        | Some u    -> sprintf "%s %s" v u
        | None      -> v

    [<System.Obsolete("This function is deprecated. Use the member \"ValueWithUnitText\" instead.")>]
    member this.GetValueWithUnit = this.ValueWithUnitText

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
        DerivesFrom : Material list option   
    }

    static member make id name materialType characteristics derivesFrom : Material=
        {
            ID              = id
            Name            = name
            MaterialType    = materialType
            Characteristics = characteristics     
            DerivesFrom     = derivesFrom       
        }

    static member create(?Id,?Name,?MaterialType,?Characteristics,?DerivesFrom) : Material = 
        Material.make Id Name MaterialType Characteristics DerivesFrom

    static member empty =
        Material.create()

    [<System.Obsolete("This function is deprecated. Use the member \"NameText\" instead.")>]
    member this.NameAsString =
        this.Name
        |> Option.defaultValue ""
        
    [<System.Obsolete("This function is deprecated. Use the member \"NameText\" instead.")>]
    member this.GetName =
        this.Name
        |> Option.defaultValue ""

    member this.NameText =
        this.Name
        |> Option.defaultValue ""

    interface IISAPrintable with
        member this.Print() = 
            this.ToString()
        member this.PrintCompact() =
            let chars = this.Characteristics |> Option.defaultValue [] |> List.length
            match this.MaterialType with
            | Some t ->
                sprintf "%s [%s; %i characteristics]" this.NameText t.AsString chars
            | None -> sprintf "%s [%i characteristics]" this.NameText chars