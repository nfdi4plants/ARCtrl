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
                "@id", Encode.string (oa |> genID) |> Some
                "@type", (Encode.list [Encode.string "Material"]) |> Some
                "additionalType", Encode.string "Material" |> Some
                Encode.tryInclude "name" Encode.string oa.Name
                Encode.tryInclude "type" MaterialType.ROCrate.encoder oa.MaterialType
                Encode.tryIncludeListOpt "characteristics" MaterialAttributeValue.ROCrate.encoder oa.Characteristics
                Encode.tryIncludeListOpt "derivesFrom" encoder oa.DerivesFrom
                "@context", ROCrateContext.Material.context_jsonvalue |> Some
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<Material> =       
            let rec decode() =
                Decode.object (fun get ->
                    match get.Optional.Field "additionalType" Decode.uri with
                    | Some "Material" | None -> ()
                    | Some _ -> get.Required.Field "FailBecauseNotSample" Decode.unit
                    {                       
                        ID = get.Optional.Field "@id" Decode.uri
                        Name = get.Optional.Field "name" Decode.string
                        MaterialType = get.Optional.Field "type" MaterialType.ROCrate.decoder
                        Characteristics = get.Optional.Field "characteristics" (Decode.list MaterialAttributeValue.ROCrate.decoder)
                        DerivesFrom = get.Optional.Field "derivesFrom" (Decode.list <| decode())
                    }
                )
            decode()

    module ISAJson =
    
        let rec encoder (idMap : IDTable.IDTableWrite option) (c : Material) = 
            let f (oa : Material) =
                [
                    Encode.tryInclude "@id" Encode.string (oa |> ROCrate.genID |> Some)
                    Encode.tryInclude "name" Encode.string oa.Name
                    Encode.tryInclude "type" MaterialType.ISAJson.encoder oa.MaterialType
                    Encode.tryIncludeListOpt "characteristics" (MaterialAttributeValue.ISAJson.encoder idMap) oa.Characteristics
                    Encode.tryIncludeListOpt "derivesFrom" (encoder idMap) oa.DerivesFrom
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f c
            | Some idMap -> IDTable.encode ROCrate.genID f c idMap


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
  