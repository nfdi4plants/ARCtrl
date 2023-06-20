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
        Performers : Person list option
        Comments : Comment list option
    }
   
    static member make 
        (id : URI option)
        (fleName : string option)
        (measurementType : OntologyAnnotation option)
        (technologyType : OntologyAnnotation option)
        (technologyPlatform : string option)
        (sheets : ArcTable list option)
        (performers : Person list option)
        (comments : Comment list option) = 
        {
                ID = id
                FileName = fleName
                MeasurementType = measurementType
                TechnologyType = technologyType
                TechnologyPlatform = technologyPlatform
                Sheets = sheets
                Performers = performers
                Comments = comments
            }
      


    [<NamedParams>]
    static member create (?ID : URI, ?FileName : string, ?MeasurementType : OntologyAnnotation, ?TechnologyType : OntologyAnnotation, ?TechnologyPlatform : string, ?Sheets : ArcTable list, ?Performers : Person list, ?Comments : Comment list) = 
        ArcAssay.make ID FileName MeasurementType TechnologyType TechnologyPlatform Sheets Performers Comments

    static member getIdentifier (assay : Assay) = 
        raise (System.NotImplementedException())

    static member setPerformers performers assay =
        {assay with Performers = performers}

    static member fromAssay (assay : Assay) : ArcAssay =
        raise (System.NotImplementedException())

    static member toAssay (assay : ArcAssay) : Assay =
        raise (System.NotImplementedException())