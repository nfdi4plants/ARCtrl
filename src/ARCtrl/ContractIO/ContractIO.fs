module ARCtrl.Contract

open ARCtrl
open ARCtrl.Contract
open FsSpreadsheet

let fulfillReadContract basePath (c : Contract) =
    match c.DTOType with
    | Some DTOType.ISA_Assay 
    | Some DTOType.ISA_Investigation
    | Some DTOType.ISA_Study 
    | Some DTOType.ISA_Datamap ->
        let path = ArcPathHelper.combine basePath c.Path
        let wb = FileSystemHelper.readFileXlsx path |> box |> DTO.Spreadsheet
        Ok {c with DTO = Some wb}
    | Some DTOType.PlainText ->
        let path = ArcPathHelper.combine basePath c.Path
        let text = FileSystemHelper.readFileText path |> DTO.Text
        Ok {c with DTO = Some text}
    | _ -> 
        Error (sprintf "Contract %s is not an ISA contract" c.Path) 

let fulfillWriteContract basePath (c : Contract) =
    match c.DTO with
    | Some (DTO.Spreadsheet wb) ->
        let path = ArcPathHelper.combine basePath c.Path
        FileSystemHelper.ensureDirectory path
        FileSystemHelper.writeFileXlsx path (wb :?> FsWorkbook)
        Ok ()
    | Some (DTO.Text t) ->
        let path = ArcPathHelper.combine basePath c.Path
        FileSystemHelper.ensureDirectory path
        FileSystemHelper.writeFileText path t
        Ok ()
    | None -> 
        let path = ArcPathHelper.combine basePath c.Path
        FileSystemHelper.ensureDirectory path
        FileSystemHelper.writeFileText path ""
        Ok ()
    | _ -> 
        Error (sprintf "Contract %s is not an ISA contract" c.Path)

let fulfillUpdateContract basePath (c : Contract) =
    match c.DTO with
    | Some (DTO.Spreadsheet wb) ->
        let path = ArcPathHelper.combine basePath c.Path
        FileSystemHelper.ensureDirectory path
        FileSystemHelper.writeFileXlsx path (wb :?> FsWorkbook)
        Ok ()
    | Some (DTO.Text t) ->
        let path = ArcPathHelper.combine basePath c.Path
        FileSystemHelper.ensureDirectory path
        FileSystemHelper.writeFileText path t
        Ok ()
    | None -> 
        let path = ArcPathHelper.combine basePath c.Path
        FileSystemHelper.ensureDirectory path
        FileSystemHelper.writeFileText path ""
        Ok ()
    | _ -> 
        Error (sprintf "Contract %s is not an ISA contract" c.Path)