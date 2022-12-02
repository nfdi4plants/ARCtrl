namespace ISADotNet.API

open ISADotNet

module MaterialAttribute =

    /// Returns the name of the characteristic as string if it exists
    let tryGetNameAsString (ma : MaterialAttribute) =
        ma.CharacteristicType
        |> Option.bind (OntologyAnnotation.tryGetNameAsString)

    /// Returns the name of the characteristic as string
    let getNameAsString (ma : MaterialAttribute) =
        tryGetNameAsString ma
        |> Option.defaultValue ""

    /// Returns the name of the characteristic and its number as string (e.g. "temperature #2")
    let getNameAsStringWithNumber (ma : MaterialAttribute) =
        ma.CharacteristicType
        |> Option.map (OntologyAnnotation.getNameAsStringWithNumber)
        |> Option.defaultValue ""

    /// Returns true if the given name matches the name of the characteristic
    let nameEqualsString (name : string) (ma : MaterialAttribute) =
        match ma.CharacteristicType with
        | Some oa -> OntologyAnnotation.nameEqualsString name oa
        | None -> false

    /// Returns true if the given numbered name matches the name of the characteristic (e.g. "temperature #2")
    let nameWithNumberEqualsString (name : string) (ma : MaterialAttribute) =
        match ma.CharacteristicType with
        | Some oa -> OntologyAnnotation.nameWithNumberEqualsString name oa
        | None -> false

module MaterialAttributeValue =

    /// Returns the name of the characteristic value as string if it exists
    let tryGetNameAsString (mv : MaterialAttributeValue) =
        mv.Category
        |> Option.bind (MaterialAttribute.tryGetNameAsString)

    /// Returns the name of the characteristic value as string
    let getNameAsString (mv : MaterialAttributeValue) =
        tryGetNameAsString mv
        |> Option.defaultValue ""

    /// Returns true if the given name matches the name of the characteristic value
    let nameEqualsString (name : string) (mv : MaterialAttributeValue) =
        match mv.Category with
        | Some oa -> MaterialAttribute.nameEqualsString name oa
        | None -> false

    /// Returns the value of the characteristic value as string if it exists (with unit)
    let tryGetValueAsString (mv : MaterialAttributeValue) =
        let unit = mv.Unit |> Option.bind (OntologyAnnotation.tryGetNameAsString)
        mv.Value
        |> Option.map (fun v ->
            let s = v |> Value.toString
            match unit with
            | Some u -> s + " " + u
            | None -> s
        )

    /// Returns the value of the characteristic value as string (with unit)
    let getValueAsString (mv : MaterialAttributeValue) =
        tryGetValueAsString mv
        |> Option.defaultValue ""


module Material = 
    
    let getUnits (m:Material) = 
        m.Characteristics
        |> Option.defaultValue []
        |> List.choose (fun c -> c.Unit)

module Source = 
    
    let getUnits (s:Source) = 
        s.Characteristics
        |> Option.defaultValue []
        |> List.choose (fun c -> c.Unit)

module Sample = 

    let getCharacteristicUnits (s:Sample) =
        s.Characteristics
        |> Option.defaultValue []
        |> List.choose (fun c -> c.Unit)

    let getFactorUnits (s:Sample) =
        s.FactorValues
        |> Option.defaultValue []
        |> List.choose (fun c -> c.Unit)

    let getUnits (s:Sample) =
        List.append (getCharacteristicUnits s) (getFactorUnits s)
        
        