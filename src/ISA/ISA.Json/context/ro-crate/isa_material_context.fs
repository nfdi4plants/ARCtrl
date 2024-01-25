namespace ARCtrl.ISA.Json.ROCrateContext

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

module Material =

  type IContext = {
    sdo : string
    arc : string
    Material : string
    ArcMaterial : string

    ``type``: string
    name: string
    characteristics: string
    derivesFrom: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"

      "ArcMaterial", Encode.string "arc:ARC#ARC_00000108"
      "Material", Encode.string "sdo:Thing"

      "type", Encode.string "arc:ARC#ARC_00000085"
      "name", Encode.string "arc:ARC#ARC_00000019"
      "characteristics", Encode.string "arc:ARC#ARC_00000080"
      "derivesFrom", Encode.string "arc:ARC#ARC_00000082"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "ArcMaterial": "arc:ARC#ARC_00000108",
    "Material": "sdo:Thing",

    "type": "arc:ARC#ARC_00000085",
    "name": "arc:ARC#ARC_00000019",
    "characteristics": "arc:ARC#ARC_00000080",
    "derivesFrom": "arc:ARC#ARC_00000082"
  }
}
    """