module ARCtrl.NET.Contract

open ARCtrl.Contract
open FsSpreadsheet
open FsSpreadsheet.Net

let fulfillReadContract basePath (c : Contract) =
    let log = Logging.createLogger("ReadContractHandler")
    match c.DTOType with
    | Some DTOType.ISA_Assay 
    | Some DTOType.ISA_Investigation 
    | Some DTOType.ISA_Study ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        let wb = FsWorkbook.fromXlsxFile path |> box |> DTO.Spreadsheet
        {c with DTO = Some wb}
    | Some DTOType.PlainText ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        let text = System.IO.File.ReadAllText(path) |> DTO.Text
        {c with DTO = Some text}
    | _ -> 
        log.Info(sprintf "Detected unhandled Contract %A." c) 
        c

let fulfillWriteContract basePath (c : Contract) =
    let log = Logging.createLogger("WriteContractHandler")
    match c.DTO with
    | Some (DTO.Spreadsheet wb) ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        Path.ensureDirectory path
        FsWorkbook.toXlsxFile path (wb :?> FsWorkbook)
    | Some (DTO.Text t) ->
        let path = System.IO.Path.Combine(basePath, c.Path)
        Path.ensureDirectory path
        System.IO.File.WriteAllText(path,t)
    | None -> 
        let path = System.IO.Path.Combine(basePath, c.Path)
        Path.ensureDirectory path
        System.IO.File.Create(path).Close()
    | _ -> 
        log.Info(sprintf "Detected unhandled Contract %A." c)

//let fulfillExecuteContract basePath (c : Contract) =
//    let log = Logging.createLogger("ExecuteContractHandler")
//    match c.DTO with
//    | Some (DTO.CLITool tool) ->
//        let path = System.IO.Path.Combine(basePath, c.Path)
//        Path.ensureDirectory path
//        FsWorkbook.toFile path (wb :?> FsWorkbook)
//    | _ -> log.Info(sprintf "Contract %O is not an Execute contract" c)