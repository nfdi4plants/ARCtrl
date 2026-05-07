namespace ARCtrl

open ARCtrl.CWL
open Fable.Core
open ARCtrl.ROCrate
open ARCtrl.Yaml

module YamlHelper =

    [<AttachMembers>]
    type ProcessingUnitYAML() =
        member _.fromYAMLString (s: string) = Decode.decodeCWLProcessingUnit s
        member _.toYAMLString (pu: CWLProcessingUnit) = Encode.encodeProcessingUnit pu

    [<AttachMembers>]
    type ParameterReferenceYAML() =
        member _.fromYAMLString (s: string) = DecodeParameters.decodeYAMLParameterFile s
        member _.toYAMLString (pr: CWLParameterReference ResizeArray) = 
            let pairs = 
                pr 
                |> Seq.map (fun p -> 
                    let values = 
                        if p.Values.Count = 1 then
                            YAMLicious.Encode.string p.Values.[0]
                        else
                            YAMLicious.Encode.seq YAMLicious.Encode.string p.Values
                    (p.Key, values)
                )
                |> Seq.toList
            Encode.yMap pairs |> Encode.writeYaml

    [<AttachMembers>]
    type LDGraphYAML() =
        member _.fromROCrateYamlString (s : string)  = LDGraph.fromROCrateYamlString s
        member _.toROCrateYamlString(?spaces) = LDGraph.toROCrateYamlString(?spaces=spaces)

    [<AttachMembers>]
    type LDNodeYAML() =
        member _.fromROCrateYamlString (s : string) = LDNode.fromROCrateYamlString s
        member _.toROCrateYamlString(?spaces) = LDNode.toROCrateYamlString(?spaces=spaces)

open YamlHelper

[<AttachMembers>]
type YamlController =
    static member ProcessingUnit = ProcessingUnitYAML()
    static member ParameterReference = ParameterReferenceYAML()
    static member LDNode = LDNodeYAML()
    static member LDGraph = LDGraphYAML()
