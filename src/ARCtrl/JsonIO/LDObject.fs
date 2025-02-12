namespace ARCtrl.Json

open ARCtrl
open ARCtrl.ROCrate
open System
open ARCtrl.ROCrate
open Thoth.Json.Core
open DynamicObj

[<AutoOpen>]
module LDNodeExtensions =

    type LDNode with

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString LDNode.decoder s

        /// exports in json-ld format
        static member toROCrateJsonString(?spaces) =
            fun (obj:LDNode) ->
                LDNode.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?spaces) = 
            LDNode.toROCrateJsonString(?spaces=spaces) this