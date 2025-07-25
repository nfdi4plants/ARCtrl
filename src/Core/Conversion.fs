module ARCtrl.Process.Conversion

open ARCtrl
open ARCtrl.Helper
open System.Collections.Generic
open ColumnIndex

module Person = 

    let orcidKey = "ORCID"
    let AssayIdentifierPrefix = "performer (ARC:00000168)"
    let createAssayIdentifierKey = sprintf "%s %s" AssayIdentifierPrefix// TODO: Replace with ISA ontology term for assay

    let setSourceAssayComment (person : Person) (assayIdentifier: string): Person =
        let person = person.Copy()
        let k = createAssayIdentifierKey assayIdentifier
        let comment = Comment(k, assayIdentifier)
        person.Comments.Add(comment)
        person

    /// <summary>
    /// This functions helps encoding/decoding ISA-JSON. It returns a sequence of ArcAssay-Identifiers.
    /// </summary>
    /// <param name="person"></param>
    let getSourceAssayIdentifiersFromComments (person : Person) =
        person.Comments 
        |> Seq.choose (fun c -> 
            let isAssaySource = 
                c.Name 
                |> Option.map (fun n -> 
                    n.StartsWith AssayIdentifierPrefix
                )
                |> Option.defaultValue false
            if isAssaySource then c.Value else None
        )

    let removeSourceAssayComments (person: Person) : ResizeArray<Comment> =
        person.Comments |> ResizeArray.filter (fun c -> c.Name.IsSome && c.Name.Value.StartsWith AssayIdentifierPrefix |> not)

    let setOrcidFromComments (person : Person) =
        let person = person.Copy()
        let isOrcidComment (c : Comment) = 
            c.Name.IsSome && (c.Name.Value.ToUpper().EndsWith(orcidKey))
        let orcid,comments = 
            let orcid = 
                person.Comments
                |> Seq.tryPick (fun c -> if isOrcidComment c then c.Value else None)
            let comments = 
                person.Comments
                |> ResizeArray.filter (isOrcidComment >> not)
            orcid, comments
        person.ORCID <- orcid
        person.Comments <- comments
        person

    let setCommentFromORCID (person : Person) =
        let person = person.Copy()
        match person.ORCID with
        | Some orcid -> 
            let comment = Comment.create (name = orcidKey, value = orcid)
            person.Comments.Add comment
        | None -> ()
        person

/// Functions for transforming base level ARC Table and ISA Json Objects
module JsonTypes = 

    /// Convert a CompositeCell to a ISA Value and Unit tuple.
    let valueOfCell (value : CompositeCell) =
        match value with
        | CompositeCell.FreeText ("") -> None, None
        | CompositeCell.FreeText (text) -> Some (Value.fromString text), None        
        | CompositeCell.Term (term) when term.isEmpty() -> None, None
        | CompositeCell.Term (term) -> Some (Value.Ontology term), None
        | CompositeCell.Unitized (text,unit) -> 
            (if text = "" then None else Value.fromString text |> Some), 
            if unit.isEmpty() then None else unit |> Some
        | CompositeCell.Data (data) -> failwith "Data cell should not be parsed to isa value"

    /// Convert a CompositeHeader and Cell tuple to a ISA Component
    let composeComponent (header : CompositeHeader) (value : CompositeCell) : Component =
        let v,u = valueOfCell value
        Component.create (?value = v,?unit = u,componentType = header.ToTerm())

    /// Convert a CompositeHeader and Cell tuple to a ISA ProcessParameterValue
    let composeParameterValue (header : CompositeHeader) (value : CompositeCell) : ProcessParameterValue =
        let v,u = valueOfCell value
        let p = ProtocolParameter.create(ParameterName = header.ToTerm())
        ProcessParameterValue.create(p,?Value = v,?Unit = u)

    /// Convert a CompositeHeader and Cell tuple to a ISA FactorValue
    let composeFactorValue (header : CompositeHeader) (value : CompositeCell) : FactorValue =
        let v,u = valueOfCell value
        let f = Factor.create(Name = header.ToString(),FactorType = header.ToTerm())
        FactorValue.create(Category = f,?Value = v,?Unit = u)

    /// Convert a CompositeHeader and Cell tuple to a ISA MaterialAttributeValue
    let composeCharacteristicValue (header : CompositeHeader) (value : CompositeCell) : MaterialAttributeValue =
        let v,u = valueOfCell value
        let c = MaterialAttribute.create(CharacteristicType = header.ToTerm())
        MaterialAttributeValue.create(Category = c,?Value = v,?Unit = u)

    let composeFreetextMaterialName (headerFT : string) (name : string) =
        $"{headerFT}={name}"
        

    /// Convert a CompositeHeader and Cell tuple to a ISA ProcessInput
    let composeProcessInput (header : CompositeHeader) (value : CompositeCell) : ProcessInput =
        match header with
        | CompositeHeader.Input IOType.Source -> ProcessInput.createSource(value.ToString())
        | CompositeHeader.Input IOType.Sample -> ProcessInput.createSample(value.ToString())
        | CompositeHeader.Input IOType.Material -> ProcessInput.createMaterial(value.ToString())
        | CompositeHeader.Input IOType.Data -> ProcessInput.createRawData(value.ToString())
        | CompositeHeader.Input (IOType.FreeText ft) ->
            ProcessInput.createMaterial(composeFreetextMaterialName ft (value.ToString()))
        | _ ->
            failwithf "Could not parse input header %O" header


    /// Convert a CompositeHeader and Cell tuple to a ISA ProcessOutput
    let composeProcessOutput (header : CompositeHeader) (value : CompositeCell) : ProcessOutput =
        match header with
        | CompositeHeader.Output IOType.Source
        | CompositeHeader.Output IOType.Sample -> ProcessOutput.createSample(value.ToString())       
        | CompositeHeader.Output IOType.Material -> ProcessOutput.createMaterial(value.ToString())
        | CompositeHeader.Output IOType.Data -> ProcessOutput.createRawData(value.ToString())
        | CompositeHeader.Output (IOType.FreeText ft) ->
            ProcessOutput.createMaterial(composeFreetextMaterialName ft (value.ToString()))
        | _ ->
            failwithf "Could not parse output header %O" header

    /// Convert an ISA Value and Unit tuple to a CompositeCell
    let cellOfValue (value : Value option) (unit : OntologyAnnotation option) =
        let value = value |> Option.defaultValue (Value.Name "")
        match value,unit with
        | Value.Ontology oa, None -> CompositeCell.Term oa
        | Value.Name "", None -> CompositeCell.Term (OntologyAnnotation())
        | Value.Name text, None -> CompositeCell.Term (OntologyAnnotation(text))
        | Value.Name name, Some u -> CompositeCell.Unitized (name,u)
        | Value.Float f, Some u -> CompositeCell.Unitized (f.ToString(),u)
        | Value.Float f, None -> CompositeCell.Unitized (f.ToString(),OntologyAnnotation())
        | Value.Int i, Some u -> CompositeCell.Unitized (i.ToString(),u)
        | Value.Int i, None -> CompositeCell.Unitized (i.ToString(),OntologyAnnotation())
        | _ -> failwithf "Could not parse value %O with unit %O" value unit

    /// Convert an ISA Component to a CompositeHeader and Cell tuple
    let decomposeComponent (c : Component) : CompositeHeader*CompositeCell =
        let header = CompositeHeader.Component (c.ComponentType.Value)
        let bodyCell = cellOfValue c.ComponentValue c.ComponentUnit 
        header, bodyCell

    /// Convert an ISA ProcessParameterValue to a CompositeHeader and Cell tuple
    let decomposeParameterValue (ppv : ProcessParameterValue) : CompositeHeader*CompositeCell =
        CompositeHeader.Parameter (ppv.Category.Value.ParameterName.Value),
        cellOfValue ppv.Value ppv.Unit

    /// Convert an ISA FactorValue to a CompositeHeader and Cell tuple
    let decomposeFactorValue (fv : FactorValue) : CompositeHeader*CompositeCell =
        CompositeHeader.Factor (fv.Category.Value.FactorType.Value),
        cellOfValue fv.Value fv.Unit

    /// Convert an ISA MaterialAttributeValue to a CompositeHeader and Cell tuple
    let decomposeCharacteristicValue (cv : MaterialAttributeValue) : CompositeHeader*CompositeCell =
        CompositeHeader.Characteristic (cv.Category.Value.CharacteristicType.Value),
        cellOfValue cv.Value cv.Unit
    
    /// Convert an ISA ProcessOutput to a CompositeHeader and Cell tuple
    let decomposeProcessInput (pi : ProcessInput) : CompositeHeader*CompositeCell =
        match pi with
        | ProcessInput.Source s -> CompositeHeader.Input IOType.Source, CompositeCell.FreeText (s.Name |> Option.defaultValue "")
        | ProcessInput.Sample s -> CompositeHeader.Input IOType.Sample, CompositeCell.FreeText (s.Name |> Option.defaultValue "")
        | ProcessInput.Material m -> CompositeHeader.Input IOType.Material, CompositeCell.FreeText (m.Name |> Option.defaultValue "")
        | ProcessInput.Data d -> 
            let dataType = d.DataType.Value
            match dataType with
            | DataFile.ImageFile -> CompositeHeader.Input IOType.Data, CompositeCell.FreeText (d.Name |> Option.defaultValue "")
            | DataFile.RawDataFile -> CompositeHeader.Input IOType.Data, CompositeCell.FreeText (d.Name |> Option.defaultValue "")
            | DataFile.DerivedDataFile -> CompositeHeader.Input IOType.Data, CompositeCell.FreeText (d.Name |> Option.defaultValue "")

    /// Convert an ISA ProcessOutput to a CompositeHeader and Cell tuple
    let decomposeProcessOutput (po : ProcessOutput) : CompositeHeader*CompositeCell =
        match po with
        | ProcessOutput.Sample s -> CompositeHeader.Output IOType.Sample, CompositeCell.FreeText (s.Name |> Option.defaultValue "")
        | ProcessOutput.Material m -> CompositeHeader.Output IOType.Material, CompositeCell.FreeText (m.Name |> Option.defaultValue "")
        | ProcessOutput.Data d -> 
            let dataType = d.DataType.Value
            match dataType with
            | DataFile.ImageFile -> CompositeHeader.Output IOType.Data, CompositeCell.FreeText (d.Name |> Option.defaultValue "")
            | DataFile.RawDataFile -> CompositeHeader.Output IOType.Data, CompositeCell.FreeText (d.Name |> Option.defaultValue "")
            | DataFile.DerivedDataFile -> CompositeHeader.Output IOType.Data, CompositeCell.FreeText (d.Name |> Option.defaultValue "")

            
    /// This function creates a string containing the name and the ontology short-string of the given ontology annotation term
    ///
    /// TechnologyPlatforms are plain strings in ISA-JSON.
    ///
    /// This function allows us, to parse them as an ontology term.
    let composeTechnologyPlatform (tp : OntologyAnnotation) = 
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
    let decomposeTechnologyPlatform (name : string) = 
        let pattern = """^(?<value>.+) \((?<ontology>[^(]*:[^)]*)\)$"""        

        match name with 
        | Regex.ActivePatterns.Regex pattern r -> 
            let oa = (r.Groups.Item "ontology").Value   |> OntologyAnnotation.fromTermAnnotation 
            let v =  (r.Groups.Item "value").Value
            OntologyAnnotation.create(name = v, ?tan = oa.TermAccessionNumber, ?tsr = oa.TermSourceREF)
        | _ ->
            OntologyAnnotation.create(name = name)


/// Functions for parsing ArcTables to ISA json Processes and vice versa
module ProcessParsing = 
 
    // Explanation of the Getter logic:
    // The getter logic is used to treat every value of the table only once
    // First, the headers are checked for what getter applies to the respective column. E.g. a ProtocolType getter will only return a function for parsing protocolType cells if the header depicts a protocolType.
    // The appropriate getters are then applied in the context of the processGetter, parsing the cells of the matrix

    /// If the given headers depict a component, returns a function for parsing the values of the matrix to the values of this component
    let tryComponentGetter (generalI : int) (valueI : int) (valueHeader : CompositeHeader) =
        match valueHeader with
        | CompositeHeader.Component oa ->
            let newOA = oa.Copy()
            newOA.SetColumnIndex valueI
            let cat = CompositeHeader.Component newOA
            fun (table : ArcTable) i ->
                let cell = 
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader cat None               
                JsonTypes.composeComponent cat cell
            |> Some
        | _ -> None    
            
    /// If the given headers depict a parameter, returns a function for parsing the values of the matrix to the values of this type
    let tryParameterGetter (generalI : int) (valueI : int) (valueHeader : CompositeHeader) =
        match valueHeader with
        | CompositeHeader.Parameter oa ->
            let cat = 
                OntologyAnnotation.setColumnIndex valueI oa 
                |> CompositeHeader.Parameter
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader cat None
                JsonTypes.composeParameterValue cat cell
            |> Some
        | _ -> None 

    /// If the given headers depict a factor, returns a function for parsing the values of the matrix to the values of this type
    let tryFactorGetter (generalI : int) (valueI : int) (valueHeader : CompositeHeader) =
        match valueHeader with
        | CompositeHeader.Factor oa ->
            let cat = 
                OntologyAnnotation.setColumnIndex valueI oa
                |> CompositeHeader.Factor
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader cat None
                JsonTypes.composeFactorValue cat cell
            |> Some
        | _ -> None 

    /// If the given headers depict a protocolType, returns a function for parsing the values of the matrix to the values of this type
    let tryCharacteristicGetter (generalI : int) (valueI : int) (valueHeader : CompositeHeader) =
        match valueHeader with
        | CompositeHeader.Characteristic oa ->
            let cat = 
                OntologyAnnotation.setColumnIndex valueI oa
                |> CompositeHeader.Characteristic              
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader cat None
                JsonTypes.composeCharacteristicValue cat cell
            |> Some
        | _ -> None 

    /// If the given headers depict a protocolType, returns a function for parsing the values of the matrix to the values of this type
    let tryGetProtocolTypeGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.ProtocolType ->
            fun (table : ArcTable) i ->               
                match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                | Some cell -> cell.AsTerm
                | None -> OntologyAnnotation()
            |> Some
        | _ -> None 


    /// If the given headers depict a protocolREF, returns a function for parsing the values of the matrix to the values of this type
    let tryGetProtocolREFGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.ProtocolREF ->
            fun (table : ArcTable) i ->             
                match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                | Some cell -> cell.AsFreeText
                | None -> ""
            |> Some
        | _ -> None

    /// If the given headers depict a protocolDescription, returns a function for parsing the values of the matrix to the values of this type
    let tryGetProtocolDescriptionGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.ProtocolDescription ->
            fun (table : ArcTable) i ->
                match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                | Some cell -> cell.AsFreeText
                | None -> ""
            |> Some
        | _ -> None
   
    /// If the given headers depict a protocolURI, returns a function for parsing the values of the matrix to the values of this type
    let tryGetProtocolURIGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.ProtocolUri ->
            fun (table : ArcTable) i ->
                match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                | Some cell -> cell.AsFreeText
                | None -> ""
            |> Some
        | _ -> None

    /// If the given headers depict a protocolVersion, returns a function for parsing the values of the matrix to the values of this type
    let tryGetProtocolVersionGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.ProtocolVersion ->
            fun (table : ArcTable) i ->
                match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                | Some cell -> cell.AsFreeText
                | None -> ""
            |> Some
        | _ -> None

    /// If the given headers depict an input, returns a function for parsing the values of the matrix to the values of this type
    let tryGetInputGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.Input io ->
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader header None
                JsonTypes.composeProcessInput header cell
            |> Some
        | _ -> None

    /// If the given headers depict an output, returns a function for parsing the values of the matrix to the values of this type
    let tryGetOutputGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.Output io ->
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader header None
                JsonTypes.composeProcessOutput header cell
            |> Some
        | _ -> None

    /// If the given headers depict a comment, returns a function for parsing the values of the matrix to the values of this type
    let tryGetCommentGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.Comment c ->
            fun (table : ArcTable) i ->           
                match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                | Some cell -> Comment(c,cell.AsFreeText)
                | None -> Comment(c)
            |> Some
        | _ -> None

    /// Given the header sequence of an ArcTable, returns a function for parsing each row of the table to a process
    let getProcessGetter (processNameRoot : string) (headers : CompositeHeader seq) =
    
        let headers = 
            headers
            |> Seq.indexed

        let valueHeaders =
            headers
            |> Seq.filter (snd >> fun h -> h.IsCvParamColumn)
            |> Seq.indexed
            |> Seq.toList

        let charGetters =
            valueHeaders 
            |> List.choose (fun (valueI,(generalI,header)) -> tryCharacteristicGetter generalI valueI header)

        let factorValueGetters =
            valueHeaders
            |> List.choose (fun (valueI,(generalI,header)) -> tryFactorGetter generalI valueI header)

        let parameterValueGetters =
            valueHeaders
            |> List.choose (fun (valueI,(generalI,header)) -> tryParameterGetter generalI valueI header)

        let componentGetters =
            valueHeaders
            |> List.choose (fun (valueI,(generalI,header)) -> tryComponentGetter generalI valueI header)

        let protocolTypeGetter = 
            headers
            |> Seq.tryPick (fun (generalI,header) -> tryGetProtocolTypeGetter generalI header)

        let protocolREFGetter = 
            headers
            |> Seq.tryPick (fun (generalI,header) -> tryGetProtocolREFGetter generalI header)

        let protocolDescriptionGetter = 
            headers
            |> Seq.tryPick (fun (generalI,header) -> tryGetProtocolDescriptionGetter generalI header)

        let protocolURIGetter = 
            headers
            |> Seq.tryPick (fun (generalI,header) -> tryGetProtocolURIGetter generalI header)

        let protocolVersionGetter =
            headers
            |> Seq.tryPick (fun (generalI,header) -> tryGetProtocolVersionGetter generalI header)

        let commentGetters = 
            headers
            |> Seq.choose (fun (generalI,header) -> tryGetCommentGetter generalI header)
            |> Seq.toList

        // This is a little more complex, as data and material objects can't contain characteristics. So in the case where the input of the table is a data object but characteristics exist. An additional sample object with the same name is created to contain the characteristics.
        let inputGetter =
            match headers |> Seq.tryPick (fun (generalI,header) -> tryGetInputGetter generalI header) with
            | Some inputGetter ->
                fun (table : ArcTable) i ->
                    let chars = charGetters |> Seq.map (fun f -> f table i) |> Seq.toList
                    let input = inputGetter table i

                    if ((input.isSample() || input.isSource())|> not) && (chars.IsEmpty |> not) then
                        [
                        input
                        ProcessInput.createSample(input.Name, characteristics = chars)
                        ]
                    else
                        if chars.Length > 0 then
                            input
                            |> ProcessInput.setCharacteristicValues chars
                        else
                            input                           
                        |> List.singleton
            | None ->
                fun (table : ArcTable) i ->
                    let chars = charGetters |> Seq.map (fun f -> f table i) |> Seq.toList
                    ProcessInput.Source (Source.create(Name = $"{processNameRoot}_Input_{i}", Characteristics = chars))
                    |> List.singleton
            
        // This is a little more complex, as data and material objects can't contain factors. So in the case where the output of the table is a data object but factors exist. An additional sample object with the same name is created to contain the factors.
        let outputGetter =
            match headers |> Seq.tryPick (fun (generalI,header) -> tryGetOutputGetter generalI header) with
            | Some outputGetter ->
                fun (table : ArcTable) i ->
                    let factors = factorValueGetters |> Seq.map (fun f -> f table i) |> Seq.toList
                    let output = outputGetter table i
                    if (output.isSample() |> not) && (factors.IsEmpty |> not) then
                        [
                        output
                        ProcessOutput.createSample(output.Name, factors = factors)
                        ]
                    else
                        if factors.Length > 0 then
                            output
                            |> ProcessOutput.setFactorValues factors
                        else
                            output
                        |> List.singleton
            | None ->
                fun (table : ArcTable) i ->
                    let factors = factorValueGetters |> Seq.map (fun f -> f table i) |> Seq.toList
                    ProcessOutput.Sample (Sample.create(Name = $"{processNameRoot}_Output_{i}", FactorValues = factors))
                    |> List.singleton

        fun (table : ArcTable) i ->

            let pn = processNameRoot |> Option.fromValueWithDefault "" |> Option.map (fun p -> Process.composeName p i)

            let paramvalues = parameterValueGetters |> List.map (fun f -> f table i) |> Option.fromValueWithDefault [] 
            let parameters = paramvalues |> Option.map (List.map (fun pv -> pv.Category.Value))

            let comments = commentGetters |> List.map (fun f -> f table i) |> Option.fromValueWithDefault []

            let protocol : Protocol option = 
                Protocol.make 
                    None
                    (protocolREFGetter |> Option.map (fun f -> f table i))
                    (protocolTypeGetter |> Option.map (fun f -> f table i))
                    (protocolDescriptionGetter |> Option.map (fun f -> f table i))
                    (protocolURIGetter |> Option.map (fun f -> f table i))
                    (protocolVersionGetter |> Option.map (fun f -> f table i))
                    (parameters)
                    (componentGetters |> List.map (fun f -> f table i) |> Option.fromValueWithDefault [])
                    None
                |> fun p ->     
                    match p with
                    | {       
                            Name            = None
                            ProtocolType    = None
                            Description     = None
                            Uri             = None
                            Version         = None
                            Components      = None
                        } -> None
                    | _ -> Some p

            let inputs,outputs = 
                let inputs = inputGetter table i
                let outputs = outputGetter table i
                if inputs.Length = 1 && outputs.Length = 2 then 
                    [inputs.[0];inputs.[0]],outputs
                elif inputs.Length = 2 && outputs.Length = 1 then
                    inputs,[outputs.[0];outputs.[0]]
                else
                    inputs,outputs

            Process.make 
                None 
                pn 
                (protocol) 
                (paramvalues)
                None
                None
                None
                None          
                (Some inputs)
                (Some outputs)
                comments

    /// Groups processes by their name, or by the name of the protocol they execute
    ///
    /// Process names are taken from the Worksheet name and numbered: SheetName_1, SheetName_2, etc.
    /// 
    /// This function decomposes this name into a root name and a number, and groups processes by root name.
    let groupProcesses (ps : Process list) = 
        ps
        |> List.groupBy (fun x -> 
            if x.Name.IsSome && (x.Name.Value |> Process.decomposeName |> snd).IsSome then
                (x.Name.Value |> Process.decomposeName |> fst)
            elif x.ExecutesProtocol.IsSome && x.ExecutesProtocol.Value.Name.IsSome then
                x.ExecutesProtocol.Value.Name.Value
            // Removed this case in order to fix https://github.com/nfdi4plants/ARCtrl/issues/512
            // The problem is, that through this case table-names with underscores clash, e.g. "Table_MS" and "Table_RNASeq" would be grouped together.
            //elif x.Name.IsSome && x.Name.Value.Contains "_" then
            //    let lastUnderScoreIndex = x.Name.Value.LastIndexOf '_'
            //    x.Name.Value.Remove lastUnderScoreIndex
            elif x.Name.IsSome then
                x.Name.Value
            elif x.ExecutesProtocol.IsSome && x.ExecutesProtocol.Value.ID.IsSome then 
                x.ExecutesProtocol.Value.ID.Value              
            else
                Identifier.createMissingIdentifier()        
        )

    /// Merges processes with the same name, protocol and param values
    let mergeIdenticalProcesses (processes : list<Process>) =
        processes
        |> List.groupBy (fun x -> 
            if x.Name.IsSome && (x.Name.Value |> Process.decomposeName |> snd).IsSome then
                (x.Name.Value |> Process.decomposeName |> fst), HashCodes.boxHashOption x.ExecutesProtocol, x.ParameterValues |> Option.map HashCodes.boxHashSeq, x.Comments |> Option.map HashCodes.boxHashSeq
            elif x.ExecutesProtocol.IsSome && x.ExecutesProtocol.Value.Name.IsSome then
                x.ExecutesProtocol.Value.Name.Value, HashCodes.boxHashOption x.ExecutesProtocol, x.ParameterValues |> Option.map HashCodes.boxHashSeq, x.Comments |> Option.map HashCodes.boxHashSeq
            else
                Identifier.createMissingIdentifier(), HashCodes.boxHashOption x.ExecutesProtocol, x.ParameterValues |> Option.map HashCodes.boxHashSeq, x.Comments |> Option.map HashCodes.boxHashSeq
        )
        |> fun l ->
            l
            |> List.mapi (fun i ((n,_,_,_),processes) -> 
                let pVs = processes.[0].ParameterValues
                let inputs = processes |> List.collect (fun p -> p.Inputs |> Option.defaultValue []) |> Option.fromValueWithDefault []
                let outputs = processes |> List.collect (fun p -> p.Outputs |> Option.defaultValue []) |> Option.fromValueWithDefault []
                let n = if l.Length > 1 then Process.composeName n i else n
                Process.create(Name = n,?ExecutesProtocol = processes.[0].ExecutesProtocol,?ParameterValues = pVs,?Inputs = inputs,?Outputs = outputs,?Comments = processes.[0].Comments)
            )


    // Transform a isa json process into a isa tab row, where each row is a header+value list
    let processToRows (p : Process) =
        let pvs = p.ParameterValues |> Option.defaultValue [] |> List.map (fun ppv -> JsonTypes.decomposeParameterValue ppv, ColumnIndex.tryGetParameterColumnIndex ppv)
        // Get the component
        let components = 
            match p.ExecutesProtocol with
            | Some prot ->
                prot.Components |> Option.defaultValue [] |> List.map (fun ppv -> JsonTypes.decomposeComponent ppv, ColumnIndex.tryGetComponentIndex ppv)
            | None -> []
        // Get the values of the protocol
        let protVals = 
            match p.ExecutesProtocol with
            | Some prot ->
                [
                    if prot.Name.IsSome then CompositeHeader.ProtocolREF, CompositeCell.FreeText prot.Name.Value
                    if prot.ProtocolType.IsSome then CompositeHeader.ProtocolType, CompositeCell.Term prot.ProtocolType.Value
                    if prot.Description.IsSome then CompositeHeader.ProtocolDescription, CompositeCell.FreeText prot.Description.Value
                    if prot.Uri.IsSome then CompositeHeader.ProtocolUri, CompositeCell.FreeText prot.Uri.Value
                    if prot.Version.IsSome then CompositeHeader.ProtocolVersion, CompositeCell.FreeText prot.Version.Value
                ]
            | None -> []
        let comments = 
            p.Comments 
            |> Option.defaultValue [] 
            |> List.map (fun c -> 
                CompositeHeader.Comment (Option.defaultValue "" c.Name),
                CompositeCell.FreeText (Option.defaultValue "" c.Value)
            )
        // zip the inputs and outpus so they are aligned as rows
        p.Outputs |> Option.defaultValue []
        |> List.zip (p.Inputs |> Option.defaultValue [])
        // This grouping here and the picking of the "inputForCharas" etc is done, so there can be rows where data do have characteristics, which is not possible in isa json
        |> List.groupBy (fun (i,o) ->
            i.Name,o.Name
        )
        |> List.map (fun ((i,o),ios) ->
            let inputForCharas = 
                ios
                |> List.tryPick (fun (i,o) -> if i.isSource() || i.isSample() then Some i else None)
                |> Option.defaultValue (ios.Head |> fst)
            let inputForType =
                ios
                |> List.tryPick (fun (i,o) -> if i.isData() || i.isMaterial() then Some i  else None)
                |> Option.defaultValue (ios.Head |> fst)
            let chars = 
                inputForCharas |> ProcessInput.getCharacteristicValues |> List.map (fun cv -> JsonTypes.decomposeCharacteristicValue cv, ColumnIndex.tryGetCharacteristicColumnIndex cv)
            let outputForFactors = 
                ios
                |> List.tryPick (fun (i,o) -> if o.isSample() then Some o else None)
                |> Option.defaultValue (ios.Head |> snd)
            let outputForType = 
                ios
                |> List.tryPick (fun (i,o) -> if o.isData() || o.isMaterial() then Some o else None)
                |> Option.defaultValue (ios.Head |> snd)
            let factors = outputForFactors |> ProcessOutput.getFactorValues |> List.map (fun fv -> JsonTypes.decomposeFactorValue fv, ColumnIndex.tryGetFactorColumnIndex fv)
            let vals = 
                (chars @ components @ pvs @ factors)
                |> List.sortBy (snd >> Option.defaultValue 10000)
                |> List.map fst
            [
                yield JsonTypes.decomposeProcessInput inputForType
                yield! protVals
                yield! vals
                yield! comments
                yield JsonTypes.decomposeProcessOutput outputForType
            ]
        )
  
type CompositeHeader with

    member this.TryParameter() = 
        match this with 
        | CompositeHeader.Parameter oa -> Some (ProtocolParameter.create(ParameterName = oa))
        | _ -> None

    member this.TryFactor() =
        match this with
        | CompositeHeader.Factor oa -> Some (Factor.create(FactorType = oa))
        | _ -> None

    member this.TryCharacteristic() =
        match this with
        | CompositeHeader.Characteristic oa -> Some (MaterialAttribute.create(CharacteristicType = oa))
        | _ -> None

    member this.TryComponent() =
        match this with
        | CompositeHeader.Component oa -> Some (Component.create(componentType = oa))
        | _ -> None

type CompositeCell with

    /// <summary>
    /// This function is used to improve interoperability with ISA-JSON types. It is not recommended for default ARCtrl usage.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="unit"></param>
    static member fromValue(value : Value, ?unit : OntologyAnnotation) =
        JsonTypes.cellOfValue (Some value) unit


module CompositeRow =

    let toProtocol (tableName : string) (row : (CompositeHeader*CompositeCell) seq) =
        row
        |> Seq.fold (fun p hc ->
            match hc with
            | CompositeHeader.ProtocolType, CompositeCell.Term oa -> 
                Protocol.setProtocolType p oa
            | CompositeHeader.ProtocolVersion, CompositeCell.FreeText v -> Protocol.setVersion p v
            | CompositeHeader.ProtocolUri, CompositeCell.FreeText v -> Protocol.setUri p v
            | CompositeHeader.ProtocolDescription, CompositeCell.FreeText v -> Protocol.setDescription p v
            | CompositeHeader.ProtocolREF, CompositeCell.FreeText v -> Protocol.setName p v
            | CompositeHeader.Parameter oa, _ -> 
                let pp = ProtocolParameter.create(ParameterName = oa)
                Protocol.addParameter (pp) p
            | CompositeHeader.Component oa, CompositeCell.Unitized(v,unit) -> 
                let c = Component.create(componentType = oa, value = Value.fromString v, unit = unit)
                Protocol.addComponent c p        
            | CompositeHeader.Component oa, CompositeCell.Term t -> 
                let c = Component.create(componentType = oa, value = Value.Ontology t)
                Protocol.addComponent c p     
            | _ -> p
        ) (Protocol.create(Name = tableName))

type ArcTable with

    /// Create a new table from an ISA protocol.
    ///
    /// The table will have at most one row, with the protocol information and the component values
    static member fromProtocol (p : Protocol) : ArcTable = 
        
        let t = ArcTable.init (p.Name |> Option.defaultValue (Identifier.createMissingIdentifier()))

        for pp in p.Parameters |> Option.defaultValue [] do

            //t.AddParameterColumn(pp, ?index = pp.TryGetColumnIndex())

            t.AddColumn(CompositeHeader.Parameter pp.ParameterName.Value, ?index = pp.TryGetColumnIndex())

        for c in p.Components |> Option.defaultValue [] do
            let v = c.ComponentValue |> Option.map ((fun v -> CompositeCell.fromValue(v,?unit = c.ComponentUnit)) >> ResizeArray.singleton)
            t.AddColumn(
                CompositeHeader.Parameter c.ComponentType.Value, 
                ?cells = v,
                ?index = c.TryGetColumnIndex())
        p.Description   |> Option.map (fun d -> t.AddProtocolDescriptionColumn(ResizeArray.singleton d))  |> ignore
        p.Version       |> Option.map (fun d -> t.AddProtocolVersionColumn(ResizeArray.singleton d))      |> ignore
        p.ProtocolType  |> Option.map (fun d -> t.AddProtocolTypeColumn(ResizeArray.singleton d))         |> ignore
        p.Uri           |> Option.map (fun d -> t.AddProtocolUriColumn(ResizeArray.singleton d))          |> ignore
        p.Name          |> Option.map (fun d -> t.AddProtocolNameColumn(ResizeArray.singleton d))         |> ignore
        t

    /// Returns the list of protocols executed in this ArcTable
    member this.GetProtocols() : Protocol list = 

        if this.RowCount = 0 then
            this.Headers
            |> Seq.fold (fun (p : Protocol) h -> 
                match h with
                | CompositeHeader.ProtocolType -> 
                    Protocol.setProtocolType p (OntologyAnnotation())
                | CompositeHeader.ProtocolVersion -> Protocol.setVersion p ""
                | CompositeHeader.ProtocolUri -> Protocol.setUri p ""
                | CompositeHeader.ProtocolDescription -> Protocol.setDescription p ""
                | CompositeHeader.ProtocolREF -> Protocol.setName p ""
                | CompositeHeader.Parameter oa -> 
                    let pp = ProtocolParameter.create(ParameterName = oa)
                    Protocol.addParameter (pp) p
                | CompositeHeader.Component oa -> 
                    let c = Component.create(componentType = oa)
                    Protocol.addComponent c p
                | _ -> p
            ) (Protocol.create(Name = this.Name))
            |> List.singleton
        else
            List.init this.RowCount (fun i ->
                this.GetRow(i, SkipValidation = true) 
                |> Seq.zip this.Headers
                |> CompositeRow.toProtocol this.Name                   
            )
            |> List.distinct

    /// Returns the list of processes specidified in this ArcTable
    member this.GetProcesses() : Process list = 
        if this.RowCount = 0 then 
            Process.create(Name = this.Name)
            |> List.singleton
        else
            let getter = ProcessParsing.getProcessGetter this.Name this.Headers          
            [
                for i in 0..this.RowCount-1 do
                    yield getter this i        
            ]
            |> ProcessParsing.mergeIdenticalProcesses


    /// Create a new table from a list of processes
    ///
    /// The name will be used as the sheet name
    /// 
    /// The processes SHOULD have the same headers, or even execute the same protocol
    static member fromProcesses name (ps : Process list) : ArcTable = 
        ps
        |> List.collect (fun p -> ProcessParsing.processToRows p)
        |> fun rows -> ArcTableAux.Unchecked.alignByHeaders true rows
        |> fun (headers, rows) -> ArcTable.fromArcTableValues(name,headers,rows)    

type ArcTables with

    /// Return a list of all the processes in all the tables.
    member this.GetProcesses() : Process list = 
        this.Tables
        |> Seq.toList
        |> List.collect (fun t -> t.GetProcesses())

    /// Create a collection of tables from a list of processes.
    ///
    /// For this, the processes are grouped by nameroot ("nameroot_1", "nameroot_2" ...) or exectued protocol if no name exists
    ///
    /// Then each group is converted to a table with this nameroot as sheetname
    static member fromProcesses (ps : Process list) : ArcTables = 
        ps
        |> ProcessParsing.groupProcesses
        //|> fun x -> printfn "fromProcesses 1"; x
        |> List.map (fun (name,ps) ->
            //printfn "fromProcesses-%s 0" name
            ps
            |> List.collect (fun p -> ProcessParsing.processToRows p)
            //|> fun x -> printfn "fromProcesses-%s 1" name; x
            |> fun rows -> ArcTableAux.Unchecked.alignByHeaders true rows
            //|> fun x -> printfn "fromProcesses-%s 2" name; x
            |> fun (headers, rows) -> ArcTable.fromArcTableValues(name,headers,rows)
            //|> fun x -> printfn "fromProcesses-%s 3" name; x
        )
        |> ResizeArray
        |> ArcTables


    ///// Copies ArcAssay object without the pointer to the parent ArcInvestigation
    /////
    ///// In order to copy the pointer to the parent ArcInvestigation as well, use the Copy() method of the ArcInvestigation instead.
    //member this.ToAssay() : Assay = 
    //    let processSeq = ArcTables(this.Tables).GetProcesses()
    //    let assayMaterials =
    //        AssayMaterials.create(
    //            ?Samples = (ProcessSequence.getSamples processSeq |> Option.fromValueWithDefault []),
    //            ?OtherMaterials = (ProcessSequence.getMaterials processSeq |> Option.fromValueWithDefault [])
    //        )
    //        |> Option.fromValueWithDefault AssayMaterials.empty
    //    let fileName = 
    //        if Identifier.isMissingIdentifier this.Identifier then
    //            None
    //        else 
    //            Some (Identifier.Assay.fileNameFromIdentifier this.Identifier)
    //    Assay.create(
    //        ?FileName = fileName,
    //        ?MeasurementType = this.MeasurementType,
    //        ?TechnologyType = this.TechnologyType,
    //        ?TechnologyPlatform = (this.TechnologyPlatform |> Option.map ArcAssay.composeTechnologyPlatform),
    //        ?DataFiles = (ProcessSequence.getData processSeq |> Option.fromValueWithDefault []),
    //        ?Materials = assayMaterials,
    //        ?CharacteristicCategories = (ProcessSequence.getCharacteristics processSeq |> Option.fromValueWithDefault []),
    //        ?UnitCategories = (ProcessSequence.getUnits processSeq |> Option.fromValueWithDefault []),
    //        ?ProcessSequence = (processSeq |> Option.fromValueWithDefault []),
    //        ?Comments = (this.Comments |> List.ofArray |> Option.fromValueWithDefault [])
    //        )

    //// Create an ArcAssay from an ISA Json Assay.
    //static member fromAssay (a : Assay) : ArcAssay = 
    //    let tables = (a.ProcessSequence |> Option.map (ArcTables.fromProcesses >> fun t -> t.Tables))
    //    let identifer = 
    //        match a.FileName with
    //        | Some fn -> Identifier.Assay.identifierFromFileName fn
    //        | None -> Identifier.createMissingIdentifier()
    //    ArcAssay.create(
    //        identifer,
    //        ?measurementType = (a.MeasurementType |> Option.map (fun x -> x.Copy())),
    //        ?technologyType = (a.TechnologyType |> Option.map (fun x -> x.Copy())),
    //        ?technologyPlatform = (a.TechnologyPlatform |> Option.map ArcAssay.decomposeTechnologyPlatform),
    //        ?tables = tables,
    //        ?comments = (a.Comments |> Option.map Array.ofList)
    //        )



    ///// <summary>
    ///// Creates an ISA-Json compatible Study from ArcStudy.
    ///// </summary>
    ///// <param name="arcAssays">If this parameter is given, will transform these ArcAssays to Assays and include them as children of the Study. If not, tries to get them from the parent ArcInvestigation instead. If ArcStudy has no parent ArcInvestigation either, initializes new ArcAssay from registered Identifiers.</param>
    //member this.ToStudy(?arcAssays: ResizeArray<ArcAssay>) : Study = 
    //    let processSeq = ArcTables(this.Tables).GetProcesses()
    //    let protocols = ProcessSequence.getProtocols processSeq |> Option.fromValueWithDefault []
    //    let studyMaterials =
    //        StudyMaterials.create(
    //            ?Sources = (ProcessSequence.getSources processSeq |> Option.fromValueWithDefault []),
    //            ?Samples = (ProcessSequence.getSamples processSeq |> Option.fromValueWithDefault []),
    //            ?OtherMaterials = (ProcessSequence.getMaterials processSeq |> Option.fromValueWithDefault [])
    //        )
    //        |> Option.fromValueWithDefault StudyMaterials.empty
    //    let identifier,fileName = 
    //        if Identifier.isMissingIdentifier this.Identifier then
    //            None, None
    //        else
    //            Some this.Identifier, Some (Identifier.Study.fileNameFromIdentifier this.Identifier)
    //    let assays = 
    //        arcAssays |> Option.defaultValue (this.GetRegisteredAssaysOrIdentifier())
    //        |> List.ofSeq |> List.map (fun a -> a.ToAssay())
    //    Study.create(
    //        ?FileName = fileName,
    //        ?Identifier = identifier,
    //        ?Title = this.Title,
    //        ?Description = this.Description,
    //        ?SubmissionDate = this.SubmissionDate,
    //        ?PublicReleaseDate = this.PublicReleaseDate,
    //        ?Publications = (this.Publications |> List.ofArray |> Option.fromValueWithDefault []),
    //        ?Contacts = (this.Contacts |> List.ofArray |> Option.fromValueWithDefault []),
    //        ?StudyDesignDescriptors = (this.StudyDesignDescriptors |> List.ofArray |> Option.fromValueWithDefault []),
    //        ?Protocols = protocols,
    //        ?Materials = studyMaterials,
    //        ?ProcessSequence = (processSeq |> Option.fromValueWithDefault []),
    //        ?Assays = (assays |> Option.fromValueWithDefault []),
    //        ?CharacteristicCategories = (ProcessSequence.getCharacteristics processSeq |> Option.fromValueWithDefault []),
    //        ?UnitCategories = (ProcessSequence.getUnits processSeq |> Option.fromValueWithDefault []),
    //        ?Comments = (this.Comments |> List.ofArray |> Option.fromValueWithDefault [])
    //        )

    //// Create an ArcStudy from an ISA Json Study.
    //static member fromStudy (s : Study) : (ArcStudy * ResizeArray<ArcAssay>) = 
    //    let tables = (s.ProcessSequence |> Option.map (ArcTables.fromProcesses >> fun t -> t.Tables))
    //    let identifer = 
    //        match s.FileName with
    //        | Some fn -> Identifier.Study.identifierFromFileName fn
    //        | None -> Identifier.createMissingIdentifier()
    //    let assays = s.Assays |> Option.map (List.map ArcAssay.fromAssay >> ResizeArray) |> Option.defaultValue (ResizeArray())
    //    let assaysIdentifiers = assays |> Seq.map (fun a -> a.Identifier) |> ResizeArray
    //    ArcStudy.create(
    //        identifer,
    //        ?title = s.Title,
    //        ?description = s.Description,
    //        ?submissionDate = s.SubmissionDate,
    //        ?publicReleaseDate = s.PublicReleaseDate,
    //        ?publications = (s.Publications |> Option.map Array.ofList),
    //        ?contacts = (s.Contacts|> Option.map Array.ofList),
    //        ?studyDesignDescriptors = (s.StudyDesignDescriptors |> Option.map Array.ofList),
    //        ?tables = tables,
    //        ?registeredAssayIdentifiers = Some assaysIdentifiers,
    //        ?comments = (s.Comments |> Option.map Array.ofList)
    //        ),
    //    assays


    ///// Transform an ArcInvestigation to an ISA Json Investigation.
    //member this.ToInvestigation() : Investigation = 
    //    let studies = this.RegisteredStudies |> Seq.toList |> List.map (fun a -> a.ToStudy()) |> Option.fromValueWithDefault []
    //    let identifier =
    //        if Identifier.isMissingIdentifier this.Identifier then None
    //        else Some this.Identifier
    //    Investigation.create(
    //        FileName = ARCtrl.Path.InvestigationFileName,
    //        ?Identifier = identifier,
    //        ?Title = this.Title,
    //        ?Description = this.Description,
    //        ?SubmissionDate = this.SubmissionDate,
    //        ?PublicReleaseDate = this.PublicReleaseDate,
    //        ?Publications = (this.Publications |> List.ofArray |> Option.fromValueWithDefault []),
    //        ?Contacts = (this.Contacts |> List.ofArray |> Option.fromValueWithDefault []),
    //        ?Studies = studies,
    //        ?Comments = (this.Comments |> List.ofArray |> Option.fromValueWithDefault [])
    //        )

    //// Create an ArcInvestigation from an ISA Json Investigation.
    //static member fromInvestigation (i : Investigation) : ArcInvestigation = 
    //    let identifer = 
    //        match i.Identifier with
    //        | Some i -> i
    //        | None -> Identifier.createMissingIdentifier()
    //    let studiesRaw, assaysRaw = 
    //        i.Studies 
    //        |> Option.defaultValue []
    //        |> List.map ArcStudy.fromStudy
    //        |> List.unzip
    //    let studies = ResizeArray(studiesRaw)
    //    let studyIdentifiers = studiesRaw |> Seq.map (fun a -> a.Identifier) |> ResizeArray
    //    let assays = assaysRaw |> Seq.concat |> Seq.distinctBy (fun a -> a.Identifier) |> ResizeArray
    //    let i = ArcInvestigation.create(
    //        identifer,
    //        ?title = i.Title,
    //        ?description = i.Description,
    //        ?submissionDate = i.SubmissionDate,
    //        ?publicReleaseDate = i.PublicReleaseDate,
    //        ?publications = (i.Publications |> Option.map Array.ofList),
    //        studies = studies,
    //        assays = assays,
    //        registeredStudyIdentifiers = studyIdentifiers,
    //        ?contacts = (i.Contacts |> Option.map Array.ofList),            
    //        ?comments = (i.Comments |> Option.map Array.ofList)
    //        )      
    //    i