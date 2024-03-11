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
    
      "Component", Encode.string "sdo:PropertyValue"

      "category", Encode.string "sdo:name"
      "categoryCode", Encode.string "sdo:propertyID"
      "value", Encode.string "sdo:value"
      "valueCode", Encode.string "sdo:valueReference"
      "unit", Encode.string "sdo:unitText"
      "unitCode", Encode.string "sdo:unitCode"
    ]

  let context_str =
   """
{
  "@context": {
    "sdo": "http://schema.org/",
    
    "Component": "sdo:PropertyValue",

    "componentName": "sdo",
    "componentType": "arc:ARC#ARC_00000102"
  }
}
    """