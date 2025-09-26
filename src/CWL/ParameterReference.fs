namespace ARCtrl.CWL

open Fable.Core
open System

[<AttachMembers>]
type CWLParameterReference = {
        Key: string 
        Values: string ResizeArray
        Type: CWLType option
    }

    with

    static member create(key : string, ?values : string ResizeArray, ?type_ : CWLType) =
        { Key = key
          Values = defaultArg values (ResizeArray<string>())
          Type = type_ }