namespace ARCtrl.Json.ROCrateContext

open Thoth.Json.Core

module Source =

  type IContext = {
    sdo : string
    arc : string

    Source: string
    ArcSource: string

    identifier: string
    characteristics: string
    name: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "bio", Encode.string "https://bioschemas.org/"

      "Source", Encode.string "bio:Sample"

      "name", Encode.string "sdo:name"
      "characteristics", Encode.string "bio:additionalProperty"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Source": "sdo:Thing",
    "ArcSource": "arc:ARC#ARC_00000071",

    "identifier": "sdo:identifier",

    "name": "arc:ARC#ARC_00000019",
    "characteristics": "arc:ARC#ARC_00000080"
  }
}
    """