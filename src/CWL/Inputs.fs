namespace ARCtrl.CWL

open CWLTypes
open Outputs.Workflow

module Inputs =

    type InputBinding = {
        Prefix: string option
        Position: int option
        ItemSeparator: string option
        Separate: bool option
    }
    
    type Input = {
        Name: string
        Type: CWLType
        InputBinding: InputBinding option
    }

    module Workflow =

        type StepInput = {
            Id: string
            Source: StepOutput option
            linkMerge: string option
            defaultValue: string option
            valueFrom: string option
        }
