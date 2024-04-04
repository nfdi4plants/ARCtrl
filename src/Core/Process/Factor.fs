namespace ARCtrl.Process

open ARCtrl
open ARCtrl.Helper
open Fable.Core

[<AttachMembers>]
type Factor = 
    {
        Name : string option
        FactorType : OntologyAnnotation option
        Comments : ResizeArray<Comment> option
    }

    static member make name factorType comments =
        {
            Name    = name
            FactorType = factorType
            Comments = comments         
        }

    static member create(?Name,?FactorType,?Comments) : Factor =
        Factor.make Name FactorType Comments

    static member empty =
        Factor.create()

    /// Create a ISAJson Factor from ISATab string entries
    static member fromString (name : string, term:string, source:string, accession:string, ?comments : ResizeArray<Comment>) =
        let oa = OntologyAnnotation.create (term, source, accession, ?comments = comments)
        Factor.make (Option.fromValueWithDefault "" name) (Option.fromValueWithDefault (OntologyAnnotation()) oa) None

    /// Get ISATab string entries from an ISAJson Factor object
    static member toStringObject (factor : Factor) =
        factor.FactorType |> Option.map OntologyAnnotation.toStringObject |> Option.defaultValue {|TermName = ""; TermAccessionNumber = ""; TermSourceREF = ""|}

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

    /// If a factor with the given name exists in the list, returns it
    static member tryGetByName (name : string) (factors:Factor list) =
        List.tryFind (fun (f:Factor) -> f.Name = Some name) factors

    /// If a factor with the given name exists in the list exists, returns true
    static member existsByName (name : string) (factors:Factor list) =
        List.exists (fun (f:Factor) -> f.Name = Some name) factors

    /// adds the given factor to the factors  
    static member add (factors:Factor list) (factor : Factor) =
        List.append factors [factor]

    /// If a factor with the given name exists in the list, removes it
    static member removeByName (name : string) (factors : Factor list) = 
        List.filter (fun (f:Factor) -> f.Name = Some name |> not) factors

    /// Returns comments of a factor
    static member getComments (factor : Factor) =
        factor.Comments

    /// Applies function f on comments of a factor
    static member mapComments (f : ResizeArray<Comment> -> ResizeArray<Comment>) (factor : Factor) =
        { factor with 
            Comments = Option.map f factor.Comments}

    /// Replaces comments of a factor by given comment list
    static member setComments (factor : Factor) (comments : ResizeArray<Comment>) =
        { factor with
            Comments = Some comments }

    /// Returns factor type of a factor
    static member getFactorType (factor : Factor) =
        factor.FactorType

    /// Applies function f on factor type of a factor
    static member mapFactorType (f : OntologyAnnotation -> OntologyAnnotation) (factor : Factor) =
        { factor with 
            FactorType = Option.map f factor.FactorType}

    /// Replaces factor type of a factor by given factor type
    static member setFactorType (factor : Factor) (factorType : OntologyAnnotation) =
        { factor with
            FactorType = Some factorType }

    /// Returns the name of the factor as string if it exists
    static member tryGetName (f : Factor) =
        f.Name

    /// Returns the name of the factor as string
    static member getNameAsString (f : Factor) =
        f.NameText

    /// Returns true if the given name matches the name of the factor
    static member nameEqualsString (name : string) (f : Factor) =
        f.NameText = name

    member this.Copy() =
        let nextComments = this.Comments |> Option.map (ResizeArray.map (fun c -> c.Copy()))
        Factor.make this.Name this.FactorType nextComments
