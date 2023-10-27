namespace ARCtrl.ISA.Json.ROCrateContext

module Sample =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Sample": "sdo:Thing",
    "ArcSample": "arc:ARC#ARC_00000070",

    "name": "arc:name",
    "characteristics": "arc:ARC#ARC_00000080",
    "factorValues": "arc:ARC#ARC_00000083",
    "derivesFrom": "arc:ARC#ARC_00000082"
  }
}
    """