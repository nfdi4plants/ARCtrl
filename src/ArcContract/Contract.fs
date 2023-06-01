namespace Contract

open Fable.Core
open Fable.Core.JsInterop

type FsSpreadsheet = unit

[<StringEnum>]
[<RequireQualifiedAccess>]
type DTOType = 
    | [<CompiledName("Spreadsheet")>] Spreadsheet // isa.assay.xlsx
    | [<CompiledName("JSON")>] JSON // arc.json
    | [<CompiledName("Markdown")>] Markdown // README.md
    | [<CompiledName("CWL")>] CWL // workflow.cwl, might be a new DTO once we 
    | [<CompiledName("PlainText")>] PlainText // any other
    | [<CompiledName("Cli")>] CLI
    //| [<CompiledName("EmptyFile")>] EmptyFile // .gitkeep

[<AttachMembers>]
type CLITool = 
    {
        Name: string
        Arguments: string array
    }

    static member create(name,arguments) = {Name = name; Arguments = arguments}

[<Erase>]
[<RequireQualifiedAccess>]
type DTO =
    | Spreadsheet of FsSpreadsheet
    | Text of string
    | CLITool of CLITool

[<StringEnum>]
type Operation =
    | [<CompiledName("CREATE")>] CREATE
    | [<CompiledName("UPDATE")>] UPDATE
    | [<CompiledName("DELETE")>] DELETE
    | [<CompiledName("READ")>] READ
    | [<CompiledName("EXECUTE")>] EXECUTE

[<AttachMembers>]
type Contract = 
    {
        Operation : Operation
        Path: string
        DTOType : DTOType option
        DTO: DTO option
    }
    [<NamedParams(fromIndex=2)>]
    static member create(op, path, ?dtoType, ?dto) = {Operation= op; Path = path; DTOType = dtoType; DTO = dto}
    /// <summary>Create a CREATE contract with all necessary information.</summary>
    /// <param name="path">The path relative from ARC root, at which the new file should be created.</param>
    /// <param name="dtoType">The file type.</param>
    /// <param name="dto">The file data.</param>
    /// <returns>Returns a CREATE contract.</returns>
    static member createCreate(path, dtoType: DTOType, dto: DTO) = {Operation= Operation.CREATE; Path = path; DTOType = Some dtoType; DTO = Some dto}
    /// <summary>Create a UPDATE contract with all necessary information.
    /// 
    /// Update contracts will overwrite in case of a string as DTO and will specifically update relevant changes only for spreadsheet files.
    /// </summary>
    /// <param name="path">The path relative from ARC root, at which the file should be updated.</param>
    /// <param name="dtoType">The file type.</param>
    /// <param name="dto">The file data.</param>
    /// <returns>Returns a UPDATE contract.</returns>
    static member createUpdate(path, dtoType: DTOType, dto: DTO) = {Operation= Operation.UPDATE; Path = path; DTOType = Some dtoType; DTO = Some dto}
    /// <summary>Create a DELETE contract with all necessary information. </summary>
    /// <param name="path">The path relative from ARC root, at which the file should be deleted.</param>
    /// <returns>Returns a DELETE contract.</returns>
    static member createDelete(path) = {Operation= Operation.DELETE; Path = path; DTOType = None; DTO = None}
    /// <summary>Create a READ contract with all necessary information.
    /// 
    /// Created without DTO, any api user should update the READ contract with the io read result for further api use.
    /// Please check documentation for the exact workflow.
    /// </summary>
    /// <param name="path">The path relative from ARC root, at which the file to read is located.</param>
    /// <param name="dtoType">The file type.</param>
    /// <returns>Returns a READ contract.</returns>
    static member createRead(path, dtoType: DTOType) = {Operation= Operation.READ; Path = path; DTOType = Some dtoType; DTO = None}
    /// <summary>Create a EXECUTE contract with all necessary information.
    /// 
    /// This contract type is used to communicate cli tool execution.
    /// </summary>
    /// <param name="dto">The cli tool information.</param>
    /// <param name="path">The path relative from ARC root, at which the cli tool should be executed. **Default:** ARC root</param>
    /// <returns>Returns a EXECUTE contract.</returns>
    [<NamedParams(fromIndex=1)>]
    static member createExecute(dto: CLITool, ?path: string) = 
        let path = Option.defaultValue "" path
        {Operation= Operation.EXECUTE; Path = path; DTOType = Some DTOType.CLI; DTO = Some <| DTO.CLITool dto}

module Samples = 

    let gitPull = 

        Contract.create(
            op = EXECUTE,
            path = "", //relative to arc root
            dto = DTO.CLITool (
                CLITool.create(
                    name = "git",
                    arguments = [|"pull"|]
                )
            )
        )

    // update a study person name
    let updateStudyContracts = [
        // UPDATE INVESTIGATION METADATA
        Contract.create(
            op = UPDATE,
            path = "path/to/investigation.xlsx",          
            dtoType = DTOType.Spreadsheet,
            dto = DTO.Spreadsheet ((*investigation spreadsheet data here*))
            
        ) 
        Contract.create(
            op = UPDATE,
            path = "path/to/study.xlsx",
            dtoType = DTOType.Spreadsheet,
            dto = DTO.Spreadsheet ((*study spreadsheet data here*))           
        ) 
    ]

    // delete a study
    let deleteStudyContracts = [
        // UPDATE INVESTIGATION METADATA
        Contract.create(
            op = DELETE,
            path = "path/to/studyFOLDER(!)"
        ) 
        Contract.create(
            op = UPDATE,
            path = "path/to/investigation.xlsx",
            
            dtoType = DTOType.Spreadsheet,
            dto = DTO.Spreadsheet ((*study spreadsheet data here*))
            
        )
    ]

    // Assay add, when no study exists
    let addAssayContracts = [
        // create spreadsheet assays/AssayName/isa.assay.xlsx  
        Contract.create(
            op = CREATE,
            path = "path/to/isa.assay.xlsx",
            dtoType = DTOType.Spreadsheet,
            dto = DTO.Spreadsheet ((*assay spreadsheet data here*))
        ) 
        // create empty file assays/AssayName/dataset/.gitkeep 
        Contract.create(
            op = CREATE,
            path = "path/to/dataset/.gitkeep"
        )        
        // create text assays/AssayName/README.md
        Contract.create(
            op = CREATE,
            path = "path/to/README.md",
            dtoType = DTOType.Markdown,
            dto = DTO.Text "# Markdown"
            
        )
        // create empty file assays/AssayName/protocols/.gitkeep
        Contract.create(
            op = CREATE,
            path = "path/to/protocols/.gitkeep"
        )
        // create spreadsheet studies/StudyName/isa.study.xlsx  
        Contract.create(
            op = CREATE,
            path = "path/to/study.xlsx",
            dtoType = DTOType.Spreadsheet,
            dto = DTO.Spreadsheet ((*study spreadsheet data here*))
        )
        // create empty file studies/StudyName/resources/.gitkeep 
        Contract.create(
            op = CREATE,
            path = "path/to/resources/.gitkeep"
        )        
        // create text studies/StudyName/README.md
        Contract.create(
            op = CREATE,
            path = "path/to/README.md",
            dtoType = DTOType.Markdown,
            dto = DTO.Text "# Markdown"
        )
        // create empty file studies/StudyName/protocols/.gitkeep
        Contract.create(
            op = CREATE,
            path = "path/to/protocols/.gitkeep"
        )
        // update spreadsheet isa.investigation.xlsx
        Contract.create(
            op = UPDATE,
            path = "path/to/investigation.xlsx",
            dtoType = DTOType.Spreadsheet,
            dto = DTO.Spreadsheet ((*investigation spreadsheet data here*))
            ) 
    ]