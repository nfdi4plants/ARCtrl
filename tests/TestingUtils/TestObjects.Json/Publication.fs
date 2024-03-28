module TestObjects.Json.Publication

let publication =

    """
    {
  "doi": "doi:10.1186/jbiol54",
  "pubMedID": "17439666",
  "status": {
    "annotationValue": "indexed in Pubmed"
  },
  "title": "Growth control of the eukaryote cell: a systems biology study in yeast.",
  "authorList": "Castrillo JI, Zeef LA, Hoyle DC, Zhang N, Hayes A, Gardner DC, Cornell MJ, Petty J, Hakes L, Wardleworth L, Rash B, Brown M, Dunn WB, Broadhurst D, O'Donoghue K, Hester SS, Dunkley TP, Hart SR, Swainston N, Li P, Gaskell SJ, Paton NW, Lilley KS, Kell DB, Oliver SG."
}
    """

let publicationLD =

    """
    {
  "@id": "doi:10.1186/jbiol54",
  "@type": "Publication",
  "@context": {
    "sdo": "http://schema.org/",

    "Publication": "sdo:ScholarlyArticle",
    
    "pubMedID": "sdo:url",
    "doi": "sdo:sameAs",
    "title": "sdo:headline",
    "status": "sdo:creativeWorkStatus",
    "authorList": "sdo:author",
    "comments": "sdo:disambiguatingDescription"
  },
  "doi": "doi:10.1186/jbiol54",
  "pubMedID": "17439666",
  "status": {
    "@id": "#UserTerm_indexed_in_Pubmed",
    "@type": "OntologyAnnotation",
    "@context": {
      "sdo": "http://schema.org/",

      "OntologyAnnotation": "sdo:DefinedTerm",
      
      "annotationValue": "sdo:name",
      "termSource": "sdo:inDefinedTermSet",
      "termAccession": "sdo:termCode",
      "comments": "sdo:disambiguatingDescription"
    },
    "annotationValue": "indexed in Pubmed"
  },
  "title": "Growth control of the eukaryote cell: a systems biology study in yeast.",
  "authorList": "Castrillo JI, Zeef LA, Hoyle DC, Zhang N, Hayes A, Gardner DC, Cornell MJ, Petty J, Hakes L, Wardleworth L, Rash B, Brown M, Dunn WB, Broadhurst D, O'Donoghue K, Hester SS, Dunkley TP, Hart SR, Swainston N, Li P, Gaskell SJ, Paton NW, Lilley KS, Kell DB, Oliver SG."
}
    """

