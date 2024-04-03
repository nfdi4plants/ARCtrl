namespace ARCtrl

open Fable.Core
open ARCtrl.Helper

[<AttachMembers>]
type Organisation =
| [<CompiledName "DataPLANT">] DataPLANT
| Other of string
    static member ofString (str:string) = 
        match str.ToLower() with
        | "dataplant" -> DataPLANT
        | _ -> Other str

    override this.ToString() =
        match this with
        | DataPLANT -> "DataPLANT"
        | Other anyElse -> anyElse

    member this.IsOfficial() = this = DataPLANT

[<AttachMembers>]
type Template(id: System.Guid, table: ArcTable, ?name: string, ?description, ?organisation: Organisation, ?version: string, ?authors: ResizeArray<Person>, 
    ?repos: ResizeArray<OntologyAnnotation>, ?tags: ResizeArray<OntologyAnnotation>, ?lastUpdated: System.DateTime) =

    let name = defaultArg name ""
    let description = defaultArg description ""
    let organisation = defaultArg organisation (Other "Custom Organisation")
    let version = defaultArg version "0.0.0"
    let authors = defaultArg authors <| ResizeArray()
    let repos = defaultArg repos <| ResizeArray()
    let tags = defaultArg tags <| ResizeArray()
    let lastUpdated = defaultArg lastUpdated (System.DateTime.Now)

    member val Id : System.Guid = id with get, set
    member val Table : ArcTable = table with get, set
    member val Name : string = name with get, set
    member val Description : string = description with get, set
    member val Organisation : Organisation = organisation with get, set
    member val Version : string = version with get, set
    member val Authors : ResizeArray<Person> = authors with get, set
    member val EndpointRepositories : ResizeArray<OntologyAnnotation> = repos with get, set
    member val Tags : ResizeArray<OntologyAnnotation> = tags with get, set
    member val LastUpdated : System.DateTime = lastUpdated with get, set

    static member make id table name description organisation version authors repos tags lastUpdated =
        Template(id, table, name, description, organisation, version, authors, repos, tags, lastUpdated)

    static member create(id, table, ?name, ?description, ?organisation, ?version, ?authors, ?repos, ?tags, ?lastUpdated) =
        Template(id, table, ?name=name, ?description = description, ?organisation=organisation, ?version=version, ?authors=authors, ?repos=repos, ?tags=tags, ?lastUpdated=lastUpdated)

    static member init(templateName: string) =
        let guid = System.Guid.NewGuid()
        let table = ArcTable.init(templateName)
        Template(guid, table, templateName)

    member this.SemVer
        with get() = ARCtrl.Helper.SemVer.SemVer.tryOfString this.Version

    /// <summary>
    /// Use this function to check if this Template and the input Template refer to the same object.
    ///
    /// If true, updating one will update the other due to mutability.
    /// </summary>
    /// <param name="other">The other Template to test for reference.</param>
    member this.ReferenceEquals (other: Template) = System.Object.ReferenceEquals(this,other)

    member this.StructurallyEquals (other: Template) =
        this.GetHashCode() = other.GetHashCode()

    // custom check
    override this.Equals other =
        match other with
        | :? Template as template ->
            this.StructurallyEquals(template)
        | _ -> 
            false

    override this.GetHashCode() =
        [|
            box (this.Id.ToString())
            box (this.Table.GetHashCode())
            box this.Name
            box (this.Organisation.GetHashCode())
            box this.Version
            HashCodes.boxHashSeq this.Authors
            HashCodes.boxHashSeq this.EndpointRepositories
            HashCodes.boxHashSeq this.Tags
            box (HashCodes.hashDateTime this.LastUpdated)
        |]
        |> HashCodes.boxHashArray 
        |> fun x -> x :?> int
