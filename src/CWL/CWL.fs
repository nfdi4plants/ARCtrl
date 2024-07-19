namespace ARCtrl.CWL

open DynamicObj
open CWLTypes
open Requirements
open Inputs
open Outputs

module CWL =

    type CWL (
            cwlVersion: string,
            cls: Class,
            outputs: Output [],
            ?baseCommand: string [],
            ?requirements: Requirement [],
            ?hints: Requirement [],
            ?inputs: Input []
        ) =
        inherit DynamicObj ()

        let mutable _cwlVersion: string = cwlVersion
        let mutable _class: Class = cls
        let mutable _outputs: Output [] = outputs
        let mutable _baseCommand: string [] option = baseCommand
        let mutable _requirements: Requirement [] option = requirements
        let mutable _hints: Requirement [] option = hints
        let mutable _inputs: Input [] option = inputs

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