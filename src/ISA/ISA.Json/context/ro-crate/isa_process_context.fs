namespace ARCtrl.ISA.Json.ROCrateContext

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif

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
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"

      "Process", Encode.string "sdo:Thing"
      "ArcProcess", Encode.string "arc:ARC#ARC_00000048"

      "name", Encode.string "arc:ARC#ARC_00000019"
      "executesProtocol", Encode.string "arc:ARC#ARC_00000086"
      "performer", Encode.string "arc:ARC#ARC_00000089"
      "date", Encode.string "arc:ARC#ARC_00000090"
      "previousProcess", Encode.string "arc:ARC#ARC_00000091"
      "nextProcess", Encode.string "arc:ARC#ARC_00000092"
      "input", Encode.string "arc:ARC#ARC_00000095"
      "output", Encode.string "arc:ARC#ARC_00000096"

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