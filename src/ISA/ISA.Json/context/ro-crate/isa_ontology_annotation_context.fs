namespace ARCtrl.ISA.Json.ROCrateContext

module OntologyAnnotation =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "OntologyAnnotation": "sdo:DefinedTerm",
    
    "annotationValue": "sdo:name",
    "termSource": "sdo:inDefinedTermSet",
    "termAccession": "sdo:termCode",
    "comments": "sdo:disambiguatingDescription"
  }
}
    """