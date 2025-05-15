namespace ARCtrl.ROCrate

open DynamicObj
open Fable.Core
open ARCtrl.ROCrate

[<AttachMembers>]
type FormalParameter(
    id: string,
    name: string,
    ?description: string,
    ?valueType: string,
    ?defaultValue: string,
    ?minimumValue: string,
    ?maximumValue: string,
    ?unit: string,
    ?required: bool
) as this =
    inherit LDNode(id = id, schemaType = ResizeArray ["FormalParameter"])
    do
        DynObj.setProperty (nameof name) name this
        DynObj.setOptionalProperty (nameof description) description this
        DynObj.setOptionalProperty (nameof valueType) valueType this
        DynObj.setOptionalProperty (nameof defaultValue) defaultValue this
        DynObj.setOptionalProperty (nameof minimumValue) minimumValue this
        DynObj.setOptionalProperty (nameof maximumValue) maximumValue this
        DynObj.setOptionalProperty (nameof unit) unit this
        DynObj.setOptionalProperty (nameof required) required this

    member this.GetName() = DynObj.getMandatoryDynamicPropertyOrThrow<string> "FormalParameter" (nameof name) this
    static member getName = fun (fp: FormalParameter) -> fp.GetName()

    member this.GetDescription() = 
        match DynObj.tryGetPropertyValue "description" this with
        | Some desc -> Some desc
        | None -> None
    static member getDescription = fun (fp: FormalParameter) -> fp.GetDescription()

    member this.GetValueType() = 
        match DynObj.tryGetPropertyValue "valueType" this with
        | Some valueType -> valueType
        | None -> None
    static member getValueType = fun (fp: FormalParameter) -> fp.GetValueType()

    member this.GetDefaultValue() = 
        match DynObj.tryGetPropertyValue "defaultValue" this with
        | Some value -> Some value
        | None -> None
    static member getDefaultValue = fun (fp: FormalParameter) -> fp.GetDefaultValue()

    member this.GetMinimumValue() = 
        match DynObj.tryGetPropertyValue "minimumValue" this with
        | Some value -> Some value
        | None -> None
    static member getMinimumValue = fun (fp: FormalParameter) -> fp.GetMinimumValue()

    member this.GetMaximumValue() = 
        match DynObj.tryGetPropertyValue "maximumValue" this with
        | Some value -> Some value
        | None -> None
    static member getMaximumValue = fun (fp: FormalParameter) -> fp.GetMaximumValue()

    member this.GetUnit() = 
        match DynObj.tryGetPropertyValue "unit" this with
        | Some unit -> Some unit
        | None -> None
    static member getUnit = fun (fp: FormalParameter) -> fp.GetUnit()

    member this.IsRequired() = 
        match DynObj.tryGetPropertyValue "required" this with
        | Some required -> Some required
        | None -> None
    static member isRequired = fun (fp: FormalParameter) -> fp.IsRequired()
