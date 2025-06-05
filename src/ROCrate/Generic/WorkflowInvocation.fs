namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
open ARCtrl.Helper

[<AttachMembers>]
type LDWorkflowInvocation =

    static member schemaType = ResizeArray [LDCreateAction.schemaType; LDLabProcess.schemaType]

    static member genID(name : string, ?runName : string) =
        match runName with
        | Some rn -> $"#WorkflowInvocation_R_{rn}_{name}" |> Helper.ID.clean
        | None -> $"#WorkflowInvocation_{name}" |> Helper.ID.clean

    static member validate (wp : LDNode, ?context : LDContext) =
        LDCreateAction.validate(wp, ?context = context)
        && LDLabProcess.validate(wp, ?context = context)

    static member create(name : string, instrument : LDNode, ?objects : ResizeArray<LDNode>, ?results : ResizeArray<LDNode>, ?description : string, ?agents : ResizeArray<LDNode>, ?id : string, ?endTime : System.DateTime, ?disambiguatingDescriptions : ResizeArray<string>, ?executesLabProtocol : LDNode, ?parameterValues : ResizeArray<LDNode>, ?context : LDContext) =
        let id =
            match id with
            | Some i -> i
            | None -> LDWorkflowInvocation.genID(name)
        let at = ResizeArray ["WorkflowInvocation"]
        let ca = LDNode(id, LDWorkflowInvocation.schemaType, additionalType = at, ?context = context)
        ca.SetProperty(LDCreateAction.name, name, ?context = context)
        ca.SetOptionalProperty(LDCreateAction.object_, objects, ?context = context)
        ca.SetOptionalProperty(LDCreateAction.result, results, ?context = context)
        ca.SetProperty(LDCreateAction.instrument, instrument, ?context = context)
        ca.SetOptionalProperty(LDCreateAction.agent, agents, ?context = context)
        ca.SetOptionalProperty(LDCreateAction.description, description, ?context = context)
        ca.SetOptionalProperty(LDCreateAction.endTime, endTime, ?context = context)
        ca.SetOptionalProperty(LDCreateAction.disambiguatingDescription, disambiguatingDescriptions, ?context = context)
        ca.SetOptionalProperty(LDLabProcess.executesLabProtocol, executesLabProtocol, ?context = context)
        ca.SetOptionalProperty(LDLabProcess.parameterValue, parameterValues, ?context = context)
        ca
    