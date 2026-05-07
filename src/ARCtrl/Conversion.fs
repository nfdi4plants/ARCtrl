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
         member this.ToROCrateAssay(?groupProcesses, ?fs, ?skipDatamap) = AssayConversion.composeAssay(this, ?groupProcesses = groupProcesses, ?fs = fs, ?skipDatamap = skipDatamap)

         static member fromROCrateAssay (a : LDNode, ?graph : LDGraph, ?context : LDContext) = AssayConversion.decomposeAssay(a, ?graph = graph, ?context = context)

    type ArcStudy with
         member this.ToROCrateStudy(?groupProcesses, ?fs, ?skipDatamap) = StudyConversion.composeStudy(this, ?groupProcesses = groupProcesses, ?fs = fs, ?skipDatamap = skipDatamap)

         static member fromROCrateStudy (a : LDNode, ?graph : LDGraph, ?context : LDContext) = StudyConversion.decomposeStudy(a, ?graph = graph, ?context = context)

    type ArcWorkflow with
        member this.ToROCrateWorkflow(?fs, ?skipDatamap) = WorkflowConversion.composeWorkflow(this, ?fs = fs, ?skipDatamap = skipDatamap)

        static member fromROCrateWorkflow (a : LDNode, ?graph : LDGraph, ?context : LDContext) = WorkflowConversion.decomposeWorkflow(a, ?graph = graph, ?context = context)

    type ArcRun with
        member this.ToROCrateRun(?fs, ?skipDatamap) = RunConversion.composeRun(this, ?fs = fs, ?skipDatamap = skipDatamap)

        static member fromROCrateRun (a : LDNode, ?graph : LDGraph, ?context : LDContext) = RunConversion.decomposeRun(a, ?graph = graph, ?context = context)

    type ArcInvestigation with
        member this.ToROCrateInvestigation(?groupProcesses, ?fs, ?ignoreBrokenWR) = InvestigationConversion.composeInvestigation(this, ?groupProcesses = groupProcesses, ?fs = fs, ?ignoreBrokenWR = ignoreBrokenWR)
    
        static member fromROCrateInvestigation (a : LDNode, ?graph : LDGraph, ?context : LDContext) = InvestigationConversion.decomposeInvestigation(a, ?graph = graph, ?context = context)

    type Dataset with

        // Assay
        static member toArcAssay(a : LDNode, ?graph : LDGraph, ?context : LDContext) = AssayConversion.decomposeAssay(a, ?graph = graph, ?context = context)

        static member fromArcAssay (a : ArcAssay, ?groupProcesses : bool, ?fs : FileSystem) = AssayConversion.composeAssay(a, ?groupProcesses = groupProcesses, ?fs = fs)

        // Study
        static member toArcStudy(a : LDNode, ?graph : LDGraph, ?context : LDContext) = StudyConversion.decomposeStudy(a, ?graph = graph, ?context = context)

        static member fromArcStudy (a : ArcStudy, ?groupProcesses : bool, ?skipDatamap : bool, ?fs : FileSystem) = StudyConversion.composeStudy(a, ?groupProcesses = groupProcesses, ?skipDatamap = skipDatamap, ?fs = fs)

        // Workflow
        static member toArcWorkflow(a : LDNode, ?graph : LDGraph, ?context : LDContext) = WorkflowConversion.decomposeWorkflow(a, ?graph = graph, ?context = context)

        static member fromArcWorkflow (a : ArcWorkflow, ?skipDatamap : bool, ?fs : FileSystem) = WorkflowConversion.composeWorkflow(a, ?skipDatamap = skipDatamap, ?fs = fs)

        // Run
        static member toArcRun(a : LDNode, ?graph : LDGraph, ?context : LDContext) = RunConversion.decomposeRun(a, ?graph = graph, ?context = context)

        static member fromArcRun (a : ArcRun, ?skipDatamap : bool, ?fs : FileSystem) = RunConversion.composeRun(a, ?skipDatamap = skipDatamap, ?fs = fs)

        // Investigation
        static member toArcInvestigation(a : LDNode, ?graph : LDGraph, ?context : LDContext) = InvestigationConversion.decomposeInvestigation(a, ?graph = graph, ?context = context)

        static member fromArcInvestigation (a : ArcInvestigation, ?groupProcesses : bool, ?fs : FileSystem, ?ignoreBrokenWR) = InvestigationConversion.composeInvestigation(a, ?groupProcesses = groupProcesses, ?fs = fs, ?ignoreBrokenWR = ignoreBrokenWR)


    [<Fable.Core.AttachMembers>]
    type Conversion =

        // Assay
        static member arcAssayToDataset(a : ArcAssay, ?groupProcesses, ?fs) = a.ToROCrateAssay(?groupProcesses = groupProcesses, ?fs = fs)

        static member datasetToArcAssay(a : LDNode, ?graph : LDGraph, ?context : LDContext) =
            ArcAssay.fromROCrateAssay(a, ?graph = graph, ?context = context)

        // Study
        static member arcStudyToDataset(a : ArcStudy, ?groupProcesses, ?skipDatamap, ?fs) = a.ToROCrateStudy(?groupProcesses = groupProcesses, ?skipDatamap = skipDatamap, ?fs = fs)

        static member datasetToArcStudy(a : LDNode, ?graph : LDGraph, ?context : LDContext) =
            ArcStudy.fromROCrateStudy(a, ?graph = graph, ?context = context)

        // Workflow
        static member arcWorkflowToDataset(a : ArcWorkflow, ?skipDatamap, ?fs) = a.ToROCrateWorkflow(?skipDatamap = skipDatamap, ?fs = fs)

        static member datasetToArcWorkflow(a : LDNode, ?graph : LDGraph, ?context : LDContext) =
            ArcWorkflow.fromROCrateWorkflow(a, ?graph = graph, ?context = context)

        // Run
        static member arcRunToDataset(a : ArcRun, ?skipDatamap, ?fs) = a.ToROCrateRun(?skipDatamap = skipDatamap, ?fs = fs)

        static member datasetToArcRun(a : LDNode, ?graph : LDGraph, ?context : LDContext) =
            ArcRun.fromROCrateRun(a, ?graph = graph, ?context = context)

        // Investigation
        static member arcInvestigationToDataset(a : ArcInvestigation, ?groupProcesses, ?fs, ?ignoreBrokenWR) = a.ToROCrateInvestigation(?groupProcesses = groupProcesses, ?fs = fs, ?ignoreBrokenWR = ignoreBrokenWR)

        static member datasetToArcInvestigation(a : LDNode, ?graph : LDGraph, ?context : LDContext) =
            ArcInvestigation.fromROCrateInvestigation(a, ?graph = graph, ?context = context)       


