namespace ARCtrl.ISA.Json.ROCrateContext

module Factor =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Factor": "sdo:Thing",
    "ArcFactor": "arc:ARC#ARC_00000044",

    "factorName": "arc:ARC#ARC_00000019",
    "factorType": "arc:ARC#ARC_00000078",

    "comments": "sdo:disambiguatingDescription"
  }
}
    """