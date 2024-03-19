namespace ARCtrl.ISA.Json.ROCrateContext

open Thoth.Json.Core

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