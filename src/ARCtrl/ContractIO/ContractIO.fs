module ARCtrl.Contract

open ARCtrl
open ARCtrl.Contract
open FsSpreadsheet

let fulfillReadContract basePath (c : Async<Contract>) =
    async {
        let! c = c
        try
            match c.DTOType with
            | Some DTOType.ISA_Assay 
            | Some DTOType.ISA_Investigation
            | Some DTOType.ISA_Study 
            | Some DTOType.ISA_Datamap ->
                let path = ArcPathHelper.combine basePath c.Path
                let wb = FileSystemHelper.readFileXlsx path |> box |> DTO.Spreadsheet
                return Ok {c with DTO = Some wb}
            | Some DTOType.PlainText ->
                let path = ArcPathHelper.combine basePath c.Path
                let text = FileSystemHelper.readFileText path |> DTO.Text
                return Ok {c with DTO = Some text}
            | _ -> 
                return Error (sprintf "Contract %s is not an ISA contract" c.Path)
        with
        | e -> return Error (sprintf "Error reading contract %s: %s" c.Path e.Message)
    }

let fullfillContractBatchBy contractF basePath (cs : (Async<Contract>) []) : Async<Result<Contract [], string []>> =
    async {
        let! cs = Async.Sequential cs
        let res = 
            cs
            |> Array.map (contractF basePath)
            |> Array.fold (fun acc cr ->
                match acc, cr with
                | Ok acc, Ok cr -> Ok (Array.append acc [|cr|])
                | Error e, Ok _ -> Error e
                | Error acc, Error e -> Error (Array.append acc [|e|])
                | Ok _, Error e -> Error [|e|]
            ) (Ok [||])
        return res
    }

let fulfillWriteContract basePath (c : Async<Contract>) =
    try 
        match c.DTO with
        | Some (DTO.Spreadsheet wb) ->
            let path = ArcPathHelper.combine basePath c.Path
            FileSystemHelper.ensureDirectoryOfFile path
            FileSystemHelper.writeFileXlsx path (wb :?> FsWorkbook)
            Ok (c)
        | Some (DTO.Text t) ->
            let path = ArcPathHelper.combine basePath c.Path
            FileSystemHelper.ensureDirectoryOfFile path
            FileSystemHelper.writeFileText path t
            Ok (c)
        | None -> 
            let path = ArcPathHelper.combine basePath c.Path
            FileSystemHelper.ensureDirectoryOfFile path
            FileSystemHelper.writeFileText path ""
            Ok (c)
        | _ -> 
            Error (sprintf "Contract %s is not an ISA contract" c.Path)
    with
    | e -> Error (sprintf "Error writing contract %s: %s" c.Path e.Message)

let fulfillUpdateContract basePath (c : Contract) =
    try 
        match c.DTO with
        | Some (DTO.Spreadsheet wb) ->
            let path = ArcPathHelper.combine basePath c.Path
            FileSystemHelper.ensureDirectoryOfFile path
            FileSystemHelper.writeFileXlsx path (wb :?> FsWorkbook)
            Ok (c)
        | Some (DTO.Text t) ->
            let path = ArcPathHelper.combine basePath c.Path
            FileSystemHelper.ensureDirectoryOfFile path
            FileSystemHelper.writeFileText path t
            Ok (c)
        | None -> 
            let path = ArcPathHelper.combine basePath c.Path
            FileSystemHelper.ensureDirectoryOfFile path
            FileSystemHelper.writeFileText path ""
            Ok (c)
        | _ -> 
            Error (sprintf "Contract %s is not an ISA contract" c.Path)
    with
    | e -> Error (sprintf "Error updating contract %s: %s" c.Path e.Message)

let fullfillRenameContract basePath (c : Contract) =
    match c.DTO with
    | Some (DTO.Text t) when t = c.Path ->
        Error (sprintf "Rename Contract %s old and new Path are the same" c.Path)
    | Some (DTO.Text t) ->
        let newPath = ArcPathHelper.combine basePath t
        let oldPath = ArcPathHelper.combine basePath c.Path
        FileSystemHelper.renameFileOrDirectory oldPath newPath
        Ok (c)
    | _ -> Error (sprintf "Rename Contract %s does not contain new Path" c.Path)
    

let fullfillDeleteContract basePath (c : Contract) =
    let path = ArcPathHelper.combine basePath c.Path
    FileSystemHelper.deleteFileOrDirectory path
    Ok (c)

let fullFillContract basePath (c : Contract) =
    match c.Operation with
    | Operation.READ -> fulfillReadContract basePath c
    | Operation.CREATE -> fulfillWriteContract basePath c
    | Operation.UPDATE -> fulfillUpdateContract basePath c
    | Operation.DELETE -> fullfillDeleteContract basePath c
    | Operation.RENAME -> fullfillRenameContract basePath c
    | _ -> Error (sprintf "Operation %A not supported" c.Operation)

let fullFillContractBatch basePath (cs : Contract []) =
    fullfillContractBatchBy fullFillContract basePath cs