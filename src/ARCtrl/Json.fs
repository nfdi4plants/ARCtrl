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
        member _.fromROCrateJsonString (s: string) = LDNode.fromROCrateJsonString s |> ARCtrl.Conversion.BaseTypes.decomposeDefinedTerm
        member _.toJsonString (oa: OntologyAnnotation, ?spaces) = OntologyAnnotation.toJsonString(?spaces=spaces) oa
        member _.toISAJsonString (oa: OntologyAnnotation, ?spaces) = OntologyAnnotation.toISAJsonString(?spaces=spaces) oa
        member _.toROCrateJsonString(oa: OntologyAnnotation, ?spaces) = ARCtrl.Conversion.BaseTypes.composeDefinedTerm oa |> LDNode.toROCrateJsonString(?spaces=spaces)

    [<AttachMembers>]
    type PersonJson() =
        member _.fromJsonString (s: string) = Person.fromJsonString s
        member _.fromISAJsonString (s: string) = Person.fromISAJsonString s
        member _.fromROCrateJsonString (s: string) = LDNode.fromROCrateJsonString s |> ARCtrl.Conversion.PersonConversion.decomposePerson
        member _.toJsonString (person: ARCtrl.Person, ?spaces) = Person.toJsonString(?spaces=spaces) person
        member _.toISAJsonString (person: ARCtrl.Person, ?spaces, ?useIDReferencing) = Person.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) person
        member _.toROCrateJsonString(person: ARCtrl.Person, ?spaces) = ARCtrl.Conversion.PersonConversion.composePerson person |> LDNode.toROCrateJsonString(?spaces=spaces)

    [<AttachMembers>]
    type DatamapJson() =
        member _.fromJsonString (s: string) = Datamap.fromJsonString s
        member _.toJsonString (datamap: Datamap, ?spaces) = Datamap.toJsonString(?spaces=spaces) datamap

    [<AttachMembers>]
    type AssayJson() =
        member _.fromJsonString (s: string) = ArcAssay.fromJsonString s
        member _.fromCompressedJsonString (s: string) = ArcAssay.fromCompressedJsonString s
        member _.fromISAJsonString (s: string) = ArcAssay.fromISAJsonString s
        member _.fromROCrateJsonString (s: string) = LDNode.fromROCrateJsonString s |> ARCtrl.Conversion.AssayConversion.decomposeAssay
        member _.toJsonString (assay: ArcAssay, ?spaces) = ArcAssay.toJsonString(?spaces=spaces) assay
        member _.toCompressedJsonString (assay: ArcAssay,?spaces) = ArcAssay.toCompressedJsonString(?spaces=spaces) assay
        member _.toISAJsonString (assay: ArcAssay, ?spaces, ?useIDReferencing) = ArcAssay.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) assay
        member _.toROCrateJsonString(assay: ArcAssay, ?spaces) = ARCtrl.Conversion.AssayConversion.composeAssay assay |> LDNode.toROCrateJsonString(?spaces=spaces)

    [<AttachMembers>]
    type StudyJson() =
        member _.fromJsonString (s: string) = ArcStudy.fromJsonString s
        member _.fromCompressedJsonString (s: string) = ArcStudy.fromCompressedJsonString s
        member _.fromISAJsonString (s: string) = ArcStudy.fromISAJsonString s
        member _.fromROCrateJsonString (s: string) = LDNode.fromROCrateJsonString s |> ARCtrl.Conversion.StudyConversion.decomposeStudy
        member _.toJsonString (study: ArcStudy, ?spaces) = ArcStudy.toJsonString(?spaces=spaces) study
        member _.toCompressedJsonString (study: ArcStudy, ?spaces) = ArcStudy.toCompressedJsonString(?spaces=spaces) study
        member _.toISAJsonString (study: ArcStudy, ?assays,?spaces, ?useIDReferencing) = ArcStudy.toISAJsonString(?assays=assays,?spaces=spaces, ?useIDReferencing = useIDReferencing) study
        member _.toROCrateJsonString(study: ArcStudy, ?assays,?spaces) = ARCtrl.Conversion.StudyConversion.composeStudy study |> LDNode.toROCrateJsonString(?spaces=spaces)
    [<AttachMembers>]
    type WorkflowJson() =
        member _.fromJsonString (s: string) = ArcWorkflow.fromJsonString s
        member _.fromCompressedJsonString (s: string) = ArcWorkflow.fromCompressedJsonString s
        member _.toJsonString (workflow: ArcWorkflow, ?spaces) = ArcWorkflow.toJsonString(?spaces=spaces) workflow
        member _.toCompressedJsonString (workflow: ArcWorkflow, ?spaces) = ArcWorkflow.toCompressedJsonString(?spaces=spaces) workflow

    [<AttachMembers>]
    type RunJson() =
        member _.fromJsonString (s: string) = ArcRun.fromJsonString s
        member _.fromCompressedJsonString (s: string) = ArcRun.fromCompressedJsonString s
        member _.toJsonString (run: ArcRun, ?spaces) = ArcRun.toJsonString(?spaces=spaces) run
        member _.toCompressedJsonString (run: ArcRun, ?spaces) = ArcRun.toCompressedJsonString(?spaces=spaces) run

    [<AttachMembers>]
    type InvestigationJson() =
        member _.fromJsonString (s: string) = ArcInvestigation.fromJsonString s
        member _.fromCompressedJsonString (s: string) = ArcInvestigation.fromCompressedJsonString s
        member _.fromISAJsonString (s: string) = ArcInvestigation.fromISAJsonString s
        member _.fromROCrateJsonString (s: string) = LDNode.fromROCrateJsonString s |> ARCtrl.Conversion.InvestigationConversion.decomposeInvestigation
        member _.toJsonString (investigation: ArcInvestigation, ?spaces) = ArcInvestigation.toJsonString(?spaces=spaces) investigation
        member _.toCompressedJsonString (investigation: ArcInvestigation, ?spaces) = ArcInvestigation.toCompressedJsonString(?spaces=spaces) investigation
        member _.toISAJsonString (investigation: ArcInvestigation, ?spaces, ?useIDReferencing) = ArcInvestigation.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) investigation
        member _.toROCrateJsonString(investigation: ArcInvestigation, ?spaces) = ARCtrl.Conversion.InvestigationConversion.composeInvestigation investigation |> LDNode.toROCrateJsonString(?spaces=spaces)

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
    static member Person = PersonJson()
    static member Datamap = DatamapJson()
    static member Assay = AssayJson()
    static member Study = StudyJson()
    static member Workflow = WorkflowJson()
    static member Run = RunJson()
    static member Investigation = InvestigationJson()
    static member ARC = ARCJson()
    static member LDGraph = LDGraphJson()
    static member LDNode = LDNodeJson()