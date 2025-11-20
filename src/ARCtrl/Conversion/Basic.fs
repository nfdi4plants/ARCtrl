namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.FileSystem
open System.Collections.Generic
//open ColumnIndex



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
        

    static member composeFile (d : Data, ?fs : FileSystem) : LDNode =
        let createFile() =
            let dataType = d.DataType |> Option.map (fun dt -> dt.AsString) 
            LDFile.create(d.NameText,d.NameText,?disambiguatingDescription = dataType, ?encodingFormat = d.Format, ?usageInfo = d.SelectorFormat)
        match fs with
        | Some fs ->
            match fs.Tree.TryGetPath d.NameText with
            | None ->
                createFile()
            | Some (File _) ->
                createFile()
            | Some (Folder _ as fs) ->
                let file = createFile()
                file.SchemaType <- ResizeArray [LDFile.schemaType; LDDataset.schemaType]
                let subFiles = 
                    fs.ToFilePaths(true)
                    |> Array.map (fun fp ->
                        let fullPath = ArcPathHelper.combine d.NameText fp
                        LDFile.create(fullPath,fullPath)
                    )
                    |> ResizeArray
                LDDataset.setHasParts(file, subFiles)
                file
        | None ->
             createFile()
            

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
    static member composeProcessInput (header : CompositeHeader) (value : CompositeCell) (fs : FileSystem option) : LDNode =
        match header with
        | CompositeHeader.Input IOType.Source -> LDSample.createSource(value.AsFreeText)
        | CompositeHeader.Input IOType.Sample -> LDSample.createSample(value.AsFreeText)
        | CompositeHeader.Input IOType.Material -> LDSample.createMaterial(value.AsFreeText)
        | CompositeHeader.Input IOType.Data ->
            match value with
            | CompositeCell.FreeText ft ->
                LDFile.create(ft,ft)
            | CompositeCell.Data od ->
                BaseTypes.composeFile(od, ?fs = fs)
            | _ -> failwithf "Could not parse input data %O" value
        | CompositeHeader.Input (IOType.FreeText ft) ->
            let n = LDNode(id = BaseTypes.composeFreetextMaterialName ft value.AsFreeText, schemaType = ResizeArray [ft])
            n.SetProperty(LDSample.name, value.AsFreeText)
            n
        | _ ->
            failwithf "Could not parse input header %O" header


    /// Convert a CompositeHeader and Cell tuple to a ISA ProcessOutput
    static member composeProcessOutput (header : CompositeHeader) (value : CompositeCell) (fs : FileSystem option) : LDNode =
        match header with
        | CompositeHeader.Output IOType.Source 
        | CompositeHeader.Output IOType.Sample -> LDSample.createSample(value.AsFreeText)
        | CompositeHeader.Output IOType.Material -> LDSample.createMaterial(value.AsFreeText)
        | CompositeHeader.Output IOType.Data ->
            match value with
            | CompositeCell.FreeText ft ->
                LDFile.create(ft,ft)
            | CompositeCell.Data od ->
                BaseTypes.composeFile(od, ?fs = fs)
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
    static member decomposePropertyValue (pv : LDNode, ?context : LDContext) : OntologyAnnotation*Value option*OntologyAnnotation option =
        let header = BaseTypes.headerOntologyOfPropertyValue(pv, ?context = context)
        let value =           
            let v = LDPropertyValue.tryGetValueAsString(pv, ?context = context)
            let vRef = LDPropertyValue.tryGetValueReferenceAsString(pv, ?context = context)
            match v,vRef with
            | _, Some vr ->
                Some (Value.Ontology (OntologyAnnotation.fromTermAnnotation(vr,?name = v)))
            | Some v, None ->
                Value.Name v |> Some
            | None, None ->
                None
        let unit =
            let u = LDPropertyValue.tryGetUnitTextAsString(pv, ?context = context)
            let uRef = LDPropertyValue.tryGetUnitCodeAsString(pv, ?context = context)
            match u,uRef with
            | Some u, None ->
                Some (OntologyAnnotation(name = u))
            | _, Some uRef ->
                Some (OntologyAnnotation.fromTermAnnotation(uRef, ?name = u))
            | None, None ->
                None
        header, value, unit

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