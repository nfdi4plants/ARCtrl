namespace ARCtrl.Json.ROCrateContext

open Thoth.Json.Core

module Process =

  type IContext = {
    sdo : string
    arc : string

    Process: string
    ArcProcess: string

    name: string
    executesProtocol: string
    performer: string
    date: string
    previousProcess: string
    nextProcess: string
    input: string
    output: string

    comments: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "bio", Encode.string "https://bioschemas.org/"

      "Process", Encode.string "bio:LabProcess"

      "name", Encode.string "sdo:name"
      "executesProtocol", Encode.string "bio:executesLabProtocol"
      "parameterValues", Encode.string "bio:parameterValue"
      "performer", Encode.string "sdo:agent"
      "date", Encode.string "sdo:endTime"
      "inputs", Encode.string "sdo:object"
      "outputs", Encode.string "sdo:result"

      "comments", Encode.string "sdo:disambiguatingDescription"
    ]