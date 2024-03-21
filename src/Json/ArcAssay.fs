namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl

module JsonHelper =
  let DecodeOa : Decoder<OntologyAnnotation> = OntologyAnnotation.decoder (ConverterOptions()) 
  let DecodeTables = Decode.list ArcTable.decoder
  let DecodePersons : Decoder<Person list> = Decode.list (Person.decoder (ConverterOptions())) 
  let DecodeComments : Decoder<Comment list> = Decode.list (Comment.decoder (ConverterOptions())) 
  let DecodeFactors : Decoder<Factor list> = Decode.list (Factor.decoder (ConverterOptions())) 
  let DecodePublications: Decoder<Publication list> = Decode.list (Publication.decoder (ConverterOptions())) 
  let DecodeOntologySourceReferences: Decoder<OntologySourceReference list> = Decode.list (OntologySourceReference.decoder (ConverterOptions ()))
  let tryGetTables (get:Decode.IGetters) (fieldName:string) = get.Optional.Field(fieldName) DecodeTables |> Option.map ResizeArray |> Option.defaultValue (ResizeArray())
  let tryGetPersons (get:Decode.IGetters) (fieldName:string) = get.Optional.Field(fieldName) DecodePersons |> Option.map Array.ofList |> Option.defaultValue (Array.empty)
  let tryGetComments (get:Decode.IGetters) (fieldName:string) = get.Optional.Field(fieldName) DecodeComments |> Option.map Array.ofList |> Option.defaultValue (Array.empty)
  let tryGetPublications (get:Decode.IGetters) (fieldName:string) = get.Optional.Field(fieldName) DecodePublications |> Option.map Array.ofList |> Option.defaultValue (Array.empty)
  let tryGetOAs (get:Decode.IGetters) (fieldName:string) = get.Optional.Field(fieldName) (Decode.list DecodeOa) |> Option.map Array.ofList |> Option.defaultValue (Array.empty)
  let tryGetFactors (get:Decode.IGetters) (fieldName:string) = get.Optional.Field(fieldName) DecodeFactors |> Option.map Array.ofList |> Option.defaultValue (Array.empty)
  let tryGetStringResizeArray (get: Decode.IGetters) (fieldName:string) : ResizeArray<string> = get.Optional.Field(fieldName) (Decode.list Decode.string) |> Option.map ResizeArray |> Option.defaultValue (ResizeArray())
  let tryGetOntologySourceReferences (get: Decode.IGetters) (fieldName:string) : OntologySourceReference [] = get.Optional.Field(fieldName) (DecodeOntologySourceReferences) |> Option.map Array.ofList |> Option.defaultValue (Array.empty)
  let EncoderOA (t: OntologyAnnotation) = OntologyAnnotation.encoder (ConverterOptions()) t
  let EncoderOAs (t: seq<OntologyAnnotation>) = Encode.seq (Seq.map EncoderOA t)
  let EncoderTables (t: seq<ArcTable>) = Encode.seq (Seq.map ArcTable.encoder t)
  let EncoderPerson (t: Person) = Person.encoder (ConverterOptions()) t
  let EncoderPersons (t: seq<Person>) = Encode.seq (Seq.map EncoderPerson t)
  let EncoderComment (t:Comment) = Comment.encoder (ConverterOptions()) t
  let EncoderComments (t:seq<Comment>) = Encode.seq (Seq.map EncoderComment t)
  let EncoderPublication (t:Publication) = Publication.encoder (ConverterOptions()) t
  let EncoderPublications (t:seq<Publication>) = Encode.seq (Seq.map EncoderPublication t)
  let EncoderFactor (t: Factor) = Factor.encoder (ConverterOptions()) t
  let EncoderFactors (t: seq<Factor>) = Encode.seq (Seq.map EncoderFactor t)
  let EncoderOntologySourceReference (t: OntologySourceReference) = OntologySourceReference.encoder (ConverterOptions()) t
  let EncoderOntologySourceReferences (t: seq<OntologySourceReference>) = Encode.seq (Seq.map EncoderOntologySourceReference t)

open JsonHelper

module ArcAssay = 
    let encoder (assay:ArcAssay) = 
        Encode.object [ 
            "Identifier", Encode.string assay.Identifier
            if assay.MeasurementType.IsSome then
                "MeasurementType", EncoderOA assay.MeasurementType.Value
            if assay.TechnologyType.IsSome then
                "TechnologyType", EncoderOA assay.TechnologyType.Value
            if assay.TechnologyPlatform.IsSome then
                "TechnologyPlatform", EncoderOA assay.TechnologyPlatform.Value
            if assay.Tables.Count <> 0 then 
                "Tables", EncoderTables assay.Tables
            if assay.Performers.Length <> 0 then
                "Performers", EncoderPersons assay.Performers
            if assay.Comments.Length <> 0 then
                "Comments", EncoderComments assay.Comments
        ]
  
    let decoder : Decoder<ArcAssay> =
        Decode.object (fun get ->
            ArcAssay.make 
                (get.Required.Field("Identifier") Decode.string)
                (get.Optional.Field("MeasurementType") DecodeOa)
                (get.Optional.Field("TechnologyType") DecodeOa)
                (get.Optional.Field("TechnologyPlatform") DecodeOa)
                (tryGetTables get "Tables")
                (tryGetPersons get "Performers")
                (tryGetComments get "Comments")
        )

    let compressedEncoder (stringTable : StringTableMap) (oaTable : OATableMap) (cellTable : CellTableMap) (assay:ArcAssay) =
        Encode.object [ 
            "Identifier", Encode.string assay.Identifier
            if assay.MeasurementType.IsSome then
                "MeasurementType", OATable.encodeOA oaTable assay.MeasurementType.Value
            if assay.TechnologyType.IsSome then
                "TechnologyType", OATable.encodeOA oaTable assay.TechnologyType.Value
            if assay.TechnologyPlatform.IsSome then
                "TechnologyPlatform", OATable.encodeOA oaTable assay.TechnologyPlatform.Value
            if assay.Tables.Count <> 0 then 
                "Tables", Encode.seq (Seq.map (ArcTable.compressedEncoder stringTable oaTable cellTable) assay.Tables) 
            if assay.Performers.Length <> 0 then
                "Performers", EncoderPersons assay.Performers
            if assay.Comments.Length <> 0 then
                "Comments", EncoderComments assay.Comments
        ]


    let compressedDecoder (stringTable : StringTableArray) (oaTable : OATableArray) (cellTable : CellTableArray)  : Decoder<ArcAssay> =
        Decode.object (fun get ->
            let tables = 
                get.Optional.Field("Tables") (Decode.array (ArcTable.compressedDecoder stringTable oaTable cellTable))
                |> Option.map ResizeArray 
                |> Option.defaultValue (ResizeArray())
            ArcAssay.make 
                (get.Required.Field("Identifier") Decode.string)
                (get.Optional.Field("MeasurementType") (OATable.decodeOA oaTable))
                (get.Optional.Field("TechnologyType") (OATable.decodeOA oaTable))
                (get.Optional.Field("TechnologyPlatform") (OATable.decodeOA oaTable))
                tables
                (tryGetPersons get "Performers")
                (tryGetComments get "Comments")
        )


    /// exports in json-ld format
    let toJsonldString (a:ArcAssay) = 
        Assay.encoder (ConverterOptions(SetID=true,IsJsonLD=true)) None (a.ToAssay())
        |> Encode.toJsonString 2

    let toJsonldStringWithContext (a:ArcAssay) = 
        Assay.encoder (ConverterOptions(SetID=true,IsJsonLD=true)) None (a.ToAssay())
        |> Encode.toJsonString 2

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (Assay.decoder (ConverterOptions())) s
        |> ArcAssay.fromAssay

    let toJsonString (a:ArcAssay) = 
        Assay.encoder (ConverterOptions()) None (a.ToAssay())
        |> Encode.toJsonString 2

    let toArcJsonString (a:ArcAssay) : string =
        let spaces = 0
        Encode.toJsonString spaces (encoder a)

    let fromArcJsonString (jsonString: string) =
        try GDecode.fromJsonString decoder jsonString with
        | e -> failwithf "Error. Unable to parse json string to ArcAssay: %s" e.Message

[<AutoOpen>]
module ArcAssayExtensions =

    open System.Collections.Generic

    type ArcAssay with
        static member fromArcJsonString (jsonString: string) : ArcAssay = 
            try GDecode.fromJsonString ArcAssay.decoder jsonString with
            | e -> failwithf "Error. Unable to parse json string to ArcAssay: %s" e.Message

        member this.ToArcJsonString(?spaces) : string =
            let spaces = defaultArg spaces 0
            Encode.toJsonString spaces (ArcAssay.encoder this)

        static member toArcJsonString (a:ArcAssay) = a.ToArcJsonString()

        static member fromCompressedJsonString (jsonString: string) : ArcAssay = 
            let decoder = 
                Decode.object(fun get ->
                    let stringTable = get.Required.Field "stringTable" (StringTable.decoder)
                    let oaTable = get.Required.Field "oaTable" (OATable.decoder stringTable)
                    let cellTable = get.Required.Field "cellTable" (CellTable.decoder stringTable oaTable)
                    get.Required.Field "assay" (ArcAssay.compressedDecoder stringTable oaTable cellTable)
                )
            try GDecode.fromJsonString decoder jsonString with
            | e -> failwithf "Error. Unable to parse json string to ArcAssay: %s" e.Message

        member this.ToCompressedJsonString(?spaces) : string =
            let spaces = defaultArg spaces 0
            let stringTable = Dictionary()
            let oaTable = Dictionary()
            let cellTable = Dictionary()
            let arcAssay = ArcAssay.compressedEncoder stringTable oaTable cellTable this
            let jObject = 
                Encode.object [
                    "cellTable", CellTable.arrayFromMap cellTable |> CellTable.encoder stringTable oaTable
                    "oaTable", OATable.arrayFromMap oaTable |> OATable.encoder stringTable
                    "stringTable", StringTable.arrayFromMap stringTable |> StringTable.encoder
                    "assay", arcAssay
                ] 
            Encode.toJsonString spaces jObject

        static member toCompressedJsonString (a:ArcAssay) = a.ToCompressedJsonString()