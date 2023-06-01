namespace ISA 

type Sample = 
    {
        ID : URI option
        Name : string option
        Characteristics : MaterialAttributeValue list option
        FactorValues : FactorValue list option
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
