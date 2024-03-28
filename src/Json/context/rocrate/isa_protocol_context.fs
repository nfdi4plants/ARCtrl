namespace ARCtrl.Json.ROCrateContext

open Thoth.Json.Core

module Protocol =

  type IContext = {
    sdo : string
    arc : string

    Protocol: string
    ArcProtocol: string

    name: string
    protocolType: string
    description: string
    version: string
    components: string
    parameters: string
    uri: string
    comments: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "bio", Encode.string "https://bioschemas.org/"

      "Protocol", Encode.string "bio:LabProtocol"

      "name", Encode.string "sdo:name"
      "protocolType", Encode.string "bio:intendedUse"
      "description", Encode.string "sdo:description"
      "version", Encode.string "sdo:version"
      "components", Encode.string "bio:labEquipment"
      "reagents", Encode.string "bio:reagent"
      "computationalTools", Encode.string "bio:computationalTool"

      "uri", Encode.string "sdo:url"
      "comments", Encode.string "sdo:comment"
    ]
