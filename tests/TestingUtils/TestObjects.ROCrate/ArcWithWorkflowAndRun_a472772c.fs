module TestObjects.ROCrate.ArcWithWorkflowAndRun

/// The crate that ARCtrl at a472772c writes for an ARC with the workflow W and the run R. The datasets of the workflow
/// and of the run list only their own CWL file and none of the files it needs.
let a472772c = """{
  "@context": [
    "https://w3id.org/ro/crate/1.2/context",
    {
      "Sample": "https://bioschemas.org/Sample",
      "LabProtocol": "https://bioschemas.org/LabProtocol",
      "LabProcess": "https://bioschemas.org/LabProcess",
      "computationalTool": "https://bioschemas.org/properties/computationalTool",
      "labEquipment": "https://bioschemas.org/properties/labEquipment",
      "reagent": "https://bioschemas.org/properties/reagent",
      "intendedUse": "https://bioschemas.org/properties/intendedUse",
      "executesLabProtocol": "https://bioschemas.org/properties/executesLabProtocol",
      "parameterValue": "https://bioschemas.org/properties/parameterValue",
      "columnIndex": "https://w3id.org/ro/terms/arc#columnIndex"
    }
  ],
  "@graph": [
    {
      "@id": "#FormalParameter_W_W_x",
      "@type": "FormalParameter",
      "additionalType": "string",
      "name": "x",
      "valueRequired": true
    },
    {
      "@id": "#FormalParameter_W_W_y",
      "@type": "FormalParameter",
      "additionalType": "string",
      "name": "y",
      "valueRequired": true
    },
    {
      "@id": "https://w3id.org/workflowhub/workflow-ro-crate#cwl",
      "@type": "ComputerLanguage",
      "name": "Common Workflow Language",
      "alternateName": "CWL",
      "identifier": {
        "@id": "https://w3id.org/cwl/v1.2/"
      },
      "url": {
        "@id": "https://www.commonwl.org/"
      }
    },
    {
      "@id": "WorkflowStep_workflows/W/workflow.cwl_s",
      "@type": "ComputationalWorkflow",
      "name": "s",
      "identifier": "tool.cwl"
    },
    {
      "@id": "#Organization_DataPLANT",
      "@type": "Organization",
      "name": "DataPLANT"
    },
    {
      "@id": "./workflows/W/workflow.cwl",
      "@type": [
        "File",
        "ComputationalWorkflow",
        "SoftwareSourceCode",
        "LabProtocol"
      ],
      "additionalType": "WorkflowProtocol",
      "input": {
        "@id": "#FormalParameter_W_W_x"
      },
      "output": {
        "@id": "#FormalParameter_W_W_y"
      },
      "name": "./workflows/W/workflow.cwl",
      "programmingLanguage": {
        "@id": "https://w3id.org/workflowhub/workflow-ro-crate#cwl"
      },
      "hasPart": {
        "@id": "WorkflowStep_workflows/W/workflow.cwl_s"
      },
      "sdPublisher": {
        "@id": "#Organization_DataPLANT"
      },
      "dateCreated": "2026-11-03T15:22:50.6890904Z"
    },
    {
      "@id": "./workflows/W/",
      "@type": "Dataset",
      "additionalType": "Workflow",
      "identifier": "W",
      "description": "A workflow of an old crate",
      "hasPart": {
        "@id": "./workflows/W/workflow.cwl"
      },
      "name": "Old Workflow",
      "mainEntity": {
        "@id": "./workflows/W/workflow.cwl"
      }
    },
    {
      "@id": "#FormalParameter_R_R_x",
      "@type": "FormalParameter",
      "additionalType": "string",
      "name": "x",
      "valueRequired": true
    },
    {
      "@id": "#FormalParameter_R_R_d",
      "@type": "FormalParameter",
      "additionalType": "Directory",
      "name": "d",
      "valueRequired": true
    },
    {
      "@id": "#FormalParameter_R_R_y",
      "@type": "FormalParameter",
      "additionalType": "string",
      "name": "y",
      "valueRequired": true
    },
    {
      "@id": "WorkflowStep_runs/R/run.cwl_main",
      "@type": "ComputationalWorkflow",
      "name": "main",
      "identifier": "../../workflows/W/workflow.cwl"
    },
    {
      "@id": "./runs/R/run.cwl",
      "@type": [
        "File",
        "ComputationalWorkflow",
        "SoftwareSourceCode",
        "LabProtocol"
      ],
      "additionalType": "WorkflowProtocol",
      "input": [
        {
          "@id": "#FormalParameter_R_R_x"
        },
        {
          "@id": "#FormalParameter_R_R_d"
        }
      ],
      "output": {
        "@id": "#FormalParameter_R_R_y"
      },
      "name": "./runs/R/run.cwl",
      "programmingLanguage": {
        "@id": "https://w3id.org/workflowhub/workflow-ro-crate#cwl"
      },
      "hasPart": {
        "@id": "WorkflowStep_runs/R/run.cwl_main"
      },
      "sdPublisher": {
        "@id": "#Organization_DataPLANT"
      },
      "dateCreated": "2026-11-03T15:22:50.6994758Z"
    },
    {
      "@id": "#WorkflowParameter_x_",
      "@type": "PropertyValue",
      "additionalType": "WorkflowInput",
      "name": "x",
      "exampleOfWork": {
        "@id": "#FormalParameter_R_R_x"
      },
      "value": "hello"
    },
    {
      "@id": "./assays/A/dataset",
      "@type": "File",
      "name": "./assays/A/dataset"
    },
    {
      "@id": "#WorkflowParameter_d_",
      "@type": "PropertyValue",
      "additionalType": "WorkflowInput",
      "name": "d",
      "exampleOfWork": {
        "@id": "#FormalParameter_R_R_d"
      },
      "value": {
        "@id": "./assays/A/dataset"
      }
    },
    {
      "@id": "#WorkflowInvocation_R",
      "@type": [
        "CreateAction",
        "LabProcess"
      ],
      "additionalType": "WorkflowInvocation",
      "name": "R",
      "object": [
        {
          "@id": "#WorkflowParameter_x_"
        },
        {
          "@id": "#WorkflowParameter_d_"
        }
      ],
      "instrument": {
        "@id": "./runs/R/run.cwl"
      },
      "executesLabProtocol": {
        "@id": "./runs/R/run.cwl"
      }
    },
    {
      "@id": "./runs/R/",
      "@type": "Dataset",
      "additionalType": "Run",
      "identifier": "R",
      "description": "A run of an old crate",
      "hasPart": {
        "@id": "./runs/R/run.cwl"
      },
      "name": "Old Run",
      "mentions": {
        "@id": "#WorkflowInvocation_R"
      },
      "about": {
        "@id": "#WorkflowInvocation_R"
      },
      "mainEntity": {
        "@id": "./runs/R/run.cwl"
      },
      "conformsTo": [
        {
          "@id": "https://w3id.org/ro/wfrun/process/0.5"
        },
        {
          "@id": "https://w3id.org/ro/wfrun/workflow/0.5"
        },
        {
          "@id": "https://w3id.org/workflowhub/workflow-ro-crate/1.0"
        }
      ]
    },
    {
      "@id": "#LICENSE",
      "@type": "CreativeWork",
      "text": "ALL RIGHTS RESERVED BY THE AUTHORS"
    },
    {
      "@id": "./",
      "@type": "Dataset",
      "additionalType": "Investigation",
      "identifier": "I",
      "datePublished": "2026-11-03T16:22:50.6784497",
      "description": "An ARC of an old crate",
      "hasPart": [
        {
          "@id": "./workflows/W/"
        },
        {
          "@id": "./runs/R/"
        }
      ],
      "name": "Old Investigation",
      "sdDatePublished": "2026-11-03T16:22:50.7030745",
      "license": {
        "@id": "#LICENSE"
      }
    },
    {
      "@id": "ro-crate-metadata.json",
      "@type": "CreativeWork",
      "conformsTo": {
        "@id": "https://w3id.org/ro/crate/1.2"
      },
      "about": {
        "@id": "./"
      }
    }
  ]
}"""
