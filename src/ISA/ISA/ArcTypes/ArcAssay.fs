namespace ARCtrl.ISA

open Fable.Core
open ARCtrl.ISA.Aux

// "MyAssay"; "assays/MyAssay/isa.assay.xlsx"

[<AttachMembers>]
type ArcAssay(identifier: string, ?measurementType : OntologyAnnotation, ?technologyType : OntologyAnnotation, ?technologyPlatform : OntologyAnnotation, ?tables: ResizeArray<ArcTable>, ?performers : Person [], ?comments : Comment []) = 
    let tables = defaultArg tables <| ResizeArray()
    let performers = defaultArg performers [||]
    let comments = defaultArg comments [||]
    let mutable identifier : string = identifier

    /// Must be unique in one study
    member this.Identifier 
        with get() = identifier
        and internal set(i) = identifier <- i

    static member FileName = ARCtrl.Path.AssayFileName

    member val MeasurementType : OntologyAnnotation option = measurementType with get, set
    member val TechnologyType : OntologyAnnotation option = technologyType with get, set
    member val TechnologyPlatform : OntologyAnnotation option = technologyPlatform with get, set
    member val Tables : ResizeArray<ArcTable> = tables with get, set
    member val Performers : Person [] = performers with get, set
    member val Comments : Comment [] = comments with get, set

    static member init (identifier : string) = ArcAssay(identifier)
    static member create (identifier: string, ?measurementType : OntologyAnnotation, ?technologyType : OntologyAnnotation, ?technologyPlatform : OntologyAnnotation, ?tables: ResizeArray<ArcTable>, ?performers : Person [], ?comments : Comment []) = 
        ArcAssay(identifier = identifier, ?measurementType = measurementType, ?technologyType = technologyType, ?technologyPlatform = technologyPlatform, ?tables =tables, ?performers = performers, ?comments = comments)

    static member make 
        (identifier : string)
        (measurementType : OntologyAnnotation option)
        (technologyType : OntologyAnnotation option)
        (technologyPlatform : OntologyAnnotation option)
        (tables : ResizeArray<ArcTable>)
        (performers : Person [])
        (comments : Comment []) = 
        ArcAssay(identifier = identifier, ?measurementType = measurementType, ?technologyType = technologyType, ?technologyPlatform = technologyPlatform, tables =tables, performers = performers, comments = comments)

    member this.TableCount 
        with get() = ArcTables(this.Tables).Count

    member this.TableNames 
        with get() = ArcTables(this.Tables).TableNames

    // - Table API - //
    // remark should this return ArcTable?
    member this.AddTable(table:ArcTable, ?index: int) : unit = ArcTables(this.Tables).AddTable(table, ?index = index)

    static member addTable(table:ArcTable, ?index: int) =
        fun (assay:ArcAssay) ->
            let c = assay.Copy()
            c.AddTable(table, ?index = index)
            c

    // - Table API - //
    member this.AddTables(tables:seq<ArcTable>, ?index: int) = ArcTables(this.Tables).AddTables(tables, ?index = index)

    static member addTables(tables:seq<ArcTable>, ?index: int) =
        fun (assay:ArcAssay) ->
            let c = assay.Copy()
            c.AddTables(tables, ?index = index)
            c

    // - Table API - //
    member this.InitTable(tableName:string, ?index: int) = ArcTables(this.Tables).InitTable(tableName, ?index = index)

    static member initTable(tableName: string, ?index: int) =
        fun (assay:ArcAssay) ->
            let c = assay.Copy()
            c.InitTable(tableName, ?index=index)
            

    // - Table API - //
    member this.InitTables(tableNames:seq<string>, ?index: int) =  ArcTables(this.Tables).InitTables(tableNames, ?index = index)

    static member initTables(tableNames:seq<string>, ?index: int) =
        fun (assay:ArcAssay) ->
            let c = assay.Copy()
            c.InitTables(tableNames, ?index=index)
            c

    // - Table API - //
    member this.GetTableAt(index:int) : ArcTable = ArcTables(this.Tables).GetTableAt(index)

    /// Receive **copy** of table at `index`
    static member getTableAt(index:int) : ArcAssay -> ArcTable =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.GetTableAt(index)

    // - Table API - //
    member this.GetTable(name: string) : ArcTable = ArcTables(this.Tables).GetTable(name)

    /// Receive **copy** of table with `name` = `ArcTable.Name`
    static member getTable(name: string) : ArcAssay -> ArcTable =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.GetTable(name)

    // - Table API - //
    member this.UpdateTableAt(index:int, table:ArcTable) = ArcTables(this.Tables).UpdateTableAt(index, table)

    static member updateTableAt(index:int, table:ArcTable) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.UpdateTableAt(index, table)
            newAssay

    // - Table API - //
    member this.UpdateTable(name: string, table:ArcTable) : unit = ArcTables(this.Tables).UpdateTable(name, table)

    static member updateTable(name: string, table:ArcTable) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.UpdateTable(name, table)
            newAssay

    // - Table API - //
    member this.SetTableAt(index:int, table:ArcTable) = ArcTables(this.Tables).SetTableAt(index, table)

    static member setTableAt(index:int, table:ArcTable) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.SetTableAt(index, table)
            newAssay

    // - Table API - //
    member this.SetTable(name: string, table:ArcTable) : unit = ArcTables(this.Tables).SetTable(name, table)

    static member setTable(name: string, table:ArcTable) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.SetTable(name, table)
            newAssay

    // - Table API - //
    member this.RemoveTableAt(index:int) : unit = ArcTables(this.Tables).RemoveTableAt(index)

    static member removeTableAt(index:int) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.RemoveTableAt(index)
            newAssay

    // - Table API - //
    member this.RemoveTable(name: string) : unit = ArcTables(this.Tables).RemoveTable(name)

    static member removeTable(name: string) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.RemoveTable(name)
            newAssay

    // - Table API - //
    // Remark: This must stay `ArcTable -> unit` so name cannot be changed here.
    member this.MapTableAt(index: int, updateFun: ArcTable -> unit) = ArcTables(this.Tables).MapTableAt(index, updateFun)

    static member mapTableAt(index:int, updateFun: ArcTable -> unit) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()    
            newAssay.MapTableAt(index, updateFun)
            newAssay

    // - Table API - //
    member this.MapTable(name: string, updateFun: ArcTable -> unit) : unit = ArcTables(this.Tables).MapTable(name, updateFun)

    static member updateTable(name: string, updateFun: ArcTable -> unit) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.MapTable(name, updateFun)
            newAssay

    // - Table API - //
    member this.RenameTableAt(index: int, newName: string) : unit = ArcTables(this.Tables).RenameTableAt(index, newName)

    static member renameTableAt(index: int, newName: string) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()    
            newAssay.RenameTableAt(index, newName)
            newAssay

    // - Table API - //
    member this.RenameTable(name: string, newName: string) : unit = ArcTables(this.Tables).RenameTable(name, newName)

    static member renameTable(name: string, newName: string) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.RenameTable(name, newName)
            newAssay

    // - Column CRUD API - //
    member this.AddColumnAt(tableIndex:int, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) = 
        ArcTables(this.Tables).AddColumnAt(tableIndex, header, ?cells=cells, ?columnIndex = columnIndex, ?forceReplace = forceReplace)

    static member addColumnAt(tableIndex:int, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) : ArcAssay -> ArcAssay = 
        fun (assay: ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.AddColumnAt(tableIndex, header, ?cells=cells, ?columnIndex=columnIndex, ?forceReplace=forceReplace)
            newAssay

    // - Column CRUD API - //
    member this.AddColumn(tableName: string, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) =
            ArcTables(this.Tables).AddColumn(tableName, header, ?cells=cells, ?columnIndex = columnIndex, ?forceReplace = forceReplace)

    static member addColumn(tableName: string, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.AddColumn(tableName, header, ?cells=cells, ?columnIndex=columnIndex, ?forceReplace=forceReplace)
            newAssay

    // - Column CRUD API - //
    member this.RemoveColumnAt(tableIndex: int, columnIndex: int) =
        ArcTables(this.Tables).RemoveColumnAt(tableIndex, columnIndex)

    static member removeColumnAt(tableIndex: int, columnIndex: int) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.RemoveColumnAt(tableIndex, columnIndex)
            newAssay

    // - Column CRUD API - //
    member this.RemoveColumn(tableName: string, columnIndex: int) : unit =
        ArcTables(this.Tables).RemoveColumn(tableName, columnIndex)

    static member removeColumn(tableName: string, columnIndex: int) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.RemoveColumn(tableName, columnIndex)
            newAssay

    // - Column CRUD API - //
    member this.UpdateColumnAt(tableIndex: int, columnIndex: int, header: CompositeHeader, ?cells: CompositeCell []) =
        ArcTables(this.Tables).UpdateColumnAt(tableIndex, columnIndex, header, ?cells = cells)

    static member updateColumnAt(tableIndex: int, columnIndex: int, header: CompositeHeader, ?cells: CompositeCell []) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.UpdateColumnAt(tableIndex, columnIndex, header, ?cells=cells)
            newAssay

    // - Column CRUD API - //
    member this.UpdateColumn(tableName: string, columnIndex: int, header: CompositeHeader, ?cells: CompositeCell []) =
        ArcTables(this.Tables).UpdateColumn(tableName, columnIndex, header, ?cells=cells)

    static member updateColumn(tableName: string, columnIndex: int, header: CompositeHeader, ?cells: CompositeCell []) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.UpdateColumn(tableName, columnIndex, header, ?cells=cells)
            newAssay

    // - Column CRUD API - //
    member this.GetColumnAt(tableIndex: int, columnIndex: int) =
        ArcTables(this.Tables).GetColumnAt(tableIndex, columnIndex)

    static member getColumnAt(tableIndex: int, columnIndex: int) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.GetColumnAt(tableIndex, columnIndex)

    // - Column CRUD API - //
    member this.GetColumn(tableName: string, columnIndex: int) =
        ArcTables(this.Tables).GetColumn(tableName, columnIndex)

    static member getColumn(tableName: string, columnIndex: int) =
        fun (assay: ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.GetColumn(tableName, columnIndex)

    // - Row CRUD API - //
    member this.AddRowAt(tableIndex:int, ?cells: CompositeCell [], ?rowIndex: int) = 
        ArcTables(this.Tables).AddRowAt(tableIndex, ?cells=cells, ?rowIndex = rowIndex)

    static member addRowAt(tableIndex:int, ?cells: CompositeCell [], ?rowIndex: int) : ArcAssay -> ArcAssay = 
        fun (assay: ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.AddRowAt(tableIndex, ?cells=cells, ?rowIndex=rowIndex)
            newAssay

    // - Row CRUD API - //
    member this.AddRow(tableName: string, ?cells: CompositeCell [], ?rowIndex: int) =
        ArcTables(this.Tables).AddRow(tableName, ?cells=cells, ?rowIndex = rowIndex)

    static member addRow(tableName: string, ?cells: CompositeCell [], ?rowIndex: int) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.AddRow(tableName, ?cells=cells, ?rowIndex=rowIndex)
            newAssay

    // - Row CRUD API - //
    member this.RemoveRowAt(tableIndex: int, rowIndex: int) =
        ArcTables(this.Tables).RemoveRowAt(tableIndex, rowIndex)

    static member removeRowAt(tableIndex: int, rowIndex: int) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.RemoveColumnAt(tableIndex, rowIndex)
            newAssay

    // - Row CRUD API - //
    member this.RemoveRow(tableName: string, rowIndex: int) : unit =
        ArcTables(this.Tables).RemoveRow(tableName, rowIndex)

    static member removeRow(tableName: string, rowIndex: int) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.RemoveRow(tableName, rowIndex)
            newAssay

    // - Row CRUD API - //
    member this.UpdateRowAt(tableIndex: int, rowIndex: int, cells: CompositeCell []) =
        ArcTables(this.Tables).UpdateRowAt(tableIndex, rowIndex, cells)

    static member updateRowAt(tableIndex: int, rowIndex: int, cells: CompositeCell []) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.UpdateRowAt(tableIndex, rowIndex, cells)
            newAssay

    // - Row CRUD API - //
    member this.UpdateRow(tableName: string, rowIndex: int, cells: CompositeCell []) =
        ArcTables(this.Tables).UpdateRow(tableName, rowIndex, cells)

    static member updateRow(tableName: string, rowIndex: int, cells: CompositeCell []) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.UpdateRow(tableName, rowIndex, cells)
            newAssay

    // - Row CRUD API - //
    member this.GetRowAt(tableIndex: int, rowIndex: int) =
        ArcTables(this.Tables).GetRowAt(tableIndex, rowIndex)

    static member getRowAt(tableIndex: int, rowIndex: int) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.GetRowAt(tableIndex, rowIndex)

    // - Row CRUD API - //
    member this.GetRow(tableName: string, rowIndex: int) =
        ArcTables(this.Tables).GetRow(tableName, rowIndex)

    static member getRow(tableName: string, rowIndex: int) =
        fun (assay: ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.GetRow(tableName, rowIndex)

    // - Mutable properties API - //
    static member setPerformers performers (assay: ArcAssay) =
        assay.Performers <- performers
        assay

    member this.Copy() : ArcAssay =
        let nextTables = ResizeArray()
        for table in this.Tables do
            let copy = table.Copy()
            nextTables.Add(copy)
        let nextComments = this.Comments |> Array.map (fun c -> c.Copy())
        let nextPerformers = this.Performers |> Array.map (fun c -> c.Copy())
        ArcAssay(
            this.Identifier,
            ?measurementType = this.MeasurementType,
            ?technologyType = this.TechnologyType, 
            ?technologyPlatform = this.TechnologyPlatform, 
            tables=nextTables, 
            performers=nextPerformers, 
            comments=nextComments
        )

    /// This function creates a string containing the name and the ontology short-string of the given ontology annotation term
    ///
    /// TechnologyPlatforms are plain strings in ISA-JSON.
    ///
    /// This function allows us, to parse them as an ontology term.
    static member composeTechnologyPlatform (tp : OntologyAnnotation) = 
        match tp.TANInfo with
        | Some _ ->
            $"{tp.NameText} ({tp.TermAccessionShort})"
        | None ->
            $"{tp.NameText}"

    /// This function parses the given string containing the name and the ontology short-string of the given ontology annotation term
    ///
    /// TechnologyPlatforms are plain strings in ISA-JSON.
    ///
    /// This function allows us, to parse them as an ontology term.
    static member decomposeTechnologyPlatform (name : string) = 
        let pattern = """(?<value>[^\(]+) \((?<ontology>[^(]*:[^)]*)\)"""

        let r = System.Text.RegularExpressions.Regex.Match(name,pattern)
        

        if r.Success then
            let oa = (r.Groups.Item "ontology").Value   |> OntologyAnnotation.fromTermAnnotation 
            let v =  (r.Groups.Item "value").Value      |> Value.fromString
            {oa with Name = (Some (AnnotationValue.Text v.Text))}
        else 
            OntologyAnnotation.fromString(termName = name)

    /// Transform an ArcAssay to an ISA Json Assay.
    member this.ToAssay() : Assay = 
        let processSeq = ArcTables(this.Tables).GetProcesses()
        let assayMaterials =
            AssayMaterials.create(
                ?Samples = (ProcessSequence.getSamples processSeq |> Option.fromValueWithDefault []),
                ?OtherMaterials = (ProcessSequence.getMaterials processSeq |> Option.fromValueWithDefault [])
            )
            |> Option.fromValueWithDefault AssayMaterials.empty
        let fileName = 
            if ARCtrl.ISA.Identifier.isMissingIdentifier this.Identifier then
                None
            else 
                Some (ARCtrl.ISA.Identifier.Assay.fileNameFromIdentifier this.Identifier)
        Assay.create(
            ?FileName = fileName,
            ?MeasurementType = this.MeasurementType,
            ?TechnologyType = this.TechnologyType,
            ?TechnologyPlatform = (this.TechnologyPlatform |> Option.map ArcAssay.composeTechnologyPlatform),
            ?DataFiles = (ProcessSequence.getData processSeq |> Option.fromValueWithDefault []),
            ?Materials = assayMaterials,
            ?CharacteristicCategories = (ProcessSequence.getCharacteristics processSeq |> Option.fromValueWithDefault []),
            ?UnitCategories = (ProcessSequence.getUnits processSeq |> Option.fromValueWithDefault []),
            ?ProcessSequence = (processSeq |> Option.fromValueWithDefault []),
            ?Comments = (this.Comments |> List.ofArray |> Option.fromValueWithDefault [])
            )

    // Create an ArcAssay from an ISA Json Assay.
    static member fromAssay (a : Assay) : ArcAssay = 
        let tables = (a.ProcessSequence |> Option.map (ArcTables.fromProcesses >> fun t -> t.Tables))
        let identifer = 
            match a.FileName with
            | Some fn -> Identifier.Assay.identifierFromFileName fn
            | None -> Identifier.createMissingIdentifier()
        ArcAssay.create(
            identifer,
            ?measurementType = (a.MeasurementType |> Option.map (fun x -> x.Copy())),
            ?technologyType = (a.TechnologyType |> Option.map (fun x -> x.Copy())),
            ?technologyPlatform = (a.TechnologyPlatform |> Option.map ArcAssay.decomposeTechnologyPlatform),
            ?tables = tables,
            ?comments = (a.Comments |> Option.map Array.ofList)
            )