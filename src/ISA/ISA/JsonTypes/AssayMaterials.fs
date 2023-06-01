namespace ISA

type AssayMaterials =
    {
        Samples : Sample list option
        OtherMaterials : Material list option
    }

    static member make samples otherMaterials =
        {
            Samples = samples
            OtherMaterials = otherMaterials       
        }

    static member create(?Samples,?OtherMaterials) : AssayMaterials =
        AssayMaterials.make Samples OtherMaterials

    static member empty =
        AssayMaterials.create()
