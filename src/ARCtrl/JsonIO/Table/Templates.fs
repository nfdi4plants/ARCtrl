namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open ARCtrl.Helper

module Templates =

    let encoder (templates: Template []) =
        templates 
        |> Array.map (Template.encoder)
        |> Encode.array

    let decoder =
        Decode.array Template.decoder
        
    let fromJsonString (jsonString: string) =
        try Decode.fromJsonString decoder jsonString with
        | exn         -> failwithf "Error. Given json string cannot be parsed to Templates map: %A" exn

    let toJsonString (spaces: int) (templates: Template []) =
        Encode.toJsonString spaces (encoder templates)

[<AutoOpen>]
module TemplateExtension =

    type Template with

        static member fromJsonString (jsonString: string) =
            try Decode.fromJsonString Template.decoder jsonString with
            | exn     -> failwithf "Error. Given json string cannot be parsed to Template: %A" exn

        static member toJsonString(?spaces: int) =
            fun (template:Template) ->
                Encode.toJsonString (Encode.defaultSpaces spaces) (Template.encoder template)

        member this.toJsonString(?spaces: int) =
            Template.toJsonString(?spaces=spaces) this

        static member fromCompressedJsonString (s: string) =
            try Decode.fromJsonString (Compression.decode Template.decoderCompressed) s with
            | e -> failwithf "Error. Unable to parse json string to ArcStudy: %s" e.Message

        static member toCompressedJsonString(?spaces) =
            fun (obj:Template) ->
                let spaces = defaultArg spaces 0
                Encode.toJsonString spaces (Compression.encode Template.encoderCompressed obj)

        member this.toCompressedJsonString(?spaces) =
            Template.toCompressedJsonString(?spaces=spaces) this
