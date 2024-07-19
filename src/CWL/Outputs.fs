namespace ARCtrl.CWL

open CWLTypes

module Outputs =

    type OutputBinding = {
        Glob: string option
    }

    type Output = {
        Name: string
        Type: CWLType
        OutputBinding: OutputBinding option
    }

