namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.FileSystem
open System.Collections.Generic
//open ColumnIndex

open ColumnIndex
open ARCtrl.Helper.Regex.ActivePatterns

module CompositeRow =

    let toProtocol (tableName : string) (row : (CompositeHeader*CompositeCell) seq) =
        let id = tableName
        row
        |> Seq.fold (fun p hc ->
            match hc with
            | CompositeHeader.ProtocolType, CompositeCell.Term oa -> 
                LDLabProtocol.setIntendedUseAsDefinedTerm(p, BaseTypes.composeDefinedTerm oa)
                
            | CompositeHeader.ProtocolVersion, CompositeCell.FreeText v ->
                LDLabProtocol.setVersionAsString(p,v)
                
            | CompositeHeader.ProtocolUri, CompositeCell.FreeText v ->
                LDLabProtocol.setUrl(p,v)
                
            | CompositeHeader.ProtocolDescription, CompositeCell.FreeText v ->
                LDLabProtocol.setDescriptionAsString(p,v)
                
            | CompositeHeader.ProtocolREF, CompositeCell.FreeText v ->
                LDLabProtocol.setNameAsString(p,v)             
            //| CompositeHeader.Parameter oa, _ ->
            //    DefinedTerm.create
            //    let pp = ProtocolParameter.create(ParameterName = oa)
            //    Protocol.addParameter (pp) p
            | CompositeHeader.Component _, CompositeCell.Term _
            | CompositeHeader.Component _, CompositeCell.Unitized _ ->            
                let c = BaseTypes.composeComponent (fst hc) (snd hc)
                let newC = ResizeArray.appendSingleton c (LDLabProtocol.getLabEquipments(p))
                LDLabProtocol.setLabEquipments(p,newC)  
            | _ -> ()
            p
        ) (LDLabProtocol.create(id = id, name = tableName))


[<AutoOpen>]
module TableTypeExtensions = 

    type ArcTable with

        /// Create a new table from an ISA protocol.
        ///
        /// The table will have at most one row, with the protocol information and the component values
        static member fromProtocol (p : LDNode, ?graph : LDGraph, ?context : LDContext) : ArcTable = 

            let name = LDLabProtocol.getNameAsString(p, ?context = context)
            let t = ArcTable.init name

            //for pp in LabProtocol.getPa p.Parameters |> Option.defaultValue [] do

            //    //t.AddParameterColumn(pp, ?index = pp.TryGetColumnIndex())

            //    t.AddColumn(CompositeHeader.Parameter pp.ParameterName.Value, ?index = pp.TryGetColumnIndex())

            for c in LDLabProtocol.getComponents(p, ?graph = graph, ?context = context) do
                let h,v = BaseTypes.decomposeComponent(c, ?context = context)
                t.AddColumn(
                    h, 
                    cells = ResizeArray.singleton v,
                    ?index = c.TryGetColumnIndex())
            LDLabProtocol.tryGetDescriptionAsString(p, ?context = context)  |> Option.map (fun d -> t.AddProtocolDescriptionColumn(ResizeArray.singleton d))  |> ignore
            LDLabProtocol.tryGetVersionAsString(p, ?context = context)       |> Option.map (fun d -> t.AddProtocolVersionColumn(ResizeArray.singleton d))      |> ignore
            ProcessConversion.tryGetProtocolType(p, ?context =context) |> Option.map (fun d -> t.AddProtocolTypeColumn(ResizeArray.singleton d))         |> ignore
            LDLabProtocol.tryGetUrl(p, ?context = context)           |> Option.map (fun d -> t.AddProtocolUriColumn(ResizeArray.singleton d))          |> ignore
            t.AddProtocolNameColumn(ResizeArray.singleton name)
            t

        /// Returns the list of protocols executed in this ArcTable
        member this.GetProtocols() : LDNode list = 

            if this.RowCount = 0 then
                this.Headers
                |> Seq.fold (fun (p : LDNode) h -> 
                    match h with
                    //| CompositeHeader.Parameter oa -> 
                    //    let pp = ProtocolParameter.create(ParameterName = oa)
                    //    Protocol.addParameter (pp) p
                    | CompositeHeader.Component oa ->
                        let n, na = oa.NameText, oa.TermAccessionOntobeeUrl
                        let c = LDPropertyValue.createComponent(n, "Empty Component Value", propertyID = na)
                        let newC = ResizeArray.appendSingleton c (LDLabProtocol.getLabEquipments p)
                        LDLabProtocol.setLabEquipments(p,newC)
                    | _ -> ()
                    p
                ) (LDLabProtocol.create(id = Identifier.createMissingIdentifier(), name = this.Name))
                |> List.singleton
            else
                List.init this.RowCount (fun i ->
                    this.GetRow(i, SkipValidation = true) 
                    |> Seq.zip this.Headers
                    |> CompositeRow.toProtocol this.Name                   
                )
                |> List.distinct

        /// Returns the list of processes specidified in this ArcTable
        member this.GetProcesses(?assayName, ?studyName, ?fs : FileSystem) : LDNode list = 
            if this.RowCount = 0 then
                //let input = ResizeArray [Sample.createSample(name = $"{this.Name}_Input", additionalProperties = ResizeArray [])]
                //let output = ResizeArray [Sample.createSample(name = $"{this.Name}_Output", additionalProperties = ResizeArray [])]
                LDLabProcess.create(name = this.Name(*, objects = input, results = output*))
                |> List.singleton
            else
                let getter = ProcessConversion.getProcessGetter assayName studyName this.Name this.Headers fs        
                [
                    for i in 0..this.RowCount-1 do
                        yield getter this i        
                ]
                //|> ProcessConversion.mergeIdenticalProcesses


        /// Create a new table from a list of processes
        ///
        /// The name will be used as the sheet name
        /// 
        /// The processes SHOULD have the same headers, or even execute the same protocol
        static member fromProcesses(name,ps : LDNode list, ?graph : LDGraph, ?context : LDContext) : ArcTable = 
            ps
            |> List.collect (fun p -> ProcessConversion.processToRows(p,?context = context,?graph = graph) |> List.ofSeq)
            |> ArcTableAux.Unchecked.alignByHeaders true
            |> fun (headers, rows) -> ArcTable.fromArcTableValues(name,headers,rows)    

    type ArcTables with

        /// Return a list of all the processes in all the tables.
        member this.GetProcesses(?assayName, ?studyName, ?fs) : LDNode list = 
            this.Tables
            |> Seq.toList
            |> List.collect (fun t -> t.GetProcesses(?assayName = assayName, ?studyName = studyName, ?fs = fs))

        /// Create a collection of tables from a list of processes.
        ///
        /// For this, the processes are grouped by nameroot ("nameroot_1", "nameroot_2" ...) or exectued protocol if no name exists
        ///
        /// Then each group is converted to a table with this nameroot as sheetname
        static member fromProcesses (ps : LDNode list, ?graph : LDGraph, ?context : LDContext) : ArcTables = 
            ProcessConversion.groupProcesses(ps, ?graph = graph, ?context = context)
            |> List.map (fun (name,ps) ->
                ps
                |> List.collect (fun p -> ProcessConversion.processToRows(p,?graph = graph, ?context = context) |> List.ofSeq)
                |> fun rows -> ArcTableAux.Unchecked.alignByHeaders true rows
                |> fun (headers, rows) -> ArcTable.fromArcTableValues(name,headers,rows)
            )
            |> ResizeArray
            |> ArcTables