namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
open ARCtrl.Helper

[<AttachMembers>]
type LDWorkflowInvocation =

    static member validate (wp : LDNode, ?context : LDContext) =
        LDCreateAction.validate(wp, ?context = context)
        && LDLabProcess.validate(wp, ?context = context)

    static member create(name : string, agent : LDNode, instrument : LDNode, ?objects : ResizeArray<LDNode>, ?results : ResizeArray<LDNode>, ?description : string, ?id : string, ?endTime : System.DateTime, ?disambiguatingDescriptions : ResizeArray<string>, ?executesLabProtocol : LDNode, ?parameterValue : LDNode, ?context : LDContext) =
        let id =
            match id with
            | Some i -> i
            | None -> $"#WorkflowInvocation_{ARCtrl.Helper.Identifier.createMissingIdentifier()}" |> Helper.ID.clean
        let objects = Option.defaultValue (ResizeArray []) objects
        let results = Option.defaultValue (ResizeArray []) results
        let ca = LDNode(id, ResizeArray [LDCreateAction.schemaType], ?context = context)
        ca.SetProperty(LDCreateAction.name, name, ?context = context)
        ca.SetProperty(LDCreateAction.agent, agent, ?context = context)
        ca.SetProperty(LDCreateAction.object_, objects, ?context = context)
        ca.SetProperty(LDCreateAction.result, results, ?context = context)
        ca.SetProperty(LDCreateAction.instrument, instrument, ?context = context)
        ca.SetOptionalProperty(LDCreateAction.description, description, ?context = context)
        ca.SetOptionalProperty(LDCreateAction.endTime, endTime, ?context = context)
        ca.SetOptionalProperty(LDCreateAction.disambiguatingDescription, disambiguatingDescriptions, ?context = context)
        ca.SetOptionalProperty(LDLabProcess.executesLabProtocol, executesLabProtocol, ?context = context)
        ca.SetOptionalProperty(LDLabProcess.parameterValue, parameterValue, ?context = context)
        ca
    