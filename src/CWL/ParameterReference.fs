namespace ARCtrl.CWL

open Fable.Core
open System

[<AttachMembers>]
type CWLParameterReference = {
    Key: string 
    Values: string ResizeArray
    Type: string option
}

