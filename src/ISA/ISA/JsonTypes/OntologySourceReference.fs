namespace ISA

type OntologySourceReference =
    {
        Description : string option
        File : string option
        Name : string option
        Version : string option
        Comments : Comment list option
    }

    static member make description file name version comments  =
        {

            Description = description
            File        = file
            Name        = name
            Version     = version
            Comments    = comments
        }

    static member create(?Description,?File,?Name,?Version,?Comments) : OntologySourceReference =
        OntologySourceReference.make Description File Name Version Comments

    static member empty =
        OntologySourceReference.create()