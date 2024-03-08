namespace ARCtrl.ISA.Json.ROCrateContext

open Thoth.Json.Core

module FactorValue =

  type IContext = {
    sdo : string
    arc : string
    FactorValue : string
    ArcFactorValue : string

    category: string
    value: string
    unit: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"

      "FactorValue", Encode.string "sdo:PropertyValue"
      "ArcFactorValue", Encode.string "arc:ARC#ARC_00000084"

      "category", Encode.string "arc:category"
      "value", Encode.string "arc:ARC#ARC_00000044"
      "unit", Encode.string "arc:ARC#ARC_00000106"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "FactorValue": "sdo:PropertyValue",
    "ArcFactorValue": "arc:ARC#ARC_00000084",

    "category": "arc:category",
    "value": "arc:ARC#ARC_00000044",
    "unit": "arc:ARC#ARC_00000106"
  }
}
    """