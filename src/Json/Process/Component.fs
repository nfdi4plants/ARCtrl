namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module Component =

    module ROCrate =

        let encoder : Component -> IEncodable = 
            PropertyValue.ROCrate.encoder

        let decoder : Decoder<Component> =
            PropertyValue.ROCrate.decoder<Component> (Component.createAsPV)
    
    module ISAJson =

        let genID (c : Component) = 
            PropertyValue.ROCrate.genID c

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
