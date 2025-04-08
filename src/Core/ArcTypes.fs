namespace rec ARCtrl

open Fable.Core
open ARCtrl.Helper

module ArcTypesAux =

    open System.Collections.Generic

    module ErrorMsgs =

        let unableToFindAssayIdentifier assayIdentifier investigationIdentifier = 
            $"Error. Unable to find assay with identifier '{assayIdentifier}' in investigation {investigationIdentifier}."

        let unableToFindStudyIdentifier studyIdentifer investigationIdentifier =
            $"Error. Unable to find study with identifier '{studyIdentifer}' in investigation {investigationIdentifier}."

        let unableToFindWorkflowIdentifier workflowIdentifier investigationIdentifier =
            $"Error. Unable to find workflow with identifier '{workflowIdentifier}' in investigation {investigationIdentifier}."

        let unableToFindRunIdentifier runIdentifier investigationIdentifier =
            $"Error. Unable to find run with identifier '{runIdentifier}' in investigation {investigationIdentifier}."

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

        let inline validateExistingStudyRegisterInInvestigation (studyIdent: string) (existingStudyIdents: seq<string>) =
            match existingStudyIdents |> Seq.tryFind (fun x -> x = studyIdent)  with
            | None ->
                failwith $"The given study with identifier '{studyIdent}' must be added to Investigation before it can be registered."
            | Some _ ->
                ()
        
        let inline validateUniqueRegisteredStudyIdentifiers (studyIdent: string) (studyIdents: seq<string>) =
            match studyIdents |> Seq.contains studyIdent with
            | true ->
                failwith $"Study with identifier '{studyIdent}' is already registered!"
            | false ->
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

        let inline validateUniqueWorkflowIdentifier (workflow: ArcWorkflow) (existingWorkflows: seq<ArcWorkflow>) =
            match existingWorkflows |> Seq.tryFindIndex (fun x -> x.Identifier = workflow.Identifier) with
            | Some i ->
                failwith $"Cannot create workflow with name {workflow.Identifier}, as workflow names must be unique and workflow at index {i} has the same name."
            | None ->
                ()

        let inline validateUniqueRunIdentifier (run: ArcRun) (existingRuns: seq<ArcRun>) =
            match existingRuns |> Seq.tryFindIndex (fun x -> x.Identifier = run.Identifier) with
            | Some i ->
                failwith $"Cannot create run with name {run.Identifier}, as run names must be unique and run at index {i} has the same name."
            | None ->
                ()

    /// <summary>
    /// Some functions can change ArcInvestigation.Assays elements. After these functions we must remove all registered assays which might have gone lost.
    /// </summary>
    /// <param name="inv"></param>
    let inline removeMissingRegisteredAssays (inv: ArcInvestigation) : unit =
        let existingAssays = inv.AssayIdentifiers
        for study in inv.Studies do
            let rai : ResizeArray<string> = study.RegisteredAssayIdentifiers
            let registeredAssays = ResizeArray(rai)
            for registeredAssay in registeredAssays do
                if Seq.contains registeredAssay existingAssays |> not then
                    study.DeregisterAssay registeredAssay |> ignore

    let inline updateAppendArray (append:bool) (origin: 'A []) (next: 'A []) = 
        if not append then
            next
        else 
            Array.append origin next
            |> Array.distinct

    let inline updateAppendResizeArrayInplace (append:bool) (origin: ResizeArray<'A>) (next: ResizeArray<'A>) = 
        if not append then
            next
        else
            for e in next do
                if origin.Contains e |> not then
                    origin.Add(e)
            origin
       
    let inline updateAppendResizeArray (append:bool) (origin: ResizeArray<'A>) (next: ResizeArray<'A>) = 
        if not append then
            next |> ResizeArray.map id
        else
            let combined = ResizeArray()
            for e in origin do
                if combined.Contains e |> not then
                    combined.Add(e)
            for e in next do
                if combined.Contains e |> not then
                    combined.Add(e)
            combined


[<AttachMembers>]
type ArcAssay(identifier: string, ?title : string, ?description : string, ?measurementType : OntologyAnnotation, ?technologyType : OntologyAnnotation, ?technologyPlatform : OntologyAnnotation, ?tables: ResizeArray<ArcTable>, ?datamap : DataMap, ?performers : ResizeArray<Person>, ?comments : ResizeArray<Comment>) = 
    inherit ArcTables(defaultArg tables <| ResizeArray())

    let performers = defaultArg performers <| ResizeArray()
    let comments = defaultArg comments <| ResizeArray()
    let mutable identifier : string =
        let identifier = identifier.Trim()
        Helper.Identifier.checkValidCharacters identifier
        identifier
    let mutable title : string option = title
    let mutable description : string option = description
    let mutable investigation : ArcInvestigation option = None
    let mutable measurementType : OntologyAnnotation option = measurementType
    let mutable technologyType : OntologyAnnotation option = technologyType
    let mutable technologyPlatform : OntologyAnnotation option = technologyPlatform
    let mutable dataMap : DataMap option = datamap
    let mutable performers = performers
    let mutable comments  = comments
    let mutable staticHash : int = 0

    /// Must be unique in one study
    member this.Identifier with get() = identifier and internal set(i) = identifier <- i
    // read-online
    member this.Investigation with get() = investigation and internal set(i) = investigation <- i
    member this.Title with get() = title and set(t) = title <- t
    member this.Description with get() = description and set(d) = description <- d
    member this.MeasurementType with get() = measurementType and set(n) = measurementType <- n
    member this.TechnologyType with get() = technologyType and set(n) = technologyType <- n
    member this.TechnologyPlatform with get() = technologyPlatform and set(n) = technologyPlatform <- n
    member this.DataMap with get() = dataMap and set(n) = dataMap <- n
    member this.Performers with get() = performers and set(n) = performers <- n
    member this.Comments with get() = comments and set(n) = comments <- n
    member this.StaticHash with get() = staticHash and set(h) = staticHash <- h

    static member init (identifier : string) = ArcAssay(identifier)
    static member create (identifier: string, ?title : string, ?description : string, ?measurementType : OntologyAnnotation, ?technologyType : OntologyAnnotation, ?technologyPlatform : OntologyAnnotation, ?tables: ResizeArray<ArcTable>, ?datamap : DataMap, ?performers : ResizeArray<Person>, ?comments : ResizeArray<Comment>) = 
        ArcAssay(identifier = identifier, ?title = title, ?description = description, ?measurementType = measurementType, ?technologyType = technologyType, ?technologyPlatform = technologyPlatform, ?tables =tables, ?datamap = datamap, ?performers = performers, ?comments = comments)

    static member make 
        (identifier : string)
        (title : string option)
        (description : string option)
        (measurementType : OntologyAnnotation option)
        (technologyType : OntologyAnnotation option)
        (technologyPlatform : OntologyAnnotation option)
        (tables : ResizeArray<ArcTable>)
        (datamap : DataMap option)
        (performers : ResizeArray<Person>)
        (comments : ResizeArray<Comment>) = 
        ArcAssay(identifier = identifier, ?title = title, ?description = description, ?measurementType = measurementType, ?technologyType = technologyType, ?technologyPlatform = technologyPlatform, tables =tables, ?datamap = datamap, performers = performers, comments = comments)

    static member FileName = ARCtrl.ArcPathHelper.AssayFileName
    member this.StudiesRegisteredIn
        with get () = 
            match this.Investigation with
            | Some i -> 
                i.Studies
                |> Seq.filter (fun s -> s.RegisteredAssayIdentifiers |> Seq.contains this.Identifier)
                |> Seq.toArray
            | None -> [||]
        
    // - Table API - //
    static member addTable(table:ArcTable, ?index: int) =
        fun (assay:ArcAssay) ->
            let c = assay.Copy()
            c.AddTable(table, ?index = index)
            c

    // - Table API - //
    static member addTables(tables:seq<ArcTable>, ?index: int) =
        fun (assay:ArcAssay) ->
            let c = assay.Copy()
            c.AddTables(tables, ?index = index)
            c

    // - Table API - //
    static member initTable(tableName: string, ?index: int) =
        fun (assay:ArcAssay) ->
            let c = assay.Copy()
            c,c.InitTable(tableName, ?index=index)
            
    // - Table API - //
    static member initTables(tableNames:seq<string>, ?index: int) =
        fun (assay:ArcAssay) ->
            let c = assay.Copy()
            c.InitTables(tableNames, ?index=index)
            c

    // - Table API - //
    /// Receive **copy** of table at `index`
    static member getTableAt(index:int) : ArcAssay -> ArcTable =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.GetTableAt(index)

    // - Table API - //
    /// Receive **copy** of table with `name` = `ArcTable.Name`
    static member getTable(name: string) : ArcAssay -> ArcTable =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.GetTable(name)

    // - Table API - //
    static member updateTableAt(index:int, table:ArcTable) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.UpdateTableAt(index, table)
            newAssay

    // - Table API - //
    static member updateTable(name: string, table:ArcTable) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.UpdateTable(name, table)
            newAssay

    // - Table API - //
    static member setTableAt(index:int, table:ArcTable) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.SetTableAt(index, table)
            newAssay

    // - Table API - //
    static member setTable(name: string, table:ArcTable) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.SetTable(name, table)
            newAssay

    // - Table API - //
    static member removeTableAt(index:int) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.RemoveTableAt(index)
            newAssay

    // - Table API - //
    static member removeTable(name: string) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.RemoveTable(name)
            newAssay

    // - Table API - //
    static member mapTableAt(index:int, updateFun: ArcTable -> unit) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()    
            newAssay.MapTableAt(index, updateFun)
            newAssay

    // - Table API - //
    static member updateTable(name: string, updateFun: ArcTable -> unit) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.MapTable(name, updateFun)
            newAssay

    // - Table API - //
    static member renameTableAt(index: int, newName: string) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()    
            newAssay.RenameTableAt(index, newName)
            newAssay

    // - Table API - //
    static member renameTable(name: string, newName: string) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.RenameTable(name, newName)
            newAssay

    // - Column CRUD API - //
    static member addColumnAt(tableIndex:int, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) : ArcAssay -> ArcAssay = 
        fun (assay: ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.AddColumnAt(tableIndex, header, ?cells=cells, ?columnIndex=columnIndex, ?forceReplace=forceReplace)
            newAssay

    // - Column CRUD API - //
    static member addColumn(tableName: string, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.AddColumn(tableName, header, ?cells=cells, ?columnIndex=columnIndex, ?forceReplace=forceReplace)
            newAssay

    // - Column CRUD API - //
    static member removeColumnAt(tableIndex: int, columnIndex: int) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.RemoveColumnAt(tableIndex, columnIndex)
            newAssay

    // - Column CRUD API - //
    static member removeColumn(tableName: string, columnIndex: int) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.RemoveColumn(tableName, columnIndex)
            newAssay

    // - Column CRUD API - //
    static member updateColumnAt(tableIndex: int, columnIndex: int, header: CompositeHeader, ?cells: CompositeCell []) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.UpdateColumnAt(tableIndex, columnIndex, header, ?cells=cells)
            newAssay

    // - Column CRUD API - //
    static member updateColumn(tableName: string, columnIndex: int, header: CompositeHeader, ?cells: CompositeCell []) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.UpdateColumn(tableName, columnIndex, header, ?cells=cells)
            newAssay

    // - Column CRUD API - //
    static member getColumnAt(tableIndex: int, columnIndex: int) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.GetColumnAt(tableIndex, columnIndex)

    // - Column CRUD API - //
    static member getColumn(tableName: string, columnIndex: int) =
        fun (assay: ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.GetColumn(tableName, columnIndex)

    // - Row CRUD API - //
    static member addRowAt(tableIndex:int, ?cells: CompositeCell [], ?rowIndex: int) : ArcAssay -> ArcAssay = 
        fun (assay: ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.AddRowAt(tableIndex, ?cells=cells, ?rowIndex=rowIndex)
            newAssay

    // - Row CRUD API - //
    static member addRow(tableName: string, ?cells: CompositeCell [], ?rowIndex: int) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.AddRow(tableName, ?cells=cells, ?rowIndex=rowIndex)
            newAssay

    // - Row CRUD API - //
    static member removeRowAt(tableIndex: int, rowIndex: int) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.RemoveColumnAt(tableIndex, rowIndex)
            newAssay

    // - Row CRUD API - //
    static member removeRow(tableName: string, rowIndex: int) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.RemoveRow(tableName, rowIndex)
            newAssay

    // - Row CRUD API - //
    static member updateRowAt(tableIndex: int, rowIndex: int, cells: CompositeCell []) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.UpdateRowAt(tableIndex, rowIndex, cells)
            newAssay

    // - Row CRUD API - //
    static member updateRow(tableName: string, rowIndex: int, cells: CompositeCell []) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.UpdateRow(tableName, rowIndex, cells)
            newAssay

    // - Row CRUD API - //
    static member getRowAt(tableIndex: int, rowIndex: int) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.GetRowAt(tableIndex, rowIndex)

    // - Row CRUD API - //
    static member getRow(tableName: string, rowIndex: int) =
        fun (assay: ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.GetRow(tableName, rowIndex)

    // - Mutable properties API - //
    static member setPerformers performers (assay: ArcAssay) =
        assay.Performers <- performers
        assay

    member this.Copy() : ArcAssay =
        let nextTables = this.Tables |> ResizeArray.map (fun c -> c.Copy())
        let nextComments = this.Comments |> ResizeArray.map (fun c -> c.Copy())
        let nextDataMap = this.DataMap |> Option.map (fun d -> d.Copy())
        let nextPerformers = this.Performers |> ResizeArray.map (fun c -> c.Copy())
        ArcAssay.make
            this.Identifier
            this.Title
            this.Description
            this.MeasurementType
            this.TechnologyType
            this.TechnologyPlatform
            nextTables
            nextDataMap
            nextPerformers
            nextComments

    /// <summary>
    /// Updates given assay with another assay, Identifier will never be updated. By default update is full replace. Optional Parameters can be used to specify update logic.
    /// </summary>
    /// <param name="assay">The assay used for updating this assay.</param>
    /// <param name="onlyReplaceExisting">If true, this will only update fields which are `Some` or non-empty lists. Default: **false**</param>
    /// <param name="appendSequences">If true, this will append lists instead of replacing. Will return only distinct elements. Default: **false**</param>
    member this.UpdateBy(assay:ArcAssay,?onlyReplaceExisting : bool,?appendSequences : bool) =
        let onlyReplaceExisting = defaultArg onlyReplaceExisting false
        let appendSequences = defaultArg appendSequences false
        let updateAlways = onlyReplaceExisting |> not
        if assay.Title.IsSome || updateAlways then
            this.Title <- assay.Title
        if assay.Description.IsSome || updateAlways then
            this.Description <- assay.Description
        if assay.MeasurementType.IsSome || updateAlways then 
            this.MeasurementType <- assay.MeasurementType
        if assay.TechnologyType.IsSome || updateAlways then 
            this.TechnologyType <- assay.TechnologyType
        if assay.TechnologyPlatform.IsSome || updateAlways then 
            this.TechnologyPlatform <- assay.TechnologyPlatform
        if assay.Tables.Count <> 0 || updateAlways then
            let s = ArcTypesAux.updateAppendResizeArray appendSequences this.Tables assay.Tables
            this.Tables <- s
        if assay.Performers.Count <> 0 || updateAlways then
            let s = ArcTypesAux.updateAppendResizeArray appendSequences this.Performers assay.Performers
            this.Performers <- s
        if assay.Comments.Count <> 0 || updateAlways then
            let s = ArcTypesAux.updateAppendResizeArray appendSequences this.Comments assay.Comments
            this.Comments <- s

    // Use this for better print debugging and better unit test output
    override this.ToString() =
        sprintf 
            """ArcAssay({
    Identifier = "%s",
    Title = %A,
    Description = %A,
    MeasurementType = %A,
    TechnologyType = %A,
    TechnologyPlatform = %A,
    Tables = %A,
    Performers = %A,
    Comments = %A
})"""
            this.Identifier
            this.Title
            this.Description
            this.MeasurementType
            this.TechnologyType
            this.TechnologyPlatform
            this.Tables
            this.Performers
            this.Comments

    member internal this.AddToInvestigation (investigation: ArcInvestigation) =
        this.Investigation <- Some investigation

    member internal this.RemoveFromInvestigation () =
        this.Investigation <- None

    /// Updates given assay stored in an study or investigation file with values from an assay file.
    member this.UpdateReferenceByAssayFile(assay:ArcAssay,?onlyReplaceExisting : bool) =
        let onlyReplaceExisting = defaultArg onlyReplaceExisting false
        let updateAlways = onlyReplaceExisting |> not
        if assay.Title.IsSome || updateAlways then
            this.Title <- assay.Title
        if assay.Description.IsSome || updateAlways then
            this.Description <- assay.Description
        if assay.MeasurementType.IsSome || updateAlways then 
            this.MeasurementType <- assay.MeasurementType
        if assay.TechnologyPlatform.IsSome || updateAlways then 
            this.TechnologyPlatform <- assay.TechnologyPlatform
        if assay.TechnologyType.IsSome || updateAlways then 
            this.TechnologyType <- assay.TechnologyType
        if assay.Tables.Count <> 0 || updateAlways then          
            this.Tables <- assay.Tables
        if assay.Comments.Count <> 0 || updateAlways then          
            this.Comments <- assay.Comments 
        this.DataMap <- assay.DataMap
        if assay.Performers.Count <> 0 || updateAlways then          
            this.Performers <- assay.Performers  

    member this.StructurallyEquals (other: ArcAssay) : bool =
        let i = this.Identifier = other.Identifier
        let t = this.Title = other.Title
        let d = this.Description = other.Description
        let mst = this.MeasurementType = other.MeasurementType
        let tt = this.TechnologyType = other.TechnologyType
        let tp = this.TechnologyPlatform = other.TechnologyPlatform
        let dm = this.DataMap = other.DataMap
        let tables = Seq.compare this.Tables other.Tables
        let perf = Seq.compare this.Performers other.Performers
        let comments = Seq.compare this.Comments other.Comments
        // Todo maybe add reflection check to prove that all members are compared?
        [|i; t; d; mst; tt; tp; dm; tables; perf; comments|] |> Seq.forall (fun x -> x = true)

    /// <summary>
    /// Use this function to check if this ArcAssay and the input ArcAssay refer to the same object.
    ///
    /// If true, updating one will update the other due to mutability.
    /// </summary>
    /// <param name="other">The other ArcAssay to test for reference.</param>
    member this.ReferenceEquals (other: ArcAssay) = System.Object.ReferenceEquals(this,other)

    // custom check
    override this.Equals other =
        match other with
        | :? ArcAssay as assay -> 
            this.StructurallyEquals(assay)
        | _ -> false

    // Hashcode without Datamap
    member this.GetLightHashCode() = 
        [|
            box this.Identifier
            HashCodes.boxHashOption this.Title
            HashCodes.boxHashOption this.Description
            HashCodes.boxHashOption this.MeasurementType
            HashCodes.boxHashOption this.TechnologyType
            HashCodes.boxHashOption this.TechnologyPlatform
            HashCodes.boxHashSeq this.Tables
            HashCodes.boxHashSeq this.Performers
            HashCodes.boxHashSeq this.Comments
        |]
        |> HashCodes.boxHashArray 
        |> fun x -> x :?> int

    override this.GetHashCode() = 
        [|
            box this.Identifier
            HashCodes.boxHashOption this.Title
            HashCodes.boxHashOption this.Description
            HashCodes.boxHashOption this.MeasurementType
            HashCodes.boxHashOption this.TechnologyType
            HashCodes.boxHashOption this.TechnologyPlatform
            HashCodes.boxHashOption this.DataMap
            HashCodes.boxHashSeq this.Tables
            HashCodes.boxHashSeq this.Performers
            HashCodes.boxHashSeq this.Comments
        |]
        |> HashCodes.boxHashArray 
        |> fun x -> x :?> int

[<AttachMembers>]
type ArcStudy(identifier : string, ?title, ?description, ?submissionDate, ?publicReleaseDate, ?publications, ?contacts, ?studyDesignDescriptors, ?tables, ?datamap, ?registeredAssayIdentifiers: ResizeArray<string>, ?comments) = 
    inherit ArcTables(defaultArg tables <| ResizeArray())

    let publications = defaultArg publications <| ResizeArray()
    let contacts = defaultArg contacts <| ResizeArray()
    let studyDesignDescriptors = defaultArg studyDesignDescriptors <| ResizeArray()
    let registeredAssayIdentifiers = defaultArg registeredAssayIdentifiers <| ResizeArray()
    let comments = defaultArg comments <| ResizeArray()

    let mutable identifier : string =
        let identifier = identifier.Trim()
        Helper.Identifier.checkValidCharacters identifier
        identifier
    let mutable investigation : ArcInvestigation option = None
    let mutable title : string option = title
    let mutable description : string option = description 
    let mutable submissionDate : string option = submissionDate
    let mutable publicReleaseDate : string option = publicReleaseDate
    let mutable publications : ResizeArray<Publication> = publications
    let mutable contacts : ResizeArray<Person> = contacts
    let mutable studyDesignDescriptors : ResizeArray<OntologyAnnotation> = studyDesignDescriptors
    let mutable datamap : DataMap option = datamap
    let mutable registeredAssayIdentifiers : ResizeArray<string> = registeredAssayIdentifiers
    let mutable comments : ResizeArray<Comment> = comments
    let mutable staticHash : int = 0

    /// Must be unique in one investigation
    member this.Identifier with get() = identifier and internal set(i) = identifier <- i
    // read-only
    member this.Investigation with get() = investigation and internal set(i) = investigation <- i
    member this.Title with get() = title and set(n) = title <- n
    member this.Description with get() = description and set(n) = description <- n
    member this.SubmissionDate with get() = submissionDate and set(n) = submissionDate <- n
    member this.PublicReleaseDate with get() = publicReleaseDate and set(n) = publicReleaseDate <- n
    member this.Publications with get() = publications and set(n) = publications <- n
    member this.Contacts with get() = contacts and set(n) = contacts <- n
    member this.StudyDesignDescriptors with get() = studyDesignDescriptors and set(n) = studyDesignDescriptors <- n
    member this.DataMap with get() = datamap and set(n) = datamap <- n
    member this.RegisteredAssayIdentifiers with get() = registeredAssayIdentifiers and set(n) = registeredAssayIdentifiers <- n
    member this.Comments with get() = comments and set(n) = comments <- n
    member this.StaticHash with get() = staticHash and set(h) = staticHash <- h

    static member init(identifier : string) = ArcStudy identifier

    static member create(identifier : string, ?title, ?description, ?submissionDate, ?publicReleaseDate, ?publications, ?contacts, ?studyDesignDescriptors, ?tables, ?datamap, ?registeredAssayIdentifiers, ?comments) = 
        ArcStudy(identifier, ?title = title, ?description = description, ?submissionDate =  submissionDate, ?publicReleaseDate = publicReleaseDate, ?publications = publications, ?contacts = contacts, ?studyDesignDescriptors = studyDesignDescriptors, ?tables = tables, ?datamap = datamap, ?registeredAssayIdentifiers = registeredAssayIdentifiers, ?comments = comments)

    static member make identifier title description submissionDate publicReleaseDate publications contacts studyDesignDescriptors tables datamap registeredAssayIdentifiers comments = 
        ArcStudy(identifier, ?title = title, ?description = description, ?submissionDate =  submissionDate, ?publicReleaseDate = publicReleaseDate, publications = publications, contacts = contacts, studyDesignDescriptors = studyDesignDescriptors, tables = tables, ?datamap = datamap, registeredAssayIdentifiers = registeredAssayIdentifiers, comments = comments)

    /// <summary>
    /// Returns true if all fields are None/ empty sequences **except** Identifier.
    /// </summary>
    member this.isEmpty 
        with get() =
            (this.Title = None) &&
            (this.Description = None) &&
            (this.SubmissionDate = None) &&
            (this.PublicReleaseDate = None) &&
            (this.Publications.Count = 0) &&
            (this.Contacts.Count = 0) &&
            (this.StudyDesignDescriptors.Count = 0) &&
            (this.Tables.Count = 0) &&
            (this.RegisteredAssayIdentifiers.Count = 0) &&
            (this.Comments.Count = 0)

    // Not sure how to handle this best case.
    static member FileName = ARCtrl.ArcPathHelper.StudyFileName
    //member this.FileName = ArcStudy.FileName

    /// Returns the count of registered assay *identifiers*. This is not necessarily the same as the count of registered assays, as not all identifiers correspond to an existing assay.
    member this.RegisteredAssayIdentifierCount 
        with get() = this.RegisteredAssayIdentifiers.Count

    /// Returns the count of registered assays. This is not necessarily the same as the count of registered assay *identifiers*, as not all identifiers correspond to an existing assay.
    member this.RegisteredAssayCount 
        with get() = this.RegisteredAssays.Count

    /// Returns all assays registered in this study, that correspond to an existing assay object in the associated investigation.
    member this.RegisteredAssays
        with get(): ResizeArray<ArcAssay> = 
            let inv = ArcTypesAux.SanityChecks.validateRegisteredInvestigation this.Investigation
            this.RegisteredAssayIdentifiers 
            |> Seq.choose inv.TryGetAssay
            |> ResizeArray

    /// Returns all registered assay identifiers that do not correspond to an existing assay object in the associated investigation.
    member this.VacantAssayIdentifiers
        with get() = 
            let inv = ArcTypesAux.SanityChecks.validateRegisteredInvestigation this.Investigation
            this.RegisteredAssayIdentifiers 
            |> Seq.filter (inv.ContainsAssay >> not)
            |> ResizeArray

    // - Assay API - CRUD //
    /// <summary>
    /// Add assay to investigation and register it to study.
    /// </summary>
    /// <param name="assay"></param>
    member this.AddRegisteredAssay(assay: ArcAssay) =
        let inv = ArcTypesAux.SanityChecks.validateRegisteredInvestigation this.Investigation 
        inv.AddAssay(assay)
        inv.RegisterAssay(this.Identifier,assay.Identifier)

    static member addRegisteredAssay(assay: ArcAssay) =
        fun (study:ArcStudy) ->
            let newStudy = study.Copy()
            newStudy.AddRegisteredAssay(assay)
            newStudy

    // - Assay API - CRUD //
    member this.InitRegisteredAssay(assayIdentifier: string) =
        let assay = ArcAssay(assayIdentifier)
        this.AddRegisteredAssay(assay)
        assay

    static member initRegisteredAssay(assayIdentifier: string) =
        fun (study:ArcStudy) ->
            let copy = study.Copy()
            copy,copy.InitRegisteredAssay(assayIdentifier)

    // - Assay API - CRUD //
    member this.RegisterAssay(assayIdentifier: string) =
        if Seq.contains assayIdentifier this.RegisteredAssayIdentifiers then failwith $"Assay `{assayIdentifier}` is already registered on the study."
        this.RegisteredAssayIdentifiers.Add(assayIdentifier)

    static member registerAssay(assayIdentifier: string) =
        fun (study: ArcStudy) ->
            let copy = study.Copy()
            copy.RegisterAssay(assayIdentifier)
            copy

    // - Assay API - CRUD //
    member this.DeregisterAssay(assayIdentifier: string) =
        this.RegisteredAssayIdentifiers.Remove(assayIdentifier) |> ignore

    static member deregisterAssay(assayIdentifier: string) =
        fun (study: ArcStudy) ->
            let copy = study.Copy()
            copy.DeregisterAssay(assayIdentifier)
            copy

    // - Assay API - CRUD //
    member this.GetRegisteredAssay(assayIdentifier: string) =
        if Seq.contains assayIdentifier this.RegisteredAssayIdentifiers |> not then failwith $"Assay `{assayIdentifier}` is not registered on the study."
        let inv = ArcTypesAux.SanityChecks.validateRegisteredInvestigation this.Investigation
        inv.GetAssay(assayIdentifier)

    static member getRegisteredAssay(assayIdentifier: string) =
        fun (study: ArcStudy) ->
            let copy = study.Copy()
            copy.GetRegisteredAssay(assayIdentifier)

    // - Assay API - CRUD //
    static member getRegisteredAssays() =
        fun (study: ArcStudy) ->
            let copy = study.Copy()
            copy.RegisteredAssays

    /// <summary>
    /// Returns ArcAssays registered in study, or if no parent exists, initializies new ArcAssay from identifier.
    /// </summary>
    member this.GetRegisteredAssaysOrIdentifier() = 
        // Two Options:
        // 1. Init new assays with only identifier. This is possible without ArcInvestigation parent.
        // 2. Get full assays from ArcInvestigation parent.
        match this.Investigation with
        | Some i -> 
            this.RegisteredAssayIdentifiers
            |> ResizeArray.map (fun identifier -> 
                match i.TryGetAssay(identifier) with
                | Some assay -> assay
                | None -> ArcAssay.init(identifier)
            )
        | None ->
            this.RegisteredAssayIdentifiers 
            |> ResizeArray.map (fun identifier -> ArcAssay.init(identifier))   

    /// <summary>
    /// Returns ArcAssays registered in study, or if no parent exists, initializies new ArcAssay from identifier.
    /// </summary>
    static member getRegisteredAssaysOrIdentifier() =
        fun (study: ArcStudy) ->
            let copy = study.Copy()
            copy.GetRegisteredAssaysOrIdentifier()

    ////////////////////////////////////
    // - Copy & Paste from ArcAssay - //
    ////////////////////////////////////

    // - Table API - //
    // remark should this return ArcTable?
    static member addTable(table:ArcTable, ?index: int) =
        fun (study:ArcStudy) ->
            let c = study.Copy()
            c.AddTable(table, ?index = index)
            c

    // - Table API - //
    static member addTables(tables:seq<ArcTable>, ?index: int) =
        fun (study:ArcStudy) ->
            let c = study.Copy()
            c.AddTables(tables, ?index = index)
            c

    // - Table API - //
    static member initTable(tableName: string, ?index: int) =
        fun (study:ArcStudy) ->
            let c = study.Copy()
            c,c.InitTable(tableName, ?index=index)
            

    // - Table API - //
    static member initTables(tableNames:seq<string>, ?index: int) =
        fun (study:ArcStudy) ->
            let c = study.Copy()
            c.InitTables(tableNames, ?index=index)
            c

    // - Table API - //
    /// Receive **copy** of table at `index`
    static member getTableAt(index:int) : ArcStudy -> ArcTable =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetTableAt(index)

    // - Table API - //
    /// Receive **copy** of table with `name` = `ArcTable.Name`
    static member getTable(name: string) : ArcStudy -> ArcTable =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetTable(name)

    // - Table API - //
    static member updateTableAt(index:int, table:ArcTable) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.UpdateTableAt(index, table)
            newAssay

    // - Table API - //
    static member updateTable(name: string, table:ArcTable) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.UpdateTable(name, table)
            newAssay


    // - Table API - //
    static member setTableAt(index:int, table:ArcTable) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.SetTableAt(index, table)
            newAssay

    // - Table API - //
    static member setTable(name: string, table:ArcTable) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.SetTable(name, table)
            newAssay

    // - Table API - //
    static member removeTableAt(index:int) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveTableAt(index)
            newAssay

    // - Table API - //
    static member removeTable(name: string) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveTable(name)
            newAssay

    // - Table API - //
    // Remark: This must stay `ArcTable -> unit` so name cannot be changed here.
    static member mapTableAt(index:int, updateFun: ArcTable -> unit) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()    
            newAssay.MapTableAt(index, updateFun)
            newAssay

    // - Table API - //
    static member mapTable(name: string, updateFun: ArcTable -> unit) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.MapTable(name, updateFun)
            newAssay

    // - Table API - //
    static member renameTableAt(index: int, newName: string) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()    
            newAssay.RenameTableAt(index, newName)
            newAssay

    // - Table API - //
    static member renameTable(name: string, newName: string) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RenameTable(name, newName)
            newAssay

    // - Column CRUD API - //
    static member addColumnAt(tableIndex:int, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) : ArcStudy -> ArcStudy = 
        fun (study: ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.AddColumnAt(tableIndex, header, ?cells=cells, ?columnIndex=columnIndex, ?forceReplace=forceReplace)
            newAssay

    // - Column CRUD API - //
    static member addColumn(tableName: string, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.AddColumn(tableName, header, ?cells=cells, ?columnIndex=columnIndex, ?forceReplace=forceReplace)
            newAssay

    // - Column CRUD API - //
    static member removeColumnAt(tableIndex: int, columnIndex: int) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveColumnAt(tableIndex, columnIndex)
            newAssay

    // - Column CRUD API - //
    static member removeColumn(tableName: string, columnIndex: int) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveColumn(tableName, columnIndex)
            newAssay

    // - Column CRUD API - //
    static member updateColumnAt(tableIndex: int, columnIndex: int, header: CompositeHeader, ?cells: CompositeCell []) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.UpdateColumnAt(tableIndex, columnIndex, header, ?cells=cells)
            newAssay

    // - Column CRUD API - //
    static member updateColumn(tableName: string, columnIndex: int, header: CompositeHeader, ?cells: CompositeCell []) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.UpdateColumn(tableName, columnIndex, header, ?cells=cells)
            newAssay

    // - Column CRUD API - //
    static member getColumnAt(tableIndex: int, columnIndex: int) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetColumnAt(tableIndex, columnIndex)

    // - Column CRUD API - //
    static member getColumn(tableName: string, columnIndex: int) =
        fun (study: ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetColumn(tableName, columnIndex)

    // - Row CRUD API - //
    static member addRowAt(tableIndex:int, ?cells: CompositeCell [], ?rowIndex: int) : ArcStudy -> ArcStudy = 
        fun (study: ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.AddRowAt(tableIndex, ?cells=cells, ?rowIndex=rowIndex)
            newAssay

    // - Row CRUD API - //
    static member addRow(tableName: string, ?cells: CompositeCell [], ?rowIndex: int) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.AddRow(tableName, ?cells=cells, ?rowIndex=rowIndex)
            newAssay

    // - Row CRUD API - //
    static member removeRowAt(tableIndex: int, rowIndex: int) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveColumnAt(tableIndex, rowIndex)
            newAssay

    // - Row CRUD API - //
    static member removeRow(tableName: string, rowIndex: int) : ArcStudy -> ArcStudy =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.RemoveRow(tableName, rowIndex)
            newAssay

    // - Row CRUD API - //
    static member updateRowAt(tableIndex: int, rowIndex: int, cells: CompositeCell []) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.UpdateRowAt(tableIndex, rowIndex, cells)
            newAssay

    // - Row CRUD API - //
    static member updateRow(tableName: string, rowIndex: int, cells: CompositeCell []) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.UpdateRow(tableName, rowIndex, cells)
            newAssay

    // - Row CRUD API - //
    static member getRowAt(tableIndex: int, rowIndex: int) =
        fun (study:ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetRowAt(tableIndex, rowIndex)

    // - Row CRUD API - //
    static member getRow(tableName: string, rowIndex: int) =
        fun (study: ArcStudy) ->
            let newAssay = study.Copy()
            newAssay.GetRow(tableName, rowIndex)

    member internal this.AddToInvestigation (investigation: ArcInvestigation) =
        this.Investigation <- Some investigation

    member internal this.RemoveFromInvestigation () =
        this.Investigation <- None

    /// Copies ArcStudy object without the pointer to the parent ArcInvestigation
    ///
    /// This copy does only contain the identifiers of the registered ArcAssays and not the actual objects.
    ///
    /// In order to copy the ArcAssays as well, use the Copy() method of the ArcInvestigation.
    member this.Copy(?copyInvestigationRef: bool) : ArcStudy =
        let copyInvestigationRef = defaultArg copyInvestigationRef false
        let nextTables = ResizeArray()
        let nextAssayIdentifiers = ResizeArray(this.RegisteredAssayIdentifiers)
        for table in this.Tables do
            let copy = table.Copy()
            nextTables.Add(copy)
        let nextComments = this.Comments |> ResizeArray.map (fun c -> c.Copy())
        let nextContacts = this.Contacts |> ResizeArray.map (fun c -> c.Copy())
        let nextPublications = this.Publications |> ResizeArray.map (fun c -> c.Copy())
        let nextStudyDesignDescriptors = this.StudyDesignDescriptors |> ResizeArray.map (fun c -> c.Copy())
        let nextDataMap = this.DataMap |> Option.map (fun d -> d.Copy())
        let study =
            ArcStudy.make
                this.Identifier
                this.Title
                this.Description
                this.SubmissionDate
                this.PublicReleaseDate
                nextPublications
                nextContacts
                nextStudyDesignDescriptors
                nextTables
                nextDataMap
                nextAssayIdentifiers
                nextComments
        if copyInvestigationRef then study.Investigation <- this.Investigation
        study

    /// <summary>
    /// Updates given study from an investigation file against a study from a study file. Identifier will never be updated. 
    /// </summary>
    /// <param name="study">The study used for updating this study.</param>
    /// <param name="onlyReplaceExisting">If true, this will only update fields which are `Some` or non-empty lists. Default: **false**</param>
    member this.UpdateReferenceByStudyFile(study:ArcStudy,?onlyReplaceExisting : bool,?keepUnusedRefTables) =
        let onlyReplaceExisting = defaultArg onlyReplaceExisting false
        let updateAlways = onlyReplaceExisting |> not
        if study.Title.IsSome || updateAlways then 
            this.Title <- study.Title
        if study.Description.IsSome || updateAlways then 
            this.Description <- study.Description
        if study.SubmissionDate.IsSome || updateAlways then 
            this.SubmissionDate <- study.SubmissionDate
        if study.PublicReleaseDate.IsSome || updateAlways then 
            this.PublicReleaseDate <- study.PublicReleaseDate
        if study.Publications.Count <> 0 || updateAlways then
            this.Publications <- study.Publications
        if study.Contacts.Count <> 0 || updateAlways then
            this.Contacts <- study.Contacts
        if study.StudyDesignDescriptors.Count <> 0 || updateAlways then
            this.StudyDesignDescriptors <- study.StudyDesignDescriptors
        if study.Tables.Count <> 0 || updateAlways then
            let tables = ArcTables.updateReferenceTablesBySheets(ArcTables(this.Tables),ArcTables(study.Tables),?keepUnusedRefTables = keepUnusedRefTables)
            this.Tables <- tables.Tables
        this.DataMap <- study.DataMap
        if study.RegisteredAssayIdentifiers.Count <> 0 || updateAlways then
            this.RegisteredAssayIdentifiers <- study.RegisteredAssayIdentifiers
        if study.Comments.Count <> 0 || updateAlways then
            this.Comments <- study.Comments

    member this.StructurallyEquals (other: ArcStudy) : bool =
        let i = this.Identifier = other.Identifier
        let t = this.Title = other.Title
        let d = this.Description = other.Description
        let sd = this.SubmissionDate = other.SubmissionDate
        let prd = this.PublicReleaseDate = other.PublicReleaseDate 
        let dm = this.DataMap = other.DataMap
        let pub = Seq.compare this.Publications other.Publications
        let con = Seq.compare this.Contacts other.Contacts
        let sdd = Seq.compare this.StudyDesignDescriptors other.StudyDesignDescriptors
        let tables = Seq.compare this.Tables other.Tables
        let reg_tables = Seq.compare this.RegisteredAssayIdentifiers other.RegisteredAssayIdentifiers
        let comments = Seq.compare this.Comments other.Comments
        // Todo maybe add reflection check to prove that all members are compared?
        [|i; t; d; sd; prd; dm; pub; con; sdd; tables; reg_tables; comments|] |> Seq.forall (fun x -> x = true)

    /// <summary>
    /// Use this function to check if this ArcStudy and the input ArcStudy refer to the same object.
    ///
    /// If true, updating one will update the other due to mutability.
    /// </summary>
    /// <param name="other">The other ArcStudy to test for reference.</param>
    member this.ReferenceEquals (other: ArcStudy) = System.Object.ReferenceEquals(this,other)

    // Use this for better print debugging and better unit test output
    override this.ToString() =
        sprintf 
            """ArcStudy {
    Identifier = %A,
    Title = %A,
    Description = %A,
    SubmissionDate = %A,
    PublicReleaseDate = %A,
    Publications = %A,
    Contacts = %A,
    StudyDesignDescriptors = %A,
    Tables = %A,
    RegisteredAssayIdentifiers = %A,
    Comments = %A,
}"""
            this.Identifier
            this.Title
            this.Description
            this.SubmissionDate
            this.PublicReleaseDate
            this.Publications
            this.Contacts
            this.StudyDesignDescriptors
            this.Tables
            this.RegisteredAssayIdentifiers
            this.Comments

    // custom check
    override this.Equals other =
        match other with
        | :? ArcStudy as s -> 
            this.StructurallyEquals(s)
        | _ -> false

    override this.GetHashCode() = 
        [|
            box this.Identifier
            HashCodes.boxHashOption this.Title
            HashCodes.boxHashOption this.Description
            HashCodes.boxHashOption this.SubmissionDate
            HashCodes.boxHashOption this.PublicReleaseDate
            HashCodes.boxHashOption this.DataMap
            HashCodes.boxHashSeq this.Publications
            HashCodes.boxHashSeq this.Contacts
            HashCodes.boxHashSeq this.StudyDesignDescriptors
            HashCodes.boxHashSeq this.Tables
            HashCodes.boxHashSeq this.RegisteredAssayIdentifiers
            HashCodes.boxHashSeq this.Comments
        |]
        |> HashCodes.boxHashArray 
        |> fun x -> x :?> int

    member this.GetLightHashCode() = 
        [|
            box this.Identifier
            HashCodes.boxHashOption this.Title
            HashCodes.boxHashOption this.Description
            HashCodes.boxHashOption this.SubmissionDate
            HashCodes.boxHashOption this.PublicReleaseDate
            HashCodes.boxHashSeq this.Publications
            HashCodes.boxHashSeq this.Contacts
            HashCodes.boxHashSeq this.StudyDesignDescriptors
            HashCodes.boxHashSeq this.Tables
            HashCodes.boxHashSeq this.RegisteredAssayIdentifiers
            HashCodes.boxHashSeq this.Comments
        |]
        |> HashCodes.boxHashArray 
        |> fun x -> x :?> int


type ArcWorkflow(identifier : string, ?title : string, ?description : string, ?workflowType : OntologyAnnotation, ?uri : string, ?version : string, ?subWorkflowIdentifiers : ResizeArray<string>, ?parameters : ResizeArray<Process.ProtocolParameter>, ?components : ResizeArray<Process.Component>, ?datamap : DataMap, ?contacts : ResizeArray<Person>, ?comments : ResizeArray<Comment>) =

    let mutable identifier = identifier
    let mutable investigation : ArcInvestigation option = None
    let mutable title = title
    let mutable description = description
    let mutable subWorkflowIdentifiers = defaultArg subWorkflowIdentifiers (ResizeArray())
    let mutable workflowType = workflowType
    let mutable uri = uri
    let mutable version = version
    let mutable parameters = defaultArg parameters (ResizeArray())
    let mutable components = defaultArg components (ResizeArray())
    let mutable dataMap : DataMap option = datamap
    let mutable contacts = defaultArg contacts (ResizeArray())
    let mutable comments  = defaultArg comments (ResizeArray())
    let mutable staticHash : int = 0

    /// Must be unique in one study
    member this.Identifier with get() = identifier and internal set(i) = identifier <- i
    // read-online
    member this.Investigation with get() = investigation and internal set(a) = investigation <- a
    member this.Title with get() = title and set(t) = title <- t
    member this.Description with get() = description and set(d) = description <- d
    member this.SubWorkflowIdentifiers with get() = subWorkflowIdentifiers and set(s) = subWorkflowIdentifiers <- s
    member this.WorkflowType with get() = workflowType and set(w) = workflowType <- w
    member this.URI with get() = uri and set(u) = uri <- u
    member this.Version with get() = version and set(v) = version <- v
    member this.Parameters with get() = parameters and set(p) = parameters <- p
    member this.Components with get() = components and set(c) = components <- c
    member this.DataMap with get() = dataMap and set(dm) = dataMap <- dm
    member this.Contacts with get() = contacts and set(c) = contacts <- c
    member this.Comments with get() = comments and set(c) = comments <- c
    member this.StaticHash with get() = staticHash and set(s) = staticHash <- s

    static member init(identifier : string) = ArcWorkflow(identifier = identifier)
    static member create(identifier : string, ?title : string, ?description : string, ?workflowType : OntologyAnnotation, ?uri : string, ?version : string, ?subWorkflowIdentifiers : ResizeArray<string>, ?parameters : ResizeArray<Process.ProtocolParameter>, ?components : ResizeArray<Process.Component>, ?datamap : DataMap, ?contacts : ResizeArray<Person>, ?comments : ResizeArray<Comment>) = 
        ArcWorkflow(identifier = identifier, ?title = title, ?description = description, ?subWorkflowIdentifiers = subWorkflowIdentifiers, ?workflowType = workflowType, ?uri = uri, ?version = version, ?parameters = parameters, ?components = components, ?datamap = datamap, ?contacts = contacts, ?comments = comments)

    static member make (identifier : string) (title : string option) (description : string option) (workflowType : OntologyAnnotation option) (uri : string option) (version : string option) (subWorkflowIdentifiers : ResizeArray<string>) (parameters : ResizeArray<Process.ProtocolParameter>) (components : ResizeArray<Process.Component>) (datamap : DataMap option) (contacts : ResizeArray<Person>) (comments : ResizeArray<Comment>) =
        ArcWorkflow(identifier = identifier, ?title = title, ?description = description, subWorkflowIdentifiers = subWorkflowIdentifiers, ?workflowType = workflowType, ?uri = uri, ?version = version, parameters = parameters, components = components, ?datamap = datamap, contacts = contacts, comments = comments)

    static member FileName = ARCtrl.ArcPathHelper.RunFileName

    /// Returns the count of registered subWorkflow *identifiers*. This is not necessarily the same as the count of registered subWorkflows, as not all identifiers correspond to an existing subWorkflow.
    member this.SubWorkflowIdentifiersCount 
        with get() = this.SubWorkflowIdentifiers.Count

    /// Returns the count of registered subWorkflows. This is not necessarily the same as the count of registered subWorkflow *identifiers*, as not all identifiers correspond to an existing subWorkflow.
    member this.SubWorkflowCount 
        with get() = this.SubWorkflows.Count

    /// Returns all subWorkflows registered in this workflow, that correspond to an existing subWorkflow object in the associated investigation.
    member this.SubWorkflows
        with get(): ResizeArray<ArcWorkflow> = 
            let inv = ArcTypesAux.SanityChecks.validateRegisteredInvestigation this.Investigation
            this.SubWorkflowIdentifiers 
            |> Seq.choose inv.TryGetWorkflow
            |> ResizeArray

    /// Returns all registered subWorkflow identifiers that do not correspond to an existing subWorkflow object in the associated investigation.
    member this.VacantSubWorkflowIdentifiers
        with get() = 
            let inv = ArcTypesAux.SanityChecks.validateRegisteredInvestigation this.Investigation
            this.SubWorkflowIdentifiers 
            |> Seq.filter (inv.ContainsWorkflow >> not)
            |> ResizeArray

    // - SubWorkflow API - CRUD //
    /// <summary>
    /// Add subWorkflow to investigation and register it to workflow.
    /// </summary>
    /// <param name="subWorkflow"></param>
    member this.AddSubWorkflow(subWorkflow: ArcWorkflow) =
        let inv = ArcTypesAux.SanityChecks.validateRegisteredInvestigation this.Investigation 
        inv.AddWorkflow(subWorkflow)

    static member addSubWorkflow(subWorkflow: ArcWorkflow) =
        fun (workflow:ArcWorkflow) ->
            let newWorkflow = workflow.Copy()
            newWorkflow.AddSubWorkflow(subWorkflow)
            newWorkflow

    // - SubWorkflow API - CRUD //
    member this.InitSubWorkflow(subWorkflowIdentifier: string) =
        let subWorkflow = ArcWorkflow(subWorkflowIdentifier)
        this.AddSubWorkflow(subWorkflow)
        subWorkflow

    static member initSubWorkflow(subWorkflowIdentifier: string) =
        fun (workflow:ArcWorkflow) ->
            let copy = workflow.Copy()
            copy,copy.InitSubWorkflow(subWorkflowIdentifier)

    // - SubWorkflow API - CRUD //
    member this.RegisterSubWorkflow(subWorkflowIdentifier: string) =
        if Seq.contains subWorkflowIdentifier this.SubWorkflowIdentifiers then failwith $"SubWorkflow `{subWorkflowIdentifier}` is already registered on the workflow."
        this.SubWorkflowIdentifiers.Add(subWorkflowIdentifier)

    static member registerSubWorkflow(subWorkflowIdentifier: string) =
        fun (workflow: ArcWorkflow) ->
            let copy = workflow.Copy()
            copy.RegisterSubWorkflow(subWorkflowIdentifier)
            copy

    // - SubWorkflow API - CRUD //
    member this.DeregisterSubWorkflow(subWorkflowIdentifier: string) =
        this.SubWorkflowIdentifiers.Remove(subWorkflowIdentifier) |> ignore

    static member deregisterSubWorkflow(subWorkflowIdentifier: string) =
        fun (workflow: ArcWorkflow) ->
            let copy = workflow.Copy()
            copy.DeregisterSubWorkflow(subWorkflowIdentifier)
            copy

    // - SubWorkflow API - CRUD //
    member this.GetRegisteredSubWorkflow(subWorkflowIdentifier: string) =
        if Seq.contains subWorkflowIdentifier this.SubWorkflowIdentifiers |> not then failwith $"SubWorkflow `{subWorkflowIdentifier}` is not registered on the workflow."
        let inv = ArcTypesAux.SanityChecks.validateRegisteredInvestigation this.Investigation
        inv.GetWorkflow(subWorkflowIdentifier)

    static member getRegisteredSubWorkflow(subWorkflowIdentifier: string) =
        fun (workflow: ArcWorkflow) ->
            let copy = workflow.Copy()
            copy.GetRegisteredSubWorkflow(subWorkflowIdentifier)

    // - SubWorkflow API - CRUD //
    static member getRegisteredSubWorkflows() =
        fun (workflow: ArcWorkflow) ->
            let copy = workflow.Copy()
            copy.SubWorkflows

    /// <summary>
    /// Returns ArcSubWorkflows registered in workflow, or if no parent exists, initializies new ArcSubWorkflow from identifier.
    /// </summary>
    member this.GetRegisteredSubWorkflowsOrIdentifier() = 
        // Two Options:
        // 1. Init new subWorkflows with only identifier. This is possible without ArcInvestigation parent.
        // 2. Get full subWorkflows from ArcInvestigation parent.
        match this.Investigation with
        | Some i -> 
            this.SubWorkflowIdentifiers
            |> ResizeArray.map (fun identifier -> 
                match i.TryGetWorkflow(identifier) with
                | Some subWorkflow -> subWorkflow
                | None -> ArcWorkflow.init(identifier)
            )
        | None ->
            this.SubWorkflowIdentifiers 
            |> ResizeArray.map (fun identifier -> ArcWorkflow.init(identifier))


    /// <summary>
    /// Returns ArcSubWorkflows registered in workflow, or if no parent exists, initializies new ArcSubWorkflow from identifier.
    /// </summary>
    static member getRegisteredSubWorkflowsOrIdentifier() =
        fun (workflow: ArcWorkflow) ->
            let copy = workflow.Copy()
            copy.GetRegisteredSubWorkflowsOrIdentifier()

    /// Copies ArcStudy object without the pointer to the parent ArcInvestigation
    ///
    /// This copy does only contain the identifiers of the registered ArcAssays and not the actual objects.
    ///
    /// In order to copy the ArcAssays as well, use the Copy() method of the ArcInvestigation.
    member this.Copy(?copyInvestigationRef: bool) : ArcWorkflow =
        let copyInvestigationRef = defaultArg copyInvestigationRef false
        let nextParameters = this.Parameters // |> ResizeArray.map (fun p -> p.Copy())
        let nextComponents = this.Components // |> ResizeArray.map (fun c -> c.Copy())
        let nextContacts = this.Contacts |> ResizeArray.map (fun c -> c.Copy())
        let nextComments = this.Comments |> ResizeArray.map (fun c -> c.Copy())
        let workflow =
            ArcWorkflow.make
                this.Identifier
                this.Title
                this.Description
                this.WorkflowType
                this.URI
                this.Version
                this.SubWorkflowIdentifiers
                nextParameters
                nextComponents
                this.DataMap
                nextContacts
                nextComments
        if copyInvestigationRef then workflow.Investigation <- this.Investigation
        workflow

    member this.StructurallyEquals (other: ArcWorkflow) : bool =
        let i = this.Identifier = other.Identifier
        let t = this.Title = other.Title
        let d = this.Description = other.Description
        let wft = this.WorkflowType = other.WorkflowType
        let uri = this.URI = other.URI
        let ver = this.Version = other.Version
        let subwf = Seq.compare this.SubWorkflowIdentifiers other.SubWorkflowIdentifiers
        let par = Seq.compare this.Parameters other.Parameters
        let com = Seq.compare this.Components other.Components
        let dm = this.DataMap = other.DataMap
        let con = Seq.compare this.Contacts other.Contacts
        let comments = Seq.compare this.Comments other.Comments
        // Todo maybe add reflection check to prove that all members are compared?
        [|i; t; d; wft; uri; ver; subwf; par; com; dm; con; comments|] |> Seq.forall (fun x -> x = true)


    /// <summary>
    /// Use this function to check if this ArcWorkflow and the input ArcWorkflow refer to the same object.
    ///
    /// If true, updating one will update the other due to mutability.
    /// </summary>
    /// <param name="other">The other ArcWorkflow to test for reference.</param>
    member this.ReferenceEquals (other: ArcStudy) = System.Object.ReferenceEquals(this,other)

    // Use this for better print debugging and better unit test output
    override this.ToString() =
        sprintf 
            """ArcWorkflow {
    Identifier = %A,
    Title = %A,
    Description = %A,
    WorkflowType = %A,
    URI = %A,
    Version = %A,
    SubWorkflowIdentifiers = %A,
    Parameters = %A,
    Components = %A,
    DataMap = %A,
    Contacts = %A,
    Comments = %A}"""
            this.Identifier
            this.Title
            this.Description
            this.WorkflowType
            this.URI
            this.Version
            this.SubWorkflowIdentifiers
            this.Parameters
            this.Components
            this.DataMap
            this.Contacts
            this.Comments

    // custom check
    override this.Equals other =
        match other with
        | :? ArcWorkflow as s -> 
            this.StructurallyEquals(s)
        | _ -> false

    override this.GetHashCode() = 
        [|
            box this.Identifier
            HashCodes.boxHashOption this.Title
            HashCodes.boxHashOption this.Description
            HashCodes.boxHashOption this.WorkflowType
            HashCodes.boxHashOption this.URI
            HashCodes.boxHashOption this.Version
            HashCodes.boxHashSeq this.SubWorkflowIdentifiers
            HashCodes.boxHashSeq this.Parameters
            HashCodes.boxHashSeq this.Components
            HashCodes.boxHashOption this.DataMap
            HashCodes.boxHashSeq this.Contacts
            HashCodes.boxHashSeq this.Comments
        |]
        |> HashCodes.boxHashArray
        |> fun x -> x :?> int

    member this.GetLightHashCode() = 
        [|
            box this.Identifier
            HashCodes.boxHashOption this.Title
            HashCodes.boxHashOption this.Description
            HashCodes.boxHashOption this.WorkflowType
            HashCodes.boxHashOption this.URI
            HashCodes.boxHashOption this.Version
            HashCodes.boxHashSeq this.SubWorkflowIdentifiers
            HashCodes.boxHashSeq this.Parameters
            HashCodes.boxHashSeq this.Components
            HashCodes.boxHashOption this.DataMap
            HashCodes.boxHashSeq this.Contacts
            HashCodes.boxHashSeq this.Comments
        |]
        |> HashCodes.boxHashArray
        |> fun x -> x :?> int



[<AttachMembers>]
type ArcRun(identifier: string, ?title : string, ?description : string, ?measurementType : OntologyAnnotation, ?technologyType : OntologyAnnotation, ?technologyPlatform : OntologyAnnotation, ?tables: ResizeArray<ArcTable>, ?datamap : DataMap, ?performers : ResizeArray<Person>, ?comments : ResizeArray<Comment>) = 
    inherit ArcTables(defaultArg tables <| ResizeArray())

    let performers = defaultArg performers <| ResizeArray()
    let comments = defaultArg comments <| ResizeArray()
    let mutable identifier : string =
        let identifier = identifier.Trim()
        Helper.Identifier.checkValidCharacters identifier
        identifier
    let mutable title : string option = title
    let mutable description : string option = description
    let mutable investigation : ArcInvestigation option = None
    let mutable measurementType : OntologyAnnotation option = measurementType
    let mutable technologyType : OntologyAnnotation option = technologyType
    let mutable technologyPlatform : OntologyAnnotation option = technologyPlatform
    let mutable dataMap : DataMap option = datamap
    let mutable performers = performers
    let mutable comments  = comments
    let mutable staticHash : int = 0

    /// Must be unique in one study
    member this.Identifier with get() = identifier and internal set(i) = identifier <- i
    // read-online
    member this.Investigation with get() = investigation and internal set(i) = investigation <- i
    member this.Title with get() = title and set(t) = title <- t
    member this.Description with get() = description and set(d) = description <- d
    member this.MeasurementType with get() = measurementType and set(n) = measurementType <- n
    member this.TechnologyType with get() = technologyType and set(n) = technologyType <- n
    member this.TechnologyPlatform with get() = technologyPlatform and set(n) = technologyPlatform <- n
    member this.DataMap with get() = dataMap and set(n) = dataMap <- n
    member this.Performers with get() = performers and set(n) = performers <- n
    member this.Comments with get() = comments and set(n) = comments <- n
    member this.StaticHash with get() = staticHash and set(h) = staticHash <- h

    static member init (identifier : string) = ArcRun(identifier)
    static member create (identifier: string, ?title : string, ?description : string, ?measurementType : OntologyAnnotation, ?technologyType : OntologyAnnotation, ?technologyPlatform : OntologyAnnotation, ?tables: ResizeArray<ArcTable>, ?datamap : DataMap, ?performers : ResizeArray<Person>, ?comments : ResizeArray<Comment>) = 
        ArcRun(identifier = identifier, ?title = title, ?description = description, ?measurementType = measurementType, ?technologyType = technologyType, ?technologyPlatform = technologyPlatform, ?tables =tables, ?datamap = datamap, ?performers = performers, ?comments = comments)

    static member make 
        (identifier : string)
        (title : string option)
        (description : string option)
        (measurementType : OntologyAnnotation option)
        (technologyType : OntologyAnnotation option)
        (technologyPlatform : OntologyAnnotation option)
        (tables : ResizeArray<ArcTable>)
        (datamap : DataMap option)
        (performers : ResizeArray<Person>)
        (comments : ResizeArray<Comment>) = 
        ArcRun(identifier = identifier, ?title = title, ?description = description, ?measurementType = measurementType, ?technologyType = technologyType, ?technologyPlatform = technologyPlatform, tables =tables, ?datamap = datamap, performers = performers, comments = comments)

    static member FileName = ARCtrl.ArcPathHelper.RunFileName
        
    // - Table API - //
    static member addTable(table:ArcTable, ?index: int) =
        fun (run:ArcRun) ->
            let c = run.Copy()
            c.AddTable(table, ?index = index)
            c

    // - Table API - //
    static member addTables(tables:seq<ArcTable>, ?index: int) =
        fun (run:ArcRun) ->
            let c = run.Copy()
            c.AddTables(tables, ?index = index)
            c

    // - Table API - //
    static member initTable(tableName: string, ?index: int) =
        fun (run:ArcRun) ->
            let c = run.Copy()
            c,c.InitTable(tableName, ?index=index)
            
    // - Table API - //
    static member initTables(tableNames:seq<string>, ?index: int) =
        fun (run:ArcRun) ->
            let c = run.Copy()
            c.InitTables(tableNames, ?index=index)
            c

    // - Table API - //
    /// Receive **copy** of table at `index`
    static member getTableAt(index:int) : ArcRun -> ArcTable =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.GetTableAt(index)

    // - Table API - //
    /// Receive **copy** of table with `name` = `ArcTable.Name`
    static member getTable(name: string) : ArcRun -> ArcTable =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.GetTable(name)

    // - Table API - //
    static member updateTableAt(index:int, table:ArcTable) : ArcRun -> ArcRun =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.UpdateTableAt(index, table)
            newRun

    // - Table API - //
    static member updateTable(name: string, table:ArcTable) : ArcRun -> ArcRun =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.UpdateTable(name, table)
            newRun

    // - Table API - //
    static member setTableAt(index:int, table:ArcTable) : ArcRun -> ArcRun =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.SetTableAt(index, table)
            newRun

    // - Table API - //
    static member setTable(name: string, table:ArcTable) : ArcRun -> ArcRun =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.SetTable(name, table)
            newRun

    // - Table API - //
    static member removeTableAt(index:int) : ArcRun -> ArcRun =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.RemoveTableAt(index)
            newRun

    // - Table API - //
    static member removeTable(name: string) : ArcRun -> ArcRun =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.RemoveTable(name)
            newRun

    // - Table API - //
    static member mapTableAt(index:int, updateFun: ArcTable -> unit) =
        fun (run:ArcRun) ->
            let newRun = run.Copy()    
            newRun.MapTableAt(index, updateFun)
            newRun

    // - Table API - //
    static member updateTable(name: string, updateFun: ArcTable -> unit) : ArcRun -> ArcRun =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.MapTable(name, updateFun)
            newRun

    // - Table API - //
    static member renameTableAt(index: int, newName: string) : ArcRun -> ArcRun =
        fun (run:ArcRun) ->
            let newRun = run.Copy()    
            newRun.RenameTableAt(index, newName)
            newRun

    // - Table API - //
    static member renameTable(name: string, newName: string) : ArcRun -> ArcRun =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.RenameTable(name, newName)
            newRun

    // - Column CRUD API - //
    static member addColumnAt(tableIndex:int, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) : ArcRun -> ArcRun = 
        fun (run: ArcRun) ->
            let newRun = run.Copy()
            newRun.AddColumnAt(tableIndex, header, ?cells=cells, ?columnIndex=columnIndex, ?forceReplace=forceReplace)
            newRun

    // - Column CRUD API - //
    static member addColumn(tableName: string, header: CompositeHeader, ?cells: CompositeCell [], ?columnIndex: int, ?forceReplace: bool) : ArcRun -> ArcRun =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.AddColumn(tableName, header, ?cells=cells, ?columnIndex=columnIndex, ?forceReplace=forceReplace)
            newRun

    // - Column CRUD API - //
    static member removeColumnAt(tableIndex: int, columnIndex: int) =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.RemoveColumnAt(tableIndex, columnIndex)
            newRun

    // - Column CRUD API - //
    static member removeColumn(tableName: string, columnIndex: int) : ArcRun -> ArcRun =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.RemoveColumn(tableName, columnIndex)
            newRun

    // - Column CRUD API - //
    static member updateColumnAt(tableIndex: int, columnIndex: int, header: CompositeHeader, ?cells: CompositeCell []) =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.UpdateColumnAt(tableIndex, columnIndex, header, ?cells=cells)
            newRun

    // - Column CRUD API - //
    static member updateColumn(tableName: string, columnIndex: int, header: CompositeHeader, ?cells: CompositeCell []) =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.UpdateColumn(tableName, columnIndex, header, ?cells=cells)
            newRun

    // - Column CRUD API - //
    static member getColumnAt(tableIndex: int, columnIndex: int) =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.GetColumnAt(tableIndex, columnIndex)

    // - Column CRUD API - //
    static member getColumn(tableName: string, columnIndex: int) =
        fun (run: ArcRun) ->
            let newRun = run.Copy()
            newRun.GetColumn(tableName, columnIndex)

    // - Row CRUD API - //
    static member addRowAt(tableIndex:int, ?cells: CompositeCell [], ?rowIndex: int) : ArcRun -> ArcRun = 
        fun (run: ArcRun) ->
            let newRun = run.Copy()
            newRun.AddRowAt(tableIndex, ?cells=cells, ?rowIndex=rowIndex)
            newRun

    // - Row CRUD API - //
    static member addRow(tableName: string, ?cells: CompositeCell [], ?rowIndex: int) : ArcRun -> ArcRun =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.AddRow(tableName, ?cells=cells, ?rowIndex=rowIndex)
            newRun

    // - Row CRUD API - //
    static member removeRowAt(tableIndex: int, rowIndex: int) =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.RemoveColumnAt(tableIndex, rowIndex)
            newRun

    // - Row CRUD API - //
    static member removeRow(tableName: string, rowIndex: int) : ArcRun -> ArcRun =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.RemoveRow(tableName, rowIndex)
            newRun

    // - Row CRUD API - //
    static member updateRowAt(tableIndex: int, rowIndex: int, cells: CompositeCell []) =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.UpdateRowAt(tableIndex, rowIndex, cells)
            newRun

    // - Row CRUD API - //
    static member updateRow(tableName: string, rowIndex: int, cells: CompositeCell []) =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.UpdateRow(tableName, rowIndex, cells)
            newRun

    // - Row CRUD API - //
    static member getRowAt(tableIndex: int, rowIndex: int) =
        fun (run:ArcRun) ->
            let newRun = run.Copy()
            newRun.GetRowAt(tableIndex, rowIndex)

    // - Row CRUD API - //
    static member getRow(tableName: string, rowIndex: int) =
        fun (run: ArcRun) ->
            let newRun = run.Copy()
            newRun.GetRow(tableName, rowIndex)

    // - Mutable properties API - //
    static member setPerformers performers (run: ArcRun) =
        run.Performers <- performers
        run

    member this.Copy() : ArcRun =
        let nextTables = this.Tables |> ResizeArray.map (fun c -> c.Copy())
        let nextComments = this.Comments |> ResizeArray.map (fun c -> c.Copy())
        let nextDataMap = this.DataMap |> Option.map (fun d -> d.Copy())
        let nextPerformers = this.Performers |> ResizeArray.map (fun c -> c.Copy())
        ArcRun.make
            this.Identifier
            this.Title
            this.Description
            this.MeasurementType
            this.TechnologyType
            this.TechnologyPlatform
            nextTables
            nextDataMap
            nextPerformers
            nextComments

    /// <summary>
    /// Updates given run with another run, Identifier will never be updated. By default update is full replace. Optional Parameters can be used to specify update logic.
    /// </summary>
    /// <param name="run">The run used for updating this run.</param>
    /// <param name="onlyReplaceExisting">If true, this will only update fields which are `Some` or non-empty lists. Default: **false**</param>
    /// <param name="appendSequences">If true, this will append lists instead of replacing. Will return only distinct elements. Default: **false**</param>
    member this.UpdateBy(run:ArcRun,?onlyReplaceExisting : bool,?appendSequences : bool) =
        let onlyReplaceExisting = defaultArg onlyReplaceExisting false
        let appendSequences = defaultArg appendSequences false
        let updateAlways = onlyReplaceExisting |> not
        if run.Title.IsSome || updateAlways then
            this.Title <- run.Title
        if run.Description.IsSome || updateAlways then
            this.Description <- run.Description
        if run.MeasurementType.IsSome || updateAlways then 
            this.MeasurementType <- run.MeasurementType
        if run.TechnologyType.IsSome || updateAlways then 
            this.TechnologyType <- run.TechnologyType
        if run.TechnologyPlatform.IsSome || updateAlways then 
            this.TechnologyPlatform <- run.TechnologyPlatform
        if run.Tables.Count <> 0 || updateAlways then
            let s = ArcTypesAux.updateAppendResizeArray appendSequences this.Tables run.Tables
            this.Tables <- s
        if run.Performers.Count <> 0 || updateAlways then
            let s = ArcTypesAux.updateAppendResizeArray appendSequences this.Performers run.Performers
            this.Performers <- s
        if run.Comments.Count <> 0 || updateAlways then
            let s = ArcTypesAux.updateAppendResizeArray appendSequences this.Comments run.Comments
            this.Comments <- s

    // Use this for better print debugging and better unit test output
    override this.ToString() =
        sprintf 
            """ArcRun({
    Identifier = "%s",
    Title = %A,
    Description = %A,
    MeasurementType = %A,
    TechnologyType = %A,
    TechnologyPlatform = %A,
    Tables = %A,
    Performers = %A,
    Comments = %A
})"""
            this.Identifier
            this.Title
            this.Description
            this.MeasurementType
            this.TechnologyType
            this.TechnologyPlatform
            this.Tables
            this.Performers
            this.Comments

    member internal this.AddToInvestigation (investigation: ArcInvestigation) =
        this.Investigation <- Some investigation

    member internal this.RemoveFromInvestigation () =
        this.Investigation <- None

    member this.StructurallyEquals (other: ArcRun) : bool =
        let i = this.Identifier = other.Identifier
        let t = this.Title = other.Title
        let d = this.Description = other.Description
        let mst = this.MeasurementType = other.MeasurementType
        let tt = this.TechnologyType = other.TechnologyType
        let tp = this.TechnologyPlatform = other.TechnologyPlatform
        let dm = this.DataMap = other.DataMap
        let tables = Seq.compare this.Tables other.Tables
        let perf = Seq.compare this.Performers other.Performers
        let comments = Seq.compare this.Comments other.Comments
        // Todo maybe add reflection check to prove that all members are compared?
        [|i; t; d; mst; tt; tp; dm; tables; perf; comments|] |> Seq.forall (fun x -> x = true)

    /// <summary>
    /// Use this function to check if this ArcRun and the input ArcRun refer to the same object.
    ///
    /// If true, updating one will update the other due to mutability.
    /// </summary>
    /// <param name="other">The other ArcRun to test for reference.</param>
    member this.ReferenceEquals (other: ArcRun) = System.Object.ReferenceEquals(this,other)

    // custom check
    override this.Equals other =
        match other with
        | :? ArcRun as run -> 
            this.StructurallyEquals(run)
        | _ -> false

    // Hashcode without Datamap
    member this.GetLightHashCode() = 
        [|
            box this.Identifier
            HashCodes.boxHashOption this.Title
            HashCodes.boxHashOption this.Description
            HashCodes.boxHashOption this.MeasurementType
            HashCodes.boxHashOption this.TechnologyType
            HashCodes.boxHashOption this.TechnologyPlatform
            HashCodes.boxHashSeq this.Tables
            HashCodes.boxHashSeq this.Performers
            HashCodes.boxHashSeq this.Comments
        |]
        |> HashCodes.boxHashArray 
        |> fun x -> x :?> int

    override this.GetHashCode() = 
        [|
            box this.Identifier
            HashCodes.boxHashOption this.Title
            HashCodes.boxHashOption this.Description
            HashCodes.boxHashOption this.MeasurementType
            HashCodes.boxHashOption this.TechnologyType
            HashCodes.boxHashOption this.TechnologyPlatform
            HashCodes.boxHashOption this.DataMap
            HashCodes.boxHashSeq this.Tables
            HashCodes.boxHashSeq this.Performers
            HashCodes.boxHashSeq this.Comments
        |]
        |> HashCodes.boxHashArray 
        |> fun x -> x :?> int


[<AttachMembers>]
type ArcInvestigation(identifier : string, ?title : string, ?description : string, ?submissionDate : string, ?publicReleaseDate : string, ?ontologySourceReferences, ?publications, ?contacts, ?assays : ResizeArray<ArcAssay>, ?studies : ResizeArray<ArcStudy>, ?workflows : ResizeArray<ArcWorkflow>, ?runs : ResizeArray<ArcRun>, ?registeredStudyIdentifiers : ResizeArray<string>, ?comments : ResizeArray<Comment>, ?remarks) as this = 

    let ontologySourceReferences = defaultArg ontologySourceReferences <| ResizeArray()
    let publications = defaultArg publications <| ResizeArray()
    let contacts = defaultArg contacts <| ResizeArray()
    let assays = 
        let ass = defaultArg assays (ResizeArray())
        for a in ass do 
            a.Investigation <- Some this
        ass
    let studies = 
        let sss = defaultArg studies (ResizeArray())
        for s in sss do 
            s.Investigation <- Some this
        sss
    let workflows =
        let wss = defaultArg workflows (ResizeArray())
        for w in wss do
            w.Investigation <- Some this
        wss
    let runs =
        let rss = defaultArg runs (ResizeArray())
        for r in rss do
            r.Investigation <- Some this
        rss
    let registeredStudyIdentifiers = defaultArg registeredStudyIdentifiers <| ResizeArray()
    let comments = defaultArg comments <| ResizeArray()
    let remarks = defaultArg remarks <| ResizeArray()

    let mutable identifier = identifier
    let mutable title : string option = title
    let mutable description : string option = description
    let mutable submissionDate : string option = submissionDate
    let mutable publicReleaseDate : string option = publicReleaseDate
    let mutable ontologySourceReferences : ResizeArray<OntologySourceReference> = ontologySourceReferences
    let mutable publications : ResizeArray<Publication> = publications
    let mutable contacts : ResizeArray<Person> = contacts
    let mutable assays : ResizeArray<ArcAssay> = assays
    let mutable studies : ResizeArray<ArcStudy> = studies
    let mutable workflows : ResizeArray<ArcWorkflow> = workflows
    let mutable runs : ResizeArray<ArcRun> = runs
    let mutable registeredStudyIdentifiers : ResizeArray<string> = registeredStudyIdentifiers
    let mutable comments : ResizeArray<Comment> = comments
    let mutable remarks : ResizeArray<Remark> = remarks
    let mutable staticHash : int = 0

    /// Must be unique in one investigation
    member this.Identifier with get() = identifier and internal set(i) = identifier <- i
    member this.Title with get() = title and set(n) = title <- n
    member this.Description with get() = description and set(n) = description <- n
    member this.SubmissionDate with get() = submissionDate and set(n) = submissionDate <- n
    member this.PublicReleaseDate with get() = publicReleaseDate and set(n) = publicReleaseDate <- n
    member this.OntologySourceReferences with get() = ontologySourceReferences and set(n) = ontologySourceReferences <- n
    member this.Publications with get() = publications and set(n) = publications <- n
    member this.Contacts with get() = contacts and set(n) = contacts <- n
    member this.Assays with get() : ResizeArray<ArcAssay> = assays and set(n) = assays <- n
    member this.Studies with get() : ResizeArray<ArcStudy> = studies and set(n) = studies <- n
    member this.Workflows with get() : ResizeArray<ArcWorkflow> = workflows and set(n) = workflows <- n
    member this.Runs with get() : ResizeArray<ArcRun> = runs and set(n) = runs <- n
    member this.RegisteredStudyIdentifiers with get() = registeredStudyIdentifiers and set(n) = registeredStudyIdentifiers <- n
    member this.Comments with get() = comments and set(n) = comments <- n
    member this.Remarks with get() = remarks and set(n) = remarks <- n
    member this.StaticHash with get() = staticHash and set(h) = staticHash <- h

    static member FileName = ARCtrl.ArcPathHelper.InvestigationFileName

    static member init(identifier: string) = ArcInvestigation identifier
    static member create(identifier : string, ?title : string, ?description : string, ?submissionDate : string, ?publicReleaseDate : string, ?ontologySourceReferences, ?publications, ?contacts, ?assays : ResizeArray<ArcAssay>, ?studies : ResizeArray<ArcStudy>, ?workflows : ResizeArray<ArcWorkflow>, ?runs : ResizeArray<ArcRun>, ?registeredStudyIdentifiers : ResizeArray<string>, ?comments : ResizeArray<Comment>, ?remarks) = 
        ArcInvestigation(identifier, ?title = title, ?description = description, ?submissionDate = submissionDate, ?publicReleaseDate = publicReleaseDate, ?ontologySourceReferences = ontologySourceReferences, ?publications = publications, ?contacts = contacts, ?assays = assays, ?studies = studies, ?workflows = workflows, ?runs = runs, ?registeredStudyIdentifiers = registeredStudyIdentifiers, ?comments = comments, ?remarks = remarks)

    static member make (identifier : string) (title : string option) (description : string option) (submissionDate : string option) (publicReleaseDate : string option) (ontologySourceReferences) (publications) (contacts) (assays: ResizeArray<ArcAssay>) (studies : ResizeArray<ArcStudy>) (workflows : ResizeArray<ArcWorkflow>) (runs : ResizeArray<ArcRun>) (registeredStudyIdentifiers : ResizeArray<string>) (comments : ResizeArray<Comment>) (remarks) : ArcInvestigation =
        ArcInvestigation(identifier, ?title = title, ?description = description, ?submissionDate = submissionDate, ?publicReleaseDate = publicReleaseDate, ontologySourceReferences = ontologySourceReferences, publications = publications, contacts = contacts, assays = assays, studies = studies, workflows = workflows, runs = runs, registeredStudyIdentifiers = registeredStudyIdentifiers, comments = comments, remarks = remarks)

    member this.AssayCount 
        with get() = this.Assays.Count

    member this.AssayIdentifiers 
        with get(): string [] = this.Assays |> Seq.map (fun (x:ArcAssay) -> x.Identifier) |> Array.ofSeq

    member this.UnregisteredAssays 
        with get(): ResizeArray<ArcAssay> = 
            this.Assays 
            |> ResizeArray.filter (fun a ->
                this.RegisteredStudies
                |> Seq.exists (fun s -> 
                    Seq.exists (fun i -> i = a.Identifier) s.RegisteredAssayIdentifiers
                )
                |> not
            )

    // - Assay API - CRUD //
    member this.AddAssay(assay: ArcAssay, ?registerIn: ResizeArray<ArcStudy>) =
        ArcTypesAux.SanityChecks.validateUniqueAssayIdentifier assay.Identifier (this.Assays |> Seq.map (fun x -> x.Identifier))
        assay.Investigation <- Some(this)
        this.Assays.Add(assay)
        if registerIn.IsSome then
            for study in registerIn.Value do
                study.RegisterAssay(assay.Identifier)

    static member addAssay(assay: ArcAssay, ?registerIn: ResizeArray<ArcStudy>) =
        fun (inv:ArcInvestigation) ->
            let newInvestigation = inv.Copy()
            newInvestigation.AddAssay(assay, ?registerIn=registerIn)
            newInvestigation

    // - Assay API - CRUD //
    member this.InitAssay(assayIdentifier: string, ?registerIn: ResizeArray<ArcStudy>) =
        let assay = ArcAssay(assayIdentifier)
        this.AddAssay(assay, ?registerIn=registerIn)
        assay

    static member initAssay(assayIdentifier: string, ?registerIn: ResizeArray<ArcStudy>) =
        fun (inv:ArcInvestigation) ->
            let newInvestigation = inv.Copy()
            newInvestigation.InitAssay(assayIdentifier, ?registerIn=registerIn)

    // - Assay API - CRUD //
    /// <summary>
    /// Removes assay at specified index from ArcInvestigation without deregistering it from studies.
    /// </summary>
    /// <param name="index"></param>
    member this.DeleteAssayAt(index: int) =
        this.Assays.RemoveAt(index)

    // - Assay API - CRUD //
    /// <summary>
    /// Removes assay at specified index from ArcInvestigation without deregistering it from studies.
    /// </summary>
    /// <param name="index"></param>
    static member deleteAssayAt(index: int) =
        fun (inv: ArcInvestigation) ->
            let newInvestigation = inv.Copy()
            newInvestigation.DeleteAssayAt(index)
            newInvestigation

    // - Assay API - CRUD //
    /// <summary>
    /// Removes assay with given identifier from ArcInvestigation without deregistering it from studies.
    /// </summary>
    /// <param name="index"></param>
    member this.DeleteAssay(assayIdentifier: string) =
        let index = this.GetAssayIndex(assayIdentifier)
        this.DeleteAssayAt(index)

    // - Assay API - CRUD //
    /// <summary>
    /// Removes assay with given identifier from ArcInvestigation without deregistering it from studies.
    /// </summary>
    /// <param name="index"></param>
    static member deleteAssay(assayIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.DeleteAssay(assayIdentifier)
            newInv


    // - Assay API - CRUD //
    /// <summary>
    /// Removes assay at specified index from ArcInvestigation and deregisteres it from all studies.
    /// </summary>
    /// <param name="index"></param>
    member this.RemoveAssayAt(index: int) =
        let ident = this.GetAssayAt(index).Identifier
        this.Assays.RemoveAt(index)
        for study in this.Studies do
            study.DeregisterAssay(ident)

    /// <summary>
    /// Removes assay at specified index from ArcInvestigation and deregisteres it from all studies.
    /// </summary>
    /// <param name="index"></param>
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

    // - Assay API - CRUD //
    /// <summary>
    /// Removes assay with specified identifier from ArcInvestigation and deregisteres it from all studies.
    /// </summary>
    /// <param name="index"></param>
    static member removeAssay(assayIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.RemoveAssay(assayIdentifier)
            newInv

    /// <summary>
    /// Renames an assay in the whole investigation  
    /// </summary>
    /// <param name="oldIdentifier">Identifier of the assay to be renamed</param>
    /// <param name="newIdentifier">Identifier to which the assay should be renamed to</param>
    member this.RenameAssay(oldIdentifier: string, newIdentifier: string) =        
        this.Assays 
        |> Seq.iter (fun a -> 
            if a.Identifier = oldIdentifier then 
                a.Identifier <- newIdentifier
        )
        this.Studies
        |> Seq.iter (fun s ->
            let index = 
                s.RegisteredAssayIdentifiers 
                |> Seq.tryFindIndex (fun ai -> ai = oldIdentifier)
            match index with
            | None -> ()
            | Some index -> 
                s.RegisteredAssayIdentifiers.[index] <- newIdentifier
        )

    static member renameAssay(oldIdentifier: string, newIdentifier: string) =       
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.RenameAssay(oldIdentifier,newIdentifier)
            newInv

    // - Assay API - CRUD //
    member this.SetAssayAt(index: int, assay: ArcAssay) =
        ArcTypesAux.SanityChecks.validateUniqueAssayIdentifier assay.Identifier (this.Assays |> Seq.removeAt index |> Seq.map (fun a -> a.Identifier))
        assay.Investigation <- Some(this)
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
        match this.TryGetAssay(assayIdentifier) with
        | Some a -> a
        | None -> failwith (ArcTypesAux.ErrorMsgs.unableToFindAssayIdentifier assayIdentifier this.Identifier)

    static member getAssay(assayIdentifier: string) : ArcInvestigation -> ArcAssay =
        fun (inv: ArcInvestigation) ->
            let newInvestigation = inv.Copy()
            newInvestigation.GetAssay(assayIdentifier)

    // - Assay API - CRUD //
    member this.TryGetAssay(assayIdentifier: string) : ArcAssay option =
        Seq.tryFind (fun a -> a.Identifier = assayIdentifier) this.Assays

    static member tryGetAssay(assayIdentifier: string) : ArcInvestigation -> ArcAssay option =
        fun (inv: ArcInvestigation) ->
            let newInvestigation = inv.Copy()
            newInvestigation.TryGetAssay(assayIdentifier)
    
    member this.ContainsAssay(assayIdentifier: string) =
        this.Assays
        |> Seq.exists (fun a -> a.Identifier = assayIdentifier)

    static member containsAssay (assayIdentifier: string) : ArcInvestigation -> bool =
        fun (inv: ArcInvestigation) ->            
            inv.ContainsAssay(assayIdentifier)

    /// Returns the count of registered study *identifiers*. This is not necessarily the same as the count of registered studies, as not all identifiers correspond to an existing study object.
    member this.RegisteredStudyIdentifierCount 
        with get() = this.RegisteredStudyIdentifiers.Count

    /// Returns all studies registered in this investigation, that correspond to an existing study object investigation.
    member this.RegisteredStudies 
        with get() : ResizeArray<ArcStudy> = 
            this.RegisteredStudyIdentifiers 
            |> ResizeArray.choose (fun identifier -> this.TryGetStudy identifier)

    /// Returns the count of registered studies. This is not necessarily the same as the count of registered study *identifiers*, as not all identifiers correspond to an existing study object.
    member this.RegisteredStudyCount 
        with get() = this.RegisteredStudies.Count

    /// Returns all registered study identifiers that do not correspond to an existing study object in the investigation.
    member this.VacantStudyIdentifiers
        with get() = 
            this.RegisteredStudyIdentifiers 
            |> ResizeArray.filter (this.ContainsStudy >> not)

    member this.StudyCount 
        with get() = this.Studies.Count

    member this.StudyIdentifiers
        with get() = this.Studies |> Seq.map (fun (x:ArcStudy) -> x.Identifier) |> Seq.toArray

    member this.UnregisteredStudies 
        with get() = 
            this.Studies 
            |> ResizeArray.filter (fun s -> 
                this.RegisteredStudyIdentifiers
                |> Seq.exists ((=) s.Identifier)
                |> not
            )

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
    member this.InitStudy (studyIdentifier: string) =
        let study = ArcStudy.init(studyIdentifier)
        this.AddStudy(study)
        study

    static member initStudy(studyIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy,copy.InitStudy(studyIdentifier)

    // - Study API - CRUD //
    member this.RegisterStudy (studyIdentifier : string) = 
        ArcTypesAux.SanityChecks.validateExistingStudyRegisterInInvestigation studyIdentifier this.StudyIdentifiers 
        ArcTypesAux.SanityChecks.validateUniqueRegisteredStudyIdentifiers studyIdentifier this.RegisteredStudyIdentifiers       
        this.RegisteredStudyIdentifiers.Add(studyIdentifier)

    static member registerStudy(studyIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.RegisterStudy(studyIdentifier)
            copy

    // - Study API - CRUD //
    member this.AddRegisteredStudy (study: ArcStudy) = 
        this.AddStudy study
        this.RegisterStudy(study.Identifier)

    static member addRegisteredStudy(study: ArcStudy) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            let study = study.Copy()
            copy.AddRegisteredStudy(study)
            copy

    /// <summary>
    /// Removes study at specified index from ArcInvestigation without deregistering it.
    /// </summary>
    /// <param name="index"></param>
    member this.DeleteStudyAt(index: int) =
        this.Studies.RemoveAt(index)

    /// <summary>
    /// Removes study at specified index from ArcInvestigation without deregistering it.
    /// </summary>
    /// <param name="index"></param>
    static member deleteStudyAt(index: int) =
        fun (i: ArcInvestigation) ->
            let copy = i.Copy()
            copy.DeleteStudyAt(index)
            copy

    /// <summary>
    /// Removes study with specified identifier from ArcInvestigation without deregistering it.
    /// </summary>
    /// <param name="studyIdentifier"></param>
    member this.DeleteStudy(studyIdentifier: string) =
        let index = this.Studies.FindIndex(fun s -> s.Identifier = studyIdentifier)
        this.DeleteStudyAt(index)

    /// <summary>
    /// Removes study with specified identifier from ArcInvestigation without deregistering it.
    /// </summary>
    /// <param name="studyIdentifier"></param>
    static member deleteStudy(studyIdentifier: string) =
        fun (i: ArcInvestigation) ->
            let copy = i.Copy()
            copy.DeleteStudy studyIdentifier
            copy

    // - Study API - CRUD //
    /// <summary>
    /// Removes study at specified index from ArcInvestigation and deregisteres it.
    /// </summary>
    /// <param name="index"></param>
    member this.RemoveStudyAt(index: int) =
        let ident = this.GetStudyAt(index).Identifier
        this.Studies.RemoveAt(index)
        this.DeregisterStudy(ident)

    // - Study API - CRUD //
    /// <summary>
    /// Removes study at specified index from ArcInvestigation and deregisteres it.
    /// </summary>
    /// <param name="index"></param>
    static member removeStudyAt(index: int) =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.RemoveStudyAt(index)
            newInv

    // - Study API - CRUD //
    /// <summary>
    /// Removes study with specified identifier from ArcInvestigation and deregisteres it.
    /// </summary>
    /// <param name="studyIdentifier"></param>
    member this.RemoveStudy(studyIdentifier: string) =
        let index = this.GetStudyIndex(studyIdentifier)
        this.RemoveStudyAt(index)

    /// <summary>
    /// Removes study with specified identifier from ArcInvestigation and deregisteres it.
    /// </summary>
    /// <param name="studyIdentifier"></param>
    static member removeStudy(studyIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.RemoveStudy(studyIdentifier)
            copy

    /// <summary>
    /// Renames a study in the whole investigation
    /// </summary>
    /// <param name="oldIdentifier">Identifier of the study to be renamed</param>
    /// <param name="newIdentifier">Identifier to which the study should be renamed to</param>
    member this.RenameStudy(oldIdentifier: string, newIdentifier: string) =        
        this.Studies 
        |> Seq.iter (fun s -> 
            if s.Identifier = oldIdentifier then 
                s.Identifier <- newIdentifier
        )
        let index =
            this.RegisteredStudyIdentifiers
            |> Seq.tryFindIndex (fun si -> si = oldIdentifier)
        match index with
        | None -> ()
        | Some index -> this.RegisteredStudyIdentifiers.[index] <- newIdentifier

    /// <summary>
    /// Renames a study in the whole investigation
    /// </summary>
    /// <param name="oldIdentifier">Identifier of the study to be renamed</param>
    /// <param name="newIdentifier">Identifier to which the study should be renamed to</param>
    static member renameStudy(oldIdentifier: string, newIdentifier: string) =       
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.RenameStudy(oldIdentifier,newIdentifier)
            newInv

    // - Study API - CRUD //
    member this.SetStudyAt(index: int, study: ArcStudy) =
        ArcTypesAux.SanityChecks.validateUniqueStudyIdentifier study (this.Studies |> Seq.removeAt index)
        study.Investigation <- Some this
        this.Studies.[index] <- study

    static member setStudyAt(index: int, study: ArcStudy) =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.SetStudyAt(index, study)
            newInv

    // - Study API - CRUD //
    member this.SetStudy(studyIdentifier: string, study: ArcStudy) =
        let index = this.GetStudyIndex studyIdentifier
        this.SetStudyAt(index,study)

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
        match this.TryGetStudy studyIdentifier with
        | Some s -> s
        | None -> failwith (ArcTypesAux.ErrorMsgs.unableToFindStudyIdentifier studyIdentifier this.Identifier)

    static member getStudy(studyIdentifier: string) : ArcInvestigation -> ArcStudy =
        fun (inv: ArcInvestigation) ->
            let newInv = inv.Copy()
            newInv.GetStudy(studyIdentifier)

    member this.TryGetStudy(studyIdentifier: string) : ArcStudy option =
        this.Studies |> Seq.tryFind (fun s -> s.Identifier = studyIdentifier)
        
    static member tryGetStudy(studyIdentifier : string) : ArcInvestigation -> ArcStudy option = 
        fun (inv: ArcInvestigation) -> 
            let newInv = inv.Copy()
            newInv.TryGetStudy(studyIdentifier)

    member this.ContainsStudy(studyIdentifier: string) =
        this.Studies
        |> Seq.exists (fun s -> s.Identifier = studyIdentifier)

    static member containsStudy (studyIdentifier: string) : ArcInvestigation -> bool =
        fun (inv: ArcInvestigation) ->            
            inv.ContainsStudy(studyIdentifier)

    // - Study API - CRUD //
    /// <summary>
    /// Register an existing assay from ArcInvestigation.Assays to a existing study.
    /// </summary>
    /// <param name="studyIdentifier"></param>
    /// <param name="assay"></param>
    member this.RegisterAssayAt(studyIndex: int, assayIdentifier: string) =
        let study = this.GetStudyAt(studyIndex)
        ArcTypesAux.SanityChecks.validateAssayRegisterInInvestigation assayIdentifier (this.Assays |> Seq.map (fun a -> a.Identifier))
        ArcTypesAux.SanityChecks.validateUniqueAssayIdentifier assayIdentifier study.RegisteredAssayIdentifiers
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

    member this.DeregisterStudy(studyIdentifier: string) =
        this.RegisteredStudyIdentifiers.Remove(studyIdentifier) |> ignore

    static member deregisterStudy(studyIdentifier: string) =
        fun (i: ArcInvestigation) ->
            let copy = i.Copy()
            copy.DeregisterStudy(studyIdentifier)
            copy

    member this.WorkflowCount
        with get() = this.Workflows.Count

    member this.WorkflowIdentifiers
        with get() = this.Workflows |> Seq.map (fun (x:ArcWorkflow) -> x.Identifier) |> Seq.toArray

    member this.GetWorkflowIndex(workflowIdentifier: string) : int =
        let index = this.Workflows.FindIndex (fun w -> w.Identifier = workflowIdentifier)
        if index = -1 then failwith $"Unable to find workflow with specified identifier '{workflowIdentifier}'!"
        index

    static member getWorkflowIndex(workflowIdentifier: string) : ArcInvestigation -> int =
        fun (inv: ArcInvestigation) -> inv.GetWorkflowIndex(workflowIdentifier)

    member this.AddWorkflow(workflow: ArcWorkflow) =
        ArcTypesAux.SanityChecks.validateUniqueWorkflowIdentifier workflow this.Workflows
        workflow.Investigation <- Some this
        this.Workflows.Add(workflow)

    static member addWorkflow(workflow: ArcWorkflow) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.AddWorkflow(workflow)
            copy

    member this.InitWorkflow(workflowIdentifier: string) =
        let workflow = ArcWorkflow.init(workflowIdentifier)
        this.AddWorkflow(workflow)
        workflow

    static member initWorkflow(workflowIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.InitWorkflow(workflowIdentifier)

    member this.DeleteWorkflowAt(index: int) =
        this.Workflows.RemoveAt(index)

    static member deleteWorkflowAt(index: int) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.DeleteWorkflowAt(index)
            copy

    member this.DeleteWorkflow(workflowIdentifier: string) =
        let index = this.Workflows.FindIndex(fun w -> w.Identifier = workflowIdentifier)
        this.DeleteWorkflowAt(index)

    static member deleteWorkflow(workflowIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.DeleteWorkflow(workflowIdentifier)
            copy

    member this.GetWorkflowAt(index: int) : ArcWorkflow =
        this.Workflows.[index]

    static member getWorkflowAt(index: int) : ArcInvestigation -> ArcWorkflow =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.GetWorkflowAt(index)

    member this.GetWorkflow(workflowIdentifier: string) : ArcWorkflow =
        match this.TryGetWorkflow workflowIdentifier with
        | Some w -> w
        | None -> failwith (ArcTypesAux.ErrorMsgs.unableToFindWorkflowIdentifier workflowIdentifier this.Identifier)

    static member getWorkflow(workflowIdentifier: string) : ArcInvestigation -> ArcWorkflow =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.GetWorkflow(workflowIdentifier)

    member this.TryGetWorkflow(workflowIdentifier: string) : ArcWorkflow option =
        this.Workflows |> Seq.tryFind (fun w -> w.Identifier = workflowIdentifier)

    static member tryGetWorkflow(workflowIdentifier: string) : ArcInvestigation -> ArcWorkflow option =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.TryGetWorkflow(workflowIdentifier)

    member this.ContainsWorkflow(workflowIdentifier: string) : bool =
        this.Workflows
        |> Seq.exists (fun w -> w.Identifier = workflowIdentifier)

    static member containsWorkflow(workflowIdentifier: string) : ArcInvestigation -> bool =
        fun (inv: ArcInvestigation) ->
            inv.ContainsWorkflow(workflowIdentifier)

    member this.SetWorkflowAt(index: int, workflow: ArcWorkflow) =
        ArcTypesAux.SanityChecks.validateUniqueWorkflowIdentifier workflow this.Workflows
        workflow.Investigation <- Some this
        this.Workflows.[index] <- workflow

    member this.SetWorkflow(workflowIdentifier: string, workflow: ArcWorkflow) =
        let index = this.GetWorkflowIndex(workflowIdentifier)
        this.SetWorkflowAt(index, workflow)

    static member setWorkflow(workflowIdentifier: string, workflow: ArcWorkflow) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.SetWorkflow(workflowIdentifier, workflow)
            copy

    static member setWorkflowAt(index: int, workflow: ArcWorkflow) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.SetWorkflowAt(index, workflow)
            copy

    member this.RunCount
        with get() = this.Runs.Count

    member this.RunIdentifiers
        with get() = this.Runs |> Seq.map (fun (x:ArcRun) -> x.Identifier) |> Seq.toArray

    member this.GetRunIndex(runIdentifier: string) : int =
        let index = this.Runs.FindIndex (fun r -> r.Identifier = runIdentifier)
        if index = -1 then failwith $"Unable to find run with specified identifier '{runIdentifier}'!"
        index

    static member getRunIndex(runIdentifier: string) : ArcInvestigation -> int =
        fun (inv: ArcInvestigation) -> inv.GetRunIndex(runIdentifier)

    member this.AddRun(run: ArcRun) =
        ArcTypesAux.SanityChecks.validateUniqueRunIdentifier run this.Runs
        run.Investigation <- Some this
        this.Runs.Add(run)

    static member addRun(run: ArcRun) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.AddRun(run)
            copy

    member this.InitRun(runIdentifier: string) =
        let run = ArcRun.init(runIdentifier)
        this.AddRun(run)
        run

    static member initRun(runIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.InitRun(runIdentifier)

    member this.DeleteRunAt(index: int) =
        this.Runs.RemoveAt(index)

    static member deleteRunAt(index: int) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.DeleteRunAt(index)
            copy

    member this.DeleteRun(runIdentifier: string) =
        let index = this.Runs.FindIndex(fun w -> w.Identifier = runIdentifier)
        this.DeleteRunAt(index)

    static member deleteRun(runIdentifier: string) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.DeleteRun(runIdentifier)
            copy

    member this.GetRunAt(index: int) : ArcRun =
        this.Runs.[index]

    static member getRunAt(index: int) : ArcInvestigation -> ArcRun =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.GetRunAt(index)

    member this.GetRun(runIdentifier: string) : ArcRun =
        match this.TryGetRun runIdentifier with
        | Some w -> w
        | None -> failwith (ArcTypesAux.ErrorMsgs.unableToFindRunIdentifier runIdentifier this.Identifier)

    static member getRun(runIdentifier: string) : ArcInvestigation -> ArcRun =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.GetRun(runIdentifier)

    member this.TryGetRun(runIdentifier: string) : ArcRun option =
        this.Runs |> Seq.tryFind (fun w -> w.Identifier = runIdentifier)

    static member tryGetRun(runIdentifier: string) : ArcInvestigation -> ArcRun option =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.TryGetRun(runIdentifier)

    member this.ContainsRun(runIdentifier: string) : bool =
        this.Runs
        |> Seq.exists (fun w -> w.Identifier = runIdentifier)

    static member containsRun(runIdentifier: string) : ArcInvestigation -> bool =
        fun (inv: ArcInvestigation) ->
            inv.ContainsRun(runIdentifier)

    member this.SetRunAt(index: int, run: ArcRun) =
        ArcTypesAux.SanityChecks.validateUniqueRunIdentifier run this.Runs
        run.Investigation <- Some this
        this.Runs.[index] <- run

    member this.SetRun(runIdentifier: string, run: ArcRun) =
        let index = this.GetRunIndex(runIdentifier)
        this.SetRunAt(index, run)

    static member setRunAt(index: int, run: ArcRun) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.SetRunAt(index, run)
            copy

    static member setRun(runIdentifier: string, run: ArcRun) =
        fun (inv: ArcInvestigation) ->
            let copy = inv.Copy()
            copy.SetRun(runIdentifier, run)
            copy

    /// <summary>
    /// Returns all fully distinct Contacts/Performers from assays/studies/investigation. 
    /// </summary>
    member this.GetAllPersons() : Person [] =
        let persons = ResizeArray()
        for a in this.Assays do
            persons.AddRange(a.Performers)
        for s in this.Studies do
            persons.AddRange(s.Contacts)
        persons.AddRange(this.Contacts)
        persons
        |> Array.ofSeq
        |> Array.distinct

    /// <summary>
    /// Returns all fully distinct Contacts/Performers from assays/studies/investigation unfiltered. 
    /// </summary>
    member this.GetAllPublications() : Publication [] =
        let pubs = ResizeArray()
        for s in this.Studies do
            pubs.AddRange(s.Publications)
        pubs.AddRange(this.Publications)
        pubs
        |> Array.ofSeq
        |> Array.distinct

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
    
    /// Updates the IOtypes of the IO columns (Input, Output) across all tables in the investigation if possible.
    ///
    /// If an entity (Row Value of IO Column) with the same name as an entity with a higher IOType specifity is found, the IOType of the entity with the lower IOType specificity is updated.
    ///
    /// E.g. In Table1, there is a column "Output [Sample Name]" with an entity "Sample1". In Table2, there is a column "Input [Source Name]" with the same entity "Sample1". By equality of the entities, the IOType of the Input column in Table2 is inferred to be Sample, resulting in "Input [Sample Name]".
    ///
    /// E.g. RawDataFile is more specific than Source, but less specific than DerivedDataFile.
    ///
    /// E.g. Sample is equally specific to RawDataFile.
    member this.UpdateIOTypeByEntityID() =
        let ioMap = 
            [
                for study in this.Studies do
                    yield! study.Tables
                for assay in this.Assays do
                    yield! assay.Tables
                for run in this.Runs do
                    yield! run.Tables
            ]
            |> ResizeArray
            |> ArcTablesAux.getIOMap
        for study in this.Studies do
            ArcTablesAux.applyIOMap ioMap study.Tables
        for assay in this.Assays do
            ArcTablesAux.applyIOMap ioMap assay.Tables
        for run in this.Runs do
            ArcTablesAux.applyIOMap ioMap run.Tables

    member this.Copy() : ArcInvestigation =
        let nextAssays = ResizeArray()
        let nextStudies = ResizeArray()
        let nextWorkflows = ResizeArray()
        let nextRuns = ResizeArray()
        for assay in this.Assays do
            let copy = assay.Copy()
            nextAssays.Add(copy)
        for study in this.Studies do
            let copy = study.Copy()
            nextStudies.Add(copy)
        for workflow in this.Workflows do
            let copy = workflow.Copy()
            nextWorkflows.Add(copy)
        for run in this.Runs do
            let copy = run.Copy()
            nextRuns.Add(copy)
        let nextComments = this.Comments |> ResizeArray.map (fun c -> c.Copy())
        let nextRemarks = this.Remarks |> ResizeArray.map (fun c -> c.Copy())
        let nextContacts = this.Contacts |> ResizeArray.map (fun c -> c.Copy())
        let nextPublications = this.Publications |> ResizeArray.map (fun c -> c.Copy())
        let nextOntologySourceReferences = this.OntologySourceReferences |> ResizeArray.map (fun c -> c.Copy())
        let nextStudyIdentifiers = ResizeArray(this.RegisteredStudyIdentifiers)
        let i = ArcInvestigation(
            this.Identifier,
            ?title = this.Title,
            ?description = this.Description,
            ?submissionDate = this.SubmissionDate,
            ?publicReleaseDate = this.PublicReleaseDate,
            studies = nextStudies,
            assays = nextAssays,
            workflows = nextWorkflows,
            runs = nextRuns,
            registeredStudyIdentifiers = nextStudyIdentifiers,
            ontologySourceReferences = nextOntologySourceReferences,
            publications = nextPublications,
            contacts = nextContacts,
            comments = nextComments,
            remarks = nextRemarks
        )
        i

    ///// <summary>
    ///// Updates given investigation with another investigation, Identifier will never be updated. By default update is full replace. Optional Parameters can be used to specify update logic.
    ///// </summary>
    ///// <param name="investigation">The investigation used for updating this investigation.</param>
    ///// <param name="onlyReplaceExisting">If true, this will only update fields which are `Some` or non-empty lists. Default: **false**</param>
    ///// <param name="appendSequences">If true, this will append lists instead of replacing. Will return only distinct elements. Default: **false**</param>
    //member this.UpdateBy(inv:ArcInvestigation,?onlyReplaceExisting : bool,?appendSequences : bool) =
    //    let onlyReplaceExisting = defaultArg onlyReplaceExisting false
    //    let appendSequences = defaultArg appendSequences false
    //    let updateAlways = onlyReplaceExisting |> not
    //    if inv.Title.IsSome || updateAlways then 
    //        this.Title <- inv.Title
    //    if inv.Description.IsSome || updateAlways then 
    //        this.Description <- inv.Description
    //    if inv.SubmissionDate.IsSome || updateAlways then 
    //        this.SubmissionDate <- inv.SubmissionDate
    //    if inv.PublicReleaseDate.IsSome || updateAlways then 
    //        this.PublicReleaseDate <- inv.PublicReleaseDate
    //    if inv.OntologySourceReferences.Length <> 0 || updateAlways then
    //        let s = ArcTypesAux.updateAppendArray appendSequences this.OntologySourceReferences inv.OntologySourceReferences
    //        this.OntologySourceReferences <- s
    //    if inv.Publications.Length <> 0 || updateAlways then
    //        let s = ArcTypesAux.updateAppendArray appendSequences this.Publications inv.Publications
    //        this.Publications <- s
    //    if inv.Contacts.Length <> 0 || updateAlways then
    //        let s = ArcTypesAux.updateAppendArray appendSequences this.Contacts inv.Contacts
    //        this.Contacts <- s
    //    if inv.Assays.Count <> 0 || updateAlways then
    //        let s = ArcTypesAux.updateAppendResizeArray appendSequences this.Assays inv.Assays
    //        this.Assays <- s
    //    if inv.Studies.Count <> 0 || updateAlways then
    //        let s = ArcTypesAux.updateAppendResizeArray appendSequences this.Studies inv.Studies
    //        this.Studies <- s
    //    if inv.RegisteredStudyIdentifiers.Count <> 0 || updateAlways then
    //        let s = ArcTypesAux.updateAppendResizeArray appendSequences this.RegisteredStudyIdentifiers inv.RegisteredStudyIdentifiers
    //        this.RegisteredStudyIdentifiers <- s
    //    if inv.Comments.Length <> 0 || updateAlways then
    //        let s = ArcTypesAux.updateAppendArray appendSequences this.Comments inv.Comments
    //        this.Comments <- s
    //    if inv.Remarks.Length <> 0 || updateAlways then
    //        let s = ArcTypesAux.updateAppendArray appendSequences this.Remarks inv.Remarks
    //        this.Remarks <- s

    member this.StructurallyEquals (other: ArcInvestigation) : bool =
        let i = this.Identifier = other.Identifier
        let t = this.Title = other.Title
        let d = this.Description = other.Description
        let sd = this.SubmissionDate = other.SubmissionDate
        let prd = this.PublicReleaseDate = other.PublicReleaseDate 
        let pub = Seq.compare this.Publications other.Publications
        let con = Seq.compare this.Contacts other.Contacts
        let osr = Seq.compare this.OntologySourceReferences other.OntologySourceReferences
        let assays = Seq.compare this.Assays other.Assays
        let studies = Seq.compare this.Studies other.Studies
        let workflows = Seq.compare this.Workflows other.Workflows
        let runs = Seq.compare this.Runs other.Runs
        let reg_studies = Seq.compare this.RegisteredStudyIdentifiers other.RegisteredStudyIdentifiers
        let comments = Seq.compare this.Comments other.Comments
        let remarks = Seq.compare this.Remarks other.Remarks
        // Todo maybe add reflection check to prove that all members are compared?
        [|i; t; d; sd; prd; pub; con; osr; assays; studies; workflows; runs; reg_studies; comments; remarks|] |> Seq.forall (fun x -> x = true)

    /// <summary>
    /// Use this function to check if this ArcInvestigation and the input ArcInvestigation refer to the same object.
    ///
    /// If true, updating one will update the other due to mutability.
    /// </summary>
    /// <param name="other">The other ArcInvestigation to test for reference.</param>
    member this.ReferenceEquals (other: ArcInvestigation) = System.Object.ReferenceEquals(this,other)

    // Use this for better print debugging and better unit test output
    override this.ToString() =
        sprintf 
            """ArcInvestigation {
    Identifier = %A,
    Title = %A,
    Description = %A,
    SubmissionDate = %A,
    PublicReleaseDate = %A,
    OntologySourceReferences = %A,
    Publications = %A,
    Contacts = %A,
    Assays = %A,
    Studies = %A,
    Workflows = %A,
    Runs = %A,
    RegisteredStudyIdentifiers = %A,
    Comments = %A,
    Remarks = %A,
}"""
            this.Identifier
            this.Title
            this.Description
            this.SubmissionDate
            this.PublicReleaseDate
            this.OntologySourceReferences
            this.Publications
            this.Contacts
            this.Assays
            this.Studies
            this.Workflows
            this.Runs
            this.RegisteredStudyIdentifiers
            this.Comments
            this.Remarks

    // custom check
    override this.Equals other =
        match other with
        | :? ArcInvestigation as i -> 
            this.StructurallyEquals(i)
        | _ -> false

    override this.GetHashCode() = 
        [|
            box this.Identifier
            HashCodes.boxHashOption this.Title
            HashCodes.boxHashOption this.Description
            HashCodes.boxHashOption this.SubmissionDate
            HashCodes.boxHashOption this.PublicReleaseDate
            HashCodes.boxHashSeq this.Publications
            HashCodes.boxHashSeq this.Contacts
            HashCodes.boxHashSeq this.OntologySourceReferences
            HashCodes.boxHashSeq this.Assays
            HashCodes.boxHashSeq this.Studies
            HashCodes.boxHashSeq this.Workflows
            HashCodes.boxHashSeq this.Runs
            HashCodes.boxHashSeq this.RegisteredStudyIdentifiers
            HashCodes.boxHashSeq this.Comments
            HashCodes.boxHashSeq this.Remarks
        |]
        |> HashCodes.boxHashArray 
        |> fun x -> x :?> int

    member this.GetLightHashCode() =
        [|
            box this.Identifier
            HashCodes.boxHashOption this.Title
            HashCodes.boxHashOption this.Description
            HashCodes.boxHashOption this.SubmissionDate
            HashCodes.boxHashOption this.PublicReleaseDate
            HashCodes.boxHashSeq this.Publications
            HashCodes.boxHashSeq this.Contacts
            HashCodes.boxHashSeq this.OntologySourceReferences
            HashCodes.boxHashSeq this.RegisteredStudyIdentifiers
            HashCodes.boxHashSeq this.Comments
            HashCodes.boxHashSeq this.Remarks
        |]
        |> HashCodes.boxHashArray 
        |> fun x -> x :?> int