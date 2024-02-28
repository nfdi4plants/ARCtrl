namespace ARCtrl.ISA.Json.ROCrateContext

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

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
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"

      "MaterialAttributeValue", Encode.string "sdo:PropertyValue"
      "ArcMaterialAttributeValue", Encode.string "arc:ARC#ARC_00000079"

      "category", Encode.string "arc:ARC#ARC_00000049"
      "value", Encode.string "arc:ARC#ARC_00000036"
      "unit", Encode.string "arc:ARC#ARC_00000106"
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