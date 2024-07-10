namespace ARCtrl

open ARCtrl
open ARCtrl.Helper 
open Fable.Core

type DataContext(?id,?name : string,?dataType,?format,?selectorFormat, ?explication, ?unit, ?objectType, ?label, ?description, ?generatedBy, ?comments) =

    inherit Data(?id = id,?name = name, ?dataType = dataType, ?format = format, ?selectorFormat = selectorFormat, ?comments = comments)

    let mutable _explication : OntologyAnnotation option = explication
    let mutable _unit : OntologyAnnotation option = unit
    let mutable _objectType : OntologyAnnotation option = objectType
    let mutable _label : string option = label
    let mutable _description : string option = description
    let mutable _generatedBy : string option = generatedBy

    member this.Explication
        with get() = _explication
        and set(explication) = _explication <- explication

    member this.Unit
        with get() = _unit
        and set(unit) = _unit <- unit

    member this.ObjectType
        with get() = _objectType
        and set(objectType) = _objectType <- objectType

    member this.Label
        with get() = _label
        and set(label) = _label <- label

    member this.Description
        with get() = _description
        and set(description) = _description <- description

    member this.GeneratedBy
        with get() = _generatedBy
        and set(generatedBy) = _generatedBy <- generatedBy


    member this.AsData() =
        Data(?id = this.ID,?name = this.Name, ?dataType = this.DataType, ?format = this.Format, ?selectorFormat = this.SelectorFormat, comments = this.Comments)

    static member fromData(data : Data, ?explication,?unit,?objectType,?label, ?description,?generatedBy) =
        DataContext(?id = data.ID,?name = data.Name, ?dataType = data.DataType, ?format = data.Format, ?selectorFormat = data.SelectorFormat, ?explication = explication, ?unit = unit, ?objectType = objectType, ?label = label, ?description = description, ?generatedBy = generatedBy, comments = data.Comments)

    interface IPropertyValue with
        member this.AlternateName() = this.Label
        member this.MeasurementMethod() = this.GeneratedBy
        member this.GetCategory() = this.Explication
        member this.GetValue() = this.ObjectType |> Option.map Value.Ontology
        member this.GetUnit() = this.Unit
        member this.GetAdditionalType() = "DataContext"
        member this.Description() = this.Description

    static member createAsPV (alternateName : string option) (measurementMethod : string option) (description : string option) (category : OntologyAnnotation option) (value : Value option) (unit : OntologyAnnotation option) =
        let objectType = 
            match value with
            | Some (Value.Ontology oa)  -> 
                Some oa
            | Some v -> OntologyAnnotation(name = v.Text) |> Some
            | None -> None
        DataContext(?label = alternateName, ?generatedBy = measurementMethod, ?description = description, ?objectType = objectType, ?unit = unit, ?explication = category)

    member this.Copy() = 
        let copy = new DataContext()
        copy.ID <- this.ID
        copy.Name <- this.Name
        copy.DataType <- this.DataType
        copy.Format <- this.Format
        copy.SelectorFormat <- this.SelectorFormat
        copy.Explication <- this.Explication
        copy.Unit <- this.Unit
        copy.ObjectType <- this.ObjectType
        copy.Description <- this.Description
        copy.GeneratedBy <- this.GeneratedBy
        copy.Comments <- this.Comments
        copy

    override this.GetHashCode() = 
        [|
            HashCodes.boxHashOption this.ID
            HashCodes.boxHashOption this.Name
            HashCodes.boxHashOption this.DataType
            HashCodes.boxHashOption this.Format
            HashCodes.boxHashOption this.SelectorFormat
            HashCodes.boxHashSeq this.Comments
            HashCodes.boxHashOption this.Explication
            HashCodes.boxHashOption this.Unit
            HashCodes.boxHashOption this.ObjectType
            HashCodes.boxHashOption this.Label
            HashCodes.boxHashOption this.Description
            HashCodes.boxHashOption this.GeneratedBy
        |]
        |> HashCodes.boxHashArray
        |> fun x -> x :?> int

    override this.Equals(obj) =
        HashCodes.hash this = HashCodes.hash obj