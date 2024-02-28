namespace ARCtrl.ISA.Json.ROCrateContext

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

module ProtocolParameter =

  type IContext = {
    sdo : string
    arc : string

    ProtocolParamter: string
    ArcProtocolParameter: string

    parameterName: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"

      "ProtocolParameter", Encode.string "sdo:Thing"
      "ArcProtocolParameter", Encode.string "arc:ARC#ARC_00000063"

      "parameterName", Encode.string "arc:ARC#ARC_00000100"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "ProtocolParameter": "sdo:Thing",
    "ArcProtocolParameter": "arc:ARC#ARC_00000063",

    "parameterName": "arc:ARC#ARC_00000100"
  }
}
    """