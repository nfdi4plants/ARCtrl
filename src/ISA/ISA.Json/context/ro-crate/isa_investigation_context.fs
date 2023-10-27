namespace ARCtrl.ISA.Json.ROCrateContext

module Investigation =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Investigation": "sdo:Dataset",

    "identifier" : "sdo:identifier",
    "title": "sdo:headline",
    "description": "sdo:description",
    "submissionDate": "sdo:dateCreated",
    "publicReleaseDate": "sdo:datePublished",
    "publications": "sdo:citation",
    "people": "sdo:creator",
    "studies": "sdo:hasPart",
    "ontologySourceReferences": "sdo:mentions",
    "comments": "sdo:disambiguatingDescription",

    "publications?": "sdo:subjectOf?",
    "filename": "sdo:alternateName"
  }
}
    """