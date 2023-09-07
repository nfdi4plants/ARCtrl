namespace rec ARCtrl.ISA

open Fable.Core
open ARCtrl.ISA.Aux

module ArcTypesAux =
    module SanityChecks = 

        let inline validateRegisteredInvestigation (investigation: ArcInvestigation option) =
            match investigation with
            | None -> failwith "Cannot execute this function. Object is not part of ArcInvestigation."
            | Some i -> i

        let inline validateAssayRegisterInInvestigation (assayIdent: string) (existingAssayIdents: seq<string>) =
            match existingAssayIdents |> Seq.tryFind (fun x -> x = assayIdent)  with
            | None ->
                failwith $"The given assay must be added to Investigation before it can be registered."
            | Some _ ->
                ()

        let inline validateUniqueAssayIdentifier (assayIdent: string) (existingAssayIdents: seq<string>) =
            match existingAssayIdents |> Seq.tryFindIndex (fun x -> x = assayIdent)  with
            | Some i ->
                failwith $"Cannot create assay with name {assayIdent}, as assay names must be unique and assay at index {i} has the same name."
            | None ->
                ()

        let inline validateUniqueStudyIdentifier (study: ArcStudy) (existingStudies: seq<ArcStudy>) =
            match existingStudies |> Seq.tryFindIndex (fun x -> x.Identifier = study.Identifier) with
            | Some i ->
                failwith $"Cannot create study with name {study.Identifier}, as study names must be unique and study at index {i} has the same name."
            | None ->
                ()

    /// <summary>
    /// Some functions can change ArcInvestigation.Assays elements. After these functions we must remove all registered assays which might have gone lost.
    /// </summary>
    /// <param name="inv"></param>
    let inline removeMissingRegisteredAssays (inv: ArcInvestigation) : unit =
        let existingAssays = inv.AssayIdentifiers
        for study in inv.Studies do
            let registeredAssays = study.AssayIdentifiers 
            for registeredAssay in registeredAssays do
                if Seq.contains registeredAssay existingAssays |> not then
                    study.Assays.Remove registeredAssay |> ignore


[<AttachMembers>]
type ArcAssay(identifier: string, ?measurementType : OntologyAnnotation, ?technologyType : OntologyAnnotation, ?technologyPlatform : OntologyAnnotation, ?tables: ResizeArray<ArcTable>, ?performers : Person [], ?comments : Comment []) = 
    let tables = defaultArg tables <| ResizeArray()
    let performers = defaultArg performers [||]
    let comments = defaultArg comments [||]
    let mutable identifier : string = identifier
    let mutable investigation : ArcInvestigation option = None

    /// Must be unique in one study
    member this.Identifier 
        with get() = identifier
        and internal set(i) = identifier <- i

    // read-online
    member this.Investigation 
        with get() = investigation
        and internal set(i) = investigation <- i

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

    member internal this.AddToInvestigation (investigation: ArcInvestigation) =
        this.Investigation <- Some investigation

    member internal this.RemoveFromInvestigation () =
        this.Investigation <- None

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

[<AttachMembers>]
type ArcStudy(identifier : string, ?title, ?description, ?submissionDate, ?publicReleaseDate, ?publications, ?contacts, ?studyDesignDescriptors, ?tables, ?assays: ResizeArray<string>, ?factors, ?comments) = 
    let publications = defaultArg publications [||]
    let contacts = defaultArg contacts [||]
    let studyDesignDescriptors = defaultArg studyDesignDescriptors [||]
    let tables = defaultArg tables <| ResizeArray()
    let assays = defaultArg assays <| ResizeArray()
    let factors = defaultArg factors [||]
    let comments = defaultArg comments [||]

    let mutable identifier = identifier
    let mutable investigation : ArcInvestigation option = None
    /// Must be unique in one investigation
    member this.Identifier 
        with get() = identifier
        and internal set(i) = identifier <- i
    // read-online
    member this.Investigation 
        with get() = investigation
        and internal set(i) = investigation <- i

    member val Title : string option = title with get, set
    member val Description : string option = description with get, set
    member val SubmissionDate : string option = submissionDate with get, set
    member val PublicReleaseDate : string option = publicReleaseDate with get, set
    member val Publications : Publication [] = publications with get, set
    member val Contacts : Person [] = contacts with get, set
    member val StudyDesignDescriptors : OntologyAnnotation [] = studyDesignDescriptors with get, set
    member val Tables : ResizeArray<ArcTable> = tables with get, set
    member val Assays : ResizeArray<string> = assays with get, set
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
        with get(): string [] = Array.ofSeq this.Assays

    // - Assay API - CRUD //
    /// <summary>
    /// Add assay to investigation and register it to study.
    /// </summary>
    /// <param name="assay"></param>
    member this.AddAssay(assay: ArcAssay) =
        let inv = ArcTypesAux.SanityChecks.validateRegisteredInvestigation this.Investigation 
        inv.AddAssay(assay)
        inv.RegisterAssay(this.Identifier,assay.Identifier)

    static member addAssay(assay: ArcAssay) =
        fun (study:ArcStudy) ->
            let newStudy = study.Copy()
            newStudy.AddAssay(assay)
            newStudy

    // - Assay API - CRUD //
    member this.InitAssay(assayIdentifier: string) =
        let assay = ArcAssay(assayIdentifier)
        this.AddAssay(assay)
        assay

    static member initAssay(assayIdentifier: string) =
        fun (study:ArcStudy) ->
            let newStudy = study.Copy()
            newStudy.InitAssay(assayIdentifier)

    member this.RegisterAssay(assayIdentifier: string) =
        this.Assays.Add(assayIdentifier)

    static member registerAssay(assayIdentifier: string) =
        fun (study: ArcStudy) ->
            let copy = study.Copy()
            copy.RegisterAssay(assayIdentifier)
            copy

    member this.DeregisterAssay(assayIdentifier: string) =
        this.Assays.Remove(assayIdentifier) |> ignore

    static member deregisterAssay(assayIdentifier: string) =
        fun (study: ArcStudy) ->
            let copy = study.Copy()
            copy.DeregisterAssay(assayIdentifier)
            copy

    member this.GetRegisteredAssay(assayIdentifier: string) =
        if Seq.contains assayIdentifier this.Assays |> not then failwith $"Assay `{assayIdentifier}` is not registered on the study."
        let inv = ArcTypesAux.SanityChecks.validateRegisteredInvestigation this.Investigation
        inv.GetAssay(assayIdentifier)

    static member getRegisteredAssay(assayIdentifier: string) =
        fun (study: ArcStudy) ->
            let copy = study.Copy()
            copy.GetRegisteredAssay(assayIdentifier)

    member this.GetRegisteredAssays() = 
        let inv = ArcTypesAux.SanityChecks.validateRegisteredInvestigation this.Investigation
        let assays = ResizeArray()
        for assay in inv.Assays do
            if Seq.contains assay.Identifier this.AssayIdentifiers then
                assays.Add assay
        assays

    static member getRegisteredAssays() =
        fun (study: ArcStudy) ->
            let copy = study.Copy()
            copy.GetRegisteredAssays()

    /// <summary>
    /// Returns ArcAssays registered in study, or if no parent exists, initializies new ArcAssay from identifier.
    /// </summary>
    member this.GetRegisteredAssaysOrIdentifier() = 
        // Two Options:
        // 1. Init new assays with only identifier. This is possible without ArcInvestigation parent.
        // 2. Get full assays from ArcInvestigation parent.
        match this.Investigation with
        | Some i -> 
            this.GetRegisteredAssays()
        | None ->
            this.Assays |> Seq.map (fun x -> ArcAssay.init(x)) |> ResizeArray
        |> Seq.toList 
        |> List.map (fun a -> a.ToAssay()) 

    static member getRegisteredAssaysOrIdentifier() =
        fun (study: ArcStudy) ->
            let copy = study.Copy()
            copy.GetRegisteredAssaysOrIdentifier()

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
    member this.SetTableAt(index:int, table:ArcTable) = ArcTables(this.Tables).SetTableAt(index, table)

    static member setTableAt(index:int, table:ArcTable) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.SetTableAt(index, table)
            newAssay

    // - Table API - //
    member this.SetTable(name: string, table:ArcTable) : unit = ArcTables(this.Tables).SetTable(name, table)

    static member setTable(name: string, table:ArcTable) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.SetTable(name, table)
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

    member internal this.AddToInvestigation (investigation: ArcInvestigation) =
        this.Investigation <- Some investigation

    member internal this.RemoveFromInvestigation () =
        this.Investigation <- None

    member this.Copy() : ArcStudy =
        let nextTables = ResizeArray()
        let nextAssays = ResizeArray(this.Assays)
        for table in this.Tables do
            let copy = table.Copy()
            nextTables.Add(copy)
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
    member this.ToStudy(arcAssays: ResizeArray<ArcAssay>) : Study = 
        let processSeq = ArcTables(this.Tables).GetProcesses()
        let protocols = ProcessSequence.getProtocols processSeq |> Option.fromValueWithDefault []
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
        let assays = arcAssays |> List.ofSeq |> List.map (fun a -> a.ToAssay())
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
            ?Assays = (assays |> Option.fromValueWithDefault []),
            ?Factors = (this.Factors |> List.ofArray |> Option.fromValueWithDefault []),
            ?CharacteristicCategories = (ProcessSequence.getCharacteristics processSeq |> Option.fromValueWithDefault []),
            ?UnitCategories = (ProcessSequence.getUnits processSeq |> Option.fromValueWithDefault []),
            ?Comments = (this.Comments |> List.ofArray |> Option.fromValueWithDefault [])
            )

    // Create an ArcStudy from an ISA Json Study.
    static member fromStudy (s : Study) : (ArcStudy * ResizeArray<ArcAssay>) = 
        let tables = (s.ProcessSequence |> Option.map (ArcTables.fromProcesses >> fun t -> t.Tables))
        let identifer = 
            match s.FileName with
            | Some fn -> Identifier.Study.identifierFromFileName fn
            | None -> Identifier.createMissingIdentifier()
        let assays = s.Assays |> Option.map (List.map ArcAssay.fromAssay >> ResizeArray) |> Option.defaultValue (ResizeArray())
        let assaysIdentifiers = assays |> Seq.map (fun a -> a.Identifier) |> ResizeArray
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
            ?assays = Some assaysIdentifiers,
            ?factors = (s.Factors |> Option.map Array.ofList),
            ?comments = (s.Comments |> Option.map Array.ofList)
            ),
        assays

[<AttachMembers>]
type ArcInvestigation(identifier : string, ?title : string, ?description : string, ?submissionDate : string, ?publicReleaseDate : string, ?ontologySourceReferences : OntologySourceReference [], ?publications : Publication [], ?contacts : Person [], ?assays : ResizeArray<ArcAssay>, ?studies : ResizeArray<ArcStudy>, ?comments : Comment [], ?remarks : Remark []) = 

    let ontologySourceReferences = defaultArg ontologySourceReferences [||]
    let publications = defaultArg publications [||]
    let contacts = defaultArg contacts [||]
    let assays = defaultArg assays (ResizeArray())
    let studies = defaultArg studies (ResizeArray())
    let comments = defaultArg comments [||]
    let remarks = defaultArg remarks [||]

    let mutable identifier = identifier
    /// Must be unique in one investigation
    member this.Identifier 
        with get() = identifier
        and internal set(i) = identifier <- i

    member val Title : string option = title with get, set
    member val Description : string option = description with get, set
    member val SubmissionDate : string option = submissionDate with get, set
    member val PublicReleaseDate : string option = publicReleaseDate with get, set
    member val OntologySourceReferences : OntologySourceReference [] = ontologySourceReferences with get, set
    member val Publications : Publication [] = publications with get, set
    member val Contacts : Person [] = contacts with get, set
    member val Assays : ResizeArray<ArcAssay> = assays with get, set
    member val Studies : ResizeArray<ArcStudy> = studies with get, set
    member val Comments : Comment [] = comments with get, set
    member val Remarks : Remark [] = remarks with get, set

    static member FileName = ARCtrl.Path.InvestigationFileName

    static member init(identifier: string) = ArcInvestigation identifier
    static member create(identifier : string, ?title : string, ?description : string, ?submissionDate : string, ?publicReleaseDate : string, ?ontologySourceReferences : OntologySourceReference [], ?publications : Publication [], ?contacts : Person [], ?assays : ResizeArray<ArcAssay>, ?studies : ResizeArray<ArcStudy>, ?comments : Comment [], ?remarks : Remark []) = 
        ArcInvestigation(identifier, ?title = title, ?description = description, ?submissionDate = submissionDate, ?publicReleaseDate = publicReleaseDate, ?ontologySourceReferences = ontologySourceReferences, ?publications = publications, ?contacts = contacts, ?assays = assays, ?studies = studies, ?comments = comments, ?remarks = remarks)

    static member make (identifier : string) (title : string option) (description : string option) (submissionDate : string option) (publicReleaseDate : string option) (ontologySourceReferences : OntologySourceReference []) (publications : Publication []) (contacts : Person []) (assays: ResizeArray<ArcAssay>) (studies : ResizeArray<ArcStudy>) (comments : Comment []) (remarks : Remark []) : ArcInvestigation =
        ArcInvestigation(identifier, ?title = title, ?description = description, ?submissionDate = submissionDate, ?publicReleaseDate = publicReleaseDate, ontologySourceReferences = ontologySourceReferences, publications = publications, contacts = contacts, assays = assays, studies = studies, comments = comments, remarks = remarks)

    member this.AssayCount 
        with get() = this.Assays.Count

    member this.AssayIdentifiers 
        with get(): seq<string> = this.Assays |> Seq.map (fun (x:ArcAssay) -> x.Identifier)

    // - Assay API - CRUD //
    member this.AddAssay(assay: ArcAssay) =
        ArcTypesAux.SanityChecks.validateUniqueAssayIdentifier assay.Identifier (this.Assays |> Seq.map (fun x -> x.Identifier))
        this.Assays.Add(assay)

    static member addAssay(assay: ArcAssay) =
        fun (inv:ArcInvestigation) ->
            let newInvestigation = inv.Copy()
            newInvestigation.AddAssay(assay)
            newInvestigation

    // - Assay API - CRUD //
    member this.InitAssay(assayIdentifier: string) =
        let assay = ArcAssay(assayIdentifier)
        this.AddAssay(assay)
        assay

    static member initAssay(assayIdentifier: string) =
        fun (inv:ArcInvestigation) ->
            let newInvestigation = inv.Copy()
            newInvestigation.InitAssay(assayIdentifier)

    // - Assay API - CRUD //
    /// <summary>
    /// Removes assay at specified index from ArcInvestigation and deregisteres it from all studies.
    /// </summary>
    /// <param name="index"></param>
    member this.RemoveAssayAt(index: int) =
        this.Assays.RemoveAt(index)
        this.DeregisterMissingAssays()

    static member removeAssayAt(index: int) =
        fun (inv: ArcInvestigation) ->
            let newInvestigation = inv.Copy()
            newInvestigation.RemoveAssayAt(index)
            newInvestigation

    // - Assay API - CRUD //
    /// <summary>
    /// Removes assay with specified identifier from ArcInvestigation and deregisteres it from all studies.
    /// </summary>
    /// <param name="index"></param>
    member this.RemoveAssay(assayIdentifier: string) =
        let index = this.GetAssayIndex(assayIdentifier)
        this.RemoveAssayAt(index)

    static member removeAssay(assayIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.RemoveAssay(assayIdentifier)
            newInv

    // - Assay API - CRUD //
    member this.SetAssayAt(index: int, assay: ArcAssay) =
        ArcTypesAux.SanityChecks.validateUniqueAssayIdentifier assay.Identifier (this.Assays |> Seq.removeAt index |> Seq.map (fun a -> a.Identifier))
        this.Assays.[index] <- assay
        this.DeregisterMissingAssays()

    static member setAssayAt(index: int, assay: ArcAssay) =
        fun (inv:ArcInvestigation) ->
            let newInvestigation = inv.Copy()
            newInvestigation.SetAssayAt(index, assay)
            newInvestigation

        // - Assay API - CRUD //
    member this.SetAssay(assayIdentifier: string, assay: ArcAssay) =
        let index = this.GetAssayIndex(assayIdentifier)
        this.SetAssayAt(index, assay)

    static member setAssay(assayIdentifier: string, assay: ArcAssay) =
        fun (inv:ArcInvestigation) ->
            let newInvestigation = inv.Copy()
            newInvestigation.SetAssay(assayIdentifier, assay)
            newInvestigation

    // - Assay API - CRUD //
    member this.GetAssayIndex(assayIdentifier: string) =
        let index = this.Assays.FindIndex (fun a -> a.Identifier = assayIdentifier)
        if index = -1 then failwith $"Unable to find assay with specified identifier '{assayIdentifier}'!"
        index

    static member getAssayIndex(assayIdentifier: string) : ArcInvestigation -> int =
        fun (inv: ArcInvestigation) -> inv.GetAssayIndex(assayIdentifier)

    // - Assay API - CRUD //
    member this.GetAssayAt(index: int) : ArcAssay =
        this.Assays.[index]

    static member getAssayAt(index: int) : ArcInvestigation -> ArcAssay =
        fun (inv: ArcInvestigation) ->
            let newInvestigation = inv.Copy()
            newInvestigation.GetAssayAt(index)

    // - Assay API - CRUD //
    member this.GetAssay(assayIdentifier: string) : ArcAssay =
        let index = this.GetAssayIndex(assayIdentifier)
        this.GetAssayAt index

    static member getAssay(assayIdentifier: string) : ArcInvestigation -> ArcAssay =
        fun (inv: ArcInvestigation) ->
            let newInvestigation = inv.Copy()
            newInvestigation.GetAssay(assayIdentifier)

    member this.StudyCount 
        with get() = this.Studies.Count

    member this.StudyIdentifiers
        with get() = this.Studies |> Seq.map (fun (x:ArcStudy) -> x.Identifier)

    // - Study API - CRUD //
    member this.AddStudy(study: ArcStudy) =
        ArcTypesAux.SanityChecks.validateUniqueStudyIdentifier study this.Studies
        study.Investigation <- Some this
        this.Studies.Add(study)

    static member addStudy(study: ArcStudy) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.AddStudy(study)
            copy

    // - Study API - CRUD //
    member this.InitStudy (studyName: string) =
        let study = ArcStudy.init(studyName)
        this.AddStudy(study)
        study

    static member initStudy(studyName: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.InitStudy(studyName)

    // - Study API - CRUD //
    member this.RemoveStudyAt(index: int) =
        this.Studies.RemoveAt(index)

    static member removeStudyAt(index: int) =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.RemoveStudyAt(index)
            newInv

    // - Study API - CRUD //
    member this.RemoveStudy(studyIdentifier: string) =
        this.GetStudy(studyIdentifier)
        |> this.Studies.Remove
        |> ignore

    static member removeStudy(studyIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.RemoveStudy(studyIdentifier)
            copy

    // - Study API - CRUD //
    member this.SetStudyAt(index: int, study: ArcStudy) =
        ArcTypesAux.SanityChecks.validateUniqueStudyIdentifier study (this.Studies |> Seq.removeAt index)
        this.Studies.[index] <- study

    static member setStudyAt(index: int, study: ArcStudy) =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.SetStudyAt(index, study)
            newInv

    // - Study API - CRUD //
    member this.SetStudy(studyIdentifier: string, study: ArcStudy) =
        let index = this.GetStudyIndex studyIdentifier
        ArcTypesAux.SanityChecks.validateUniqueStudyIdentifier study (this.Studies |> Seq.removeAt index)
        this.Studies.[index] <- study

    static member setStudy(studyIdentifier: string, study: ArcStudy) =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.SetStudy(studyIdentifier, study)
            newInv

    // - Study API - CRUD //
    member this.GetStudyIndex(studyIdentifier: string) : int =
        let index = this.Studies.FindIndex (fun s -> s.Identifier = studyIdentifier)
        if index = -1 then failwith $"Unable to find study with specified identifier '{studyIdentifier}'!"
        index

    // - Study API - CRUD //
    static member getStudyIndex(studyIdentifier: string) : ArcInvestigation -> int =
        fun (inv: ArcInvestigation) -> inv.GetStudyIndex (studyIdentifier)

    // - Study API - CRUD //
    member this.GetStudyAt(index: int) : ArcStudy =
        this.Studies.[index]

    static member getStudyAt(index: int) : ArcInvestigation -> ArcStudy =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.GetStudyAt(index)

    // - Study API - CRUD //
    member this.GetStudy(studyIdentifier: string) : ArcStudy =
        this.Studies.Find (fun s -> s.Identifier = studyIdentifier)

    static member getStudy(studyIdentifier: string) : ArcInvestigation -> ArcStudy =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.GetStudy(studyIdentifier)

    // - Study API - CRUD //
    /// <summary>
    /// Register an existing assay from ArcInvestigation.Assays to a existing study.
    /// </summary>
    /// <param name="studyIdentifier"></param>
    /// <param name="assay"></param>
    member this.RegisterAssayAt(studyIndex: int, assayIdentifier: string) =
        let study = this.GetStudyAt(studyIndex)
        ArcTypesAux.SanityChecks.validateAssayRegisterInInvestigation assayIdentifier (this.Assays |> Seq.map (fun a -> a.Identifier))
        ArcTypesAux.SanityChecks.validateUniqueAssayIdentifier assayIdentifier study.Assays
        study.RegisterAssay(assayIdentifier)

    static member registerAssayAt(studyIndex: int, assayIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.RegisterAssayAt(studyIndex, assayIdentifier)
            copy

    // - Study API - CRUD //
    /// <summary>
    /// Register an existing assay from ArcInvestigation.Assays to a existing study.
    /// </summary>
    /// <param name="studyIdentifier"></param>
    /// <param name="assay"></param>
    member this.RegisterAssay(studyIdentifier: string, assayIdentifier: string) =
        let index = this.GetStudyIndex(studyIdentifier)
        this.RegisterAssayAt(index, assayIdentifier)

    static member registerAssay(studyIdentifier: string, assayIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.RegisterAssay(studyIdentifier, assayIdentifier)
            copy

    // - Study API - CRUD //
    member this.DeregisterAssayAt(studyIndex: int, assayIdentifier: string) =
        let study = this.GetStudyAt(studyIndex)
        study.DeregisterAssay(assayIdentifier)

    static member deregisterAssayAt(studyIndex: int, assayIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.DeregisterAssayAt(studyIndex, assayIdentifier)
            copy

    // - Study API - CRUD //
    member this.DeregisterAssay(studyIdentifier: string, assayIdentifier: string) =
        let index = this.GetStudyIndex(studyIdentifier)
        this.DeregisterAssayAt(index, assayIdentifier)

    static member deregisterAssay(studyIdentifier: string, assayIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.DeregisterAssay(studyIdentifier, assayIdentifier)
            copy

    // - Study API - CRUD //
    /// <summary>
    /// Deregisters assays not found in ArcInvestigation.Assays from all studies.
    /// </summary>
    member this.DeregisterMissingAssays() =
        ArcTypesAux.removeMissingRegisteredAssays this

    static member deregisterMissingAssays() =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.DeregisterMissingAssays()
            copy

    member this.Copy() : ArcInvestigation =
        let nextAssays = ResizeArray()
        let nextStudies = ResizeArray()
        for assay in this.Assays do
            let copy = assay.Copy()
            nextAssays.Add(copy)
        for study in this.Studies do
            let copy = study.Copy()
            nextStudies.Add(copy)
        let nextComments = this.Comments |> Array.map (fun c -> c.Copy())
        let nextRemarks = this.Remarks |> Array.map (fun c -> c.Copy())
        let nextContacts = this.Contacts |> Array.map (fun c -> c.Copy())
        let nextPublications = this.Publications |> Array.map (fun c -> c.Copy())
        let nextOntologySourceReferences = this.OntologySourceReferences |> Array.map (fun c -> c.Copy())
        ArcInvestigation(
            this.Identifier,
            ?title = this.Title,
            ?description = this.Description,
            ?submissionDate = this.SubmissionDate,
            ?publicReleaseDate = this.PublicReleaseDate,
            ontologySourceReferences = nextOntologySourceReferences,
            publications = nextPublications,
            contacts = nextContacts,
            assays = nextAssays,
            studies = nextStudies, // correct mutable behaviour is tested on this field
            comments = nextComments,
            remarks = nextRemarks
        )


    /// Transform an ArcInvestigation to an ISA Json Investigation.
    member this.ToInvestigation() : Investigation = 
        let studies = this.Studies |> Seq.toList |> List.map (fun a -> a.ToStudy(a.GetRegisteredAssays())) |> Option.fromValueWithDefault []
        let identifier =
            if ARCtrl.ISA.Identifier.isMissingIdentifier this.Identifier then None
            else Some this.Identifier
        Investigation.create(
            FileName = ARCtrl.Path.InvestigationFileName,
            ?Identifier = identifier,
            ?Title = this.Title,
            ?Description = this.Description,
            ?SubmissionDate = this.SubmissionDate,
            ?PublicReleaseDate = this.PublicReleaseDate,
            ?Publications = (this.Publications |> List.ofArray |> Option.fromValueWithDefault []),
            ?Contacts = (this.Contacts |> List.ofArray |> Option.fromValueWithDefault []),
            ?Studies = studies,
            ?Comments = (this.Comments |> List.ofArray |> Option.fromValueWithDefault [])
            )

    // Create an ArcInvestigation from an ISA Json Investigation.
    static member fromInvestigation (i : Investigation) : ArcInvestigation = 
        let identifer = 
            match i.Identifier with
            | Some i -> i
            | None -> Identifier.createMissingIdentifier()
        let studiesRaw, assaysRaw = 
            i.Studies 
            |> Option.defaultValue []
            |> List.map ArcStudy.fromStudy
            |> List.unzip
        let studies = ResizeArray(studiesRaw)
        let assays = assaysRaw |> Seq.concat |> Seq.distinctBy (fun a -> a.Identifier) |> ResizeArray
        ArcInvestigation.create(
            identifer,
            ?title = i.Title,
            ?description = i.Description,
            ?submissionDate = i.SubmissionDate,
            ?publicReleaseDate = i.PublicReleaseDate,
            ?publications = (i.Publications |> Option.map Array.ofList),
            ?contacts = (i.Contacts |> Option.map Array.ofList),
            ?assays = Some assays,
            ?studies = Some studies,
            ?comments = (i.Comments |> Option.map Array.ofList)
            )
