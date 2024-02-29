namespace ARCtrl.ISA.Json.ROCrateContext

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

module Source =

  type IContext = {
    sdo : string
    arc : string

    Source: string
    ArcSource: string

    identifier: string
    characteristics: string
    name: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"

      "Source", Encode.string "sdo:Thing"
      "ArcSource", Encode.string "arc:ARC#ARC_00000071"

      "identifier", Encode.string "sdo:identifier"

      "name", Encode.string "arc:ARC#ARC_00000019"
      "characteristics", Encode.string "arc:ARC#ARC_00000080"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Source": "sdo:Thing",
    "ArcSource": "arc:ARC#ARC_00000071",

    "identifier": "sdo:identifier",

    "name": "arc:ARC#ARC_00000019",
    "characteristics": "arc:ARC#ARC_00000080"
  }
}
    """