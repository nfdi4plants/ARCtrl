namespace ARCtrl

open ARCtrl.CWL

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
                            Encode.string p.Values.[0]
                        else
                            Encode.list (p.Values |> Seq.map Encode.string |> Seq.toList)
                    (p.Key, values)
                )
                |> Seq.toList
            Encode.yMap pairs |> Encode.writeYaml

open YamlHelper

[<AttachMembers>]
type YamlController =
    static member ProcessingUnit = ProcessingUnitYAML()
    static member ParameterReference = ParameterReferenceYAML()
