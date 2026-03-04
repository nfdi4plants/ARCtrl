namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module Component =

    let encoder (value : Component) =
        [
            Encode.tryInclude "type" OntologyAnnotation.encoder value.ComponentType
            Encode.tryInclude "value" (Value.ISAJson.encoder None) value.ComponentValue
            Encode.tryInclude "unit" OntologyAnnotation.encoder value.ComponentUnit
        ]
        |> Encode.choose
        |> Encode.object

    let decoder : Decoder<Component> =
        Decode.object (fun get ->
            {
                ComponentType = get.Optional.Field "type" OntologyAnnotation.decoder
                ComponentValue = get.Optional.Field "value" (Value.ISAJson.decoder)
                ComponentUnit = get.Optional.Field "unit" OntologyAnnotation.decoder
            }
        )
    
    module ISAJson =

        let genID (c : Component) = 
            match c.ComponentType,c.ComponentValue,c.ComponentUnit with
            | Some t, Some v, Some u -> $"#Component_{t.NameText}={v.Text}{u.NameText}"
            | Some t, Some v, None-> $"#Component_{t.NameText}={v.Text}"
            | _ -> $"#EmptyComponent"


        let encoder (idMap : IDTable.IDTableWrite option) (c : Component) = 
            [
                Encode.tryInclude "componentName" Encode.string c.ComponentName
                Encode.tryInclude "componentType" (OntologyAnnotation.ISAJson.encoder idMap) c.ComponentType
            ]
            |> Encode.choose
            |> Encode.object

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
