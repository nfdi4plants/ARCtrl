namespace ISA.Spreadsheet.AssayFile

open FsSpreadsheet
open ISA.Spreadsheet
open ISA.Spreadsheet.ActivePattern
open ISA

open System

/// Functions for parsing an annotation table to the described processes
module AnnotationTable = 

    let annotationTablePrefix = "annotationTable"
    let groupColumnsByHeader (columns : seq<FsColumn>) = 
        columns
        |> Seq.groupWhen false (fun c ->         
            ISA.Regex.tryParseTermAnnotation c.[0].Value 
            |> Option.isSome
            ||
            c.[0].Value = "Unit"
        )

    let parseCompositeHeader (cells : list<FsCell>) : CompositeHeader =
        match cells with
        | Parameter p -> p
        | Factor f -> f
        | Characteristic c -> c
        | Input i -> i
        | Output o -> o
        | FreeText ft -> ft
        | _ -> raise (NotImplementedException("parseCompositeHeader"))

    let parseCompositeCells (cells : list<FsCell>) : CompositeCell =
        let cellValues = cells |> List.map (fun c -> c.Value)
        match cellValues with
        | [v] -> CompositeCell.createFreeText v
        | [v1;v2;v3] -> CompositeCell.createTermFromString(v1,v2,v3)
        | [v1;v2;v3;v4] -> CompositeCell.createUnitizedFromString(v1,v2,v3,v4)
        | _ -> 
            failwithf "Dafuq"

    let parseCompositeColumn (column : list<FsColumn>) : =
        raise (NotImplementedException("parseCompositeColumn"))


    /// Returns the protocol described by the headers and a function for parsing the values of the matrix to the processes of this protocol
    let tryParseTable (sheet : FsWorksheet) =
        let annotationTable =
            sheet.Tables
            |> Seq.tryFind (fun t -> t.Name.StartsWith annotationTablePrefix)
        match annotationTable with
        | Some t -> 
            let compositeColumns = 
                t.Columns(sheet.CellCollection)
                |> groupColumnsByHeader
                |> Seq.map parseCompositeColumn
            compositeColumns
            |> Seq.fold (fun table c -> 
                ArcTable.appendColumn c table
            ) ArcTable.empty
            |> Some
        | None ->
            None







module QRow =
    open FsSpreadsheet.DSL
    open ISA.QueryModel

    let private getTrailingSpaces (i : int) =
        let mutable tail = ""
        for i = 2 to i do 
            tail <- tail + " "
        tail
      

    let renumberHeadersBy (f : 'T -> string) (renamer : 'T -> string -> 'T) (headers : 'T list) = 
        
        let counts = System.Collections.Generic.Dictionary<string, int ref>()
        let renumberHeader num (h : 'T) = 
            counts.[f h] <- ref num
            renamer h (getTrailingSpaces num)

        headers
        |> List.map (fun header ->

            match Dictionary.tryGetValue (f header) counts with
            | Option.Some count -> 
                count := !count + 1 
                renumberHeader !count header
            | _ -> 
                counts.[f header] <- ref 1
                header
        )

    let renumberISAValueHeaders (headers : ISAValue list) =
        headers
        |> renumberHeadersBy 
            (fun i -> i.HeaderText) 
            (fun i s -> 
                i.MapCategory(fun o -> 
                    {o with Name = 
                                o.NameText + s
                                |> AnnotationValue.Text
                                |> Option.Some
                    }
                )
            )

    let renumberStringHeaders (headers : string list) =
        headers
        |> renumberHeadersBy 
            (id) 
            (fun h s -> h + s)

    let toHeaderRow (hasProtocolREF : bool) (hasProtocolType : bool) (rows : QueryModel.QRow list) =
        try 

            let outputType = 
                rows
                |> List.fold (fun outputType r -> 
                    match outputType,r.OutputType with
                    | Option.Some t1, Option.Some t2 when t1 = t2 -> Option.Some t1
                    | Option.Some t1, Option.Some t2 -> failwithf "OutputTypes %A and %A do not match" t1 t2
                    | None, t2 -> t2
                    | t1, None -> t1
                ) None
                |> Option.map IOType.toHeader
                |> Option.defaultValue IOType.defaultOutHeader 

            let valueHeaders,valueMappers = 
                rows
                |> List.collect (fun r -> r.Vals)
                |> List.groupBy (fun v -> v.HeaderText)
                |> List.sortBy (fun (h,vs) -> vs |> List.choose (fun v -> v.TryValueIndex()) |> List.append [System.Int32.MaxValue] |> List.min)
                |> List.map (fun (h,vs) -> 
                    let v = 
                        vs
                        |> List.reduce (fun v1 v2 ->
                            match v1.TryUnit,v2.TryUnit with
                            | Option.Some u1, Option.Some u2 when u1 = u2 -> v1
                            | None, None -> v1
                            | Option.Some u1, Option.Some u2 -> failwithf "Units %s and %s of value with header %s do not match" u1.NameText u2.NameText h
                            | Option.Some u1, None -> failwithf "Units %s and None of value with header %s do not match" u1.NameText h
                            | None, Option.Some u2 -> failwithf "Units None and %s of value with header %s do not match" u2.NameText h
                        )
                    let h = 
                        v.MapCategory(fun o -> 
                            {o with Name = 
                                        o.NameText.TrimEnd()
                                        |> AnnotationValue.Text
                                        |> Option.Some
                            }
                        )
                        |> ISAValue.toHeaders
                    let f (vs : ISAValue list) = 
                        vs 
                        |> List.tryPick (fun v' -> if v'.HeaderText = v.HeaderText then Option.Some (ISAValue.toValues v') else None)
                        |> Option.defaultValue (List.init h.Length (fun _ -> ""))
                    h,f
                )
                |> List.unzip
        
            row {
                IOType.defaultInHeader
                if hasProtocolREF then "Protocol REF"
                if hasProtocolType then 
                    "Protocol Type"
                    "Term Source REF (MS:1000031)"
                    "Term Accession Number (MS:1000031)"
                for v in (valueHeaders |> List.concat |> renumberStringHeaders ) do v
                outputType
            }
            ,valueMappers
        with
        | err -> failwithf "Could not parse headers of row: \n%s" err.Message

    let toValueRow i hasRef hasProtocolType (protocolRef : string option) (protocolType : OntologyAnnotation option) (valueMappers : (ISAValue list -> string list) list) (r : QueryModel.QRow) =
        let protocolVals =
            [
                if hasRef then protocolRef |> Option.defaultValue ""
                if hasProtocolType then yield! protocolType |> Option.map ProtocolType.toValues |> Option.defaultValue ["";"";""]
            ]
        try
            row {
                r.Input
                for v in protocolVals do v
                for v in valueMappers |> List.collect (fun f -> f r.Vals) do v
                r.Output
            }
        with
        | err -> failwithf "Could not parse values of row %i: \n%s" (i+1) err.Message

module QSheet =

    open FsSpreadsheet.DSL
    open ISA.QueryModel

    let toSheet i (s : QueryModel.QSheet) =

        let rows = 
            s.Rows
            |> List.map (fun r -> {r with Vals = QRow.renumberISAValueHeaders r.Vals})

        let hasRef,refs = 
            if s.Protocols |> List.exists (fun p -> p.Name.IsSome && p.Name.Value <> s.SheetName) then
                if s.Protocols.Length = 1 then
                    true, ForAll s.Protocols.Head.Name.Value
                else
                    true, 
                    s.Protocols 
                    |> List.collect (fun p -> 
                        let f,t = p.GetRowRange()
                        List.init (t-f+1) (fun i -> i + f, p.Name.Value)
                    )
                    |> Map.ofList
                    |> ForSpecific
            else 
                false, ForSpecific Map.empty
        let hasProtocolType,protcolTypes = 
            if s.Protocols |> List.exists (fun p -> p.ProtocolType.IsSome) then
                if s.Protocols.Length = 1 then
                    true, ForAll s.Protocols.Head.ProtocolType.Value
                else
                    true, 
                    s.Protocols 
                    |> List.collect (fun p -> 
                        let f,t = p.GetRowRange()
                        List.init (t-f+1) (fun i -> i + f, p.ProtocolType.Value)
                    )
                    |> Map.ofList
                    |> ForSpecific
            else 
                false, ForSpecific Map.empty
        try 
            let headers,mappers = QRow.toHeaderRow hasRef hasProtocolType rows
            sheet s.SheetName {
                table $"annotationTable{i}" {
                    headers
                    for (i,r) in Seq.indexed rows do QRow.toValueRow i hasRef hasProtocolType (refs.TryGet(i)) (protcolTypes.TryGet(i)) mappers r
                }
            }
        with
        | err -> failwithf "Could not parse sheet %s: \n%s" s.SheetName err.Message