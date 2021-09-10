namespace ISADotNet.Viz

open ISADotNet
open Cyjs.NET

module DAG =

    let show (dag : CyGraph.CyGraph) = CyGraph.show dag

    let toEmbeddedHTML (dag : CyGraph.CyGraph) = HTML.toEmbeddedHTML dag

    let toHTML (dag : CyGraph.CyGraph) = HTML.toCytoHTML dag

