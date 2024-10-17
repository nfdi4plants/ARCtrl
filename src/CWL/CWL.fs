namespace ARCtrl

open ARCtrl.CWL
open DynamicObj
open CWLTypes
open Requirements
open Inputs
open Outputs
open WorkflowSteps

module CWLProcessingUnits =

    type CWLToolDescription (
            cwlVersion: string,
            cls: CWLClass,
            outputs: Output [],
            ?baseCommand: string [],
            ?requirements: Requirement [],
            ?hints: Requirement [],
            ?inputs: Input [],
            ?metadata: DynamicObj
        ) =
        inherit DynamicObj ()

        let mutable _cwlVersion: string = cwlVersion
        let mutable _class: CWLClass = cls
        let mutable _outputs: Output [] = outputs
        let mutable _baseCommand: string [] option = baseCommand
        let mutable _requirements: Requirement [] option = requirements
        let mutable _hints: Requirement [] option = hints
        let mutable _inputs: Input [] option = inputs
        let mutable _metadata: DynamicObj option = metadata

        member this.CWLVersion
            with get() = _cwlVersion
            and set(version) = _cwlVersion <- version

        member this.Class
            with get() = _class
            and set(cls) = _class <- cls

        member this.Outputs
            with get() = _outputs
            and set(outputs) = _outputs <- outputs

        member this.BaseCommand
            with get() = _baseCommand
            and set(baseCommand) = _baseCommand <- baseCommand

        member this.Requirements
            with get() = _requirements
            and set(requirements) = _requirements <- requirements

        member this.Hints
            with get() = _hints
            and set(hints) = _hints <- hints

        member this.Inputs
            with get() = _inputs
            and set(inputs) = _inputs <- inputs

        member this.Metadata
            with get() = _metadata
            and set(metadata) = _metadata <- metadata

    type CWLWorkflowDescription(
        cwlVersion: string,
        cls: CWLClass,
        steps: WorkflowStep [],
        inputs: Input [],
        outputs: Output [],
        ?requirements: Requirement [],
        ?hints: Requirement [],
        ?metadata: DynamicObj
    ) =
        inherit DynamicObj()

        let mutable _cwlVersion: string = cwlVersion
        let mutable _class: CWLClass = cls
        let mutable _steps: WorkflowStep [] = steps
        let mutable _inputs: Input [] = inputs
        let mutable _outputs: Output [] = outputs
        let mutable _requirements: Requirement [] option = requirements
        let mutable _hints: Requirement [] option = hints
        let mutable _metadata: DynamicObj option = metadata

        member this.CWLVersion
            with get() = _cwlVersion
            and set(version) = _cwlVersion <- version

        member this.Class
            with get() = _class
            and set(cls) = _class <- cls

        member this.Steps
            with get() = _steps
            and set(steps) = _steps <- steps

        member this.Inputs
            with get() = _inputs
            and set(inputs) = _inputs <- inputs

        member this.Outputs
            with get() = _outputs
            and set(outputs) = _outputs <- outputs

        member this.Requirements
            with get() = _requirements
            and set(requirements) = _requirements <- requirements

        member this.Hints
            with get() = _hints
            and set(hints) = _hints <- hints

        member this.Metadata
            with get() = _metadata
            and set(metadata) = _metadata <- metadata