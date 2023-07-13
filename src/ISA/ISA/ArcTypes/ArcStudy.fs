namespace ISA

open Fable.Core
open ISA.Aux

module ArcStudyAux =
    module SanityChecks = 
        let inline validateUniqueAssayIdentifier (assay: ArcAssay) (existingAssays: seq<ArcAssay>) =
            match Seq.tryFindIndex (fun x -> x.FileName = assay.FileName) existingAssays with
            | Some i ->
                failwith $"Cannot create assay with name {assay.FileName}, as assay names must be unique and assay at index {i} has the same name."
            | None ->
                ()

[<AttachMembers>]
type ArcStudy = 
    {
        ID : URI option
        mutable FileName : string option
        Identifier : string option
        mutable Title : string option
        mutable Description : string option
        mutable SubmissionDate : string option
        mutable PublicReleaseDate : string option
        mutable Publications : Publication list
        mutable Contacts : Person list
        mutable StudyDesignDescriptors : OntologyAnnotation list
        mutable Materials : StudyMaterials option
        Tables : ResizeArray<ArcTable>
        // Make this mutable?
        Assays : ResizeArray<ArcAssay>
        mutable Factors : Factor list
        /// List of all the characteristics categories (or material attributes) defined in the study, used to avoid duplication of their declaration when each material_attribute_value is created. 
        mutable CharacteristicCategories : MaterialAttribute list
        /// List of all the unitsdefined in the study, used to avoid duplication of their declaration when each value is created.
        mutable UnitCategories : OntologyAnnotation list
        mutable Comments : Comment list
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
            (this.Publications = []) &&
            (this.Contacts = []) &&
            (this.StudyDesignDescriptors = []) &&
            (this.Materials = None) &&
            (this.Tables.Count = 0) &&
            (this.Assays.Count = 0) &&
            (this.Factors = []) &&
            (this.CharacteristicCategories = []) &&
            (this.UnitCategories = []) &&
            (this.Comments = [])

    [<NamedParams>]
    static member create (identifier : string, ?id, ?fileName, ?title, ?description, ?submissionDate, ?publicReleaseDate, ?publications, ?contacts, ?studyDesignDescriptors, ?materials, ?tables, ?assays, ?factors, ?characteristicCategories, ?unitCategories, ?comments) = 
        let tables = defaultArg tables <| ResizeArray()
        let assays = defaultArg assays <| ResizeArray()
        let publications = defaultArg publications []
        let contacts = defaultArg contacts []
        let studyDesignDescriptors = defaultArg studyDesignDescriptors []
        let factors = defaultArg factors []
        let characteristicCategories = defaultArg characteristicCategories []
        let unitCategories = defaultArg unitCategories []
        let comments = defaultArg comments []
        ArcStudy.make id fileName (Option.fromValueWithDefault "" identifier) title description submissionDate publicReleaseDate publications contacts studyDesignDescriptors materials tables assays factors characteristicCategories unitCategories comments

    static member createEmpty() = ArcStudy.make None None None None None None None [] [] [] None (ResizeArray()) (ResizeArray()) [] [] [] []

    //static member fromStudy (study : Study) : ArcStudy = 
    //    raise (System.NotImplementedException())

    //static member toStudy (arcStudy : ArcStudy) : Study =
    //    raise (System.NotImplementedException())

    // - Assay API - CRUD //
    member this.AddAssay(assay: ArcAssay) =
        ArcStudyAux.SanityChecks.validateUniqueAssayIdentifier assay this.Assays
        this.Assays.Add(assay)

    static member addAssay(assay: ArcAssay) =
        fun (study:ArcStudy) ->
            let newStudy = study.Copy()
            newStudy.AddAssay(assay)
            newStudy

    // - Assay API - CRUD //
    member this.AddAssayEmpty(assayName: string) =
        let assay = ArcAssay.create(assayName)
        this.AddAssay(assay)

    static member addAssayEmpty(assayName: string) =
        fun (study:ArcStudy) ->
            let newStudy = study.Copy()
            newStudy.AddAssayEmpty(assayName)
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
        with get() = ArcTables(this.Tables).TableCount

    member this.TableNames 
        with get() = ArcTables(this.Tables).TableNames

        // - Table API - //
    // remark should this return ArcTable?
    member this.AddTable(table:ArcTable, ?index: int) = ArcTables(this.Tables).AddTable(table, ?index = index)

    static member addTable(table:ArcTable, ?index: int) =
        fun (study:ArcStudy) ->
            let c = study.Copy()
            c.AddTable(table, ?index = index)
            c

    // - Table API - //
    member this.AddTables(tables:seq<ArcTable>, ?index: int) = ArcTables(this.Tables).AddTables(tables, ?index = index)

    static member addTables(tables:seq<ArcTable>, ?index: int) =
        fun (study:ArcStudy) ->
            let c = study.Copy()
            c.AddTables(tables, ?index = index)
            c

    // - Table API - //
    member this.InitTable(tableName:string, ?index: int) = ArcTables(this.Tables).InitTable(tableName, ?index = index)

    static member initTable(tableName: string, ?index: int) =
        fun (study:ArcStudy) ->
            let c = study.Copy()
            c.InitTable(tableName, ?index=index)
            c

    // - Table API - //
    member this.InitTables(tableNames:seq<string>, ?index: int) =  ArcTables(this.Tables).InitTables(tableNames, ?index = index)

    static member initTables(tableNames:seq<string>, ?index: int) =
        fun (study:ArcStudy) ->
            let c = study.Copy()
            c.InitTables(tableNames, ?index=index)
            c

    // - Table API - //
    member this.GetTableAt(index:int) : ArcTable = ArcTables(this.Tables).GetTableAt(index)

    /// Receive **copy** of table at `index`
    static member getTableAt(index:int) : ArcStudy -> ArcTable =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetTableAt(index)

    // - Table API - //
    member this.GetTable(name: string) : ArcTable = ArcTables(this.Tables).GetTable(name)

    /// Receive **copy** of table with `name` = `ArcTable.Name`
    static member getTable(name: string) : ArcStudy -> ArcTable =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetTable(name)

    // - Table API - //
    member this.UpdateTableAt(index:int, table:ArcTable) = ArcTables(this.Tables).UpdateTableAt(index, table)

    static member updateTableAt(index:int, table:ArcTable) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.UpdateTableAt(index, table)
            newAssay

    // - Table API - //
    member this.UpdateTable(name: string, table:ArcTable) : unit = ArcTables(this.Tables).UpdateTable(name, table)

    static member updateTable(name: string, table:ArcTable) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.UpdateTable(name, table)
            newAssay

    // - Table API - //
    member this.RemoveTableAt(index:int) : unit = ArcTables(this.Tables).RemoveTableAt(index)

    static member removeTableAt(index:int) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveTableAt(index)
            newAssay

    // - Table API - //
    member this.RemoveTable(name: string) : unit = ArcTables(this.Tables).RemoveTable(name)

    static member removeTable(name: string) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveTable(name)
            newAssay

    // - Table API - //
    // Remark: This must stay `ArcTable -> unit` so name cannot be changed here.
    member this.MapTableAt(index: int, updateFun: ArcTable -> unit) = ArcTables(this.Tables).MapTableAt(index, updateFun)

    static member mapTableAt(index:int, updateFun: ArcTable -> unit) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()    
            newAssay.MapTableAt(index, updateFun)
            newAssay

    // - Table API - //
    member this.MapTable(name: string, updateFun: ArcTable -> unit) : unit = ArcTables(this.Tables).MapTable(name, updateFun)

    static member mapTable(name: string, updateFun: ArcTable -> unit) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.MapTable(name, updateFun)
            newAssay

    // - Table API - //
    member this.RenameTableAt(index: int, newName: string) : unit = ArcTables(this.Tables).RenameTableAt(index, newName)

    static member renameTableAt(index: int, newName: string) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()    
            newAssay.RenameTableAt(index, newName)
            newAssay

    // - Table API - //
    member this.RenameTable(name: string, newName: string) : unit = ArcTables(this.Tables).RenameTable(name, newName)

    static member renameTable(name: string, newName: string) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RenameTable(name, newName)
            newAssay

    // - Column CRUD API - //
    member this.AddColumnAt(tableIndex:int, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) = 
        ArcTables(this.Tables).AddColumnAt(tableIndex, header, ?cells=cells, ?columnIndex = columnIndex, ?forceReplace = forceReplace)

    static member addColumnAt(tableIndex:int, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) : ArcStudy -> ArcStudy = 
        fun (study: ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.AddColumnAt(tableIndex, header, ?cells=cells, ?columnIndex=columnIndex, ?forceReplace=forceReplace)
            newAssay

    // - Column CRUD API - //
    member this.AddColumn(tableName: string, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) =
            ArcTables(this.Tables).AddColumn(tableName, header, ?cells=cells, ?columnIndex = columnIndex, ?forceReplace = forceReplace)

    static member addColumn(tableName: string, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.AddColumn(tableName, header, ?cells=cells, ?columnIndex=columnIndex, ?forceReplace=forceReplace)
            newAssay

    // - Column CRUD API - //
    member this.RemoveColumnAt(tableIndex: int, columnIndex: int) =
        ArcTables(this.Tables).RemoveColumnAt(tableIndex, columnIndex)

    static member removeColumnAt(tableIndex: int, columnIndex: int) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveColumnAt(tableIndex, columnIndex)
            newAssay

    // - Column CRUD API - //
    member this.RemoveColumn(tableName: string, columnIndex: int) : unit =
        ArcTables(this.Tables).RemoveColumn(tableName, columnIndex)

    static member removeColumn(tableName: string, columnIndex: int) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveColumn(tableName, columnIndex)
            newAssay

    // - Column CRUD API - //
    member this.UpdateColumnAt(tableIndex: int, columnIndex: int, header: CompositeHeader, ?cells: CompositeCell []) =
        ArcTables(this.Tables).UpdateColumnAt(tableIndex, columnIndex, header, ?cells = cells)

    static member updateColumnAt(tableIndex: int, columnIndex: int, header: CompositeHeader, ?cells: CompositeCell []) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.UpdateColumnAt(tableIndex, columnIndex, header, ?cells=cells)
            newAssay

    // - Column CRUD API - //
    member this.UpdateColumn(tableName: string, columnIndex: int, header: CompositeHeader, ?cells: CompositeCell []) =
        ArcTables(this.Tables).UpdateColumn(tableName, columnIndex, header, ?cells=cells)

    static member updateColumn(tableName: string, columnIndex: int, header: CompositeHeader, ?cells: CompositeCell []) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.UpdateColumn(tableName, columnIndex, header, ?cells=cells)
            newAssay

    // - Column CRUD API - //
    member this.GetColumnAt(tableIndex: int, columnIndex: int) =
        ArcTables(this.Tables).GetColumnAt(tableIndex, columnIndex)

    static member getColumnAt(tableIndex: int, columnIndex: int) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetColumnAt(tableIndex, columnIndex)

    // - Column CRUD API - //
    member this.GetColumn(tableName: string, columnIndex: int) =
        ArcTables(this.Tables).GetColumn(tableName, columnIndex)

    static member getColumn(tableName: string, columnIndex: int) =
        fun (study: ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetColumn(tableName, columnIndex)

    // - Row CRUD API - //
    member this.AddRowAt(tableIndex:int, ?cells: CompositeCell [], ?rowIndex: int) = 
        ArcTables(this.Tables).AddRowAt(tableIndex, ?cells=cells, ?rowIndex = rowIndex)

    static member addRowAt(tableIndex:int, ?cells: CompositeCell [], ?rowIndex: int) : ArcStudy -> ArcStudy = 
        fun (study: ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.AddRowAt(tableIndex, ?cells=cells, ?rowIndex=rowIndex)
            newAssay

    // - Row CRUD API - //
    member this.AddRow(tableName: string, ?cells: CompositeCell [], ?rowIndex: int) =
        ArcTables(this.Tables).AddRow(tableName, ?cells=cells, ?rowIndex = rowIndex)

    static member addRow(tableName: string, ?cells: CompositeCell [], ?rowIndex: int) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.AddRow(tableName, ?cells=cells, ?rowIndex=rowIndex)
            newAssay

    // - Row CRUD API - //
    member this.RemoveRowAt(tableIndex: int, rowIndex: int) =
        ArcTables(this.Tables).RemoveRowAt(tableIndex, rowIndex)

    static member removeRowAt(tableIndex: int, rowIndex: int) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveColumnAt(tableIndex, rowIndex)
            newAssay

    // - Row CRUD API - //
    member this.RemoveRow(tableName: string, rowIndex: int) : unit =
        ArcTables(this.Tables).RemoveRow(tableName, rowIndex)

    static member removeRow(tableName: string, rowIndex: int) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveRow(tableName, rowIndex)
            newAssay

    // - Row CRUD API - //
    member this.UpdateRowAt(tableIndex: int, rowIndex: int, cells: CompositeCell []) =
        ArcTables(this.Tables).UpdateRowAt(tableIndex, rowIndex, cells)

    static member updateRowAt(tableIndex: int, rowIndex: int, cells: CompositeCell []) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.UpdateRowAt(tableIndex, rowIndex, cells)
            newAssay

    // - Row CRUD API - //
    member this.UpdateRow(tableName: string, rowIndex: int, cells: CompositeCell []) =
        ArcTables(this.Tables).UpdateRow(tableName, rowIndex, cells)

    static member updateRow(tableName: string, rowIndex: int, cells: CompositeCell []) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.UpdateRow(tableName, rowIndex, cells)
            newAssay

    // - Row CRUD API - //
    member this.GetRowAt(tableIndex: int, rowIndex: int) =
        ArcTables(this.Tables).GetRowAt(tableIndex, rowIndex)

    static member getRowAt(tableIndex: int, rowIndex: int) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetRowAt(tableIndex, rowIndex)

    // - Row CRUD API - //
    member this.GetRow(tableName: string, rowIndex: int) =
        ArcTables(this.Tables).GetRow(tableName, rowIndex)

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

