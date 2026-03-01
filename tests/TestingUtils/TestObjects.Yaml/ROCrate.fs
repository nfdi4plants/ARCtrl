/// YAML RO-Crate test objects for parser tests.
module TestObjects.Yaml.ROCrate

open System

let private yaml (s: string) =
    let lines = s.Replace("\r\n", "\n").Split('\n')
    let content =
        lines
        |> Array.skipWhile String.IsNullOrWhiteSpace
        |> Array.rev
        |> Array.skipWhile String.IsNullOrWhiteSpace
        |> Array.rev
    let minIndent =
        content
        |> Array.filter (String.IsNullOrWhiteSpace >> not)
        |> Array.map (fun l -> l.Length - l.TrimStart(' ').Length)
        |> Array.min
    content
    |> Array.map (fun l -> if l.Length >= minIndent then l.Substring(minIndent) else l)
    |> String.concat "\n"

let context_DefinedTerm = yaml """
annotationValue: http://schema.org/name
category: http://schema.org/category
unit: http://schema.org/unitCode
id: @id
type: @type
value: @value
"""

let roCrate_minimal = yaml """
@context: https://w3id.org/ro/crate/1.2/context
@graph:
  - @id: ro-crate-metadata.json
    @type: CreativeWork
    about:
      @id: ./
    conformsTo:
      @id: https://w3id.org/ro/crate/1.2
  - @id: ./
    @type: Dataset
"""

let roCrate_withTopLevelProperties = yaml """
@id: my-graph
@context: https://w3id.org/ro/crate/1.2/context
name: Example graph
version: 2
@graph:
  - @id: ./
    @type: Dataset
"""

let roCrate_invalidGraphNotSequence = yaml """
@context: https://w3id.org/ro/crate/1.2/context
@graph:
  @id: ./
  @type: Dataset
"""

let roCrate_missingGraph = yaml """
@context: https://w3id.org/ro/crate/1.2/context
"""

module GenericObjects =

    let onlyIDAndType = yaml """
@id: MyIdentifier
@type: MyType
"""

    let twoTypesAndID = yaml """
@id: MyIdentifier
@type:
  - MyType
  - MySecondType
"""

    let onlyID = yaml """
@id: MyIdentifier
"""

    let onlyType = yaml """
@type: MyType
"""

    let withStringFields = yaml """
@id: MyIdentifier
@type: MyType
name: MyName
description: MyDescription
"""

    let withIntFields = yaml """
@id: MyIdentifier
@type: MyType
number: 42
anotherNumber: 1337
"""

    let withStringArray = yaml """
@id: MyIdentifier
@type: MyType
names:
  - MyName
  - MySecondName
"""

    let withExpandedStringFieldNoType = yaml """
@id: MyIdentifier
@type: MyType
name:
  @value: MyName
"""

    let withExpandedIntFieldWithType = yaml """
@id: MyIdentifier
@type: MyType
number:
  @value: 42
  @type: http://www.w3.org/2001/XMLSchema#int
"""

    let withLDRefObject = yaml """
@id: MyIdentifier
@type: MyType
nested:
  @id: RefIdentifier
"""

    let withNestedObject = yaml """
@id: OuterIdentifier
@type: MyType
nested:
  @id: MyIdentifier
  @type: MyType
"""

    let withObjectArray = yaml """
@id: OuterIdentifier
@type: MyType
nested:
  - @id: MyIdentifier
    @type: MyType
  - @id: MySecondIdentifier
    @type: MyType
"""

    let withAdditionalTypeString = yaml """
@id: MyIdentifier
@type: MyType
additionalType: additionalType
"""

    let withAdditionalTypeArray = yaml """
@id: MyIdentifier
@type: MyType
additionalType:
  - additionalType1
  - additionalType2
"""

    let withNullValue = yaml """
@id: MyIdentifier
@type: MyType
name: null
"""

    let withoutAtKeywords = yaml """
id: MyIdentifier
type: MyType
"""

    let withMixedFields = yaml """
@id: OuterIdentifier
@type: MyType
name: MyName
number: 42
nested:
  - @id: MyIdentifier
    @type: MyType
  - Value2
  - 1337
"""
