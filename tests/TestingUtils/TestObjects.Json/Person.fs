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
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Person": "sdo:Person",
    
    "firstName": "sdo:givenName",
    "lastName": "sdo:familyName",
    "midInitials": "sdo:additionalName",
    "email": "sdo:email",
    "address": "sdo:address",
    "phone": "sdo:telephone",
    "fax": "sdo:faxNumber",
    "comments": "sdo:disambiguatingDescription",
    "roles": "sdo:jobTitle",
    "affiliation": "sdo:affiliation"
  },
  "fax": "",
  "email": "lol@lal.das",
  "comments": [
    {
      "@id": "#Comment_Investigation_Person_REF_",
      "@type": "Comment",
      "@context": {
        "sdo": "http://schema.org/",
        "arc": "http://purl.org/nfdi4plants/ontology/",
        
        "Comment": "sdo:Comment",
        "name": "sdo:name",
        "value": "sdo:value"
      },
      "value": "",
      "name": "Investigation Person REF"
    }
  ],
  "roles": [
    {
      "@id": "#UserTerm_author",
      "@type": "OntologyAnnotation",
      "@context": {
        "sdo": "http://schema.org/",
        "arc": "http://purl.org/nfdi4plants/ontology/",

        "OntologyAnnotation": "sdo:DefinedTerm",
        
        "annotationValue": "sdo:name",
        "termSource": "sdo:inDefinedTermSet",
        "termAccession": "sdo:termCode",
        "comments": "sdo:disambiguatingDescription"
      },
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
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",

    "Person": "sdo:Person",
    
    "firstName": "sdo:givenName",
    "lastName": "sdo:familyName",
    "midInitials": "sdo:additionalName",
    "email": "sdo:email",
    "address": "sdo:address",
    "phone": "sdo:telephone",
    "fax": "sdo:faxNumber",
    "comments": "sdo:disambiguatingDescription",
    "roles": "sdo:jobTitle",
    "affiliation": "sdo:affiliation"
  },
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
      "@context": {
        "sdo": "http://schema.org/",
        "arc": "http://purl.org/nfdi4plants/ontology/",
        
        "Comment": "sdo:Comment",
        "name": "sdo:name",
        "value": "sdo:value"
      },
      "value": "",
      "name": "Investigation Person REF"
    }
  ],
  "roles": [
    {
      "@id": "#UserTerm_author",
      "@type": "OntologyAnnotation",
      "@context": {
        "sdo": "http://schema.org/",
        "arc": "http://purl.org/nfdi4plants/ontology/",

        "OntologyAnnotation": "sdo:DefinedTerm",
        
        "annotationValue": "sdo:name",
        "termSource": "sdo:inDefinedTermSet",
        "termAccession": "sdo:termCode",
        "comments": "sdo:disambiguatingDescription"
      },
      "annotationValue": "author"
    }
  ],
  "affiliation": "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
}
    """

