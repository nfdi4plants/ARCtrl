namespace ARCtrl.ISA.Json.ROCrateContext

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

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
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"
    
      "Comment", Encode.string "sdo:Comment"
      "name", Encode.string "sdo:name"
      "value", Encode.string "sdo:value"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",
    
    "Comment": "sdo:Comment",
    "name": "sdo:name",
    "value": "sdo:value"
  }
}
    """