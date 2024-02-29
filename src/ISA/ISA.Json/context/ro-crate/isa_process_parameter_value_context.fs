namespace ARCtrl.ISA.Json.ROCrateContext

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

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
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"

      "ProcessParameterValue", Encode.string "sdo:PropertyValue"
      "ArcProcessParameterValue", Encode.string "arc:ARC#ARC_00000088"

      "category", Encode.string "arc:ARC#ARC_00000062"
      "value", Encode.string "arc:ARC#ARC_00000087"
      "unit", Encode.string "arc:ARC#ARC_00000106"
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