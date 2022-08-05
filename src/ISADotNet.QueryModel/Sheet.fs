namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

open System.Collections.Generic
open System.Collections

[<AutoOpen>]
module ProtocolExtensions = 

    type Protocol with
    
        static member rowIndexKeyName = "RowIndex"

        member this.SetRowIndex(index : int) = 
            let c = Comment.create(Name = Protocol.rowIndexKeyName,Value = string index)
            let cs = 
                this.Comments 
                |> Option.defaultValue []
                |> API.CommentList.set c
            API.Protocol.setComments this cs

        member this.GetRowIndex() =
            match this.Comments with
            | Some cs -> cs |> API.CommentList.item Protocol.rowIndexKeyName |> int
            | None -> failwith "protocol does not contain any comments, so no rowIndex could be returned"
            
        member this.TryGetRowIndex() =
            this.Comments
            |> Option.bind (API.CommentList.tryItem Protocol.rowIndexKeyName)
            |> Option.map (int)

        static member setRowIndex i (p : Protocol) = p.SetRowIndex(i)

        static member rowRangeKeyName = "RowRange"

        static member composeRowRange (from : int) (to_ : int) =
            $"{from}:{to_}"

        static member decomposeRowRange (range : string) =
            let pattern = """(?<from>\d+):(?<to>\d+)"""
            let r = System.Text.RegularExpressions.Regex.Match(range,pattern)

            if r.Success then
                (r.Groups.Item "from" ).Value |> int, (r.Groups.Item "to").Value |> int
            else 
                failwithf "protocol rowRange %s could not be parsed. It should be of form \"from:to\" (e.g. 0:10)" range

        member this.SetRowRange(range : string) = 
            let c = Comment.create(Name = Protocol.rowRangeKeyName,Value = range)
            let cs = 
                this.Comments 
                |> Option.defaultValue []
                |> API.CommentList.set c
            API.Protocol.setComments this cs

        member this.SetRowRange(from : int, to_ : int) = 
            Protocol.composeRowRange from to_
            |> this.SetRowRange

        member this.GetRowRange() =
            match this.Comments with
            | Some cs -> cs |> API.CommentList.item Protocol.rowRangeKeyName |> Protocol.decomposeRowRange
            | None -> failwith "protocol does not contain any comments, so no rowRange could be returned"
            
        member this.TryGetRowRange() =
            this.Comments
            |> Option.bind (API.CommentList.tryItem Protocol.rowRangeKeyName)
            |> Option.map (Protocol.decomposeRowRange)

        static member setRowRange (range : string) = fun (p : Protocol) -> p.SetRowRange(range)

        static member setRowRange (from : int,to_ : int) = fun (p : Protocol) -> p.SetRowRange(from,to_)

        static member dropRowIndex (p : Protocol) =
            match p.Comments with 
            | None -> p
            | Some cs ->
                API.CommentList.dropByKey Protocol.rowIndexKeyName cs
                |> Option.fromValueWithDefault []
                |> fun cs -> {p with Comments = cs}

        static member rangeOfIndices (i : int list) =
            Protocol.composeRowRange (List.min i) (List.max i)

        static member mergeIndicesToRange (ps : Protocol list) =
            let indices = ps |> List.choose (fun p -> p.TryGetRowIndex())
            if indices.IsEmpty then ps.[0] 
            else
                let r = indices |> Protocol.rangeOfIndices
                ps.[0].SetRowRange r
                |> Protocol.dropRowIndex

        member this.IsChildProtocolTypeOf(parentProtocolType : OntologyAnnotation) =
            match this.ProtocolType with
            | Some pt ->
                OntologyAnnotation.isChildTerm(parentProtocolType,pt)
            | _ -> false

        member this.IsChildProtocolTypeOf(parentProtocolType : OntologyAnnotation, obo : Obo.OboOntology) =
            match this.ProtocolType with
            | Some pt ->
                OntologyAnnotation.isChildTerm(parentProtocolType,pt,obo)
            | _ -> false

    
    type ProtocolDescriptor<'T> =
        | ForAll of 'T
        | ForSpecific of Map<int,'T>

        with member this.TryGet(i) =
                match this with
                | ForAll x -> Some x
                | ForSpecific m -> Map.tryFind i m


/// Queryable type representing a collection of processes implementing the same protocol. Or in ISAtab / ISAXLSX logic a sheet in an assay or study file.
///
/// Values are represented rowwise with input and output entities.
type QSheet = 
    {
        [<JsonPropertyName(@"sheetName")>]
        SheetName : string
        [<JsonPropertyName(@"rows")>]
        Rows : QRow list
        Protocols : Protocol list
    }

    static member create sheetName rows protocols: QSheet =
        {
            SheetName = sheetName
            Rows = rows
            Protocols = protocols
        }

    static member fromProcesses name (processes : Process list) =        
        let protocols = processes |> List.choose (fun p -> p.ExecutesProtocol) |> List.distinct
        let rows = processes |> List.collect (QRow.fromProcess)
        QSheet.create name rows protocols 

    member this.Values = 
        this.Rows
        |> List.collect (fun r -> 
            r.Vals
            |> List.map (fun v -> 
                KeyValuePair ((r.Input,r.Output),v)
            )
        )
        |> IOValueCollection

    member this.TryGetChildProtocolTypeOf(parentProtocolType : OntologyAnnotation) =
        this.Protocols
        |> List.choose (fun p -> if p.IsChildProtocolTypeOf(parentProtocolType) then Some p else None)
        |> Option.fromValueWithDefault []

    member this.TryGetChildProtocolTypeOf(parentProtocolType : OntologyAnnotation, obo : Obo.OboOntology) =
        this.Protocols
        |> List.choose (fun p -> if p.IsChildProtocolTypeOf(parentProtocolType, obo) then Some p else None)
        |> Option.fromValueWithDefault []

    member this.Item (i : int) =
        this.Rows.[i]

    member this.Item (input : string) =
        let row = 
            this.Rows 
            |> List.tryFind (fun r -> r.Input = input)
        match row with
        | Some r -> r
        | None -> failwith $"Sheet \"{this.SheetName}\" does not contain row with input \"{input}\""

    member this.RowCount =
        this.Rows 
        |> List.length

    member this.InputNames =
        this.Rows 
        |> List.map (fun row -> row.Input)

    member this.OutputNames =
        this.Rows 
        |> List.map (fun row -> row.Output)
    
    member this.Inputs =
        this.Rows 
        |> List.map (fun row -> row.Input, row.InputType)

    member this.Outputs =
        this.Rows 
        |> List.map (fun row -> row.Output,row.OutputType)

    interface IEnumerable<QRow> with
        member this.GetEnumerator() = (seq this.Rows).GetEnumerator()

    interface IEnumerable with
        member this.GetEnumerator() = (this :> IEnumerable<QRow>).GetEnumerator() :> IEnumerator