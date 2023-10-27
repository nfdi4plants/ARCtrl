namespace ARCtrl.ISA.Json.ROCrateContext

module Data =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Data": "sdo:MediaObject",
    "ArcData": "arc:ARC#ARC_00000076",

    "type": "arc:ARC#ARC_00000107",

    "name": "sdo:name",
    "comments": "sdo:disambiguatingDescription"
  }
}
    """