namespace ARCtrl.ISA.Json.ROCrateContext

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
      "executesProtocol", Encode.string "bio:executesProtocol"
      "parameterValues", Encode.string "bio:parameterValues"
      "performer", Encode.string "sdo:agent"
      "date", Encode.string "sdo:endTime"
      "input", Encode.string "sdo:object"
      "output", Encode.string "sdo:result"

      "comments", Encode.string "sdo:disambiguatingDescription"
    ]

  let context_str =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Process": "sdo:Thing",
    "ArcProcess": "arc:ARC#ARC_00000048",

    "name": "arc:ARC#ARC_00000019",
    "executesProtocol": "arc:ARC#ARC_00000086",
    "performer": "arc:ARC#ARC_00000089",
    "date": "arc:ARC#ARC_00000090",
    "previousProcess": "arc:ARC#ARC_00000091",
    "nextProcess": "arc:ARC#ARC_00000092",
    "input": "arc:ARC#ARC_00000095",
    "output": "arc:ARC#ARC_00000096",

    "comments": "sdo:disambiguatingDescription"
  }
}
    """