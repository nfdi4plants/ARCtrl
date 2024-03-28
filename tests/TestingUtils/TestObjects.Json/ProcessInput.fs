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
            "@type": ["Sample"],
            "@context": {
                "sdo": "http://schema.org/",
                "bio": "https://bioschemas.org/",

                "Sample": "bio:Sample",

                "name": "sdo:name",
                "characteristics": "bio:additionalProperty",
                "factorValues": "bio:additionalProperty"
            },
            "name": "sample-P-0.1-aliquot7"
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
            "@type": ["Sample"],
            "@context": {
                "sdo": "http://schema.org/",
                "bio": "https://bioschemas.org/",

                "Sample": "bio:Sample",

                "name": "sdo:name",
                "characteristics": "bio:additionalProperty",
                "factorValues": "bio:additionalProperty"
            },
            "name": "sample-P-0.1-aliquot7"
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
            "@type": ["Source"],
            "@context": {
                "sdo": "http://schema.org/",
                "bio": "https://bioschemas.org/",

                "Source": "bio:Sample",

                "name": "sdo:name",
                "characteristics": "bio:additionalProperty"
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
            "@type": ["Source"],
            "@context": {
                "sdo": "http://schema.org/",
                "bio": "https://bioschemas.org/",

                "Source": "bio:Sample",

                "name": "sdo:name",
                "characteristics": "bio:additionalProperty"
            },
            "name": "source-culture8"
        }
    """

let data = 
    """
    {
      "@id": "#data/rawspectraldatafile-JIC64_Nitrogen_0.07_External_1_3.txt",
      "name": "JIC64_Nitrogen_0.07_External_1_3.txt",
      "type": "Raw Data File"
    }
    """
let dataLD = 
    """
    {
        "@id": "#data/rawspectraldatafile-JIC64_Nitrogen_0.07_External_1_3.txt",
        "@type": ["Data"],
        "@context": {
            "sdo": "http://schema.org/",

            "Data": "sdo:MediaObject",

            "type": "sdo:disambiguatingDescription",
            "name": "sdo:name",
            "comments": "sdo:comment"
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
        "@type": ["Data"],
        "@context": {
            "sdo": "http://schema.org/",

            "Data": "sdo:MediaObject",

            "type": "sdo:disambiguatingDescription",
            "name": "sdo:name",
            "comments": "sdo:comment"
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
        "name": "extract-G-0.1-aliquot1",
        "type": "Extract Name"
    }
    """

let materialLD = 
    """
    {
        "@id": "#material/extract-G-0.1-aliquot1",
        "@type": ["Material"],
        "@context": {
            "sdo": "http://schema.org/",
                "bio": "https://bioschemas.org/",

                "Material": "bio:Sample",

                "name": "sdo:name",
                "type": "sdo:disambiguatingDescription",
                "characteristics": "bio:additionalProperty"
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
        "@type": ["Material"],
        "@context": {
            "sdo": "http://schema.org/",
                "bio": "https://bioschemas.org/",

                "Material": "bio:Sample",

                "name": "sdo:name",
                "type": "sdo:disambiguatingDescription",
                "characteristics": "bio:additionalProperty"
        },
        "characteristics": [],
        "name": "extract-G-0.1-aliquot1",
        "type": "Extract Name"
    }
    """
