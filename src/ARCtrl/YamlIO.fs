namespace ARCtrl.Yaml

open ARCtrl
open System
open ARCtrl.ROCrate
open DynamicObj
open YAMLicious
open Fable.Core

[<AutoOpen>]
module LDNodeExtensions =

    type LDNode with

        static member fromROCrateYamlString (s:string) = 
            ARCtrl.Yaml.Decode.fromYamlString ARCtrl.Yaml.ROCrate.LDNode.decoder s

        static member toROCrateYamlString(?spaces) =
            fun (obj:LDNode) ->
                ARCtrl.Yaml.ROCrate.LDNode.encoder obj
                |> ARCtrl.Yaml.Encode.toYamlString (ARCtrl.Yaml.Encode.defaultWhitespace spaces)

        member this.ToROCrateYamlString(?spaces) =
            LDNode.toROCrateYamlString(?spaces=spaces) this

    [<AttachMembers>]
    type PyJsInterop =

        static member fromROCrateYamlString (s:string) = 
            ARCtrl.Yaml.Decode.fromYamlString ARCtrl.Yaml.ROCrate.LDNode.decoder s

        static member toROCrateYamlString(node : LDNode, ?spaces) =
            ARCtrl.Yaml.ROCrate.LDNode.encoder node
            |> ARCtrl.Yaml.Encode.toYamlString (ARCtrl.Yaml.Encode.defaultWhitespace spaces)
[<AutoOpen>]
module LDGraphExtensions =

    type LDGraph with

        static member fromROCrateYamlString (s:string) = 
            ARCtrl.Yaml.Decode.fromYamlString ARCtrl.Yaml.ROCrate.LDGraph.decoder s

        static member toROCrateYamlString(?spaces) =
            fun (obj:LDGraph) ->
                ARCtrl.Yaml.ROCrate.LDGraph.encoder obj
                |> ARCtrl.Yaml.Encode.toYamlString (ARCtrl.Yaml.Encode.defaultWhitespace spaces)

        member this.ToROCrateYamlString(?spaces) =
            LDGraph.toROCrateYamlString(?spaces=spaces) this


    [<AttachMembers>]
    type PyJsInterop =

        static member fromROCrateYamlString (s:string) = 
            ARCtrl.Yaml.Decode.fromYamlString ARCtrl.Yaml.ROCrate.LDGraph.decoder s

        static member toROCrateYamlString(graph : LDGraph, ?spaces) =
            ARCtrl.Yaml.ROCrate.LDGraph.encoder graph
            |> ARCtrl.Yaml.Encode.toYamlString (ARCtrl.Yaml.Encode.defaultWhitespace spaces)