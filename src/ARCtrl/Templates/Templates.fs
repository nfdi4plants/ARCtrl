namespace ARCtrl.Templates

open ARCtrl.ISA
open Fable.Core

// With "Erase" in js these will be "DataPLANT" and `string`
[<Erase>]
type Organisation =
| [<CompiledName "DataPLANT">] DataPLANT
| Other of string

[<AttachMembers>]
type Template(id: System.Guid, table: ArcTable, ?name: string, ?organisation: Organisation, ?version: string, ?authors: Person [], 
    ?repos: OntologyAnnotation [], ?tags: OntologyAnnotation [], ?lastUpdated: System.DateTime) =

    let name = defaultArg name ""
    let organisation = defaultArg organisation (Other "Custom Organisation")
    let version = defaultArg version "0.0.0"
    let authors = defaultArg authors [||]
    let repos = defaultArg repos [||]
    let tags = defaultArg tags [||]
    let lastUpdated = defaultArg lastUpdated (System.DateTime.Now.ToUniversalTime())

    member val Id : System.Guid = id with get, set
    member val Table : ArcTable = table with get, set
    member val Name : string = name with get, set
    member val Organisation : Organisation = organisation with get, set
    member val Version : string = version with get, set
    member val Authors : Person [] = authors with get, set
    member val EndpointRepositories : OntologyAnnotation [] = repos with get, set
    member val Tags : OntologyAnnotation [] = tags with get, set
    member val LastUpdated : System.DateTime = lastUpdated with get, set

    static member make id table name organisation version authors repos tags lastUpdated =
        Template(id, table, name, organisation, version, authors, repos, tags, lastUpdated)

    static member create(id, table, ?name, ?organisation, ?version, ?authors, ?repos, ?tags, ?lastUpdated) =
        Template(id, table, ?name=name, ?organisation=organisation, ?version=version, ?authors=authors, ?repos=repos, ?tags=tags, ?lastUpdated=lastUpdated)

    static member init(templateName: string) =
        let guid = System.Guid.NewGuid()
        let table = ArcTable.init(templateName)
        Template(guid, table, templateName)

    member this.SemVer
        with get() = ARCtrl.SemVer.SemVer.tryOfString this.Version

    member this.structurallyEquivalent (other: Template) =
        (this.Id = other.Id)
        && (this.Table.structurallyEquivalent(other.Table))
        && (this.Name = other.Name)
        && (this.Organisation = other.Organisation)
        && (this.Version = other.Version)
        && (this.Authors = other.Authors)
        && (this.EndpointRepositories = other.EndpointRepositories)
        && (this.Tags = other.Tags)
        && (this.LastUpdated = other.LastUpdated)