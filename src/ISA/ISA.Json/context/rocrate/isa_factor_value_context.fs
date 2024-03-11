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

      "FactorValue", Encode.string "sdo:PropertyValue"

      "category", Encode.string "sdo:name"
      "categoryName", Encode.string "sdo:alternateName"
      "categoryCode", Encode.string "sdo:propertyID"
      "value", Encode.string "sdo:value"
      "valueCode", Encode.string "sdo:valueReference"
      "unit", Encode.string "sdo:unitText"
      "unitCode", Encode.string "sdo:unitCode"
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