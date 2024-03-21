namespace ARCtrl.Json.ROCrateContext

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

      "Factor", Encode.string "sdo:DefinedTerm"

      "factorName", Encode.string "sdo:name"
      "annotationValue", Encode.string "sdo:name"
      "termSource", Encode.string "sdo:inDefinedTermSet"
      "termAccession", Encode.string "sdo:termCode"
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