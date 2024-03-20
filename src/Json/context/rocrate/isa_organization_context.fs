namespace ARCtrl.ISA.Json.ROCrateContext

open Thoth.Json.Core

module Organization =

  type IContext = {
    sdo : string
    arc : string

    Organization: string
    
    name: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"

      "Organization", Encode.string "sdo:Organization"
      
      "name", Encode.string "sdo:name"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Organization": "sdo:Organization",
    
    "name": "sdo:name"
  }
}
    """