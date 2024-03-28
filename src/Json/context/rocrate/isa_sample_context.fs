namespace ARCtrl.Json.ROCrateContext

open Thoth.Json.Core

module Sample =

  type IContext = {
    sdo : string
    arc : string

    Sample: string
    ArcSample: string

    name: string
    characteristics: string
    factorValues: string
    derivesFrom: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "bio", Encode.string "https://bioschemas.org/"

      "Sample", Encode.string "bio:Sample"

      "name", Encode.string "sdo:name"
      "additionalProperties", Encode.string "bio:additionalProperty"
    ]