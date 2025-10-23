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

        static member fromCompressedJsonString (s:string)  =
            try Decode.fromJsonString (Compression.decode Workflow.decoderCompressed) s with
            | e -> failwithf "Error. Unable to parse json string to ArcAssay: %s" e.Message

        static member toCompressedJsonString(?spaces) =
            fun (obj:ArcWorkflow) ->
                let spaces = defaultArg spaces 0
                Encode.toJsonString spaces (Compression.encode Workflow.encoderCompressed obj)

        member this.ToCompressedJsonString(?spaces) = 
            ArcWorkflow.toCompressedJsonString(?spaces=spaces) this