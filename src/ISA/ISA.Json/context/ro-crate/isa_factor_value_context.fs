namespace ARCtrl.ISA.Json.ROCrateContext

module FactorValue =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "FactorValue": "sdo:PropertyValue",
    "ArcFactorValue": "arc:ARC#ARC_00000084",

    "category": "arc:category",
    "value": "arc:ARC#ARC_00000044",
    "unit": "arc:ARC#ARC_00000106"
  }
}
    """