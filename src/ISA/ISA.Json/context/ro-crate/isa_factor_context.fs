namespace ARCtrl.ISA.Json.ROCrateContext

open Thoth.Json.Core

module Factor =

  type IContext = {
    sdo : string
    arc : string
    Factor : string
    ArcFactor : string

    factorName: string
    factorType: string
    comments: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"

      "Factor", Encode.string "sdo:Thing"
      "ArcFactor", Encode.string "arc:ARC#ARC_00000044"

      "factorName", Encode.string "arc:ARC#ARC_00000019"
      "factorType", Encode.string "arc:ARC#ARC_00000078"

      "comments", Encode.string "sdo:disambiguatingDescription"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Factor": "sdo:Thing",
    "ArcFactor": "arc:ARC#ARC_00000044",

    "factorName": "arc:ARC#ARC_00000019",
    "factorType": "arc:ARC#ARC_00000078",

    "comments": "sdo:disambiguatingDescription"
  }
}
    """