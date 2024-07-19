namespace ARCtrl.CWL

open CWLTypes

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
