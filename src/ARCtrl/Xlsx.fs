namespace ARCtrl

open Thoth.Json.Core
open ARCtrl.Spreadsheet
open Fable.Core
open FsSpreadsheet

module XlsxHelper =

    [<AttachMembers>]
    type DatamapXlsx() =
        member _.fromFsWorkbook (fswb: FsWorkbook) = DataMap.fromFsWorkbook fswb
        member _.toFsWorkbook (datamap: DataMap) = DataMap.toFsWorkbook datamap

    [<AttachMembers>]
    type AssayXlsx() =
        member _.fromFsWorkbook (fswb: FsWorkbook) = ArcAssay.fromFsWorkbook fswb
        member _.toFsWorkbook (assay: ArcAssay) = ArcAssay.toFsWorkbook assay

    [<AttachMembers>]
    type StudyXlsx() =
        member _.fromFsWorkbook (fswb: FsWorkbook) = ArcStudy.fromFsWorkbook fswb
        member _.toFsWorkbook (study: ArcStudy, ?assays: ResizeArray<ArcAssay>) = ArcStudy.toFsWorkbook(study,?assays=Option.map List.ofSeq assays)

    [<AttachMembers>]
    type InvestigationXlsx() =
        member _.fromFsWorkbook (fswb: FsWorkbook) = ArcInvestigation.fromFsWorkbook fswb
        member _.toFsWorkbook (investigation: ArcInvestigation) = ArcInvestigation.toFsWorkbook(investigation)
        member _.toLightFsWorkbook (investigation: ArcInvestigation) = ArcInvestigation.toLightFsWorkbook(investigation)
        

open XlsxHelper

[<AttachMembers>]
type XlsxController =
    static member Datamap = DatamapXlsx()
    static member Assay = AssayXlsx()
    static member Study = StudyXlsx()
    static member Investigation = InvestigationXlsx()