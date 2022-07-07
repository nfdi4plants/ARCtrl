namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization

[<AnyOf>]
type ISACategory =
    | [<SerializationOrder(0)>] Parameter of ProtocolParameter
    | [<SerializationOrder(1)>] Characteristic of MaterialAttribute
    | [<SerializationOrder(2)>] Factor of Factor
   
    member this.IsCharacteristicCategory =
        match this with
        | Characteristic _  -> true
        | _                 -> false

    member this.IsParameterCategory =
        match this with
        | Parameter _   -> true
        | _             -> false

    member this.IsFactorCategory =
        match this with
        | Factor _  -> true
        | _         -> false

    /// Returns the name of the Category
    member this.Name = 
        match this with
        | Parameter p       -> try p.ParameterName.Value        with | _ -> failwith $"Parameter does not contain header"
        | Characteristic c  -> try c.CharacteristicType.Value   with | _ -> failwith $"Characteristic does not contain header"
        | Factor f          -> try f.FactorType.Value           with | _ -> failwith $"Factor does not contain header"

    /// Returns the name of the Category as string
    member this.NameText = this.Name.NameText

    /// Returns the header text of the Category as string
    member this.HeaderText = 
        match this with
        | Parameter p       -> try $"Parameter [{p.NameText}]"       with | _ -> failwith $"Parameter does not contain header"
        | Characteristic c  -> try $"Characteristics [{c.NameText}]" with | _ -> failwith $"Characteristic does not contain header"
        | Factor f          -> try $"Factor [{f.NameText}]"          with | _ -> failwith $"Factor does not contain header"
