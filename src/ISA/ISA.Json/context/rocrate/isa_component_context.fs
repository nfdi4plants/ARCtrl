namespace ARCtrl.ISA.Json.ROCrateContext
open Thoth.Json.Core

module Component =

  type IContext = {
    sdo : string
    arc : string
    Component : string
    ArcComponent : string

    componentName: string
    componentType: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "arc", Encode.string "http://purl.org/nfdi4plants/ontology/"
    
      "Component", Encode.string "sdo:Thing"
      "ArcComponent", Encode.string "arc:ARC#ARC_00000065"

      "componentName", Encode.string "arc:ARC#ARC_00000019"
      "componentType", Encode.string "arc:ARC#ARC_00000102"
    ]

  let context_str =
   """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",
    
    "Component": "sdo:Thing",
    "ArcComponent": "arc:ARC#ARC_00000065",

    "componentName": "arc:ARC#ARC_00000019",
    "componentType": "arc:ARC#ARC_00000102"
  }
}
    """