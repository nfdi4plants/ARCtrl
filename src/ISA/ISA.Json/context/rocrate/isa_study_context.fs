namespace ARCtrl.ISA.Json.ROCrateContext

open Thoth.Json.Core

module Study =

  type IContext = {
    sdo : string
    arc : string

    Study: string
    ArcStudy: string

    identifier: string
    title: string
    description: string
    submissionDate: string
    publicReleaseDate: string
    publications: string
    people: string
    assays: string
    filename: string
    comments: string

    protocols: string
    materials: string
    otherMaterials: string
    sources: string
    samples: string
    processSequence: string
    factors: string
    characteristicCategories: string
    unitCategories: string
    studyDesignDescriptors: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"

      "Study", Encode.string "sdo:Dataset"
      "ArcStudy", Encode.string "arc:ARC#ARC_00000014"

      "identifier", Encode.string "sdo:identifier"
      "title", Encode.string "sdo:headline"
      "description", Encode.string "sdo:description"
      "submissionDate", Encode.string "sdo:dateCreated"
      "publicReleaseDate", Encode.string "sdo:datePublished"
      "publications", Encode.string "sdo:citation"
      "people", Encode.string "sdo:creator"
      "assays", Encode.string "sdo:hasPart"
      "filename", Encode.string "sdo:description"
      "comments", Encode.string "sdo:disambiguatingDescription"

      "protocols", Encode.string "arc:ARC#ARC_00000039"
      "materials", Encode.string "arc:ARC#ARC_00000045"
      "otherMaterials", Encode.string "arc:ARC#ARC_00000045"
      "sources", Encode.string "arc:ARC#ARC_00000045"
      "samples", Encode.string "arc:ARC#ARC_00000045"
      "processSequence", Encode.string "arc:ARC#ARC_00000047"
      "factors", Encode.string "arc:ARC#ARC_00000043"
      "characteristicCategories", Encode.string "arc:ARC#ARC_00000049"
      "unitCategories", Encode.string "arc:ARC#ARC_00000051"
      "studyDesignDescriptors", Encode.string "arc:ARC#ARC_00000037"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Study": "sdo:Dataset",
    "ArcStudy": "arc:ARC#ARC_00000014",

    "identifier": "sdo:identifier",
    "title": "sdo:headline",
    "description": "sdo:description",
    "submissionDate": "sdo:dateCreated",
    "publicReleaseDate": "sdo:datePublished",
    "publications": "sdo:citation",
    "people": "sdo:creator",
    "assays": "sdo:hasPart",
    "filename": "sdo:description",
    "comments": "sdo:disambiguatingDescription",

    "protocols": "arc:ARC#ARC_00000039",
    "materials": "arc:ARC#ARC_00000045",
    "otherMaterials": "arc:ARC#ARC_00000045",
    "sources": "arc:ARC#ARC_00000045",
    "samples": "arc:ARC#ARC_00000045",
    "processSequence": "arc:ARC#ARC_00000047",
    "factors": "arc:ARC#ARC_00000043",
    "characteristicCategories": "arc:ARC#ARC_00000049",
    "unitCategories": "arc:ARC#ARC_00000051",
    "studyDesignDescriptors": "arc:ARC#ARC_00000037"
  }
}
    """