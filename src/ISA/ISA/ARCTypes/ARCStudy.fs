namespace ISA

open Fable.Core

[<AttachMembers>]
type ARCStudy = 
    {
        Identifier : string option
        Assays : Assay array option
    }

    [<NamedParams>]
    static member create (?Identifier, ?Assays : Assay array) = 
        {
            Identifier = Identifier
            Assays = Assays
        }

    static member tryGetAssayByID (assayIdentifier : string) (study : Study) : Assay option = 
        raise (System.NotImplementedException())

    static member updateAssayByID (assay : Assay) (assayIdentifier : string) (study : Study) : Study = 
        ARCStudy.tryGetAssayByID assayIdentifier study |> ignore
        raise (System.NotImplementedException())

    static member addAssay (assay : Assay) (study : Study) : Study = 
        raise (System.NotImplementedException())

