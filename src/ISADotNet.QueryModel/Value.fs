namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization

[<AnyOf>]
type ISAValue =
    | [<SerializationOrder(0)>] Parameter of ProcessParameterValue
    | [<SerializationOrder(1)>] Characteristic of MaterialAttributeValue
    | [<SerializationOrder(2)>] Factor of FactorValue


    member this.IsCharacteristicValue =
        match this with
        | Characteristic _  -> true
        | _                 -> false

    member this.IsParameterValue =
        match this with
        | Parameter _   -> true
        | _             -> false

    member this.IsFactorValue =
        match this with
        | Factor _  -> true
        | _         -> false

    /// Returns the ontology of the category of the ISAValue
    member this.Category =
        match this with
        | Parameter p       -> try p.Category.Value.ParameterName.Value         with | _ -> failwith $"Parameter does not contain category"
        | Characteristic c  -> try c.Category.Value.CharacteristicType.Value    with | _ -> failwith $"Characteristic does not contain category"
        | Factor f          -> try f.Category.Value.FactorType.Value            with | _ -> failwith $"Factor does not contain category"

    /// Returns the ontology of the unit of the ISAValue
    member this.Unit =
        match this with
        | Parameter p       -> try p.Unit.Value with | _ -> failwith $"Parameter {p.NameText} does not contain unit"
        | Characteristic c  -> try c.Unit.Value with | _ -> failwith $"Characteristic {c.NameText} does not contain unit"
        | Factor f          -> try f.Unit.Value with | _ -> failwith $"Factor {f.NameText} does not contain unit"

    /// Returns the value of the ISAValue
    member this.Value =
        match this with
        | Parameter p       -> try p.Value.Value with | _ -> failwith $"Parameter {p.NameText} does not contain value"
        | Characteristic c  -> try c.Value.Value with | _ -> failwith $"Characteristic {c.NameText} does not contain value"
        | Factor f          -> try f.Value.Value with | _ -> failwith $"Factor {f.NameText} does not contain value"

    /// Returns the value of the ISAValue
    member this.TryValue =
        match this with
        | Parameter p       -> try Some p.Value.Value with | _ -> None
        | Characteristic c  -> try Some c.Value.Value with | _ -> None
        | Factor f          -> try Some f.Value.Value with | _ -> None

    /// Returns the name of the Value as string
    member this.HeaderText = 
        match this with
        | Parameter p       -> try $"Parameter [{this.NameText}]"       with | _ -> failwith $"Parameter does not contain header"
        | Characteristic c  -> try $"Characteristics [{this.NameText}]" with | _ -> failwith $"Characteristic does not contain header"
        | Factor f          -> try $"Factor [{this.NameText}]"          with | _ -> failwith $"Factor does not contain header"

    /// Returns true, if the ISAValue has a unit
    member this.HasUnit =
        match this with
        | Parameter p       -> p.Unit.IsSome
        | Characteristic c  -> c.Unit.IsSome
        | Factor f          -> f.Unit.IsSome

    /// Returns true, if the ISAValue has a value
    member this.HasValue =
        match this with
        | Parameter p       -> p.Value.IsSome
        | Characteristic c  -> c.Value.IsSome
        | Factor f          -> f.Value.IsSome

    /// Returns true, if the ISAValue has a category
    member this.HasCategory = 
        match this with
        | Parameter p       -> p.Category.IsSome
        | Characteristic c  -> c.Category.IsSome
        | Factor f          -> f.Category.IsSome

    /// Returns the name of the Value as string
    member this.NameText = this.Category.NameText
  
    /// Returns the ontology of the category of the Value as string
    member this.UnitText = this.Unit.NameText

    member this.ValueText = this.Value.AsString

    member this.ValueWithUnitText =
        match this with
        | Parameter p       -> p.ValueWithUnitText
        | Characteristic c  -> c.ValueWithUnitText
        | Factor f          -> f.ValueWithUnitText

    member this.ValueIndex =
        try
            match this with
            | Parameter p       -> p.GetValueIndex()
            | Characteristic c  -> c.GetValueIndex()
            | Factor f          -> f.GetValueIndex()
        with
        | _ -> failwithf $"Value index could not be retrieved for value {this.NameText}"

    member this.TryValueIndex =
        match this with
        | Parameter p       -> p.TryGetValueIndex()
        | Characteristic c  -> c.TryGetValueIndex()
        | Factor f          -> f.TryGetValueIndex()

    member this.GetAs(targetOntology : string, ont : Obo.OboOntology) = 
        match this with
        | Parameter p       -> p.GetAs(targetOntology,ont) |> Parameter
        | Characteristic c  -> c.GetAs(targetOntology,ont) |> Characteristic
        | Factor f          -> f.GetAs(targetOntology,ont) |> Factor

    member this.TryGetAs(targetOntology : string, ont : Obo.OboOntology) = 
        match this with
        | Parameter p       -> p.TryGetAs(targetOntology,ont) |> Option.map Parameter
        | Characteristic c  -> c.TryGetAs(targetOntology,ont) |> Option.map Characteristic
        | Factor f          -> f.TryGetAs(targetOntology,ont) |> Option.map Factor