namespace ARCtrl.ISA.Json.ROCrateContext

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

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
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"

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