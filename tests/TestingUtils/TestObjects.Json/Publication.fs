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
  "doi": "doi:10.1186/jbiol54",
  "pubMedID": "17439666",
  "status": {
    "@id": "#DummyOntologyAnnotation",
    "@type": "OntologyAnnotation",
    "annotationValue": "indexed in Pubmed"
  },
  "title": "Growth control of the eukaryote cell: a systems biology study in yeast.",
  "authorList": "Castrillo JI, Zeef LA, Hoyle DC, Zhang N, Hayes A, Gardner DC, Cornell MJ, Petty J, Hakes L, Wardleworth L, Rash B, Brown M, Dunn WB, Broadhurst D, O'Donoghue K, Hester SS, Dunkley TP, Hart SR, Swainston N, Li P, Gaskell SJ, Paton NW, Lilley KS, Kell DB, Oliver SG."
}
    """

