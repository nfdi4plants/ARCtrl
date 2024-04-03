namespace ARCtrl.Json.ROCrateContext

open Thoth.Json.Core

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

      "OntologyAnnotation", Encode.string "sdo:DefinedTerm"
      
      "annotationValue", Encode.string "sdo:name"
      "termSource", Encode.string "sdo:inDefinedTermSet"
      "termAccession", Encode.string "sdo:termCode"
      "comments", Encode.string "sdo:disambiguatingDescription"
    ]
