namespace ARCtrl.Json.ROCrateContext

open Thoth.Json.Core

module Comment =

  type IContext = {
    sdo : string
    arc : string
    Comment : string

    name: string
    value: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
    
      "Comment", Encode.string "sdo:Comment"
      "name", Encode.string "sdo:name"
      "value", Encode.string "sdo:text"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    
    "Comment": "sdo:Comment",
    "name": "sdo:name",
    "value": "sdo:text"
  }
}
    """