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
        #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        member _.fromXlsxFileAsync (path: string) = FsWorkbook.fromXlsxFile path |> CrossAsync.map DataMap.fromFsWorkbook
        member _.toXlsxFileAsync (path: string, datamap: DataMap) = DataMap.toFsWorkbook datamap |> FsWorkbook.toXlsxFile path
        #else
        member _.fromXlsxFile (path: string) = FsWorkbook.fromXlsxFile path |> DataMap.fromFsWorkbook
        member _.toXlsxFile (path: string, datamap: DataMap) = DataMap.toFsWorkbook datamap |> FsWorkbook.toXlsxFile path
        #endif

    [<AttachMembers>]
    type AssayXlsx() =
        member _.fromFsWorkbook (fswb: FsWorkbook) = ArcAssay.fromFsWorkbook fswb
        member _.toFsWorkbook (assay: ArcAssay) = ArcAssay.toFsWorkbook assay
        #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        member _.fromXlsxFileAsync (path: string) = FsWorkbook.fromXlsxFile path |> CrossAsync.map ArcAssay.fromFsWorkbook
        member _.toXlsxFileAsync (path: string, assay: ArcAssay) = ArcAssay.toFsWorkbook assay |> FsWorkbook.toXlsxFile path
        #else
        member _.fromXlsxFile (path: string) = FsWorkbook.fromXlsxFile path |> ArcAssay.fromFsWorkbook
        member _.toXlsxFile (path: string, assay: ArcAssay) = ArcAssay.toFsWorkbook assay |> FsWorkbook.toXlsxFile path
        #endif

    [<AttachMembers>]
    type StudyXlsx() =
        member _.fromFsWorkbook (fswb: FsWorkbook) = ArcStudy.fromFsWorkbook fswb
        member _.toFsWorkbook (study: ArcStudy, ?assays: ResizeArray<ArcAssay>) = ArcStudy.toFsWorkbook(study,?assays=Option.map List.ofSeq assays)
        #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        member _.fromXlsxFileAsync (path: string) = FsWorkbook.fromXlsxFile path |> CrossAsync.map ArcStudy.fromFsWorkbook
        member _.toXlsxFileAsync (path: string, study: ArcStudy, ?assays: ResizeArray<ArcAssay>) = ArcStudy.toFsWorkbook(study,?assays=Option.map List.ofSeq assays) |> (FsWorkbook.toXlsxFile path)
        #else
        member _.fromXlsxFile (path: string) = FsWorkbook.fromXlsxFile path |> ArcStudy.fromFsWorkbook
        member _.toXlsxFile (path: string, study: ArcStudy, ?assays: ResizeArray<ArcAssay>) = ArcStudy.toFsWorkbook(study,?assays=Option.map List.ofSeq assays) |> FsWorkbook.toXlsxFile path
        #endif

    [<AttachMembers>]
    type WorkflowXlsx() =
        member _.fromFsWorkbook (fswb: FsWorkbook) = ArcWorkflow.fromFsWorkbook fswb
        member _.toFsWorkbook (workflow: ArcWorkflow) = ArcWorkflow.toFsWorkbook(workflow)
        #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        member _.fromXlsxFileAsync (path: string) = FsWorkbook.fromXlsxFile path |> CrossAsync.map ArcWorkflow.fromFsWorkbook
        member _.toXlsxFileAsync (path: string, workflow: ArcWorkflow) = ArcWorkflow.toFsWorkbook(workflow) |> FsWorkbook.toXlsxFile path
        #else
        member _.fromXlsxFile (path: string) = FsWorkbook.fromXlsxFile path |> ArcWorkflow.fromFsWorkbook
        member _.toXlsxFile (path: string, workflow: ArcWorkflow) = ArcWorkflow.toFsWorkbook(workflow) |> FsWorkbook.toXlsxFile path
        #endif

    [<AttachMembers>]
    type RunXlsx() =
        member _.fromFsWorkbook (fswb: FsWorkbook) = ArcRun.fromFsWorkbook fswb
        member _.toFsWorkbook (run: ArcRun) = ArcRun.toFsWorkbook(run)
        #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        member _.fromXlsxFileAsync (path: string) = FsWorkbook.fromXlsxFile path |> CrossAsync.map ArcRun.fromFsWorkbook
        member _.toXlsxFileAsync (path: string, run: ArcRun) = ArcRun.toFsWorkbook(run) |> FsWorkbook.toXlsxFile path
        #else
        member _.fromXlsxFile (path: string) = FsWorkbook.fromXlsxFile path |> ArcRun.fromFsWorkbook
        member _.toXlsxFile (path: string, run: ArcRun) = ArcRun.toFsWorkbook(run) |> FsWorkbook.toXlsxFile path
        #endif

    [<AttachMembers>]
    type InvestigationXlsx() =
        member _.fromFsWorkbook (fswb: FsWorkbook) = ArcInvestigation.fromFsWorkbook fswb
        member _.toFsWorkbook (investigation: ArcInvestigation) = ArcInvestigation.toFsWorkbook(investigation)
        #if FABLE_COMPILER_JAVASCRIPT || FABLE_COMPILER_TYPESCRIPT
        member _.fromXlsxFileAsync (path: string) = FsWorkbook.fromXlsxFile path |> CrossAsync.map ArcInvestigation.fromFsWorkbook
        member _.toXlsxFileAsync (path: string, investigation: ArcInvestigation) = ArcInvestigation.toFsWorkbook(investigation) |> FsWorkbook.toXlsxFile path
        #else
        member _.fromXlsxFile (path: string) = FsWorkbook.fromXlsxFile path |> ArcInvestigation.fromFsWorkbook
        member _.toXlsxFile (path: string, investigation: ArcInvestigation) = ArcInvestigation.toFsWorkbook(investigation) |> FsWorkbook.toXlsxFile path
        #endif

open XlsxHelper

[<AttachMembers>]
type XlsxController =
    static member Datamap = DatamapXlsx()
    static member Assay = AssayXlsx()
    static member Study = StudyXlsx()
    static member Workflow = WorkflowXlsx()
    static member Run = RunXlsx()
    static member Investigation = InvestigationXlsx()