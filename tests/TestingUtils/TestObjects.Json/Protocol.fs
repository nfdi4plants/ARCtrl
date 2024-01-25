module TestObjects.Json.Protocol

let protocol =

    """
    {
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

}
    """

let protocolLD =

    """
    {
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
}
    """

let protocolWithoutIds =

    """
    {
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
                "@id": "http://purl.obolibrary.org/obo/NCIT_C16965",
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
}
    """

let protocolWithDefaultLD =

    """
    {
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
}
    """