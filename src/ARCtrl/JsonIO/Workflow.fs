namespace ARCtrl.Json

open ARCtrl
open ARCtrl.Helper

[<AutoOpen>]
module WorkflowExtensions =

    type ArcWorkflow with

        static member fromJsonString (s:string)  = 
            Decode.fromJsonString Workflow.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:ArcWorkflow) ->
                Workflow.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToJsonString(?spaces) = 
            ArcWorkflow.toJsonString(?spaces=spaces) this
