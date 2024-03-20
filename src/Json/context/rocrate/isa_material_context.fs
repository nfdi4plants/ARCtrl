namespace ARCtrl.ISA.Json.ROCrateContext

open Thoth.Json.Core

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
      "bio", Encode.string "https://bioschemas.org/"

      "Material", Encode.string "bio:Sample"

      "type", Encode.string "sdo:disambiguatingDescription"
      "name", Encode.string "sdo:name"
      "characteristics", Encode.string "bio:additionalProperty"
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