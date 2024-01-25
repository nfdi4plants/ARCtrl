namespace ARCtrl.ISA.Json.ROCrateContext

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

module Assay =

  type IContext = {
    sdo : string
    arc : string
    Assay : string
    ArcAssay: string

    measurementType: string
    technologyType: string
    technologyPlatform: string
    dataFiles: string

    materials: string
    otherMaterials: string
    samples: string
    characteristicCategories: string
    processSequences: string
    unitCategories: string

    comments: string
    filename: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"

      "Assay", Encode.string "sdo:Dataset"
      "ArcAssay", Encode.string "arc:ARC#ARC_00000042"

      "measurementType", Encode.string "sdo:variableMeasured"
      "technologyType", Encode.string "sdo:measurementTechnique"
      "technologyPlatform", Encode.string "sdo:instrument"
      "dataFiles", Encode.string "sdo:hasPart"

      "materials", Encode.string "arc:ARC#ARC_00000074"
      "otherMaterials", Encode.string "arc:ARC#ARC_00000074"
      "samples", Encode.string "arc:ARC#ARC_00000074"
      "characteristicCategories", Encode.string "arc:ARC#ARC_00000049"
      "processSequences", Encode.string "arc:ARC#ARC_00000047"
      "unitCategories", Encode.string "arc:ARC#ARC_00000051"

      "comments", Encode.string "sdo:disambiguatingDescription"
      "filename", Encode.string "sdo:url"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "https://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Assay": "sdo:Dataset",
    "ArcAssay": "arc:ARC#ARC_00000042",

    "measurementType": "sdo:variableMeasured",
    "technologyType": "sdo:measurementTechnique",
    "technologyPlatform": "sdo:instrument",
    "dataFiles": "sdo:hasPart",

    "materials": "arc:ARC#ARC_00000074",
    "otherMaterials": "arc:ARC#ARC_00000074",
    "samples": "arc:ARC#ARC_00000074",
    "characteristicCategories": "arc:ARC#ARC_00000049",
    "processSequences": "arc:ARC#ARC_00000047",
    "unitCategories": "arc:ARC#ARC_00000051",

    "comments": "sdo:disambiguatingDescription",
    "filename": "sdo:url"
  }
}
    """