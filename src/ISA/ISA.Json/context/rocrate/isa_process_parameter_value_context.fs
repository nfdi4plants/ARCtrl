namespace ARCtrl.ISA.Json.ROCrateContext

open Thoth.Json.Core

module ProcessParameterValue =

  type IContext = {
    sdo : string
    arc : string
    ProcessParameterValue : string
    ArcProcessParameterValue : string

    category: string
    value: string
    unit: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"

      "ProcessParameterValue", Encode.string "sdo:PropertyValue"

      "category", Encode.string "sdo:name"
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

    "ProcessParameterValue": "sdo:PropertyValue",
    "ArcProcessParameterValue": "arc:ARC#ARC_00000088",

    "category": "arc:ARC#ARC_00000062",
    "value": "arc:ARC#ARC_00000087",
    "unit": "arc:ARC#ARC_00000106"
  }
}
    """