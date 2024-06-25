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
                "@id", Encode.string (oa |> genID)
                "@type", (Encode.list [ Encode.string "Source"])
                Encode.tryInclude "name" Encode.string (oa.Name)
                Encode.tryIncludeListOpt "characteristics" MaterialAttributeValue.ROCrate.encoder (oa.Characteristics)      
                "@context", ROCrateContext.Source.context_jsonvalue
                ]
            |> Encode.choose
            |> Encode.object

        let rec decoder : Decoder<Source> =     
            Decode.object (fun get ->
           
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

[<AutoOpen>]
module SourceExtensions =
    
    type Source with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Source.ISAJson.decoder s   

        static member toISAJsonString(?spaces,?useIDReferencing) =
            let useIDReferencing = useIDReferencing |> Option.defaultValue false
            let idMap = if useIDReferencing then Some (System.Collections.Generic.Dictionary()) else None
            fun (f:Source) ->
                Source.ISAJson.encoder idMap f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces, ?useIDReferencing) =
            Source.toISAJsonString(?spaces=spaces, ?useIDReferencing = useIDReferencing) this

        static member fromROCrateString (s:string) = 
            Decode.fromJsonString Source.ROCrate.decoder s

        static member toROCrateString(?spaces) =
            fun (f:Source) ->
                Source.ROCrate.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateString(?spaces) =
            Source.toROCrateString(?spaces=spaces) this