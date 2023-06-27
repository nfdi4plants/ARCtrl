namespace ISA

open Fable.Core
open ArcAssayAux

[<AttachMembers>]
type ArcStudy = 
    {
        ID : URI option
        FileName : string option
        Identifier : string option
        Title : string option
        Description : string option
        SubmissionDate : string option
        PublicReleaseDate : string option
        Publications : Publication list option
        Contacts : Person list option
        StudyDesignDescriptors : OntologyAnnotation list option
        Materials : StudyMaterials option
        Tables : ResizeArray<ArcTable>
        // Make this mutable?
        Assays : ResizeArray<ArcAssay>
        Factors : Factor list option
        /// List of all the characteristics categories (or material attributes) defined in the study, used to avoid duplication of their declaration when each material_attribute_value is created. 
        CharacteristicCategories : MaterialAttribute list option
        /// List of all the unitsdefined in the study, used to avoid duplication of their declaration when each value is created.
        UnitCategories : OntologyAnnotation list option
        Comments : Comment list option
    }

    static member make id fileName identifier title description submissionDate publicReleaseDate publications contacts studyDesignDescriptors materials tables assays factors characteristicCategories unitCategories comments = 
        {
            ID = id
            FileName = fileName
            Identifier = identifier
            Title = title
            Description = description
            SubmissionDate = submissionDate
            PublicReleaseDate = publicReleaseDate
            Publications = publications
            Contacts = contacts
            StudyDesignDescriptors = studyDesignDescriptors
            Materials = materials
            Tables = tables
            Assays = assays
            Factors = factors
            CharacteristicCategories = characteristicCategories
            UnitCategories = unitCategories
            Comments = comments
        }

    member this.isEmpty 
        with get() =
            (this.ID = None) &&
            (this.FileName = None) &&
            (this.Identifier = None) &&
            (this.Title = None) &&
            (this.Description = None) &&
            (this.SubmissionDate = None) &&
            (this.PublicReleaseDate = None) &&
            (this.Publications = None) &&
            (this.Contacts = None) &&
            (this.StudyDesignDescriptors = None) &&
            (this.Materials = None) &&
            (this.Tables.Count = 0) &&
            (this.Assays.Count = 0) &&
            (this.Factors = None) &&
            (this.CharacteristicCategories = None) &&
            (this.UnitCategories = None) &&
            (this.Comments = None)

    [<NamedParams>]
    static member create (?ID, ?FileName, ?Identifier, ?Title, ?Description, ?SubmissionDate, ?PublicReleaseDate, ?Publications, ?Contacts, ?StudyDesignDescriptors, ?Materials, ?Tables, ?Assays, ?Factors, ?CharacteristicCategories, ?UnitCategories, ?Comments) = 
        let tables = defaultArg Tables <| ResizeArray()
        let assays = defaultArg Assays <| ResizeArray()
        ArcStudy.make ID FileName Identifier Title Description SubmissionDate PublicReleaseDate Publications Contacts StudyDesignDescriptors Materials tables assays Factors CharacteristicCategories UnitCategories Comments

    static member tryGetAssayByID (assayIdentifier : string) (study : Study) : Assay option = 
        raise (System.NotImplementedException())

    static member updateAssayByID (assay : Assay) (assayIdentifier : string) (study : Study) : Study = 
        ArcStudy.tryGetAssayByID assayIdentifier study |> ignore
        raise (System.NotImplementedException())


    static member fromStudy (study : Study) : ArcStudy = 
        raise (System.NotImplementedException())

    static member toStudy (arcStudy : ArcStudy) : Study =
        raise (System.NotImplementedException())


    // - Assay API - CRUD //
    member this.AddAssay(?assay: ArcAssay) =
        let assay = defaultArg assay <| ArcAssay.create()
        this.Assays.Add(assay)

    static member addAssay(?assay: ArcAssay) =
        fun (study:ArcStudy) ->
            let newStudy = study.Copy()
            newStudy.AddAssay(?assay=assay)
            newStudy

    // - Assay API - CRUD //
    member this.RemoveAssayAt(index: int) =
        this.Assays.RemoveAt(index)

    static member removeAssayAt(index: int) =
        fun (study: ArcStudy) ->
            let newStudy = study.Copy()
            newStudy.RemoveAssayAt(index)
            newStudy

    // - Assay API - CRUD //
    member this.SetAssayAt(index: int, assay: ArcAssay) =
        this.Assays.[index] <- assay

    static member setAssayAt(index: int, assay: ArcAssay) =
        fun (study:ArcStudy) ->
            let newStudy = study.Copy()
            newStudy.SetAssayAt(index, assay)
            newStudy

    // - Assay API - CRUD //
    member this.GetAssayAt(index: int) : ArcAssay =
        this.Assays.[index]

    static member getAssayAt(index: int) : ArcStudy -> ArcAssay =
        fun (study: ArcStudy) ->
            let newStudy = study.Copy()
            newStudy.GetAssayAt(index)

    ////////////////////////////////////
    // - Copy & Paste from ArcAssay - //
    ////////////////////////////////////
    
    member this.TableCount 
        with get() = this.Tables.Count

    member this.TableNames 
        with get() = 
            [for s in this.Tables do yield s.Name]

    // - Table API - //
    // remark should this return ArcTable?
    member this.AddTable(table:ArcTable, ?index: int) = 
        let index = defaultArg index this.TableCount
        SanityChecks.validateSheetIndex index true this.Tables
        SanityChecks.validateNewNameUnique table.Name this.TableNames
        this.Tables.Insert(index, table)

    static member addTable(table:ArcTable, ?index: int) =
        fun (study:ArcStudy) ->
            let c = study.Copy()
            c.AddTable(table, ?index = index)
            c

    // - Table API - //
    member this.AddTables(tables:seq<ArcTable>, ?index: int) = 
        let index = defaultArg index this.TableCount
        SanityChecks.validateSheetIndex index true this.Tables
        SanityChecks.validateNewNamesUnique (tables |> Seq.map (fun x -> x.Name)) this.TableNames
        this.Tables.InsertRange(index, tables)

    static member addTables(tables:seq<ArcTable>, ?index: int) =
        fun (study:ArcStudy) ->
            let c = study.Copy()
            c.AddTables(tables, ?index = index)
            c

    // - Table API - //
    member this.InitTable(tableName:string, ?index: int) = 
        let index = defaultArg index this.TableCount
        let table = ArcTable.init(tableName)
        SanityChecks.validateSheetIndex index true this.Tables
        SanityChecks.validateNewNameUnique table.Name this.TableNames
        this.Tables.Insert(index, table)

    static member initTable(tableName: string, ?index: int) =
        fun (study:ArcStudy) ->
            let c = study.Copy()
            c.InitTable(tableName, ?index=index)
            c

    // - Table API - //
    member this.InitTables(tableNames:seq<string>, ?index: int) = 
        let index = defaultArg index this.TableCount
        let tables = tableNames |> Seq.map (fun x -> ArcTable.init(x))
        SanityChecks.validateSheetIndex index true this.Tables
        SanityChecks.validateNewNamesUnique (tables |> Seq.map (fun x -> x.Name)) this.TableNames
        this.Tables.InsertRange(index, tables)

    static member initTables(tableNames:seq<string>, ?index: int) =
        fun (study:ArcStudy) ->
            let c = study.Copy()
            c.InitTables(tableNames, ?index=index)
            c

    // - Table API - //
    member this.GetTableAt(index:int) : ArcTable =
        SanityChecks.validateSheetIndex index false this.Tables
        this.Tables.[index]

    static member getTableAt(index:int) : ArcStudy -> ArcTable =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetTableAt(index)

    // - Table API - //
    member this.GetTable(name: string) : ArcTable =
        tryByTableName name this.Tables
        |> this.GetTableAt

    static member getTable(name: string) : ArcStudy -> ArcTable =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetTable(name)

    // - Table API - //
    member this.SetTableAt(index:int, table:ArcTable) =
        SanityChecks.validateSheetIndex index false this.Tables
        SanityChecks.validateNewNameUnique table.Name this.TableNames
        this.Tables.[index] <- table

    static member setTableAt(index:int, table:ArcTable) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.SetTableAt(index, table)
            newAssay

    // - Table API - //
    member this.SetTable(name: string, table:ArcTable) : unit =
        (tryByTableName name this.Tables, table)
        |> this.SetTableAt

    static member setTable(name: string, table:ArcTable) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.SetTable(name, table)
            newAssay

    // - Table API - //
    member this.RemoveTableAt(index:int) : unit =
        SanityChecks.validateSheetIndex index false this.Tables
        this.Tables.RemoveAt(index)

    static member removeTableAt(index:int) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveTableAt(index)
            newAssay

    // - Table API - //
    member this.RemoveTable(name: string) : unit =
        tryByTableName name this.Tables
        |> this.RemoveTableAt

    static member removeTable(name: string) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveTable(name)
            newAssay

    // - Table API - //
    // Remark: This must stay `ArcTable -> unit` so name cannot be changed here.
    member this.UpdateTableAt(index: int, updateFun: ArcTable -> unit) =
        SanityChecks.validateSheetIndex index false this.Tables
        let table = this.Tables.[index]
        updateFun table

    static member updateTableAt(index:int, updateFun: ArcTable -> unit) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()    
            newAssay.UpdateTableAt(index, updateFun)
            newAssay

    // - Table API - //
    member this.UpdateTable(name: string, updateFun: ArcTable -> unit) : unit =
        (tryByTableName name this.Tables, updateFun)
        |> this.UpdateTableAt

    static member updateTable(name: string, updateFun: ArcTable -> unit) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.UpdateTable(name, updateFun)
            newAssay

    // - Table API - //
    member this.RenameTableAt(index: int, newName: string) : unit =
        SanityChecks.validateSheetIndex index false this.Tables
        SanityChecks.validateNewNameUnique newName this.TableNames
        let table = this.GetTableAt index
        let renamed = {table with Name = newName} 
        this.SetTableAt(index, renamed)

    static member renameTableAt(index: int, newName: string) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()    
            newAssay.RenameTableAt(index, newName)
            newAssay

    // - Table API - //
    member this.RenameTable(name: string, newName: string) : unit =
        (tryByTableName name this.Tables, newName)
        |> this.RenameTableAt

    static member renameTable(name: string, newName: string) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RenameTable(name, newName)
            newAssay

    // - Column CRUD API - //
    member this.AddColumnAt(tableIndex:int, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) = 
        this.UpdateTableAt(tableIndex, fun table ->
            table.AddColumn(header, ?cells=cells, ?index=columnIndex, ?forceReplace=forceReplace)
        )

    static member addColumnAt(tableIndex:int, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) : ArcStudy -> ArcStudy = 
        fun (study: ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.AddColumnAt(tableIndex, header, ?cells=cells, ?columnIndex=columnIndex, ?forceReplace=forceReplace)
            newAssay

    // - Column CRUD API - //
    member this.AddColumn(tableName: string, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) =
        tryByTableName tableName this.Tables
        |> fun i -> this.AddColumnAt(i, header, ?cells=cells, ?columnIndex=columnIndex, ?forceReplace=forceReplace)

    static member addColumn(tableName: string, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.AddColumn(tableName, header, ?cells=cells, ?columnIndex=columnIndex, ?forceReplace=forceReplace)
            newAssay

    // - Column CRUD API - //
    member this.RemoveColumnAt(tableIndex: int, columnIndex: int) =
        this.UpdateTableAt(tableIndex, fun table ->
            table.RemoveColumn(columnIndex)
        )

    static member removeColumnAt(tableIndex: int, columnIndex: int) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveColumnAt(tableIndex, columnIndex)
            newAssay

    // - Column CRUD API - //
    member this.RemoveColumn(tableName: string, columnIndex: int) : unit =
        (tryByTableName tableName this.Tables, columnIndex)
        |> this.RemoveColumnAt

    static member removeColumn(tableName: string, columnIndex: int) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveColumn(tableName, columnIndex)
            newAssay

    // - Column CRUD API - //
    member this.SetColumnAt(tableIndex: int, columnIndex: int, column: CompositeColumn) =
        this.UpdateTableAt(tableIndex, fun table ->
            table.SetColumn(columnIndex, column)
        )

    static member setColumnAt(tableIndex: int, columnIndex: int, column: CompositeColumn) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.SetColumnAt(tableIndex, columnIndex, column)
            newAssay

    // - Column CRUD API - //
    member this.SetColumn(tableName: string, columnIndex: int, column: CompositeColumn) =
        (tryByTableName tableName this.Tables, columnIndex, column)
        |> this.SetColumnAt

    static member setColumn(tableName: string, columnIndex: int, column: CompositeColumn) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.SetColumn(tableName, columnIndex, column)
            newAssay

    // - Column CRUD API - //
    member this.GetColumnAt(tableIndex: int, columnIndex: int) =
        let table = this.GetTableAt(tableIndex)
        table.GetColumn(columnIndex)

    static member getColumnAt(tableIndex: int, columnIndex: int) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetColumnAt(tableIndex, columnIndex)

    // - Column CRUD API - //
    member this.GetColumn(tableName: string, columnIndex: int) =
        (tryByTableName tableName this.Tables, columnIndex)
        |> this.GetColumnAt

    static member getColumn(tableName: string, columnIndex: int) =
        fun (study: ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetColumn(tableName, columnIndex)

    // - Row CRUD API - //
    member this.AddRowAt(tableIndex:int, ?cells: CompositeCell [], ?rowIndex: int) = 
        this.UpdateTableAt(tableIndex, fun table ->
            table.AddRow(?cells=cells, ?index=rowIndex)
        )

    static member addRowAt(tableIndex:int, ?cells: CompositeCell [], ?rowIndex: int) : ArcStudy -> ArcStudy = 
        fun (study: ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.AddRowAt(tableIndex, ?cells=cells, ?rowIndex=rowIndex)
            newAssay

    // - Row CRUD API - //
    member this.AddRow(tableName: string, ?cells: CompositeCell [], ?rowIndex: int) =
        tryByTableName tableName this.Tables
        |> fun i -> this.AddRowAt(i, ?cells=cells, ?rowIndex=rowIndex)

    static member addRow(tableName: string, ?cells: CompositeCell [], ?rowIndex: int) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.AddRow(tableName, ?cells=cells, ?rowIndex=rowIndex)
            newAssay

    // - Row CRUD API - //
    member this.RemoveRowAt(tableIndex: int, rowIndex: int) =
        this.UpdateTableAt(tableIndex, fun table ->
            table.RemoveRow(rowIndex)
        )

    static member removeRowAt(tableIndex: int, rowIndex: int) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveColumnAt(tableIndex, rowIndex)
            newAssay

    // - Row CRUD API - //
    member this.RemoveRow(tableName: string, rowIndex: int) : unit =
        (tryByTableName tableName this.Tables, rowIndex)
        |> this.RemoveRowAt

    static member removeRow(tableName: string, rowIndex: int) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveRow(tableName, rowIndex)
            newAssay

    // - Row CRUD API - //
    member this.SetRowAt(tableIndex: int, rowIndex: int, cells: CompositeCell []) =
        this.UpdateTableAt(tableIndex, fun table ->
            table.SetRow(rowIndex, cells)
        )

    static member setRowAt(tableIndex: int, rowIndex: int, cells: CompositeCell []) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.SetRowAt(tableIndex, rowIndex, cells)
            newAssay

    // - Row CRUD API - //
    member this.SetRow(tableName: string, rowIndex: int, cells: CompositeCell []) =
        (tryByTableName tableName this.Tables, rowIndex, cells)
        |> this.SetRowAt

    static member setRow(tableName: string, rowIndex: int, cells: CompositeCell []) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.SetRow(tableName, rowIndex, cells)
            newAssay

    // - Row CRUD API - //
    member this.GetRowAt(tableIndex: int, rowIndex: int) =
        let table = this.GetTableAt(tableIndex)
        table.GetRow(rowIndex)

    static member getRowAt(tableIndex: int, rowIndex: int) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetRowAt(tableIndex, rowIndex)

    // - Row CRUD API - //
    member this.GetRow(tableName: string, rowIndex: int) =
        (tryByTableName tableName this.Tables, rowIndex)
        |> this.GetRowAt

    static member getRow(tableName: string, rowIndex: int) =
        fun (study: ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetRow(tableName, rowIndex)

    member this.Copy() : ArcStudy =
        let newTables = ResizeArray()
        let newAssays = ResizeArray()
        for table in this.Tables do
            let copy = table.Copy()
            newTables.Add(copy)
        for study in this.Assays do
            let copy = study.Copy()
            newAssays.Add(copy)
        { this with 
            Tables = newTables
            Assays = newAssays
        }

