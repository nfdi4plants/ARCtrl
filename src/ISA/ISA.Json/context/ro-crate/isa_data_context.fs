namespace ARCtrl.ISA.Json.ROCrateContext

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

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
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"

      "Data", Encode.string "sdo:MediaObject"
      "ArcData", Encode.string "arc:ARC#ARC_00000076"

      "type", Encode.string "arc:ARC#ARC_00000107"

      "name", Encode.string "sdo:name"
      "comments", Encode.string "sdo:disambiguatingDescription"
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