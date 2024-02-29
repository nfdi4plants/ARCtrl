namespace ARCtrl.ISA.Json

open Thoth.Json.Core

open ARCtrl.ISA
open System.IO

module AnnotationValue =

    /// <summary>
    /// This decodes from integer, float or string to string only!
    /// </summary>
    /// <param name="options"></param>
    /// <param name="s"></param>
    /// <param name="json"></param>
    let decoder (options : ConverterOptions) : Decoder<string> =
        { new Decoder<string> with
            member this.Decode (s,json) =
                match Decode.int.Decode(s,json) with
                | Ok i -> Ok <| string i
                | Error _ -> 
                    match Decode.float.Decode(s,json) with
                    | Ok f -> Ok <| string f
                    | Error _ -> 
                        match Decode.string.Decode(s,json) with
                        | Ok s -> Ok <| s
                        | Error e -> Error e       
        }

            // is there a option to decode force to string?
            

module OntologySourceReference = 
    
    let genID (o:OntologySourceReference) = 
        match o.File with
        | Some f -> f
        | None -> 
            match o.Name with
            | Some n -> "#OntologySourceRef_" + n.Replace(" ","_")
            | None -> "#DummyOntologySourceRef"

    let encoder (options : ConverterOptions) (osr : obj) = 
        [
            if options.SetID then "@id", GEncode.includeString (osr :?> OntologySourceReference |> genID)
            if options.IncludeType then "@type", GEncode.includeString "OntologySourceReference"
            GEncode.tryInclude "description" GEncode.includeString (osr |> GEncode.tryGetPropertyValue "Description")
            GEncode.tryInclude "file" GEncode.includeString (osr |> GEncode.tryGetPropertyValue "File")
            GEncode.tryInclude "name" GEncode.includeString (osr |> GEncode.tryGetPropertyValue "Name")
            GEncode.tryInclude "version" GEncode.includeString (osr |> GEncode.tryGetPropertyValue "Version")
            GEncode.tryInclude "comments" (Comment.encoder options) (osr |> GEncode.tryGetPropertyValue "Comments")
        ]
        |> GEncode.choose
        |> Encode.object

    let decoder (options : ConverterOptions) : Decoder<OntologySourceReference> =
        Decode.object (fun get ->
            {
                Description = get.Optional.Field "description" GDecode.uri
                File = get.Optional.Field "file" Decode.string
                Name = get.Optional.Field "name" Decode.string
                Version = get.Optional.Field "version" Decode.string
                Comments = get.Optional.Field "comments" (Decode.array (Comment.decoder options))               
            }
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s        

    let toJsonString (oa:OntologySourceReference) = 
        encoder (ConverterOptions()) oa
        |> GEncode.toJsonString 2

    /// exports in json-ld format
    let toStringLD (oa:OntologySourceReference) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) oa
        |> GEncode.toJsonString 2

    // let fromFile (path : string) = 
    //     File.ReadAllText path 
    //     |> fromString

    //let toFile (path : string) (osr:OntologySourceReference) = 
    //    File.WriteAllText(path,toString osr)

module OntologyAnnotation =  
    
    let genID (o:OntologyAnnotation) : string = 
        match o.ID with
        | Some id -> URI.toString id 
        | None -> match o.TermAccessionNumber with
                  | Some ta -> ta
                  | None -> match o.TermSourceREF with
                            | Some r -> "#" + r.Replace(" ","_")
                            | None -> "#DummyOntologyAnnotation"

    let encoder (options : ConverterOptions) (oa : obj) = 
        [
            if options.SetID then "@id", GEncode.includeString (oa :?> OntologyAnnotation |> genID)
                else GEncode.tryInclude "@id" GEncode.includeString (oa |> GEncode.tryGetPropertyValue "ID")
            if options.IncludeType then "@type", GEncode.includeString "OntologyAnnotation"
            GEncode.tryInclude "annotationValue" GEncode.includeString (oa |> GEncode.tryGetPropertyValue "Name")
            GEncode.tryInclude "termSource" GEncode.includeString (oa |> GEncode.tryGetPropertyValue "TermSourceREF")
            GEncode.tryInclude "termAccession" GEncode.includeString (oa |> GEncode.tryGetPropertyValue "TermAccessionNumber")
            GEncode.tryInclude "comments" (Comment.encoder options) (oa |> GEncode.tryGetPropertyValue "Comments")
        ]
        |> GEncode.choose
        |> Encode.object


    let decoder (options : ConverterOptions) : Decoder<OntologyAnnotation> =
        Decode.object (fun get ->
            OntologyAnnotation.create(
                ?Id = get.Optional.Field "@id" GDecode.uri,
                ?Name = get.Optional.Field "annotationValue" (AnnotationValue.decoder options),
                ?TermSourceREF = get.Optional.Field "termSource" Decode.string,
                ?TermAccessionNumber = get.Optional.Field "termAccession" Decode.string,
                ?Comments = get.Optional.Field "comments" (Decode.array (Comment.decoder options))               
            )
        )


    let compressedEncoder (stringTable : StringTableMap) (options : ConverterOptions) (oa : obj) = 
        [
            if options.SetID then "@id", GEncode.includeString (oa :?> OntologyAnnotation |> genID)
                else GEncode.tryInclude "@id" GEncode.includeString (oa |> GEncode.tryGetPropertyValue "ID")
            if options.IncludeType then "@type", GEncode.includeString "OntologyAnnotation"
            GEncode.tryInclude "a" (StringTable.encodeString stringTable) (oa |> GEncode.tryGetPropertyValue "Name")
            GEncode.tryInclude "ts" (StringTable.encodeString stringTable) (oa |> GEncode.tryGetPropertyValue "TermSourceREF")
            GEncode.tryInclude "ta" (StringTable.encodeString stringTable) (oa |> GEncode.tryGetPropertyValue "TermAccessionNumber")
            GEncode.tryInclude "comments" (Comment.encoder options) (oa |> GEncode.tryGetPropertyValue "Comments")
        ]
        |> GEncode.choose
        |> Encode.object


    let compressedDecoder (stringTable : StringTableArray) (options : ConverterOptions) : Decoder<OntologyAnnotation> =
        Decode.object (fun get ->
            OntologyAnnotation.create(
                ?Id = get.Optional.Field "@id" GDecode.uri,
                ?Name = get.Optional.Field "a" (StringTable.decodeString stringTable),
                ?TermSourceREF = get.Optional.Field "ts" (StringTable.decodeString stringTable),
                ?TermAccessionNumber = get.Optional.Field "ta" (StringTable.decodeString stringTable),
                ?Comments = get.Optional.Field "comments" (Decode.array (Comment.decoder options))               
            )
        )

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (decoder (ConverterOptions())) s        

    let toJsonString (oa:OntologyAnnotation) = 
        encoder (ConverterOptions()) oa
        |> GEncode.toJsonString 2
    
    /// exports in json-ld format
    let toStringLD (oa:OntologyAnnotation) = 
        encoder (ConverterOptions(SetID=true,IncludeType=true)) oa
        |> GEncode.toJsonString 2

    //let fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> fromString

    //let toFile (path : string) (oa:OntologyAnnotation) = 
    //    File.WriteAllText(path,toString oa)
