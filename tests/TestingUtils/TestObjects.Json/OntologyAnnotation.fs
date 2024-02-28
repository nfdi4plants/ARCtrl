module TestObjects.Json.OntologyAnnotation
    
let peptidase = 
    """{
        "@id": "protease",
        "annotationValue": "Peptidase",
        "termSource": "MS",
        "termAccession": "http://purl.obolibrary.org/obo/NCIT_C16965",
        "comments": [
            {
                "@id": "#testid",
                "name": "comment",
                "value": "This is a comment"
            }        
        ]
    }"""
    
let peptidaseLD = 
    """{
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
        "comments": [
            {
                "@id": "#testid",
                "@type": "Comment",
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",
                    
                    "Comment": "sdo:Comment",
                    "name": "sdo:name",
                    "value": "sdo:value"
                },
                "name": "comment",
                "value": "This is a comment"
            }        
        ]
    }"""
    
let peptidaseWithoutIds = 
    """{
        "annotationValue": "Peptidase",
        "termSource": "MS",
        "termAccession": "http://purl.obolibrary.org/obo/NCIT_C16965",
        "comments": [
            {
                "name": "comment",
                "value": "This is a comment"
            }        
        ]
    }"""

let peptidaseWithDefaultLD = 
    """{
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
        "comments": [
            {
                "@id": "#Comment_comment_This_is_a_comment",
                "@type": "Comment",
                "@context": {
                    "sdo": "http://schema.org/",
                    "arc": "http://purl.org/nfdi4plants/ontology/",
                    
                    "Comment": "sdo:Comment",
                    "name": "sdo:name",
                    "value": "sdo:value"
                },
                "name": "comment",
                "value": "This is a comment"
            }        
        ]
    }"""
