namespace ARCtrl.ISA.Json.ROCrateContext

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

module MaterialAttribute =

  type IContext = {
    sdo : string
    arc : string
    MaterialAttribute : string
    ArcMaterialAttribute : string

    characteristicType: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"

      "MaterialAttribute", Encode.string "sdo:Property"
      "ArcMaterialAttribute", Encode.string "arc:ARC#ARC_00000050"

      "characteristicType", Encode.string "arc:ARC#ARC_00000098"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "MaterialAttribute": "sdo:Property",
    "ArcMaterialAttribute": "arc:ARC#ARC_00000050",

    "characteristicType": "arc:ARC#ARC_00000098"
  }
}
    """