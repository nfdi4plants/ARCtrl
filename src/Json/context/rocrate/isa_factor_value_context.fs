namespace ARCtrl.Json.ROCrateContext

open Thoth.Json.Core

module FactorValue =

  type IContext = {
    sdo : string
    arc : string
    FactorValue : string
    ArcFactorValue : string

    category: string
    value: string
    unit: string
  }

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"

      "FactorValue", Encode.string "sdo:PropertyValue"
      // Will define if PropertyValue is Factor/Chara.. etc.
      "additionalType", Encode.string "sdo:additionalType"
      "category", Encode.string "sdo:name"
      "categoryName", Encode.string "sdo:alternateName"
      "categoryCode", Encode.string "sdo:propertyID"
      "value", Encode.string "sdo:value"
      "valueCode", Encode.string "sdo:valueReference"
      "unit", Encode.string "sdo:unitText"
      "unitCode", Encode.string "sdo:unitCode"
    ]