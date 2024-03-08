namespace ARCtrl.ISA.Json.ROCrateContext

open Thoth.Json.Core

module Protocol =

  type IContext = {
    sdo : string
    arc : string

    Protocol: string
    ArcProtocol: string

    name: string
    protocolType: string
    description: string
    version: string
    components: string
    parameters: string
    uri: string
    comments: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"

      "Protocol", Encode.string "sdo:Thing"
      "ArcProtocol", Encode.string "arc:ARC#ARC_00000040"

      "name", Encode.string "arc:ARC#ARC_00000019"
      "protocolType", Encode.string "arc:ARC#ARC_00000060"
      "description", Encode.string "arc:ARC#ARC_00000004"
      "version", Encode.string "arc:ARC#ARC_00000020"
      "components", Encode.string "arc:ARC#ARC_00000064"
      "parameters", Encode.string "arc:ARC#ARC_00000062"
      "uri", Encode.string "arc:ARC#ARC_00000061"
      "comments", Encode.string "arc:ARC#ARC_00000016"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Protocol": "sdo:Thing",
    "ArcProtocol": "arc:ARC#ARC_00000040",

    "name": "arc:ARC#ARC_00000019",
    "protocolType": "arc:ARC#ARC_00000060",
    "description": "arc:ARC#ARC_00000004",
    "version": "arc:ARC#ARC_00000020",
    "components": "arc:ARC#ARC_00000064",
    "parameters": "arc:ARC#ARC_00000062",
    "uri": "arc:ARC#ARC_00000061",
    "comments": "arc:ARC#ARC_00000016"
  }
}
    """