namespace ARCtrl.ISA.Json.ROCrateContext

module ROCrate =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",
    
    "CreativeWork": "sdo:CreativeWork",

    "about": "sdo:about",
    "conformsTo": "sdo:conformsTo"
  }
}
    """