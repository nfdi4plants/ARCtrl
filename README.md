# ISADotNet

[![Build and test](https://github.com/nfdi4plants/ISADotNet/actions/workflows/build-test.yml/badge.svg?branch=developer)](https://github.com/nfdi4plants/ISADotNet/actions/workflows/build-test.yml)

ISA compliant experimental metadata toolkit in F#

The library contains types and functionality for creating and working on experimental metadata in ISA format. 
Additionally, the types can easily be written to and read from `Json` files in [ISAJson format](https://isa-specs.readthedocs.io/en/latest/isajson.html) and Microsoft `Excel` files in [ISATab format](https://isa-specs.readthedocs.io/en/latest/isatab.html).

# Installation

![Nuget](https://img.shields.io/nuget/v/ISADotNet?logo=Nuget)

The `ISADotNet` nuget package can be found [here](https://www.nuget.org/packages/ISADotNet/)

The `ISADotNet.XLSX` nuget package can be found [here](https://www.nuget.org/packages/ISADotNet.XLSX/)

Adding a package reference via dotnet:
`dotnet add package ISADotNet --version 0.1.0`

Adding a package reference in F# interactive:
`#r "nuget: ISADotNet, 0.1.0"`

# What is ISA?

ISA is a specification for annotation of research data. The metadata in nested in three different layers: <b>I</b>nvestigation, <b>S</b>tudy and <b>A</b>ssay.

Around these three main entities, the following abstract datamodel is specified:

![Abstract Datamodel](https://isa-specs.readthedocs.io/en/latest/_images/isa_model_1_ccoded.png)
Source: https://isa-specs.readthedocs.io/en/latest/_images/isa_model_1_ccoded.png

Additional Info: https://isa-specs.readthedocs.io/en/latest/isamodel.html

# Usage

## Querymodel

The querymodel is used to intuitively retreive information from the ISA metadata. 

```fsharp

#r @"nuget: ISADotNet.XLSX"

open ISADotNet


let _,_,persons,a = ISADotNet.XLSX.AssayFile.Assay.fromFile assayPath

let qa = QueryModel.QAssay.fromAssay a

```

### Retreiving values:

```fsharp
let firstValue = qa.Values().First

firstValue.NameText
firstValue.ValueText
```
-> 
```
val it: string = "Sample type"

val it: string = "cell culture"
```

### Retreiving nodes:

```fsharp
qa.Sources().Head
qa.FirstSamples().Head
qa.FirstData().Head
qa.LastData().Head
```
-> 
```

val it: string = "001_uncult_8°"

val it: string = "001-007_uncult_8°_son"

val it: string = "20210913_1558_001.mzml"

val it: string = "20210913_1558_001.mzml"
```

### Nodes as anchors:

```fsharp
let nodeOfInterest = qa.Samples().[5]

qa.FirstProcessedDataOf(nodeOfInterest)
```
->
```
val nodeOfInterest: string = "008-014_uncult_22°_ext"

val it: string list =
  ["20210913_1558_021.mzml"; "20210913_1558_022.mzml";
   "20210913_1558_023.mzml"; "20210913_1558_024.mzml";
   "20210913_1558_025.mzml"; "20210913_1558_026.mzml";
   "20210913_1558_027.mzml"; "20210913_1558_028.mzml";
   "20210913_1558_029.mzml"; "20210913_1558_030.mzml";
   "20210913_1558_031.mzml"; "20210913_1558_032.mzml";
   "20210913_1558_033.mzml"; "20210913_1558_034.mzml";
   "20210913_1558_035.mzml"; "20210913_1558_036.mzml";
   "20210913_1558_037.mzml"; "20210913_1558_038.mzml";
   "20210913_1558_039.mzml"; "20210913_1558_040.mzml"]
```
----
```fsharp
qa.SourcesOf(nodeOfInterest)
```
->
```
val it: string list =
  ["008_uncult_22°"; "009_uncult_22°"; "010_uncult_22°"; "011_uncult_22°";
   "012_uncult_22°"; "013_uncult_22°"; "014_uncult_22°"]
```
----
```fsharp
qa.PreviousCharacteristicsOf(nodeOfInterest)
    .Item("Organism")
    .ValueText
```
->
```
val it: string = "Chlamydomonas rheinhardtii"
```
----
```fsharp
qa.SucceedingParametersOf(nodeOfInterest)
|> Seq.map (fun p -> p.HeaderText,p.ValueText)
|> Seq.take 5
|> Seq.toList
```
->
```
val it: (string * string) list =
  [("Parameter [Library strategy]", "Illumina library prep");
   ("Parameter [Library Selection]", "cDNA");
   ("Parameter [Library layout]", "single-end");
   ("Parameter [Library preparation kit]",
    "CleanTag® Small RNA Library Prep Kit");
   ("Parameter [Library preparation kit version]", "3")]
```
