namespace rec ARCtrl.Json

open Thoth.Json.Core

open ARCtrl

open ARCtrl.Aux

module CompositeCell =

    let [<Literal>] CellType = "celltype"
    let [<Literal>] CellValues = "values"

    let [<Literal>] CompressedCellType = "t"
    let [<Literal>] CompressedCellValues = "v"

    let encoder (cc: CompositeCell) =
        let oaToJsonString (oa:OntologyAnnotation) = OntologyAnnotation.encoder (ConverterOptions()) oa
        let t, v =
            match cc with
            | CompositeCell.FreeText s-> "FreeText", [Encode.string s]
            | CompositeCell.Term t -> "Term", [oaToJsonString t]
            | CompositeCell.Unitized (v, unit) -> "Unitized", [Encode.string v; oaToJsonString unit]
        Encode.object [
            CellType, Encode.string t
            CellValues, v |> Encode.list
    ]

    let decoder : Decoder<CompositeCell> =
        Decode.object (fun get ->
            match get.Required.Field (CellType) Decode.string with
            | "FreeText" -> 
                let s = get.Required.Field (CellValues) (Decode.index 0 Decode.string)
                CompositeCell.FreeText s
            | "Term" -> 
                let oa = get.Required.Field (CellValues) (Decode.index 0 <| OntologyAnnotation.decoder (ConverterOptions()) )
                CompositeCell.Term oa
            | "Unitized" -> 
                let v = get.Required.Field (CellValues) (Decode.index 0 <| Decode.string )
                let oa = get.Required.Field (CellValues) (Decode.index 1 <| OntologyAnnotation.decoder (ConverterOptions()) )
                CompositeCell.Unitized (v, oa)
            | anyelse -> failwithf "Error reading CompositeCell from json string: %A" anyelse 
        ) 

    let compressedEncoder (stringTable : StringTableMap) (oaTable : OATableMap) (cc: CompositeCell) =

        let t, v =
            match cc with
            | CompositeCell.FreeText s -> "FreeText", [StringTable.encodeString stringTable s]
            | CompositeCell.Term t -> "Term", [OATable.encodeOA oaTable t]
            | CompositeCell.Unitized (v, unit) -> "Unitized", [StringTable.encodeString stringTable v; OATable.encodeOA oaTable unit]
        Encode.object [
            CompressedCellType, StringTable.encodeString stringTable t
            CompressedCellValues, v |> Encode.list
    ]
    
    let compressedDecoder (stringTable : StringTableArray) (oaTable : OATableArray) : Decoder<CompositeCell> =

        Decode.object (fun get ->
            match get.Required.Field (CompressedCellType) (StringTable.decodeString stringTable) with
            | "FreeText" -> 
                let s = get.Required.Field (CompressedCellValues) (Decode.index 0 (StringTable.decodeString stringTable))
                CompositeCell.FreeText s
            | "Term" -> 
                let oa = get.Required.Field (CompressedCellValues) (Decode.index 0 <| OATable.decodeOA oaTable )
                CompositeCell.Term oa
            | "Unitized" -> 
                let v = get.Required.Field (CompressedCellValues) (Decode.index 0 <| (StringTable.decodeString stringTable) )
                let oa = get.Required.Field (CompressedCellValues) (Decode.index 1 <| OATable.decodeOA oaTable )
                CompositeCell.Unitized (v, oa)
            | anyelse -> failwithf "Error reading CompositeCell from json string: %A" anyelse 
        ) 

[<AutoOpen>]
module CompositeCellExtensions =

    type CompositeCell with
        static member fromJsonString (jsonString: string) : CompositeCell = 
            GDecode.fromJsonString CompositeCell.decoder jsonString
            
        member this.ToJsonString(?spaces) : string =
            let spaces = defaultArg spaces 0
            Encode.toJsonString spaces (CompositeCell.encoder this)

        static member toJsonString(a:CompositeCell) = a.ToJsonString()
        