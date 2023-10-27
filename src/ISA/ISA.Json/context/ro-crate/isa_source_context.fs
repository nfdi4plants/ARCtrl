namespace ARCtrl.ISA.Json.ROCrateContext

module Source =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Source": "sdo:Thing",
    "ArcSource": "arc:ARC#ARC_00000071",

    "identifier": "sdo:identifier",

    "name": "arc:ARC#ARC_00000019",
    "characteristics": "arc:ARC#ARC_00000080"
  }
}
    """