namespace ARCtrl.Json.ROCrateContext

open Thoth.Json.Core

module Data =

  type IContext = {
    sdo : string
    arc : string
    Data : string
    ArcData : string

    ``type``: string
    name: string
    comments: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"

      "Data", Encode.string "sdo:MediaObject"

      "type", Encode.string "sdo:disambiguatingDescription"
      "encodingFormat", Encode.string "sdo:encodingFormat"
      "usageInfo", Encode.string "sdo:usageInfo"
      "name", Encode.string "sdo:name"
      "comments", Encode.string "sdo:comment"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Data": "sdo:MediaObject",
    "ArcData": "arc:ARC#ARC_00000076",

    "type": "arc:ARC#ARC_00000107",

    "name": "sdo:name",
    "comments": "sdo:disambiguatingDescription"
  }
}
    """