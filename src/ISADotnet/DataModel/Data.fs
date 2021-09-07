namespace ISADotNet

open System.Text.Json
open System.Text.Json.Serialization

[<StringEnum>]
type DataFile =

    | [<StringEnumValue("Raw Data File")>]      RawDataFile // "Raw Data File"
    | [<StringEnumValue("Derived Data File")>]  DerivedDataFile // "Derived Data File"
    | [<StringEnumValue("Image File")>]         ImageFile // "Image File"

    static member RawDataFileJson       = "Raw Data File"
    static member DerivedDataFileJson   = "Derived Data File"
    static member ImageFileJson         = "Image File"

    member this.AsString =
        match this with
        | RawDataFile       -> "RawDataFileJson"
        | DerivedDataFile   -> "DerivedDataFileJson"
        | ImageFile         -> "ImageFileJson"

type Data = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"name")>]
        Name : string option
        [<JsonPropertyName(@"type")>]
        DataType : DataFile option
        [<JsonPropertyName(@"comments")>]
        Comments : Comment list option
    }

    static member create (?Id,?Name,?DataType,?Comments) : Data =
        {
            ID          = Id
            Name        = Name
            DataType    = DataType
            Comments    = Comments         
        }

    static member empty =
        Data.create()


    [<System.Obsolete("This function is deprecated. Use the member \"GetName\" instead.")>]
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
            match this.DataType with
            | Some t ->
                sprintf "%s [%s]" this.GetName t.AsString 
            | None -> sprintf "%s" this.GetName

type Source = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"name")>]
        Name : string option
        [<JsonPropertyName(@"characteristics")>]
        Characteristics : MaterialAttributeValue list option
    }

    static member create (?Id,?Name,?Characteristics) : Source =
        {
            ID              = Id
            Name            = Name
            Characteristics = Characteristics          
        }

    static member empty =
        Source.create ()

    [<System.Obsolete("This function is deprecated. Use the member \"GetName\" instead.")>]
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
            let l = this.Characteristics |> Option.defaultValue [] |> List.length
            sprintf "%s [%i characteristics]" this.GetName l 

type Sample = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"name")>]
        Name : string option
        [<JsonPropertyName(@"characteristics")>]
        Characteristics : MaterialAttributeValue list option
        [<JsonPropertyName(@"factorValues")>]
        FactorValues : FactorValue list option
        [<JsonPropertyName(@"derivesFrom")>]
        DerivesFrom : Source list option
    }

    static member create (?Id,?Name,?Characteristics,?FactorValues,?DerivesFrom) : Sample =
        {
            ID              = Id
            Name            = Name
            Characteristics = Characteristics     
            FactorValues    = FactorValues
            DerivesFrom     = DerivesFrom       
        }

    static member empty =
        Sample.create()

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
            let facts = this.FactorValues |> Option.defaultValue [] |> List.length
            sprintf "%s [%i characteristics; %i factors]" this.GetName chars facts
