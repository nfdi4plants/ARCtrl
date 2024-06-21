namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module Component =

    module ROCrate =

        let encoder : Component -> Json= 
            PropertyValue.ROCrate.encoder<Component>

        let decoder : Decoder<Component> =
            PropertyValue.ROCrate.decoder<Component> (Component.createAsPV)
    
    module ISAJson =

        let genID (c : Component) = 
            PropertyValue.ROCrate.genID c

        let encoder (idMap : IDTable.IDTableWrite option) (c : Component) = 
            let f (c : Component) =
                [
                    Encode.tryInclude "@id" Encode.string (c |> genID |> Some)
                    Encode.tryInclude "componentName" Encode.string c.ComponentName
                    Encode.tryInclude "componentType" (OntologyAnnotation.ISAJson.encoder idMap) c.ComponentType
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f c
            | Some idMap -> IDTable.encode genID f c idMap

        let decoder: Decoder<Component> =
            Decode.object (fun get ->
                let name = get.Optional.Field "componentName" Decode.uri
                let value, unit =
                    match name with
                    | Some n -> 
                        let v,u = Component.decomposeName n
                        Some v, u
                    | None -> None, None
                {
                    ComponentValue = value
                    ComponentUnit = unit
                    ComponentType = get.Optional.Field "componentType" OntologyAnnotation.ISAJson.decoder
                }
            )

[<AutoOpen>]
module ComponentExtensions =
    
    type Component with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Component.ISAJson.decoder s   

        static member toISAJsonString(?spaces, ?useIDReferencing) =
            let useIDReferencing = Option.defaultValue false useIDReferencing
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (f:Component) ->
                Component.ISAJson.encoder idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toISAJsonString(?spaces) =
            Component.toISAJsonString(?spaces=spaces) this

        static member fromROCrateJsonString (s:string) =
            Decode.fromJsonString Component.ROCrate.decoder s

        static member toROCrateJsonString(?spaces) =
            fun (f:Component) ->
                Component.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.toROCrateJsonString(?spaces) =
            Component.toROCrateJsonString(?spaces=spaces) this