namespace ARCtrl.ISA.Json.ROCrateContext

module Publication =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Publication": "sdo:ScholarlyArticle",
    
    "pubMedID": "sdo:url",
    "doi": "sdo:sameAs",
    "title": "sdo:headline",
    "status": "sdo:creativeWorkStatus",
    "authorList": "sdo:author",
    "comments": "sdo:disambiguatingDescription"
  }
}
    """