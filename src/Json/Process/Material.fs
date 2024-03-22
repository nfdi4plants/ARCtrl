namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process
open System.IO

module Material = 
    
    module ROCrate = 
        let genID (m:Material) : string = 
            match m.ID with
            | Some id -> id
            | None -> match m.Name with
                        | Some n -> "#Material_" + n.Replace(" ","_")
                        | None -> "#EmptyMaterial"
    
        let rec encoder (oa : Material) = 
            [
                "@id", Encode.string (oa |> genID)
                "@type", (Encode.list [Encode.string "Material"])
                Encode.tryInclude "name" Encode.string oa.Name
                Encode.tryInclude "type" MaterialType.ROCrate.encoder oa.MaterialType
                Encode.tryIncludeList "characteristics" MaterialAttributeValue.ROCrate.encoder oa.Characteristics
                Encode.tryIncludeList "derivesFrom" encoder oa.DerivesFrom
                "@context", ROCrateContext.Material.context_jsonvalue
            ]
            |> Encode.choose
            |> Encode.object

        let rec decoder : Decoder<Material> =       
            Decode.object (fun get -> 
                {                       
                    ID = get.Optional.Field "@id" Decode.uri
                    Name = get.Optional.Field "name" Decode.string
                    MaterialType = get.Optional.Field "type" MaterialType.ROCrate.decoder
                    Characteristics = get.Optional.Field "characteristics" (Decode.list MaterialAttributeValue.ROCrate.decoder)
                    DerivesFrom = get.Optional.Field "derivesFrom" (Decode.list decoder)
                }
            )

    module ISAJson =
    
        let rec encoder (oa : Material) = 
            [
                Encode.tryInclude "@id" Encode.string oa.ID
                Encode.tryInclude "name" Encode.string oa.Name
                Encode.tryInclude "type" MaterialType.ISAJson.encoder oa.MaterialType
                Encode.tryIncludeListOpt "characteristics" MaterialAttributeValue.ISAJson.encoder oa.Characteristics
                Encode.tryIncludeListOpt "derivesFrom" encoder oa.DerivesFrom
            ]
            |> Encode.choose
            |> Encode.object

        let allowedFields = ["@id";"@type";"name";"type";"characteristics";"derivesFrom"; "@context"]

        let decoder: Decoder<Material> = 
            let rec decode() = // rec objects trigger warning, therefore we add a inner rec function
                Decode.objectNoAdditionalProperties allowedFields (fun get -> 
                    {                       
                        ID = get.Optional.Field "@id" Decode.uri
                        Name = get.Optional.Field "name" Decode.string
                        MaterialType = get.Optional.Field "type" MaterialType.ISAJson.decoder
                        Characteristics = get.Optional.Field "characteristics" (Decode.list MaterialAttributeValue.ISAJson.decoder)
                        DerivesFrom = get.Optional.Field "derivesFrom" (Decode.list <| decode())
                    }
                )
            decode()
        
[<AutoOpen>]
module MaterialExtensions =
    
    type Material with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Material.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:Material) ->
                Material.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        static member fromROCrateJsonString (s:string) =
            Decode.fromJsonString Material.ROCrate.decoder s

        static member toROCrateJsonString(?spaces) =
            fun (f:Material) ->
                Material.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)