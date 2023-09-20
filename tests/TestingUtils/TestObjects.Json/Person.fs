module TestObjects.Json.Person

let person = 
    """
    {
  "phone": "",
  "firstName": "Juan",
  "address": "Oxford Road, Manchester M13 9PT, UK",
  "lastName": "Castrillo",
  "midInitials": "I",
  "@id": "#person/Castrillo",
  "fax": "",
  "email": "lol@lal.das",
  "comments": [
    {
      "value": "",
      "name": "Investigation Person REF"
    }
  ],
  "roles": [
    {
      "annotationValue": "author"
    }
  ],
  "affiliation": "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
}
    """

let personWithORCID = 
    """
    {
  "firstName": "Juan",
  "lastName": "Castrillo",
  "comments": [
    {
      "value": "0000-0002-1825-0097",
      "name": "ORCID"
    }
  ]
  }
    """

let personLD = 
    """
    {
  "phone": "",
  "firstName": "Juan",
  "address": "Oxford Road, Manchester M13 9PT, UK",
  "lastName": "Castrillo",
  "midInitials": "I",
  "@id": "#person/Castrillo",
  "@type": "Person",
  "fax": "",
  "email": "lol@lal.das",
  "comments": [
    {
      "@id": "#Comment_Investigation_Person_REF_",
      "@type": "Comment",
      "value": "",
      "name": "Investigation Person REF"
    }
  ],
  "roles": [
    {
      "@id": "#DummyOntologyAnnotation",
      "@type": "OntologyAnnotation",
      "annotationValue": "author"
    }
  ],
  "affiliation": "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
}
    """

let personWithoutID = 
    """
    {
  "phone": "",
  "firstName": "Juan",
  "address": "Oxford Road, Manchester M13 9PT, UK",
  "lastName": "Castrillo",
  "midInitials": "I",
  "fax": "",
  "email": "lol@lal.das",
  "comments": [
    {
      "value": "",
      "name": "Investigation Person REF"
    }
  ],
  "roles": [
    {
      "annotationValue": "author"
    }
  ],
  "affiliation": "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
}
    """

let personWithDefaultLD = 
    """
    {
  "@id": "lol@lal.das",
  "@type": "Person",
  "phone": "",
  "firstName": "Juan",
  "address": "Oxford Road, Manchester M13 9PT, UK",
  "lastName": "Castrillo",
  "midInitials": "I",
  "fax": "",
  "email": "lol@lal.das",
  "comments": [
    {
      "@id": "#Comment_Investigation_Person_REF_",
      "@type": "Comment",
      "value": "",
      "name": "Investigation Person REF"
    }
  ],
  "roles": [
    {
      "@id": "#DummyOntologyAnnotation",
      "@type": "OntologyAnnotation",
      "annotationValue": "author"
    }
  ],
  "affiliation": "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
}
    """

