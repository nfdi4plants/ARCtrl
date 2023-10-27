namespace ARCtrl.ISA.Json.ROCrateContext

module Organization =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Organization": "sdo:Organization",
    
    "name": "sdo:name"
  }
}
    """