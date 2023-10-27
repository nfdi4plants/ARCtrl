namespace ARCtrl.ISA.Json.ROCrateContext

module OntologySourceReference =

  let context =
    """
{
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "OntologySourceReference": "sdo:DefinedTermSet",
    
    "type": "sdo:",
    "title": "sdo:headline",
    "description": "sdo:description",
    "name": "sdo:name",
    "file": "sdo:url",
    "version": "sdo:version",
    "comments": "sdo:disambiguatingDescription"
  }
}
    """