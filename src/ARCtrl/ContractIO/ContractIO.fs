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

let fullfillContractBatchBy
    (contractF : string -> Async<Contract> -> Async<Result<Contract, string>>)
    (basePath : string)
    (cs : (Async<Contract>) [])
    : Async<Result<Contract [], string []>> =
        async {
            let! seq = 
                cs
                |> Array.map (contractF basePath)
                |> CrossAsync.sequential
            let res =
                seq
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
    async {
        let! c = c
        try 
            match c.DTO with
            | Some (DTO.Spreadsheet wb) ->
                let path = ArcPathHelper.combine basePath c.Path
                FileSystemHelper.ensureDirectoryOfFile path
                FileSystemHelper.writeFileXlsx path (wb :?> FsWorkbook)
                return Ok (c)
            | Some (DTO.Text t) ->
                let path = ArcPathHelper.combine basePath c.Path
                FileSystemHelper.ensureDirectoryOfFile path
                FileSystemHelper.writeFileText path t
                return Ok (c)
            | None -> 
                let path = ArcPathHelper.combine basePath c.Path
                FileSystemHelper.ensureDirectoryOfFile path
                FileSystemHelper.writeFileText path ""
                return Ok (c)
            | _ -> 
                return Error (sprintf "Contract %s is not an ISA contract" c.Path)
        with
        | e -> return Error (sprintf "Error writing contract %s: %s" c.Path e.Message)
    }

let fulfillUpdateContract basePath (c : Async<Contract>) =
    async {
        let! c = c
        try 
            match c.DTO with
            | Some (DTO.Spreadsheet wb) ->
                let path = ArcPathHelper.combine basePath c.Path
                FileSystemHelper.ensureDirectoryOfFile path
                FileSystemHelper.writeFileXlsx path (wb :?> FsWorkbook)
                return Ok (c)
            | Some (DTO.Text t) ->
                let path = ArcPathHelper.combine basePath c.Path
                FileSystemHelper.ensureDirectoryOfFile path
                FileSystemHelper.writeFileText path t
                return Ok (c)
            | None -> 
                let path = ArcPathHelper.combine basePath c.Path
                FileSystemHelper.ensureDirectoryOfFile path
                FileSystemHelper.writeFileText path ""
                return Ok (c)
            | _ -> 
                return Error (sprintf "Contract %s is not an ISA contract" c.Path)
        with
        | e -> return Error (sprintf "Error updating contract %s: %s" c.Path e.Message)
    }

let fullfillRenameContract basePath (c : Async<Contract>) =
    async {
        let! c = c
        try
            match c.DTO with
            | Some (DTO.Text t) when t = c.Path ->
                return Error (sprintf "Rename Contract %s old and new Path are the same" c.Path)
            | Some (DTO.Text t) ->
                let newPath = ArcPathHelper.combine basePath t
                let oldPath = ArcPathHelper.combine basePath c.Path
                FileSystemHelper.renameFileOrDirectory oldPath newPath
                return Ok (c)
            | _ -> return Error (sprintf "Rename Contract %s does not contain new Path" c.Path)
        with
        | e -> return Error (sprintf "Error renaming contract %s: %s" c.Path e.Message)
    }

let fullfillDeleteContract basePath (c : Async<Contract>) =
    async {
        let! c = c
        try 
            let path = ArcPathHelper.combine basePath c.Path
            FileSystemHelper.deleteFileOrDirectory path
            return Ok (c)
        with
        | e -> return Error (sprintf "Error deleting contract %s: %s" c.Path e.Message)
    }

let fullFillContract basePath (c : Async<Contract>) =
    async {
        let! cSync = c
        match cSync.Operation with
        | Operation.READ -> return! fulfillReadContract basePath c
        | Operation.CREATE -> return! fulfillWriteContract basePath c
        | Operation.UPDATE -> return! fulfillUpdateContract basePath c
        | Operation.DELETE -> return! fullfillDeleteContract basePath c
        | Operation.RENAME -> return! fullfillRenameContract basePath c
        | _ -> return Error (sprintf "Operation %A not supported" cSync.Operation)
    }

let fullFillContractBatch basePath (cs : (Async<Contract>) []) =
    fullfillContractBatchBy fullFillContract basePath cs