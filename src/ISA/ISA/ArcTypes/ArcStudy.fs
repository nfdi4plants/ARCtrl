namespace ARCtrl.ISA

open Fable.Core
open ARCtrl.ISA.Aux

module ArcStudyAux =
    module SanityChecks = 
        let inline validateUniqueAssayIdentifier (assay: ArcAssay) (existingAssays: seq<ArcAssay>) =
            match existingAssays |> Seq.tryFindIndex (fun x -> x.Identifier = assay.Identifier)  with
            | Some i ->
                failwith $"Cannot create assay with name {assay.Identifier}, as assay names must be unique and assay at index {i} has the same name."
            | None ->
                ()

[<AttachMembers>]
type ArcStudy(identifier : string, ?title, ?description, ?submissionDate, ?publicReleaseDate, ?publications, ?contacts, ?studyDesignDescriptors, ?tables, ?assays, ?factors, ?comments) = 
    let publications = defaultArg publications [||]
    let contacts = defaultArg contacts [||]
    let studyDesignDescriptors = defaultArg studyDesignDescriptors [||]
    let tables = defaultArg tables <| ResizeArray()
    let assays = defaultArg assays <| ResizeArray()
    let factors = defaultArg factors [||]
    let comments = defaultArg comments [||]

    let mutable identifier = identifier
    /// Must be unique in one investigation
    member this.Identifier 
        with get() = identifier
        and internal set(i) = identifier <- i

    member val Title : string option = title with get, set
    member val Description : string option = description with get, set
    member val SubmissionDate : string option = submissionDate with get, set
    member val PublicReleaseDate : string option = publicReleaseDate with get, set
    member val Publications : Publication [] = publications with get, set
    member val Contacts : Person [] = contacts with get, set
    member val StudyDesignDescriptors : OntologyAnnotation [] = studyDesignDescriptors with get, set
    member val Tables : ResizeArray<ArcTable> = tables with get, set
    member val Assays : ResizeArray<ArcAssay> = assays with get, set
    member val Factors : Factor [] = factors with get, set
    member val Comments : Comment []= comments with get, set

    static member init(identifier : string) = ArcStudy identifier

    static member create(identifier : string, ?title, ?description, ?submissionDate, ?publicReleaseDate, ?publications, ?contacts, ?studyDesignDescriptors, ?tables, ?assays, ?factors, ?comments) = 
        ArcStudy(identifier, ?title = title, ?description = description, ?submissionDate =  submissionDate, ?publicReleaseDate = publicReleaseDate, ?publications = publications, ?contacts = contacts, ?studyDesignDescriptors = studyDesignDescriptors, ?tables = tables, ?assays = assays, ?factors = factors, ?comments = comments)

    static member make identifier title description submissionDate publicReleaseDate publications contacts studyDesignDescriptors tables assays factors comments = 
        ArcStudy(identifier, ?title = title, ?description = description, ?submissionDate =  submissionDate, ?publicReleaseDate = publicReleaseDate, publications = publications, contacts = contacts, studyDesignDescriptors = studyDesignDescriptors, tables = tables, assays = assays, factors = factors, comments = comments)

    /// <summary>
    /// Returns true if all fields are None/ empty sequences **except** Identifier.
    /// </summary>
    member this.isEmpty 
        with get() =
            (this.Title = None) &&
            (this.Description = None) &&
            (this.SubmissionDate = None) &&
            (this.PublicReleaseDate = None) &&
            (this.Publications = [||]) &&
            (this.Contacts = [||]) &&
            (this.StudyDesignDescriptors = [||]) &&
            (this.Tables.Count = 0) &&
            (this.Assays.Count = 0) &&
            (this.Factors = [||]) &&
            (this.Comments = [||])

    // Not sure how to handle this best case.
    static member FileName = ARCtrl.Path.StudyFileName
    //member this.FileName = ArcStudy.FileName

    member this.AssayCount 
        with get() = this.Assays.Count

    member this.AssayIdentifiers 
        with get() = this.Assays |> Seq.map (fun (x:ArcAssay) -> x.Identifier)

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
    member this.InitAssay(assayName: string) =
        let assay = ArcAssay(assayName)
        this.AddAssay(assay)
        assay

    static member initAssay(assayName: string) =
        fun (study:ArcStudy) ->
            let newStudy = study.Copy()
            newStudy.InitAssay(assayName)

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
        ArcStudyAux.SanityChecks.validateUniqueAssayIdentifier assay (this.Assays |> Seq.removeAt index)
        this.Assays.[index] <- assay

    static member setAssayAt(index: int, assay: ArcAssay) =
        fun (study:ArcStudy) ->
            let newStudy = study.Copy()
            newStudy.SetAssayAt(index, assay)
            newStudy

        // - Assay API - CRUD //
    member this.SetAssay(assayIdentifier: string, assay: ArcAssay) =
        let index = this.GetAssayIndex(assayIdentifier)
        this.Assays.[index] <- assay

    static member setAssay(assayIdentifier: string, assay: ArcAssay) =
        fun (study:ArcStudy) ->
            let newStudy = study.Copy()
            newStudy.SetAssay(assayIdentifier, assay)
            newStudy

    // - Assay API - CRUD //
    member this.GetAssayIndex(assayIdentifier: string) =
        let index = this.Assays.FindIndex (fun a -> a.Identifier = assayIdentifier)
        if index = -1 then failwith $"Unable to find assay with specified identifier '{assayIdentifier}'!"
        index

    static member GetAssayIndex(assayIdentifier: string) : ArcStudy -> int =
        fun (study: ArcStudy) -> study.GetAssayIndex(assayIdentifier)

    // - Assay API - CRUD //
    member this.GetAssayAt(index: int) : ArcAssay =
        this.Assays.[index]

    static member getAssayAt(index: int) : ArcStudy -> ArcAssay =
        fun (study: ArcStudy) ->
            let newStudy = study.Copy()
            newStudy.GetAssayAt(index)

    // - Assay API - CRUD //
    member this.GetAssay(assayIdentifier: string) : ArcAssay =
        let index = this.GetAssayIndex(assayIdentifier)
        this.GetAssayAt index

    static member getAssay(assayIdentifier: string) : ArcStudy -> ArcAssay =
        fun (study: ArcStudy) ->
            let newStudy = study.Copy()
            newStudy.GetAssay(assayIdentifier)

    ////////////////////////////////////
    // - Copy & Paste from ArcAssay - //
    ////////////////////////////////////
    
    member this.TableCount 
        with get() = ArcTables(this.Tables).Count

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
        let nextTables = ResizeArray()
        let nextAssays = ResizeArray()
        for table in this.Tables do
            let copy = table.Copy()
            nextTables.Add(copy)
        for study in this.Assays do
            let copy = study.Copy()
            nextAssays.Add(copy)
        let nextComments = this.Comments |> Array.map (fun c -> c.Copy())
        let nextFactors = this.Factors |> Array.map (fun c -> c.Copy())
        let nextContacts = this.Contacts |> Array.map (fun c -> c.Copy())
        let nextPublications = this.Publications |> Array.map (fun c -> c.Copy())
        let nextStudyDesignDescriptors = this.StudyDesignDescriptors |> Array.map (fun c -> c.Copy())
        ArcStudy(
            this.Identifier, 
            ?title = this.Title, 
            ?description = this.Description, 
            ?submissionDate = this.SubmissionDate, 
            ?publicReleaseDate = this.PublicReleaseDate, 
            publications = nextPublications, 
            contacts = nextContacts,
            studyDesignDescriptors = nextStudyDesignDescriptors,
            tables = nextTables,
            assays = nextAssays,
            factors = nextFactors,
            comments = nextComments
        )

    /// Transform an ArcStudy to an ISA Json Study.
    member this.ToStudy() : Study = 
        let processSeq = ArcTables(this.Tables).GetProcesses()
        let protocols = ProcessSequence.getProtocols processSeq |> Option.fromValueWithDefault []
        let assays = this.Assays |> Seq.toList |> List.map (fun a -> a.ToAssay()) |> Option.fromValueWithDefault []
        let studyMaterials =
            StudyMaterials.create(
                ?Sources = (ProcessSequence.getSources processSeq |> Option.fromValueWithDefault []),
                ?Samples = (ProcessSequence.getSamples processSeq |> Option.fromValueWithDefault []),
                ?OtherMaterials = (ProcessSequence.getMaterials processSeq |> Option.fromValueWithDefault [])
            )
            |> Option.fromValueWithDefault StudyMaterials.empty
        let identifier,fileName = 
            if ARCtrl.ISA.Identifier.isMissingIdentifier this.Identifier then
                None, None
            else
                Some this.Identifier, Some (Identifier.Study.fileNameFromIdentifier this.Identifier)             
        Study.create(
            ?FileName = fileName,
            ?Identifier = identifier,
            ?Title = this.Title,
            ?Description = this.Description,
            ?SubmissionDate = this.SubmissionDate,
            ?PublicReleaseDate = this.PublicReleaseDate,
            ?Publications = (this.Publications |> List.ofArray |> Option.fromValueWithDefault []),
            ?Contacts = (this.Contacts |> List.ofArray |> Option.fromValueWithDefault []),
            ?StudyDesignDescriptors = (this.StudyDesignDescriptors |> List.ofArray |> Option.fromValueWithDefault []),
            ?Protocols = protocols,
            ?Materials = studyMaterials,
            ?ProcessSequence = (processSeq |> Option.fromValueWithDefault []),
            ?Assays = assays,
            ?Factors = (this.Factors |> List.ofArray |> Option.fromValueWithDefault []),
            ?CharacteristicCategories = (ProcessSequence.getCharacteristics processSeq |> Option.fromValueWithDefault []),
            ?UnitCategories = (ProcessSequence.getUnits processSeq |> Option.fromValueWithDefault []),
            ?Comments = (this.Comments |> List.ofArray |> Option.fromValueWithDefault [])
            )

    // Create an ArcStudy from an ISA Json Study.
    static member fromStudy (s : Study) : ArcStudy = 
        let tables = (s.ProcessSequence |> Option.map (ArcTables.fromProcesses >> fun t -> t.Tables))
        let identifer = 
            match s.FileName with
            | Some fn -> Identifier.Study.identifierFromFileName fn
            | None -> Identifier.createMissingIdentifier()
        let assays = s.Assays |> Option.map (List.map ArcAssay.fromAssay >> ResizeArray)
        ArcStudy.create(
            identifer,
            ?title = s.Title,
            ?description = s.Description,
            ?submissionDate = s.SubmissionDate,
            ?publicReleaseDate = s.PublicReleaseDate,
            ?publications = (s.Publications |> Option.map Array.ofList),
            ?contacts = (s.Contacts|> Option.map Array.ofList),
            ?studyDesignDescriptors = (s.StudyDesignDescriptors |> Option.map Array.ofList),
            ?tables = tables,
            ?assays = assays,
            ?factors = (s.Factors |> Option.map Array.ofList),
            ?comments = (s.Comments |> Option.map Array.ofList)
            )

