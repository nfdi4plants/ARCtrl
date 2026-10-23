namespace ARCtrl

open ARCtrl.CWL
open Fable.Core

module YamlHelper =

    [<AttachMembers>]
    type ProcessingUnitYAML() =
        member _.fromYAMLString (s: string) = Decode.decodeCWLProcessingUnit s
        member _.toYAMLString (pu: CWLProcessingUnit) = Encode.encodeProcessingUnit pu

    [<AttachMembers>]
    type DocumentYAML() =
        member _.fromYAMLString (s: string) = Decode.decodeCWLDocument s
        member _.toYAMLString (document: CWLDocument) = Encode.encodeCWLDocument document

    [<AttachMembers>]
    type ParameterReferenceYAML() =
        member _.fromYAMLString (s: string) = DecodeParameters.decodeYAMLParameterFile s
        member _.toYAMLString (pr: CWLParameterReference ResizeArray) =
            Encode.encodeYAMLParameterFile pr

open YamlHelper

[<AttachMembers>]
type YamlController =
    static member ProcessingUnit = ProcessingUnitYAML()
    static member Document = DocumentYAML()
    static member ParameterReference = ParameterReferenceYAML()
