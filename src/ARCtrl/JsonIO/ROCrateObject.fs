namespace ARCtrl.Json

open ARCtrl
open ARCtrl.ROCrate
open System
open ARCtrl.ROCrate
open Thoth.Json.Core
open DynamicObj

[<AutoOpen>]
module ROCrateObjectExtensions =

    type ROCrateObject with

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString ROCrateObject.decoder s

        /// exports in json-ld format
        static member toROCrateJsonString(?spaces) =
            fun (obj:ROCrateObject) ->
                ROCrateObject.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?spaces) = 
            ROCrateObject.toROCrateJsonString(?spaces=spaces) this