namespace ARCtrl.Json.ROCrateContext

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

      "Study", Encode.string "sdo:Dataset"

      "identifier", Encode.string "sdo:identifier"
      "title", Encode.string "sdo:headline"
      "additionalType", Encode.string "sdo:additionalType"
      "description", Encode.string "sdo:description"
      "submissionDate", Encode.string "sdo:dateCreated"
      "publicReleaseDate", Encode.string "sdo:datePublished"
      "publications", Encode.string "sdo:citation"
      "people", Encode.string "sdo:creator"
      "assays", Encode.string "sdo:hasPart"
      "filename", Encode.string "sdo:alternateName"
      "comments", Encode.string "sdo:comment"
      "processSequence", Encode.string "sdo:about"
      "studyDesignDescriptors", Encode.string "arc:ARC#ARC_00000037"
    ]