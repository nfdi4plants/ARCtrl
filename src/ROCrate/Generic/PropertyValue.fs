namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

///
[<AttachMembers>]
type PropertyValue =

    static member schemaType = "http://schema.org/PropertyValue"

    static member name = "http://schema.org/name"

    static member value = "http://schema.org/value"

    static member propertyID = "http://schema.org/propertyID"

    static member unitCode = "http://schema.org/unitCode"

    static member unitText = "http://schema.org/unitText"

    static member valueReference = "http://schema.org/valueReference"

    static member tryGetNameAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(PropertyValue.name, ?context = context) with
        | Some (:? string as n) -> Some n
        | _ -> None

    static member getNameAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(PropertyValue.name, ?context = context) with
        | Some (:? string as n) -> n
        | Some _ -> failwith $"Property of `name` of object with @id `{pv.Id}` was not a string"
        | _ -> failwith $"Could not access property `name` of object with @id `{pv.Id}`"

    static member setNameAsString(pv : LDNode, name : string, ?context : LDContext) =
        pv.SetProperty(PropertyValue.name, name, ?context = context)

    static member tryGetValueAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(PropertyValue.value, ?context = context) with
        | Some (:? string as v) -> Some v
        | _ -> None

    static member getValueAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(PropertyValue.value, ?context = context) with
        | Some (:? string as v) -> v
        | Some _ -> failwith $"Property of `value` of object with @id `{pv.Id}` was not a string"
        | _ -> failwith $"Could not access property `value` of object with @id `{pv.Id}`"

    static member setValueAsString(pv : LDNode, value : string, ?context : LDContext) =
        pv.SetProperty(PropertyValue.value, value, ?context = context)

    static member tryGetPropertyIDAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(PropertyValue.propertyID, ?context = context) with
        | Some (:? string as pid) -> Some pid
        | _ -> None

    static member setPropertyIDAsString(pv : LDNode, propertyID : string, ?context : LDContext) =
        pv.SetProperty(PropertyValue.propertyID, propertyID, ?context = context)

    static member tryGetUnitCodeAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(PropertyValue.unitCode, ?context = context) with
        | Some (:? string as uc) -> Some uc
        | _ -> None

    static member setUnitCodeAsString(pv : LDNode, unitCode : string, ?context : LDContext) =
        pv.SetProperty(PropertyValue.unitCode, unitCode, ?context = context)

    static member tryGetUnitTextAsString(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(PropertyValue.unitText, ?context = context) with
        | Some (:? string as ut) -> Some ut
        | _ -> None

    static member setUnitTextAsString(pv : LDNode, unitText : string, ?context : LDContext) =
        pv.SetProperty(PropertyValue.unitText, unitText, ?context = context)

    static member tryGetValueReference(pv : LDNode, ?context : LDContext) =
        match pv.TryGetPropertyAsSingleton(PropertyValue.valueReference, ?context = context) with
        | Some (:? string as vr) -> Some vr
        | _ -> None

    static member setValueReference(pv : LDNode, valueReference : string, ?context : LDContext) =
        pv.SetProperty(PropertyValue.valueReference, valueReference, ?context = context)

    static member validate(pv : LDNode, ?context : LDContext) =
        pv.HasType(PropertyValue.schemaType, ?context = context)
        && pv.HasProperty(PropertyValue.name, ?context = context)
        && pv.HasProperty(PropertyValue.value, ?context = context)

    static member create(id, name, value, ?propertyID, ?unitCode, ?unitText, ?valueReference, ?context : LDContext) =
        let pv = LDNode(id, schemaType = ResizeArray [PropertyValue.schemaType], ?context = context)
        PropertyValue.setNameAsString(pv, name, ?context = context)
        PropertyValue.setValueAsString(pv, value, ?context = context)
        propertyID |> Option.iter (fun pid -> PropertyValue.setPropertyIDAsString(pv, pid, ?context = context))
        unitCode |> Option.iter (fun uc -> PropertyValue.setUnitCodeAsString(pv, uc, ?context = context))
        unitText |> Option.iter (fun ut -> PropertyValue.setUnitTextAsString(pv, ut, ?context = context))
        valueReference |> Option.iter (fun vr -> PropertyValue.setValueReference(pv, vr, ?context = context))
        pv

    static member createComponent(name, value, ?propertyID, ?unitCode, ?unitText, ?valueReference, ?context : LDContext) =
        let id = $"Component_{name}_{value}"
        let c = PropertyValue.create(id, name, value, ?propertyID = propertyID, ?unitCode = unitCode, ?unitText = unitText, ?valueReference = valueReference, ?context = context)
        c.AdditionalType <- ResizeArray ["Component"]
        c

    static member createParameterValue(name, value, ?propertyID, ?unitCode, ?unitText, ?valueReference, ?context : LDContext) =
        let id = $"ParameterValue_{name}_{value}"
        let pv = PropertyValue.create(id, name, value, ?propertyID = propertyID, ?unitCode = unitCode, ?unitText = unitText, ?valueReference = valueReference, ?context = context)
        pv.AdditionalType <- ResizeArray ["ParameterValue"]
        pv

    static member createCharacteristicValue(name, value, ?propertyID, ?unitCode, ?unitText, ?valueReference, ?context : LDContext) =
        let id = $"CharacteristicValue_{name}_{value}"
        let cv = PropertyValue.create(id, name, value, ?propertyID = propertyID, ?unitCode = unitCode, ?unitText = unitText, ?valueReference = valueReference, ?context = context)
        cv.AdditionalType <- ResizeArray ["CharacteristicValue"]
        cv

    static member createFactorValue(name, value, ?propertyID, ?unitCode, ?unitText, ?valueReference, ?context : LDContext) =
        let id = $"FactorValue_{name}_{value}"
        let fv = PropertyValue.create(id, name, value, ?propertyID = propertyID, ?unitCode = unitCode, ?unitText = unitText, ?valueReference = valueReference, ?context = context)
        fv.AdditionalType <- ResizeArray ["FactorValue"]
        fv