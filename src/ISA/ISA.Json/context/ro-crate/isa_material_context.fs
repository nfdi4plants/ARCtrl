namespace ARCtrl.ISA.Json.ROCrateContext

module Material =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "ArcMaterial": "arc:ARC#ARC_00000108",
    "Material": "sdo:Thing",

    "type": "arc:ARC#ARC_00000085",
    "name": "arc:ARC#ARC_00000019",
    "characteristics": "arc:ARC#ARC_00000080",
    "derivesFrom": "arc:ARC#ARC_00000082"
  }
}
    """