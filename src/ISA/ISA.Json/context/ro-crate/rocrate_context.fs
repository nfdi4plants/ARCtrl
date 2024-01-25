namespace ARCtrl.ISA.Json.ROCrateContext

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

module ROCrate =

  type IContext = {
    sdo : string
    arc : string

    CreativeWork: string

    about: string
    conformsTo: string
  }

  let conformsTo_jsonvalue = 
    Encode.object [
      "@id", Encode.string "https://w3id.org/ro/crate/1.1"
    ]

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"
    
      "CreativeWork", Encode.string "sdo:CreativeWork"

      "about", Encode.string "sdo:about"
      "conformsTo", Encode.string "sdo:conformsTo"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",
    
    "CreativeWork": "sdo:CreativeWork",

    "about": "sdo:about",
    "conformsTo": "sdo:conformsTo"
  }
}
    """