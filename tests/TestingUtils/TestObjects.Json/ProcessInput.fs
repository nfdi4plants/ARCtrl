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
            "@type": "Sample",
            "name": "sample-P-0.1-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture8", "@type": "Source" } ]
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
            "@type": "Sample",
            "name": "sample-P-0.1-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture8", "@type": "Source" } ]
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
            "@type": "Source",
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
            "@type": "Source",
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
      "@type": "Data",
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
      "@type": "Data",
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
        "@type": "Material",
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
        "@type": "Material",
        "characteristics": [],
        "name": "extract-G-0.1-aliquot1",
        "type": "Extract Name"
    }
    """
