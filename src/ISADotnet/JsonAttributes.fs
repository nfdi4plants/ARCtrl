namespace ISADotNet

open System.Text.Json
open System.Text.Json.Serialization
open FSharp.Reflection

/// Unions marked with this attribute will be parsed like Json Enums by the JsonSerilaizer#
///
/// Use StringEnumValueAttribute to give names to the enum values. 
type StringEnumAttribute() =
    inherit System.Attribute()

/// Used on union cases of union types marked with the StringEnumAttribute
///
/// Sets the name of the enum value
type StringEnumValueAttribute(s:string) =
    inherit System.Attribute()
    member this.Value = s

/// Unions marked witht this attribute will be parsed like Json AnyOfs by the JsonSerilaizer
///
/// Use SerializationOrderAttribute to determine the order in which the cases should be tried to deserialize.
type AnyOfAttribute() =
    inherit System.Attribute()

/// Used on union cases of union types marked with the AnyOfAttribute
///
/// As in Json AnyOfs, the case name is not given. When deserializing such a value the type has to be inferred just by parsability.
///
/// The serialization order attribute arranges the order, in which the cases should be deserialized. Cases with harder parsing criteria should be given lower numbers. E.g int < string
type SerializationOrderAttribute(i:int) =
    inherit System.Attribute()
    member this.Rank = i