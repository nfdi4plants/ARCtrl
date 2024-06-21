module TestObjects.Json.Protocol

let protocol =

    """
{
    "@id": "#protocols/peptide_digestion",
    "name": "peptide_digestion",
    "protocolType": {
        "@id":"http://purl.obolibrary.org/obo/NCIT_C70845",
        "annotationValue": "Protein Digestion",
        "termSource": "NCIT",
        "termAccession": "http://purl.obolibrary.org/obo/NCIT_C70845"
    },
    "description": "The isolated proteins get solubilized. Given protease is added and the solution is heated to a given temperature. After a given amount of time, the digestion is stopped by adding a denaturation agent.",
    "uri": "http://madeUpProtocolWebsize.org/protein_digestion",
    "version": "1.0.0",
    "parameters": [
        {
            "@id": "#ProtocolParameter/http://purl.obolibrary.org/obo/NCIT_C16965",
            "parameterName": {
                "@id": "http://purl.obolibrary.org/obo/NCIT_C16965",
                "annotationValue": "Peptidase",
                "termSource": "MS",
                "termAccession": "http://purl.obolibrary.org/obo/NCIT_C16965"
            }
        },
        {
            "@id": "#ProtocolParameter/http://purl.obolibrary.org/obo/NCRO_0000029",
            "parameterName": {
                "@id": "http://purl.obolibrary.org/obo/NCRO_0000029",
                "annotationValue": "temperature",
                "termSource": "Ontobee",
                "termAccession": "http://purl.obolibrary.org/obo/NCRO_0000029"
            }
        },
        {
            "@id": "#ProtocolParameter/http://www.ebi.ac.uk/efo/EFO_0000721",
            "parameterName": {
                "@id": "http://www.ebi.ac.uk/efo/EFO_0000721",
                "annotationValue": "time",
                "termSource": "EFO",
                "termAccession": "http://www.ebi.ac.uk/efo/EFO_0000721"
            }
        }
    ],
    "components": [
        {
            "componentName": "digestion_stopper",
            "componentType": {
                "@id": "http://purl.obolibrary.org/obo/NCIT_C83719",
                "annotationValue": "Formic Acid",
                "termSource": "NCIT",
                "termAccession": "http://purl.obolibrary.org/obo/NCIT_C83719"
            }
        },
        {
            "componentName": "heater",
            "componentType": {
                "@id": "http://purl.obolibrary.org/obo/NCIT_C49986",
                "annotationValue": "Heater Device",
                "termSource": "NCIT",
                "termAccession": "http://purl.obolibrary.org/obo/NCIT_C49986"
            }
        }
        
    ]
}
    """

let protocolLD =

    """
    {
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
}
    """