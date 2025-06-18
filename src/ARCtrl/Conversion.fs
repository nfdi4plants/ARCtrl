namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open System.Collections.Generic
//open ColumnIndex

module DateTime =

    let tryFromString (s : string) =
        try Json.Decode.fromJsonString Json.Decode.datetime s |> Some
        with _ -> None

    let toString (d : System.DateTime) =
        Json.Encode.dateTime d
        |> Json.Encode.toJsonString 0

module ColumnIndex = 

    open ARCtrl

    let private tryInt (str:string) =
        match System.Int32.TryParse str with
        | true,int -> Some int
        | _ -> None

    let orderName = "columnIndex"

    let tryGetIndex (node : LDNode) =
        match node.TryGetPropertyAsSingleton(orderName) with
        | Some (:? string as ci) -> tryInt ci
        | _ -> None

    let setIndex (node : LDNode) (index : int) =
        node.SetProperty(orderName,(string index))

    [<AutoOpen>]
    module ColumnIndexExtensions = 
    
        type LDNode with

            member this.GetColumnIndex() = tryGetIndex this |> Option.get

            member this.TryGetColumnIndex() = tryGetIndex this

            member this.SetColumnIndex (index : int) = setIndex this index

/// Functions for transforming base level ARC Table and ISA Json Objects
type BaseTypes = 

    static member composeComment (comment : ARCtrl.Comment) =
        let name = match comment.Name with | Some n -> n | None -> failwith "Comment must have a name"
        LDComment.create(name = name, ?text = comment.Value)

    static member decomposeComment (comment : LDNode, ?context : LDContext) =
        let name = LDComment.getNameAsString(comment, ?context = context)
        let text = LDComment.tryGetTextAsString(comment, ?context = context)
        Comment(name = name,?value = text)

    static member ontologyTermFromNameAndID(?name : string, ?id : string) =
        match id with
        | Some t -> OntologyAnnotation.fromTermAnnotation(tan = t, ?name = name)
        | None -> OntologyAnnotation.create(?name = name)

    static member tryOntologyTermFromNameAndID(?name : string, ?id : string) =
        if name.IsNone && id.IsNone then
            None
        else
            BaseTypes.ontologyTermFromNameAndID(?name = name, ?id = id)
            |> Some

    static member composeDefinedTerm (term : OntologyAnnotation) =
        let tan = term.TermAccessionAndOntobeeUrlIfShort |> Option.fromValueWithDefault ""
        LDDefinedTerm.create(name = term.NameText, ?termCode = tan)

    static member decomposeDefinedTerm (term : LDNode, ?context : LDContext) =
        let name = LDDefinedTerm.getNameAsString(term, ?context = context)
        let id = LDDefinedTerm.tryGetTermCodeAsString(term, ?context = context)
        BaseTypes.ontologyTermFromNameAndID(name, ?id = id)

    static member composePropertyValueFromOA (term : OntologyAnnotation) =
        let tan = term.TermAccessionAndOntobeeUrlIfShort |> Option.fromValueWithDefault ""
        LDPropertyValue.create(name = term.NameText, ?propertyID = tan)

    static member decomposePropertyValueToOA (term : LDNode, ?context : LDContext) =
        let name = LDPropertyValue.getNameAsString(term, ?context = context)
        let id = LDPropertyValue.tryGetPropertyIDAsString(term, ?context = context)
        BaseTypes.ontologyTermFromNameAndID(name, ?id = id)

    /// Convert a CompositeCell to a ISA Value and Unit tuple.
    static member valuesOfCell (value : CompositeCell) =
        match value with
        | CompositeCell.FreeText ("") -> None, None, None, None
        | CompositeCell.FreeText (text) -> Some text, None, None, None        
        | CompositeCell.Term (term) when term.isEmpty() -> None, None, None, None
        | CompositeCell.Term (term) when term.TANInfo.IsSome -> term.Name, Some term.TermAccessionAndOntobeeUrlIfShort, None, None
        | CompositeCell.Term (term) -> term.Name, None, None, None
        | CompositeCell.Unitized (text,unit) ->
            let unitName, unitAccession = if unit.isEmpty() then None, None else unit.Name, unit.TermAccessionAndOntobeeUrlIfShort |> Option.fromValueWithDefault ""
            Option.fromValueWithDefault "" text,
            None,
            unitName,
            unitAccession          
        | CompositeCell.Data (data) -> failwith "Data cell should not be parsed to isa value"

    static member termOfHeader (header : CompositeHeader) =
        match header with
        | CompositeHeader.Component oa 
        | CompositeHeader.Parameter oa 
        | CompositeHeader.Factor oa 
        | CompositeHeader.Characteristic oa ->
            oa.NameText, if oa.TANInfo.IsSome then Some oa.TermAccessionAndOntobeeUrlIfShort else None
        | h -> failwithf "header %O should not be parsed to isa value" h

    /// Convert a CompositeHeader and Cell tuple to a ISA Component
    static member composeComponent (header : CompositeHeader) (value : CompositeCell) : LDNode =
        let v,va,u,ua = BaseTypes.valuesOfCell value
        let n, na = BaseTypes.termOfHeader header
        LDPropertyValue.createComponent(n, ?value = v, ?propertyID = na, ?valueReference = va, ?unitCode = ua, ?unitText = u)

    /// Convert a CompositeHeader and Cell tuple to a ISA ProcessParameterValue
    static member composeParameterValue (header : CompositeHeader) (value : CompositeCell) : LDNode =
        let v,va,u,ua = BaseTypes.valuesOfCell value
        let n, na = BaseTypes.termOfHeader header
        LDPropertyValue.createParameterValue(n, ?value = v, ?propertyID = na, ?valueReference = va, ?unitCode = ua, ?unitText = u)

    /// Convert a CompositeHeader and Cell tuple to a ISA FactorValue
    static member composeFactorValue (header : CompositeHeader) (value : CompositeCell) : LDNode =
        let v,va,u,ua = BaseTypes.valuesOfCell value
        let n, na = BaseTypes.termOfHeader header
        LDPropertyValue.createFactorValue(n, ?value = v, ?propertyID = na, ?valueReference = va, ?unitCode = ua, ?unitText = u)

    /// Convert a CompositeHeader and Cell tuple to a ISA MaterialAttributeValue
    static member composeCharacteristicValue (header : CompositeHeader) (value : CompositeCell) : LDNode  =
        let v,va,u,ua = BaseTypes.valuesOfCell value
        let n, na = BaseTypes.termOfHeader header
        LDPropertyValue.createCharacteristicValue(n, ?value = v, ?propertyID = na, ?valueReference = va, ?unitCode = ua, ?unitText = u)

    static member composeFreetextMaterialName (headerFT : string) (name : string) =
        $"{headerFT}={name}"
        

    static member composeFile (d : Data) : LDNode =
        let dataType = d.DataType |> Option.map (fun dt -> dt.AsString) 
        LDFile.create(d.NameText,d.NameText,?disambiguatingDescription = dataType, ?encodingFormat = d.Format, ?usageInfo = d.SelectorFormat)

    static member decomposeFile (f : LDNode, ?context : LDContext) : Data =
        let dataType = LDFile.tryGetDisambiguatingDescriptionAsString(f, ?context = context) |> Option.map DataFile.fromString
        let format = LDFile.tryGetEncodingFormatAsString(f, ?context = context)
        let selectorFormat = LDFile.tryGetUsageInfoAsString(f, ?context = context)
        let data = Data((*id = f.Id, *)name = LDFile.getNameAsString(f, ?context = context), ?dataType = dataType, ?format = format, ?selectorFormat = selectorFormat)
        data


    static member composeFragmentDescriptor (dc : DataContext) =
        if dc.Name.IsNone then failwith "RO-Crate parsing of DataContext failed: Cannot create a fragment descriptor without a name."
        let id = LDPropertyValue.genIdFragmentDescriptor(dc.NameText)
        let explicationName, explicationID =
            dc.Explication
            |> Option.map (fun e -> e.Name, e.TermAccessionAndOntobeeUrlIfShort |> Option.fromValueWithDefault "")
            |> Option.defaultValue (None, None)
        let unitName, unitID =
            dc.Unit
            |> Option.map (fun u -> u.Name, u.TermAccessionAndOntobeeUrlIfShort |> Option.fromValueWithDefault "")
            |> Option.defaultValue (None, None)
        let disambiguatingDescriptions = dc.Comments |> ResizeArray.map (fun c -> c.ToString()) |> Option.fromSeq
        let dataFragment = BaseTypes.composeFile(dc)
        let pattern = dc.ObjectType |> Option.map (BaseTypes.composeDefinedTerm)
        dataFragment.SetProperty(LDFile.about, LDRef(id))
        dataFragment.SetOptionalProperty(LDFile.pattern, pattern)
        LDPropertyValue.createFragmentDescriptor(
            dc.NameText,
            ?value = explicationName,
            ?valueReference = explicationID,
            ?unitText = unitName,
            ?unitCode = unitID,
            ?measurementMethod = dc.GeneratedBy,
            ?description = dc.Description,
            ?alternateName = dc.Label,
            ?disambiguatingDescriptions = disambiguatingDescriptions,
            subjectOf = dataFragment
        )

    static member decomposeFragmentDescriptor (fd : LDNode, ?graph : LDGraph, ?context : LDContext)  =
        let file = LDPropertyValue.tryGetSubjectOf(fd, ?graph = graph, ?context = context)
        let name = match file with | Some f -> LDFile.getNameAsString(f, ?context = context) | None -> failwith "RO-Crate parsing of DataContext failed: Cannot decompose a fragment descriptor without a name."
        let objectType =
            file
            |> Option.bind (fun f -> LDFile.tryGetPatternAsDefinedTerm(f, ?graph = graph, ?context = context))
            |> Option.map (fun pa -> BaseTypes.decomposeDefinedTerm(pa, ?context = context))
        let format =
            file
            |> Option.bind (fun f -> LDFile.tryGetEncodingFormatAsString(f, ?context = context))
        let selectorFormat =
            file
            |> Option.bind (fun f -> LDFile.tryGetUsageInfoAsString(f, ?context = context))
        let explicationName = LDPropertyValue.tryGetValueAsString(fd)
        let explicationID = LDPropertyValue.tryGetValueReferenceAsString(fd)
        let explication = BaseTypes.tryOntologyTermFromNameAndID(?name = explicationName, ?id = explicationID)
        let unitName = LDPropertyValue.tryGetUnitTextAsString(fd)
        let unitID = LDPropertyValue.tryGetUnitCodeAsString(fd)
        let unit = BaseTypes.tryOntologyTermFromNameAndID(?name = unitName, ?id = unitID)
        let generatedBy = LDPropertyValue.tryGetMeasurementMethodAsString(fd)
        let description = LDPropertyValue.tryGetDescriptionAsString(fd)
        let label = LDPropertyValue.tryGetAlternateNameAsString(fd)
        let comments = LDPropertyValue.getDisambiguatingDescriptionsAsString(fd) |> ResizeArray.map Comment.fromString
        DataContext(name = name, ?format = format, ?selectorFormat = selectorFormat, ?explication = explication, ?unit = unit, ?objectType = objectType, ?generatedBy = generatedBy, ?description = description, ?label = label, comments = comments)

    /// Convert a CompositeHeader and Cell tuple to a ISA ProcessInput
    static member composeProcessInput (header : CompositeHeader) (value : CompositeCell) : LDNode =
        match header with
        | CompositeHeader.Input IOType.Source -> LDSample.createSource(value.AsFreeText)
        | CompositeHeader.Input IOType.Sample -> LDSample.createSample(value.AsFreeText)
        | CompositeHeader.Input IOType.Material -> LDSample.createMaterial(value.AsFreeText)
        | CompositeHeader.Input IOType.Data ->
            match value with
            | CompositeCell.FreeText ft ->
                LDFile.create(ft,ft)
            | CompositeCell.Data od ->
                BaseTypes.composeFile od
            | _ -> failwithf "Could not parse input data %O" value
        | CompositeHeader.Input (IOType.FreeText ft) ->
            let n = LDNode(id = BaseTypes.composeFreetextMaterialName ft value.AsFreeText, schemaType = ResizeArray [ft])
            n.SetProperty(LDSample.name, value.AsFreeText)
            n
        | _ ->
            failwithf "Could not parse input header %O" header


    /// Convert a CompositeHeader and Cell tuple to a ISA ProcessOutput
    static member composeProcessOutput (header : CompositeHeader) (value : CompositeCell) : LDNode =
        match header with
        | CompositeHeader.Output IOType.Source 
        | CompositeHeader.Output IOType.Sample -> LDSample.createSample(value.AsFreeText)
        | CompositeHeader.Output IOType.Material -> LDSample.createMaterial(value.AsFreeText)
        | CompositeHeader.Output IOType.Data ->
            match value with
            | CompositeCell.FreeText ft ->
                LDFile.create(ft,ft)
            | CompositeCell.Data od ->
                BaseTypes.composeFile od
            | _ -> failwithf "Could not parse output data %O" value
        | CompositeHeader.Output (IOType.FreeText ft) ->
            let n = LDNode(id = BaseTypes.composeFreetextMaterialName ft value.AsFreeText, schemaType = ResizeArray [ft])
            n.SetProperty(LDSample.name, value.AsFreeText)
            n
        | _ -> failwithf "Could not parse output header %O" header

    static member headerOntologyOfPropertyValue (pv : LDNode, ?context : LDContext) =
        let n = LDPropertyValue.getNameAsString(pv, ?context = context)
        match LDPropertyValue.tryGetPropertyIDAsString(pv, ?context = context) with
        | Some nRef -> OntologyAnnotation.fromTermAnnotation(tan = nRef, name = n)
        | None -> OntologyAnnotation(name = n)

    /// Convert an ISA Value and Unit tuple to a CompositeCell
    static member cellOfPropertyValue (pv : LDNode, ?context : LDContext) =
        let v = LDPropertyValue.tryGetValueAsString(pv, ?context = context)
        let vRef = LDPropertyValue.tryGetValueReferenceAsString(pv, ?context = context)
        let u = LDPropertyValue.tryGetUnitTextAsString(pv, ?context = context)
        let uRef = LDPropertyValue.tryGetUnitCodeAsString(pv, ?context = context)
        match vRef,u,uRef with
        | Some vr, None, None ->
            CompositeCell.Term (OntologyAnnotation.fromTermAnnotation(vr,?name = v))
        | None, Some u, None ->
            CompositeCell.Unitized ((Option.defaultValue "" v),OntologyAnnotation(name = u))
        | None, _, Some uRef ->
            CompositeCell.Unitized ((Option.defaultValue "" v),OntologyAnnotation.fromTermAnnotation(uRef, ?name = u))
        | None, None, None ->
            CompositeCell.Term (OntologyAnnotation(?name = v))
        | _ ->
            failwithf "Could not parse value %s with unit %O and unit reference %O" (Option.defaultValue "" v) u uRef

    /// Convert an ISA Component to a CompositeHeader and Cell tuple
    static member decomposeComponent (c : LDNode, ?context : LDContext) : CompositeHeader*CompositeCell =
        let header = BaseTypes.headerOntologyOfPropertyValue(c, ?context = context) |> CompositeHeader.Component
        let bodyCell = BaseTypes.cellOfPropertyValue (c, ?context = context)
        header, bodyCell

    /// Convert an ISA ProcessParameterValue to a CompositeHeader and Cell tuple
    static member decomposeParameterValue (c : LDNode, ?context : LDContext) : CompositeHeader*CompositeCell =
        let header = BaseTypes.headerOntologyOfPropertyValue (c, ?context = context) |> CompositeHeader.Parameter
        let bodyCell = BaseTypes.cellOfPropertyValue (c, ?context = context)
        header, bodyCell

    /// Convert an ISA FactorValue to a CompositeHeader and Cell tuple
    static member decomposeFactorValue (c : LDNode, ?context : LDContext) : CompositeHeader*CompositeCell =
        let header = BaseTypes.headerOntologyOfPropertyValue (c, ?context = context) |> CompositeHeader.Factor
        let bodyCell = BaseTypes.cellOfPropertyValue (c, ?context = context)
        header, bodyCell

    /// Convert an ISA MaterialAttributeValue to a CompositeHeader and Cell tuple
    static member decomposeCharacteristicValue (c : LDNode, ?context : LDContext) : CompositeHeader*CompositeCell =
        let header = BaseTypes.headerOntologyOfPropertyValue (c, ?context = context) |> CompositeHeader.Characteristic
        let bodyCell = BaseTypes.cellOfPropertyValue (c, ?context = context)
        header, bodyCell
    
    /// Convert an ISA ProcessOutput to a CompositeHeader and Cell tuple
    static member decomposeProcessInput (pn : LDNode, ?context : LDContext) : CompositeHeader*CompositeCell =
        match pn with
        | s when LDSample.validateSource (s, ?context = context) -> CompositeHeader.Input IOType.Source, CompositeCell.FreeText (LDSample.getNameAsString (s, ?context = context))
        | m when LDSample.validateMaterial (m, ?context = context) -> CompositeHeader.Input IOType.Material, CompositeCell.FreeText (LDSample.getNameAsString (m, ?context = context))
        | s when LDSample.validate (s, ?context = context) -> CompositeHeader.Input IOType.Sample, CompositeCell.FreeText (LDSample.getNameAsString (s, ?context = context))
        | d when LDFile.validate (d, ?context = context) -> CompositeHeader.Input IOType.Data, CompositeCell.Data (BaseTypes.decomposeFile (d, ?context = context))
        | n -> CompositeHeader.Input (IOType.FreeText n.SchemaType.[0]), CompositeCell.FreeText (LDSample.getNameAsString (n, ?context = context))            


    static member decomposeProcessOutput (pn : LDNode, ?context : LDContext) : CompositeHeader*CompositeCell =
        match pn with
        | m when LDSample.validateMaterial (m, ?context = context) -> CompositeHeader.Output IOType.Material, CompositeCell.FreeText (LDSample.getNameAsString (m, ?context = context))
        | s when LDSample.validate (s, ?context = context) -> CompositeHeader.Output IOType.Sample, CompositeCell.FreeText (LDSample.getNameAsString (s, ?context = context))
        | d when LDFile.validate (d, ?context = context) -> CompositeHeader.Output IOType.Data, CompositeCell.Data (BaseTypes.decomposeFile (d, ?context = context))
        | n -> CompositeHeader.Output (IOType.FreeText n.SchemaType.[0]), CompositeCell.FreeText (LDSample.getNameAsString (n, ?context = context))

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
        let pattern = """^(?<value>.+) \((?<ontology>[^(]*:[^)]*)\)$"""        

        match name with 
        | Regex.ActivePatterns.Regex pattern r -> 
            let oa = (r.Groups.Item "ontology").Value   |> OntologyAnnotation.fromTermAnnotation 
            let v =  (r.Groups.Item "value").Value
            OntologyAnnotation.create(name = v, ?tan = oa.TermAccessionNumber, ?tsr = oa.TermSourceREF)
        | _ ->
            OntologyAnnotation.create(name = name)



    
open ColumnIndex
open ARCtrl.Helper.Regex.ActivePatterns

/// Functions for parsing ArcTables to ISA json Processes and vice versa
type ProcessConversion = 

    static member tryGetProtocolType (pv : LDNode, ?graph : LDGraph, ?context : LDContext) =
        match LDLabProtocol.tryGetIntendedUseAsDefinedTerm(pv,?graph = graph, ?context = context) with
        | Some dt ->
            Some (BaseTypes.decomposeDefinedTerm(dt, ?context = context))
        | None ->
            match LDLabProtocol.tryGetIntendedUseAsString(pv, ?context = context) with
            | Some s -> Some (OntologyAnnotation.create(name = s))
            | None -> None

    static member composeProcessName (processNameRoot : string) (i : int) =
        $"{processNameRoot}_{i}"

    static member decomposeProcessName (name : string) =
        let pattern = """(?<name>.+)_(?<num>\d+)"""

        match name with 
        | Regex pattern r ->
            (r.Groups.Item "name").Value, Some ((r.Groups.Item "num").Value |> int)
        | _ ->
            name, None
    // Explanation of the Getter logic:
    // The getter logic is used to treat every value of the table only once
    // First, the headers are checked for what getter applies to the respective column. E.g. a ProtocolType getter will only return a function for parsing protocolType cells if the header depicts a protocolType.
    // The appropriate getters are then applied in the context of the processGetter, parsing the cells of the matrix

    /// If the given headers depict a component, returns a function for parsing the values of the matrix to the values of this component
    static member tryComponentGetter (generalI : int) (valueI : int) (valueHeader : CompositeHeader) =
        match valueHeader with
        | CompositeHeader.Component oa ->
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader valueHeader None
                let c = BaseTypes.composeComponent valueHeader cell
                c.SetColumnIndex valueI
                c
            |> Some
        | _ -> None    
            
    /// If the given headers depict a parameter, returns a function for parsing the values of the matrix to the values of this type
    static member tryParameterGetter (generalI : int) (valueI : int) (valueHeader : CompositeHeader) =
        match valueHeader with
        | CompositeHeader.Parameter oa ->
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader valueHeader None
                let p = BaseTypes.composeParameterValue valueHeader cell
                p.SetColumnIndex valueI
                p
            |> Some
        | _ -> None 

    /// If the given headers depict a factor, returns a function for parsing the values of the matrix to the values of this type
    static member tryFactorGetter (generalI : int) (valueI : int) (valueHeader : CompositeHeader) =
        match valueHeader with
        | CompositeHeader.Factor oa ->
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader valueHeader None
                let f = BaseTypes.composeFactorValue valueHeader cell
                f.SetColumnIndex valueI
                f
            |> Some
        | _ -> None 

    /// If the given headers depict a protocolType, returns a function for parsing the values of the matrix to the values of this type
    static member tryCharacteristicGetter (generalI : int) (valueI : int) (valueHeader : CompositeHeader) =
        match valueHeader with
        | CompositeHeader.Characteristic oa ->        
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader valueHeader None
                let c = BaseTypes.composeCharacteristicValue valueHeader cell
                c.SetColumnIndex valueI
                c
            |> Some
        | _ -> None 

    /// If the given headers depict a protocolType, returns a function for parsing the values of the matrix to the values of this type
    static member tryGetProtocolTypeGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.ProtocolType ->
            fun (table : ArcTable) i ->
                let oa =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell.AsTerm
                    | None -> OntologyAnnotation()
                BaseTypes.composeDefinedTerm oa
            |> Some
        | _ -> None 


    /// If the given headers depict a protocolREF, returns a function for parsing the values of the matrix to the values of this type
    static member tryGetProtocolREFGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.ProtocolREF ->
            fun (table : ArcTable) i ->
                match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                | Some cell -> cell.AsFreeText
                | None -> ""
            |> Some
        | _ -> None

    /// If the given headers depict a protocolDescription, returns a function for parsing the values of the matrix to the values of this type
    static member tryGetProtocolDescriptionGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.ProtocolDescription ->
            fun (table : ArcTable) i ->
                match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                | Some cell -> cell.AsFreeText
                | None -> ""
            |> Some
        | _ -> None
   
    /// If the given headers depict a protocolURI, returns a function for parsing the values of the matrix to the values of this type
    static member tryGetProtocolURIGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.ProtocolUri ->
            fun (table : ArcTable) i ->
                match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                | Some cell -> cell.AsFreeText
                | None -> ""
            |> Some
        | _ -> None

    /// If the given headers depict a protocolVersion, returns a function for parsing the values of the matrix to the values of this type
    static member tryGetProtocolVersionGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.ProtocolVersion ->
            fun (table : ArcTable) i ->
                match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                | Some cell -> cell.AsFreeText
                | None -> ""
            |> Some
        | _ -> None

    /// If the given headers depict an input, returns a function for parsing the values of the matrix to the values of this type
    static member tryGetInputGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.Input io ->
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader header None
                BaseTypes.composeProcessInput header cell
            |> Some
        | _ -> None

    /// If the given headers depict an output, returns a function for parsing the values of the matrix to the values of this type
    static member tryGetOutputGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.Output io ->
            fun (table : ArcTable) i ->
                let cell =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell
                    | None -> ArcTableAux.getEmptyCellForHeader header None
                BaseTypes.composeProcessOutput header cell
            |> Some
        | _ -> None

    /// If the given headers depict a comment, returns a function for parsing the values of the matrix to the values of this type
    static member tryGetCommentGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.Comment c ->
            fun (table : ArcTable) i ->
                let comment =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> Comment(c,cell.AsFreeText)
                    | None -> Comment(c)
                comment.ToString()
            |> Some
        | _ -> None

    static member tryGetPerformerGetter (generalI : int) (header : CompositeHeader) =
        match header with
        | CompositeHeader.Performer ->
            fun (table : ArcTable) i ->
                let performer =
                    match ArcTableAux.Unchecked.tryGetCellAt(generalI,i) table.Values with
                    | Some cell -> cell.AsFreeText
                    | None -> ""
                let person = LDPerson.create(performer)
                person
            |> Some
        | _ -> None

    /// Given the header sequence of an ArcTable, returns a function for parsing each row of the table to a process
    static member getProcessGetter (assayName: string option) (studyName : string option) (processNameRoot : string) (headers : CompositeHeader seq) =
    
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
            |> List.choose (fun (valueI,(generalI,header)) -> ProcessConversion.tryCharacteristicGetter generalI valueI header)

        let factorValueGetters =
            valueHeaders
            |> List.choose (fun (valueI,(generalI,header)) -> ProcessConversion.tryFactorGetter generalI valueI header)

        let parameterValueGetters =
            valueHeaders
            |> List.choose (fun (valueI,(generalI,header)) -> ProcessConversion.tryParameterGetter generalI valueI header)

        let componentGetters =
            valueHeaders
            |> List.choose (fun (valueI,(generalI,header)) -> ProcessConversion.tryComponentGetter generalI valueI header)

        let protocolTypeGetter = 
            headers
            |> Seq.tryPick (fun (generalI,header) -> ProcessConversion.tryGetProtocolTypeGetter generalI header)

        let protocolREFGetter = 
            headers
            |> Seq.tryPick (fun (generalI,header) -> ProcessConversion.tryGetProtocolREFGetter generalI header)

        let protocolDescriptionGetter = 
            headers
            |> Seq.tryPick (fun (generalI,header) -> ProcessConversion.tryGetProtocolDescriptionGetter generalI header)

        let protocolURIGetter = 
            headers
            |> Seq.tryPick (fun (generalI,header) -> ProcessConversion.tryGetProtocolURIGetter generalI header)

        let protocolVersionGetter =
            headers
            |> Seq.tryPick (fun (generalI,header) -> ProcessConversion.tryGetProtocolVersionGetter generalI header)

        let performerGetter = 
            headers
            |> Seq.tryPick (fun (generalI,header) -> ProcessConversion.tryGetPerformerGetter generalI header)

        let commentGetters = 
            headers
            |> Seq.choose (fun (generalI,header) -> ProcessConversion.tryGetCommentGetter generalI header)
            |> Seq.toList

        let inputGetter =
            match headers |> Seq.tryPick (fun (generalI,header) -> ProcessConversion.tryGetInputGetter generalI header) with
            | Some inputGetter ->
                fun (table : ArcTable) i ->
                    let chars = charGetters |> Seq.map (fun f -> f table i) |> ResizeArray
                    let input = inputGetter table i

                    if chars.Count > 0 then
                        LDSample.setAdditionalProperties(input,chars)
                    input
                    |> ResizeArray.singleton
            | None when charGetters.Length <> 0 ->
                fun (table : ArcTable) i ->
                    let chars = charGetters |> Seq.map (fun f -> f table i) |> ResizeArray
                    LDSample.createSample(name = $"{processNameRoot}_Input_{i}", additionalProperties = chars)
                    |> ResizeArray.singleton
            | None ->
                fun (table : ArcTable) i ->
                    ResizeArray []
            
        // This is a little more complex, as data and material objects can't contain factors. So in the case where the output of the table is a data object but factors exist. An additional sample object with the same name is created to contain the factors.
        let outputGetter =
            match headers |> Seq.tryPick (fun (generalI,header) -> ProcessConversion.tryGetOutputGetter generalI header) with
            | Some outputGetter ->
                fun (table : ArcTable) i ->
                    let factors = factorValueGetters |> Seq.map (fun f -> f table i) |> ResizeArray
                    let output = outputGetter table i
                    if factors.Count > 0 then
                        LDSample.setAdditionalProperties(output,factors)
                    output
                    |> ResizeArray.singleton
            | None when factorValueGetters.Length <> 0 ->
                fun (table : ArcTable) i ->
                    let factors = factorValueGetters |> Seq.map (fun f -> f table i) |> ResizeArray
                    LDSample.createSample(name = $"{processNameRoot}_Output_{i}", additionalProperties = factors)
                    |> ResizeArray.singleton
            | None ->
                fun (table : ArcTable) i ->
                    ResizeArray []
        fun (table : ArcTable) i ->

            let rowCount = table.RowCount
            let pn =
                if rowCount = 1 then processNameRoot
                else ProcessConversion.composeProcessName processNameRoot i

            let paramvalues = parameterValueGetters |> List.map (fun f -> f table i) |> Option.fromValueWithDefault [] |> Option.map ResizeArray
            //let parameters = paramvalues |> Option.map (List.map (fun pv -> pv.Category.Value))

            let comments = commentGetters |> List.map (fun f -> f table i) |> Option.fromValueWithDefault [] |> Option.map ResizeArray

            let components = componentGetters |> List.map (fun f -> f table i) |> Option.fromValueWithDefault [] |> Option.map ResizeArray

            let id = LDLabProcess.genId(processNameRoot,?assayName = assayName, ?studyName = studyName) + $"_{i}"
            
            let protocol : LDNode option =
                let name = (protocolREFGetter |> Option.map (fun f -> f table i))
                let protocolId = LDLabProtocol.genId(?name = name, processName = processNameRoot)
                LDLabProtocol.create(
                    id = protocolId,
                    ?name = name,
                    ?description = (protocolDescriptionGetter |> Option.map (fun f -> f table i)),
                    ?intendedUse = (protocolTypeGetter |> Option.map (fun f -> f table i)),
                    //?comments = comments,
                    ?url = (protocolURIGetter |> Option.map (fun f -> f table i)),
                    ?version = (protocolVersionGetter |> Option.map (fun f -> f table i)),
                    ?labEquipments = components
                )
                |> Some

            let input,output = inputGetter table i, outputGetter table i

            let agent = performerGetter |> Option.map (fun f -> f table i)

            LDLabProcess.create(
                name = pn,
                objects = input,
                results = output,
                id = id,
                ?agent = agent,
                ?executesLabProtocol = protocol,
                ?parameterValues = paramvalues,
                ?disambiguatingDescriptions = comments

                )

    /// Groups processes by their name, or by the name of the protocol they execute
    ///
    /// Process names are taken from the Worksheet name and numbered: SheetName_1, SheetName_2, etc.
    /// 
    /// This function decomposes this name into a root name and a number, and groups processes by root name.
    static member groupProcesses (processes : LDNode list, ?graph : LDGraph, ?context : LDContext) = 
        processes
        |> List.groupBy (fun p ->
            match LDLabProcess.tryGetNameAsString (p, ?context = context), LDLabProcess.tryGetExecutesLabProtocol(p,?graph = graph, ?context = context) with
            | Some name, _ when ProcessConversion.decomposeProcessName name |> snd |> Option.isSome ->
                ProcessConversion.decomposeProcessName name |> fst
            | _, Some protocol when LDLabProtocol.tryGetNameAsString (protocol, ?context = context) |> Option.isSome ->
                LDLabProtocol.tryGetNameAsString (protocol, ?context = context) |> Option.defaultValue ""
            | Some name, _ when name.Contains "_" ->
                let lastUnderScoreIndex = name.LastIndexOf '_'
                name.Remove lastUnderScoreIndex
            | Some name, _ ->
                name
            | _, Some protocol  ->
                protocol.Id
            | _ ->
                Identifier.createMissingIdentifier()  
        )

    /// Merges processes with the same name, protocol and param values
    //let mergeIdenticalProcesses (processes : list<Process>) =
    //    processes
    //    |> List.groupBy (fun x -> 
    //        if x.Name.IsSome && (x.Name.Value |> Process.decomposeName |> snd).IsSome then
    //            (x.Name.Value |> Process.decomposeName |> fst), HashCodes.boxHashOption x.ExecutesProtocol, x.ParameterValues |> Option.map HashCodes.boxHashSeq, x.Comments |> Option.map HashCodes.boxHashSeq
    //        elif x.ExecutesProtocol.IsSome && x.ExecutesProtocol.Value.Name.IsSome then
    //            x.ExecutesProtocol.Value.Name.Value, HashCodes.boxHashOption x.ExecutesProtocol, x.ParameterValues |> Option.map HashCodes.boxHashSeq, x.Comments |> Option.map HashCodes.boxHashSeq
    //        else
    //            Identifier.createMissingIdentifier(), HashCodes.boxHashOption x.ExecutesProtocol, x.ParameterValues |> Option.map HashCodes.boxHashSeq, x.Comments |> Option.map HashCodes.boxHashSeq
    //    )
    //    |> fun l ->
    //        l
    //        |> List.mapi (fun i ((n,_,_,_),processes) -> 
    //            let pVs = processes.[0].ParameterValues
    //            let inputs = processes |> List.collect (fun p -> p.Inputs |> Option.defaultValue []) |> Option.fromValueWithDefault []
    //            let outputs = processes |> List.collect (fun p -> p.Outputs |> Option.defaultValue []) |> Option.fromValueWithDefault []
    //            let n = if l.Length > 1 then Process.composeName n i else n
    //            Process.create(Name = n,?ExecutesProtocol = processes.[0].ExecutesProtocol,?ParameterValues = pVs,?Inputs = inputs,?Outputs = outputs,?Comments = processes.[0].Comments)
    //        )


    // Transform a isa json process into a isa tab row, where each row is a header+value list
    static member processToRows (p : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let pvs =
            LDLabProcess.getParameterValues(p, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun ppv -> BaseTypes.decomposeParameterValue(ppv, ?context = context), ColumnIndex.tryGetIndex ppv)
        // Get the component
        let components = 
            match LDLabProcess.tryGetExecutesLabProtocol(p, ?graph = graph, ?context = context) with
            | Some prot ->
                LDLabProtocol.getComponents(prot, ?graph = graph, ?context = context)
                |> ResizeArray.map (fun ppv -> BaseTypes.decomposeComponent(ppv, ?context = context), ColumnIndex.tryGetIndex ppv)
            | None -> ResizeArray []
        // Get the values of the protocol
        let protVals = 
            match LDLabProcess.tryGetExecutesLabProtocol(p, ?graph = graph, ?context = context) with
            | Some prot ->
                [
                    match LDLabProtocol.tryGetNameAsString (prot, ?context = context) with | Some name -> yield (CompositeHeader.ProtocolREF, CompositeCell.FreeText name) | None -> ()
                    match LDLabProtocol.tryGetDescriptionAsString (prot, ?context = context) with | Some desc -> yield (CompositeHeader.ProtocolDescription, CompositeCell.FreeText desc) | None -> ()
                    match LDLabProtocol.tryGetUrl (prot, ?context = context) with | Some uri -> yield (CompositeHeader.ProtocolUri, CompositeCell.FreeText uri) | None -> ()
                    match LDLabProtocol.tryGetVersionAsString(prot, ?context = context) with | Some version -> yield (CompositeHeader.ProtocolVersion, CompositeCell.FreeText version) | None -> ()
                    match ProcessConversion.tryGetProtocolType(prot, ?graph = graph, ?context = context) with
                    | Some intendedUse -> yield (CompositeHeader.ProtocolType, CompositeCell.Term intendedUse)
                    | None -> ()
                ]
            | None -> []
        let comments = 
            LDLabProcess.getDisambiguatingDescriptionsAsString(p, ?context = context)
            |> ResizeArray.map (fun c ->
                let c = Comment.fromString c
                CompositeHeader.Comment (Option.defaultValue "" c.Name),
                CompositeCell.FreeText (Option.defaultValue "" c.Value)
            )

        let inputs = LDLabProcess.getObjects(p, ?graph = graph, ?context = context)
        let outputs = LDLabProcess.getResults(p, ?graph = graph, ?context = context)

        let inputs,outputs =
            if inputs.Count = 0 && outputs.Count <> 0 then
                ResizeArray.create outputs.Count None,
                ResizeArray.map Option.Some outputs
            elif inputs.Count <> 0 && outputs.Count = 0 then
                 ResizeArray.map Option.Some inputs,
                 ResizeArray.create inputs.Count None
            else               
                ResizeArray.map Option.Some inputs,
                ResizeArray.map Option.Some outputs


        if inputs.Count = 0 && outputs.Count = 0 then
            let vals =
                [
                    yield! components
                    yield! pvs
                ]
                |> List.sortBy (snd >> Option.defaultValue 10000)
                |> List.map fst
            [
                yield! protVals
                yield! vals
                yield! comments
            ]
            |> ResizeArray.singleton
        else
        // zip the inputs and outpus so they are aligned as rows
            outputs
            |> ResizeArray.zip inputs
            // This grouping here and the picking of the "inputForCharas" etc is done, so there can be rows where data do have characteristics, which is not possible in isa json
            |> ResizeArray.map (fun (i,o) ->
                let chars =
                    match i with
                    | Some i -> 
                        LDSample.getCharacteristics(i, ?graph = graph, ?context = context)
                        |> ResizeArray.map (fun cv -> BaseTypes.decomposeCharacteristicValue(cv, ?context = context), ColumnIndex.tryGetIndex cv)
                    | None -> ResizeArray []            
                let factors =
                    match o with
                    | Some o -> 
                        LDSample.getFactors(o, ?graph = graph, ?context = context)
                        |> ResizeArray.map (fun fv -> BaseTypes.decomposeFactorValue(fv, ?context = context), ColumnIndex.tryGetIndex fv)
                    | None -> ResizeArray []


                let vals =
                    [
                        yield! chars
                        yield! components
                        yield! pvs
                        yield! factors
                    ]
                    |> List.sortBy (snd >> Option.defaultValue 10000)
                    |> List.map fst
                [
                    if i.IsSome then yield BaseTypes.decomposeProcessInput(i.Value, ?context = context)
                    yield! protVals
                    yield! vals
                    yield! comments
                    if o.IsSome then yield BaseTypes.decomposeProcessOutput(o.Value, ?context = context)
                ]
            )

//[<AutoOpen>]
//module CoreTypeExtensions = 

//    type CompositeHeader with

//        member this.TryParameter() = 
//            match this with 
//            | CompositeHeader.Parameter oa -> Some (DefinedTerm.create (ParameterName = oa))
//            | _ -> None

//        member this.TryFactor() =
//            match this with
//            | CompositeHeader.Factor oa -> Some (Factor.create(FactorType = oa))
//            | _ -> None

//        member this.TryCharacteristic() =
//            match this with
//            | CompositeHeader.Characteristic oa -> Some (MaterialAttribute.create(CharacteristicType = oa))
//            | _ -> None

//        member this.TryComponent() =
//            match this with
//            | CompositeHeader.Component oa -> Some (Component.create(componentType = oa))
//            | _ -> None

//    type CompositeCell with

//        /// <summary>
//        /// This function is used to improve interoperability with ISA-JSON types. It is not recommended for default ARCtrl usage.
//        /// </summary>
//        /// <param name="value"></param>
//        /// <param name="unit"></param>
//        static member fromValue(value : Value, ?unit : OntologyAnnotation) =
//            BaseTypes.cellOfValue (Some value) unit


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
        member this.GetProcesses(?assayName, ?studyName) : LDNode list = 
            if this.RowCount = 0 then
                //let input = ResizeArray [Sample.createSample(name = $"{this.Name}_Input", additionalProperties = ResizeArray [])]
                //let output = ResizeArray [Sample.createSample(name = $"{this.Name}_Output", additionalProperties = ResizeArray [])]
                LDLabProcess.create(name = this.Name(*, objects = input, results = output*))
                |> List.singleton
            else
                let getter = ProcessConversion.getProcessGetter assayName studyName this.Name this.Headers          
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
        member this.GetProcesses(?assayName, ?studyName) : LDNode list = 
            this.Tables
            |> Seq.toList
            |> List.collect (fun t -> t.GetProcesses(?assayName = assayName, ?studyName = studyName))

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

type DatamapConversion =

    static member composeFragmentDescriptors (datamap : DataMap) : ResizeArray<LDNode> =
        datamap.DataContexts
        |> ResizeArray.map BaseTypes.composeFragmentDescriptor

    static member decomposeFragmentDescriptors (fragmentDescriptors : ResizeArray<LDNode>, ?graph : LDGraph, ?context : LDContext) : DataMap =
        fragmentDescriptors
        |> ResizeArray.map (fun fd -> BaseTypes.decomposeFragmentDescriptor(fd, ?graph = graph, ?context = context))
        |> DataMap

type PersonConversion = 

    static member orcidKey = "ORCID"   

    static member composeAffiliation (affiliation : string) : LDNode =
        try 
            ARCtrl.Json.Decode.fromJsonString Json.LDNode.decoder affiliation
        with
        | _ -> LDOrganization.create(name = affiliation)

    static member decomposeAffiliation (affiliation : LDNode, ?context : LDContext) : string =
        let hasOnlyName = 
            affiliation.GetPropertyNames(?context = context)
            |> Seq.filter(fun n -> n <> LDOrganization.name)
            |> Seq.isEmpty
        if hasOnlyName then
            LDOrganization.getNameAsString(affiliation, ?context = context)
        else
            Json.LDNode.encoder affiliation
            |> ARCtrl.Json.Encode.toJsonString 0

    static member composeAddress (address : string) : obj =
        try 
            ARCtrl.Json.Decode.fromJsonString Json.LDNode.decoder address
            |> box
        with
        | _ -> address

    static member decomposeAddress (address : obj) : string =
        match address with
        | :? string as s -> s
        | :? LDNode as n -> 
            Json.LDNode.encoder n
            |> ARCtrl.Json.Encode.toJsonString 0
        | _ -> failwith "Address must be a string or a Json.LDNode"

    static member composePerson (person : ARCtrl.Person) =
        let givenName =
            match person.FirstName with
            | Some fn -> fn
            | None -> failwith "Person must have a given name"
        let jobTitles = 
            person.Roles
            |> ResizeArray.map BaseTypes.composeDefinedTerm
            |> Option.fromSeq
        let disambiguatingDescriptions = 
            person.Comments
            |> ResizeArray.map (fun c -> c.ToString())
            |> Option.fromSeq
        let address =
            person.Address
            |> Option.map PersonConversion.composeAddress
        let affiliation = 
            person.Affiliation
            |> Option.map PersonConversion.composeAffiliation
        LDPerson.create(givenName, ?orcid = person.ORCID, ?affiliation = affiliation, ?email = person.EMail, ?familyName = person.LastName, ?jobTitles = jobTitles, ?additionalName = person.MidInitials, ?address = address, ?disambiguatingDescriptions = disambiguatingDescriptions, ?faxNumber = person.Fax, ?telephone = person.Phone)

    static member decomposePerson (person : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let orcid = ORCID.tryGetOrcidNumber person.Id
        let address = 
            match LDPerson.tryGetAddressAsString(person, ?context = context) with
            | Some s -> 
                Some s
            | None ->
                match LDPerson.tryGetAddressAsPostalAddress(person, ?graph = graph, ?context = context) with
                | Some a -> Some (PersonConversion.decomposeAddress a)
                | None -> None
        let roles = 
            LDPerson.getJobTitlesAsDefinedTerm(person, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun r -> BaseTypes.decomposeDefinedTerm(r, ?context = context))
        let comments =
            LDPerson.getDisambiguatingDescriptionsAsString(person, ?context = context)
            |> ResizeArray.map Comment.fromString
        let affiliation =
            LDPerson.tryGetAffiliation(person, ?graph = graph, ?context = context)
            |> Option.map (fun a -> PersonConversion.decomposeAffiliation(a, ?context = context))
        Person.create(
            firstName = LDPerson.getGivenNameAsString(person, ?context = context),
            ?lastName = LDPerson.tryGetFamilyNameAsString(person, ?context = context),
            ?midInitials = LDPerson.tryGetAdditionalNameAsString(person, ?context = context),
            ?email = LDPerson.tryGetEmailAsString(person, ?context = context),
            ?fax = LDPerson.tryGetFaxNumberAsString(person, ?context = context),
            ?phone = LDPerson.tryGetTelephoneAsString(person, ?context = context),
            ?orcid = orcid,
            ?affiliation = affiliation,
            roles = roles,
            ?address = address,
            comments = comments
        )
        

type ScholarlyArticleConversion =

    static member composeAuthor (author : string) : LDNode =
        try 
            ARCtrl.Json.Decode.fromJsonString Json.LDNode.decoder author
        with
        | _ -> LDPerson.create(givenName = author)

    static member splitAuthors (a : string) =
        let mutable bracketCount = 0
        let authors = ResizeArray<string>()
        let sb = System.Text.StringBuilder()
        for c in a do
            if c = '{' then 
                bracketCount <- bracketCount + 1
                sb.Append(c) |> ignore
            elif c = '}' then 
                bracketCount <- bracketCount - 1
                sb.Append(c) |> ignore
            elif c = ',' && bracketCount = 0 then
                authors.Add(sb.ToString())
                sb.Clear() |> ignore
            else 
                sb.Append(c) |> ignore
        authors.Add(sb.ToString())
        authors

    static member composeAuthors (authors : string) : ResizeArray<LDNode> =
        ScholarlyArticleConversion.splitAuthors authors
        |> Seq.map ScholarlyArticleConversion.composeAuthor
        |> ResizeArray

    static member decomposeAuthor (author : LDNode, ?context : LDContext) : string =
        let hasOnlyGivenName = 
            author.GetPropertyNames(?context = context)
            |> Seq.filter(fun n -> n <> LDPerson.givenName)
            |> Seq.isEmpty
        if hasOnlyGivenName then
            LDPerson.getGivenNameAsString(author, ?context = context)
        else
            Json.LDNode.encoder author
            |> ARCtrl.Json.Encode.toJsonString 0

    static member decomposeAuthors (authors : ResizeArray<LDNode>, ?context : LDContext) : string =
        authors
        |> ResizeArray.map (fun a -> ScholarlyArticleConversion.decomposeAuthor (a,?context = context))
        |> String.concat ","

    static member composeScholarlyArticle (publication : Publication) =
        let title = match publication.Title with | Some t -> t | None -> failwith "Publication must have a title"
        let authors = 
            publication.Authors
            |> Option.map ScholarlyArticleConversion.composeAuthors
        let comments = 
            publication.Comments
            |> ResizeArray.map (BaseTypes.composeComment)
            |> Option.fromSeq
        let identifiers = ResizeArray [
            if publication.DOI.IsSome && publication.DOI.Value <> "" then
                LDPropertyValue.createDOI publication.DOI.Value
            if publication.PubMedID.IsSome && publication.PubMedID.Value <> "" then
                LDPropertyValue.createPubMedID publication.PubMedID.Value
        ]
        let status = publication.Status |> Option.map BaseTypes.composeDefinedTerm
        let scholarlyArticle = 
            LDScholarlyArticle.create(
                headline = title,
                identifiers = identifiers,
                ?authors = authors,
                //?url = publication.DOI,
                ?creativeWorkStatus = status,
                ?comments = comments            
            )
        scholarlyArticle

    static member decomposeScholarlyArticle (sa : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let title = LDScholarlyArticle.getHeadlineAsString(sa, ?context = context)
        let authors = 
            LDScholarlyArticle.getAuthors(sa, ?graph = graph, ?context = context)
            |> Option.fromSeq
            |> Option.map (fun a -> ScholarlyArticleConversion.decomposeAuthors(a, ?context = context))
        let comments = 
            LDScholarlyArticle.getComments(sa, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> BaseTypes.decomposeComment(c, ?context = context))
        let status = 
            LDScholarlyArticle.tryGetCreativeWorkStatus(sa, ?graph = graph, ?context = context)
            |> Option.map (fun s -> BaseTypes.decomposeDefinedTerm(s, ?context = context))
        let identifiers = LDScholarlyArticle.getIdentifiersAsPropertyValue(sa, ?graph = graph, ?context = context)
        let doi =
            identifiers
            |> ResizeArray.tryPick (fun i -> LDPropertyValue.tryGetAsDOI(i, ?context = context))
            |> function
               | Some p -> Some p
               | None -> LDScholarlyArticle.tryGetSameAsAsString(sa, ?context = context)
        let pubMedID =
            identifiers
            |> ResizeArray.tryPick (fun i -> LDPropertyValue.tryGetAsPubMedID(i, ?context = context))
            |> function
               | Some p -> Some p
               | None -> LDScholarlyArticle.tryGetUrlAsString(sa, ?context = context)
        ARCtrl.Publication.create(
            title = title,
            ?authors = authors,
            ?status = status,
            comments = comments,
            ?doi = doi,
            ?pubMedID = pubMedID
        )

type AssayConversion =

    static member getDataFilesFromProcesses (processes : LDNode ResizeArray, ?fragmentDescriptors : LDNode ResizeArray, ?graph : LDGraph, ?context : LDContext) =
        let dataFromFragmentDescriptors =
            fragmentDescriptors
            |> Option.defaultValue (ResizeArray [])
            |> ResizeArray.choose (fun df -> LDPropertyValue.tryGetSubjectOf(df, ?graph = graph, ?context = context))
        let data = 
            processes
            |> ResizeArray.collect (fun p -> 
                let inputs = LDLabProcess.getObjectsAsData(p, ?graph = graph, ?context = context)
                let outputs = LDLabProcess.getResultsAsData(p, ?graph = graph, ?context = context)
                ResizeArray.append inputs outputs
            )
            |> ResizeArray.append dataFromFragmentDescriptors
            |> ResizeArray.distinct
        let files =
            data
            |> ResizeArray.filter (fun d -> 
                DataAux.pathAndSelectorFromName d.Id |> snd |> Option.isNone
            )
        let filesFromfragments = 
            data
            |> ResizeArray.filter (fun d -> 
                DataAux.pathAndSelectorFromName d.Id |> snd |> Option.isSome
            )
            |> ResizeArray.groupBy (fun d ->
                DataAux.pathAndSelectorFromName d.Id |> fst
            )
            |> ResizeArray.map (fun (path,fragments) ->
                let file =
                    match files |> ResizeArray.tryFind (fun d -> d.Id = path) with
                    | Some f -> f
                    | None ->
                        let comments = 
                            LDFile.getComments(fragments.[0], ?graph = graph, ?context = context)
                            |> Option.fromSeq
                        LDFile.create(
                            id = path,
                            name = path,
                            ?comments = comments,
                            ?disambiguatingDescription = LDFile.tryGetDisambiguatingDescriptionAsString(fragments.[0], ?context = context),
                            ?encodingFormat = LDFile.tryGetEncodingFormatAsString(fragments.[0], ?context = context),
                            ?context = fragments.[0].TryGetContext()
                        )
                LDDataset.setHasParts(file, fragments,?context = context)
                file            
            )
        ResizeArray.append files filesFromfragments

    static member composeAssay (assay : ArcAssay) =
        let measurementMethod = assay.TechnologyType |> Option.map BaseTypes.composeDefinedTerm
        let measurementTechnique = assay.TechnologyPlatform |> Option.map BaseTypes.composeDefinedTerm
        let variableMeasured = assay.MeasurementType |> Option.map BaseTypes.composePropertyValueFromOA
        let creators = 
            assay.Performers
            |> ResizeArray.map (fun c -> PersonConversion.composePerson c)
            |> Option.fromSeq
        let processSequence = 
            ArcTables(assay.Tables).GetProcesses(assayName = assay.Identifier)
            |> ResizeArray
            |> Option.fromSeq
        let fragmentDescriptors =
            assay.DataMap
            |> Option.map DatamapConversion.composeFragmentDescriptors
        let dataFiles = 
            processSequence
            |> Option.map (fun ps -> AssayConversion.getDataFilesFromProcesses(ps, ?fragmentDescriptors = fragmentDescriptors))
        let variableMeasureds =
            match variableMeasured, fragmentDescriptors with
            | Some vm, Some fds -> ResizeArray.appendSingleton vm fds |> Some
            | Some vm, None -> ResizeArray.singleton vm |> Some
            | None, Some fds -> fds |> Some
            | None, None -> None
        let comments = 
            assay.Comments
            |> ResizeArray.map (fun c -> BaseTypes.composeComment c)
            |> Option.fromSeq
        LDDataset.createAssay(
            identifier = assay.Identifier,
            ?name = assay.Title,
            ?description = assay.Description, 
            ?creators = creators,
            ?hasParts = dataFiles,
            ?measurementMethod = measurementMethod,
            ?measurementTechnique = measurementTechnique,
            ?variableMeasureds = variableMeasureds,
            ?abouts = processSequence,
            ?comments = comments
        )

    static member decomposeAssay (assay : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let measurementMethod = 
            LDDataset.tryGetMeasurementMethodAsDefinedTerm(assay, ?graph = graph, ?context = context)
            |> Option.map (fun m -> BaseTypes.decomposeDefinedTerm(m, ?context = context))
        let measurementTechnique = 
            LDDataset.tryGetMeasurementTechniqueAsDefinedTerm(assay, ?graph = graph, ?context = context)
            |> Option.map (fun m -> BaseTypes.decomposeDefinedTerm(m, ?context = context))
        let variableMeasured = 
            LDDataset.tryGetVariableMeasuredAsMeasurementType(assay, ?graph = graph, ?context = context)
            |> Option.map (fun v -> BaseTypes.decomposePropertyValueToOA(v, ?context = context))
        let perfomers = 
            LDDataset.getCreators(assay, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> PersonConversion.decomposePerson(c, ?graph = graph, ?context = context))
        let dataMap = 
            LDDataset.getVariableMeasuredAsFragmentDescriptors(assay, ?graph = graph, ?context = context)
            |> fun fds -> DatamapConversion.decomposeFragmentDescriptors(fds, ?graph = graph, ?context = context)
            |> Option.fromValueWithDefault (DataMap.init())
        let tables = 
            LDDataset.getAboutsAsLabProcess(assay, ?graph = graph, ?context = context)
            |> fun ps -> ArcTables.fromProcesses(List.ofSeq ps, ?graph = graph, ?context = context)
        let comments =
            LDDataset.getComments(assay, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> BaseTypes.decomposeComment(c, ?context = context))
        ArcAssay.create(
            identifier = LDDataset.getIdentifierAsString(assay, ?context = context),
            ?title = LDDataset.tryGetNameAsString(assay, ?context = context),
            ?description = LDDataset.tryGetDescriptionAsString(assay, ?context = context),
            ?measurementType = variableMeasured,
            ?technologyType = measurementMethod,
            ?technologyPlatform = measurementTechnique,
            tables = tables.Tables,
            ?datamap = dataMap,
            performers = perfomers,
            comments = comments
        )

type StudyConversion = 

    static member composeStudy (study : ArcStudy) =
        let dateCreated = study.SubmissionDate |> Option.bind DateTime.tryFromString
        let datePublished = study.PublicReleaseDate |> Option.bind DateTime.tryFromString
        let dateModified = System.DateTime.Now
        let publications = 
            study.Publications
            |> ResizeArray.map (fun p -> ScholarlyArticleConversion.composeScholarlyArticle p)
            |> Option.fromSeq
        let creators =
            study.Contacts
            |> ResizeArray.map (fun c -> PersonConversion.composePerson c)
            |> Option.fromSeq
        let processSequence = 
            ArcTables(study.Tables).GetProcesses(studyName = study.Identifier)
            |> ResizeArray
            |> Option.fromSeq
        let fragmentDescriptors =
            study.DataMap
            |> Option.map DatamapConversion.composeFragmentDescriptors
        let dataFiles = 
            processSequence
            |> Option.map (fun ps -> AssayConversion.getDataFilesFromProcesses(ps, ?fragmentDescriptors = fragmentDescriptors))
        let comments = 
            study.Comments
            |> ResizeArray.map (fun c -> BaseTypes.composeComment c)
            |> Option.fromSeq
        LDDataset.createStudy(
            identifier = study.Identifier,
            ?name = study.Title,
            ?description = study.Description,
            ?dateCreated = dateCreated,
            ?datePublished = datePublished,
            dateModified = dateModified,
            ?creators = creators,
            ?citations = publications,
            ?hasParts = dataFiles,
            ?variableMeasureds = fragmentDescriptors,
            ?abouts = processSequence,
            ?comments = comments
        )

    static member decomposeStudy (study : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let dateCreated = 
            LDDataset.tryGetDateCreatedAsDateTime(study, ?context = context)
            |> Option.map DateTime.toString
        let datePublished = 
            LDDataset.tryGetDatePublishedAsDateTime(study, ?context = context)
            |> Option.map DateTime.toString
        let publications = 
            LDDataset.getCitations(study, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun p -> ScholarlyArticleConversion.decomposeScholarlyArticle(p, ?graph = graph, ?context = context))
        let creators = 
            LDDataset.getCreators(study, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> PersonConversion.decomposePerson(c, ?graph = graph, ?context = context))
        let dataMap = 
            LDDataset.getVariableMeasuredAsFragmentDescriptors(study, ?graph = graph, ?context = context)
            |> fun fds -> DatamapConversion.decomposeFragmentDescriptors(fds, ?graph = graph, ?context = context)
            |> Option.fromValueWithDefault (DataMap.init())
        let tables = 
            LDDataset.getAboutsAsLabProcess(study, ?graph = graph, ?context = context)
            |> fun ps -> ArcTables.fromProcesses(List.ofSeq ps, ?graph = graph, ?context = context)
        let comments =
            LDDataset.getComments(study, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> BaseTypes.decomposeComment(c, ?context = context))
        ArcStudy.create(
            identifier = LDDataset.getIdentifierAsString(study, ?context = context),
            ?title = LDDataset.tryGetNameAsString(study, ?context = context),
            ?description = LDDataset.tryGetDescriptionAsString(study, ?context = context),
            ?submissionDate = dateCreated,
            ?publicReleaseDate = datePublished,
            ?datamap = dataMap,
            contacts = creators,
            publications = publications,
            tables = tables.Tables,
            comments = comments
        )

type InvestigationConversion =

    static member composeInvestigation (investigation : ArcInvestigation) =
        let name = match investigation.Title with | Some t -> t | None -> failwith "Investigation must have a title"
        let dateCreated = investigation.SubmissionDate |> Option.bind DateTime.tryFromString
        let datePublished =
            investigation.PublicReleaseDate
            |> Option.bind DateTime.tryFromString
            |> Option.defaultValue (System.DateTime.Now)
        //let dateModified = System.DateTime.Now
        let publications = 
            investigation.Publications
            |> ResizeArray.map (fun p -> ScholarlyArticleConversion.composeScholarlyArticle p)
            |> Option.fromSeq
        let creators =
            investigation.Contacts
            |> ResizeArray.map (fun c -> PersonConversion.composePerson c)
            |> Option.fromSeq
        let comments = 
            investigation.Comments
            |> ResizeArray.map (fun c -> BaseTypes.composeComment c)
            |> Option.fromSeq
        let hasParts =
            investigation.Assays
            |> ResizeArray.map (fun a -> AssayConversion.composeAssay a)
            |> ResizeArray.append (investigation.Studies |> ResizeArray.map (fun s -> StudyConversion.composeStudy s))
            |> Option.fromSeq
        let mentions =
            ResizeArray [] // TODO
            |> Option.fromSeq
        LDDataset.createInvestigation(
            identifier = investigation.Identifier,
            name = name,
            ?description = investigation.Description,
            ?dateCreated = dateCreated,
            datePublished = datePublished,
            //dateModified = dateModified,
            ?creators = creators,
            ?citations = publications,
            ?hasParts = hasParts,
            ?mentions = mentions,
            ?comments = comments
        )

    static member decomposeInvestigation (investigation : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let title =
            match LDDataset.tryGetNameAsString(investigation, ?context = context) with
            | Some t -> Some t
            | None -> LDDataset.tryGetHeadlineAsString(investigation, ?context = context)
        let dateCreated = 
            LDDataset.tryGetDateCreatedAsDateTime(investigation, ?context = context)
            |> Option.map DateTime.toString
        let datePublished = 
            LDDataset.tryGetDatePublishedAsDateTime(investigation, ?context = context)
            |> Option.map DateTime.toString
        let publications = 
            LDDataset.getCitations(investigation, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun p -> ScholarlyArticleConversion.decomposeScholarlyArticle(p, ?graph = graph, ?context = context))
        let creators = 
            LDDataset.getCreators(investigation, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> PersonConversion.decomposePerson(c, ?graph = graph, ?context = context))
        let datasets = 
            LDDataset.getHasPartsAsDataset  (investigation, ?graph = graph, ?context = context)
        let studies = 
            datasets
            |> ResizeArray.filter (fun d -> LDDataset.validateStudy(d, ?context = context))
            |> ResizeArray.map (fun d -> StudyConversion.decomposeStudy(d, ?graph = graph, ?context = context))
        let assays = 
            datasets
            |> ResizeArray.filter (fun d -> LDDataset.validateAssay(d, ?context = context))
            |> ResizeArray.map (fun d -> AssayConversion.decomposeAssay(d, ?graph = graph, ?context = context))
        let comments =
            LDDataset.getComments(investigation, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> BaseTypes.decomposeComment(c, ?context = context))
        ArcInvestigation.create(
            identifier = LDDataset.getIdentifierAsString(investigation, ?context = context),
            ?title = title,
            ?description = LDDataset.tryGetDescriptionAsString(investigation, ?context = context),
            ?submissionDate = dateCreated,
            ?publicReleaseDate = datePublished,
            contacts = creators,
            publications = publications,
            studies = studies,
            assays = assays,
            comments = comments
        )

[<AutoOpen>]
module TypeExtensions =

    type ArcAssay with
         member this.ToROCrateAssay() = AssayConversion.composeAssay this

         static member fromROCrateAssay (a : LDNode, ?graph : LDGraph, ?context : LDContext) = AssayConversion.decomposeAssay(a, ?graph = graph, ?context = context)

    type ArcStudy with
         member this.ToROCrateStudy() = StudyConversion.composeStudy this

         static member fromROCrateStudy (a : LDNode, ?graph : LDGraph, ?context : LDContext) = StudyConversion.decomposeStudy(a, ?graph = graph, ?context = context)

    type ArcInvestigation with
        member this.ToROCrateInvestigation() = InvestigationConversion.composeInvestigation this
    
        static member fromROCrateInvestigation (a : LDNode, ?graph : LDGraph, ?context : LDContext) = InvestigationConversion.decomposeInvestigation(a, ?graph = graph, ?context = context)

    [<Fable.Core.AttachMembers>]
    type Conversion =

        static member arcAssayToDataset(a : ArcAssay) = a.ToROCrateAssay()

        static member datasetToArcAssay(a : LDNode, ?graph : LDGraph, ?context : LDContext) =
            ArcAssay.fromROCrateAssay(a, ?graph = graph, ?context = context)

        static member arcStudyToDataset(a : ArcStudy) = a.ToROCrateStudy()

        static member datasetToArcStudy(a : LDNode, ?graph : LDGraph, ?context : LDContext) =
            ArcStudy.fromROCrateStudy(a, ?graph = graph, ?context = context)

        static member arcInvestigationToDataset(a : ArcInvestigation) = a.ToROCrateInvestigation()

        static member datasetToArcInvestigation(a : LDNode, ?graph : LDGraph, ?context : LDContext) =
            ArcInvestigation.fromROCrateInvestigation(a, ?graph = graph, ?context = context)       

    type Dataset with
        static member toArcAssay(a : LDNode, ?graph : LDGraph, ?context : LDContext) = AssayConversion.decomposeAssay(a, ?graph = graph, ?context = context)

        static member fromArcAssay (a : ArcAssay) = AssayConversion.composeAssay a

        static member toArcStudy(a : LDNode, ?graph : LDGraph, ?context : LDContext) = StudyConversion.decomposeStudy(a, ?graph = graph, ?context = context)

        static member fromArcStudy (a : ArcStudy) = StudyConversion.composeStudy a

        static member toArcInvestigation(a : LDNode, ?graph : LDGraph, ?context : LDContext) = InvestigationConversion.decomposeInvestigation(a, ?graph = graph, ?context = context)

        static member fromArcInvestigation (a : ArcInvestigation) = InvestigationConversion.composeInvestigation a

