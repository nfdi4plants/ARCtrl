/// All json objects tested against offical validator: https://validator.schema.org
module TestObjects.Json.ROCrate

let definedTerm = """{
  "@id": "http://purl.obolibrary.org/obo/NCIT_C16965",
  "@type": "OntologyAnnotation",
  "annotationValue": "Peptidase",
  "termSource": "MS",
  "termAccession": "http://purl.obolibrary.org/obo/NCIT_C16965",
  "comments": [
    "Comment {Name = \"comment\", Value= \"This is a comment\"}"
  ],
  "@context": {
    "sdo": "http://schema.org/",
    "OntologyAnnotation": "sdo:DefinedTerm",
    "annotationValue": "sdo:name",
    "termSource": "sdo:inDefinedTermSet",
    "termAccession": "sdo:termCode",
    "comments": "sdo:disambiguatingDescription"
  }
}"""

let propertyValue = """{
  "@id": "http://purl.obolibrary.org/obo/NCIT_C16965",
  "@type": "PropertyValue",
  "category": "Peptidase",
  "categoryCode": "http://purl.obolibrary.org/obo/NCIT_C16965",
  "comments": [
    "Comment {Name = \"comment\", Value= \"This is a comment\"}"
  ],
  "@context": {
    "sdo": "http://schema.org/",
    "additionalType": "sdo:additionalType",
    "alternateName": "sdo:alternateName",
    "measurementMethod": "sdo:measurementMethod",
    "description": "sdo:description",
    "category": "sdo:name",
    "categoryCode": "sdo:propertyID",
    "value": "sdo:value",
    "valueCode": "sdo:valueReference",
    "unit": "sdo:unitText",
    "unitCode": "sdo:unitCode",
    "comments": "sdo:disambiguatingDescription"
  }
}"""

let comment = """{
  "@id": "#Comment_My,_cool__comment_wiht_=_lots;_of_special_<>_chars_STARTING_VALUE",
  "@type": "Comment",
  "name": "My, cool  comment wiht = lots; of special <> chars",
  "value": "STARTING VALUE",
  "@context": {
    "sdo": "http://schema.org/",
    "Comment": "sdo:Comment",
    "name": "sdo:name",
    "value": "sdo:text"
  }
}"""

let person = """{
  "@id": "myfantasymail@wow.de",
  "@type": "Person",
  "orcid": "0000-0002-8510-6810",
  "firstName": "Kevin",
  "lastName": "Frey",
  "email": "myfantasymail@wow.de",
  "phone": "09812-39128",
  "address": "Musterstraße 42, 12345 Beispielstadt, Deutschland",
  "affiliation": {
    "@type": "Organization",
    "@id": "#Organization_RPTU",
    "name": "RPTU",
    "@context": {
      "sdo": "http://schema.org/",
      "Organization": "sdo:Organization",
      "name": "sdo:name"
    }
  },
  "roles": [
    {
      "@id": "#UserTerm_researcher",
      "@type": "OntologyAnnotation",
      "annotationValue": "researcher",
      "@context": {
        "sdo": "http://schema.org/",
        "OntologyAnnotation": "sdo:DefinedTerm",
        "annotationValue": "sdo:name",
        "termSource": "sdo:inDefinedTermSet",
        "termAccession": "sdo:termCode",
        "comments": "sdo:disambiguatingDescription"
      }
    },
    {
      "@id": "dev:00000001",
      "@type": "OntologyAnnotation",
      "annotationValue": "developer",
      "termSource": "dev",
      "termAccession": "dev:00000001",
      "@context": {
        "sdo": "http://schema.org/",
        "OntologyAnnotation": "sdo:DefinedTerm",
        "annotationValue": "sdo:name",
        "termSource": "sdo:inDefinedTermSet",
        "termAccession": "sdo:termCode",
        "comments": "sdo:disambiguatingDescription"
      }
    }
  ],
  "comments": [
    "Comment {Name = \"Wow\", Value= \"VeryWow\"}"
  ],
  "@context": {
    "sdo": "http://schema.org/",
    "Person": "sdo:Person",
    "orcid": "sdo:identifier",
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
  }
}"""

let publication = """{
  "@id": "10.3390/ijms24087444",
  "@type": "Publication",
  "pubMedID": "37108605",
  "doi": "10.3390/ijms24087444",
  "authorList": [
    {
      "@type": "Person",
      "name": "Felix Jung",
      "@context": {
        "sdo": "http://schema.org/",
        "Person": "sdo:Person",
        "name": "sdo:name"
      }
    },
    {
      "@type": "Person",
      "name": "Kevin Frey",
      "@context": {
        "sdo": "http://schema.org/",
        "Person": "sdo:Person",
        "name": "sdo:name"
      }
    },
    {
      "@type": "Person",
      "name": "David Zimmer",
      "@context": {
        "sdo": "http://schema.org/",
        "Person": "sdo:Person",
        "name": "sdo:name"
      }
    },
    {
      "@type": "Person",
      "name": "Timo Mühlhaus",
      "@context": {
        "sdo": "http://schema.org/",
        "Person": "sdo:Person",
        "name": "sdo:name"
      }
    }
  ],
  "title": "DeepSTABp: A Deep Learning Approach for the Prediction of Thermal Protein Stability",
  "status": {
    "@id": "EFO:0001796",
    "@type": "OntologyAnnotation",
    "annotationValue": "published",
    "termSource": "EFO",
    "termAccession": "EFO:0001796",
    "@context": {
      "sdo": "http://schema.org/",
      "OntologyAnnotation": "sdo:DefinedTerm",
      "annotationValue": "sdo:name",
      "termSource": "sdo:inDefinedTermSet",
      "termAccession": "sdo:termCode",
      "comments": "sdo:disambiguatingDescription"
    }
  },
  "comments": [
    "Comment {Name = \"ByeBye\", Value= \"World\"}",
    "Comment {Name = \"Hello\", Value= \"Space\"}"
  ],
  "@context": {
    "sdo": "http://schema.org/",
    "Publication": "sdo:ScholarlyArticle",
    "pubMedID": "sdo:url",
    "doi": "sdo:sameAs",
    "title": "sdo:headline",
    "status": "sdo:creativeWorkStatus",
    "authorList": "sdo:author",
    "comments": "sdo:disambiguatingDescription"
  }
}"""