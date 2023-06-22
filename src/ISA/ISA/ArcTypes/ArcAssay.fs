namespace ISA

open Fable.Core

module ArcAssayAux =

    // This was used before switching to non option
    ///// Gets sheets.Count from option. Returns default Count 0 if sheets isNone.
    //let getSheetsCount (sheets: ResizeArray<ArcTable>) =
    //    Option.map (fun (s: ResizeArray<ArcTable>) -> s.Count) sheets |> Option.defaultValue 0

    module SanityChecks =
        
        let validateSheetIndex (index: int) (allowAppend: bool) (sheets: ResizeArray<ArcTable>) =
            let eval x y = if allowAppend then x > y else x >= y
            if index < 0 then failwith "Cannot insert ArcTable at index < 0."
            if eval index sheets.Count then failwith $"Specified index is out of range! Assay contains only {sheets.Count} tables."

open ArcAssayAux

// "MyAssay"; "assays/MyAssay/isa.assay.xlsx"

[<AttachMembers>]
type ArcAssay = 

    {
        ID : URI option
        FileName : string option
        MeasurementType : OntologyAnnotation option
        TechnologyType : OntologyAnnotation option
        TechnologyPlatform : string option
        Sheets : ResizeArray<ArcTable>
        Performers : Person list option
        Comments : Comment list option
    }
   
    static member make 
        (id : URI option)
        (fleName : string option)
        (measurementType : OntologyAnnotation option)
        (technologyType : OntologyAnnotation option)
        (technologyPlatform : string option)
        (sheets : ResizeArray<ArcTable>)
        (performers : Person list option)
        (comments : Comment list option) = 
        {
            ID = id
            FileName = fleName
            MeasurementType = measurementType
            TechnologyType = technologyType
            TechnologyPlatform = technologyPlatform
            Sheets = sheets
            Performers = performers
            Comments = comments
        }

    member this.SheetCount 
        with get() = this.Sheets.Count

    member this.SheetNames 
        with get() = 
            [for s in this.Sheets do yield s.Name]

    [<NamedParams>]
    static member create (?ID : URI, ?FileName : string, ?MeasurementType : OntologyAnnotation, ?TechnologyType : OntologyAnnotation, ?TechnologyPlatform : string, ?Sheets : ResizeArray<ArcTable>, ?Performers : Person list, ?Comments : Comment list) = 
        let Sheets = ResizeArray()
        ArcAssay.make ID FileName MeasurementType TechnologyType TechnologyPlatform Sheets Performers Comments

    member this.AddTable(?table:ArcTable, ?index: int) = 
        let index = defaultArg index this.SheetCount
        let table = defaultArg table (ArcTable.init("New Table"))
        SanityChecks.validateSheetIndex index true this.Sheets
        this.Sheets.Insert(index, table)

    member this.AddTables(tables:seq<ArcTable>, ?index: int) = 
        let index = defaultArg index this.SheetCount
        SanityChecks.validateSheetIndex index true this.Sheets
        this.Sheets.InsertRange(index, tables)


    member this.GetTable(index:int) : ArcTable =
        SanityChecks.validateSheetIndex index false this.Sheets
        this.Sheets.[index]

    static member getTable(index:int) : ArcAssay -> ArcTable =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.GetTable(index)


    member this.SetTable(index:int, table:ArcTable) =
        SanityChecks.validateSheetIndex index false this.Sheets
        this.Sheets.[index] <- table

    static member setTable(index:int, table:ArcTable) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.SetTable(index, table)
            newAssay


    member this.RemoveTable(index:int) : unit =
        SanityChecks.validateSheetIndex index false this.Sheets
        this.Sheets.RemoveAt(index)

    static member removeTable(index:int) : ArcAssay -> ArcAssay =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()
            newAssay.RemoveTable(index)
            newAssay


    member this.UpdateTable(index: int, updateFun: ArcTable -> ArcTable) =
        SanityChecks.validateSheetIndex index false this.Sheets
        let table = this.Sheets.[index]
        let newTable = updateFun table
        this.SetTable(index,newTable)

    static member updateTable(index:int, updateFun: ArcTable -> ArcTable) =
        fun (assay:ArcAssay) ->
            let newAssay = assay.Copy()    
            newAssay.UpdateTable(index, updateFun)
            newAssay


    member this.Copy() : ArcAssay =
        let newSheets = ResizeArray()
        for sheet in this.Sheets do
            let copy = sheet.Copy()
            newSheets.Add(copy)
        { this with Sheets = newSheets }
        
    static member getIdentifier (assay : Assay) = 
        raise (System.NotImplementedException())

    static member setPerformers performers assay =
        {assay with Performers = performers}

    static member fromAssay (assay : Assay) : ArcAssay =
        raise (System.NotImplementedException())

    static member toAssay (assay : ArcAssay) : Assay =
        raise (System.NotImplementedException())