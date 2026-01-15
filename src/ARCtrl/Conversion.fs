namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.FileSystem
open System.Collections.Generic
//open ColumnIndex

open ColumnIndex
open ARCtrl.Helper.Regex.ActivePatterns

[<AutoOpen>]
module TypeExtensions =

    type ArcAssay with
         member this.ToROCrateAssay(?fs) = AssayConversion.composeAssay(this, ?fs = fs)

         static member fromROCrateAssay (a : LDNode, ?graph : LDGraph, ?context : LDContext) = AssayConversion.decomposeAssay(a, ?graph = graph, ?context = context)

    type ArcStudy with
         member this.ToROCrateStudy(?fs) = StudyConversion.composeStudy(this, ?fs = fs)

         static member fromROCrateStudy (a : LDNode, ?graph : LDGraph, ?context : LDContext) = StudyConversion.decomposeStudy(a, ?graph = graph, ?context = context)

    type ArcWorkflow with
        member this.ToROCrateWorkflow(?fs) = WorkflowConversion.composeWorkflow(this, ?fs = fs)

        static member fromROCrateWorkflow (a : LDNode, ?graph : LDGraph, ?context : LDContext) = WorkflowConversion.decomposeWorkflow(a, ?graph = graph, ?context = context)

    type ArcRun with
        member this.ToROCrateRun(?fs) = RunConversion.composeRun(this, ?fs = fs)

        static member fromROCrateRun (a : LDNode, ?graph : LDGraph, ?context : LDContext) = RunConversion.decomposeRun(a, ?graph = graph, ?context = context)

    type ArcInvestigation with
        member this.ToROCrateInvestigation(?fs, ?ignoreBrokenWR) = InvestigationConversion.composeInvestigation(this, ?fs = fs, ?ignoreBrokenWR = ignoreBrokenWR)
    
        static member fromROCrateInvestigation (a : LDNode, ?graph : LDGraph, ?context : LDContext) = InvestigationConversion.decomposeInvestigation(a, ?graph = graph, ?context = context)

    type Dataset with

        // Assay
        static member toArcAssay(a : LDNode, ?graph : LDGraph, ?context : LDContext) = AssayConversion.decomposeAssay(a, ?graph = graph, ?context = context)

        static member fromArcAssay (a : ArcAssay) = AssayConversion.composeAssay a

        // Study
        static member toArcStudy(a : LDNode, ?graph : LDGraph, ?context : LDContext) = StudyConversion.decomposeStudy(a, ?graph = graph, ?context = context)

        static member fromArcStudy (a : ArcStudy) = StudyConversion.composeStudy a

        // Workflow
        static member toArcWorkflow(a : LDNode, ?graph : LDGraph, ?context : LDContext) = WorkflowConversion.decomposeWorkflow(a, ?graph = graph, ?context = context)

        static member fromArcWorkflow (a : ArcWorkflow) = WorkflowConversion.composeWorkflow a

        // Run
        static member toArcRun(a : LDNode, ?graph : LDGraph, ?context : LDContext) = RunConversion.decomposeRun(a, ?graph = graph, ?context = context)

        static member fromArcRun (a : ArcRun) = RunConversion.composeRun a

        // Investigation
        static member toArcInvestigation(a : LDNode, ?graph : LDGraph, ?context : LDContext) = InvestigationConversion.decomposeInvestigation(a, ?graph = graph, ?context = context)

        static member fromArcInvestigation (a : ArcInvestigation) = InvestigationConversion.composeInvestigation a


    [<Fable.Core.AttachMembers>]
    type Conversion =

        // Assay
        static member arcAssayToDataset(a : ArcAssay, ?fs) = a.ToROCrateAssay(?fs = fs)

        static member datasetToArcAssay(a : LDNode, ?graph : LDGraph, ?context : LDContext) =
            ArcAssay.fromROCrateAssay(a, ?graph = graph, ?context = context)

        // Study
        static member arcStudyToDataset(a : ArcStudy, ?fs) = a.ToROCrateStudy(?fs = fs)

        static member datasetToArcStudy(a : LDNode, ?graph : LDGraph, ?context : LDContext) =
            ArcStudy.fromROCrateStudy(a, ?graph = graph, ?context = context)

        // Workflow
        static member arcWorkflowToDataset(a : ArcWorkflow, ?fs) = a.ToROCrateWorkflow(?fs = fs)

        static member datasetToArcWorkflow(a : LDNode, ?graph : LDGraph, ?context : LDContext) =
            ArcWorkflow.fromROCrateWorkflow(a, ?graph = graph, ?context = context)

        // Run
        static member arcRunToDataset(a : ArcRun, ?fs) = a.ToROCrateRun(?fs = fs)

        static member datasetToArcRun(a : LDNode, ?graph : LDGraph, ?context : LDContext) =
            ArcRun.fromROCrateRun(a, ?graph = graph, ?context = context)

        // Investigation
        static member arcInvestigationToDataset(a : ArcInvestigation, ?fs, ?ignoreBrokenWR) = a.ToROCrateInvestigation(?fs = fs, ?ignoreBrokenWR = ignoreBrokenWR)

        static member datasetToArcInvestigation(a : LDNode, ?graph : LDGraph, ?context : LDContext) =
            ArcInvestigation.fromROCrateInvestigation(a, ?graph = graph, ?context = context)       


