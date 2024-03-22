namespace ARCtrl.Json


open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module Component =

    module ISAJson =

        let encoder (c : Component) = 
            [
                "componentName", Encode.string c.ComponentName // TODO: tryInclude
                Encode.tryInclude "componentType" OntologyAnnotation.ISAJson.encoder c.ComponentType
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
    
//    let genID (c:Component) = 
//        let name = Component.composeName c.ComponentValue c.ComponentUnit
//        "#Component_" + name.Replace(" ","_")
//        // match c.ComponentName with
//        // | Some cn -> "#Component_" + cn.Replace(" ","_")
//        // | None -> "#EmptyComponent"

//    let encoder (options : ConverterOptions) (oa : Component) = 
//        if options.IsJsonLD then
//            [
//                if options.SetID then 
//                    "@id", Encode.string (oa |> genID)
//                "@type", (Encode.list [Encode.string "Component"])
//                if oa.ComponentType.IsSome then
//                    Encode.tryInclude "category" Encode.string (oa.ComponentType.Value.Name)
//                    Encode.tryInclude "categoryCode" Encode.string (oa.ComponentType.Value.TermAccessionNumber)
//                if oa.ComponentValue.IsSome then "value", Encode.string (oa.ValueText)
//                if oa.ComponentValue.IsSome && oa.ComponentValue.Value.IsAnOntology then
//                    Encode.tryInclude "valueCode" Encode.string (oa.ComponentValue.Value.AsOntology()).TermAccessionNumber
//                if oa.ComponentUnit.IsSome then Encode.tryInclude "unit" Encode.string (oa.ComponentUnit.Value.Name)
//                if oa.ComponentUnit.IsSome then Encode.tryInclude "unitCode" Encode.string (oa.ComponentUnit.Value.TermAccessionNumber)
//                "@context", ROCrateContext.Component.context_jsonvalue
//            ]
//        else
//            [
//                if options.SetID then 
//                    "@id", Encode.string (oa |> genID)
//                "componentName", Encode.string (Component.composeName oa.ComponentValue oa.ComponentUnit)
//                Encode.tryInclude "componentType" (OntologyAnnotation.encoder options) (oa.ComponentType)
//            ]
//        |> Encode.choose
//        |> Encode.object

//    let decoder (options : ConverterOptions) : Decoder<Component> =
//        if not options.IsJsonLD then
//            Decode.object (fun get ->
//                let name = get.Optional.Field "componentName" GDecode.uri
//                let value, unit =
//                    match name with
//                    | Some n -> 
//                        let v,u = Component.decomposeName n
//                        Some v, u
//                    | None -> None, None
//                {
//                    ComponentName = None
//                    ComponentValue = value
//                    ComponentUnit = unit
//                    ComponentType = get.Optional.Field "componentType" (OntologyAnnotation.decoder options)
//                }
//            )
//        else 
//            Decode.object (fun get ->
//                let categoryName = get.Optional.Field "category" (Decode.string)
//                let categoryCode = get.Optional.Field "categoryCode" (Decode.string)
//                let category =
//                    match categoryName,categoryCode with
//                    | None,None -> None
//                    | _ -> Some (OntologyAnnotation.make None categoryName None categoryCode None)
//                let valueName = get.Optional.Field "value" (Value.decoder options)
//                let valueCode = get.Optional.Field "valueCode" (Decode.string)
//                let value =
//                    match valueName,valueCode with
//                    | Some (Value.Name name), Some code ->
//                        let oa = OntologyAnnotation.make None (Some name) None (Some (URI.fromString code)) None
//                        let vo = Value.Ontology(oa)
//                        Some vo
//                    | None, Some code ->
//                        let oa = OntologyAnnotation.make None None None (Some (URI.fromString code)) None
//                        let vo = Value.Ontology(oa)
//                        Some vo
//                    | Some (Value.Name name), None -> valueName
//                    | Some (Value.Float name), None -> valueName
//                    | Some (Value.Int name), None -> valueName
//                    | _ -> None
//                let unitName = get.Optional.Field "unit" (Decode.string)
//                let unitCode = get.Optional.Field "unitCode" (Decode.string)
//                let unit = 
//                    match unitName,unitCode with
//                    | None,None -> None
//                    | _ -> Some (OntologyAnnotation.make None unitName None unitCode None)
//                {
//                    ComponentName = None
//                    ComponentValue = value
//                    ComponentUnit = unit
//                    ComponentType = category
//                }
//            )

[<AutoOpen>]
module ComponentExtensions =
    
    type Component with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Component.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:Component) ->
                Component.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)
