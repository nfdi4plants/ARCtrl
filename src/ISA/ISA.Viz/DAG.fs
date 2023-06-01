namespace ISADotNet.Viz

open ISADotNet
open ISADotNet.API
open Cyjs.NET
open Cyjs.NET.Elements

type DAG =

    static member show (dag : CyGraph.CyGraph) = CyGraph.show dag

    static member toEmbeddedHTML (dag : CyGraph.CyGraph) = HTML.toEmbeddedHTML dag

    static member toHTML (dag : CyGraph.CyGraph) = HTML.toCytoHTML dag

    static member fromProcessSequence (ps : Process list, ?Schema : Schema) =
        
        let schema = Option.defaultValue ISADotNet.Viz.Schema.DefaultGrey Schema

        let rootNodes = API.ProcessSequence.getRootInputs ps |> List.map ProcessInput.getName

        let makeSampleName (processName : string option) (processId : int) (nameroot : string) (materialId : int) =
            match processName with
            | Some n -> $"{n}_{nameroot}{materialId}"
            | None -> $"Process{processId}_{nameroot}{materialId}"

        let edges = 
            ps
            |> List.mapi (fun pi p ->
                if p.Inputs = None then failwith $"Could no create DAG from Process sequence, process {p.Name} has no inputs"
                if p.Outputs = None then failwith $"Could no create DAG from Process sequence, process {p.Name} has no outputs"
                p.Outputs.Value
                |> List.zip p.Inputs.Value
                |> List.mapi (fun (mi) (i,o) -> 
                    (ProcessInput.tryGetName i |> function | Some n -> n | None -> makeSampleName p.Name pi "Input" mi),
                    (ProcessOutput.tryGetName o |> function | Some n -> n | None -> makeSampleName p.Name pi "Output" mi),
                    p.Name |> function | Some n -> n | None -> $"Process{pi}"
                )
                |> List.distinct
            )
            |> List.concat
            |> List.append (rootNodes |> List.map (fun i -> "Root",i,""))
                          
        let cyNodes = 
            edges
            |> List.collect (fun (i,o,e) -> [i;o])
            |> List.distinct
            |> List.map (fun n -> node n [CyParam.label n])
        
        let cyEgdes = 
            edges
            |> List.mapi (fun index (i,o,e) -> edge (string index) i o [CyParam.label e])
        
        CyGraph.initEmpty ()
        |> CyGraph.withElements cyNodes
        |> CyGraph.withElements cyEgdes
        |> CyGraph.withStyle "node"     
                [
                    CyParam.content =. CyParam.label
                    CyParam.color schema.VertexLabelColor
                    CyParam.Background.color schema.VertexColor
                ]
        |> CyGraph.withStyle "edge"     
                [
                    CyParam.content =. CyParam.label
                    CyParam.Curve.style "bezier"
                    CyParam.opacity 0.666
                    CyParam.width 7
                    CyParam.Target.Arrow.shape "triangle"
                    CyParam.Source.Arrow.shape "circle"

                    CyParam.color schema.EdgeLabelColor
                    CyParam.Line.color schema.EdgeColor
                    CyParam.Target.Arrow.color schema.EdgeColor
                    CyParam.Source.Arrow.color schema.EdgeColor
                ]
        |> CyGraph.withLayout (Layout.initBreadthfirst id) 
        |> CyGraph.withSize(800, 400) 