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
    "@type": ["Process"],
    "@context": {
        "sdo": "http://schema.org/",
        "bio": "https://bioschemas.org/",

        "Process": "bio:LabProcess",

        "name": "sdo:name",
        "executesProtocol": "bio:executesProtocol",
        "parameterValues": "bio:parameterValues",
        "performer": "sdo:agent",
        "date": "sdo:endTime",
        "input": "sdo:object",
        "output": "sdo:result",

        "comments": "sdo:disambiguatingDescription"
    },
    "name": "standard_trypsin_digestion",
    "executesProtocol": {
        "@id": "#protocols/peptide_digestion",
        "@type": ["Protocol"], 
        "@context": {
            "sdo": "http://schema.org/",
            "bio": "https://bioschemas.org/",

            "Protocol": "bio:LabProtocol",

            "name": "sdo:name",
            "protocolType": "bio:intendedUse",
            "description": "sdo:description",
            "version": "sdo:version",
            "components": "bio:labEquipment",
            "uri": "sdo:url",
            "comments": "sdo:comment"
        },
        "name": "peptide_digestion",
        "protocolType": {
            "@id": "protein_digestion",
            "@type": "OntologyAnnotation",
            "@context": {
                "sdo": "http://schema.org/", 

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
        "components": [
            {
                "@id": "#Component_digestion_stopper",
                "@type": ["Component"],
                "@context": {
                    "sdo": "http://schema.org/",
        
                    "Component": "sdo:PropertyValue",

                    "category": "sdo:name",
                    "categoryCode": "sdo:propertyID",
                    "value": "sdo:value",
                    "valueCode": "sdo:valueReference",
                    "unit": "sdo:unitText",
                    "unitCode": "sdo:unitCode"
                },
                "value": "digestion_stopper",
                "category": "Formic Acid",
                "categoryCode": "http://purl.obolibrary.org/obo/NCIT_C83719"
            },
            {
                "@id": "#Component_heater",
                "@type": ["Component"],
                "@context": {
                    "sdo": "http://schema.org/",
        
                    "Component": "sdo:PropertyValue",

                    "category": "sdo:name",
                    "categoryCode": "sdo:propertyID",
                    "value": "sdo:value",
                    "valueCode": "sdo:valueReference",
                    "unit": "sdo:unitText",
                    "unitCode": "sdo:unitCode"
                },
                "value": "heater",
                "category": "Heater Device",
                "categoryCode": "http://purl.obolibrary.org/obo/NCIT_C49986"
            }
        ],
        "comments": []
    },
    "parameterValues": [
        {
            "@id": "#Param_Peptidase_Trypsin/P",
            "@type": ["ProcessParameterValue"],
            "@context": {
                "sdo": "http://schema.org/", 

                "ProcessParameterValue": "sdo:PropertyValue",

                "category": "sdo:name",
                "categoryCode": "sdo:propertyID",
                "value": "sdo:value",
                "valueCode": "sdo:valueReference",
                "unit": "sdo:unitText",
                "unitCode": "sdo:unitCode"
            },
            "category": "Peptidase",
            "categoryCode": "http://purl.obolibrary.org/obo/NCIT_C16965",
            "value": "Trypsin/P",
            "valueCode": "http://purl.obolibrary.org/obo/MS_1001313"
        },
        {
            "@id": "#Param_temperature_37",
            "@type": ["ProcessParameterValue"],
            "@context": {
                "sdo": "http://schema.org/", 

                "ProcessParameterValue": "sdo:PropertyValue",

                "category": "sdo:name",
                "categoryCode": "sdo:propertyID",
                "value": "sdo:value",
                "valueCode": "sdo:valueReference",
                "unit": "sdo:unitText",
                "unitCode": "sdo:unitCode"
            },
            "category": "temperature",
            "categoryCode": "http://purl.obolibrary.org/obo/NCRO_0000029",
            "value": 37,
            "unit": "degree Celsius",
            "unitCode": "http://www.ontology-of-units-of-measure.org/resource/om-2/degreeCelsius"
        },
        {
            "@id": "#Param_time_1",
            "@type": ["ProcessParameterValue"],
            "@context": {
                "sdo": "http://schema.org/", 

                "ProcessParameterValue": "sdo:PropertyValue",

                "category": "sdo:name",
                "categoryCode": "sdo:propertyID",
                "value": "sdo:value",
                "valueCode": "sdo:valueReference",
                "unit": "sdo:unitText",
                "unitCode": "sdo:unitCode"
            },
            "category": "time",
            "categoryCode": "http://www.ebi.ac.uk/efo/EFO_0000721",
            "value": 1,
            "unit": "hour",
            "unitCode": "http://purl.obolibrary.org/obo/UO_0000032"
        }
    ],
    "date": "2020-10-23",
    "performer": "TUKL",
    "inputs": [
        {
            "@id": "#sample/WT_protein", "@type": ["Source"],
            "@context": {
                "sdo": "http://schema.org/",
                "bio": "https://bioschemas.org/",

                "Source": "bio:Sample",

                "name": "sdo:name",
                "characteristics": "bio:additionalProperty"
            }
        },
        {
            "@id": "#sample/MUT1_protein", "@type": ["Source"],
            "@context": {
                "sdo": "http://schema.org/",
                "bio": "https://bioschemas.org/",

                "Source": "bio:Sample",

                "name": "sdo:name",
                "characteristics": "bio:additionalProperty"
            }
        },
        {
            "@id": "#sample/MUT2_protein", "@type": ["Source"],
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
            "@id": "#sample/WT_digested", "@type": ["Sample"],
            "@context": {
                "sdo": "http://schema.org/",
                "bio": "https://bioschemas.org/",

                "Sample": "bio:Sample",

                "name": "sdo:name",
                "characteristics": "bio:additionalProperty",
                "factorValues": "bio:additionalProperty"
            }
        },
        {
            "@id": "#sample/MUT1_digested", "@type": ["Sample"],
            "@context": {
                "sdo": "http://schema.org/",
                "bio": "https://bioschemas.org/",

                "Sample": "bio:Sample",

                "name": "sdo:name",
                "characteristics": "bio:additionalProperty",
                "factorValues": "bio:additionalProperty"
            }
        },
        {
            "@id": "#sample/MUT2_digested", "@type": ["Sample"],
            "@context": {
                "sdo": "http://schema.org/",
                "bio": "https://bioschemas.org/",

                "Sample": "bio:Sample",

                "name": "sdo:name",
                "characteristics": "bio:additionalProperty",
                "factorValues": "bio:additionalProperty"
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
    "@type": ["Process"],
    "@context": {
        "sdo": "http://schema.org/",
        "bio": "https://bioschemas.org/",

        "Process": "bio:LabProcess",

        "name": "sdo:name",
        "executesProtocol": "bio:executesProtocol",
        "parameterValues": "bio:parameterValues",
        "performer": "sdo:agent",
        "date": "sdo:endTime",
        "input": "sdo:object",
        "output": "sdo:result",

        "comments": "sdo:disambiguatingDescription"
    }
    "name": "standard_trypsin_digestion",
    "executesProtocol": {
        "@id": "http://madeUpProtocolWebsize.org/protein_digestion",
        "@type": ["Protocol"], 
        "@context": {
            "sdo": "http://schema.org/",
            "bio": "https://bioschemas.org/",

            "Protocol": "bio:LabProtocol",

            "name": "sdo:name",
            "protocolType": "bio:intendedUse",
            "description": "sdo:description",
            "version": "sdo:version",
            "components": "bio:labEquipment",
            "uri": "sdo:url",
            "comments": "sdo:comment"
        },
        "name": "peptide_digestion",
        "protocolType": {
            "@id": "http://purl.obolibrary.org/obo/NCIT_C70845",
            "@type": "OntologyAnnotation",
            "@context": {
                "sdo": "http://schema.org/", 

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
        "components": [
            {
                "@id": "#Component_digestion_stopper",
                "@type": ["Component"],
                "@context": {
                    "sdo": "http://schema.org/",
        
                    "Component": "sdo:PropertyValue",

                    "category": "sdo:name",
                    "categoryCode": "sdo:propertyID",
                    "value": "sdo:value",
                    "valueCode": "sdo:valueReference",
                    "unit": "sdo:unitText",
                    "unitCode": "sdo:unitCode"
                },
                "value": "digestion_stopper",
                "category": "Formic Acid",
                "categoryCode": "http://purl.obolibrary.org/obo/NCIT_C83719"
                }
            },
            {
                "@id": "#Component_heater",
                "@type": ["Component"],
                "@context": {
                    "sdo": "http://schema.org/",
        
                    "Component": "sdo:PropertyValue",

                    "category": "sdo:name",
                    "categoryCode": "sdo:propertyID",
                    "value": "sdo:value",
                    "valueCode": "sdo:valueReference",
                    "unit": "sdo:unitText",
                    "unitCode": "sdo:unitCode"
                },
                "value": "heater",
                "category": "Heater Device",
                "categoryCode": "http://purl.obolibrary.org/obo/NCIT_C49986"
                }
            }
            
        ],
        "comments": []
    },
    "parameterValues": [
        {
            "@id": "#Param_Peptidase_Trypsin/P",
            "@type": ["ProcessParameterValue"],
            "@context": {
                "sdo": "http://schema.org/", 

                "ProcessParameterValue": "sdo:PropertyValue",

                "category": "sdo:name",
                "categoryCode": "sdo:propertyID",
                "value": "sdo:value",
                "valueCode": "sdo:valueReference",
                "unit": "sdo:unitText",
                "unitCode": "sdo:unitCode"
            },
            "category": "Peptidase",
            "categoryCode": "http://purl.obolibrary.org/obo/NCIT_C16965",
            "value": "Trypsin/P",
            "valueCode": "http://purl.obolibrary.org/obo/MS_1001313"
        },
        {
            "@id": "#Param_temperature_37",
            "@type": ["ProcessParameterValue"],
            "@context": {
                "sdo": "http://schema.org/", 

                "ProcessParameterValue": "sdo:PropertyValue",

                "category": "sdo:name",
                "categoryCode": "sdo:propertyID",
                "value": "sdo:value",
                "valueCode": "sdo:valueReference",
                "unit": "sdo:unitText",
                "unitCode": "sdo:unitCode"
            },
            "category": "temperature",
            "categoryCode": "http://purl.obolibrary.org/obo/NCRO_0000029",
            "value": 37,
            "unit": "degree Celsius",
            "unitCode": "http://www.ontology-of-units-of-measure.org/resource/om-2/degreeCelsius"
        },
        {
            "@id": "#Param_time_1",
            "@type": ["ProcessParameterValue"],
            "@context": {
                "sdo": "http://schema.org/", 

                "ProcessParameterValue": "sdo:PropertyValue",

                "category": "sdo:name",
                "categoryCode": "sdo:propertyID",
                "value": "sdo:value",
                "valueCode": "sdo:valueReference",
                "unit": "sdo:unitText",
                "unitCode": "sdo:unitCode"
            },
            "category": "time",
            "categoryCode": "http://www.ebi.ac.uk/efo/EFO_0000721",
            "value": 1,
            "unit": "hour",
            "unitCode": "http://purl.obolibrary.org/obo/UO_0000032"
        }
    ],
    "date": "2020-10-23",
    "performer": "TUKL",
    "comments": []

}
    """