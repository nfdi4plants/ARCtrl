module TestObjects.Json.Process

let process' = 

    """
    {
    "@id": "#process/standard_trypsin_digestion",
    "name": "standard_trypsin_digestion",
    "executesProtocol": {
        "@id": "#protocols/peptide_digestion",
        "name": "peptide_digestion",
    
        "protocolType": {
            "@id": "protein_digestion",
            "annotationValue": "Protein Digestion",
            "termSource": "NCIT",
            "termAccession": "http://purl.obolibrary.org/obo/NCIT_C70845",
            "comments": []
        },
        "description": "The isolated proteins get solubilized. Given protease is added and the solution is heated to a given temperature. After a given amount of time, the digestion is stopped by adding a denaturation agent.",
        "uri": "http://madeUpProtocolWebsize.org/protein_digestion",
        "version": "1.0.0",
        "parameters": [
            {
                "@id": "protease",
                "parameterName": {
                    "@id": "protease",
                    "annotationValue": "Peptidase",
                    "termSource": "MS",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C16965",
                    "comments": []
                }
            },
            {
                "@id": "temperature",
                "parameterName": {
                    "@id": "temperature",
                    "annotationValue": "temperature",
                    "termSource": "Ontobee",
                    "termAccession": "http://purl.obolibrary.org/obo/NCRO_0000029",
                    "comments": []
    
                }
            },
            {
                "@id": "time",
                "parameterName": {
                    "@id": "time",
                    "annotationValue": "time",
                    "termSource": "EFO",
                    "termAccession": "http://www.ebi.ac.uk/efo/EFO_0000721",
                    "comments": []
                }
            }
        ],
        "components": [
            {
                "componentName": "digestion_stopper",
                "componentType": {
                    "@id": "formic_acid",
                    "annotationValue": "Formic Acid",
                    "termSource": "NCIT",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C83719",
                    "comments": []
                }
            },
            {
                "componentName": "heater",
                "componentType": {
                    "@id": "heater",
                    "annotationValue": "Heater Device",
                    "termSource": "NCIT",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C49986",
                    "comments": []
                }
            }
            
        ],
        "comments": []
    },
    "parameterValues": [
        {
            "category": {
                "@id": "protease",
                "parameterName": {
                    "@id": "protease",
                    "annotationValue": "Peptidase",
                    "termSource": "MS",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C16965",
                    "comments": []
                }
            },
            "value": {
                "@id": "trypsin",
                "annotationValue": "Trypsin/P",
                "termSource": "NCI",
                "termAccession": "http://purl.obolibrary.org/obo/MS_1001313",
                "comments": []
                
            }
        },
        {
            "category": {
                "@id": "temperature",
                "parameterName": {
                    "@id": "temperature",
                    "annotationValue": "temperature",
                    "termSource": "Ontobee",
                    "termAccession": "http://purl.obolibrary.org/obo/NCRO_0000029",
                    "comments": []

                }
            },
            "value": 37,
            "unit": {
                "@id": "degree_celcius",
                "annotationValue": "degree Celsius",
                "termSource": "OM2",
                "termAccession": "http://www.ontology-of-units-of-measure.org/resource/om-2/degreeCelsius",
                "comments": []
            }
        },
        {
            "category": {
                "@id": "time",
                "parameterName": {
                    "@id": "time",
                    "annotationValue": "time",
                    "termSource": "EFO",
                    "termAccession": "http://www.ebi.ac.uk/efo/EFO_0000721",
                    "comments": []
                }
            },
            "value": 1,
            "unit": {
                "@id": "h",
                "annotationValue": "hour",
                "termSource": "UO",
                "termAccession": "http://purl.obolibrary.org/obo/UO_0000032",
                "comments": []
            }
        }
    ],
    "date": "2020-10-23",
    "performer": "TUKL",
    "previousProcess": { "@id": "#process/protein_extraction" },
    "nextProcess":  { "@id": "#process/massspec_measurement"},
    "inputs": [
        {
            "@id": "#sample/WT_protein"
        },
        {
            "@id": "#sample/MUT1_protein"
        },
        {
            "@id": "#sample/MUT2_protein"
        }
    ],
    "outputs": [
        {
            "@id": "#sample/WT_digested"
        },
        {
            "@id": "#sample/MUT1_digested"
        },
        {
            "@id": "#sample/MUT2_digested"
        }
    ],
    "comments": []

}
    """

let processLD = 

    """
    {
    "@id": "#process/standard_trypsin_digestion",
    "@type": "Process",
    "name": "standard_trypsin_digestion",
    "executesProtocol": {
        "@id": "#protocols/peptide_digestion",
        "@type": "Protocol",
        "name": "peptide_digestion",
    
        "protocolType": {
            "@id": "protein_digestion",
            "@type": "OntologyAnnotation",
            "annotationValue": "Protein Digestion",
            "termSource": "NCIT",
            "termAccession": "http://purl.obolibrary.org/obo/NCIT_C70845",
            "comments": []
        },
        "description": "The isolated proteins get solubilized. Given protease is added and the solution is heated to a given temperature. After a given amount of time, the digestion is stopped by adding a denaturation agent.",
        "uri": "http://madeUpProtocolWebsize.org/protein_digestion",
        "version": "1.0.0",
        "parameters": [
            {
                "@id": "protease",
                "@type": "ProtocolParameter",
                "parameterName": {
                    "@id": "protease",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "Peptidase",
                    "termSource": "MS",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C16965",
                    "comments": []
                }
            },
            {
                "@id": "temperature",
                "@type": "ProtocolParameter",
                "parameterName": {
                    "@id": "temperature",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "temperature",
                    "termSource": "Ontobee",
                    "termAccession": "http://purl.obolibrary.org/obo/NCRO_0000029",
                    "comments": []
    
                }
            },
            {
                "@id": "time",
                "@type": "ProtocolParameter",
                "parameterName": {
                    "@id": "time",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "time",
                    "termSource": "EFO",
                    "termAccession": "http://www.ebi.ac.uk/efo/EFO_0000721",
                    "comments": []
                }
            }
        ],
        "components": [
            {
                "@id": "#Component_digestion_stopper",
                "@type": "Component",
                "componentName": "digestion_stopper",
                "componentType": {
                    "@id": "formic_acid",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "Formic Acid",
                    "termSource": "NCIT",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C83719",
                    "comments": []
                }
            },
            {
                "@id": "#Component_heater",
                "@type": "Component",
                "componentName": "heater",
                "componentType": {
                    "@id": "heater",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "Heater Device",
                    "termSource": "NCIT",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C49986",
                    "comments": []
                }
            }
            
        ],
        "comments": []
    },
    "parameterValues": [
        {
            "@id": "#Param_Peptidase_Trypsin/P",
            "@type": "ProcessParameterValue",
            "category": {
                "@id": "protease",
                "@type": "ProtocolParameter",
                "parameterName": {
                    "@id": "protease",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "Peptidase",
                    "termSource": "MS",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C16965",
                    "comments": []
                }
            },
            "value": {
                "@id": "trypsin",
                "@type": "OntologyAnnotation",
                "annotationValue": "Trypsin/P",
                "termSource": "NCI",
                "termAccession": "http://purl.obolibrary.org/obo/MS_1001313",
                "comments": []
                
            }
        },
        {
            "@id": "#Param_temperature_37",
            "@type": "ProcessParameterValue",
            "category": {
                "@id": "temperature",
                "@type": "ProtocolParameter",
                "parameterName": {
                    "@id": "temperature",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "temperature",
                    "termSource": "Ontobee",
                    "termAccession": "http://purl.obolibrary.org/obo/NCRO_0000029",
                    "comments": []

                }
            },
            "value": 37,
            "unit": {
                "@id": "degree_celcius",
                "@type": "OntologyAnnotation",
                "annotationValue": "degree Celsius",
                "termSource": "OM2",
                "termAccession": "http://www.ontology-of-units-of-measure.org/resource/om-2/degreeCelsius",
                "comments": []
            }
        },
        {
            "@id": "#Param_time_1",
            "@type": "ProcessParameterValue",
            "category": {
                "@id": "time",
                "@type": "ProtocolParameter",
                "parameterName": {
                    "@id": "time",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "time",
                    "termSource": "EFO",
                    "termAccession": "http://www.ebi.ac.uk/efo/EFO_0000721",
                    "comments": []
                }
            },
            "value": 1,
            "unit": {
                "@id": "h",
                "@type": "OntologyAnnotation",
                "annotationValue": "hour",
                "termSource": "UO",
                "termAccession": "http://purl.obolibrary.org/obo/UO_0000032",
                "comments": []
            }
        }
    ],
    "date": "2020-10-23",
    "performer": "TUKL",
    "previousProcess": { "@id": "#process/protein_extraction", "@type": "Process" },
    "nextProcess":  { "@id": "#process/massspec_measurement", "@type": "Process"},
    "inputs": [
        {
            "@id": "#sample/WT_protein", "@type": "Source"
        },
        {
            "@id": "#sample/MUT1_protein", "@type": "Source"
        },
        {
            "@id": "#sample/MUT2_protein", "@type": "Source"
        }
    ],
    "outputs": [
        {
            "@id": "#sample/WT_digested", "@type": "Sample"
        },
        {
            "@id": "#sample/MUT1_digested", "@type": "Sample"
        },
        {
            "@id": "#sample/MUT2_digested", "@type": "Sample"
        }
    ],
    "comments": []

}
    """

let processWithoutIDs = 

    """
    {
    "name": "standard_trypsin_digestion",
    "executesProtocol": {
        "name": "peptide_digestion",
    
        "protocolType": {
            "annotationValue": "Protein Digestion",
            "termSource": "NCIT",
            "termAccession": "http://purl.obolibrary.org/obo/NCIT_C70845",
            "comments": []
        },
        "description": "The isolated proteins get solubilized. Given protease is added and the solution is heated to a given temperature. After a given amount of time, the digestion is stopped by adding a denaturation agent.",
        "uri": "http://madeUpProtocolWebsize.org/protein_digestion",
        "version": "1.0.0",
        "parameters": [
            {
                "parameterName": {
                    "annotationValue": "Peptidase",
                    "termSource": "MS",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C16965",
                    "comments": []
                }
            },
            {
                "parameterName": {
                    "annotationValue": "temperature",
                    "termSource": "Ontobee",
                    "termAccession": "http://purl.obolibrary.org/obo/NCRO_0000029",
                    "comments": []
    
                }
            },
            {
                "parameterName": {
                    "@id": "http://www.ebi.ac.uk/efo/EFO_0000721",
                    "annotationValue": "time",
                    "termSource": "EFO",
                    "termAccession": "http://www.ebi.ac.uk/efo/EFO_0000721",
                    "comments": []
                }
            }
        ],
        "components": [
            {
                "componentName": "digestion_stopper",
                "componentType": {
                    "annotationValue": "Formic Acid",
                    "termSource": "NCIT",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C83719",
                    "comments": []
                }
            },
            {
                "componentName": "heater",
                "componentType": {
                    "annotationValue": "Heater Device",
                    "termSource": "NCIT",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C49986",
                    "comments": []
                }
            }
            
        ],
        "comments": []
    },
    "parameterValues": [
        {
            "category": {
                "parameterName": {
                    "@id": "http://purl.obolibrary.org/obo/NCIT_C16965",
                    "annotationValue": "Peptidase",
                    "termSource": "MS",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C16965",
                    "comments": []
                }
            },
            "value": {
                "annotationValue": "Trypsin/P",
                "termSource": "NCI",
                "termAccession": "http://purl.obolibrary.org/obo/MS_1001313",
                "comments": []
                
            }
        },
        {
            "category": {
                "parameterName": {
                    "annotationValue": "temperature",
                    "termSource": "Ontobee",
                    "termAccession": "http://purl.obolibrary.org/obo/NCRO_0000029",
                    "comments": []

                }
            },
            "value": 37,
            "unit": {
                "annotationValue": "degree Celsius",
                "termSource": "OM2",
                "termAccession": "http://www.ontology-of-units-of-measure.org/resource/om-2/degreeCelsius",
                "comments": []
            }
        },
        {
            "category": {
                "parameterName": {
                    "annotationValue": "time",
                    "termSource": "EFO",
                    "termAccession": "http://www.ebi.ac.uk/efo/EFO_0000721",
                    "comments": []
                }
            },
            "value": 1,
            "unit": {
                "annotationValue": "hour",
                "termSource": "UO",
                "termAccession": "http://purl.obolibrary.org/obo/UO_0000032",
                "comments": []
            }
        }
    ],
    "date": "2020-10-23",
    "performer": "TUKL",
    "comments": []

}
    """

let processWithDefaultLD = 

    """
    {
    "@id": "#Process_standard_trypsin_digestion",
    "@type": "Process",
    "name": "standard_trypsin_digestion",
    "executesProtocol": {
        "@id": "http://madeUpProtocolWebsize.org/protein_digestion",
        "@type": "Protocol",
        "name": "peptide_digestion",
    
        "protocolType": {
            "@id": "http://purl.obolibrary.org/obo/NCIT_C70845",
            "@type": "OntologyAnnotation",
            "annotationValue": "Protein Digestion",
            "termSource": "NCIT",
            "termAccession": "http://purl.obolibrary.org/obo/NCIT_C70845",
            "comments": []
        },
        "description": "The isolated proteins get solubilized. Given protease is added and the solution is heated to a given temperature. After a given amount of time, the digestion is stopped by adding a denaturation agent.",
        "uri": "http://madeUpProtocolWebsize.org/protein_digestion",
        "version": "1.0.0",
        "parameters": [
            {
                "@id": "#EmptyProtocolParameter",
                "@type": "ProtocolParameter",
                "parameterName": {
                    "@id": "http://purl.obolibrary.org/obo/NCIT_C16965",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "Peptidase",
                    "termSource": "MS",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C16965",
                    "comments": []
                }
            },
            {
                "@id": "#EmptyProtocolParameter",
                "@type": "ProtocolParameter",
                "parameterName": {
                    "@id": "http://purl.obolibrary.org/obo/NCRO_0000029",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "temperature",
                    "termSource": "Ontobee",
                    "termAccession": "http://purl.obolibrary.org/obo/NCRO_0000029",
                    "comments": []
    
                }
            },
            {
                "@id": "#Param_http://www.ebi.ac.uk/efo/EFO_0000721",
                "@type": "ProtocolParameter",
                "parameterName": {
                    "@id": "http://www.ebi.ac.uk/efo/EFO_0000721",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "time",
                    "termSource": "EFO",
                    "termAccession": "http://www.ebi.ac.uk/efo/EFO_0000721",
                    "comments": []
                }
            }
        ],
        "components": [
            {
                "@id": "#Component_digestion_stopper",
                "@type": "Component",
                "componentName": "digestion_stopper",
                "componentType": {
                    "@id": "http://purl.obolibrary.org/obo/NCIT_C83719",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "Formic Acid",
                    "termSource": "NCIT",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C83719",
                    "comments": []
                }
            },
            {
                "@id": "#Component_heater",
                "@type": "Component",
                "componentName": "heater",
                "componentType": {
                    "@id": "http://purl.obolibrary.org/obo/NCIT_C49986",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "Heater Device",
                    "termSource": "NCIT",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C49986",
                    "comments": []
                }
            }
            
        ],
        "comments": []
    },
    "parameterValues": [
        {
            "@id": "#Param_Peptidase_Trypsin/P",
            "@type": "ProcessParameterValue",
            "category": {
                "@id": "#Param_http://purl.obolibrary.org/obo/NCIT_C16965",
                "@type": "ProtocolParameter",
                "parameterName": {
                    "@id": "http://purl.obolibrary.org/obo/NCIT_C16965",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "Peptidase",
                    "termSource": "MS",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C16965",
                    "comments": []
                }
            },
            "value": {
                "@id": "http://purl.obolibrary.org/obo/MS_1001313",
                "@type": "OntologyAnnotation",
                "annotationValue": "Trypsin/P",
                "termSource": "NCI",
                "termAccession": "http://purl.obolibrary.org/obo/MS_1001313",
                "comments": []
                
            }
        },
        {
            "@id": "#Param_temperature_37",
            "@type": "ProcessParameterValue",
            "category": {
                "@id": "#EmptyProtocolParameter",
                "@type": "ProtocolParameter",
                "parameterName": {
                    "@id": "http://purl.obolibrary.org/obo/NCRO_0000029",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "temperature",
                    "termSource": "Ontobee",
                    "termAccession": "http://purl.obolibrary.org/obo/NCRO_0000029",
                    "comments": []

                }
            },
            "value": 37,
            "unit": {
                "@id": "http://www.ontology-of-units-of-measure.org/resource/om-2/degreeCelsius",
                "@type": "OntologyAnnotation",
                "annotationValue": "degree Celsius",
                "termSource": "OM2",
                "termAccession": "http://www.ontology-of-units-of-measure.org/resource/om-2/degreeCelsius",
                "comments": []
            }
        },
        {
            "@id": "#Param_time_1",
            "@type": "ProcessParameterValue",
            "category": {
                "@id": "#EmptyProtocolParameter",
                "@type": "ProtocolParameter",
                "parameterName": {
                    "@id": "http://www.ebi.ac.uk/efo/EFO_0000721",
                    "@type": "OntologyAnnotation",
                    "annotationValue": "time",
                    "termSource": "EFO",
                    "termAccession": "http://www.ebi.ac.uk/efo/EFO_0000721",
                    "comments": []
                }
            },
            "value": 1,
            "unit": {
                "@id": "http://purl.obolibrary.org/obo/UO_0000032",
                "@type": "OntologyAnnotation",
                "annotationValue": "hour",
                "termSource": "UO",
                "termAccession": "http://purl.obolibrary.org/obo/UO_0000032",
                "comments": []
            }
        }
    ],
    "date": "2020-10-23",
    "performer": "TUKL",
    "comments": []

}
    """