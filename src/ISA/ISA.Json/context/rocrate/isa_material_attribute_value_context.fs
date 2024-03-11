namespace ARCtrl.ISA.Json.ROCrateContext

open Thoth.Json.Core
module MaterialAttributeValue =

  type IContext = {
    sdo : string
    arc : string
    MaterialAttributeValue : string
    ArcMaterialAttributeValue : string

    category: string
    value: string
    unit: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"

      "MaterialAttributeValue", Encode.string "sdo:PropertyValue"

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

    "MaterialAttributeValue": "sdo:PropertyValue",
    "ArcMaterialAttributeValue": "arc:ARC#ARC_00000079",

    "category": "arc:ARC#ARC_00000049",
    "value": "arc:ARC#ARC_00000036",
    "unit": "arc:ARC#ARC_00000106"
  }
}
   """