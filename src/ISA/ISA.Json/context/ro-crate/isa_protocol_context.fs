namespace ARCtrl.ISA.Json.ROCrateContext

module Protocol =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Protocol": "sdo:Thing",
    "ArcProtocol": "arc:ARC#ARC_00000040",

    "name": "arc:ARC#ARC_00000019",
    "protocolType": "arc:ARC#ARC_00000060",
    "description": "arc:ARC#ARC_00000004",
    "version": "arc:ARC#ARC_00000020",
    "components": "arc:ARC#ARC_00000064",
    "parameters": "arc:ARC#ARC_00000062",
    "uri": "arc:ARC#ARC_00000061",
    "comments": "arc:ARC#ARC_00000016"
  }
}
    """