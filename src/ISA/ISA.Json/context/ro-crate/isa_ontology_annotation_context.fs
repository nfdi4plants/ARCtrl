namespace ARCtrl.ISA.Json.ROCrateContext

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

module OntologyAnnotation =

  type IContext = {
    sdo : string
    arc : string
    OntologyAnnotation : string

    annotationValue: string
    termSource: string
    termAccession: string
    comments: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"

      "OntologyAnnotation", Encode.string "sdo:DefinedTerm"
      
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

    "OntologyAnnotation": "sdo:DefinedTerm",
    
    "annotationValue": "sdo:name",
    "termSource": "sdo:inDefinedTermSet",
    "termAccession": "sdo:termCode",
    "comments": "sdo:disambiguatingDescription"
  }
}
    """