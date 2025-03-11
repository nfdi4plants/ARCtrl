namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate
open ARCtrl.Helper

///
[<AttachMembers>]
type LDPropertyValue =

    static member schemaType = "http://schema.org/PropertyValue"

    static member name = "http://schema.org/name"

    static member value = "http://schema.org/value"

    static member propertyID = "http://schema.org/propertyID"

    static member unitCode = "http://schema.org/unitCode"

    static member unitText = "http://schema.org/unitText"

    static member valueReference = "http://schema.org/valueReference"

    static member measurementMethod = "http://schema.org/measurementMethod"

    static member description = "http://schema.org/description"

    static member alternateName = "http://schema.org/alternateName"

    static member subjectOf = "http://schema.org/subjectOf"

    static member disambiguatingDescription = "http://schema.org/disambiguatingDescription"

    static member doiKey = "DOI"

    static member doiURL = "http://purl.obolibrary.org/obo/OBI_0002110"

    static member pubmedIDKey = "PubMedID"

    static member pubmedIDURL = "http://purl.obolibrary.org/obo/OBI_0001617"

    static member tryGetNameAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(LDPropertyValue.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(LDPropertyValue.name, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Property of `name` of object with @id `{pv.Id}` was not a string"
        | _ -> failwith $"Could not access property `name` of object with @id `{pv.Id}`"

    static member setNameAsString(pv : LDNode, name : string, ?context : LDContext) =
        pv.SetProperty(LDPropertyValue.name, name, ?context = context)

    static member tryGetValueAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(LDPropertyValue.value, ?context = context) with
        | Some (:? string as v) -> Some v
        | _ -> None

    static member getValueAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(LDPropertyValue.value, ?context = context) with
        | Some (:? string as v) -> v
        | Some _ -> failwith $"Property of `value` of object with @id `{pv.Id}` was not a string"
        | _ -> failwith $"Could not access property `value` of object with @id `{pv.Id}`"

    static member setValueAsString(pv : LDNode, value : string, ?context : LDContext) =
        pv.SetProperty(LDPropertyValue.value, value, ?context = context)

    static member tryGetPropertyIDAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(LDPropertyValue.propertyID, ?context = context) with
        | Some (:? string as pid) -> Some pid
        | _ -> None

    static member setPropertyIDAsString(pv : LDNode, propertyID : string, ?context : LDContext) =
        pv.SetProperty(LDPropertyValue.propertyID, propertyID, ?context = context)

    static member tryGetUnitCodeAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(LDPropertyValue.unitCode, ?context = context) with
        | Some (:? string as uc) -> Some uc
        | _ -> None

    static member setUnitCodeAsString(pv : LDNode, unitCode : string, ?context : LDContext) =
        pv.SetProperty(LDPropertyValue.unitCode, unitCode, ?context = context)

    static member tryGetUnitTextAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(LDPropertyValue.unitText, ?context = context) with
        | Some (:? string as ut) -> Some ut
        | _ -> None

    static member setUnitTextAsString(pv : LDNode, unitText : string, ?context : LDContext) =
        pv.SetProperty(LDPropertyValue.unitText, unitText, ?context = context)

    static member tryGetValueReferenceAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(LDPropertyValue.valueReference, ?context = context) with
        | Some (:? string as vr) -> Some vr
        | _ -> None

    static member setValueReferenceAsString(pv : LDNode, valueReference : string, ?context : LDContext) =
        pv.SetProperty(LDPropertyValue.valueReference, valueReference, ?context = context)

    // 
    static member tryGetMeasurementMethodAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(LDPropertyValue.measurementMethod, ?context = context) with
        | Some (:? string as vr) -> Some vr
        | _ -> None

    static member setMeasurementMethodAsString(pv : LDNode, measurementMethod : string, ?context : LDContext) =
        pv.SetProperty(LDPropertyValue.measurementMethod, measurementMethod, ?context = context)

    static member tryGetDescriptionAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(LDPropertyValue.description, ?context = context) with
        | Some (:? string as vr) -> Some vr
        | _ -> None

    static member setDescriptionAsString(pv : LDNode, description : string, ?context : LDContext) =
        pv.SetProperty(LDPropertyValue.description, description, ?context = context)

    static member tryGetAlternateNameAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(LDPropertyValue.alternateName, ?context = context) with
        | Some (:? string as vr) -> Some vr
        | _ -> None

    static member setAlternateNameAsString(pv : LDNode, alternateName : string, ?context : LDContext) =
        pv.SetProperty(LDPropertyValue.alternateName, alternateName, ?context = context)

    static member getDisambiguatingDescriptionsAsString(pv : LDNode, ?context : LDContext) =
        let filter = fun (o : obj) context -> o :? string
        pv.GetPropertyValues(LDPropertyValue.disambiguatingDescription, filter = filter, ?context = context)
        |> ResizeArray.map (fun (o : obj) -> o :?> string)

    static member setDisambiguatingDescriptionsAsString(lp : LDNode, disambiguatingDescriptions : ResizeArray<string>, ?context : LDContext) =
        lp.SetProperty(LDPropertyValue.disambiguatingDescription, disambiguatingDescriptions, ?context = context)

    static member tryGetSubjectOf(pv : LDNode, ?graph : LDGraph, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleNode(LDPropertyValue.subjectOf, ?graph = graph, ?context = context) with
        | Some so -> Some so
        | _ -> None

    static member setSubjectOf(pv : LDNode, subjectOf : LDNode, ?context : LDContext) =
        pv.SetProperty(LDPropertyValue.subjectOf, subjectOf, ?context = context)

    static member validate(pv : LDNode, ?context : LDContext) =
        pv.HasType(LDPropertyValue.schemaType, ?context = context)
        && pv.HasProperty(LDPropertyValue.name, ?context = context)
        //&& pv.HasProperty(LDPropertyValue.value, ?context = context)

    static member validateComponent (pv : LDNode, ?context : LDContext) =
        LDPropertyValue.validate(pv, ?context = context)
        && pv.AdditionalType.Contains("Component")

    static member validateParameterValue (pv : LDNode, ?context : LDContext) =
        LDPropertyValue.validate(pv, ?context = context)
        && (pv.AdditionalType.Contains("ParameterValue") || pv.AdditionalType.Contains("ProcessParameterValue"))

    static member validateCharacteristicValue (pv : LDNode, ?context : LDContext) =
        LDPropertyValue.validate(pv, ?context = context)
        && (pv.AdditionalType.Contains("CharacteristicValue") || pv.AdditionalType.Contains("MaterialAttributeValue"))

    static member validateFactorValue (pv : LDNode, ?context : LDContext) =
        LDPropertyValue.validate(pv, ?context = context)
        && pv.AdditionalType.Contains("FactorValue")

    static member validateFragmentDescriptor (pv : LDNode, ?context : LDContext) =
        LDPropertyValue.validate(pv, ?context = context)
        //&& pv.AdditionalType.Contains("FragmentDescriptor")
        && LDPropertyValue.getNameAsString(pv, ?context = context) = "FragmentDescriptor"
        //&& LDPropertyValue.tryGetPropertyIDAsString(pv, ?context = context) = (Some "URLToFragmentDescriptor")

    static member validateDOI (pv : LDNode, ?context : LDContext) =
        LDPropertyValue.validate(pv, ?context = context)
        && 
        match
            LDPropertyValue.tryGetNameAsString(pv, ?context = context),
            LDPropertyValue.tryGetValueAsString(pv, ?context = context),
            LDPropertyValue.tryGetPropertyIDAsString(pv, ?context = context)
        with
        | Some name, Some value, Some id when name = LDPropertyValue.doiKey && id = LDPropertyValue.doiURL -> true
        | _ -> false

    static member validatePubMedID (pv : LDNode, ?context : LDContext) =
        LDPropertyValue.validate(pv, ?context = context)
        && 
        match
            LDPropertyValue.tryGetNameAsString(pv, ?context = context),
            LDPropertyValue.tryGetValueAsString(pv, ?context = context),
            LDPropertyValue.tryGetPropertyIDAsString(pv, ?context = context)
        with
        | Some name, Some value, Some id when name = LDPropertyValue.pubmedIDKey && id = LDPropertyValue.pubmedIDURL -> true
        | _ -> false

    static member genId(name : string, ?value : string, ?propertyID : string, ?prefix) =
        let prefix = Option.defaultValue "PV" prefix
        match value,propertyID with
        | Some value, Some pid -> $"#{prefix}_{name}_{value}"(*_{pid}*)
        | Some value, None -> $"#{prefix}_{name}_{value}"
        | None, Some pid -> $"#{prefix}_{name}"(*_{pid}*)
        | _ -> $"#{prefix}_{name}"
        |> Helper.ID.clean

    static member genIdComponent(name : string, ?value : string, ?propertyID : string) =
        LDPropertyValue.genId(name, ?value = value, ?propertyID = propertyID, prefix = "Component")

    static member genIdParameterValue(name : string, ?value : string, ?propertyID : string) =
        LDPropertyValue.genId(name, ?value = value, ?propertyID = propertyID, prefix = "ParameterValue")

    static member genIdCharacteristicValue(name : string, ?value : string, ?propertyID : string) =
        LDPropertyValue.genId(name, ?value = value, ?propertyID = propertyID, prefix = "CharacteristicValue")

    static member genIdFactorValue(name : string, ?value : string, ?propertyID : string) =
        LDPropertyValue.genId(name, ?value = value, ?propertyID = propertyID, prefix = "FactorValue")

    static member genIdFragmentDescriptor(fileName : string) =
        $"#Descriptor_{fileName}"

    static member create(name, ?value, ?id : string, ?propertyID, ?unitCode, ?unitText, ?valueReference, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> LDPropertyValue.genId(name, ?value  = value, ?propertyID = propertyID)
        let pv = LDNode(id, schemaType = ResizeArray [LDPropertyValue.schemaType], ?context = context)
        LDPropertyValue.setNameAsString(pv, name, ?context = context)
        //LDPropertyValue.setValueAsString(pv, value, ?context = context)
        pv.SetOptionalProperty(LDPropertyValue.value, value, ?context = context)
        propertyID |> Option.iter (fun pid -> LDPropertyValue.setPropertyIDAsString(pv, pid, ?context = context))
        unitCode |> Option.iter (fun uc -> LDPropertyValue.setUnitCodeAsString(pv, uc, ?context = context))
        unitText |> Option.iter (fun ut -> LDPropertyValue.setUnitTextAsString(pv, ut, ?context = context))
        valueReference |> Option.iter (fun vr -> LDPropertyValue.setValueReferenceAsString(pv, vr, ?context = context))
        pv

    static member createComponent(name, ?value, ?id, ?propertyID, ?unitCode, ?unitText, ?valueReference, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> LDPropertyValue.genIdComponent(name, ?value = value, ?propertyID = propertyID)
        let c = LDPropertyValue.create(name, id = id, ?value = value, ?propertyID = propertyID, ?unitCode = unitCode, ?unitText = unitText, ?valueReference = valueReference, ?context = context)
        c.AdditionalType <- ResizeArray ["Component"]
        c

    static member createParameterValue(name, ?value, ?id, ?propertyID, ?unitCode, ?unitText, ?valueReference, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> LDPropertyValue.genIdParameterValue(name, ?value = value, ?propertyID = propertyID)
        let pv = LDPropertyValue.create(name, id = id, ?value = value, ?propertyID = propertyID, ?unitCode = unitCode, ?unitText = unitText, ?valueReference = valueReference, ?context = context)
        pv.AdditionalType <- ResizeArray ["ParameterValue"]
        pv

    static member createCharacteristicValue(name, ?value, ?id, ?propertyID, ?unitCode, ?unitText, ?valueReference, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> LDPropertyValue.genIdCharacteristicValue(name, ?value = value, ?propertyID = propertyID)
        let cv = LDPropertyValue.create(name, id = id, ?value = value, ?propertyID = propertyID, ?unitCode = unitCode, ?unitText = unitText, ?valueReference = valueReference, ?context = context)
        cv.AdditionalType <- ResizeArray ["CharacteristicValue"]
        cv

    static member createFactorValue(name, ?value, ?id, ?propertyID, ?unitCode, ?unitText, ?valueReference, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> LDPropertyValue.genIdFactorValue(name, ?value = value, ?propertyID = propertyID)
        let fv = LDPropertyValue.create(name, id = id, ?value = value, ?propertyID = propertyID, ?unitCode = unitCode, ?unitText = unitText, ?valueReference = valueReference, ?context = context)
        fv.AdditionalType <- ResizeArray ["FactorValue"]
        fv

    static member createFragmentDescriptor(fileName, ?value, ?id, ?propertyID, ?unitCode, ?unitText, ?valueReference, ?measurementMethod, ?description, ?alternateName, ?disambiguatingDescriptions, ?subjectOf, ?context : LDContext) =
        let id = match id with
                 | Some i -> i
                 | None -> LDPropertyValue.genIdFragmentDescriptor(fileName)
        let name = "FragmentDescriptor"
        let fd = LDPropertyValue.create(name, id = id, ?value = value, ?propertyID = propertyID, ?unitCode = unitCode, ?unitText = unitText, ?valueReference = valueReference, ?context = context)
        fd.AdditionalType <- ResizeArray ["FragmentDescriptor"]
        if measurementMethod.IsSome then LDPropertyValue.setMeasurementMethodAsString(fd, measurementMethod.Value, ?context = context)
        if description.IsSome then LDPropertyValue.setDescriptionAsString(fd, description.Value, ?context = context)
        if alternateName.IsSome then LDPropertyValue.setAlternateNameAsString(fd, alternateName.Value, ?context = context)
        if disambiguatingDescriptions.IsSome then LDPropertyValue.setDisambiguatingDescriptionsAsString(fd, disambiguatingDescriptions.Value, ?context = context)
        if subjectOf.IsSome then LDPropertyValue.setSubjectOf(fd, subjectOf.Value, ?context = context)
        fd

    static member createDOI(value, ?context : LDContext) =
        let id = value
        LDPropertyValue.create(name = LDPropertyValue.doiKey, value = value, id = id, propertyID = LDPropertyValue.doiURL, ?context = context)

    static member createPubMedID(value, ?context : LDContext) =
        let id = value
        LDPropertyValue.create(name = LDPropertyValue.pubmedIDKey, value = value, id = id, propertyID = LDPropertyValue.pubmedIDURL, ?context = context)

    static member tryGetAsDOI(pv : LDNode, ?context : LDContext) =
        if LDPropertyValue.validateDOI(pv, ?context = context) then
            Some (LDPropertyValue.getValueAsString(pv, ?context = context))
        else None

    static member tryGetAsPubMedID(pv : LDNode, ?context : LDContext) =
        if LDPropertyValue.validatePubMedID(pv, ?context = context) then
            Some (LDPropertyValue.getValueAsString(pv, ?context = context))
        else None