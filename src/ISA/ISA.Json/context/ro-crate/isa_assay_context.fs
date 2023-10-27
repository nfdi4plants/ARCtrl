namespace ARCtrl.ISA.Json.ROCrateContext

module Assay =

  let context =
    """
{
  "@context": {
    "sdo": "https://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Assay": "sdo:Dataset",
    "ArcAssay": "arc:ARC#ARC_00000042",

    "measurementType": "sdo:variableMeasured",
    "technologyType": "sdo:measurementTechnique",
    "technologyPlatform": "sdo:instrument",
    "dataFiles": "sdo:hasPart",

    "materials": "arc:ARC#ARC_00000074",
    "otherMaterials": "arc:ARC#ARC_00000074",
    "samples": "arc:ARC#ARC_00000074",
    "characteristicCategories": "arc:ARC#ARC_00000049",
    "processSequences": "arc:ARC#ARC_00000047",
    "unitCategories": "arc:ARC#ARC_00000051",

    "comments": "sdo:disambiguatingDescription",
    "filename": "sdo:url"
  }
}
    """