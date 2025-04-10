module TestObjects.ROCrate.ArcPrototype

/// https://git.nfdi4plants.org/muehlhaus/ArcPrototype/-/tree/ed12349933062b4440ed2d1e0dc05482853d752d
let ed123499 = """{
  "@context": [
    "https://w3id.org/ro/crate/1.1/context",
    {
      "Sample": "https://bioschemas.org/Sample",
      "additionalProperty": "http://schema.org/additionalProperty",
      "intendedUse": "https://bioschemas.org/intendedUse",
      "computationalTool": "https://bioschemas.org/computationalTool",
      "labEquipment": "https://bioschemas.org/labEquipment",
      "reagent": "https://bioschemas.org/reagent",
      "LabProtocol": "https://bioschemas.org/LabProtocol",
      "executesLabProtocol": "https://bioschemas.org/executesLabProtocol",
      "parameterValue": "https://bioschemas.org/parameterValue",
      "LabProcess": "https://bioschemas.org/LabProcess",
      "measurementMethod": "http://schema.org/measurementMethod"
    }
  ],
  "@graph": [
    {
      "@id": "#Organization_RPTU_University_of_Kaiserslautern",
      "@type": "Organization",
      "name": "RPTU University of Kaiserslautern"
    },
    {
      "@id": "http://purl.org/spar/scoro/principal-investigator",
      "@type": "DefinedTerm",
      "name": "principal investigator",
      "termCode": "http://purl.org/spar/scoro/principal-investigator"
    },
    {
      "@id": "http://orcid.org/0000-0003-3925-6778",
      "@type": "Person",
      "givenName": "Timo",
      "affiliation": {
        "@id": "#Organization_RPTU_University_of_Kaiserslautern"
      },
      "email": "timo.muehlhaus@rptu.de",
      "familyName": "Mühlhaus",
      "jobTitle": {
        "@id": "http://purl.org/spar/scoro/principal-investigator"
      },
      "address": "RPTU University of Kaiserslautern, Paul-Ehrlich-Str. 23 , 67663 Kaiserslautern",
      "telephone": "0 49 (0)631 205 4657"
    },
    {
      "@id": "#Person_Christoph_Garth",
      "@type": "Person",
      "givenName": "Christoph",
      "affiliation": {
        "@id": "#Organization_RPTU_University_of_Kaiserslautern"
      },
      "email": "garth@rptu.de",
      "familyName": "Garth",
      "jobTitle": {
        "@id": "http://purl.org/spar/scoro/principal-investigator"
      }
    },
    {
      "@id": "http://purl.org/spar/scoro/research-assistant",
      "@type": "DefinedTerm",
      "name": "research assistant",
      "termCode": "http://purl.org/spar/scoro/research-assistant"
    },
    {
      "@id": "http://orcid.org/0000-0002-8241-5300",
      "@type": "Person",
      "givenName": "Oliver",
      "affiliation": {
        "@id": "#Organization_RPTU_University_of_Kaiserslautern"
      },
      "email": "maus@nfdi4plants.org",
      "familyName": "Maus",
      "jobTitle": {
        "@id": "http://purl.org/spar/scoro/research-assistant"
      },
      "address": "RPTU University of Kaiserslautern, Erwin-Schrödinger-Str. 56 , 67663 Kaiserslautern"
    },
    {
      "@id": "studies/MaterialPreparation/resources/Sample1_e36ca6b8-19ba-4504-aa82-d4781765873d.png",
      "@type": "File",
      "name": "studies/MaterialPreparation/resources/Sample1_e36ca6b8-19ba-4504-aa82-d4781765873d.png"
    },
    {
      "@id": "studies/MaterialPreparation/resources/Sample2_714ca2b7-22b7-4f69-b83d-9165f624da25.png",
      "@type": "File",
      "name": "studies/MaterialPreparation/resources/Sample2_714ca2b7-22b7-4f69-b83d-9165f624da25.png"
    },
    {
      "@id": "studies/MaterialPreparation/resources/Sample3_66fac760-acc7-4ed4-ba21-2cb67fa36e4d.png",
      "@type": "File",
      "name": "studies/MaterialPreparation/resources/Sample3_66fac760-acc7-4ed4-ba21-2cb67fa36e4d.png"
    },
    {
      "@id": "studies/MaterialPreparation/resources/Sample4_cba5f40c-fc05-44d6-a589-b0e3dafaeefe.png",
      "@type": "File",
      "name": "studies/MaterialPreparation/resources/Sample4_cba5f40c-fc05-44d6-a589-b0e3dafaeefe.png"
    },
    {
      "@id": "studies/MaterialPreparation/resources/Sample5_84c37b60-2342-4226-a36c-4b8dfe84ebe9.png",
      "@type": "File",
      "name": "studies/MaterialPreparation/resources/Sample5_84c37b60-2342-4226-a36c-4b8dfe84ebe9.png"
    },
    {
      "@id": "studies/MaterialPreparation/resources/Sample6_208df064-4b1c-4da0-a1f8-6412e1fb2284.png",
      "@type": "File",
      "name": "studies/MaterialPreparation/resources/Sample6_208df064-4b1c-4da0-a1f8-6412e1fb2284.png"
    },
    {
      "@id": "#CharacteristicValue_organism_Arabidopsis_thaliana",
      "@type": "PropertyValue",
      "additionalType": "CharacteristicValue",
      "name": "organism",
      "value": "Arabidopsis thaliana",
      "propertyID": "https://bioregistry.io/OBI:0100026",
      "valueReference": "http://purl.obolibrary.org/obo/NCBITaxon_3702",
      "columnIndex": "0"
    },
    {
      "@id": "#CharacteristicValue_biological_replicate_1",
      "@type": "PropertyValue",
      "additionalType": "CharacteristicValue",
      "name": "biological replicate",
      "value": "1",
      "propertyID": "http://purl.org/nfdi4plants/ontology/dpbo/DPBO_0000042",
      "columnIndex": "1"
    },
    {
      "@id": "#Source_Source1",
      "@type": "Sample",
      "additionalType": "Source",
      "name": "Source1",
      "additionalProperty": [
        {
          "@id": "#CharacteristicValue_organism_Arabidopsis_thaliana"
        },
        {
          "@id": "#CharacteristicValue_biological_replicate_1"
        }
      ]
    },
    {
      "@id": "#Sample_Cultivation_Flask1",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "Cultivation Flask1"
    },
    {
      "@id": "https://bioregistry.io/EFO:0003789",
      "@type": "DefinedTerm",
      "name": "growth protocol",
      "termCode": "https://bioregistry.io/EFO:0003789"
    },
    {
      "@id": "#Protocol_CellCultivation",
      "@type": "LabProtocol",
      "intendedUse": {
        "@id": "https://bioregistry.io/EFO:0003789"
      }
    },
    {
      "@id": "#Process_CellCultivation_0",
      "@type": "LabProcess",
      "name": "CellCultivation_0",
      "object": {
        "@id": "#Source_Source1"
      },
      "result": {
        "@id": "#Sample_Cultivation_Flask1"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_CellCultivation"
      }
    },
    {
      "@id": "#Source_Source2",
      "@type": "Sample",
      "additionalType": "Source",
      "name": "Source2",
      "additionalProperty": [
        {
          "@id": "#CharacteristicValue_organism_Arabidopsis_thaliana"
        },
        {
          "@id": "#CharacteristicValue_biological_replicate_1"
        }
      ]
    },
    {
      "@id": "#Sample_Cultivation_Flask2",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "Cultivation Flask2"
    },
    {
      "@id": "#Process_CellCultivation_1",
      "@type": "LabProcess",
      "name": "CellCultivation_1",
      "object": {
        "@id": "#Source_Source2"
      },
      "result": {
        "@id": "#Sample_Cultivation_Flask2"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_CellCultivation"
      }
    },
    {
      "@id": "#Source_Source3",
      "@type": "Sample",
      "additionalType": "Source",
      "name": "Source3",
      "additionalProperty": [
        {
          "@id": "#CharacteristicValue_organism_Arabidopsis_thaliana"
        },
        {
          "@id": "#CharacteristicValue_biological_replicate_1"
        }
      ]
    },
    {
      "@id": "#Sample_Cultivation_Flask3",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "Cultivation Flask3"
    },
    {
      "@id": "#Process_CellCultivation_2",
      "@type": "LabProcess",
      "name": "CellCultivation_2",
      "object": {
        "@id": "#Source_Source3"
      },
      "result": {
        "@id": "#Sample_Cultivation_Flask3"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_CellCultivation"
      }
    },
    {
      "@id": "#CharacteristicValue_biological_replicate_2",
      "@type": "PropertyValue",
      "additionalType": "CharacteristicValue",
      "name": "biological replicate",
      "value": "2",
      "propertyID": "http://purl.org/nfdi4plants/ontology/dpbo/DPBO_0000042",
      "columnIndex": "1"
    },
    {
      "@id": "#Source_Source4",
      "@type": "Sample",
      "additionalType": "Source",
      "name": "Source4",
      "additionalProperty": [
        {
          "@id": "#CharacteristicValue_organism_Arabidopsis_thaliana"
        },
        {
          "@id": "#CharacteristicValue_biological_replicate_2"
        }
      ]
    },
    {
      "@id": "#Sample_Cultivation_Flask4",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "Cultivation Flask4"
    },
    {
      "@id": "#Process_CellCultivation_3",
      "@type": "LabProcess",
      "name": "CellCultivation_3",
      "object": {
        "@id": "#Source_Source4"
      },
      "result": {
        "@id": "#Sample_Cultivation_Flask4"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_CellCultivation"
      }
    },
    {
      "@id": "#Source_Source5",
      "@type": "Sample",
      "additionalType": "Source",
      "name": "Source5",
      "additionalProperty": [
        {
          "@id": "#CharacteristicValue_organism_Arabidopsis_thaliana"
        },
        {
          "@id": "#CharacteristicValue_biological_replicate_2"
        }
      ]
    },
    {
      "@id": "#Sample_Cultivation_Flask5",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "Cultivation Flask5"
    },
    {
      "@id": "#Process_CellCultivation_4",
      "@type": "LabProcess",
      "name": "CellCultivation_4",
      "object": {
        "@id": "#Source_Source5"
      },
      "result": {
        "@id": "#Sample_Cultivation_Flask5"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_CellCultivation"
      }
    },
    {
      "@id": "#Source_Source6",
      "@type": "Sample",
      "additionalType": "Source",
      "name": "Source6",
      "additionalProperty": [
        {
          "@id": "#CharacteristicValue_organism_Arabidopsis_thaliana"
        },
        {
          "@id": "#CharacteristicValue_biological_replicate_2"
        }
      ]
    },
    {
      "@id": "#Sample_Cultivation_Flask6",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "Cultivation Flask6"
    },
    {
      "@id": "#Process_CellCultivation_5",
      "@type": "LabProcess",
      "name": "CellCultivation_5",
      "object": {
        "@id": "#Source_Source6"
      },
      "result": {
        "@id": "#Sample_Cultivation_Flask6"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_CellCultivation"
      }
    },
    {
      "@id": "#CharacteristicValue_Performed_Procedure_Step_SOP_Instance_UID_e36ca6b8-19ba-4504-aa82-d4781765873d",
      "@type": "PropertyValue",
      "additionalType": "CharacteristicValue",
      "name": "Performed Procedure Step SOP Instance UID",
      "value": "e36ca6b8-19ba-4504-aa82-d4781765873d",
      "propertyID": "https://bioregistry.io/NCIT:C69261",
      "columnIndex": "0"
    },
    {
      "@id": "#Sample_Sample1",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "Sample1",
      "additionalProperty": {
        "@id": "#CharacteristicValue_Performed_Procedure_Step_SOP_Instance_UID_e36ca6b8-19ba-4504-aa82-d4781765873d"
      }
    },
    {
      "@id": "#Protocol_AccessoryDataRetrieval",
      "@type": "LabProtocol"
    },
    {
      "@id": "#Process_AccessoryDataRetrieval_0",
      "@type": "LabProcess",
      "name": "AccessoryDataRetrieval_0",
      "object": {
        "@id": "#Sample_Sample1"
      },
      "result": {
        "@id": "studies/MaterialPreparation/resources/Sample1_e36ca6b8-19ba-4504-aa82-d4781765873d.png"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_AccessoryDataRetrieval"
      }
    },
    {
      "@id": "#CharacteristicValue_Performed_Procedure_Step_SOP_Instance_UID_714ca2b7-22b7-4f69-b83d-9165f624da25",
      "@type": "PropertyValue",
      "additionalType": "CharacteristicValue",
      "name": "Performed Procedure Step SOP Instance UID",
      "value": "714ca2b7-22b7-4f69-b83d-9165f624da25",
      "propertyID": "https://bioregistry.io/NCIT:C69261",
      "columnIndex": "0"
    },
    {
      "@id": "#Sample_Sample2",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "Sample2",
      "additionalProperty": {
        "@id": "#CharacteristicValue_Performed_Procedure_Step_SOP_Instance_UID_714ca2b7-22b7-4f69-b83d-9165f624da25"
      }
    },
    {
      "@id": "#Process_AccessoryDataRetrieval_1",
      "@type": "LabProcess",
      "name": "AccessoryDataRetrieval_1",
      "object": {
        "@id": "#Sample_Sample2"
      },
      "result": {
        "@id": "studies/MaterialPreparation/resources/Sample2_714ca2b7-22b7-4f69-b83d-9165f624da25.png"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_AccessoryDataRetrieval"
      }
    },
    {
      "@id": "#CharacteristicValue_Performed_Procedure_Step_SOP_Instance_UID_66fac760-acc7-4ed4-ba21-2cb67fa36e4d",
      "@type": "PropertyValue",
      "additionalType": "CharacteristicValue",
      "name": "Performed Procedure Step SOP Instance UID",
      "value": "66fac760-acc7-4ed4-ba21-2cb67fa36e4d",
      "propertyID": "https://bioregistry.io/NCIT:C69261",
      "columnIndex": "0"
    },
    {
      "@id": "#Sample_Sample3",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "Sample3",
      "additionalProperty": {
        "@id": "#CharacteristicValue_Performed_Procedure_Step_SOP_Instance_UID_66fac760-acc7-4ed4-ba21-2cb67fa36e4d"
      }
    },
    {
      "@id": "#Process_AccessoryDataRetrieval_2",
      "@type": "LabProcess",
      "name": "AccessoryDataRetrieval_2",
      "object": {
        "@id": "#Sample_Sample3"
      },
      "result": {
        "@id": "studies/MaterialPreparation/resources/Sample3_66fac760-acc7-4ed4-ba21-2cb67fa36e4d.png"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_AccessoryDataRetrieval"
      }
    },
    {
      "@id": "#CharacteristicValue_Performed_Procedure_Step_SOP_Instance_UID_cba5f40c-fc05-44d6-a589-b0e3dafaeefe",
      "@type": "PropertyValue",
      "additionalType": "CharacteristicValue",
      "name": "Performed Procedure Step SOP Instance UID",
      "value": "cba5f40c-fc05-44d6-a589-b0e3dafaeefe",
      "propertyID": "https://bioregistry.io/NCIT:C69261",
      "columnIndex": "0"
    },
    {
      "@id": "#Sample_Sample4",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "Sample4",
      "additionalProperty": {
        "@id": "#CharacteristicValue_Performed_Procedure_Step_SOP_Instance_UID_cba5f40c-fc05-44d6-a589-b0e3dafaeefe"
      }
    },
    {
      "@id": "#Process_AccessoryDataRetrieval_3",
      "@type": "LabProcess",
      "name": "AccessoryDataRetrieval_3",
      "object": {
        "@id": "#Sample_Sample4"
      },
      "result": {
        "@id": "studies/MaterialPreparation/resources/Sample4_cba5f40c-fc05-44d6-a589-b0e3dafaeefe.png"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_AccessoryDataRetrieval"
      }
    },
    {
      "@id": "#CharacteristicValue_Performed_Procedure_Step_SOP_Instance_UID_84c37b60-2342-4226-a36c-4b8dfe84ebe9",
      "@type": "PropertyValue",
      "additionalType": "CharacteristicValue",
      "name": "Performed Procedure Step SOP Instance UID",
      "value": "84c37b60-2342-4226-a36c-4b8dfe84ebe9",
      "propertyID": "https://bioregistry.io/NCIT:C69261",
      "columnIndex": "0"
    },
    {
      "@id": "#Sample_Sample5",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "Sample5",
      "additionalProperty": {
        "@id": "#CharacteristicValue_Performed_Procedure_Step_SOP_Instance_UID_84c37b60-2342-4226-a36c-4b8dfe84ebe9"
      }
    },
    {
      "@id": "#Process_AccessoryDataRetrieval_4",
      "@type": "LabProcess",
      "name": "AccessoryDataRetrieval_4",
      "object": {
        "@id": "#Sample_Sample5"
      },
      "result": {
        "@id": "studies/MaterialPreparation/resources/Sample5_84c37b60-2342-4226-a36c-4b8dfe84ebe9.png"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_AccessoryDataRetrieval"
      }
    },
    {
      "@id": "#CharacteristicValue_Performed_Procedure_Step_SOP_Instance_UID_208df064-4b1c-4da0-a1f8-6412e1fb2284",
      "@type": "PropertyValue",
      "additionalType": "CharacteristicValue",
      "name": "Performed Procedure Step SOP Instance UID",
      "value": "208df064-4b1c-4da0-a1f8-6412e1fb2284",
      "propertyID": "https://bioregistry.io/NCIT:C69261",
      "columnIndex": "0"
    },
    {
      "@id": "#Sample_Sample6",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "Sample6",
      "additionalProperty": {
        "@id": "#CharacteristicValue_Performed_Procedure_Step_SOP_Instance_UID_208df064-4b1c-4da0-a1f8-6412e1fb2284"
      }
    },
    {
      "@id": "#Process_AccessoryDataRetrieval_5",
      "@type": "LabProcess",
      "name": "AccessoryDataRetrieval_5",
      "object": {
        "@id": "#Sample_Sample6"
      },
      "result": {
        "@id": "studies/MaterialPreparation/resources/Sample6_208df064-4b1c-4da0-a1f8-6412e1fb2284.png"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_AccessoryDataRetrieval"
      }
    },
    {
      "@id": "studies/MaterialPreparation/",
      "@type": "Dataset",
      "additionalType": "Study",
      "identifier": "MaterialPreparation",
      "dateModified": "2025-03-14T13:46:21.4200199",
      "description": "In this a devised study to have an exemplary experimental material description.",
      "hasPart": [
        {
          "@id": "studies/MaterialPreparation/resources/Sample1_e36ca6b8-19ba-4504-aa82-d4781765873d.png"
        },
        {
          "@id": "studies/MaterialPreparation/resources/Sample2_714ca2b7-22b7-4f69-b83d-9165f624da25.png"
        },
        {
          "@id": "studies/MaterialPreparation/resources/Sample3_66fac760-acc7-4ed4-ba21-2cb67fa36e4d.png"
        },
        {
          "@id": "studies/MaterialPreparation/resources/Sample4_cba5f40c-fc05-44d6-a589-b0e3dafaeefe.png"
        },
        {
          "@id": "studies/MaterialPreparation/resources/Sample5_84c37b60-2342-4226-a36c-4b8dfe84ebe9.png"
        },
        {
          "@id": "studies/MaterialPreparation/resources/Sample6_208df064-4b1c-4da0-a1f8-6412e1fb2284.png"
        }
      ],
      "name": "Prototype for experimental data",
      "about": [
        {
          "@id": "#Process_CellCultivation_0"
        },
        {
          "@id": "#Process_CellCultivation_1"
        },
        {
          "@id": "#Process_CellCultivation_2"
        },
        {
          "@id": "#Process_CellCultivation_3"
        },
        {
          "@id": "#Process_CellCultivation_4"
        },
        {
          "@id": "#Process_CellCultivation_5"
        },
        {
          "@id": "#Process_AccessoryDataRetrieval_0"
        },
        {
          "@id": "#Process_AccessoryDataRetrieval_1"
        },
        {
          "@id": "#Process_AccessoryDataRetrieval_2"
        },
        {
          "@id": "#Process_AccessoryDataRetrieval_3"
        },
        {
          "@id": "#Process_AccessoryDataRetrieval_4"
        },
        {
          "@id": "#Process_AccessoryDataRetrieval_5"
        }
      ]
    },
    {
      "@id": "#Source_Input_[MyStudyObject]",
      "@type": "Sample",
      "additionalType": "Source",
      "name": "Input [MyStudyObject]"
    },
    {
      "@id": "#Sample_MyGel",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "MyGel"
    },
    {
      "@id": "#Protocol_experiment2",
      "@type": "LabProtocol"
    },
    {
      "@id": "#ParameterValue_protein_assay_SDS-PAGE",
      "@type": "PropertyValue",
      "additionalType": "ParameterValue",
      "name": "protein assay",
      "value": "SDS-PAGE",
      "propertyID": "https://bioregistry.io/EFO:0001458",
      "valueReference": "https://bioregistry.io/EFO:0010936",
      "columnIndex": "0"
    },
    {
      "@id": "#Process_experiment2",
      "@type": "LabProcess",
      "name": "experiment2",
      "object": {
        "@id": "#Source_Input_[MyStudyObject]"
      },
      "result": {
        "@id": "#Sample_MyGel"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_experiment2"
      },
      "parameterValue": {
        "@id": "#ParameterValue_protein_assay_SDS-PAGE"
      }
    },
    {
      "@id": "studies/experiment2/",
      "@type": "Dataset",
      "additionalType": "Study",
      "identifier": "experiment2",
      "dateModified": "2025-03-14T13:46:21.4224202",
      "hasPart": [],
      "about": {
        "@id": "#Process_experiment2"
      }
    },
    {
      "@id": "#Person_Oliver_Maus",
      "@type": "Person",
      "givenName": "Oliver",
      "affiliation": {
        "@id": "#Organization_RPTU_University_of_Kaiserslautern"
      },
      "email": "mailto:maus@nfdi4plants.org",
      "familyName": "Maus",
      "jobTitle": {
        "@id": "http://purl.org/spar/scoro/research-assistant"
      },
      "disambiguatingDescription": "Comment {Name = \"Worksheet\"}"
    },
    {
      "@id": "assays/measurement1/dataset/sample1.raw",
      "@type": "File",
      "name": "assays/measurement1/dataset/sample1.raw"
    },
    {
      "@id": "assays/measurement1/dataset/sample2.raw",
      "@type": "File",
      "name": "assays/measurement1/dataset/sample2.raw"
    },
    {
      "@id": "assays/measurement1/dataset/sample3.raw",
      "@type": "File",
      "name": "assays/measurement1/dataset/sample3.raw"
    },
    {
      "@id": "assays/measurement1/dataset/sample4.raw",
      "@type": "File",
      "name": "assays/measurement1/dataset/sample4.raw"
    },
    {
      "@id": "assays/measurement1/dataset/sample5.raw",
      "@type": "File",
      "name": "assays/measurement1/dataset/sample5.raw"
    },
    {
      "@id": "assays/measurement1/dataset/sample6.raw",
      "@type": "File",
      "name": "assays/measurement1/dataset/sample6.raw"
    },
    {
      "@id": "http://purl.obolibrary.org/obo/xsd_double",
      "@type": "DefinedTerm",
      "name": "double",
      "termCode": "http://purl.obolibrary.org/obo/xsd_double"
    },
    {
      "@id": "assays/measurement1/dataset/proteomics_result.csv#col=1",
      "@type": "File",
      "name": "assays/measurement1/dataset/proteomics_result.csv#col=1",
      "encodingFormat": "text/csv",
      "usageInfo": "https://datatracker.ietf.org/doc/html/rfc7111",
      "about": {
        "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=1"
      },
      "pattern": {
        "@id": "http://purl.obolibrary.org/obo/xsd_double"
      }
    },
    {
      "@id": "assays/measurement1/dataset/proteomics_result.csv#col=2",
      "@type": "File",
      "name": "assays/measurement1/dataset/proteomics_result.csv#col=2",
      "encodingFormat": "text/csv",
      "usageInfo": "https://datatracker.ietf.org/doc/html/rfc7111",
      "about": {
        "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=2"
      },
      "pattern": {
        "@id": "http://purl.obolibrary.org/obo/xsd_double"
      }
    },
    {
      "@id": "assays/measurement1/dataset/proteomics_result.csv#col=3",
      "@type": "File",
      "name": "assays/measurement1/dataset/proteomics_result.csv#col=3",
      "encodingFormat": "text/csv",
      "usageInfo": "https://datatracker.ietf.org/doc/html/rfc7111",
      "about": {
        "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=3"
      },
      "pattern": {
        "@id": "http://purl.obolibrary.org/obo/xsd_double"
      }
    },
    {
      "@id": "assays/measurement1/dataset/proteomics_result.csv#col=4",
      "@type": "File",
      "name": "assays/measurement1/dataset/proteomics_result.csv#col=4",
      "encodingFormat": "text/csv",
      "usageInfo": "https://datatracker.ietf.org/doc/html/rfc7111",
      "about": {
        "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=4"
      },
      "pattern": {
        "@id": "http://purl.obolibrary.org/obo/xsd_double"
      }
    },
    {
      "@id": "assays/measurement1/dataset/proteomics_result.csv#col=5",
      "@type": "File",
      "name": "assays/measurement1/dataset/proteomics_result.csv#col=5",
      "encodingFormat": "text/csv",
      "usageInfo": "https://datatracker.ietf.org/doc/html/rfc7111",
      "about": {
        "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=5"
      },
      "pattern": {
        "@id": "http://purl.obolibrary.org/obo/xsd_double"
      }
    },
    {
      "@id": "assays/measurement1/dataset/proteomics_result.csv#col=6",
      "@type": "File",
      "name": "assays/measurement1/dataset/proteomics_result.csv#col=6",
      "encodingFormat": "text/csv",
      "usageInfo": "https://datatracker.ietf.org/doc/html/rfc7111",
      "about": {
        "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=6"
      },
      "pattern": {
        "@id": "http://purl.obolibrary.org/obo/xsd_double"
      }
    },
    {
      "@id": "assays/measurement1/dataset/proteomics_result.csv#col=7",
      "@type": "File",
      "name": "assays/measurement1/dataset/proteomics_result.csv#col=7",
      "encodingFormat": "text/csv",
      "usageInfo": "https://datatracker.ietf.org/doc/html/rfc7111",
      "about": {
        "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=7"
      },
      "pattern": {
        "@id": "http://purl.obolibrary.org/obo/xsd_double"
      }
    },
    {
      "@id": "assays/measurement1/dataset/proteomics_result.csv#col=8",
      "@type": "File",
      "name": "assays/measurement1/dataset/proteomics_result.csv#col=8",
      "encodingFormat": "text/csv",
      "usageInfo": "https://datatracker.ietf.org/doc/html/rfc7111",
      "about": {
        "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=8"
      },
      "pattern": {
        "@id": "http://purl.obolibrary.org/obo/xsd_double"
      }
    },
    {
      "@id": "assays/measurement1/dataset/proteomics_result.csv#col=9",
      "@type": "File",
      "name": "assays/measurement1/dataset/proteomics_result.csv#col=9",
      "encodingFormat": "text/csv",
      "usageInfo": "https://datatracker.ietf.org/doc/html/rfc7111",
      "about": {
        "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=9"
      },
      "pattern": {
        "@id": "http://purl.obolibrary.org/obo/xsd_double"
      }
    },
    {
      "@id": "assays/measurement1/dataset/proteomics_result.csv#col=10",
      "@type": "File",
      "name": "assays/measurement1/dataset/proteomics_result.csv#col=10",
      "encodingFormat": "text/csv",
      "usageInfo": "https://datatracker.ietf.org/doc/html/rfc7111",
      "about": {
        "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=10"
      },
      "pattern": {
        "@id": "http://purl.obolibrary.org/obo/xsd_double"
      }
    },
    {
      "@id": "assays/measurement1/dataset/proteomics_result.csv#col=11",
      "@type": "File",
      "name": "assays/measurement1/dataset/proteomics_result.csv#col=11",
      "encodingFormat": "text/csv",
      "usageInfo": "https://datatracker.ietf.org/doc/html/rfc7111",
      "about": {
        "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=11"
      },
      "pattern": {
        "@id": "http://purl.obolibrary.org/obo/xsd_double"
      }
    },
    {
      "@id": "assays/measurement1/dataset/proteomics_result.csv#col=12",
      "@type": "File",
      "name": "assays/measurement1/dataset/proteomics_result.csv#col=12",
      "encodingFormat": "text/csv",
      "usageInfo": "https://datatracker.ietf.org/doc/html/rfc7111",
      "about": {
        "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=12"
      },
      "pattern": {
        "@id": "http://purl.obolibrary.org/obo/xsd_double"
      }
    },
    {
      "@id": "assays/measurement1/dataset/proteomics_result.csv",
      "@type": "File",
      "name": "assays/measurement1/dataset/proteomics_result.csv",
      "encodingFormat": "text/csv",
      "hasPart": [
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=1"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=2"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=3"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=4"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=5"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=6"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=7"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=8"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=9"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=10"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=11"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=12"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=1"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=2"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=3"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=4"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=5"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=6"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=7"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=8"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=9"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=10"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=11"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv#col=12"
        }
      ]
    },
    {
      "@id": "#Sample_sample_eppi_1",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "sample eppi 1"
    },
    {
      "@id": "#Component_sonicator_Fisherbrand™_Model_705_Sonic_Dismembrator",
      "@type": "PropertyValue",
      "additionalType": "Component",
      "name": "sonicator",
      "value": "Fisherbrand™ Model 705 Sonic Dismembrator",
      "propertyID": "https://bioregistry.io/OBI:0400114",
      "columnIndex": "1"
    },
    {
      "@id": "#Component_centrifuge_Eppendorf™_Centrifuge_5420",
      "@type": "PropertyValue",
      "additionalType": "Component",
      "name": "centrifuge",
      "value": "Eppendorf™ Centrifuge 5420",
      "propertyID": "https://bioregistry.io/OBI:0400106",
      "columnIndex": "3"
    },
    {
      "@id": "#Protocol_Cell_Lysis",
      "@type": "LabProtocol",
      "labEquipment": [
        {
          "@id": "#Component_sonicator_Fisherbrand™_Model_705_Sonic_Dismembrator"
        },
        {
          "@id": "#Component_centrifuge_Eppendorf™_Centrifuge_5420"
        }
      ]
    },
    {
      "@id": "#ParameterValue_cell_lysis_Sonication",
      "@type": "PropertyValue",
      "additionalType": "ParameterValue",
      "name": "cell lysis",
      "value": "Sonication",
      "propertyID": "https://bioregistry.io/OBI:0302894",
      "valueReference": "https://bioregistry.io/NCIT:C81871",
      "columnIndex": "0"
    },
    {
      "@id": "#ParameterValue_centrifugation_10",
      "@type": "PropertyValue",
      "additionalType": "ParameterValue",
      "name": "centrifugation",
      "value": "10",
      "propertyID": "https://bioregistry.io/OBI:0302886",
      "unitText": "g unit",
      "columnIndex": "2"
    },
    {
      "@id": "#Process_Cell_Lysis_0",
      "@type": "LabProcess",
      "name": "Cell Lysis_0",
      "object": {
        "@id": "#Sample_Cultivation_Flask1"
      },
      "result": {
        "@id": "#Sample_sample_eppi_1"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Cell_Lysis"
      },
      "parameterValue": [
        {
          "@id": "#ParameterValue_cell_lysis_Sonication"
        },
        {
          "@id": "#ParameterValue_centrifugation_10"
        }
      ]
    },
    {
      "@id": "#Sample_sample_eppi_2",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "sample eppi 2"
    },
    {
      "@id": "#Process_Cell_Lysis_1",
      "@type": "LabProcess",
      "name": "Cell Lysis_1",
      "object": {
        "@id": "#Sample_Cultivation_Flask2"
      },
      "result": {
        "@id": "#Sample_sample_eppi_2"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Cell_Lysis"
      },
      "parameterValue": [
        {
          "@id": "#ParameterValue_cell_lysis_Sonication"
        },
        {
          "@id": "#ParameterValue_centrifugation_10"
        }
      ]
    },
    {
      "@id": "#Sample_sample_eppi_3",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "sample eppi 3"
    },
    {
      "@id": "#Process_Cell_Lysis_2",
      "@type": "LabProcess",
      "name": "Cell Lysis_2",
      "object": {
        "@id": "#Sample_Cultivation_Flask3"
      },
      "result": {
        "@id": "#Sample_sample_eppi_3"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Cell_Lysis"
      },
      "parameterValue": [
        {
          "@id": "#ParameterValue_cell_lysis_Sonication"
        },
        {
          "@id": "#ParameterValue_centrifugation_10"
        }
      ]
    },
    {
      "@id": "#Sample_sample_eppi_4",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "sample eppi 4"
    },
    {
      "@id": "#Process_Cell_Lysis_3",
      "@type": "LabProcess",
      "name": "Cell Lysis_3",
      "object": {
        "@id": "#Sample_Cultivation_Flask4"
      },
      "result": {
        "@id": "#Sample_sample_eppi_4"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Cell_Lysis"
      },
      "parameterValue": [
        {
          "@id": "#ParameterValue_cell_lysis_Sonication"
        },
        {
          "@id": "#ParameterValue_centrifugation_10"
        }
      ]
    },
    {
      "@id": "#Sample_sample_eppi_5",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "sample eppi 5"
    },
    {
      "@id": "#Process_Cell_Lysis_4",
      "@type": "LabProcess",
      "name": "Cell Lysis_4",
      "object": {
        "@id": "#Sample_Cultivation_Flask5"
      },
      "result": {
        "@id": "#Sample_sample_eppi_5"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Cell_Lysis"
      },
      "parameterValue": [
        {
          "@id": "#ParameterValue_cell_lysis_Sonication"
        },
        {
          "@id": "#ParameterValue_centrifugation_10"
        }
      ]
    },
    {
      "@id": "#Sample_sample_eppi_6",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "sample eppi 6"
    },
    {
      "@id": "#Process_Cell_Lysis_5",
      "@type": "LabProcess",
      "name": "Cell Lysis_5",
      "object": {
        "@id": "#Sample_Cultivation_Flask6"
      },
      "result": {
        "@id": "#Sample_sample_eppi_6"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Cell_Lysis"
      },
      "parameterValue": [
        {
          "@id": "#ParameterValue_cell_lysis_Sonication"
        },
        {
          "@id": "#ParameterValue_centrifugation_10"
        }
      ]
    },
    {
      "@id": "#Sample_sample_eppi_extracted_1",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "sample eppi extracted 1"
    },
    {
      "@id": "#Protocol_extractionProtocol.txt_Protein_Extraction",
      "@type": "LabProtocol",
      "name": "extractionProtocol.txt"
    },
    {
      "@id": "#Process_Protein_Extraction_0",
      "@type": "LabProcess",
      "name": "Protein Extraction_0",
      "object": {
        "@id": "#Sample_sample_eppi_1"
      },
      "result": {
        "@id": "#Sample_sample_eppi_extracted_1"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_extractionProtocol.txt_Protein_Extraction"
      }
    },
    {
      "@id": "#Sample_sample_eppi_extracted_2",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "sample eppi extracted 2"
    },
    {
      "@id": "#Process_Protein_Extraction_1",
      "@type": "LabProcess",
      "name": "Protein Extraction_1",
      "object": {
        "@id": "#Sample_sample_eppi_2"
      },
      "result": {
        "@id": "#Sample_sample_eppi_extracted_2"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_extractionProtocol.txt_Protein_Extraction"
      }
    },
    {
      "@id": "#Sample_sample_eppi_extracted_3",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "sample eppi extracted 3"
    },
    {
      "@id": "#Process_Protein_Extraction_2",
      "@type": "LabProcess",
      "name": "Protein Extraction_2",
      "object": {
        "@id": "#Sample_sample_eppi_3"
      },
      "result": {
        "@id": "#Sample_sample_eppi_extracted_3"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_extractionProtocol.txt_Protein_Extraction"
      }
    },
    {
      "@id": "#Sample_sample_eppi_extracted_4",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "sample eppi extracted 4"
    },
    {
      "@id": "#Process_Protein_Extraction_3",
      "@type": "LabProcess",
      "name": "Protein Extraction_3",
      "object": {
        "@id": "#Sample_sample_eppi_4"
      },
      "result": {
        "@id": "#Sample_sample_eppi_extracted_4"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_extractionProtocol.txt_Protein_Extraction"
      }
    },
    {
      "@id": "#Sample_sample_eppi_extracted_5",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "sample eppi extracted 5"
    },
    {
      "@id": "#Process_Protein_Extraction_4",
      "@type": "LabProcess",
      "name": "Protein Extraction_4",
      "object": {
        "@id": "#Sample_sample_eppi_5"
      },
      "result": {
        "@id": "#Sample_sample_eppi_extracted_5"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_extractionProtocol.txt_Protein_Extraction"
      }
    },
    {
      "@id": "#Sample_sample_eppi_extracted_6",
      "@type": "Sample",
      "additionalType": "Sample",
      "name": "sample eppi extracted 6"
    },
    {
      "@id": "#Process_Protein_Extraction_5",
      "@type": "LabProcess",
      "name": "Protein Extraction_5",
      "object": {
        "@id": "#Sample_sample_eppi_6"
      },
      "result": {
        "@id": "#Sample_sample_eppi_extracted_6"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_extractionProtocol.txt_Protein_Extraction"
      }
    },
    {
      "@id": "#Component_cleavage_agent_name_Trypsin",
      "@type": "PropertyValue",
      "additionalType": "Component",
      "name": "cleavage agent name",
      "value": "Trypsin",
      "propertyID": "https://www.ebi.ac.uk/ols4/ontologies/ms/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252FMS_1001045",
      "valueReference": "https://www.ebi.ac.uk/ols4/ontologies/ms/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252FMS_1001251",
      "columnIndex": "0"
    },
    {
      "@id": "#Component_instrument_model_TripleTOF_5600+",
      "@type": "PropertyValue",
      "additionalType": "Component",
      "name": "instrument model",
      "value": "TripleTOF 5600+",
      "propertyID": "https://www.ebi.ac.uk/ols4/ontologies/ms/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252FMS_1000031",
      "valueReference": "https://www.ebi.ac.uk/ols4/ontologies/ms/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252FMS_1002584",
      "columnIndex": "1"
    },
    {
      "@id": "#Protocol_Protein_Measurement",
      "@type": "LabProtocol",
      "labEquipment": [
        {
          "@id": "#Component_cleavage_agent_name_Trypsin"
        },
        {
          "@id": "#Component_instrument_model_TripleTOF_5600+"
        }
      ]
    },
    {
      "@id": "#Process_Protein_Measurement_0",
      "@type": "LabProcess",
      "name": "Protein Measurement_0",
      "object": {
        "@id": "#Sample_sample_eppi_extracted_1"
      },
      "result": {
        "@id": "assays/measurement1/dataset/sample1.raw"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Protein_Measurement"
      }
    },
    {
      "@id": "#Process_Protein_Measurement_1",
      "@type": "LabProcess",
      "name": "Protein Measurement_1",
      "object": {
        "@id": "#Sample_sample_eppi_extracted_2"
      },
      "result": {
        "@id": "assays/measurement1/dataset/sample2.raw"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Protein_Measurement"
      }
    },
    {
      "@id": "#Process_Protein_Measurement_2",
      "@type": "LabProcess",
      "name": "Protein Measurement_2",
      "object": {
        "@id": "#Sample_sample_eppi_extracted_3"
      },
      "result": {
        "@id": "assays/measurement1/dataset/sample3.raw"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Protein_Measurement"
      }
    },
    {
      "@id": "#Process_Protein_Measurement_3",
      "@type": "LabProcess",
      "name": "Protein Measurement_3",
      "object": {
        "@id": "#Sample_sample_eppi_extracted_4"
      },
      "result": {
        "@id": "assays/measurement1/dataset/sample4.raw"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Protein_Measurement"
      }
    },
    {
      "@id": "#Process_Protein_Measurement_4",
      "@type": "LabProcess",
      "name": "Protein Measurement_4",
      "object": {
        "@id": "#Sample_sample_eppi_extracted_5"
      },
      "result": {
        "@id": "assays/measurement1/dataset/sample5.raw"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Protein_Measurement"
      }
    },
    {
      "@id": "#Process_Protein_Measurement_5",
      "@type": "LabProcess",
      "name": "Protein Measurement_5",
      "object": {
        "@id": "#Sample_sample_eppi_extracted_6"
      },
      "result": {
        "@id": "assays/measurement1/dataset/sample6.raw"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Protein_Measurement"
      }
    },
    {
      "@id": "#Protocol_Computational_Proteome_Analysis",
      "@type": "LabProtocol"
    },
    {
      "@id": "#ParameterValue_software_ProteomIqon",
      "@type": "PropertyValue",
      "additionalType": "ParameterValue",
      "name": "software",
      "value": "ProteomIqon",
      "propertyID": "https://www.ebi.ac.uk/ols4/ontologies/ms/classes/http%253A%252F%252Fpurl.obolibrary.org%252Fobo%252FMS_1000531",
      "columnIndex": "0"
    },
    {
      "@id": "#Process_Computational_Proteome_Analysis_0",
      "@type": "LabProcess",
      "name": "Computational Proteome Analysis_0",
      "object": {
        "@id": "assays/measurement1/dataset/sample1.raw"
      },
      "result": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=1"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Computational_Proteome_Analysis"
      },
      "parameterValue": {
        "@id": "#ParameterValue_software_ProteomIqon"
      }
    },
    {
      "@id": "#Process_Computational_Proteome_Analysis_1",
      "@type": "LabProcess",
      "name": "Computational Proteome Analysis_1",
      "object": {
        "@id": "assays/measurement1/dataset/sample1.raw"
      },
      "result": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=2"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Computational_Proteome_Analysis"
      },
      "parameterValue": {
        "@id": "#ParameterValue_software_ProteomIqon"
      }
    },
    {
      "@id": "#Process_Computational_Proteome_Analysis_2",
      "@type": "LabProcess",
      "name": "Computational Proteome Analysis_2",
      "object": {
        "@id": "assays/measurement1/dataset/sample2.raw"
      },
      "result": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=3"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Computational_Proteome_Analysis"
      },
      "parameterValue": {
        "@id": "#ParameterValue_software_ProteomIqon"
      }
    },
    {
      "@id": "#Process_Computational_Proteome_Analysis_3",
      "@type": "LabProcess",
      "name": "Computational Proteome Analysis_3",
      "object": {
        "@id": "assays/measurement1/dataset/sample2.raw"
      },
      "result": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=4"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Computational_Proteome_Analysis"
      },
      "parameterValue": {
        "@id": "#ParameterValue_software_ProteomIqon"
      }
    },
    {
      "@id": "#Process_Computational_Proteome_Analysis_4",
      "@type": "LabProcess",
      "name": "Computational Proteome Analysis_4",
      "object": {
        "@id": "assays/measurement1/dataset/sample3.raw"
      },
      "result": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=5"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Computational_Proteome_Analysis"
      },
      "parameterValue": {
        "@id": "#ParameterValue_software_ProteomIqon"
      }
    },
    {
      "@id": "#Process_Computational_Proteome_Analysis_5",
      "@type": "LabProcess",
      "name": "Computational Proteome Analysis_5",
      "object": {
        "@id": "assays/measurement1/dataset/sample3.raw"
      },
      "result": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=6"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Computational_Proteome_Analysis"
      },
      "parameterValue": {
        "@id": "#ParameterValue_software_ProteomIqon"
      }
    },
    {
      "@id": "#Process_Computational_Proteome_Analysis_6",
      "@type": "LabProcess",
      "name": "Computational Proteome Analysis_6",
      "object": {
        "@id": "assays/measurement1/dataset/sample4.raw"
      },
      "result": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=7"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Computational_Proteome_Analysis"
      },
      "parameterValue": {
        "@id": "#ParameterValue_software_ProteomIqon"
      }
    },
    {
      "@id": "#Process_Computational_Proteome_Analysis_7",
      "@type": "LabProcess",
      "name": "Computational Proteome Analysis_7",
      "object": {
        "@id": "assays/measurement1/dataset/sample4.raw"
      },
      "result": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=8"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Computational_Proteome_Analysis"
      },
      "parameterValue": {
        "@id": "#ParameterValue_software_ProteomIqon"
      }
    },
    {
      "@id": "#Process_Computational_Proteome_Analysis_8",
      "@type": "LabProcess",
      "name": "Computational Proteome Analysis_8",
      "object": {
        "@id": "assays/measurement1/dataset/sample5.raw"
      },
      "result": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=9"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Computational_Proteome_Analysis"
      },
      "parameterValue": {
        "@id": "#ParameterValue_software_ProteomIqon"
      }
    },
    {
      "@id": "#Process_Computational_Proteome_Analysis_9",
      "@type": "LabProcess",
      "name": "Computational Proteome Analysis_9",
      "object": {
        "@id": "assays/measurement1/dataset/sample5.raw"
      },
      "result": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=10"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Computational_Proteome_Analysis"
      },
      "parameterValue": {
        "@id": "#ParameterValue_software_ProteomIqon"
      }
    },
    {
      "@id": "#Process_Computational_Proteome_Analysis_10",
      "@type": "LabProcess",
      "name": "Computational Proteome Analysis_10",
      "object": {
        "@id": "assays/measurement1/dataset/sample6.raw"
      },
      "result": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=11"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Computational_Proteome_Analysis"
      },
      "parameterValue": {
        "@id": "#ParameterValue_software_ProteomIqon"
      }
    },
    {
      "@id": "#Process_Computational_Proteome_Analysis_11",
      "@type": "LabProcess",
      "name": "Computational Proteome Analysis_11",
      "object": {
        "@id": "assays/measurement1/dataset/sample6.raw"
      },
      "result": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=12"
      },
      "executesLabProtocol": {
        "@id": "#Protocol_Computational_Proteome_Analysis"
      },
      "parameterValue": {
        "@id": "#ParameterValue_software_ProteomIqon"
      }
    },
    {
      "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=1",
      "@type": "PropertyValue",
      "name": "FragmentDescriptor",
      "value": "protein amount",
      "propertyID": "https://github.com/nfdi4plants/ARC-specification/blob/dev/ISA-XLSX.md#datamap-table-sheets",
      "unitCode": "http://purl.obolibrary.org/obo/UO_0000274",
      "unitText": "microgram per milliliter",
      "valueReference": "http://purl.obolibrary.org/obo/OBA_VT0010120",
      "disambiguatingDescription": "Comment {Name = \"GeneratedBy\", Value = \"\"}",
      "subjectOf": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=1"
      }
    },
    {
      "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=2",
      "@type": "PropertyValue",
      "name": "FragmentDescriptor",
      "value": "protein amount",
      "propertyID": "https://github.com/nfdi4plants/ARC-specification/blob/dev/ISA-XLSX.md#datamap-table-sheets",
      "unitCode": "http://purl.obolibrary.org/obo/UO_0000274",
      "unitText": "microgram per milliliter",
      "valueReference": "http://purl.obolibrary.org/obo/OBA_VT0010120",
      "disambiguatingDescription": "Comment {Name = \"GeneratedBy\", Value = \"\"}",
      "subjectOf": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=2"
      }
    },
    {
      "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=3",
      "@type": "PropertyValue",
      "name": "FragmentDescriptor",
      "value": "protein amount",
      "propertyID": "https://github.com/nfdi4plants/ARC-specification/blob/dev/ISA-XLSX.md#datamap-table-sheets",
      "unitCode": "http://purl.obolibrary.org/obo/UO_0000274",
      "unitText": "microgram per milliliter",
      "valueReference": "http://purl.obolibrary.org/obo/OBA_VT0010120",
      "disambiguatingDescription": "Comment {Name = \"GeneratedBy\", Value = \"\"}",
      "subjectOf": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=3"
      }
    },
    {
      "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=4",
      "@type": "PropertyValue",
      "name": "FragmentDescriptor",
      "value": "protein amount",
      "propertyID": "https://github.com/nfdi4plants/ARC-specification/blob/dev/ISA-XLSX.md#datamap-table-sheets",
      "unitCode": "http://purl.obolibrary.org/obo/UO_0000274",
      "unitText": "microgram per milliliter",
      "valueReference": "http://purl.obolibrary.org/obo/OBA_VT0010120",
      "disambiguatingDescription": "Comment {Name = \"GeneratedBy\", Value = \"\"}",
      "subjectOf": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=4"
      }
    },
    {
      "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=5",
      "@type": "PropertyValue",
      "name": "FragmentDescriptor",
      "value": "protein amount",
      "propertyID": "https://github.com/nfdi4plants/ARC-specification/blob/dev/ISA-XLSX.md#datamap-table-sheets",
      "unitCode": "http://purl.obolibrary.org/obo/UO_0000274",
      "unitText": "microgram per milliliter",
      "valueReference": "http://purl.obolibrary.org/obo/OBA_VT0010120",
      "disambiguatingDescription": "Comment {Name = \"GeneratedBy\", Value = \"\"}",
      "subjectOf": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=5"
      }
    },
    {
      "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=6",
      "@type": "PropertyValue",
      "name": "FragmentDescriptor",
      "value": "protein amount",
      "propertyID": "https://github.com/nfdi4plants/ARC-specification/blob/dev/ISA-XLSX.md#datamap-table-sheets",
      "unitCode": "http://purl.obolibrary.org/obo/UO_0000274",
      "unitText": "microgram per milliliter",
      "valueReference": "http://purl.obolibrary.org/obo/OBA_VT0010120",
      "disambiguatingDescription": "Comment {Name = \"GeneratedBy\", Value = \"\"}",
      "subjectOf": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=6"
      }
    },
    {
      "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=7",
      "@type": "PropertyValue",
      "name": "FragmentDescriptor",
      "value": "protein amount",
      "propertyID": "https://github.com/nfdi4plants/ARC-specification/blob/dev/ISA-XLSX.md#datamap-table-sheets",
      "unitCode": "http://purl.obolibrary.org/obo/UO_0000274",
      "unitText": "microgram per milliliter",
      "valueReference": "http://purl.obolibrary.org/obo/OBA_VT0010120",
      "disambiguatingDescription": "Comment {Name = \"GeneratedBy\", Value = \"\"}",
      "subjectOf": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=7"
      }
    },
    {
      "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=8",
      "@type": "PropertyValue",
      "name": "FragmentDescriptor",
      "value": "protein amount",
      "propertyID": "https://github.com/nfdi4plants/ARC-specification/blob/dev/ISA-XLSX.md#datamap-table-sheets",
      "unitCode": "http://purl.obolibrary.org/obo/UO_0000274",
      "unitText": "microgram per milliliter",
      "valueReference": "http://purl.obolibrary.org/obo/OBA_VT0010120",
      "disambiguatingDescription": "Comment {Name = \"GeneratedBy\", Value = \"\"}",
      "subjectOf": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=8"
      }
    },
    {
      "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=9",
      "@type": "PropertyValue",
      "name": "FragmentDescriptor",
      "value": "protein amount",
      "propertyID": "https://github.com/nfdi4plants/ARC-specification/blob/dev/ISA-XLSX.md#datamap-table-sheets",
      "unitCode": "http://purl.obolibrary.org/obo/UO_0000274",
      "unitText": "microgram per milliliter",
      "valueReference": "http://purl.obolibrary.org/obo/OBA_VT0010120",
      "disambiguatingDescription": "Comment {Name = \"GeneratedBy\", Value = \"\"}",
      "subjectOf": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=9"
      }
    },
    {
      "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=10",
      "@type": "PropertyValue",
      "name": "FragmentDescriptor",
      "value": "protein amount",
      "propertyID": "https://github.com/nfdi4plants/ARC-specification/blob/dev/ISA-XLSX.md#datamap-table-sheets",
      "unitCode": "http://purl.obolibrary.org/obo/UO_0000274",
      "unitText": "microgram per milliliter",
      "valueReference": "http://purl.obolibrary.org/obo/OBA_VT0010120",
      "disambiguatingDescription": "Comment {Name = \"GeneratedBy\", Value = \"\"}",
      "subjectOf": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=10"
      }
    },
    {
      "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=11",
      "@type": "PropertyValue",
      "name": "FragmentDescriptor",
      "value": "protein amount",
      "propertyID": "https://github.com/nfdi4plants/ARC-specification/blob/dev/ISA-XLSX.md#datamap-table-sheets",
      "unitCode": "http://purl.obolibrary.org/obo/UO_0000274",
      "unitText": "microgram per milliliter",
      "valueReference": "http://purl.obolibrary.org/obo/OBA_VT0010120",
      "disambiguatingDescription": "Comment {Name = \"GeneratedBy\", Value = \"\"}",
      "subjectOf": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=11"
      }
    },
    {
      "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=12",
      "@type": "PropertyValue",
      "name": "FragmentDescriptor",
      "value": "protein amount",
      "propertyID": "https://github.com/nfdi4plants/ARC-specification/blob/dev/ISA-XLSX.md#datamap-table-sheets",
      "unitCode": "http://purl.obolibrary.org/obo/UO_0000274",
      "unitText": "microgram per milliliter",
      "valueReference": "http://purl.obolibrary.org/obo/OBA_VT0010120",
      "disambiguatingDescription": "Comment {Name = \"GeneratedBy\", Value = \"\"}",
      "subjectOf": {
        "@id": "assays/measurement1/dataset/proteomics_result.csv#col=12"
      }
    },
    {
      "@id": "assay/measurement1/",
      "@type": "Dataset",
      "additionalType": "Assay",
      "identifier": "measurement1",
      "creator": {
        "@id": "#Person_Oliver_Maus"
      },
      "hasPart": [
        {
          "@id": "assays/measurement1/dataset/sample1.raw"
        },
        {
          "@id": "assays/measurement1/dataset/sample2.raw"
        },
        {
          "@id": "assays/measurement1/dataset/sample3.raw"
        },
        {
          "@id": "assays/measurement1/dataset/sample4.raw"
        },
        {
          "@id": "assays/measurement1/dataset/sample5.raw"
        },
        {
          "@id": "assays/measurement1/dataset/sample6.raw"
        },
        {
          "@id": "assays/measurement1/dataset/proteomics_result.csv"
        }
      ],
      "about": [
        {
          "@id": "#Process_Cell_Lysis_0"
        },
        {
          "@id": "#Process_Cell_Lysis_1"
        },
        {
          "@id": "#Process_Cell_Lysis_2"
        },
        {
          "@id": "#Process_Cell_Lysis_3"
        },
        {
          "@id": "#Process_Cell_Lysis_4"
        },
        {
          "@id": "#Process_Cell_Lysis_5"
        },
        {
          "@id": "#Process_Protein_Extraction_0"
        },
        {
          "@id": "#Process_Protein_Extraction_1"
        },
        {
          "@id": "#Process_Protein_Extraction_2"
        },
        {
          "@id": "#Process_Protein_Extraction_3"
        },
        {
          "@id": "#Process_Protein_Extraction_4"
        },
        {
          "@id": "#Process_Protein_Extraction_5"
        },
        {
          "@id": "#Process_Protein_Measurement_0"
        },
        {
          "@id": "#Process_Protein_Measurement_1"
        },
        {
          "@id": "#Process_Protein_Measurement_2"
        },
        {
          "@id": "#Process_Protein_Measurement_3"
        },
        {
          "@id": "#Process_Protein_Measurement_4"
        },
        {
          "@id": "#Process_Protein_Measurement_5"
        },
        {
          "@id": "#Process_Computational_Proteome_Analysis_0"
        },
        {
          "@id": "#Process_Computational_Proteome_Analysis_1"
        },
        {
          "@id": "#Process_Computational_Proteome_Analysis_2"
        },
        {
          "@id": "#Process_Computational_Proteome_Analysis_3"
        },
        {
          "@id": "#Process_Computational_Proteome_Analysis_4"
        },
        {
          "@id": "#Process_Computational_Proteome_Analysis_5"
        },
        {
          "@id": "#Process_Computational_Proteome_Analysis_6"
        },
        {
          "@id": "#Process_Computational_Proteome_Analysis_7"
        },
        {
          "@id": "#Process_Computational_Proteome_Analysis_8"
        },
        {
          "@id": "#Process_Computational_Proteome_Analysis_9"
        },
        {
          "@id": "#Process_Computational_Proteome_Analysis_10"
        },
        {
          "@id": "#Process_Computational_Proteome_Analysis_11"
        }
      ],
      "variableMeasured": [
        {
          "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=1"
        },
        {
          "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=2"
        },
        {
          "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=3"
        },
        {
          "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=4"
        },
        {
          "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=5"
        },
        {
          "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=6"
        },
        {
          "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=7"
        },
        {
          "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=8"
        },
        {
          "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=9"
        },
        {
          "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=10"
        },
        {
          "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=11"
        },
        {
          "@id": "#Descriptor_assays/measurement1/dataset/proteomics_result.csv#col=12"
        }
      ]
    },
    {
      "@id": "assay/measurement2/",
      "@type": "Dataset",
      "additionalType": "Assay",
      "identifier": "measurement2"
    },
    {
      "@id": "./",
      "@type": "Dataset",
      "additionalType": "Investigation",
      "identifier": "ArcPrototype",
      "creator": [
        {
          "@id": "http://orcid.org/0000-0003-3925-6778"
        },
        {
          "@id": "#Person_Christoph_Garth"
        },
        {
          "@id": "http://orcid.org/0000-0002-8241-5300"
        }
      ],
      "datePublished": "2025-03-14T13:46:21.4227877",
      "description": "A prototypic ARC that implements all specification standards accordingly",
      "hasPart": [
        {
          "@id": "studies/MaterialPreparation/"
        },
        {
          "@id": "studies/experiment2/"
        },
        {
          "@id": "assay/measurement1/"
        },
        {
          "@id": "assay/measurement2/"
        }
      ],
      "name": "ArcPrototype",
      "license": "ALL RIGHTS RESERVED BY THE AUTHORS"
    },
    {
      "@id": "ro-crate-metadata.json",
      "@type": "CreativeWork",
      "conformsTo": {
        "@id": "https://w3id.org/ro/crate/1.1"
      },
      "about": {
        "@id": "./"
      }
    }
  ]
}"""