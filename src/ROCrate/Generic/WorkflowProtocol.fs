namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
open ARCtrl.Helper

[<AttachMembers>]
type LDWorkflowProtocol =

    static member schemaType =
        ResizeArray [LDFile.schemaType; LDComputationalWorkflow.schemaType; LDSoftwareSourceCode.schemaType; LDLabProtocol.schemaType]

    static member validate (wp : LDNode, ?context : LDContext) =
        LDComputationalWorkflow.validate(wp, ?context = context)
        && LDSoftwareSourceCode.validate(wp, ?context = context)
        && LDLabProtocol.validate(wp, ?context = context)

    static member create
        (
            ?id : string,
            ?inputs : ResizeArray<LDNode>,
            ?outputs : ResizeArray<LDNode>,
            ?creator : LDNode,
            ?dateCreated : string,
            ?licenses : ResizeArray<LDNode>,
            ?name : string,
            ?programmingLanguages : ResizeArray<LDNode>,
            ?sdPublisher : LDNode,
            ?url : string,
            ?version : string,
            ?description : string,
            ?hasParts : ResizeArray<LDNode>,
            ?intendedUse : LDNode,
            ?comments : ResizeArray<LDNode>,
            ?computationalTools : ResizeArray<LDNode>,
            ?context : LDContext
        ) =
        let id =
            match id with
            | Some i -> i
            | None -> $"#ComputationalWorkflow_{ARCtrl.Helper.Identifier.createMissingIdentifier()}" |> Helper.ID.clean
        let at = ResizeArray ["WorkflowProtocol"]
        let wp = LDNode(id, LDWorkflowProtocol.schemaType, additionalType = at, ?context = context)
        wp.SetOptionalProperty(LDComputationalWorkflow.input, inputs, ?context = context)
        wp.SetOptionalProperty(LDComputationalWorkflow.output, outputs, ?context = context)
        wp.SetOptionalProperty(LDComputationalWorkflow.creator, creator, ?context = context)
        wp.SetOptionalProperty(LDComputationalWorkflow.dateCreated, dateCreated, ?context = context)
        wp.SetOptionalProperty(LDComputationalWorkflow.license, licenses, ?context = context)
        wp.SetOptionalProperty(LDComputationalWorkflow.name, name, ?context = context)
        wp.SetOptionalProperty(LDComputationalWorkflow.programmingLanguage, programmingLanguages, ?context = context)
        wp.SetOptionalProperty(LDComputationalWorkflow.sdPublisher, sdPublisher, ?context = context)
        wp.SetOptionalProperty(LDComputationalWorkflow.url, url, ?context = context)
        wp.SetOptionalProperty(LDComputationalWorkflow.version, version, ?context = context)
        wp.SetOptionalProperty(LDComputationalWorkflow.description, description, ?context = context)
        wp.SetOptionalProperty(LDComputationalWorkflow.hasPart, hasParts, ?context = context)
        wp.SetOptionalProperty(LDLabProtocol.intendedUse, intendedUse, ?context = context)
        wp.SetOptionalProperty(LDLabProtocol.computationalTool, computationalTools, ?context = context)
        wp.SetOptionalProperty(LDComputationalWorkflow.comment, comments, ?context = context)
        wp
    