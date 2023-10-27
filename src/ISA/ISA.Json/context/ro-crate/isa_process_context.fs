namespace ARCtrl.ISA.Json.ROCrateContext

module Process =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Process": "sdo:Thing",
    "ArcProcess": "arc:ARC#ARC_00000048",

    "name": "arc:ARC#ARC_00000019",
    "executesProtocol": "arc:ARC#ARC_00000086",
    "performer": "arc:ARC#ARC_00000089",
    "date": "arc:ARC#ARC_00000090",
    "previousProcess": "arc:ARC#ARC_00000091",
    "nextProcess": "arc:ARC#ARC_00000092",
    "input": "arc:ARC#ARC_00000095",
    "output": "arc:ARC#ARC_00000096",

    "comments": "sdo:disambiguatingDescription"
  }
}
    """