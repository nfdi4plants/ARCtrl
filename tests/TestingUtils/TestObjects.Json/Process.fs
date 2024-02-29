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
    "@type": ["Process","ArcProcess"],
    "@context": {
        "sdo": "http://schema.org/",
        "arc": "http://purl.org/nfdi4plants/ontology/",

        "Process": "sdo:Thing",
        "ArcProcess": "arc:ARC#ARC_00000048",

        "name": "arc:ARC#ARC_00000019",
        "executesProtocol": "arc:ARC#ARC_00000086",
        "performer": "arc:ARC#ARC_00000089",
        "date": "arc:ARC#ARC_00000090",
        "previousProcess": "arc:ARC#ARC_00000091",
        "nextProcess": "arc:ARC#ARC_00000092",
        "input": "arc:ARC#ARC_00000095",
        "output": "arc:ARC#ARC_00000096",

        "comments": "sdo:disambiguatingDescription"
    },
    "name": "standard_trypsin_digestion",
    "executesProtocol": {
        "@id": "#protocols/peptide_digestion",
        "@type": ["Protocol","ArcProtocol"], 
        "@context": {
            "sdo": "http://schema.org/",
            "arc": "http://purl.org/nfdi4plants/ontology/",

            "Protocol": "sdo:Thing",
            "ArcProtocol": "arc:ARC#ARC_00000040",

            "name": "arc:ARC#ARC_00000019",
            "protocolType": "arc:ARC#ARC_00000060",
            "description": "arc:ARC#ARC_00000004",
            "version": "arc:ARC#ARC_00000020",
            "components": "arc:ARC#ARC_00000064",
            "parameters": "arc:ARC#ARC_00000062",
            "uri": "arc:ARC#ARC_00000061",
            "comments": "arc:ARC#ARC_00000016"
        },
        "name": "peptide_digestion",
    
        "protocolType": {
            "@id": "protein_digestion",
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
                "@type": ["ProtocolParameter","ArcProtocolParameter"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "ProtocolParameter": "sdo:Thing",
                    "ArcProtocolParameter": "arc:ARC#ARC_00000063",

                    "parameterName": "arc:ARC#ARC_00000100"
                },
                "parameterName": {
                    "@id": "protease",
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
                    "annotationValue": "Peptidase",
                    "termSource": "MS",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C16965",
                    "comments": []
                }
            },
            {
                "@id": "temperature",
                "@type": ["ProtocolParameter","ArcProtocolParameter"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "ProtocolParameter": "sdo:Thing",
                    "ArcProtocolParameter": "arc:ARC#ARC_00000063",

                    "parameterName": "arc:ARC#ARC_00000100"
                },
                "parameterName": {
                    "@id": "temperature",
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
                    "annotationValue": "temperature",
                    "termSource": "Ontobee",
                    "termAccession": "http://purl.obolibrary.org/obo/NCRO_0000029",
                    "comments": []
    
                }
            },
            {
                "@id": "time",
                "@type": ["ProtocolParameter","ArcProtocolParameter"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "ProtocolParameter": "sdo:Thing",
                    "ArcProtocolParameter": "arc:ARC#ARC_00000063",

                    "parameterName": "arc:ARC#ARC_00000100"
                },
                "parameterName": {
                    "@id": "time",
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
                "@type": ["Component","ArcComponent"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",
                    
                    "Component": "sdo:Thing",
                    "ArcComponent": "arc:ARC#ARC_00000065",

                    "componentName": "arc:ARC#ARC_00000019",
                    "componentType": "arc:ARC#ARC_00000102"
                },
                "componentName": "digestion_stopper",
                "componentType": {
                    "@id": "formic_acid",
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
                    "annotationValue": "Formic Acid",
                    "termSource": "NCIT",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C83719",
                    "comments": []
                }
            },
            {
                "@id": "#Component_heater",
                "@type": ["Component","ArcComponent"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",
                    
                    "Component": "sdo:Thing",
                    "ArcComponent": "arc:ARC#ARC_00000065",

                    "componentName": "arc:ARC#ARC_00000019",
                    "componentType": "arc:ARC#ARC_00000102"
                },
                "componentName": "heater",
                "componentType": {
                    "@id": "heater",
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
            "@type": ["ProcessParameterValue","ArcProcessParameterValue"],
            "@context": {
                "sdo": "http://schema.org/",
                "arc": "http://purl.org/nfdi4plants/ontology/",

                "ProcessParameterValue": "sdo:PropertyValue",
                "ArcProcessParameterValue": "arc:ARC#ARC_00000088",

                "category": "arc:ARC#ARC_00000062",
                "value": "arc:ARC#ARC_00000087",
                "unit": "arc:ARC#ARC_00000106"
            },
            "category": {
                "@id": "protease",
                "@type": ["ProtocolParameter","ArcProtocolParameter"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "ProtocolParameter": "sdo:Thing",
                    "ArcProtocolParameter": "arc:ARC#ARC_00000063",

                    "parameterName": "arc:ARC#ARC_00000100"
                },
                "parameterName": {
                    "@id": "protease",
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
                    "annotationValue": "Peptidase",
                    "termSource": "MS",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C16965",
                    "comments": []
                }
            },
            "value": {
                "@id": "trypsin",
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
                "annotationValue": "Trypsin/P",
                "termSource": "NCI",
                "termAccession": "http://purl.obolibrary.org/obo/MS_1001313",
                "comments": []
                
            }
        },
        {
            "@id": "#Param_temperature_37",
            "@type": ["ProcessParameterValue","ArcProcessParameterValue"],
            "@context": {
                "sdo": "http://schema.org/",
                "arc": "http://purl.org/nfdi4plants/ontology/",

                "ProcessParameterValue": "sdo:PropertyValue",
                "ArcProcessParameterValue": "arc:ARC#ARC_00000088",

                "category": "arc:ARC#ARC_00000062",
                "value": "arc:ARC#ARC_00000087",
                "unit": "arc:ARC#ARC_00000106"
            },
            "category": {
                "@id": "temperature",
                "@type": ["ProtocolParameter","ArcProtocolParameter"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "ProtocolParameter": "sdo:Thing",
                    "ArcProtocolParameter": "arc:ARC#ARC_00000063",

                    "parameterName": "arc:ARC#ARC_00000100"
                },
                "parameterName": {
                    "@id": "temperature",
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
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "OntologyAnnotation": "sdo:DefinedTerm",
                    
                    "annotationValue": "sdo:name",
                    "termSource": "sdo:inDefinedTermSet",
                    "termAccession": "sdo:termCode",
                    "comments": "sdo:disambiguatingDescription"
                },
                "annotationValue": "degree Celsius",
                "termSource": "OM2",
                "termAccession": "http://www.ontology-of-units-of-measure.org/resource/om-2/degreeCelsius",
                "comments": []
            }
        },
        {
            "@id": "#Param_time_1",
            "@type": ["ProcessParameterValue","ArcProcessParameterValue"],
            "@context": {
                "sdo": "http://schema.org/",
                "arc": "http://purl.org/nfdi4plants/ontology/",

                "ProcessParameterValue": "sdo:PropertyValue",
                "ArcProcessParameterValue": "arc:ARC#ARC_00000088",

                "category": "arc:ARC#ARC_00000062",
                "value": "arc:ARC#ARC_00000087",
                "unit": "arc:ARC#ARC_00000106"
            },
            "category": {
                "@id": "time",
                "@type": ["ProtocolParameter","ArcProtocolParameter"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "ProtocolParameter": "sdo:Thing",
                    "ArcProtocolParameter": "arc:ARC#ARC_00000063",

                    "parameterName": "arc:ARC#ARC_00000100"
                },
                "parameterName": {
                    "@id": "time",
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
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "OntologyAnnotation": "sdo:DefinedTerm",
                    
                    "annotationValue": "sdo:name",
                    "termSource": "sdo:inDefinedTermSet",
                    "termAccession": "sdo:termCode",
                    "comments": "sdo:disambiguatingDescription"
                },
                "annotationValue": "hour",
                "termSource": "UO",
                "termAccession": "http://purl.obolibrary.org/obo/UO_0000032",
                "comments": []
            }
        }
    ],
    "date": "2020-10-23",
    "performer": "TUKL",
    "previousProcess": {
        "@id": "#process/protein_extraction",
        "@type": ["Process","ArcProcess"],
        "@context": {
            "sdo": "http://schema.org/",
            "arc": "http://purl.org/nfdi4plants/ontology/",

            "Process": "sdo:Thing",
            "ArcProcess": "arc:ARC#ARC_00000048",

            "name": "arc:ARC#ARC_00000019",
            "executesProtocol": "arc:ARC#ARC_00000086",
            "performer": "arc:ARC#ARC_00000089",
            "date": "arc:ARC#ARC_00000090",
            "previousProcess": "arc:ARC#ARC_00000091",
            "nextProcess": "arc:ARC#ARC_00000092",
            "input": "arc:ARC#ARC_00000095",
            "output": "arc:ARC#ARC_00000096",

            "comments": "sdo:disambiguatingDescription"
        }
    },
    "nextProcess":  { 
        "@id": "#process/massspec_measurement", 
        "@type": ["Process","ArcProcess"],
        "@context": {
            "sdo": "http://schema.org/",
            "arc": "http://purl.org/nfdi4plants/ontology/",

            "Process": "sdo:Thing",
            "ArcProcess": "arc:ARC#ARC_00000048",

            "name": "arc:ARC#ARC_00000019",
            "executesProtocol": "arc:ARC#ARC_00000086",
            "performer": "arc:ARC#ARC_00000089",
            "date": "arc:ARC#ARC_00000090",
            "previousProcess": "arc:ARC#ARC_00000091",
            "nextProcess": "arc:ARC#ARC_00000092",
            "input": "arc:ARC#ARC_00000095",
            "output": "arc:ARC#ARC_00000096",

            "comments": "sdo:disambiguatingDescription"
        }
    },
    "inputs": [
        {
            "@id": "#sample/WT_protein", "@type": ["Source","ArcSource"],
            "@context": {
                "sdo": "http://schema.org/",
                "arc": "http://purl.org/nfdi4plants/ontology/",

                "Source": "sdo:Thing",
                "ArcSource": "arc:ARC#ARC_00000071",

                "identifier": "sdo:identifier",

                "name": "arc:ARC#ARC_00000019",
                "characteristics": "arc:ARC#ARC_00000080"
            }
        },
        {
            "@id": "#sample/MUT1_protein", "@type": ["Source","ArcSource"],
            "@context": {
                "sdo": "http://schema.org/",
                "arc": "http://purl.org/nfdi4plants/ontology/",

                "Source": "sdo:Thing",
                "ArcSource": "arc:ARC#ARC_00000071",

                "identifier": "sdo:identifier",

                "name": "arc:ARC#ARC_00000019",
                "characteristics": "arc:ARC#ARC_00000080"
            }
        },
        {
            "@id": "#sample/MUT2_protein", "@type": ["Source","ArcSource"],
            "@context": {
                "sdo": "http://schema.org/",
                "arc": "http://purl.org/nfdi4plants/ontology/",

                "Source": "sdo:Thing",
                "ArcSource": "arc:ARC#ARC_00000071",

                "identifier": "sdo:identifier",

                "name": "arc:ARC#ARC_00000019",
                "characteristics": "arc:ARC#ARC_00000080"
            }
        }
    ],
    "outputs": [
        {
            "@id": "#sample/WT_digested", "@type": ["Sample","ArcSample"],
            "@context": {
                "sdo": "http://schema.org/",
                "arc": "http://purl.org/nfdi4plants/ontology/",

                "Sample": "sdo:Thing",
                "ArcSample": "arc:ARC#ARC_00000070",

                "name": "arc:name",
                "characteristics": "arc:ARC#ARC_00000080",
                "factorValues": "arc:ARC#ARC_00000083",
                "derivesFrom": "arc:ARC#ARC_00000082"
            }
        },
        {
            "@id": "#sample/MUT1_digested", "@type": ["Sample","ArcSample"],
            "@context": {
                "sdo": "http://schema.org/",
                "arc": "http://purl.org/nfdi4plants/ontology/",

                "Sample": "sdo:Thing",
                "ArcSample": "arc:ARC#ARC_00000070",

                "name": "arc:name",
                "characteristics": "arc:ARC#ARC_00000080",
                "factorValues": "arc:ARC#ARC_00000083",
                "derivesFrom": "arc:ARC#ARC_00000082"
            }
        },
        {
            "@id": "#sample/MUT2_digested", "@type": ["Sample","ArcSample"],
            "@context": {
                "sdo": "http://schema.org/",
                "arc": "http://purl.org/nfdi4plants/ontology/",

                "Sample": "sdo:Thing",
                "ArcSample": "arc:ARC#ARC_00000070",

                "name": "arc:name",
                "characteristics": "arc:ARC#ARC_00000080",
                "factorValues": "arc:ARC#ARC_00000083",
                "derivesFrom": "arc:ARC#ARC_00000082"
            }
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
    "@type": ["Process","ArcProcess"],
    "@context": {
        "sdo": "http://schema.org/",
        "arc": "http://purl.org/nfdi4plants/ontology/",

        "Process": "sdo:Thing",
        "ArcProcess": "arc:ARC#ARC_00000048",

        "name": "arc:ARC#ARC_00000019",
        "executesProtocol": "arc:ARC#ARC_00000086",
        "performer": "arc:ARC#ARC_00000089",
        "date": "arc:ARC#ARC_00000090",
        "previousProcess": "arc:ARC#ARC_00000091",
        "nextProcess": "arc:ARC#ARC_00000092",
        "input": "arc:ARC#ARC_00000095",
        "output": "arc:ARC#ARC_00000096",

        "comments": "sdo:disambiguatingDescription"
    },
    "name": "standard_trypsin_digestion",
    "executesProtocol": {
        "@id": "http://madeUpProtocolWebsize.org/protein_digestion",
        "@type": ["Protocol","ArcProtocol"], 
        "@context": {
            "sdo": "http://schema.org/",
            "arc": "http://purl.org/nfdi4plants/ontology/",

            "Protocol": "sdo:Thing",
            "ArcProtocol": "arc:ARC#ARC_00000040",

            "name": "arc:ARC#ARC_00000019",
            "protocolType": "arc:ARC#ARC_00000060",
            "description": "arc:ARC#ARC_00000004",
            "version": "arc:ARC#ARC_00000020",
            "components": "arc:ARC#ARC_00000064",
            "parameters": "arc:ARC#ARC_00000062",
            "uri": "arc:ARC#ARC_00000061",
            "comments": "arc:ARC#ARC_00000016"
        },
        "name": "peptide_digestion",
    
        "protocolType": {
            "@id": "http://purl.obolibrary.org/obo/NCIT_C70845",
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
                "@type": ["ProtocolParameter","ArcProtocolParameter"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "ProtocolParameter": "sdo:Thing",
                    "ArcProtocolParameter": "arc:ARC#ARC_00000063",

                    "parameterName": "arc:ARC#ARC_00000100"
                },
                "parameterName": {
                    "@id": "http://purl.obolibrary.org/obo/NCIT_C16965",
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
                    "annotationValue": "Peptidase",
                    "termSource": "MS",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C16965",
                    "comments": []
                }
            },
            {
                "@id": "#EmptyProtocolParameter",
                "@type": ["ProtocolParameter","ArcProtocolParameter"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "ProtocolParameter": "sdo:Thing",
                    "ArcProtocolParameter": "arc:ARC#ARC_00000063",

                    "parameterName": "arc:ARC#ARC_00000100"
                },
                "parameterName": {
                    "@id": "http://purl.obolibrary.org/obo/NCRO_0000029",
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
                    "annotationValue": "temperature",
                    "termSource": "Ontobee",
                    "termAccession": "http://purl.obolibrary.org/obo/NCRO_0000029",
                    "comments": []
    
                }
            },
            {
                "@id": "#Param_http://www.ebi.ac.uk/efo/EFO_0000721",
                "@type": ["ProtocolParameter","ArcProtocolParameter"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "ProtocolParameter": "sdo:Thing",
                    "ArcProtocolParameter": "arc:ARC#ARC_00000063",

                    "parameterName": "arc:ARC#ARC_00000100"
                },
                "parameterName": {
                    "@id": "http://www.ebi.ac.uk/efo/EFO_0000721",
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
                "@type": ["Component","ArcComponent"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",
                    
                    "Component": "sdo:Thing",
                    "ArcComponent": "arc:ARC#ARC_00000065",

                    "componentName": "arc:ARC#ARC_00000019",
                    "componentType": "arc:ARC#ARC_00000102"
                },
                "componentName": "digestion_stopper",
                "componentType": {
                    "@id": "http://purl.obolibrary.org/obo/NCIT_C83719",
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
                    "annotationValue": "Formic Acid",
                    "termSource": "NCIT",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C83719",
                    "comments": []
                }
            },
            {
                "@id": "#Component_heater",
                "@type": ["Component","ArcComponent"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",
                    
                    "Component": "sdo:Thing",
                    "ArcComponent": "arc:ARC#ARC_00000065",

                    "componentName": "arc:ARC#ARC_00000019",
                    "componentType": "arc:ARC#ARC_00000102"
                },
                "componentName": "heater",
                "componentType": {
                    "@id": "http://purl.obolibrary.org/obo/NCIT_C49986",
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
            "@type": ["ProcessParameterValue","ArcProcessParameterValue"],
            "@context": {
                "sdo": "http://schema.org/",
                "arc": "http://purl.org/nfdi4plants/ontology/",

                "ProcessParameterValue": "sdo:PropertyValue",
                "ArcProcessParameterValue": "arc:ARC#ARC_00000088",

                "category": "arc:ARC#ARC_00000062",
                "value": "arc:ARC#ARC_00000087",
                "unit": "arc:ARC#ARC_00000106"
            },
            "category": {
                "@id": "#Param_http://purl.obolibrary.org/obo/NCIT_C16965",
                "@type": ["ProtocolParameter","ArcProtocolParameter"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "ProtocolParameter": "sdo:Thing",
                    "ArcProtocolParameter": "arc:ARC#ARC_00000063",

                    "parameterName": "arc:ARC#ARC_00000100"
                },
                "parameterName": {
                    "@id": "http://purl.obolibrary.org/obo/NCIT_C16965",
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
                    "annotationValue": "Peptidase",
                    "termSource": "MS",
                    "termAccession": "http://purl.obolibrary.org/obo/NCIT_C16965",
                    "comments": []
                }
            },
            "value": {
                "@id": "http://purl.obolibrary.org/obo/MS_1001313",
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
                "annotationValue": "Trypsin/P",
                "termSource": "NCI",
                "termAccession": "http://purl.obolibrary.org/obo/MS_1001313",
                "comments": []
                
            }
        },
        {
            "@id": "#Param_temperature_37",
            "@type": ["ProcessParameterValue","ArcProcessParameterValue"],
            "@context": {
                "sdo": "http://schema.org/",
                "arc": "http://purl.org/nfdi4plants/ontology/",

                "ProcessParameterValue": "sdo:PropertyValue",
                "ArcProcessParameterValue": "arc:ARC#ARC_00000088",

                "category": "arc:ARC#ARC_00000062",
                "value": "arc:ARC#ARC_00000087",
                "unit": "arc:ARC#ARC_00000106"
            },
            "category": {
                "@id": "#EmptyProtocolParameter",
                "@type": ["ProtocolParameter","ArcProtocolParameter"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "ProtocolParameter": "sdo:Thing",
                    "ArcProtocolParameter": "arc:ARC#ARC_00000063",

                    "parameterName": "arc:ARC#ARC_00000100"
                },
                "parameterName": {
                    "@id": "http://purl.obolibrary.org/obo/NCRO_0000029",
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
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "OntologyAnnotation": "sdo:DefinedTerm",
                    
                    "annotationValue": "sdo:name",
                    "termSource": "sdo:inDefinedTermSet",
                    "termAccession": "sdo:termCode",
                    "comments": "sdo:disambiguatingDescription"
                },
                "annotationValue": "degree Celsius",
                "termSource": "OM2",
                "termAccession": "http://www.ontology-of-units-of-measure.org/resource/om-2/degreeCelsius",
                "comments": []
            }
        },
        {
            "@id": "#Param_time_1",
            "@type": ["ProcessParameterValue","ArcProcessParameterValue"],
            "@context": {
                "sdo": "http://schema.org/",
                "arc": "http://purl.org/nfdi4plants/ontology/",

                "ProcessParameterValue": "sdo:PropertyValue",
                "ArcProcessParameterValue": "arc:ARC#ARC_00000088",

                "category": "arc:ARC#ARC_00000062",
                "value": "arc:ARC#ARC_00000087",
                "unit": "arc:ARC#ARC_00000106"
            },
            "category": {
                "@id": "#EmptyProtocolParameter",
                "@type": ["ProtocolParameter","ArcProtocolParameter"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "ProtocolParameter": "sdo:Thing",
                    "ArcProtocolParameter": "arc:ARC#ARC_00000063",

                    "parameterName": "arc:ARC#ARC_00000100"
                },
                "parameterName": {
                    "@id": "http://www.ebi.ac.uk/efo/EFO_0000721",
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
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "OntologyAnnotation": "sdo:DefinedTerm",
                    
                    "annotationValue": "sdo:name",
                    "termSource": "sdo:inDefinedTermSet",
                    "termAccession": "sdo:termCode",
                    "comments": "sdo:disambiguatingDescription"
                },
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