namespace ARC.API

open Fable.Core
open Fable.Core.JsInterop

type FsSpreadsheet = unit

module Contract =

    [<StringEnum>]
    [<RequireQualifiedAccess>]
    type DTOType = 
        | [<CompiledName("Spreadsheet")>] Spreadsheet // isa.assay.xlsx
        | [<CompiledName("JSON")>] JSON // arc.json
        | [<CompiledName("Markdown")>] Markdown // README.md
        | [<CompiledName("CWL")>] CWL // workflow.cwl, might be a new DTO once we 
        | [<CompiledName("PlainText")>] PlainText // any other
        | [<CompiledName("EmptyFile")>] EmptyFile // .gitkeep

    [<Erase>]
    [<RequireQualifiedAccess>]
    type DTOData =
        | Spreadsheet of FsSpreadsheet
        | JSON of string

    [<StringEnum>]
    type Operation =
        | [<CompiledName("CREATE")>] CREATE
        | [<CompiledName("UPDATE")>] UPDATE
        | [<CompiledName("DELETE")>] DELETE

    [<AttachMembers>]
    type DTO = 
        {
            Type: DTOType
            Data: DTOData
        }
        static member create (dtoType, dtoData) = {Type = dtoType; Data = dtoData}

    [<AttachMembers>]
    type ARCContract = 
        {
            Operation : Operation
            Path: string 
            DTO: DTO option
        }
        [<NamedParams(2)>]
        static member create(op, path, ?dto) = {Operation= op; Path = path; DTO = dto}

    // update a study person name
    let updateStudyContracts = [
        // UPDATE INVESTIGATION METADATA
        ARCContract.create(
            op = UPDATE,
            path = "path/to/investigation.xlsx",
            dto = DTO.create(
                dtoType = DTOType.Spreadsheet,
                dtoData = DTOData.Spreadsheet ((*investigation spreadsheet data here*))
            )
        ) 
        ARCContract.create(
            op = UPDATE,
            path = "path/to/study.xlsx",
            dto = DTO.create(
                dtoType = DTOType.Spreadsheet,
                dtoData = DTOData.Spreadsheet ((*study spreadsheet data here*))
            )
        ) 
    ]

    // delete a study
    let deleteStudyContracts = [
        // UPDATE INVESTIGATION METADATA
        ARCContract.create(
            op = DELETE,
            path = "path/to/studyFOLDER(!)"
        ) 
        ARCContract.create(
            op = UPDATE,
            path = "path/to/investigation.xlsx",
            dto = DTO.create(
                dtoType = DTOType.Spreadsheet,
                dtoData = DTOData.Spreadsheet ((*study spreadsheet data here*))
            )
        ) 
    ]

    //// Assay add
    // create spreadsheet assays/AssayName/isa.assay.xlsx  
    // create text assays/AssayName/dataset/.gitkeep 
    // create text assays/AssayName/dataset/Readme.md
    // create text assays/AssayName/protocols/.gitkeep 
    // update spreadsheet isa.investigation.xlsx
      
    //// Assay register 
    // update spreadsheet isa.investigation.xlsx

    //type ARCContract = unit
    //    {
    //        ISA list
    //        CWL list
        
    //    }

    //type ARCContract = 
    //    | ISA of ISAContract
    //    | CWL of CWLContract

module Assay =
    
    // Immutable ARC
    let register (assay : ISA.Assay) (arc : ARC.ARC) : ARC.ARC * Contract.ARCContract list = 
        // add the assay to the ARC ISA
        // return the new ARC with the updated ISA and Filesystem
        raise (System.NotImplementedException()) 

    // Immutable ARC
    let add (assay : ISA.Assay) (arc : ARC.ARC) : ARC.ARC * Contract.ARCContract list = 
        register assay arc |> ignore
        // add the assay to the ARC Filesystem 
        // return the new ARC with the updated ISA and Filesystem
        raise (System.NotImplementedException()) 

module ARC = 

    let add = 1 
    let remove = 1 



    let addAssay assay arc = 

        raise (System.NotImplementedException())

    module Assay = 

        let add assay arc = 

            raise (System.NotImplementedException())
   
   
module Study = 

    let addAssay assay arc = 

        raise (System.NotImplementedException())

    module Assay = 

        let add assay arc = 

            raise (System.NotImplementedException())
    

module Usage = 

    let a  = 1
    let s = 6354646
    let ass = 2

    ARC.addAssay ass a
    Study.addAssay s a
    
    ARC.Assay.add ass a
    Study.Assay.add s a



module ARC1 = 

    let diff (arc1 : ARC.ARC) (arc2 : ARC.ARC) : Contract.ARCContract list = 
        raise (System.NotImplementedException()) 





module Assay2 =

    // Mutable ARC
    let add (assay : ISA.Assay) (arc : ARC.ARC) : Contract.ARCContract list = 
        raise (System.NotImplementedException())
        


