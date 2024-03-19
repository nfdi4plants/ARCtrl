namespace ARCtrl.ISA.Json.ROCrateContext

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
    processSequences: string
    unitCategories: string

    comments: string
    filename: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"

      "Assay", Encode.string "sdo:Dataset"
      "additionalType", Encode.string "sdo:additionalType"
      "measurementType", Encode.string "sdo:variableMeasured"
      "technologyType", Encode.string "sdo:measurementTechnique"
      "technologyPlatform", Encode.string "sdo:instrument"
      "dataFiles", Encode.string "sdo:hasPart"

      "processSequences", Encode.string "sdo:about"

      "comments", Encode.string "sdo:comment"
      "filename", Encode.string "sdo:url"
    ]