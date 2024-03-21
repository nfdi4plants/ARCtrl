namespace ARCtrl.Json

open Thoth.Json.Core
open ARCtrl
open System.IO

module AnnotationValue =

    /// <summary>
    /// This decodes from integer, float or string to string only!
    /// </summary>
    /// <param name="options"></param>
    /// <param name="s"></param>
    /// <param name="json"></param>
    let decoder : Decoder<string> =
        Decode.oneOf [
            Decode.map string Decode.int
            Decode.map string Decode.float
            Decode.string
        ]
           

module OntologyAnnotation =  

    let encoder (oa : OntologyAnnotation) = 
        [
            Encode.tryInclude "annotationValue" Encode.string (oa.Name)
            Encode.tryInclude "termSource" Encode.string (oa.TermSourceREF)
            Encode.tryInclude "termAccession" Encode.string (oa.TermAccessionNumber)
            Encode.tryIncludeSeq "comments" Comment.encoder (oa.Comments)
        ]
        |> Encode.choose
        |> Encode.object


    let decoder : Decoder<OntologyAnnotation> =
        Decode.object (fun get ->
            OntologyAnnotation.create(
                ?name = get.Optional.Field "annotationValue" (AnnotationValue.decoder),
                ?tsr = get.Optional.Field "termSource" Decode.string,
                ?tan = get.Optional.Field "termAccession" Decode.string,
                ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.decoder)               
            )
        )

    let compressedEncoder (stringTable : StringTableMap) (oa : OntologyAnnotation) = 
        [
            Encode.tryInclude "a" (StringTable.encodeString stringTable) (oa.Name)
            Encode.tryInclude "ts" (StringTable.encodeString stringTable) (oa.TermSourceREF)
            Encode.tryInclude "ta" (StringTable.encodeString stringTable) (oa.TermAccessionNumber)
            Encode.tryIncludeSeq "comments" Comment.encoder (oa.Comments)
        ]
        |> Encode.choose
        |> Encode.object


    let compressedDecoder (stringTable : StringTableArray) : Decoder<OntologyAnnotation> =
        Decode.object (fun get ->
            OntologyAnnotation(
                ?name = get.Optional.Field "a" (StringTable.decodeString stringTable),
                ?tsr = get.Optional.Field "ts" (StringTable.decodeString stringTable),
                ?tan = get.Optional.Field "ta" (StringTable.decodeString stringTable),
                ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.decoder)               
            )
        )

    module ROCrate =

        let genID (o:OntologyAnnotation) : string = 
            match o.TermAccessionNumber with
            | Some ta -> URI.toString ta
            | None -> 
                match o.TermSourceREF with
                | Some r -> "#" + r.Replace(" ","_")
                | None -> 
                    match o.Name with
                    | Some n -> "#UserTerm_" + n .Replace(" ","_")
                    | None -> "#DummyOntologyAnnotation"

        let encoder (oa : OntologyAnnotation) = 
            [
                "@id", Encode.string (oa |> genID)
                "@type", Encode.string "OntologyAnnotation"
                Encode.tryInclude "annotationValue" Encode.string (oa.Name)
                Encode.tryInclude "termSource" Encode.string (oa.TermSourceREF)
                Encode.tryInclude "termAccession" Encode.string (oa.TermAccessionNumber)
                Encode.tryIncludeSeq "comments" Comment.ROCrate.encoderDisambiguatingDescription (oa.Comments)
                "@context", ROCrateContext.OntologyAnnotation.context_jsonvalue
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<OntologyAnnotation> =
            Decode.object (fun get ->
                OntologyAnnotation.create(
                    ?name = get.Optional.Field "annotationValue" AnnotationValue.decoder,
                    ?tsr = get.Optional.Field "termSource" Decode.string,
                    ?tan = get.Optional.Field "termAccession" Decode.string,
                    ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.decoder)               
                )
            )

    module ISAJson =
        
        let encoder = encoder
        let decoder = decoder

[<AutoOpen>]
module OntologyAnnotationExtensions =

    type OntologyAnnotation with
       
        static member fromJsonString (s:string)  = 
            Decode.fromJsonString OntologyAnnotation.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:OntologyAnnotation) ->
                OntologyAnnotation.encoder obj
                |> Encode.toJsonString (defaultArg spaces 2)                  

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString OntologyAnnotation.ROCrate.decoder s

        /// exports in json-ld format
        static member toROCrateJsonString(?spaces) =
            fun (obj:OntologyAnnotation) ->
                OntologyAnnotation.ROCrate.encoder obj
                |> Encode.toJsonString (defaultArg spaces 2)

        static member toISAJsonString(?spaces) =
            fun (obj:OntologyAnnotation) ->
                OntologyAnnotation.ISAJson.encoder obj
                |> Encode.toJsonString (defaultArg spaces 2)

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString OntologyAnnotation.ISAJson.decoder s