namespace ARCtrl.Json.ROCrateContext

open Thoth.Json.Core

module PropertyValue =

  let context_jsonvalue =
    Encode.object [
      "sdo", Encode.string "http://schema.org/"
      "additionalType", Encode.string "sdo:additionalType"
      "alternateName", Encode.string "sdo:alternateName"
      "measurementMethod", Encode.string "sdo:measurementMethod"
      "description", Encode.string "sdo:description"
      "category", Encode.string "sdo:name"
      "categoryCode", Encode.string "sdo:propertyID"
      "value", Encode.string "sdo:value"
      "valueCode", Encode.string "sdo:valueReference"
      "unit", Encode.string "sdo:unitText"
      "unitCode", Encode.string "sdo:unitCode"
      "comments", Encode.string "sdo:disambiguatingDescription"
    ]