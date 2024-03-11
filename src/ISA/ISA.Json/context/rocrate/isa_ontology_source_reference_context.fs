namespace ARCtrl.ISA.Json.ROCrateContext

open Thoth.Json.Core

module OntologySourceReference =

  type IContext = {
    sdo : string
    arc : string

    OntologySourceReference: string
    
    description: string
    name: string
    file: string
    version: string
    comments: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"

      "OntologySourceReference", Encode.string "sdo:DefinedTermSet"
      
      "description", Encode.string "sdo:description"
      "name", Encode.string "sdo:name"
      "file", Encode.string "sdo:url"
      "version", Encode.string "sdo:version"
      "comments", Encode.string "sdo:disambiguatingDescription"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "OntologySourceReference": "sdo:DefinedTermSet",
    
    "description": "sdo:description",
    "name": "sdo:name",
    "file": "sdo:url",
    "version": "sdo:version",
    "comments": "sdo:disambiguatingDescription"
  }
}
    """