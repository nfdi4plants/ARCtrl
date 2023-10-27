namespace ARCtrl.ISA.Json.ROCrateContext

module MaterialAttribute =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "MaterialAttribute": "sdo:Property",
    "ArcMaterialAttribute": "arc:ARC#ARC_00000050",

    "characteristicType": "arc:ARC#ARC_00000098"
  }
}
    """