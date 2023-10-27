namespace ARCtrl.ISA.Json.ROCrateContext

module MaterialAttributeValue =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "MaterialAttributeValue": "sdo:PropertyValue",
    "ArcMaterialAttributeValue": "arc:ARC#ARC_00000079",

    "category": "arc:ARC#ARC_00000049",
    "value": "arc:ARC#ARC_00000036",
    "unit": "arc:ARC#ARC_00000106"
  }
}
   """