namespace ARCtrl.ISA.Json

open Thoth.Json.Core

open ARCtrl.ISA

open JsonHelper

module ArcStudy = 
    let encoder (study:ArcStudy) = 
        Encode.object [ 
            "Identifier", Encode.string study.Identifier
            if study.Title.IsSome then
                "Title", Encode.string study.Title.Value
            if study.Description.IsSome then
                "Description", Encode.string study.Description.Value
            if study.SubmissionDate.IsSome then
                "SubmissionDate", Encode.string study.SubmissionDate.Value
            if study.PublicReleaseDate.IsSome then
                "PublicReleaseDate", Encode.string study.PublicReleaseDate.Value
            if study.Publications.Length <> 0 then
                "Publications", EncoderPublications study.Publications
            if study.Contacts.Length <> 0 then
                "Contacts", EncoderPersons study.Contacts
            if study.StudyDesignDescriptors.Length <> 0 then
                "StudyDesignDescriptors", EncoderOAs study.StudyDesignDescriptors
            if study.TableCount <> 0 then
                "Tables", EncoderTables study.Tables
            if study.RegisteredAssayIdentifiers.Count <> 0 then
                "RegisteredAssayIdentifiers", Encode.seq (Seq.map Encode.string study.RegisteredAssayIdentifiers)
            if study.Factors.Length <> 0 then
                "Factors", EncoderFactors study.Factors
            if study.Comments.Length <> 0 then
                "Comments", EncoderComments study.Comments
        ]
  
    let decoder : Decoder<ArcStudy> =
        Decode.object (fun get ->
            ArcStudy.make 
                (get.Required.Field("Identifier") Decode.string)
                (get.Optional.Field("Title") Decode.string)
                (get.Optional.Field("Description") Decode.string)
                (get.Optional.Field("SubmissionDate") Decode.string)
                (get.Optional.Field("PublicReleaseDate") Decode.string)
                (tryGetPublications get "Publications")
                (tryGetPersons get "Contacts")
                (tryGetOAs get "StudyDesignDescriptors")
                (tryGetTables get "Tables")
                (tryGetStringResizeArray get "RegisteredAssayIdentifiers")
                (tryGetFactors get "Factors")
                (tryGetComments get "Comments")
    )

    let compressedEncoder (stringTable : StringTableMap) (oaTable : OATableMap) (cellTable : CellTableMap) (study:ArcStudy) =
        Encode.object [ 
            "Identifier", Encode.string study.Identifier
            if study.Title.IsSome then
                "Title", Encode.string study.Title.Value
            if study.Description.IsSome then
                "Description", Encode.string study.Description.Value
            if study.SubmissionDate.IsSome then
                "SubmissionDate", Encode.string study.SubmissionDate.Value
            if study.PublicReleaseDate.IsSome then
                "PublicReleaseDate", Encode.string study.PublicReleaseDate.Value
            if study.Publications.Length <> 0 then
                "Publications", EncoderPublications study.Publications
            if study.Contacts.Length <> 0 then
                "Contacts", EncoderPersons study.Contacts
            if study.StudyDesignDescriptors.Length <> 0 then
                "StudyDesignDescriptors", EncoderOAs study.StudyDesignDescriptors
            if study.TableCount <> 0 then
                "Tables", Encode.seq (Seq.map (ArcTable.compressedEncoder stringTable oaTable cellTable) study.Tables) 
            if study.RegisteredAssayIdentifiers.Count <> 0 then
                "RegisteredAssayIdentifiers", Encode.seq (Seq.map Encode.string study.RegisteredAssayIdentifiers)
            if study.Factors.Length <> 0 then
                "Factors", EncoderFactors study.Factors
            if study.Comments.Length <> 0 then
                "Comments", EncoderComments study.Comments
        ]

    let compressedDecoder (stringTable : StringTableArray) (oaTable : OATableArray) (cellTable : CellTableArray) : Decoder<ArcStudy> =
        Decode.object (fun get ->
            let tables = 
                get.Optional.Field("Tables") (Decode.array (ArcTable.compressedDecoder stringTable oaTable cellTable))
                |> Option.map ResizeArray 
                |> Option.defaultValue (ResizeArray())
            ArcStudy.make 
                (get.Required.Field("Identifier") Decode.string)
                (get.Optional.Field("Title") Decode.string)
                (get.Optional.Field("Description") Decode.string)
                (get.Optional.Field("SubmissionDate") Decode.string)
                (get.Optional.Field("PublicReleaseDate") Decode.string)
                (tryGetPublications get "Publications")
                (tryGetPersons get "Contacts")
                (tryGetOAs get "StudyDesignDescriptors")
                tables
                (tryGetStringResizeArray get "RegisteredAssayIdentifiers")
                (tryGetFactors get "Factors")
                (tryGetComments get "Comments")
        )


    /// exports in json-ld format
    let toStringLD (a:ArcStudy) (assays: ResizeArray<ArcAssay>) = 
        Study.encoder (ConverterOptions(SetID=true,IncludeType=true)) (a.ToStudy(assays))
        |> GEncode.toJsonString 2

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (Study.decoder (ConverterOptions())) s
        |> ArcStudy.fromStudy

    let toJsonString (a:ArcStudy) (assays: ResizeArray<ArcAssay>) = 
        Study.encoder (ConverterOptions()) (a.ToStudy(assays))
        |> GEncode.toJsonString 2

    let toArcJsonString (a:ArcStudy) : string =
        let spaces = 0
        GEncode.toJsonString spaces (encoder a)

    let fromArcJsonString (jsonString: string) =
        try GDecode.fromJsonString decoder jsonString with
        | e -> failwithf "Error. Unable to parse json string to ArcStudy: %s" e.Message

[<AutoOpen>]
module ArcStudyExtensions =
    
    open System.Collections.Generic

    type ArcStudy with
        static member fromArcJsonString (jsonString: string) : ArcStudy = 
            try GDecode.fromJsonString ArcStudy.decoder jsonString with
            | e -> failwithf "Error. Unable to parse json string to ArcStudy: %s" e.Message

        member this.ToArcJsonString(?spaces) : string =
            let spaces = defaultArg spaces 0
            GEncode.toJsonString spaces (ArcStudy.encoder this)

        static member toArcJsonString(a:ArcStudy) = a.ToArcJsonString()

        static member fromCompressedJsonString (jsonString: string) : ArcStudy = 
            let decoder = 
                Decode.object(fun get ->
                    let stringTable = get.Required.Field "stringTable" (StringTable.decoder)
                    let oaTable = get.Required.Field "oaTable" (OATable.decoder stringTable)
                    let cellTable = get.Required.Field "cellTable" (CellTable.decoder stringTable oaTable)
                    get.Required.Field "study" (ArcStudy.compressedDecoder stringTable oaTable cellTable)
                )
            try GDecode.fromJsonString decoder jsonString with
            | e -> failwithf "Error. Unable to parse json string to ArcAssay: %s" e.Message

        member this.ToCompressedJsonString(?spaces) : string =
            let spaces = defaultArg spaces 0
            let stringTable = Dictionary()
            let oaTable = Dictionary()
            let cellTable = Dictionary()
            let arcStudy = ArcStudy.compressedEncoder stringTable oaTable cellTable this
            let jObject = 
                Encode.object [
                    "cellTable", CellTable.arrayFromMap cellTable |> CellTable.encoder stringTable oaTable
                    "oaTable", OATable.arrayFromMap oaTable |> OATable.encoder stringTable
                    "stringTable", StringTable.arrayFromMap stringTable |> StringTable.encoder
                    "study", arcStudy
                ] 
            GEncode.toJsonString spaces jObject

        static member toCompressedJsonString (s : ArcStudy) = 
            s.ToCompressedJsonString()