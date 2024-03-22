namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl

module ArcStudy = 

    /// exports in json-ld format
    let toJsonldString (a:ArcStudy) (assays: ResizeArray<ArcAssay>) = 
        Study.encoder (ConverterOptions(SetID=true,IsJsonLD=true)) (a.ToStudy(assays))
        |> Encode.toJsonString 2

    let toJsonldStringWithContext (a:ArcStudy) (assays: ResizeArray<ArcAssay>) = 
        Study.encoder (ConverterOptions(SetID=true,IsJsonLD=true)) (a.ToStudy(assays))
        |> Encode.toJsonString 2

    let fromJsonString (s:string) = 
        GDecode.fromJsonString (Study.decoder (ConverterOptions())) s
        |> ArcStudy.fromStudy

    let toJsonString (a:ArcStudy) (assays: ResizeArray<ArcAssay>) = 
        Study.encoder (ConverterOptions()) (a.ToStudy(assays))
        |> Encode.toJsonString 2

    let toArcJsonString (a:ArcStudy) : string =
        let spaces = 0
        Encode.toJsonString spaces (encoder a)

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
            Encode.toJsonString spaces (ArcStudy.encoder this)

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
            Encode.toJsonString spaces jObject

        static member toCompressedJsonString (s : ArcStudy) = 
            s.ToCompressedJsonString()