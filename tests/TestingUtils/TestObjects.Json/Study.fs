module TestObjects.Json.Study

let studyWithIDTable =
    """
    {
        "@id": "studies/Study1/",
        "filename" : "studies/Study1/isa.study.xlsx",
        "identifier" : "Study1",
        "protocols" : [
            {
                "@id" : "http://madeUpProtocolWebsize.org/protein_digestion",
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
                            "termSource": "NCIT",
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
                ]
            }
        ],
        "materials" : {           
            "sources" : [
                {
                    "@id": "#Source_Source1",
                    "name": "Source1",
                    "characteristics" : [
                        {
                            "@id" : "#MaterialAttributeValue/organism=Arabidopsis thaliana",
                            "category" : {
                                "@id" : "#MaterialAttribute/http://purl.obolibrary.org/obo/OBI_0100026"
                            },
                            "value" : {
                                "@id": "http://purl.obolibrary.org/obo/NCBITaxon_3702",
                                "annotationValue": "Arabidopsis thaliana",
                                "termSource": "NCBITaxon",
                                "termAccession": "http://purl.obolibrary.org/obo/NCBITaxon_3702"
                            }
                        }
                    ]            
                },
                {
                    "@id": "#Source_Source2",
                    "name": "Source2",
                    "characteristics" : [{"@id" : "#MaterialAttributeValue/organism=Arabidopsis thaliana"}
                    ]
            
                }
            ],
            "samples" : [
            
                {
                    "@id": "#Sample_Sample1",
                    "name": "Sample1"           
                },
                {
                    "@id": "#Sample_Sample2",
                    "name": "Sample2"           
                }
            ]
        },
        "processSequence" : [
            {
                "@id": "#Process_Process1",
                "name": "Process1",
                "executesProtocol": {
                    "@id" : "http://madeUpProtocolWebsize.org/protein_digestion"
                },
                "parameterValues" : [
                    {
                        "category" : {"@id" : "#ProtocolParameter/http://purl.obolibrary.org/obo/NCIT_C16965"},
                        "value": {
                            "@id": "http://purl.obolibrary.org/obo/MS_1001313",
                            "annotationValue": "Trypsin/P",
                            "termSource": "MS",
                            "termAccession": "http://purl.obolibrary.org/obo/MS_1001313"
                        }
                    },
                    {
                        "category" : {"@id" : "#ProtocolParameter/http://purl.obolibrary.org/obo/NCRO_0000029"},
                        "value": 23,
                        "unit" : {"@id" : "http://www.ontology-of-units-of-measure.org/resource/om-2/degreeCelsius"}
                    },
                    {
                        "category" : {"@id" : "#ProtocolParameter/http://www.ebi.ac.uk/efo/EFO_0000721"},
                        "value": 10,
                        "unit" : {"@id" : "http://purl.obolibrary.org/obo/UO_0000032"}
                    }
                ],
                "inputs" : [
                    {"@id" : "#Source_Source1"},
                    {"@id" : "#Source_Source2"}
                ],
                "outputs" : [
                    {"@id" : "#Sample_Sample1"},
                    {"@id" : "#Sample_Sample2"}
                ]
            }

        ],
        "characteristicCategories": [
            {
                "@id" : "#MaterialAttribute/http://purl.obolibrary.org/obo/OBI_0100026",
                "characteristicType" : {
                    "@id": "http://purl.obolibrary.org/obo/OBI_0100026",
                    "annotationValue": "organism",
                    "termSource": "OBI",
                    "termAccession": "http://purl.obolibrary.org/obo/OBI_0100026"
                }
            }
        ],
        "unitCategories" : [
            {
                "@id": "http://www.ontology-of-units-of-measure.org/resource/om-2/degreeCelsius",
                "annotationValue": "degree Celsius",
                "termSource": "OM2",
                "termAccession": "http://www.ontology-of-units-of-measure.org/resource/om-2/degreeCelsius"
            },
            {
                "@id": "http://purl.obolibrary.org/obo/UO_0000032",
                "annotationValue": "hour",
                "termSource": "UO",
                "termAccession": "http://purl.obolibrary.org/obo/UO_0000032"
            }
        ]

    }
    """