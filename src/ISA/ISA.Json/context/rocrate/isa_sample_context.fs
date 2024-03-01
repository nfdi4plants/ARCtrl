namespace ARCtrl.ISA.Json.ROCrateContext

open Thoth.Json.Core

module Sample =

  type IContext = {
    sdo : string
    arc : string

    Sample: string
    ArcSample: string

    name: string
    characteristics: string
    factorValues: string
    derivesFrom: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"

      "Sample", Encode.string "sdo:Thing"
      "ArcSample", Encode.string "arc:ARC#ARC_00000070"

      "name", Encode.string "arc:name"
      "characteristics", Encode.string "arc:ARC#ARC_00000080"
      "factorValues", Encode.string "arc:ARC#ARC_00000083"
      "derivesFrom", Encode.string "arc:ARC#ARC_00000082"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Sample": "sdo:Thing",
    "ArcSample": "arc:ARC#ARC_00000070",

    "name": "arc:name",
    "characteristics": "arc:ARC#ARC_00000080",
    "factorValues": "arc:ARC#ARC_00000083",
    "derivesFrom": "arc:ARC#ARC_00000082"
  }
}
    """