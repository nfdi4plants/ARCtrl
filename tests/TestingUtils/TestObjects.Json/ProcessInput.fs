module TestObjects.Json.ProcessInput 

let sampleSimple = 
    """
        {
            "@id": "#sample/sample-P-0.1-aliquot7",
            "name": "sample-P-0.1-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture8" } ]
        }
    """
let sampleSimpleLD = 
    """
        {
            "@id": "#sample/sample-P-0.1-aliquot7",
            "@type": ["Sample","ArcSample"],
            "@context": {
                "sdo": "http://schema.org/",
                "arc": "http://purl.org/nfdi4plants/ontology/",

                "Sample": "sdo:Thing",
                "ArcSample": "arc:ARC#ARC_00000070",

                "name": "arc:name",
                "characteristics": "arc:ARC#ARC_00000080",
                "factorValues": "arc:ARC#ARC_00000083",
                "derivesFrom": "arc:ARC#ARC_00000082"
            },
            "name": "sample-P-0.1-aliquot7",
            "derivesFrom": [ { 
                "@id": "#source/source-culture8", 
                "@type": ["Source","ArcSource"],
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "Source": "sdo:Thing",
                    "ArcSource": "arc:ARC#ARC_00000071",

                    "identifier": "sdo:identifier",

                    "name": "arc:ARC#ARC_00000019",
                    "characteristics": "arc:ARC#ARC_00000080"
                } 
            } ]
        }
    """
let sampleSimpleWithoutID = 
    """
        {
            "name": "sample-P-0.1-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture8" } ]
        }
    """
let sampleSimpleWithDefaultLD = 
    """
        {
            "@id": "#Sample_sample-P-0.1-aliquot7",
            "@type": ["Sample","ArcSample"],
            "@context": {
                "sdo": "http://schema.org/",
                "arc": "http://purl.org/nfdi4plants/ontology/",

                "Sample": "sdo:Thing",
                "ArcSample": "arc:ARC#ARC_00000070",

                "name": "arc:name",
                "characteristics": "arc:ARC#ARC_00000080",
                "factorValues": "arc:ARC#ARC_00000083",
                "derivesFrom": "arc:ARC#ARC_00000082"
            },
            "name": "sample-P-0.1-aliquot7",
            "derivesFrom": [ { 
                "@id": "#source/source-culture8", 
                "@type": ["Source","ArcSource"] ,
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",

                    "Source": "sdo:Thing",
                    "ArcSource": "arc:ARC#ARC_00000071",

                    "identifier": "sdo:identifier",

                    "name": "arc:ARC#ARC_00000019",
                    "characteristics": "arc:ARC#ARC_00000080"
                }
            } ]
        }
    """

let sampleComplex = 
    """
        {
            "@id": "#sample/sample-P-0.1-aliquot7",
            "name": "sample-P-0.1-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture8" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "external"
                }
              }
            ],
            "name": "sample-P-0.1-aliquot7"
          }
    """

let source = 
    """
        { 
            "@id": "#source/source-culture8",
            "name": "source-culture8"
        }
    """
let sourceLD = 
    """
        { 
            "@id": "#source/source-culture8",
            "@type": ["Source", "ArcSource"],
            "@context": {
                "sdo": "http://schema.org/",
                "arc": "http://purl.org/nfdi4plants/ontology/",

                "Source": "sdo:Thing",
                "ArcSource": "arc:ARC#ARC_00000071",

                "identifier": "sdo:identifier",

                "name": "arc:ARC#ARC_00000019",
                "characteristics": "arc:ARC#ARC_00000080"
            },
            "name": "source-culture8"
        }
    """
let sourceWithoutID = 
    """
        { 
            "name": "source-culture8"
        }
    """
let sourceWithDefaultLD = 
    """
        { 
            "@id": "#Source_source-culture8",
            "@type": ["Source","ArcSource"],
            "@context": {
                "sdo": "http://schema.org/",
                "arc": "http://purl.org/nfdi4plants/ontology/",

                "Source": "sdo:Thing",
                "ArcSource": "arc:ARC#ARC_00000071",

                "identifier": "sdo:identifier",

                "name": "arc:ARC#ARC_00000019",
                "characteristics": "arc:ARC#ARC_00000080"
            },
            "name": "source-culture8"
        }
    """

let data = 
    """
    {
      "@id": "#data/rawspectraldatafile-JIC64_Nitrogen_0.07_External_1_3.txt",
      "comments": [],
      "name": "JIC64_Nitrogen_0.07_External_1_3.txt",
      "type": "Raw Data File"
    }
    """
let dataLD = 
    """
    {
        "@id": "#data/rawspectraldatafile-JIC64_Nitrogen_0.07_External_1_3.txt",
        "@type": ["Data","ArcData"],
        "@context": {
            "sdo": "http://schema.org/",
            "arc": "http://purl.org/nfdi4plants/ontology/",

            "Data": "sdo:MediaObject",
            "ArcData": "arc:ARC#ARC_00000076",

            "type": "arc:ARC#ARC_00000107",

            "name": "sdo:name",
            "comments": "sdo:disambiguatingDescription"
        },
        "comments": [],
        "name": "JIC64_Nitrogen_0.07_External_1_3.txt",
        "type": "Raw Data File"
    }
    """
let dataWithoutID = 
    """
    {
      "comments": [],
      "name": "JIC64_Nitrogen_0.07_External_1_3.txt",
      "type": "Raw Data File"
    }
    """
let dataWithDefaultLD = 
    """
    {
        "@id": "JIC64_Nitrogen_0.07_External_1_3.txt",
        "@type": ["Data","ArcData"],
        "@context": {
            "sdo": "http://schema.org/",
            "arc": "http://purl.org/nfdi4plants/ontology/",

            "Data": "sdo:MediaObject",
            "ArcData": "arc:ARC#ARC_00000076",

            "type": "arc:ARC#ARC_00000107",

            "name": "sdo:name",
            "comments": "sdo:disambiguatingDescription"
        },
        "comments": [],
        "name": "JIC64_Nitrogen_0.07_External_1_3.txt",
        "type": "Raw Data File"
    }
    """

let material = 
    """
    {
        "@id": "#material/extract-G-0.1-aliquot1",
        "characteristics": [],
        "name": "extract-G-0.1-aliquot1",
        "type": "Extract Name"
    }
    """

let materialLD = 
    """
    {
        "@id": "#material/extract-G-0.1-aliquot1",
        "@type": ["Material","ArcMaterial"],
        "@context": {
            "sdo": "http://schema.org/",
            "arc": "http://purl.org/nfdi4plants/ontology/",

            "ArcMaterial": "arc:ARC#ARC_00000108",
            "Material": "sdo:Thing",

            "type": "arc:ARC#ARC_00000085",
            "name": "arc:ARC#ARC_00000019",
            "characteristics": "arc:ARC#ARC_00000080",
            "derivesFrom": "arc:ARC#ARC_00000082"
        },
        "characteristics": [],
        "name": "extract-G-0.1-aliquot1",
        "type": "Extract Name"
    }
    """

let materialWithoutID = 
    """
    {
        "characteristics": [],
        "name": "extract-G-0.1-aliquot1",
        "type": "Extract Name"
    }
    """

let materialWithDefaultLD = 
    """
    {
        "@id": "#Material_extract-G-0.1-aliquot1",
        "@type": ["Material","ArcMaterial"],
        "@context": {
            "sdo": "http://schema.org/",
            "arc": "http://purl.org/nfdi4plants/ontology/",

            "ArcMaterial": "arc:ARC#ARC_00000108",
            "Material": "sdo:Thing",

            "type": "arc:ARC#ARC_00000085",
            "name": "arc:ARC#ARC_00000019",
            "characteristics": "arc:ARC#ARC_00000080",
            "derivesFrom": "arc:ARC#ARC_00000082"
        },
        "characteristics": [],
        "name": "extract-G-0.1-aliquot1",
        "type": "Extract Name"
    }
    """
