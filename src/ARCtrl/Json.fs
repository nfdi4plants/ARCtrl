namespace ARCtrl

open Thoth.Json.Core
open ARCtrl.Json
open ARCtrl.ROCrate
open Fable.Core

module JsonHelper =

    [<AttachMembers>]
    type CommentJson() =
        member _.fromJsonString (s: string) = Comment.fromJsonString s
        member _.fromISAJsonString (s: string) = Comment.fromISAJsonString s
        member _.fromROCrateJsonString (s: string) = Comment.fromROCrateJsonString s
        member _.toJsonString (comment: Comment, ?spaces) = Comment.toJsonString(?spaces=spaces) comment
        member _.toISAJsonString (comment: Comment, ?spaces) = Comment.toISAJsonString(?spaces=spaces) comment
        member _.toROCrateJsonString (comment: Comment, ?spaces) = Comment.toROCrateJsonString(?spaces=spaces) comment

    [<AttachMembers>]
    type OntologySourceReferenceJson() =
        member _.fromJsonString (s: string) = OntologySourceReference.fromJsonString s
        member _.fromISAJsonString (s: string) = OntologySourceReference.fromISAJsonString s
        member _.fromROCrateJsonString (s: string) = OntologySourceReference.fromROCrateJsonString s
        member _.toJsonString (reference: OntologySourceReference, ?spaces) = OntologySourceReference.toJsonString(?spaces=spaces) reference
        member _.toISAJsonString (reference: OntologySourceReference, ?spaces) = OntologySourceReference.toISAJsonString(?spaces=spaces) reference
        member _.toROCrateJsonString (reference: OntologySourceReference, ?spaces) = OntologySourceReference.toROCrateJsonString(?spaces=spaces) reference

    [<AttachMembers>]
    type OntologyAnnotationJson() =
        member _.fromJsonString (s: string) = OntologyAnnotation.fromJsonString s
        member _.fromISAJsonString (s: string) = OntologyAnnotation.fromISAJsonString s
        member _.fromROCrateJsonString (s: string) = OntologyAnnotation.fromROCrateJsonString s
        member _.toJsonString (oa: OntologyAnnotation, ?spaces) = OntologyAnnotation.toJsonString(?spaces=spaces) oa
        member _.toISAJsonString (oa: OntologyAnnotation, ?spaces) = OntologyAnnotation.toISAJsonString(?spaces=spaces) oa
        member _.toROCrateJsonString(oa: OntologyAnnotation, ?spaces) = OntologyAnnotation.toROCrateJsonString(?spaces=spaces) oa

    [<AttachMembers>]
    type PersonJson() =
        member _.fromJsonString (s: string) = Person.fromJsonString s
        member _.fromISAJsonString (s: string) = Person.fromISAJsonString s
        member _.fromROCrateJsonString (s: string) = Person.fromROCrateJsonString s
        member _.toJsonString (person: ARCtrl.Person, ?spaces) = Person.toJsonString(?spaces=spaces) person
        member _.toISAJsonString (person: ARCtrl.Person, ?spaces, ?useIDReferencing) = Person.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) person
        member _.toROCrateJsonString(person: ARCtrl.Person, ?spaces) = ARCtrl.Person.toROCrateJsonString(?spaces=spaces) person

    [<AttachMembers>]
    type PublicationJson() =
        member _.fromJsonString (s: string) = Publication.fromJsonString s
        member _.fromISAJsonString (s: string) = Publication.fromISAJsonString s
        member _.fromROCrateJsonString (s: string) = Publication.fromROCrateJsonString s
        member _.toJsonString (publication: Publication, ?spaces) = Publication.toJsonString(?spaces=spaces) publication
        member _.toISAJsonString (publication: Publication, ?spaces, ?useIDReferencing) = Publication.toISAJsonString(?spaces=spaces, ?useIDReferencing=useIDReferencing) publication
        member _.toROCrateJsonString (publication: Publication, ?spaces) = Publication.toROCrateJsonString(?spaces=spaces) publication

    [<AttachMembers>]
    type IOTypeJson() =
        member _.fromJsonString (s: string) = IOType.fromJsonString s
        member _.toJsonString (ioType: IOType, ?spaces) = IOType.toJsonString(?spaces=spaces) ioType

    [<AttachMembers>]
    type CompositeHeaderJson() =
        member _.fromJsonString (s: string) = CompositeHeader.fromJsonString s
        member _.toJsonString (header: CompositeHeader, ?spaces) = CompositeHeader.toJsonString(?spaces=spaces) header

    [<AttachMembers>]
    type CompositeCellJson() =
        member _.fromJsonString (s: string) = CompositeCell.fromJsonString s
        member _.toJsonString (cell: CompositeCell, ?spaces) = CompositeCell.toJsonString(?spaces=spaces) cell

    [<AttachMembers>]
    type ArcTableJson() =
        member _.fromJsonString (s: string) = ArcTable.fromJsonString s
        member _.fromCompressedJsonString (s: string) = ArcTable.fromCompressedJsonString s
        member _.toJsonString (table: ArcTable, ?spaces) = ArcTable.toJsonString(?spaces=spaces) table
        member _.toCompressedJsonString (table: ArcTable, ?spaces) = ArcTable.toCompressedJsonString(?spaces=spaces) table

    [<AttachMembers>]
    type DataJson() =
        member _.fromJsonString (s: string) = Decode.fromJsonString Data.decoder s
        member _.fromISAJsonString (s: string) = Data.fromISAJsonString s
        member _.fromROCrateJsonString (s: string) = Data.fromROCrateJsonString s
        member _.toJsonString (data: Data, ?spaces) = Data.encoder data |> Encode.toJsonString (Encode.defaultSpaces spaces)
        member _.toISAJsonString (data: Data, ?spaces, ?useIDReferencing) = Data.toISAJsonString(?spaces=spaces, ?useIDReferencing=useIDReferencing) data
        member _.toROCrateJsonString (data: Data, ?spaces) = Data.toROCrateJsonString(?spaces=spaces) data

    [<AttachMembers>]
    type DataFileJson() =
        member _.fromISAJsonString (s: string) = DataFile.fromISAJsonString s
        member _.fromROCrateJsonString (s: string) = DataFile.fromROCrateJsonString s
        member _.toISAJsonString (dataFile: DataFile, ?spaces) = DataFile.toISAJsonString(?spaces=spaces) dataFile
        member _.toROCrateJsonString (dataFile: DataFile, ?spaces) = DataFile.toROCrateJsonString(?spaces=spaces) dataFile

    [<AttachMembers>]
    type DataContextJson() =
        member _.fromJsonString (s: string) = Decode.fromJsonString DataContext.decoder s
        member _.toJsonString (dataContext: DataContext, ?spaces) = DataContext.encoder dataContext |> Encode.toJsonString (Encode.defaultSpaces spaces)

    [<AttachMembers>]
    type TemplateJson() =
        member _.fromJsonString (s: string) = Template.fromJsonString s
        member _.fromCompressedJsonString (s: string) = Template.fromCompressedJsonString s
        member _.toJsonString (template: Template, ?spaces) = Template.toJsonString(?spaces=spaces) template
        member _.toCompressedJsonString (template: Template, ?spaces) = Template.toCompressedJsonString(?spaces=spaces) template

    [<AttachMembers>]
    type DatamapJson() =
        member _.fromJsonString (s: string) = Datamap.fromJsonString s
        member _.toJsonString (datamap: Datamap, ?spaces) = Datamap.toJsonString(?spaces=spaces) datamap

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
        //member _.fromROCrateJsonString (s: string) = ArcInvestigation.fromROCrateJsonString s
        member _.toJsonString (investigation: ArcInvestigation, ?spaces) = ArcInvestigation.toJsonString(?spaces=spaces) investigation
        member _.toCompressedJsonString (investigation: ArcInvestigation, ?spaces) = ArcInvestigation.toCompressedJsonString(?spaces=spaces) investigation
        member _.toISAJsonString (investigation: ArcInvestigation, ?spaces, ?useIDReferencing) = ArcInvestigation.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) investigation
        //member _.toROCrateJsonString(investigation: ArcInvestigation, ?spaces) = ArcInvestigation.toROCrateJsonString(?spaces=spaces) investigation

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
    static member Comment = CommentJson()
    static member OntologySourceReference = OntologySourceReferenceJson()
    static member OntologyAnnotation = OntologyAnnotationJson()
    static member Person = PersonJson()
    static member Publication = PublicationJson()
    static member IOType = IOTypeJson()
    static member CompositeHeader = CompositeHeaderJson()
    static member CompositeCell = CompositeCellJson()
    static member ArcTable = ArcTableJson()
    static member Data = DataJson()
    static member DataFile = DataFileJson()
    static member DataContext = DataContextJson()
    static member Template = TemplateJson()
    static member Datamap = DatamapJson()
    static member Assay = AssayJson()
    static member Study = StudyJson()
    static member Workflow = WorkflowJson()
    static member Run = RunJson()
    static member Investigation = InvestigationJson()
    static member ARC = ARCJson()
    static member LDGraph = LDGraphJson()
    static member LDNode = LDNodeJson()
