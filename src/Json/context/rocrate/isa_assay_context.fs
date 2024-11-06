namespace ARCtrl.Json.ROCrateContext

open Thoth.Json.Core

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
    processSequence: string
    unitCategories: string

    comments: string
    filename: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "Assay", Encode.string "sdo:Dataset"

      "identifier", Encode.string "sdo:identifier"
      "additionalType", Encode.string "sdo:additionalType"
      "measurementType", Encode.string "sdo:variableMeasured"
      "technologyType", Encode.string "sdo:measurementTechnique"
      "technologyPlatform", Encode.string "sdo:measurementMethod"
      "dataFiles", Encode.string "sdo:hasPart"
      "performers", Encode.string "sdo:creator"
      "processSequence", Encode.string "sdo:about"

      "comments", Encode.string "sdo:comment"
      "filename", Encode.string "sdo:url"
    ]