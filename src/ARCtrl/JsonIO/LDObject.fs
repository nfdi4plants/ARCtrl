namespace ARCtrl.Json

open ARCtrl
open ARCtrl.ROCrate
open System
open ARCtrl.ROCrate
open Thoth.Json.Core
open DynamicObj

[<AutoOpen>]
module LDObjectExtensions =

    type LDObject with

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString LDObject.decoder s

        /// exports in json-ld format
        static member toROCrateJsonString(?spaces) =
            fun (obj:LDObject) ->
                LDObject.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?spaces) = 
            LDObject.toROCrateJsonString(?spaces=spaces) this