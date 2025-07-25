namespace ARCtrl

open Thoth.Json.Core
open ARCtrl.Json
open ARCtrl.ROCrate
open Fable.Core

module JsonHelper =

    [<AttachMembers>]
    type OntologyAnnotationJson() =
        member _.fromJsonString (s: string) = OntologyAnnotation.fromJsonString s
        member _.fromISAJsonString (s: string) = OntologyAnnotation.fromISAJsonString s
        member _.fromROCrateJsonString (s: string) = OntologyAnnotation.fromROCrateJsonString s
        member _.toJsonString (oa: OntologyAnnotation, ?spaces) = OntologyAnnotation.toJsonString(?spaces=spaces) oa
        member _.toISAJsonString (oa: OntologyAnnotation, ?spaces) = OntologyAnnotation.toISAJsonString(?spaces=spaces) oa
        member _.toROCrateJsonString(oa: OntologyAnnotation, ?spaces) = OntologyAnnotation.toROCrateJsonString(?spaces=spaces) oa

    [<AttachMembers>]
    type AssayJson() =
        member _.fromJsonString (s: string) = ArcAssay.fromJsonString s
        member _.fromCompressedJsonString (s: string) = ArcAssay.fromCompressedJsonString s
        member _.fromISAJsonString (s: string) = ArcAssay.fromISAJsonString s
        member _.fromROCrateJsonString (s: string) = ArcAssay.fromROCrateJsonString s
        member _.toJsonString (assay: ArcAssay, ?spaces) = ArcAssay.toJsonString(?spaces=spaces) assay
        member _.toCompressedJsonString (assay: ArcAssay,?spaces) = ArcAssay.toCompressedJsonString(?spaces=spaces) assay
        member _.toISAJsonString (assay: ArcAssay, ?spaces, ?useIDReferencing) = ArcAssay.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) assay
        member _.toROCrateJsonString(assay: ArcAssay, studyName, ?spaces) = ArcAssay.toROCrateJsonString(studyName, ?spaces=spaces) assay

    [<AttachMembers>]
    type StudyJson() =
        member _.fromJsonString (s: string) = ArcStudy.fromJsonString s
        member _.fromCompressedJsonString (s: string) = ArcStudy.fromCompressedJsonString s
        member _.fromISAJsonString (s: string) = ArcStudy.fromISAJsonString s
        member _.fromROCrateJsonString (s: string) = ArcStudy.fromROCrateJsonString s
        member _.toJsonString (study: ArcStudy, ?spaces) = ArcStudy.toJsonString(?spaces=spaces) study
        member _.toCompressedJsonString (study: ArcStudy, ?spaces) = ArcStudy.toCompressedJsonString(?spaces=spaces) study
        member _.toISAJsonString (study: ArcStudy, ?assays,?spaces, ?useIDReferencing) = ArcStudy.toISAJsonString(?assays=assays,?spaces=spaces, ?useIDReferencing = useIDReferencing) study
        member _.toROCrateJsonString(study: ArcStudy, ?assays,?spaces) = ArcStudy.toROCrateJsonString(?assays=assays,?spaces=spaces) study

    [<AttachMembers>]
    type InvestigationJson() =
        member _.fromJsonString (s: string) = ArcInvestigation.fromJsonString s
        member _.fromCompressedJsonString (s: string) = ArcInvestigation.fromCompressedJsonString s
        member _.fromISAJsonString (s: string) = ArcInvestigation.fromISAJsonString s
        member _.fromROCrateJsonString (s: string) = ArcInvestigation.fromROCrateJsonString s
        member _.toJsonString (investigation: ArcInvestigation, ?spaces) = ArcInvestigation.toJsonString(?spaces=spaces) investigation
        member _.toCompressedJsonString (investigation: ArcInvestigation, ?spaces) = ArcInvestigation.toCompressedJsonString(?spaces=spaces) investigation
        member _.toISAJsonString (investigation: ArcInvestigation, ?spaces, ?useIDReferencing) = ArcInvestigation.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) investigation
        member _.toROCrateJsonString(investigation: ArcInvestigation, ?spaces) = ArcInvestigation.toROCrateJsonString(?spaces=spaces) investigation

    [<AttachMembers>]
    type ARCJson() =
        member _.fromROCrateJsonString (s: string) = ARC.fromROCrateJsonString s
        member _.toROCrateJsonString(?spaces) = ARC.toROCrateJsonString(?spaces=spaces)

    [<AttachMembers>]
    type LDGraphJson() =
        member _.fromROCrateJsonString (s : string)  = LDGraph.fromROCrateJsonString s
        member _.toROCrateJsonString(?spaces) = LDGraph.toROCrateJsonString(?spaces=spaces)

    [<AttachMembers>]
    type LDNodeJson() =
        member _.fromROCrateJsonString (s : string) = LDNode.fromROCrateJsonString s
        member _.toROCrateJsonString(?spaces) = LDNode.toROCrateJsonString(?spaces=spaces)

open JsonHelper

[<AttachMembers>]
type JsonController =
    static member OntologyAnnotation = OntologyAnnotationJson()
    static member Assay = AssayJson()
    static member Study = StudyJson()
    static member Investigation = InvestigationJson()
    static member ARC = ARCJson()
    static member LDGraph = LDGraphJson()
    static member LDNode = LDNodeJson()