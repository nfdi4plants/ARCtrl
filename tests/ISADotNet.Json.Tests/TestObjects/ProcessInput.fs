module TestFiles.ProcessInput 

let sampleSimple = 
    """
        {
            "@id": "#sample/sample-P-0.1-aliquot7",
            "name": "sample-P-0.1-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture8" } ]
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

let data = 
    """
    {
      "@id": "#data/rawspectraldatafile-JIC64_Nitrogen_0.07_External_1_3.txt",
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
