namespace ISA


type ProtocolParameter = 
    {
        ID : URI option
        ParameterName : OntologyAnnotation option
    }
    
    static member make id parameterName : ProtocolParameter= 
        {
            ID = id
            ParameterName = parameterName
        
        }

    static member create (?Id,?ParameterName) : ProtocolParameter =
        ProtocolParameter.make Id ParameterName

    static member empty =
        ProtocolParameter.create()

    /// Create a ISAJson Protocol Parameter from ISATab string entries
    static member fromString (term:string) (source:string) (accession:string) =
        let oa = OntologyAnnotation.fromString term source accession
        ProtocolParameter.make None (Option.fromValueWithDefault OntologyAnnotation.empty oa)

    /// Create a ISAJson Protocol parameter from ISATab string entries
    static member fromStringWithComments (term:string) (source:string) (accession:string) (comments : Comment list) =
        let oa = OntologyAnnotation.fromStringWithComments term source accession comments
        ProtocolParameter.make None (Option.fromValueWithDefault OntologyAnnotation.empty oa)


    /// Get ISATab string entries from an ISAJson ProtocolParameter object (name,source,accession)
    static member toString (pp : ProtocolParameter) =
        pp.ParameterName |> Option.map OntologyAnnotation.toString |> Option.defaultValue ("","","")        

    /// Returns the name of the parameter as string
    member this.NameText =
        this.ParameterName
        |> Option.map (fun oa -> oa.NameText)
        |> Option.defaultValue ""

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            "OA " + this.NameText

    member this.MapCategory(f : OntologyAnnotation -> OntologyAnnotation) =
        {this with ParameterName = Option.map f this.ParameterName}

    member this.SetCategory(c : OntologyAnnotation) =
        {this with ParameterName = Some c}