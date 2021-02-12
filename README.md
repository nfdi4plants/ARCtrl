# ISADotNet
ISA compliant experimental metadata toolkit in F#

The library contains types and functionality for creating and working on experimental metadata in ISA format. 
Additionally, the types can easily be written to and read from `Json` files in [ISAJson format](https://isa-specs.readthedocs.io/en/latest/isajson.html) and Microsoft `Excel` files in [ISATab format](https://isa-specs.readthedocs.io/en/latest/isatab.html).



# Installation

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

