namespace ARCtrl.ISA.Json.ROCrateContext

module Component =

  let context =
   """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",
    
    "Component": "sdo:Thing",
    "ArcComponent": "arc:ARC#ARC_00000065",

    "componentName": "arc:ARC#ARC_00000019",
    "componentType": "arc:ARC#ARC_00000102"
  }
}
    """