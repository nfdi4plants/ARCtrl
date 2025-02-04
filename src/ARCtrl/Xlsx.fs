namespace ARCtrl

open Thoth.Json.Core
open ARCtrl.Spreadsheet
open Fable.Core
open FsSpreadsheet

#if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
open FsSpreadsheet.Js
#endif
#if FABLE_COMPILER_PYTHON
open FsSpreadsheet.Py
#endif
#if !FABLE_COMPILER
open FsSpreadsheet.Net
#endif


module XlsxHelper =

    [<AttachMembers>]
    type DatamapXlsx() =
        member _.fromFsWorkbook (fswb: FsWorkbook) = DataMap.fromFsWorkbook fswb
        member _.toFsWorkbook (datamap: DataMap) = DataMap.toFsWorkbook datamap
        member _.fromXlsxFile (path: string) = FsWorkbook.fromXlsxFile path |> DataMap.fromFsWorkbook
        member _.toXlsxFile (path: string, datamap: DataMap) = DataMap.toFsWorkbook datamap |> FsWorkbook.toXlsxFile path

    [<AttachMembers>]
    type AssayXlsx() =
        member _.fromFsWorkbook (fswb: FsWorkbook) = ArcAssay.fromFsWorkbook fswb
        member _.toFsWorkbook (assay: ArcAssay) = ArcAssay.toFsWorkbook assay
        member _.fromXlsxFile (path: string) = FsWorkbook.fromXlsxFile path |> ArcAssay.fromFsWorkbook
        member _.toXlsxFile (path: string, assay: ArcAssay) = ArcAssay.toFsWorkbook assay |> FsWorkbook.toXlsxFile path

    [<AttachMembers>]
    type StudyXlsx() =
        member _.fromFsWorkbook (fswb: FsWorkbook) = ArcStudy.fromFsWorkbook fswb
        member _.toFsWorkbook (study: ArcStudy, ?assays: ResizeArray<ArcAssay>) = ArcStudy.toFsWorkbook(study,?assays=Option.map List.ofSeq assays)
        member _.fromXlsxFile (path: string) = FsWorkbook.fromXlsxFile path |> ArcStudy.fromFsWorkbook
        member _.toXlsxFile (path: string, study: ArcStudy, ?assays: ResizeArray<ArcAssay>) = ArcStudy.toFsWorkbook(study,?assays=Option.map List.ofSeq assays) |> FsWorkbook.toXlsxFile path


    [<AttachMembers>]
    type InvestigationXlsx() =
        member _.fromFsWorkbook (fswb: FsWorkbook) = ArcInvestigation.fromFsWorkbook fswb
        member _.toFsWorkbook (investigation: ArcInvestigation) = ArcInvestigation.toFsWorkbook(investigation)
        member _.fromXlsxFile (path: string) = FsWorkbook.fromXlsxFile path |> ArcInvestigation.fromFsWorkbook
        member _.toXlsxFile (path: string, investigation: ArcInvestigation) = ArcInvestigation.toFsWorkbook(investigation) |> FsWorkbook.toXlsxFile path

open XlsxHelper

[<AttachMembers>]
type XlsxController =
    static member Datamap = DatamapXlsx()
    static member Assay = AssayXlsx()
    static member Study = StudyXlsx()
    static member Investigation = InvestigationXlsx()