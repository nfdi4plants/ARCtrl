namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process

module Source =
    
    module ROCrate = 

        let genID (s:Source) : string = 
            match s.ID with
            | Some id -> URI.toString id
            | None -> match s.Name with
                      | Some n -> "#Source_" + n.Replace(" ","_")
                      | None -> "#EmptySource"
    
        let rec encoder (oa : Source) = 
            [
                "@id", Encode.string (oa |> genID) |> Some
                "@type", (Encode.list [ Encode.string "Source"]) |> Some
                "additionalType", Encode.string "Source" |> Some
                Encode.tryInclude "name" Encode.string (oa.Name)
                Encode.tryIncludeListOpt "characteristics" MaterialAttributeValue.ROCrate.encoder (oa.Characteristics)      
                "@context", ROCrateContext.Source.context_jsonvalue |> Some
                ]
            |> Encode.choose
            |> Encode.object

        let rec decoder : Decoder<Source> =     
            Decode.object (fun get ->
                match get.Optional.Field "additionalType" Decode.uri with
                | Some "Source" | None -> ()
                | Some _ -> get.Required.Field "FailBecauseNotSample" Decode.unit
                {
                    ID = get.Optional.Field "@id" Decode.uri
                    Name = get.Optional.Field "name" Decode.string
                    Characteristics = get.Optional.Field "characteristics" (Decode.list MaterialAttributeValue.ROCrate.decoder)
                } 
            
            )

    module ISAJson = 
    
        let rec encoder (idMap : IDTable.IDTableWrite option) (oa : Source) = 
            let f (oa : Source) =
                [
                    Encode.tryInclude "@id" Encode.string (oa |> ROCrate.genID |> Some)
                    Encode.tryInclude "name" Encode.string oa.Name
                    Encode.tryIncludeListOpt "characteristics" (MaterialAttributeValue.ISAJson.encoder idMap) oa.Characteristics
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f oa
            | Some idMap -> IDTable.encode ROCrate.genID f oa idMap

        let allowedFields = ["@id";"name";"characteristics";"@type"; "@context"]

        let decoder: Decoder<Source> =     
            Decode.objectNoAdditionalProperties allowedFields (fun get ->
                {
                    ID = get.Optional.Field "@id" Decode.uri
                    Name = get.Optional.Field "name" Decode.string
                    Characteristics = get.Optional.Field "characteristics" (Decode.list MaterialAttributeValue.ISAJson.decoder)
                } 
            )