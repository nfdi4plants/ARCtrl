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

    member this.IsDerivedData =
        match this with
        | DerivedDataFile -> true
        | _ -> false

    member this.IsRawData =
        match this with
        | RawDataFile -> true
        | _ -> false

    member this.IsImage =
        match this with
        | ImageFile -> true
        | _ -> false

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

    static member make id name dataType comments =
        {
            ID      = id
            Name    = name
            DataType = dataType
            Comments = comments         
        }

    static member create (?Id,?Name,?DataType,?Comments) = 
        Data.make Id Name DataType Comments

    static member empty =
        Data.create()

    member this.NameAsString =
        this.Name
        |> Option.defaultValue ""

    interface IISAPrintable with
        member this.Print() = 
            this.ToString()
        member this.PrintCompact() =
            match this.DataType with
            | Some t ->
                sprintf "%s [%s]" this.NameAsString t.AsString 
            | None -> sprintf "%s" this.NameAsString

type Source = 
    {
        [<JsonPropertyName(@"@id")>]
        ID : URI option
        [<JsonPropertyName(@"name")>]
        Name : string option
        [<JsonPropertyName(@"characteristics")>]
        Characteristics : MaterialAttributeValue list option
    }

    static member make id name characteristics : Source=
        {
            ID              = id
            Name            = name
            Characteristics = characteristics          
        }

    static member create(?Id,?Name,?Characteristics) =
        Source.make Id Name Characteristics

    static member empty =
        Source.create()

    member this.NameAsString =
        this.Name
        |> Option.defaultValue ""

    interface IISAPrintable with
        member this.Print() = 
            this.ToString()
        member this.PrintCompact() =
            let l = this.Characteristics |> Option.defaultValue [] |> List.length
            sprintf "%s [%i characteristics]" this.NameAsString l 

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

    static member make id name characteristics factorValues derivesFrom : Sample=
        {
            ID              = id
            Name            = name
            Characteristics = characteristics     
            FactorValues    = factorValues
            DerivesFrom     = derivesFrom       
        }

    static member create(?Id,?Name,?Characteristics, ?FactorValues, ?DerivesFrom) =
        Sample.make Id Name Characteristics FactorValues DerivesFrom


    static member empty =
        Sample.create()

    member this.NameAsString =
        this.Name
        |> Option.defaultValue ""

    interface IISAPrintable with
        member this.Print() = 
            this.ToString()
        member this.PrintCompact() =
            let chars = this.Characteristics |> Option.defaultValue [] |> List.length
            let facts = this.FactorValues |> Option.defaultValue [] |> List.length
            sprintf "%s [%i characteristics; %i factors]" this.NameAsString chars facts
