namespace ARCtrl.Json

open Thoth.Json.Core
open ARCtrl
open StringTable

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

        let encoderDefinedTerm (oa : OntologyAnnotation) = 
            [
                "@id", Encode.string (oa |> genID) |> Some
                "@type", Encode.string "OntologyAnnotation" |> Some
                Encode.tryInclude "annotationValue" Encode.string (oa.Name)
                Encode.tryInclude "termSource" Encode.string (oa.TermSourceREF)
                Encode.tryInclude "termAccession" Encode.string (oa.TermAccessionNumber)
                Encode.tryIncludeSeq "comments" Comment.ROCrate.encoderDisambiguatingDescription (oa.Comments)
                "@context", ROCrateContext.OntologyAnnotation.context_jsonvalue |> Some
            ]
            |> Encode.choose
            |> Encode.object

        let decoderDefinedTerm : Decoder<OntologyAnnotation> =
            Decode.object (fun get ->
                OntologyAnnotation.create(
                    ?name = get.Optional.Field "annotationValue" AnnotationValue.decoder,
                    ?tsr = get.Optional.Field "termSource" Decode.string,
                    ?tan = get.Optional.Field "termAccession" Decode.string,
                    ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.ROCrate.decoderDisambiguatingDescription )               
                )
            )

        let encoderPropertyValue (oa : OntologyAnnotation) = 
            [
                "@id", Encode.string (oa |> genID) |> Some
                "@type", Encode.string "PropertyValue" |> Some

                Encode.tryInclude "category" Encode.string oa.Name
                Encode.tryInclude "categoryCode" Encode.string oa.TermAccessionNumber
                Encode.tryIncludeSeq "comments" Comment.ROCrate.encoderDisambiguatingDescription (oa.Comments)
                "@context", ROCrateContext.PropertyValue.context_jsonvalue |> Some
            ]
            |> Encode.choose
            |> Encode.object

        let decoderPropertyValue : Decoder<OntologyAnnotation> =
            Decode.object (fun get ->
                OntologyAnnotation.create(
                    ?name = get.Optional.Field "category" Decode.string,
                    ?tan = get.Optional.Field "categoryCode" Decode.string,
                    ?comments = get.Optional.Field "comments" (Decode.resizeArray Comment.ROCrate.decoderDisambiguatingDescription)               
                )

            )

    module ISAJson =
        
        let encoder (idMap : IDTable.IDTableWrite option) (oa : OntologyAnnotation) = 
            let f = fun (oa : OntologyAnnotation) ->
                let comments = 
                    oa.Comments
                    |> Seq.filter (fun c -> 
                        match c.Name with
                        | Some n when n = Process.ColumnIndex.orderName -> false
                        | _ -> true)
                [
                    Encode.tryInclude "@id" Encode.string (ROCrate.genID oa |> Some)
                    Encode.tryInclude "annotationValue" Encode.string (oa.Name)
                    Encode.tryInclude "termSource" Encode.string (oa.TermSourceREF)
                    Encode.tryInclude "termAccession" Encode.string (oa.TermAccessionNumber)
                    Encode.tryIncludeSeq "comments" Comment.encoder comments
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f oa
            | Some idMap -> IDTable.encode ROCrate.genID f oa idMap

        let decoder = decoder

[<AutoOpen>]
module OntologyAnnotationExtensions =

    type OntologyAnnotation with
       
        static member fromJsonString (s:string)  = 
            Decode.fromJsonString OntologyAnnotation.decoder s

        static member toJsonString(?spaces) = 
            fun (obj:OntologyAnnotation) ->
                OntologyAnnotation.encoder obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)                  

        member this.ToJsonString(?spaces) =
            OntologyAnnotation.toJsonString(?spaces=spaces) this

        static member fromROCrateJsonString (s:string) = 
            Decode.fromJsonString OntologyAnnotation.ROCrate.decoderDefinedTerm s

        /// exports in json-ld format
        static member toROCrateJsonString(?spaces) =
            fun (obj:OntologyAnnotation) ->
                OntologyAnnotation.ROCrate.encoderDefinedTerm obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToROCrateJsonString(?spaces) =
            OntologyAnnotation.toROCrateJsonString(?spaces=spaces) this

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString OntologyAnnotation.ISAJson.decoder s

        static member toISAJsonString(?spaces) =
            fun (obj:OntologyAnnotation) ->
                OntologyAnnotation.ISAJson.encoder None obj
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        member this.ToISAJsonString(?spaces) =
            OntologyAnnotation.toISAJsonString(?spaces=spaces) this