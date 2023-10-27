namespace ARCtrl.ISA.Json.ROCrateContext

module ProcessParameterValue =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "ProcessParameterValue": "sdo:PropertyValue",
    "ArcProcessParameterValue": "arc:ARC#ARC_00000088",

    "category": "arc:ARC#ARC_00000062",
    "value": "arc:ARC#ARC_00000087",
    "unit": "arc:ARC#ARC_00000106"
  }
}
    """