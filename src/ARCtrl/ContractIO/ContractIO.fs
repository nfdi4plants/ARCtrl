module ARCtrl.Contract

open ARCtrl
open ARCtrl.Contract
open FsSpreadsheet
open CrossAsync


let fulfillReadContractAsync basePath (c : Contract) =
    crossAsync {
        try
            match c.DTOType with
            | Some DTOType.ISA_Assay 
            | Some DTOType.ISA_Investigation
            | Some DTOType.ISA_Study 
            | Some DTOType.ISA_Datamap ->
                let path = ArcPathHelper.combine basePath c.Path
                let! wb = FileSystemHelper.readFileXlsxAsync path
                let dto = wb |> box |> DTO.Spreadsheet
                return Ok {c with DTO = Some dto}
            | Some DTOType.PlainText ->
                let path = ArcPathHelper.combine basePath c.Path
                let! text = FileSystemHelper.readFileTextAsync path
                let dto = text |> DTO.Text
                return Ok {c with DTO = Some dto}
            | _ -> 
                return Error (sprintf "Contract %s is neither an ISA nor a freetext contract" c.Path)
        with
        | e -> return Error (sprintf "Error reading contract %s: %s" c.Path e.Message)
    }
    |> catchWith (fun e -> Error (sprintf "Error reading contract %s: %s" c.Path e.Message))

let fullfillContractBatchAsyncBy
    (contractF : string -> Contract -> CrossAsync<Result<Contract, string>>)
    (basePath : string)
    (cs : Contract [])
    : CrossAsync<Result<Contract [], string []>> =
        crossAsync {
            let! seq = 
                cs 
                |> CrossAsync.startSequential (contractF basePath) 
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

let fulfillWriteContractAsync basePath (c : Contract) =
    crossAsync {
        try 
            match c.DTO with
            | Some (DTO.Text t) ->
                let path = ArcPathHelper.combine basePath c.Path
                do! FileSystemHelper.ensureDirectoryOfFileAsync path
                do! FileSystemHelper.writeFileTextAsync path t
                return Ok (c)
            | Some (DTO.Spreadsheet wb) ->
                let path = ArcPathHelper.combine basePath c.Path
                do! FileSystemHelper.ensureDirectoryOfFileAsync path
                do! FileSystemHelper.writeFileXlsxAsync path (wb :?> FsWorkbook)
                return Ok (c)           
            | None ->
                let path = ArcPathHelper.combine basePath c.Path
                do! FileSystemHelper.ensureDirectoryOfFileAsync path
                do! FileSystemHelper.writeFileTextAsync path ""
                return Ok (c)
            | _ -> 
                return Error (sprintf "Contract %s is not an ISA contract" c.Path)
        with
        | e -> return Error (sprintf "Error writing contract %s: %s" c.Path e.Message)
    }
    |> catchWith (fun e -> Error (sprintf "Error writing contract %s: %s" c.Path e.Message))

let fulfillUpdateContractAsync basePath (c : Contract) =
    crossAsync {
        try 
            match c.DTO with
            | Some (DTO.Text t) ->
                let path = ArcPathHelper.combine basePath c.Path
                do! FileSystemHelper.ensureDirectoryOfFileAsync path
                do! FileSystemHelper.writeFileTextAsync path t
                return Ok (c)
            | Some (DTO.Spreadsheet wb) ->
                let path = ArcPathHelper.combine basePath c.Path
                do! FileSystemHelper.ensureDirectoryOfFileAsync path
                do! FileSystemHelper.writeFileXlsxAsync path (wb :?> FsWorkbook)
                return Ok (c)
            | None -> 
                let path = ArcPathHelper.combine basePath c.Path
                do! FileSystemHelper.ensureDirectoryOfFileAsync path
                do! FileSystemHelper.writeFileTextAsync path ""
                return Ok (c)
            | _ -> 
                return Error (sprintf "Contract %s is not an ISA contract" c.Path)
        with
        | e -> return Error (sprintf "Error updating contract %s: %s" c.Path e.Message)
    }
    |> catchWith (fun e -> Error (sprintf "Error updating contract %s: %s" c.Path e.Message))

let fullfillRenameContractAsync basePath (c : Contract) =
    crossAsync {
        try
            match c.DTO with
            | Some (DTO.Text t) when t = c.Path ->
                return Error (sprintf "Rename Contract %s old and new Path are the same" c.Path)
            | Some (DTO.Text t) ->
                let newPath = ArcPathHelper.combine basePath t
                let oldPath = ArcPathHelper.combine basePath c.Path
                do! FileSystemHelper.renameFileOrDirectoryAsync oldPath newPath
                return Ok (c)
            | _ -> return Error (sprintf "Rename Contract %s does not contain new Path" c.Path)
        with
        | e -> return Error (sprintf "Error renaming contract %s: %s" c.Path e.Message)
    }
    |> catchWith (fun e -> Error (sprintf "Error renaming contract %s: %s" c.Path e.Message))

let fullfillDeleteContractAsync basePath (c : Contract) =
    crossAsync {
        try 
            let path = ArcPathHelper.combine basePath c.Path
            do! FileSystemHelper.deleteFileOrDirectoryAsync path
            return Ok (c)
        with
        | e -> return Error (sprintf "Error deleting contract %s: %s" c.Path e.Message)
    }
    |> catchWith (fun e -> Error (sprintf "Error deleting contract %s: %s" c.Path e.Message))

let fullFillContract basePath (c : Contract) =
    crossAsync {
        match c.Operation with
        | Operation.READ -> return! fulfillReadContractAsync basePath c
        | Operation.CREATE -> return! fulfillWriteContractAsync basePath c
        | Operation.UPDATE -> return! fulfillUpdateContractAsync basePath c
        | Operation.DELETE -> return! fullfillDeleteContractAsync basePath c
        | Operation.RENAME -> return! fullfillRenameContractAsync basePath c
        | _ -> return Error (sprintf "Operation %A not supported" c.Operation)
    }
    |> catchWith (fun e -> Error (sprintf "Error fulfilling contract %s: %s" c.Path e.Message))

let fullFillContractBatchAsync basePath (cs : Contract []) =
    fullfillContractBatchAsyncBy fullFillContract basePath cs