namespace ISA

open Fable.Core

// "MyAssay"; "assays/MyAssay/isa.assay.xlsx"

[<AttachMembers>]
type ArcAssay = 

    {
        ID : URI option
        FileName : string option
        MeasurementType : OntologyAnnotation option
        TechnologyType : OntologyAnnotation option
        TechnologyPlatform : string option
        Sheets : ArcTable list option
        Comments : Comment list option
    }
   
    static member make 
        (id : URI option)
        (fleName : string option)
        (measurementType : OntologyAnnotation option)
        (technologyType : OntologyAnnotation option)
        (technologyPlatform : string option)
        (sheets : ArcTable list option)
        (comments : Comment list option) = 
        {
                ID = id
                FileName = fleName
                MeasurementType = measurementType
                TechnologyType = technologyType
                TechnologyPlatform = technologyPlatform
                Sheets = sheets
                Comments = comments
            }
      
    [<NamedParams>]
    static member create (?ID : URI, ?FileName : string, ?MeasurementType : OntologyAnnotation, ?TechnologyType : OntologyAnnotation, ?TechnologyPlatform : string, ?Sheets : ArcTable list, ?Comments : Comment list) = 
        ARCAssay.make ID FileName MeasurementType TechnologyType TechnologyPlatform Sheets Comments

    static member getIdentifier (assay : Assay) = 
        raise (System.NotImplementedException())
