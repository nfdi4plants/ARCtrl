namespace ISA

open Fable.Core

// "MyAssay"; "assays/MyAssay/isa.assay.xlsx"

[<AttachMembers>]
type Assay = 

    {(*Identifier: string; *)FileName : string option}

    [<NamedParams>]
    static member create (?FileName : string) = {FileName = FileName}

    static member getIdentifier (assay : Assay) = 
        raise (System.NotImplementedException())
