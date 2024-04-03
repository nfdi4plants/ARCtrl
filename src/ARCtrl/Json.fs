namespace ARCtrl

open Thoth.Json.Core
open ARCtrl.Json
open Fable.Core

module JsonHelper =

    [<AttachMembers>]
    type AssayJson() =
        member this.fromJsonString (s: string) = ArcAssay.fromJsonString s
        member this.toJsonString (?spaces) = ArcAssay.toJsonString(?spaces=spaces)
        member this.fromCompressedJsonString (s: string) = ArcAssay.fromCompressedJsonString s
        member this.toCompressedJsonString (?spaces) = ArcAssay.toCompressedJsonString(?spaces=spaces)
        member this.fromISAJsonString (s: string) = ArcAssay.fromISAJsonString s
        member this.toISAJsonString (?spaces) = ArcAssay.toISAJsonString(?spaces=spaces)
        member this.fromROCrateJsonString (s: string) = ArcAssay.fromROCrateJsonString s
        member this.toROCrateJsonString(studyName, ?spaces) = ArcAssay.toROCrateJsonString(studyName, ?spaces=spaces)

    [<AttachMembers>]
    type StudyJson() =
        member this.fromJsonString (s: string) = ArcStudy.fromJsonString s
        member this.toJsonString (?spaces) = ArcStudy.toJsonString(?spaces=spaces)
        member this.fromCompressedJsonString (s: string) = ArcStudy.fromCompressedJsonString s
        member this.toCompressedJsonString (?spaces) = ArcStudy.toCompressedJsonString(?spaces=spaces)
        member this.fromISAJsonString (s: string) = ArcStudy.fromISAJsonString s
        member this.toISAJsonString (?assays,?spaces) = ArcStudy.toISAJsonString(?assays=assays,?spaces=spaces)
        member this.fromROCrateJsonString (s: string) = ArcStudy.fromROCrateJsonString s
        member this.toROCrateJsonString(?assays,?spaces) = ArcStudy.toROCrateJsonString(?assays=assays,?spaces=spaces)

    [<AttachMembers>]
    type InvestigationJson() =
        member this.fromJsonString (s: string) = ArcInvestigation.fromJsonString s
        member this.toJsonString (?spaces) = ArcInvestigation.toJsonString(?spaces=spaces)
        member this.fromCompressedJsonString (s: string) = ArcInvestigation.fromCompressedJsonString s
        member this.toCompressedJsonString (?spaces) = ArcInvestigation.toCompressedJsonString(?spaces=spaces)
        member this.fromISAJsonString (s: string) = ArcInvestigation.fromISAJsonString s
        member this.toISAJsonString (?spaces) = ArcInvestigation.toISAJsonString(?spaces=spaces)
        member this.fromROCrateJsonString (s: string) = ArcInvestigation.fromROCrateJsonString s
        member this.toROCrateJsonString(?spaces) = ArcInvestigation.toROCrateJsonString(?spaces=spaces)

    [<AttachMembers>]
    type ARCJson() =
        member this.fromROCrateJsonString (s: string) = ARC.fromROCrateJsonString s
        member this.toROCrateJsonString(?spaces) = ARC.toROCrateJsonString(?spaces=spaces)

open JsonHelper

[<AttachMembers>]
type JsonController =
    static member Assay = AssayJson()
    static member Study = StudyJson()
    static member Investigation = InvestigationJson()
    static member ARC = ARCJson()