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

    /// Returns true if the given name matches the name of the characteristic
    let nameEqualsString (name : string) (ma : MaterialAttribute) =
        match ma.CharacteristicType with
        | Some oa -> OntologyAnnotation.nameEqualsString name oa
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
