module TestObjects.ROCrate.ArcPrototypeDeprecated

/// https://git.nfdi4plants.org/muehlhaus/ArcPrototype/-/tree/ed12349933062b4440ed2d1e0dc05482853d752d
let ed123499 = """{
  "@type": "CreativeWork",
  "@id": "ro-crate-metadata.json",
  "about": {
    "@id": "./",
    "@type": "Investigation",
    "additionalType": "Investigation",
    "identifier": "ArcPrototype",
    "filename": "isa.investigation.xlsx",
    "title": "ArcPrototype",
    "description": "A prototypic ARC that implements all specification standards accordingly",
    "people": [
      {
        "@id": "timo.muehlhaus@rptu.de",
        "@type": "Person",
        "orcid": "http://orcid.org/0000-0003-3925-6778",
        "firstName": "Timo",
        "lastName": "Mühlhaus",
        "email": "timo.muehlhaus@rptu.de",
        "phone": "0 49 (0)631 205 4657",
        "address": "RPTU University of Kaiserslautern, Paul-Ehrlich-Str. 23 , 67663 Kaiserslautern",
        "affiliation": {
          "@type": "Organization",
          "@id": "#Organization_RPTU_University_of_Kaiserslautern",
          "name": "RPTU University of Kaiserslautern",
          "@context": {
            "sdo": "http://schema.org/",
            "Organization": "sdo:Organization",
            "name": "sdo:name"
          }
        },
        "roles": [
          {
            "@id": "http://purl.org/spar/scoro/principal-investigator",
            "@type": "OntologyAnnotation",
            "annotationValue": "principal investigator",
            "termSource": "scoro",
            "termAccession": "http://purl.org/spar/scoro/principal-investigator",
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
      },
      {
        "@id": "garth@rptu.de",
        "@type": "Person",
        "firstName": "Christoph",
        "lastName": "Garth",
        "email": "garth@rptu.de",
        "affiliation": {
          "@type": "Organization",
          "@id": "#Organization_RPTU_University_of_Kaiserslautern",
          "name": "RPTU University of Kaiserslautern",
          "@context": {
            "sdo": "http://schema.org/",
            "Organization": "sdo:Organization",
            "name": "sdo:name"
          }
        },
        "roles": [
          {
            "@id": "http://purl.org/spar/scoro/principal-investigator",
            "@type": "OntologyAnnotation",
            "annotationValue": "principal investigator",
            "termSource": "scoro",
            "termAccession": "http://purl.org/spar/scoro/principal-investigator",
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
      },
      {
        "@id": "maus@nfdi4plants.org",
        "@type": "Person",
        "orcid": "0000-0002-8241-5300",
        "firstName": "Oliver",
        "lastName": "Maus",
        "email": "maus@nfdi4plants.org",
        "address": "RPTU University of Kaiserslautern, Erwin-Schrödinger-Str. 56 , 67663 Kaiserslautern",
        "affiliation": {
          "@type": "Organization",
          "@id": "#Organization_RPTU_University_of_Kaiserslautern",
          "name": "RPTU University of Kaiserslautern",
          "@context": {
            "sdo": "http://schema.org/",
            "Organization": "sdo:Organization",
            "name": "sdo:name"
          }
        },
        "roles": [
          {
            "@id": "http://purl.org/spar/scoro/research-assistant",
            "@type": "OntologyAnnotation",
            "annotationValue": "research assistant",
            "termSource": "scoro",
            "termAccession": "http://purl.org/spar/scoro/research-assistant",
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
      }
    ],
    "studies": [
      {
        "@id": "#study/MaterialPreparation",
        "@type": [
          "Study"
        ],
        "additionalType": "Study",
        "identifier": "MaterialPreparation",
        "filename": "studies/MaterialPreparation/isa.study.xlsx",
        "title": "Prototype for experimental data",
        "description": "In this a devised study to have an exemplary experimental material description.",
        "processSequence": [
          {
            "@id": "#Process_CellCultivation",
            "@type": [
              "Process"
            ],
            "name": "CellCultivation",
            "executesProtocol": {
              "@id": "#Protocol_MaterialPreparation_CellCultivation",
              "@type": [
                "Protocol"
              ],
              "protocolType": {
                "@id": "https://bioregistry.io/EFO:0003789",
                "@type": "OntologyAnnotation",
                "annotationValue": "growth protocol",
                "termSource": "EFO",
                "termAccession": "https://bioregistry.io/EFO:0003789",
                "@context": {
                  "sdo": "http://schema.org/",
                  "OntologyAnnotation": "sdo:DefinedTerm",
                  "annotationValue": "sdo:name",
                  "termSource": "sdo:inDefinedTermSet",
                  "termAccession": "sdo:termCode",
                  "comments": "sdo:disambiguatingDescription"
                }
              },
              "@context": {
                "sdo": "http://schema.org/",
                "bio": "https://bioschemas.org/",
                "Protocol": "bio:LabProtocol",
                "name": "sdo:name",
                "protocolType": "bio:intendedUse",
                "description": "sdo:description",
                "version": "sdo:version",
                "components": "bio:labEquipment",
                "reagents": "bio:reagent",
                "computationalTools": "bio:computationalTool",
                "uri": "sdo:url",
                "comments": "sdo:comment"
              }
            },
            "inputs": [
              {
                "@id": "#Source_Source1",
                "@type": [
                  "Source"
                ],
                "additionalType": "Source",
                "name": "Source1",
                "characteristics": [
                  {
                    "@id": "#MaterialAttributeValue/organism=Arabidopsis thaliana",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "organism",
                    "categoryCode": "OBI:0100026",
                    "value": "Arabidopsis thaliana",
                    "valueCode": "http://purl.obolibrary.org/obo/NCBITaxon_3702",
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
                  },
                  {
                    "@id": "#MaterialAttributeValue/biological replicate=1",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "biological replicate",
                    "categoryCode": "DPBO:0000042",
                    "value": "1",
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
                  }
                ],
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Source": "bio:Sample",
                  "name": "sdo:name",
                  "characteristics": "bio:additionalProperty"
                }
              },
              {
                "@id": "#Source_Source2",
                "@type": [
                  "Source"
                ],
                "additionalType": "Source",
                "name": "Source2",
                "characteristics": [
                  {
                    "@id": "#MaterialAttributeValue/organism=Arabidopsis thaliana",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "organism",
                    "categoryCode": "OBI:0100026",
                    "value": "Arabidopsis thaliana",
                    "valueCode": "http://purl.obolibrary.org/obo/NCBITaxon_3702",
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
                  },
                  {
                    "@id": "#MaterialAttributeValue/biological replicate=1",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "biological replicate",
                    "categoryCode": "DPBO:0000042",
                    "value": "1",
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
                  }
                ],
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Source": "bio:Sample",
                  "name": "sdo:name",
                  "characteristics": "bio:additionalProperty"
                }
              },
              {
                "@id": "#Source_Source3",
                "@type": [
                  "Source"
                ],
                "additionalType": "Source",
                "name": "Source3",
                "characteristics": [
                  {
                    "@id": "#MaterialAttributeValue/organism=Arabidopsis thaliana",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "organism",
                    "categoryCode": "OBI:0100026",
                    "value": "Arabidopsis thaliana",
                    "valueCode": "http://purl.obolibrary.org/obo/NCBITaxon_3702",
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
                  },
                  {
                    "@id": "#MaterialAttributeValue/biological replicate=1",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "biological replicate",
                    "categoryCode": "DPBO:0000042",
                    "value": "1",
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
                  }
                ],
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Source": "bio:Sample",
                  "name": "sdo:name",
                  "characteristics": "bio:additionalProperty"
                }
              },
              {
                "@id": "#Source_Source4",
                "@type": [
                  "Source"
                ],
                "additionalType": "Source",
                "name": "Source4",
                "characteristics": [
                  {
                    "@id": "#MaterialAttributeValue/organism=Arabidopsis thaliana",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "organism",
                    "categoryCode": "OBI:0100026",
                    "value": "Arabidopsis thaliana",
                    "valueCode": "http://purl.obolibrary.org/obo/NCBITaxon_3702",
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
                  },
                  {
                    "@id": "#MaterialAttributeValue/biological replicate=2",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "biological replicate",
                    "categoryCode": "DPBO:0000042",
                    "value": "2",
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
                  }
                ],
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Source": "bio:Sample",
                  "name": "sdo:name",
                  "characteristics": "bio:additionalProperty"
                }
              },
              {
                "@id": "#Source_Source5",
                "@type": [
                  "Source"
                ],
                "additionalType": "Source",
                "name": "Source5",
                "characteristics": [
                  {
                    "@id": "#MaterialAttributeValue/organism=Arabidopsis thaliana",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "organism",
                    "categoryCode": "OBI:0100026",
                    "value": "Arabidopsis thaliana",
                    "valueCode": "http://purl.obolibrary.org/obo/NCBITaxon_3702",
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
                  },
                  {
                    "@id": "#MaterialAttributeValue/biological replicate=2",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "biological replicate",
                    "categoryCode": "DPBO:0000042",
                    "value": "2",
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
                  }
                ],
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Source": "bio:Sample",
                  "name": "sdo:name",
                  "characteristics": "bio:additionalProperty"
                }
              },
              {
                "@id": "#Source_Source6",
                "@type": [
                  "Source"
                ],
                "additionalType": "Source",
                "name": "Source6",
                "characteristics": [
                  {
                    "@id": "#MaterialAttributeValue/organism=Arabidopsis thaliana",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "organism",
                    "categoryCode": "OBI:0100026",
                    "value": "Arabidopsis thaliana",
                    "valueCode": "http://purl.obolibrary.org/obo/NCBITaxon_3702",
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
                  },
                  {
                    "@id": "#MaterialAttributeValue/biological replicate=2",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "biological replicate",
                    "categoryCode": "DPBO:0000042",
                    "value": "2",
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
                  }
                ],
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Source": "bio:Sample",
                  "name": "sdo:name",
                  "characteristics": "bio:additionalProperty"
                }
              }
            ],
            "outputs": [
              {
                "@id": "#Sample_Cultivation_Flask1",
                "@type": [
                  "Sample"
                ],
                "additionalType": "Sample",
                "name": "Cultivation Flask1",
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Sample": "bio:Sample",
                  "name": "sdo:name",
                  "additionalProperties": "bio:additionalProperty"
                }
              },
              {
                "@id": "#Sample_Cultivation_Flask2",
                "@type": [
                  "Sample"
                ],
                "additionalType": "Sample",
                "name": "Cultivation Flask2",
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Sample": "bio:Sample",
                  "name": "sdo:name",
                  "additionalProperties": "bio:additionalProperty"
                }
              },
              {
                "@id": "#Sample_Cultivation_Flask3",
                "@type": [
                  "Sample"
                ],
                "additionalType": "Sample",
                "name": "Cultivation Flask3",
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Sample": "bio:Sample",
                  "name": "sdo:name",
                  "additionalProperties": "bio:additionalProperty"
                }
              },
              {
                "@id": "#Sample_Cultivation_Flask4",
                "@type": [
                  "Sample"
                ],
                "additionalType": "Sample",
                "name": "Cultivation Flask4",
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Sample": "bio:Sample",
                  "name": "sdo:name",
                  "additionalProperties": "bio:additionalProperty"
                }
              },
              {
                "@id": "#Sample_Cultivation_Flask5",
                "@type": [
                  "Sample"
                ],
                "additionalType": "Sample",
                "name": "Cultivation Flask5",
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Sample": "bio:Sample",
                  "name": "sdo:name",
                  "additionalProperties": "bio:additionalProperty"
                }
              },
              {
                "@id": "#Sample_Cultivation_Flask6",
                "@type": [
                  "Sample"
                ],
                "additionalType": "Sample",
                "name": "Cultivation Flask6",
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Sample": "bio:Sample",
                  "name": "sdo:name",
                  "additionalProperties": "bio:additionalProperty"
                }
              }
            ],
            "@context": {
              "sdo": "http://schema.org/",
              "bio": "https://bioschemas.org/",
              "Process": "bio:LabProcess",
              "name": "sdo:name",
              "executesProtocol": "bio:executesLabProtocol",
              "parameterValues": "bio:parameterValue",
              "performer": "sdo:agent",
              "date": "sdo:endTime",
              "inputs": "sdo:object",
              "outputs": "sdo:result",
              "comments": "sdo:disambiguatingDescription"
            }
          },
          {
            "@id": "#Process_AccessoryDataRetrieval",
            "@type": [
              "Process"
            ],
            "name": "AccessoryDataRetrieval",
            "inputs": [
              {
                "@id": "#Sample_Sample1",
                "@type": [
                  "Sample"
                ],
                "additionalType": "Sample",
                "name": "Sample1",
                "additionalProperties": [
                  {
                    "@id": "#MaterialAttributeValue/Performed Procedure Step SOP Instance UID=e36ca6b8-19ba-4504-aa82-d4781765873d",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "Performed Procedure Step SOP Instance UID",
                    "categoryCode": "NCIT:C69261",
                    "value": "e36ca6b8-19ba-4504-aa82-d4781765873d",
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
                  }
                ],
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Sample": "bio:Sample",
                  "name": "sdo:name",
                  "additionalProperties": "bio:additionalProperty"
                }
              },
              {
                "@id": "#Sample_Sample2",
                "@type": [
                  "Sample"
                ],
                "additionalType": "Sample",
                "name": "Sample2",
                "additionalProperties": [
                  {
                    "@id": "#MaterialAttributeValue/Performed Procedure Step SOP Instance UID=714ca2b7-22b7-4f69-b83d-9165f624da25",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "Performed Procedure Step SOP Instance UID",
                    "categoryCode": "NCIT:C69261",
                    "value": "714ca2b7-22b7-4f69-b83d-9165f624da25",
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
                  }
                ],
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Sample": "bio:Sample",
                  "name": "sdo:name",
                  "additionalProperties": "bio:additionalProperty"
                }
              },
              {
                "@id": "#Sample_Sample3",
                "@type": [
                  "Sample"
                ],
                "additionalType": "Sample",
                "name": "Sample3",
                "additionalProperties": [
                  {
                    "@id": "#MaterialAttributeValue/Performed Procedure Step SOP Instance UID=66fac760-acc7-4ed4-ba21-2cb67fa36e4d",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "Performed Procedure Step SOP Instance UID",
                    "categoryCode": "NCIT:C69261",
                    "value": "66fac760-acc7-4ed4-ba21-2cb67fa36e4d",
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
                  }
                ],
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Sample": "bio:Sample",
                  "name": "sdo:name",
                  "additionalProperties": "bio:additionalProperty"
                }
              },
              {
                "@id": "#Sample_Sample4",
                "@type": [
                  "Sample"
                ],
                "additionalType": "Sample",
                "name": "Sample4",
                "additionalProperties": [
                  {
                    "@id": "#MaterialAttributeValue/Performed Procedure Step SOP Instance UID=cba5f40c-fc05-44d6-a589-b0e3dafaeefe",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "Performed Procedure Step SOP Instance UID",
                    "categoryCode": "NCIT:C69261",
                    "value": "cba5f40c-fc05-44d6-a589-b0e3dafaeefe",
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
                  }
                ],
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Sample": "bio:Sample",
                  "name": "sdo:name",
                  "additionalProperties": "bio:additionalProperty"
                }
              },
              {
                "@id": "#Sample_Sample5",
                "@type": [
                  "Sample"
                ],
                "additionalType": "Sample",
                "name": "Sample5",
                "additionalProperties": [
                  {
                    "@id": "#MaterialAttributeValue/Performed Procedure Step SOP Instance UID=84c37b60-2342-4226-a36c-4b8dfe84ebe9",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "Performed Procedure Step SOP Instance UID",
                    "categoryCode": "NCIT:C69261",
                    "value": "84c37b60-2342-4226-a36c-4b8dfe84ebe9",
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
                  }
                ],
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Sample": "bio:Sample",
                  "name": "sdo:name",
                  "additionalProperties": "bio:additionalProperty"
                }
              },
              {
                "@id": "#Sample_Sample6",
                "@type": [
                  "Sample"
                ],
                "additionalType": "Sample",
                "name": "Sample6",
                "additionalProperties": [
                  {
                    "@id": "#MaterialAttributeValue/Performed Procedure Step SOP Instance UID=208df064-4b1c-4da0-a1f8-6412e1fb2284",
                    "@type": "PropertyValue",
                    "additionalType": "MaterialAttributeValue",
                    "category": "Performed Procedure Step SOP Instance UID",
                    "categoryCode": "NCIT:C69261",
                    "value": "208df064-4b1c-4da0-a1f8-6412e1fb2284",
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
                  }
                ],
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Sample": "bio:Sample",
                  "name": "sdo:name",
                  "additionalProperties": "bio:additionalProperty"
                }
              }
            ],
            "outputs": [
              {
                "@id": "Sample1_e36ca6b8-19ba-4504-aa82-d4781765873d.png",
                "@type": [
                  "Data"
                ],
                "name": "Sample1_e36ca6b8-19ba-4504-aa82-d4781765873d.png",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "Sample2_714ca2b7-22b7-4f69-b83d-9165f624da25.png",
                "@type": [
                  "Data"
                ],
                "name": "Sample2_714ca2b7-22b7-4f69-b83d-9165f624da25.png",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "Sample3_66fac760-acc7-4ed4-ba21-2cb67fa36e4d.png",
                "@type": [
                  "Data"
                ],
                "name": "Sample3_66fac760-acc7-4ed4-ba21-2cb67fa36e4d.png",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "Sample4_cba5f40c-fc05-44d6-a589-b0e3dafaeefe.png",
                "@type": [
                  "Data"
                ],
                "name": "Sample4_cba5f40c-fc05-44d6-a589-b0e3dafaeefe.png",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "Sample5_84c37b60-2342-4226-a36c-4b8dfe84ebe9.png",
                "@type": [
                  "Data"
                ],
                "name": "Sample5_84c37b60-2342-4226-a36c-4b8dfe84ebe9.png",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "Sample6_208df064-4b1c-4da0-a1f8-6412e1fb2284.png",
                "@type": [
                  "Data"
                ],
                "name": "Sample6_208df064-4b1c-4da0-a1f8-6412e1fb2284.png",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              }
            ],
            "@context": {
              "sdo": "http://schema.org/",
              "bio": "https://bioschemas.org/",
              "Process": "bio:LabProcess",
              "name": "sdo:name",
              "executesProtocol": "bio:executesLabProtocol",
              "parameterValues": "bio:parameterValue",
              "performer": "sdo:agent",
              "date": "sdo:endTime",
              "inputs": "sdo:object",
              "outputs": "sdo:result",
              "comments": "sdo:disambiguatingDescription"
            }
          }
        ],
        "assays": [
          {
            "@id": "#assay/measurement1",
            "@type": [
              "Assay"
            ],
            "additionalType": "Assay",
            "identifier": "measurement1",
            "filename": "assays/measurement1/isa.assay.xlsx",
            "performers": [
              {
                "@id": "mailto:maus@nfdi4plants.org",
                "@type": "Person",
                "firstName": "Oliver",
                "lastName": "Maus",
                "email": "mailto:maus@nfdi4plants.org",
                "affiliation": {
                  "@type": "Organization",
                  "@id": "#Organization_RPTU_University_of_Kaiserslautern",
                  "name": "RPTU University of Kaiserslautern",
                  "@context": {
                    "sdo": "http://schema.org/",
                    "Organization": "sdo:Organization",
                    "name": "sdo:name"
                  }
                },
                "roles": [
                  {
                    "@id": "http://purl.org/spar/scoro/research-assistant",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "research assistant",
                    "termSource": "scoro",
                    "termAccession": "http://purl.org/spar/scoro/research-assistant",
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
                  "Comment {Name = \"Worksheet\"}"
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
              }
            ],
            "dataFiles": [
              {
                "@id": "sample1.raw",
                "@type": [
                  "Data"
                ],
                "name": "sample1.raw",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "sample2.raw",
                "@type": [
                  "Data"
                ],
                "name": "sample2.raw",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "sample3.raw",
                "@type": [
                  "Data"
                ],
                "name": "sample3.raw",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "sample4.raw",
                "@type": [
                  "Data"
                ],
                "name": "sample4.raw",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "sample5.raw",
                "@type": [
                  "Data"
                ],
                "name": "sample5.raw",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "sample6.raw",
                "@type": [
                  "Data"
                ],
                "name": "sample6.raw",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "sample1.raw#",
                "@type": [
                  "Data"
                ],
                "name": "sample1.raw#",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "proteomics_result.csv#col=1",
                "@type": [
                  "Data"
                ],
                "name": "proteomics_result.csv#col=1",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "proteomics_result.csv#col=2",
                "@type": [
                  "Data"
                ],
                "name": "proteomics_result.csv#col=2",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "proteomics_result.csv#col=3",
                "@type": [
                  "Data"
                ],
                "name": "proteomics_result.csv#col=3",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "proteomics_result.csv#col=4",
                "@type": [
                  "Data"
                ],
                "name": "proteomics_result.csv#col=4",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "proteomics_result.csv#col=5",
                "@type": [
                  "Data"
                ],
                "name": "proteomics_result.csv#col=5",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "proteomics_result.csv#col=6",
                "@type": [
                  "Data"
                ],
                "name": "proteomics_result.csv#col=6",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "proteomics_result.csv#col=7",
                "@type": [
                  "Data"
                ],
                "name": "proteomics_result.csv#col=7",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "proteomics_result.csv#col=8",
                "@type": [
                  "Data"
                ],
                "name": "proteomics_result.csv#col=8",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "proteomics_result.csv#col=9",
                "@type": [
                  "Data"
                ],
                "name": "proteomics_result.csv#col=9",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "proteomics_result.csv#col=10",
                "@type": [
                  "Data"
                ],
                "name": "proteomics_result.csv#col=10",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "proteomics_result.csv#col=11",
                "@type": [
                  "Data"
                ],
                "name": "proteomics_result.csv#col=11",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              },
              {
                "@id": "proteomics_result.csv#col=12",
                "@type": [
                  "Data"
                ],
                "name": "proteomics_result.csv#col=12",
                "type": "Raw Data File",
                "@context": {
                  "sdo": "http://schema.org/",
                  "Data": "sdo:MediaObject",
                  "type": "sdo:disambiguatingDescription",
                  "encodingFormat": "sdo:encodingFormat",
                  "usageInfo": "sdo:usageInfo",
                  "name": "sdo:name",
                  "comments": "sdo:comment"
                }
              }
            ],
            "processSequence": [
              {
                "@id": "#Process_Cell_Lysis",
                "@type": [
                  "Process"
                ],
                "name": "Cell Lysis",
                "executesProtocol": {
                  "@id": "#Protocol_MaterialPreparation_measurement1_Cell_Lysis",
                  "@type": [
                    "Protocol"
                  ],
                  "components": [
                    {
                      "@id": "#Component/sonicator=Fisherbrand™ Model 705 Sonic Dismembrator",
                      "@type": "PropertyValue",
                      "additionalType": "Component",
                      "alternateName": "Fisherbrand™ Model 705 Sonic Dismembrator ()",
                      "category": "sonicator",
                      "categoryCode": "OBI:0400114",
                      "value": "Fisherbrand™ Model 705 Sonic Dismembrator",
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
                    },
                    {
                      "@id": "#Component/centrifuge=Eppendorf™ Centrifuge 5420",
                      "@type": "PropertyValue",
                      "additionalType": "Component",
                      "alternateName": "Eppendorf™ Centrifuge 5420 ()",
                      "category": "centrifuge",
                      "categoryCode": "OBI:0400106",
                      "value": "Eppendorf™ Centrifuge 5420",
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
                    }
                  ],
                  "@context": {
                    "sdo": "http://schema.org/",
                    "bio": "https://bioschemas.org/",
                    "Protocol": "bio:LabProtocol",
                    "name": "sdo:name",
                    "protocolType": "bio:intendedUse",
                    "description": "sdo:description",
                    "version": "sdo:version",
                    "components": "bio:labEquipment",
                    "reagents": "bio:reagent",
                    "computationalTools": "bio:computationalTool",
                    "uri": "sdo:url",
                    "comments": "sdo:comment"
                  }
                },
                "parameterValues": [
                  {
                    "@id": "#ProcessParameterValue/cell lysis=Sonication",
                    "@type": "PropertyValue",
                    "additionalType": "ProcessParameterValue",
                    "category": "cell lysis",
                    "categoryCode": "OBI:0302894",
                    "value": "Sonication",
                    "valueCode": "https://bioregistry.io/NCIT:C81871",
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
                  },
                  {
                    "@id": "#ProcessParameterValue/centrifugation=10g unit",
                    "@type": "PropertyValue",
                    "additionalType": "ProcessParameterValue",
                    "category": "centrifugation",
                    "categoryCode": "OBI:0302886",
                    "value": 10,
                    "unit": "g unit",
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
                  }
                ],
                "inputs": [
                  {
                    "@id": "#Sample_Cultivation_Flask1",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "Cultivation Flask1",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_Cultivation_Flask2",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "Cultivation Flask2",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_Cultivation_Flask3",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "Cultivation Flask3",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_Cultivation_Flask4",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "Cultivation Flask4",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_Cultivation_Flask5",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "Cultivation Flask5",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_Cultivation_Flask6",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "Cultivation Flask6",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  }
                ],
                "outputs": [
                  {
                    "@id": "#Sample_sample_eppi_1",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi 1",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_2",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi 2",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_3",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi 3",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_4",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi 4",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_5",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi 5",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_6",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi 6",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  }
                ],
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Process": "bio:LabProcess",
                  "name": "sdo:name",
                  "executesProtocol": "bio:executesLabProtocol",
                  "parameterValues": "bio:parameterValue",
                  "performer": "sdo:agent",
                  "date": "sdo:endTime",
                  "inputs": "sdo:object",
                  "outputs": "sdo:result",
                  "comments": "sdo:disambiguatingDescription"
                }
              },
              {
                "@id": "#Process_Protein_Extraction",
                "@type": [
                  "Process"
                ],
                "name": "Protein Extraction",
                "executesProtocol": {
                  "@id": "#Protocol_extractionProtocol.txt",
                  "@type": [
                    "Protocol"
                  ],
                  "name": "extractionProtocol.txt",
                  "@context": {
                    "sdo": "http://schema.org/",
                    "bio": "https://bioschemas.org/",
                    "Protocol": "bio:LabProtocol",
                    "name": "sdo:name",
                    "protocolType": "bio:intendedUse",
                    "description": "sdo:description",
                    "version": "sdo:version",
                    "components": "bio:labEquipment",
                    "reagents": "bio:reagent",
                    "computationalTools": "bio:computationalTool",
                    "uri": "sdo:url",
                    "comments": "sdo:comment"
                  }
                },
                "inputs": [
                  {
                    "@id": "#Sample_sample_eppi_1",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi 1",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_2",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi 2",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_3",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi 3",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_4",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi 4",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_5",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi 5",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_6",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi 6",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  }
                ],
                "outputs": [
                  {
                    "@id": "#Sample_sample_eppi_extracted_1",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi extracted 1",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_extracted_2",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi extracted 2",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_extracted_3",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi extracted 3",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_extracted_4",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi extracted 4",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_extracted_5",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi extracted 5",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_extracted_6",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi extracted 6",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  }
                ],
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Process": "bio:LabProcess",
                  "name": "sdo:name",
                  "executesProtocol": "bio:executesLabProtocol",
                  "parameterValues": "bio:parameterValue",
                  "performer": "sdo:agent",
                  "date": "sdo:endTime",
                  "inputs": "sdo:object",
                  "outputs": "sdo:result",
                  "comments": "sdo:disambiguatingDescription"
                }
              },
              {
                "@id": "#Process_Protein_Measurement",
                "@type": [
                  "Process"
                ],
                "name": "Protein Measurement",
                "executesProtocol": {
                  "@id": "#Protocol_MaterialPreparation_measurement1_Protein_Measurement",
                  "@type": [
                    "Protocol"
                  ],
                  "components": [
                    {
                      "@id": "#Component/cleavage agent name=Trypsin",
                      "@type": "PropertyValue",
                      "additionalType": "Component",
                      "alternateName": "Trypsin (MS:1001251)",
                      "category": "cleavage agent name",
                      "categoryCode": "MS:1001045",
                      "value": "Trypsin",
                      "valueCode": "https://www.ebi.ac.uk/ols4/ontologies/ms/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252FMS_1001251",
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
                    },
                    {
                      "@id": "#Component/instrument model=TripleTOF 5600+",
                      "@type": "PropertyValue",
                      "additionalType": "Component",
                      "alternateName": "TripleTOF 5600+ (MS:1002584)",
                      "category": "instrument model",
                      "categoryCode": "MS:1000031",
                      "value": "TripleTOF 5600+",
                      "valueCode": "https://www.ebi.ac.uk/ols4/ontologies/ms/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252FMS_1002584",
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
                    }
                  ],
                  "@context": {
                    "sdo": "http://schema.org/",
                    "bio": "https://bioschemas.org/",
                    "Protocol": "bio:LabProtocol",
                    "name": "sdo:name",
                    "protocolType": "bio:intendedUse",
                    "description": "sdo:description",
                    "version": "sdo:version",
                    "components": "bio:labEquipment",
                    "reagents": "bio:reagent",
                    "computationalTools": "bio:computationalTool",
                    "uri": "sdo:url",
                    "comments": "sdo:comment"
                  }
                },
                "inputs": [
                  {
                    "@id": "#Sample_sample_eppi_extracted_1",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi extracted 1",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_extracted_2",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi extracted 2",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_extracted_3",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi extracted 3",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_extracted_4",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi extracted 4",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_extracted_5",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi extracted 5",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  },
                  {
                    "@id": "#Sample_sample_eppi_extracted_6",
                    "@type": [
                      "Sample"
                    ],
                    "additionalType": "Sample",
                    "name": "sample eppi extracted 6",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "bio": "https://bioschemas.org/",
                      "Sample": "bio:Sample",
                      "name": "sdo:name",
                      "additionalProperties": "bio:additionalProperty"
                    }
                  }
                ],
                "outputs": [
                  {
                    "@id": "sample1.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample1.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "sample2.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample2.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "sample3.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample3.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "sample4.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample4.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "sample5.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample5.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "sample6.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample6.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  }
                ],
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Process": "bio:LabProcess",
                  "name": "sdo:name",
                  "executesProtocol": "bio:executesLabProtocol",
                  "parameterValues": "bio:parameterValue",
                  "performer": "sdo:agent",
                  "date": "sdo:endTime",
                  "inputs": "sdo:object",
                  "outputs": "sdo:result",
                  "comments": "sdo:disambiguatingDescription"
                }
              },
              {
                "@id": "#Process_Computational_Proteome_Analysis",
                "@type": [
                  "Process"
                ],
                "name": "Computational Proteome Analysis",
                "parameterValues": [
                  {
                    "@id": "#ProcessParameterValue/software=ProteomIqon",
                    "@type": "PropertyValue",
                    "additionalType": "ProcessParameterValue",
                    "category": "software",
                    "categoryCode": "MS:1000531",
                    "value": "ProteomIqon",
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
                  }
                ],
                "inputs": [
                  {
                    "@id": "sample1.raw#",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample1.raw#",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "sample1.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample1.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "sample2.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample2.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "sample2.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample2.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "sample3.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample3.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "sample3.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample3.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "sample4.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample4.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "sample4.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample4.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "sample5.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample5.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "sample5.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample5.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "sample6.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample6.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "sample6.raw",
                    "@type": [
                      "Data"
                    ],
                    "name": "sample6.raw",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  }
                ],
                "outputs": [
                  {
                    "@id": "proteomics_result.csv#col=1",
                    "@type": [
                      "Data"
                    ],
                    "name": "proteomics_result.csv#col=1",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "proteomics_result.csv#col=2",
                    "@type": [
                      "Data"
                    ],
                    "name": "proteomics_result.csv#col=2",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "proteomics_result.csv#col=3",
                    "@type": [
                      "Data"
                    ],
                    "name": "proteomics_result.csv#col=3",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "proteomics_result.csv#col=4",
                    "@type": [
                      "Data"
                    ],
                    "name": "proteomics_result.csv#col=4",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "proteomics_result.csv#col=5",
                    "@type": [
                      "Data"
                    ],
                    "name": "proteomics_result.csv#col=5",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "proteomics_result.csv#col=6",
                    "@type": [
                      "Data"
                    ],
                    "name": "proteomics_result.csv#col=6",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "proteomics_result.csv#col=7",
                    "@type": [
                      "Data"
                    ],
                    "name": "proteomics_result.csv#col=7",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "proteomics_result.csv#col=8",
                    "@type": [
                      "Data"
                    ],
                    "name": "proteomics_result.csv#col=8",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "proteomics_result.csv#col=9",
                    "@type": [
                      "Data"
                    ],
                    "name": "proteomics_result.csv#col=9",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "proteomics_result.csv#col=10",
                    "@type": [
                      "Data"
                    ],
                    "name": "proteomics_result.csv#col=10",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "proteomics_result.csv#col=11",
                    "@type": [
                      "Data"
                    ],
                    "name": "proteomics_result.csv#col=11",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  },
                  {
                    "@id": "proteomics_result.csv#col=12",
                    "@type": [
                      "Data"
                    ],
                    "name": "proteomics_result.csv#col=12",
                    "type": "Raw Data File",
                    "@context": {
                      "sdo": "http://schema.org/",
                      "Data": "sdo:MediaObject",
                      "type": "sdo:disambiguatingDescription",
                      "encodingFormat": "sdo:encodingFormat",
                      "usageInfo": "sdo:usageInfo",
                      "name": "sdo:name",
                      "comments": "sdo:comment"
                    }
                  }
                ],
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Process": "bio:LabProcess",
                  "name": "sdo:name",
                  "executesProtocol": "bio:executesLabProtocol",
                  "parameterValues": "bio:parameterValue",
                  "performer": "sdo:agent",
                  "date": "sdo:endTime",
                  "inputs": "sdo:object",
                  "outputs": "sdo:result",
                  "comments": "sdo:disambiguatingDescription"
                }
              }
            ],
            "@context": {
              "sdo": "http://schema.org/",
              "Assay": "sdo:Dataset",
              "identifier": "sdo:identifier",
              "additionalType": "sdo:additionalType",
              "measurementType": "sdo:variableMeasured",
              "technologyType": "sdo:measurementTechnique",
              "technologyPlatform": "sdo:measurementMethod",
              "dataFiles": "sdo:hasPart",
              "performers": "sdo:creator",
              "processSequence": "sdo:about",
              "comments": "sdo:comment",
              "filename": "sdo:url"
            }
          },
          {
            "@id": "#assay/measurement2",
            "@type": [
              "Assay"
            ],
            "additionalType": "Assay",
            "identifier": "measurement2",
            "filename": "assays/measurement2/isa.assay.xlsx",
            "performers": [
              {
                "@id": "#EmptyPerson",
                "@type": "Person",
                "comments": [
                  "Comment {Name = \"Worksheet\"}"
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
              }
            ],
            "@context": {
              "sdo": "http://schema.org/",
              "Assay": "sdo:Dataset",
              "identifier": "sdo:identifier",
              "additionalType": "sdo:additionalType",
              "measurementType": "sdo:variableMeasured",
              "technologyType": "sdo:measurementTechnique",
              "technologyPlatform": "sdo:measurementMethod",
              "dataFiles": "sdo:hasPart",
              "performers": "sdo:creator",
              "processSequence": "sdo:about",
              "comments": "sdo:comment",
              "filename": "sdo:url"
            }
          }
        ],
        "@context": {
          "sdo": "http://schema.org/",
          "Study": "sdo:Dataset",
          "identifier": "sdo:identifier",
          "title": "sdo:headline",
          "additionalType": "sdo:additionalType",
          "description": "sdo:description",
          "submissionDate": "sdo:dateCreated",
          "publicReleaseDate": "sdo:datePublished",
          "publications": "sdo:citation",
          "people": "sdo:creator",
          "assays": "sdo:hasPart",
          "filename": "sdo:alternateName",
          "comments": "sdo:comment",
          "processSequence": "sdo:about",
          "studyDesignDescriptors": "arc:ARC#ARC_00000037"
        }
      },
      {
        "@id": "#study/experiment2",
        "@type": [
          "Study"
        ],
        "additionalType": "Study",
        "identifier": "experiment2",
        "filename": "studies/experiment2/isa.study.xlsx",
        "processSequence": [
          {
            "@id": "#Process_experiment2",
            "@type": [
              "Process"
            ],
            "name": "experiment2",
            "parameterValues": [
              {
                "@id": "#ProcessParameterValue/protein assay=SDS-PAGE",
                "@type": "PropertyValue",
                "additionalType": "ProcessParameterValue",
                "category": "protein assay",
                "categoryCode": "EFO:0001458",
                "value": "SDS-PAGE",
                "valueCode": "https://bioregistry.io/EFO:0010936",
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
              }
            ],
            "inputs": [
              {
                "@id": "#Source_Input_[MyStudyObject]",
                "@type": [
                  "Source"
                ],
                "additionalType": "Source",
                "name": "Input [MyStudyObject]",
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Source": "bio:Sample",
                  "name": "sdo:name",
                  "characteristics": "bio:additionalProperty"
                }
              }
            ],
            "outputs": [
              {
                "@id": "#Sample_MyGel",
                "@type": [
                  "Sample"
                ],
                "additionalType": "Sample",
                "name": "MyGel",
                "@context": {
                  "sdo": "http://schema.org/",
                  "bio": "https://bioschemas.org/",
                  "Sample": "bio:Sample",
                  "name": "sdo:name",
                  "additionalProperties": "bio:additionalProperty"
                }
              }
            ],
            "@context": {
              "sdo": "http://schema.org/",
              "bio": "https://bioschemas.org/",
              "Process": "bio:LabProcess",
              "name": "sdo:name",
              "executesProtocol": "bio:executesLabProtocol",
              "parameterValues": "bio:parameterValue",
              "performer": "sdo:agent",
              "date": "sdo:endTime",
              "inputs": "sdo:object",
              "outputs": "sdo:result",
              "comments": "sdo:disambiguatingDescription"
            }
          }
        ],
        "@context": {
          "sdo": "http://schema.org/",
          "Study": "sdo:Dataset",
          "identifier": "sdo:identifier",
          "title": "sdo:headline",
          "additionalType": "sdo:additionalType",
          "description": "sdo:description",
          "submissionDate": "sdo:dateCreated",
          "publicReleaseDate": "sdo:datePublished",
          "publications": "sdo:citation",
          "people": "sdo:creator",
          "assays": "sdo:hasPart",
          "filename": "sdo:alternateName",
          "comments": "sdo:comment",
          "processSequence": "sdo:about",
          "studyDesignDescriptors": "arc:ARC#ARC_00000037"
        }
      }
    ],
    "@context": {
      "sdo": "http://schema.org/",
      "Investigation": "sdo:Dataset",
      "identifier": "sdo:identifier",
      "title": "sdo:headline",
      "additionalType": "sdo:additionalType",
      "description": "sdo:description",
      "submissionDate": "sdo:dateCreated",
      "publicReleaseDate": "sdo:datePublished",
      "publications": "sdo:citation",
      "people": "sdo:creator",
      "studies": "sdo:hasPart",
      "ontologySourceReferences": "sdo:mentions",
      "comments": "sdo:comment",
      "filename": "sdo:alternateName"
    }
  },
  "conformsTo": {
    "@id": "https://w3id.org/ro/crate/1.1"
  },
  "@context": {
    "sdo": "http://schema.org/",
    "arc": "http://purl.org/nfdi4plants/ontology/",
    "CreativeWork": "sdo:CreativeWork",
    "about": "sdo:about",
    "conformsTo": "sdo:conformsTo"
  }
}"""