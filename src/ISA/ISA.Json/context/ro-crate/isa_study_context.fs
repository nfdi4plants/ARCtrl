namespace ARCtrl.ISA.Json.ROCrateContext

module Study =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Study": "sdo:Dataset",
    "ArcStudy": "arc:ARC#ARC_00000014",

    "identifier": "sdo:identifier",
    "title": "sdo:headline",
    "description": "sdo:description",
    "submissionDate": "sdo:dateCreated",
    "publicReleaseDate": "sdo:datePublished",
    "publications": "sdo:citation",
    "people": "sdo:creator",
    "assays": "sdo:hasPart",
    "filename": "sdo:description",
    "comments": "sdo:disambiguatingDescription",

    "protocols": "arc:ARC#ARC_00000039",
    "materials": "arc:ARC#ARC_00000045",
    "otherMaterials": "arc:ARC#ARC_00000045",
    "sources": "arc:ARC#ARC_00000045",
    "samples": "arc:ARC#ARC_00000045",
    "processSequence": "arc:ARC#ARC_00000047",
    "factors": "arc:ARC#ARC_00000043",
    "characteristicCategories": "arc:ARC#ARC_00000049",
    "unitCategories": "arc:ARC#ARC_00000051",
    "studyDesignDescriptors": "arc:ARC#ARC_00000037"
  }
}
    """