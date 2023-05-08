module TestFiles.Investigation

let investigation =

    """
    {
  "submissionDate": "2007-04-30",
  "people": [
    {
      "phone": "",
      "firstName": "Oliver",
      "address": "Oxford Road, Manchester M13 9PT, UK",
      "lastName": "Stephen",
      "midInitials": "G",
      "@id": "#person/Stephen",
      "fax": "",
      "comments": [
        {
          "value": "",
          "name": "Investigation Person REF"
        }
      ],
      "roles": [
        {
          "annotationValue": "corresponding author"
        }
      ],
      "affiliation": "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    },
    {
      "phone": "",
      "firstName": "Juan",
      "address": "Oxford Road, Manchester M13 9PT, UK",
      "lastName": "Castrillo",
      "midInitials": "I",
      "@id": "#person/Castrillo",
      "fax": "",
      "comments": [
        {
          "value": "",
          "name": "Investigation Person REF"
        }
      ],
      "roles": [
        {
          "annotationValue": "author"
        }
      ],
      "affiliation": "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    },
    {
      "phone": "",
      "firstName": "Leo",
      "address": "Oxford Road, Manchester M13 9PT, UK",
      "lastName": "Zeef",
      "midInitials": "A",
      "@id": "#person/Zeef",
      "fax": "",
      "comments": [
        {
          "value": "",
          "name": "Investigation Person REF"
        }
      ],
      "roles": [
        {
          "annotationValue": "author"
        }
      ],
      "affiliation": "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
    }
  ],
  "publications": [
    {
      "doi": "doi:10.1186/jbiol54",
      "pubMedID": "17439666",
      "status": {
        "annotationValue": "indexed in Pubmed"
      },
      "title": "Growth control of the eukaryote cell: a systems biology study in yeast.",
      "authorList": "Castrillo JI, Zeef LA, Hoyle DC, Zhang N, Hayes A, Gardner DC, Cornell MJ, Petty J, Hakes L, Wardleworth L, Rash B, Brown M, Dunn WB, Broadhurst D, O'Donoghue K, Hester SS, Dunkley TP, Hart SR, Swainston N, Li P, Gaskell SJ, Paton NW, Lilley KS, Kell DB, Oliver SG."
    }
  ],
  "description": "Background Cell growth underlies many key cellular and developmental processes, yet a limited number of studies have been carried out on cell-growth regulation. Comprehensive studies at the transcriptional, proteomic and metabolic levels under defined controlled conditions are currently lacking. Results Metabolic control analysis is being exploited in a systems biology study of the eukaryotic cell. Using chemostat culture, we have measured the impact of changes in flux (growth rate) on the transcriptome, proteome, endometabolome and exometabolome of the yeast Saccharomyces cerevisiae. Each functional genomic level shows clear growth-rate-associated trends and discriminates between carbon-sufficient and carbon-limited conditions. Genes consistently and significantly upregulated with increasing growth rate are frequently essential and encode evolutionarily conserved proteins of known function that participate in many protein-protein interactions. In contrast, more unknown, and fewer essential, genes are downregulated with increasing growth rate; their protein products rarely interact with one another. A large proportion of yeast genes under positive growth-rate control share orthologs with other eukaryotes, including humans. Significantly, transcription of genes encoding components of the TOR complex (a major controller of eukaryotic cell growth) is not subject to growth-rate regulation. Moreover, integrative studies reveal the extent and importance of post-transcriptional control, patterns of control of metabolic fluxes at the level of enzyme synthesis, and the relevance of specific enzymatic reactions in the control of metabolic fluxes during cell growth. Conclusion This work constitutes a first comprehensive systems biology study on growth-rate control in the eukaryotic cell. The results have direct implications for advanced studies on cell growth, in vivo regulation of metabolic fluxes for comprehensive metabolic engineering, and for the design of genome-scale systems biology models of the eukaryotic cell.",
  "studies": [
    {
      "submissionDate": "2007-04-30",
      "processSequence": [
        {
          "outputs": [
            { "@id": "#sample/sample-E-0.07-aliquot1" },
            { "@id": "#sample/sample-E-0.07-aliquot2" },
            { "@id": "#sample/sample-E-0.07-aliquot3" },
            { "@id": "#sample/sample-E-0.07-aliquot4" },
            { "@id": "#sample/sample-E-0.07-aliquot5" },
            { "@id": "#sample/sample-E-0.07-aliquot6" }
          ],
          "inputs": [ { "@id": "#source/source-culture13" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol13",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-N-0.2-aliquot1" },
            { "@id": "#sample/sample-N-0.2-aliquot2" },
            { "@id": "#sample/sample-N-0.2-aliquot3" },
            { "@id": "#sample/sample-N-0.2-aliquot4" },
            { "@id": "#sample/sample-N-0.2-aliquot5" },
            { "@id": "#sample/sample-N-0.2-aliquot6" },
            { "@id": "#sample/sample-N-0.2-aliquot7" },
            { "@id": "#sample/sample-N-0.2-aliquot8" },
            { "@id": "#sample/sample-N-0.2-aliquot9" },
            { "@id": "#sample/sample-N-0.2-aliquot10" },
            { "@id": "#sample/sample-N-0.2-aliquot11" }
          ],
          "inputs": [ { "@id": "#source/source-culture6" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol6",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-C-0.1-aliquot1" },
            { "@id": "#sample/sample-C-0.1-aliquot2" },
            { "@id": "#sample/sample-C-0.1-aliquot3" },
            { "@id": "#sample/sample-C-0.1-aliquot4" },
            { "@id": "#sample/sample-C-0.1-aliquot5" },
            { "@id": "#sample/sample-C-0.1-aliquot6" },
            { "@id": "#sample/sample-C-0.1-aliquot7" },
            { "@id": "#sample/sample-C-0.1-aliquot8" },
            { "@id": "#sample/sample-C-0.1-aliquot9" },
            { "@id": "#sample/sample-C-0.1-aliquot10" },
            { "@id": "#sample/sample-C-0.1-aliquot11" }
          ],
          "inputs": [ { "@id": "#source/source-culture2" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol2",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-E-0.2-aliquot1" },
            { "@id": "#sample/sample-E-0.2-aliquot2" },
            { "@id": "#sample/sample-E-0.2-aliquot3" },
            { "@id": "#sample/sample-E-0.2-aliquot4" },
            { "@id": "#sample/sample-E-0.2-aliquot5" },
            { "@id": "#sample/sample-E-0.2-aliquot6" }
          ],
          "inputs": [ { "@id": "#source/source-culture15" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol15",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-S-0.1-aliquot1" },
            { "@id": "#sample/sample-S-0.1-aliquot2" },
            { "@id": "#sample/sample-S-0.1-aliquot3" },
            { "@id": "#sample/sample-S-0.1-aliquot4" },
            { "@id": "#sample/sample-S-0.1-aliquot5" },
            { "@id": "#sample/sample-S-0.1-aliquot6" },
            { "@id": "#sample/sample-S-0.1-aliquot7" },
            { "@id": "#sample/sample-S-0.1-aliquot8" },
            { "@id": "#sample/sample-S-0.1-aliquot9" },
            { "@id": "#sample/sample-S-0.1-aliquot10" },
            { "@id": "#sample/sample-S-0.1-aliquot11" }
          ],
          "inputs": [ { "@id": "#source/source-culture11" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol11",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-N-0.07-aliquot1" },
            { "@id": "#sample/sample-N-0.07-aliquot2" },
            { "@id": "#sample/sample-N-0.07-aliquot3" },
            { "@id": "#sample/sample-N-0.07-aliquot4" },
            { "@id": "#sample/sample-N-0.07-aliquot5" },
            { "@id": "#sample/sample-N-0.07-aliquot6" },
            { "@id": "#sample/sample-N-0.07-aliquot7" },
            { "@id": "#sample/sample-N-0.07-aliquot8" },
            { "@id": "#sample/sample-N-0.07-aliquot9" },
            { "@id": "#sample/sample-N-0.07-aliquot10" }
          ],
          "inputs": [ { "@id": "#source/source-culture4" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol4",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-E-0.1-aliquot1" },
            { "@id": "#sample/sample-E-0.1-aliquot2" },
            { "@id": "#sample/sample-E-0.1-aliquot3" },
            { "@id": "#sample/sample-E-0.1-aliquot4" },
            { "@id": "#sample/sample-E-0.1-aliquot5" },
            { "@id": "#sample/sample-E-0.1-aliquot6" }
          ],
          "inputs": [ { "@id": "#source/source-culture14" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol14",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-S-0.2-aliquot1" },
            { "@id": "#sample/sample-S-0.2-aliquot2" },
            { "@id": "#sample/sample-S-0.2-aliquot3" },
            { "@id": "#sample/sample-S-0.2-aliquot4" },
            { "@id": "#sample/sample-S-0.2-aliquot5" },
            { "@id": "#sample/sample-S-0.2-aliquot6" },
            { "@id": "#sample/sample-S-0.2-aliquot7" },
            { "@id": "#sample/sample-S-0.2-aliquot8" },
            { "@id": "#sample/sample-S-0.2-aliquot9" },
            { "@id": "#sample/sample-S-0.2-aliquot10" },
            { "@id": "#sample/sample-S-0.2-aliquot11" }
          ],
          "inputs": [ { "@id": "#source/source-culture12" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol12",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-P-0.2-aliquot1" },
            { "@id": "#sample/sample-P-0.2-aliquot2" },
            { "@id": "#sample/sample-P-0.2-aliquot3" },
            { "@id": "#sample/sample-P-0.2-aliquot4" },
            { "@id": "#sample/sample-P-0.2-aliquot5" },
            { "@id": "#sample/sample-P-0.2-aliquot6" },
            { "@id": "#sample/sample-P-0.2-aliquot7" },
            { "@id": "#sample/sample-P-0.2-aliquot8" },
            { "@id": "#sample/sample-P-0.2-aliquot9" },
            { "@id": "#sample/sample-P-0.2-aliquot10" },
            { "@id": "#sample/sample-P-0.2-aliquot11" }
          ],
          "inputs": [ { "@id": "#source/source-culture9" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol9",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-S-0.07-aliquot1" },
            { "@id": "#sample/sample-S-0.07-aliquot2" },
            { "@id": "#sample/sample-S-0.07-aliquot3" },
            { "@id": "#sample/sample-S-0.07-aliquot4" },
            { "@id": "#sample/sample-S-0.07-aliquot5" },
            { "@id": "#sample/sample-S-0.07-aliquot6" },
            { "@id": "#sample/sample-S-0.07-aliquot7" },
            { "@id": "#sample/sample-S-0.07-aliquot8" },
            { "@id": "#sample/sample-S-0.07-aliquot9" },
            { "@id": "#sample/sample-S-0.07-aliquot10" }
          ],
          "inputs": [ { "@id": "#source/source-culture10" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol10",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-C-0.07-aliquot1" },
            { "@id": "#sample/sample-C-0.07-aliquot2" },
            { "@id": "#sample/sample-C-0.07-aliquot3" },
            { "@id": "#sample/sample-C-0.07-aliquot4" },
            { "@id": "#sample/sample-C-0.07-aliquot5" },
            { "@id": "#sample/sample-C-0.07-aliquot6" },
            { "@id": "#sample/sample-C-0.07-aliquot7" },
            { "@id": "#sample/sample-C-0.07-aliquot8" },
            { "@id": "#sample/sample-C-0.07-aliquot9" },
            { "@id": "#sample/sample-C-0.07-aliquot10" }
          ],
          "inputs": [ { "@id": "#source/source-culture1" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol1",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-C-0.2-aliquot1" },
            { "@id": "#sample/sample-C-0.2-aliquot2" },
            { "@id": "#sample/sample-C-0.2-aliquot3" },
            { "@id": "#sample/sample-C-0.2-aliquot4" },
            { "@id": "#sample/sample-C-0.2-aliquot5" },
            { "@id": "#sample/sample-C-0.2-aliquot6" },
            { "@id": "#sample/sample-C-0.2-aliquot7" },
            { "@id": "#sample/sample-C-0.2-aliquot8" },
            { "@id": "#sample/sample-C-0.2-aliquot9" },
            { "@id": "#sample/sample-C-0.2-aliquot10" },
            { "@id": "#sample/sample-C-0.2-aliquot11" }
          ],
          "inputs": [ { "@id": "#source/source-culture3" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol3",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-G-0.07-aliquot1" },
            { "@id": "#sample/sample-G-0.07-aliquot2" },
            { "@id": "#sample/sample-G-0.07-aliquot3" },
            { "@id": "#sample/sample-G-0.07-aliquot4" },
            { "@id": "#sample/sample-G-0.07-aliquot5" },
            { "@id": "#sample/sample-G-0.07-aliquot6" }
          ],
          "inputs": [ { "@id": "#source/source-culture16" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol16",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-G-0.2-aliquot1" },
            { "@id": "#sample/sample-G-0.2-aliquot2" },
            { "@id": "#sample/sample-G-0.2-aliquot3" },
            { "@id": "#sample/sample-G-0.2-aliquot4" },
            { "@id": "#sample/sample-G-0.2-aliquot5" },
            { "@id": "#sample/sample-G-0.2-aliquot6" }
          ],
          "inputs": [ { "@id": "#source/source-culture18" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol18",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-P-0.07-aliquot1" },
            { "@id": "#sample/sample-P-0.07-aliquot2" },
            { "@id": "#sample/sample-P-0.07-aliquot3" },
            { "@id": "#sample/sample-P-0.07-aliquot4" },
            { "@id": "#sample/sample-P-0.07-aliquot5" },
            { "@id": "#sample/sample-P-0.07-aliquot6" },
            { "@id": "#sample/sample-P-0.07-aliquot7" },
            { "@id": "#sample/sample-P-0.07-aliquot8" },
            { "@id": "#sample/sample-P-0.07-aliquot9" },
            { "@id": "#sample/sample-P-0.07-aliquot10" }
          ],
          "inputs": [ { "@id": "#source/source-culture7" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol7",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-P-0.1-aliquot1" },
            { "@id": "#sample/sample-P-0.1-aliquot2" },
            { "@id": "#sample/sample-P-0.1-aliquot3" },
            { "@id": "#sample/sample-P-0.1-aliquot4" },
            { "@id": "#sample/sample-P-0.1-aliquot5" },
            { "@id": "#sample/sample-P-0.1-aliquot6" },
            { "@id": "#sample/sample-P-0.1-aliquot7" },
            { "@id": "#sample/sample-P-0.1-aliquot8" },
            { "@id": "#sample/sample-P-0.1-aliquot9" },
            { "@id": "#sample/sample-P-0.1-aliquot10" },
            { "@id": "#sample/sample-P-0.1-aliquot11" }
          ],
          "inputs": [ { "@id": "#source/source-culture8" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol8",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-N-0.1-aliquot1" },
            { "@id": "#sample/sample-N-0.1-aliquot2" },
            { "@id": "#sample/sample-N-0.1-aliquot3" },
            { "@id": "#sample/sample-N-0.1-aliquot4" },
            { "@id": "#sample/sample-N-0.1-aliquot5" },
            { "@id": "#sample/sample-N-0.1-aliquot6" },
            { "@id": "#sample/sample-N-0.1-aliquot7" },
            { "@id": "#sample/sample-N-0.1-aliquot8" },
            { "@id": "#sample/sample-N-0.1-aliquot9" },
            { "@id": "#sample/sample-N-0.1-aliquot10" },
            { "@id": "#sample/sample-N-0.1-aliquot11" }
          ],
          "inputs": [ { "@id": "#source/source-culture5" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol5",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        },
        {
          "outputs": [
            { "@id": "#sample/sample-G-0.1-aliquot1" },
            { "@id": "#sample/sample-G-0.1-aliquot2" },
            { "@id": "#sample/sample-G-0.1-aliquot3" },
            { "@id": "#sample/sample-G-0.1-aliquot4" },
            { "@id": "#sample/sample-G-0.1-aliquot5" },
            { "@id": "#sample/sample-G-0.1-aliquot6" }
          ],
          "inputs": [ { "@id": "#source/source-culture17" } ],
          "parameterValues": [],
          "@id": "#process/growth_protocol17",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth_protocol" }
        }
      ],
      "people": [
        {
          "phone": "",
          "firstName": "Stephen",
          "address": "Oxford Road, Manchester M13 9PT, UK",
              "lastName": "Oliver",
          "midInitials": "G",
          "@id": "#person/Oliver",
          "fax": "",
          "comments": [
            {
              "value": "",
              "name": "Study Person REF"
            }
          ],
          "roles": [
            {
              "annotationValue": "corresponding author"
            }
          ],
          "affiliation": "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
        },
        {
          "phone": "",
          "firstName": "Castrillo",
          "address": "Oxford Road, Manchester M13 9PT, UK",
              "lastName": "Juan",
          "midInitials": "I",
          "@id": "#person/Juan",
          "fax": "",
          "comments": [
            {
              "value": "",
              "name": "Study Person REF"
            }
          ],
          "roles": [
            {
              "annotationValue": "author"
            }
          ],
          "affiliation": "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
        },
        {
          "phone": "",
          "firstName": "Zeef",
          "address": "Oxford Road, Manchester M13 9PT, UK",
              "lastName": "Leo",
          "midInitials": "A",
          "@id": "#person/Leo",
          "fax": "",
          "comments": [
            {
              "value": "",
              "name": "Study Person REF"
            }
          ],
          "roles": [
            {
              "annotationValue": "author"
            }
          ],
          "affiliation": "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
        }
      ],
      "comments": [
        {
          "value": "",
          "name": "Study Funding Agency"
        },
        {
          "value": "",
          "name": "Study Grant Number"
        }
      ],
      "description": "We wished to study the impact of growth rate on the total complement of mRNA molecules, proteins, and metabolites in S. cerevisiae, independent of any nutritional or other physiological effects. To achieve this, we carried out our analyses on yeast grown in steady-state chemostat culture under four different nutrient limitations (glucose, ammonium, phosphate, and sulfate) at three different dilution (that is, growth) rates (D = u = 0.07, 0.1, and 0.2/hour, equivalent to population doubling times (Td) of 10 hours, 7 hours, and 3.5 hours, respectively; u = specific growth rate defined as grams of biomass generated per gram of biomass present per unit time).",
      "unitCategories": [
        {
          "@id": "#Unit/l/hour",
          "annotationValue": "l/hour"
        }
      ],
      "studyDesignDescriptors": [
        {
          "termAccession": "http://purl.obolibrary.org/obo/OBI_0000115",
          "termSource": "OBI",
          "annotationValue": "intervention design"
        }
      ],
      "publicReleaseDate": "2009-03-10",
      "characteristicCategories": [
        {
          "@id": "#characteristic_category/genotype",
          "characteristicType": {
            "annotationValue": "genotype"
          }
        },
        {
          "@id": "#characteristic_category/strain",
          "characteristicType": {
            "annotationValue": "strain"
          }
        },
        {
          "@id": "#characteristic_category/organism",
          "characteristicType": {
            "annotationValue": "organism"
          }
        }
      ],
      "assays": [
        {
          "measurementType": {
            "termAccession": "http://purl.obolibrary.org/obo/OBI_0000615",
            "termSource": "OBI",
            "annotationValue": "protein expression profiling"
          },
          "dataFiles": [
            {
              "@id": "#data/proteinassignmentfile-proteins.csv",
              "comments": [
                {
                  "value": "8761",
                  "name": "PRIDE Accession"
                },
                {
                  "value": "8761",
                  "name": "PRIDE Processed Data Accession"
                }
              ],
              "name": "proteins.csv",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/derivedspectraldatafile-PRIDE_Exp_Complete_Ac_8763.xml",
              "comments": [
                {
                  "value": "8763",
                  "name": "PRIDE Accession"
                },
                {
                  "value": "8763",
                  "name": "PRIDE Processed Data Accession"
                }
              ],
              "name": "PRIDE_Exp_Complete_Ac_8763.xml",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-spectrum.mzdata",
              "comments": [
                {
                  "value": "8761",
                  "name": "PRIDE Accession"
                },
                {
                  "value": "8761",
                  "name": "PRIDE Processed Data Accession"
                }
              ],
              "name": "spectrum.mzdata",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/posttranslationalmodificationassignmentfile-ptms.csv",
              "comments": [
                {
                  "value": "8761",
                  "name": "PRIDE Accession"
                },
                {
                  "value": "8761",
                  "name": "PRIDE Processed Data Accession"
                }
              ],
              "name": "ptms.csv",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/derivedspectraldatafile-PRIDE_Exp_Complete_Ac_8762.xml",
              "comments": [
                {
                  "value": "8762",
                  "name": "PRIDE Accession"
                },
                {
                  "value": "8762",
                  "name": "PRIDE Processed Data Accession"
                }
              ],
              "name": "PRIDE_Exp_Complete_Ac_8762.xml",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/derivedspectraldatafile-PRIDE_Exp_Complete_Ac_8761.xml",
              "comments": [
                {
                  "value": "8761",
                  "name": "PRIDE Accession"
                },
                {
                  "value": "8761",
                  "name": "PRIDE Processed Data Accession"
                }
              ],
              "name": "PRIDE_Exp_Complete_Ac_8761.xml",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/peptideassignmentfile-peptides.csv",
              "comments": [
                {
                  "value": "8761",
                  "name": "PRIDE Accession"
                },
                {
                  "value": "8761",
                  "name": "PRIDE Processed Data Accession"
                }
              ],
              "name": "peptides.csv",
              "type": "Raw Data File"
            }
          ],
          "technologyType": {
            "termSource": "OBI",
            "annotationValue": "mass spectrometry"
          },
          "processSequence": [
            {
              "outputs": [
                { "@id": "#material/labeledextract-JC_C-0.2" },
                { "@id": "#material/labeledextract-Pool2" }
              ],
              "inputs": [ { "@id": "#material/extract-C-0.2" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/8762" },
              "@id": "#process/ITRAQ_labeling4",

              "comments": [],
              "previousProcess": { "@id": "#process/protein_extraction4" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/ITRAQ_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/derivedspectraldatafile-PRIDE_Exp_Complete_Ac_8761.xml" } ],
              "name": "datatransformation1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/datatransformation1",
              "parameterValues": [],
              "inputs": [
                { "@id": "#data/proteinassignmentfile-proteins.csv" },
                { "@id": "#data/posttranslationalmodificationassignmentfile-ptms.csv" },
                { "@id": "#data/peptideassignmentfile-peptides.csv" }
              ],
              "comments": [],
              "previousProcess": { "@id": "#process/norm1" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/derivedspectraldatafile-PRIDE_Exp_Complete_Ac_8762.xml" } ],
              "name": "datatransformation2",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/datatransformation2",
              "parameterValues": [],
              "inputs": [
                { "@id": "#data/proteinassignmentfile-proteins.csv" },
                { "@id": "#data/posttranslationalmodificationassignmentfile-ptms.csv" },
                { "@id": "#data/peptideassignmentfile-peptides.csv" }
              ],
              "comments": [],
              "previousProcess": { "@id": "#process/norm2" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.1" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.1-aliquot11" } ],
              "parameterValues": [],
              "@id": "#process/protein_extraction2",

              "comments": [],
              "nextProcess": { "@id": "#process/ITRAQ_labeling2" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/protein_extraction" }
            },
            {
              "outputs": [
                { "@id": "#data/proteinassignmentfile-proteins.csv" },
                { "@id": "#data/posttranslationalmodificationassignmentfile-ptms.csv" },
                { "@id": "#data/peptideassignmentfile-peptides.csv" }
              ],
              "name": "norm1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/datatransformation1" },
              "@id": "#process/norm1",
              "parameterValues": [],
              "inputs": [ { "@id": "#data/rawspectraldatafile-spectrum.mzdata" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/8761" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-spectrum.mzdata" } ],
              "name": "8762",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/norm2" },
              "@id": "#process/8762",
              "parameterValues": [],
              "inputs": [
                { "@id": "#material/labeledextract-JC_C-0.2" },
                { "@id": "#material/labeledextract-JC_N-0.2" },
                { "@id": "#material/labeledextract-JC_P-0.1" },
                { "@id": "#material/labeledextract-Pool2" }
              ],
              "comments": [],
              "previousProcess": { "@id": "#process/ITRAQ_labeling6" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.2" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.2-aliquot11" } ],
              "parameterValues": [],
              "@id": "#process/protein_extraction7",

              "comments": [],
              "nextProcess": { "@id": "#process/ITRAQ_labeling7" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/protein_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.1" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.1-aliquot11" } ],
              "parameterValues": [],
              "@id": "#process/protein_extraction3",

              "comments": [],
              "nextProcess": { "@id": "#process/ITRAQ_labeling3" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/protein_extraction" }
            },
            {
              "outputs": [
                { "@id": "#data/proteinassignmentfile-proteins.csv" },
                { "@id": "#data/posttranslationalmodificationassignmentfile-ptms.csv" },
                { "@id": "#data/peptideassignmentfile-peptides.csv" }
              ],
              "name": "norm3",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/datatransformation3" },
              "@id": "#process/norm3",
              "parameterValues": [],
              "inputs": [ { "@id": "#data/rawspectraldatafile-spectrum.mzdata" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/8763" },
              "performer": ""
            },
            {
              "outputs": [
                { "@id": "#material/labeledextract-JC_P-0.2" },
                { "@id": "#material/labeledextract-Pool3" }
              ],
              "inputs": [ { "@id": "#material/extract-P-0.2" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/8763" },
              "@id": "#process/ITRAQ_labeling7",

              "comments": [],
              "previousProcess": { "@id": "#process/protein_extraction7" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/ITRAQ_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.1" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.1-aliquot11" } ],
              "parameterValues": [],
              "@id": "#process/protein_extraction1",

              "comments": [],
              "nextProcess": { "@id": "#process/ITRAQ_labeling1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/protein_extraction" }
            },
            {
              "outputs": [
                { "@id": "#data/proteinassignmentfile-proteins.csv" },
                { "@id": "#data/posttranslationalmodificationassignmentfile-ptms.csv" },
                { "@id": "#data/peptideassignmentfile-peptides.csv" }
              ],
              "name": "norm2",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/datatransformation2" },
              "@id": "#process/norm2",
              "parameterValues": [],
              "inputs": [ { "@id": "#data/rawspectraldatafile-spectrum.mzdata" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/8762" },
              "performer": ""
            },
            {
              "outputs": [
                { "@id": "#material/labeledextract-JC_N-0.1" },
                { "@id": "#material/labeledextract-Pool1" }
              ],
              "inputs": [ { "@id": "#material/extract-N-0.1" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/8761" },
              "@id": "#process/ITRAQ_labeling3",

              "comments": [],
              "previousProcess": { "@id": "#process/protein_extraction3" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/ITRAQ_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.2" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.2-aliquot11" } ],
              "parameterValues": [],
              "@id": "#process/protein_extraction8",

              "comments": [],
              "nextProcess": { "@id": "#process/ITRAQ_labeling8" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/protein_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.1" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.1-aliquot11" } ],
              "parameterValues": [],
              "@id": "#process/protein_extraction6",

              "comments": [],
              "nextProcess": { "@id": "#process/ITRAQ_labeling6" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/protein_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-spectrum.mzdata" } ],
              "name": "8763",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/norm3" },
              "@id": "#process/8763",
              "parameterValues": [],
              "inputs": [
                { "@id": "#material/labeledextract-JC_P-0.2" },
                { "@id": "#material/labeledextract-JC_S-0.2" },
                { "@id": "#material/labeledextract-Pool3" }
              ],
              "comments": [],
              "previousProcess": { "@id": "#process/ITRAQ_labeling8" },
              "performer": ""
            },
            {
              "outputs": [
                { "@id": "#material/labeledextract-JC_S-0.1" },
                { "@id": "#material/labeledextract-Pool1" }
              ],
              "inputs": [ { "@id": "#material/extract-S-0.1" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/8761" },
              "@id": "#process/ITRAQ_labeling1",

              "comments": [],
              "previousProcess": { "@id": "#process/protein_extraction1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/ITRAQ_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.2" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.2-aliquot11" } ],
              "parameterValues": [],
              "@id": "#process/protein_extraction5",

              "comments": [],
              "nextProcess": { "@id": "#process/ITRAQ_labeling5" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/protein_extraction" }
            },
            {
              "outputs": [
                { "@id": "#material/labeledextract-JC_C-0.1" },
                { "@id": "#material/labeledextract-Pool1" }
              ],
              "inputs": [ { "@id": "#material/extract-C-0.1" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/8761" },
              "@id": "#process/ITRAQ_labeling2",

              "comments": [],
              "previousProcess": { "@id": "#process/protein_extraction2" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/ITRAQ_labeling" }
            },
            {
              "outputs": [
                { "@id": "#material/labeledextract-JC_S-0.2" },
                { "@id": "#material/labeledextract-Pool3" }
              ],
              "inputs": [ { "@id": "#material/extract-S-0.2" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/8763" },
              "@id": "#process/ITRAQ_labeling8",

              "comments": [],
              "previousProcess": { "@id": "#process/protein_extraction8" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/ITRAQ_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-spectrum.mzdata" } ],
              "name": "8761",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/norm1" },
              "@id": "#process/8761",
              "parameterValues": [],
              "inputs": [
                { "@id": "#material/labeledextract-JC_S-0.1" },
                { "@id": "#material/labeledextract-JC_C-0.1" },
                { "@id": "#material/labeledextract-JC_N-0.1" },
                { "@id": "#material/labeledextract-Pool1" }
              ],
              "comments": [],
              "previousProcess": { "@id": "#process/ITRAQ_labeling3" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.2" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.2-aliquot11" } ],
              "parameterValues": [],
              "@id": "#process/protein_extraction4",

              "comments": [],
              "nextProcess": { "@id": "#process/ITRAQ_labeling4" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/protein_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/derivedspectraldatafile-PRIDE_Exp_Complete_Ac_8763.xml" } ],
              "name": "datatransformation3",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/datatransformation3",
              "parameterValues": [],
              "inputs": [
                { "@id": "#data/proteinassignmentfile-proteins.csv" },
                { "@id": "#data/posttranslationalmodificationassignmentfile-ptms.csv" },
                { "@id": "#data/peptideassignmentfile-peptides.csv" }
              ],
              "comments": [],
              "previousProcess": { "@id": "#process/norm3" },
              "performer": ""
            },
            {
              "outputs": [
                { "@id": "#material/labeledextract-JC_N-0.2" },
                { "@id": "#material/labeledextract-Pool2" }
              ],
              "inputs": [ { "@id": "#material/extract-N-0.2" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/8762" },
              "@id": "#process/ITRAQ_labeling5",

              "comments": [],
              "previousProcess": { "@id": "#process/protein_extraction5" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/ITRAQ_labeling" }
            },
            {
              "outputs": [
                { "@id": "#material/labeledextract-JC_P-0.1" },
                { "@id": "#material/labeledextract-Pool2" }
              ],
              "inputs": [ { "@id": "#material/extract-P-0.1" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/8762" },
              "@id": "#process/ITRAQ_labeling6",

              "comments": [],
              "previousProcess": { "@id": "#process/protein_extraction6" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/ITRAQ_labeling" }
            }
          ],
          "@id": "#assay/a_proteome.txt",
          "unitCategories": [
            {
              "@id": "#Unit/l/hr",
              "annotationValue": "l/hr"
            }
          ],
          "characteristicCategories": [
            {
              "@id": "#characteristic_category/Label",
              "characteristicType": {
                "annotationValue": "Label"
              }
            }
          ],
          "materials": {
            "otherMaterials": [
              {
                "@id": "#material/labeledextract-JC_P-0.1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "iTRAQ reagent 115"
                    }
                  }
                ],
                "name": "labeledextract-JC_P-0.1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-S-0.1",
                "characteristics": [],
                "name": "extract-S-0.1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-JC_N-0.1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "iTRAQ reagent 115"
                    }
                  }
                ],
                "name": "labeledextract-JC_N-0.1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-P-0.1",
                "characteristics": [],
                "name": "extract-P-0.1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-JC_S-0.2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "iTRAQ reagent 115"
                    }
                  }
                ],
                "name": "labeledextract-JC_S-0.2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-JC_N-0.2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "iTRAQ reagent 116"
                    }
                  }
                ],
                "name": "labeledextract-JC_N-0.2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-JC_S-0.1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "iTRAQ reagent 117"
                    }
                  }
                ],
                "name": "labeledextract-JC_S-0.1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-S-0.2",
                "characteristics": [],
                "name": "extract-S-0.2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.1",
                "characteristics": [],
                "name": "extract-C-0.1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.2",
                "characteristics": [],
                "name": "extract-C-0.2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-JC_C-0.1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "iTRAQ reagent 116"
                    }
                  }
                ],
                "name": "labeledextract-JC_C-0.1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-N-0.1",
                "characteristics": [],
                "name": "extract-N-0.1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-Pool1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "iTRAQ reagent 114"
                    }
                  }
                ],
                "name": "labeledextract-Pool1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-N-0.2",
                "characteristics": [],
                "name": "extract-N-0.2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-JC_P-0.2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "iTRAQ reagent 116"
                    }
                  }
                ],
                "name": "labeledextract-JC_P-0.2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-Pool2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "iTRAQ reagent 114"
                    }
                  }
                ],
                "name": "labeledextract-Pool2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-Pool3",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "iTRAQ reagent 117"
                    }
                  }
                ],
                "name": "labeledextract-Pool3",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-JC_C-0.2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "iTRAQ reagent 117"
                    }
                  }
                ],
                "name": "labeledextract-JC_C-0.2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-P-0.2",
                "characteristics": [],
                "name": "extract-P-0.2",
                "type": "Extract Name"
              }
            ],
            "samples": [
              { "@id": "#sample/sample-S-0.1-aliquot11" },
              { "@id": "#sample/sample-N-0.1-aliquot11" },
              { "@id": "#sample/sample-C-0.1-aliquot11" },
              { "@id": "#sample/sample-P-0.2-aliquot11" },
              { "@id": "#sample/sample-N-0.2-aliquot11" },
              { "@id": "#sample/sample-P-0.1-aliquot11" },
              { "@id": "#sample/sample-C-0.2-aliquot11" },
              { "@id": "#sample/sample-S-0.2-aliquot11" }
            ]
          },
          "technologyPlatform": "iTRAQ",
          "filename": "a_proteome.txt"
        },
        {
          "measurementType": {
            "termAccession": "http://purl.obolibrary.org/obo/OBI_0000366",
            "termSource": "OBI",
            "annotationValue": "metabolite profiling"
          },
          "dataFiles": [
            {
              "@id": "#data/rawspectraldatafile-JIC64_Nitrogen_0.07_External_1_3.txt",
              "comments": [],
              "name": "JIC64_Nitrogen_0.07_External_1_3.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC103_GlucoseO2_0.10_External_1_1.txt",
              "comments": [],
              "name": "JIC103_GlucoseO2_0.10_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC8_Carbon_0.20_Internal_2_1.txt",
              "comments": [],
              "name": "JIC8_Carbon_0.20_Internal_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC55_Carbon_0.07_External_1_1.txt",
              "comments": [],
              "name": "JIC55_Carbon_0.07_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC1_Carbon_0.07_Internal_1_1.txt",
              "comments": [],
              "name": "JIC1_Carbon_0.07_Internal_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC16_Nitrogen_0.20_Internal_1_1.txt",
              "comments": [],
              "name": "JIC16_Nitrogen_0.20_Internal_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC60_Carbon_0.10_External_3_1.txt",
              "comments": [],
              "name": "JIC60_Carbon_0.10_External_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC26_Phosphate_0.20_Internal_2_1.txt",
              "comments": [],
              "name": "JIC26_Phosphate_0.20_Internal_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC47_GlucoseO2_0.07_Internal_2_1.txt",
              "comments": [],
              "name": "JIC47_GlucoseO2_0.07_Internal_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC24_Phosphate_0.10_Internal_3_1.txt",
              "comments": [],
              "name": "JIC24_Phosphate_0.10_Internal_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC4_Carbon_0.10_Internal_1_1.txt",
              "comments": [],
              "name": "JIC4_Carbon_0.10_Internal_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC85_Sulphate_0.10_External_1_1.txt",
              "comments": [],
              "name": "JIC85_Sulphate_0.10_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC36_Sulphate_0.20_Internal_1_3.txt",
              "comments": [],
              "name": "JIC36_Sulphate_0.20_Internal_1_3.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC11_Nitrogen_0.07_Internal_2_1.txt",
              "comments": [],
              "name": "JIC11_Nitrogen_0.07_Internal_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC86_Sulphate_0.10_External_2_1.txt",
              "comments": [],
              "name": "JIC86_Sulphate_0.10_External_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC82_Sulphate_0.07_External_1_1.txt",
              "comments": [],
              "name": "JIC82_Sulphate_0.07_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC59_Carbon_0.10_External_2_1.txt",
              "comments": [],
              "name": "JIC59_Carbon_0.10_External_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC42_Ethanol_0.10_Internal_3_1.txt",
              "comments": [],
              "name": "JIC42_Ethanol_0.10_Internal_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC94_Ethanol_0.10_External_1_1.txt",
              "comments": [],
              "name": "JIC94_Ethanol_0.10_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC100_GlucoseO2_0.07_External_1_2.txt",
              "comments": [],
              "name": "JIC100_GlucoseO2_0.07_External_1_2.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC56_Carbon_0.07_External_2_1.txt",
              "comments": [],
              "name": "JIC56_Carbon_0.07_External_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC81_Phosphate_0.20_External_3_1.txt",
              "comments": [],
              "name": "JIC81_Phosphate_0.20_External_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC80_Phosphate_0.20_External_2_1.txt",
              "comments": [],
              "name": "JIC80_Phosphate_0.20_External_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC55_Carbon_0.07_External_1_3.txt",
              "comments": [],
              "name": "JIC55_Carbon_0.07_External_1_3.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC13_Nitrogen_0.10_Internal_1_1.txt",
              "comments": [],
              "name": "JIC13_Nitrogen_0.10_Internal_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC37_Ethanol_0.07_Internal_1_1.txt",
              "comments": [],
              "name": "JIC37_Ethanol_0.07_Internal_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC38_Ethanol_0.07_Internal_2_1.txt",
              "comments": [],
              "name": "JIC38_Ethanol_0.07_Internal_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC92_Ethanol_0.07_External_2_1.txt",
              "comments": [],
              "name": "JIC92_Ethanol_0.07_External_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC104_GlucoseO2_0.10_External_2_1.txt",
              "comments": [],
              "name": "JIC104_GlucoseO2_0.10_External_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC22_Phosphate_0.10_Internal_1_1.txt",
              "comments": [],
              "name": "JIC22_Phosphate_0.10_Internal_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC95_Ethanol_0.10_External_2_1.txt",
              "comments": [],
              "name": "JIC95_Ethanol_0.10_External_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC73_Phosphate_0.07_External_1_3.txt",
              "comments": [],
              "name": "JIC73_Phosphate_0.07_External_1_3.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC12_Nitrogen_0.07_Internal_3_1.txt",
              "comments": [],
              "name": "JIC12_Nitrogen_0.07_Internal_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC52_GlucoseO2_0.20_Internal_1_1.txt",
              "comments": [],
              "name": "JIC52_GlucoseO2_0.20_Internal_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC78_Phosphate_0.10_External_3_1.txt",
              "comments": [],
              "name": "JIC78_Phosphate_0.10_External_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC23_Phosphate_0.10_Internal_2_1.txt",
              "comments": [],
              "name": "JIC23_Phosphate_0.10_Internal_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC62_Carbon_0.20_External_2_1.txt",
              "comments": [],
              "name": "JIC62_Carbon_0.20_External_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC40_Ethanol_0.10_Internal_1_1.txt",
              "comments": [],
              "name": "JIC40_Ethanol_0.10_Internal_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC57_Carbon_0.07_External_3_1.txt",
              "comments": [],
              "name": "JIC57_Carbon_0.07_External_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC79_Phosphate_0.20_External_1_1.txt",
              "comments": [],
              "name": "JIC79_Phosphate_0.20_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC88_Sulphate_0.20_External_1_1.txt",
              "comments": [],
              "name": "JIC88_Sulphate_0.20_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC46_GlucoseO2_0.07_Internal_1_2.txt",
              "comments": [],
              "name": "JIC46_GlucoseO2_0.07_Internal_1_2.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC63_Carbon_0.20_External_3_1.txt",
              "comments": [],
              "name": "JIC63_Carbon_0.20_External_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC93_Ethanol_0.07_External_3_1.txt",
              "comments": [],
              "name": "JIC93_Ethanol_0.07_External_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC3_Carbon_0.07_Internal_3_1.txt",
              "comments": [],
              "name": "JIC3_Carbon_0.07_Internal_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC91_Ethanol_0.07_External_1_3.txt",
              "comments": [],
              "name": "JIC91_Ethanol_0.07_External_1_3.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC37_Ethanol_0.07_Internal_1_3.txt",
              "comments": [],
              "name": "JIC37_Ethanol_0.07_Internal_1_3.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC15_Nitrogen_0.10_Internal_3_1.txt",
              "comments": [],
              "name": "JIC15_Nitrogen_0.10_Internal_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC65_Nitrogen_0.07_External_2_1.txt",
              "comments": [],
              "name": "JIC65_Nitrogen_0.07_External_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC74_Phosphate_0.07_External_2_1.txt",
              "comments": [],
              "name": "JIC74_Phosphate_0.07_External_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC5_Carbon_0.10_Internal_2_1.txt",
              "comments": [],
              "name": "JIC5_Carbon_0.10_Internal_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC10_Nitrogen_0.07_Internal_1_1.txt",
              "comments": [],
              "name": "JIC10_Nitrogen_0.07_Internal_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC90_Sulphate_0.20_External_3_1.txt",
              "comments": [],
              "name": "JIC90_Sulphate_0.20_External_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-/Users/eamonnmaguire/Dropbox/Presentations",
              "comments": [],
              "name": "/Users/eamonnmaguire/Dropbox/Presentations",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC50_GlucoseO2_0.10_Internal_2_1.txt",
              "comments": [],
              "name": "JIC50_GlucoseO2_0.10_Internal_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC10_Nitrogen_0.07_Internal_1_3.txt",
              "comments": [],
              "name": "JIC10_Nitrogen_0.07_Internal_1_3.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC18_Nitrogen_0.20_Internal_3_1.txt",
              "comments": [],
              "name": "JIC18_Nitrogen_0.20_Internal_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC27_Phosphate_0.20_Internal_3_1.txt",
              "comments": [],
              "name": "JIC27_Phosphate_0.20_Internal_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC2_Carbon_0.07_Internal_2_1.txt",
              "comments": [],
              "name": "JIC2_Carbon_0.07_Internal_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC58_Carbon_0.10_External_1_1.txt",
              "comments": [],
              "name": "JIC58_Carbon_0.10_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC17_Nitrogen_0.20_Internal_2_1.txt",
              "comments": [],
              "name": "JIC17_Nitrogen_0.20_Internal_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC106_GlucoseO2_0.20_External_1_1.txt",
              "comments": [],
              "name": "JIC106_GlucoseO2_0.20_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC72_Nitrogen_0.20_External_3_1.txt",
              "comments": [],
              "name": "JIC72_Nitrogen_0.20_External_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC77_Phosphate_0.10_External_2_1.txt",
              "comments": [],
              "name": "JIC77_Phosphate_0.10_External_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC107_GlucoseO2_0.20_External_2_1.txt",
              "comments": [],
              "name": "JIC107_GlucoseO2_0.20_External_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC53_GlucoseO2_0.20_Internal_2_1.txt",
              "comments": [],
              "name": "JIC53_GlucoseO2_0.20_Internal_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC105_GlucoseO2_0.10_External_3_1.txt",
              "comments": [],
              "name": "JIC105_GlucoseO2_0.10_External_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC75_Phosphate_0.07_External_3_1.txt",
              "comments": [],
              "name": "JIC75_Phosphate_0.07_External_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC89_Sulphate_0.20_External_2_1.txt",
              "comments": [],
              "name": "JIC89_Sulphate_0.20_External_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC41_Ethanol_0.10_Internal_2_1.txt",
              "comments": [],
              "name": "JIC41_Ethanol_0.10_Internal_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC51_GlucoseO2_0.10_Internal_3_1.txt",
              "comments": [],
              "name": "JIC51_GlucoseO2_0.10_Internal_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC21_Phosphate_0.07_Internal_2_1.txt",
              "comments": [],
              "name": "JIC21_Phosphate_0.07_Internal_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC55_Carbon_0.07_External_1_2.txt",
              "comments": [],
              "name": "JIC55_Carbon_0.07_External_1_2.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC54_GlucoseO2_0.20_Internal_3_1.txt",
              "comments": [],
              "name": "JIC54_GlucoseO2_0.20_Internal_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC48_GlucoseO2_0.07_Internal_3_1.txt",
              "comments": [],
              "name": "JIC48_GlucoseO2_0.07_Internal_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC1_Carbon_0.07_Internal_1_3.txt",
              "comments": [],
              "name": "JIC1_Carbon_0.07_Internal_1_3.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC73_Phosphate_0.07_External_1_1.txt",
              "comments": [],
              "name": "JIC73_Phosphate_0.07_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC101_GlucoseO2_0.07_External_2_1.txt",
              "comments": [],
              "name": "JIC101_GlucoseO2_0.07_External_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC91_Ethanol_0.07_External_1_1.txt",
              "comments": [],
              "name": "JIC91_Ethanol_0.07_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC76_Phosphate_0.10_External_1_1.txt",
              "comments": [],
              "name": "JIC76_Phosphate_0.10_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC6_Carbon_0.10_Internal_3_1.txt",
              "comments": [],
              "name": "JIC6_Carbon_0.10_Internal_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC46_GlucoseO2_0.07_Internal_1_3.txt",
              "comments": [],
              "name": "JIC46_GlucoseO2_0.07_Internal_1_3.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC14_Nitrogen_0.10_Internal_2_1.txt",
              "comments": [],
              "name": "JIC14_Nitrogen_0.10_Internal_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC67_Nitrogen_0.10_External_1_1.txt",
              "comments": [],
              "name": "JIC67_Nitrogen_0.10_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-/Users/eamonnmaguire/Downloads/sample-data",
              "comments": [],
              "name": "/Users/eamonnmaguire/Downloads/sample-data",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC69_Nitrogen_0.10_External_3_1.txt",
              "comments": [],
              "name": "JIC69_Nitrogen_0.10_External_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC25_Phosphate_0.20_Internal_1_1.txt",
              "comments": [],
              "name": "JIC25_Phosphate_0.20_Internal_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC39_Ethanol_0.07_Internal_3_1.txt",
              "comments": [],
              "name": "JIC39_Ethanol_0.07_Internal_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC66_Nitrogen_0.07_External_3_1.txt",
              "comments": [],
              "name": "JIC66_Nitrogen_0.07_External_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC64_Nitrogen_0.07_External_1_1.txt",
              "comments": [],
              "name": "JIC64_Nitrogen_0.07_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC10_Nitrogen_0.07_Internal_1_2.txt",
              "comments": [],
              "name": "JIC10_Nitrogen_0.07_Internal_1_2.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC108_GlucoseO2_0.20_External_3_1.txt",
              "comments": [],
              "name": "JIC108_GlucoseO2_0.20_External_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC7_Carbon_0.20_Internal_1_1.txt",
              "comments": [],
              "name": "JIC7_Carbon_0.20_Internal_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-/Users/eamonnmaguire/Dropbox/Presentation_Images",
              "comments": [],
              "name": "/Users/eamonnmaguire/Dropbox/Presentation Images",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC70_Nitrogen_0.20_External_1_1.txt",
              "comments": [],
              "name": "JIC70_Nitrogen_0.20_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-/Users/eamonnmaguire/Dropbox/ISAtab",
              "comments": [],
              "name": "/Users/eamonnmaguire/Dropbox/ISAtab",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC102_GlucoseO2_0.07_External_3_1.txt",
              "comments": [],
              "name": "JIC102_GlucoseO2_0.07_External_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC71_Nitrogen_0.20_External_2_1.txt",
              "comments": [],
              "name": "JIC71_Nitrogen_0.20_External_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC68_Nitrogen_0.10_External_2_1.txt",
              "comments": [],
              "name": "JIC68_Nitrogen_0.10_External_2_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC1_Carbon_0.07_Internal_1_2.txt",
              "comments": [],
              "name": "JIC1_Carbon_0.07_Internal_1_2.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC73_Phosphate_0.07_External_1_2.txt",
              "comments": [],
              "name": "JIC73_Phosphate_0.07_External_1_2.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC96_Ethanol_0.10_External_3_1.txt",
              "comments": [],
              "name": "JIC96_Ethanol_0.10_External_3_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC46_GlucoseO2_0.07_Internal_1_1.txt",
              "comments": [],
              "name": "JIC46_GlucoseO2_0.07_Internal_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC100_GlucoseO2_0.07_External_1_1.txt",
              "comments": [],
              "name": "JIC100_GlucoseO2_0.07_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC49_GlucoseO2_0.10_Internal_1_1.txt",
              "comments": [],
              "name": "JIC49_GlucoseO2_0.10_Internal_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC64_Nitrogen_0.07_External_1_2.txt",
              "comments": [],
              "name": "JIC64_Nitrogen_0.07_External_1_2.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC37_Ethanol_0.07_Internal_1_2.txt",
              "comments": [],
              "name": "JIC37_Ethanol_0.07_Internal_1_2.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC20_Phosphate_0.07_Internal_1_1.txt",
              "comments": [],
              "name": "JIC20_Phosphate_0.07_Internal_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC61_Carbon_0.20_External_1_1.txt",
              "comments": [],
              "name": "JIC61_Carbon_0.20_External_1_1.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC91_Ethanol_0.07_External_1_2.txt",
              "comments": [],
              "name": "JIC91_Ethanol_0.07_External_1_2.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/rawspectraldatafile-JIC9_Carbon_0.20_Internal_3_1.txt",
              "comments": [],
              "name": "JIC9_Carbon_0.20_Internal_3_1.txt",
              "type": "Raw Data File"
            }
          ],
          "technologyType": {
            "termSource": "OBI",
            "annotationValue": "mass spectrometry"
          },
          "processSequence": [
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC106_GlucoseO2_0.20_External_1_1.txt" } ],
              "name": "JIC106_GlucoseO2_0.20_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC106_GlucoseO2_0.20_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.2-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction78" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.1-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.1-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction5",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC85_Sulphate_0.10_External_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC36_Sulphate_0.20_Internal_1_3.txt" } ],
              "name": "JIC36_Sulphate_0.20_Internal_1_3",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC36_Sulphate_0.20_Internal_1_3",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-S-0.2-aliquot8" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction1" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.2-aliquot8" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.2-aliquot8" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction51",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC7_Carbon_0.20_Internal_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC102_GlucoseO2_0.07_External_3_1.txt" } ],
              "name": "JIC102_GlucoseO2_0.07_External_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC102_GlucoseO2_0.07_External_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.07-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction74" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.2-aliquot7" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.2-aliquot7" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction44",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC72_Nitrogen_0.20_External_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC73_Phosphate_0.07_External_1_2.txt" } ],
              "name": "JIC73_Phosphate_0.07_External_1_2",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC73_Phosphate_0.07_External_1_2",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.07-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction19" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.2-aliquot9" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.2-aliquot9" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction34",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC17_Nitrogen_0.20_Internal_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC37_Ethanol_0.07_Internal_1_2.txt" } ],
              "name": "JIC37_Ethanol_0.07_Internal_1_2",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC37_Ethanol_0.07_Internal_1_2",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-E-0.07-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction81" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-E-0.1-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-E-0.1-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction91",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC95_Ethanol_0.10_External_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC8_Carbon_0.20_Internal_2_1.txt" } ],
              "name": "JIC8_Carbon_0.20_Internal_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC8_Carbon_0.20_Internal_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.2-aliquot9" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction52" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.2-aliquot9" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.2-aliquot9" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction16",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC26_Phosphate_0.20_Internal_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.1-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.1-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction57",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC58_Carbon_0.10_External_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.1-aliquot8" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.1-aliquot8" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction48",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC4_Carbon_0.10_Internal_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC73_Phosphate_0.07_External_1_3.txt" } ],
              "name": "JIC73_Phosphate_0.07_External_1_3",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC73_Phosphate_0.07_External_1_3",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.07-aliquot7" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction20" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-E-0.1-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-E-0.1-aliquot3" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction86",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC42_Ethanol_0.10_Internal_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.07-aliquot8" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.07-aliquot8" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction27",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC10_Nitrogen_0.07_Internal_1_3" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.2-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.2-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction25",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC80_Phosphate_0.20_External_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.2-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.2-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction42",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC70_Nitrogen_0.20_External_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC51_GlucoseO2_0.10_Internal_3_1.txt" } ],
              "name": "JIC51_GlucoseO2_0.10_Internal_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC51_GlucoseO2_0.10_Internal_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.1-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction68" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.07-aliquot8" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.07-aliquot8" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction10",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC20_Phosphate_0.07_Internal_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC105_GlucoseO2_0.10_External_3_1.txt" } ],
              "name": "JIC105_GlucoseO2_0.10_External_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC105_GlucoseO2_0.10_External_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.1-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction77" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.1-aliquot8" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.1-aliquot8" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction30",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC13_Nitrogen_0.10_Internal_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC10_Nitrogen_0.07_Internal_1_3.txt" } ],
              "name": "JIC10_Nitrogen_0.07_Internal_1_3",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC10_Nitrogen_0.07_Internal_1_3",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.07-aliquot8" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction27" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.2-aliquot8" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.2-aliquot8" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction33",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC16_Nitrogen_0.20_Internal_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC14_Nitrogen_0.10_Internal_2_1.txt" } ],
              "name": "JIC14_Nitrogen_0.10_Internal_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC14_Nitrogen_0.10_Internal_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.1-aliquot9" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction31" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC27_Phosphate_0.20_Internal_3_1.txt" } ],
              "name": "JIC27_Phosphate_0.20_Internal_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC27_Phosphate_0.20_Internal_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.2-aliquot10" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction17" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC4_Carbon_0.10_Internal_1_1.txt" } ],
              "name": "JIC4_Carbon_0.10_Internal_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC4_Carbon_0.10_Internal_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.1-aliquot8" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction48" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC9_Carbon_0.20_Internal_3_1.txt" } ],
              "name": "JIC9_Carbon_0.20_Internal_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC9_Carbon_0.20_Internal_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.2-aliquot10" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction53" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC90_Sulphate_0.20_External_3_1.txt" } ],
              "name": "JIC90_Sulphate_0.20_External_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC90_Sulphate_0.20_External_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-S-0.2-aliquot7" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction9" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.1-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.1-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction21",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC76_Phosphate_0.10_External_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-E-0.07-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-E-0.07-aliquot4" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction87",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC91_Ethanol_0.07_External_1_3" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.07-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.07-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction19",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC73_Phosphate_0.07_External_1_2" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.07-aliquot7" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.07-aliquot7" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction56",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC57_Carbon_0.07_External_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.2-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.2-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction7",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC88_Sulphate_0.20_External_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC67_Nitrogen_0.10_External_1_1.txt" } ],
              "name": "JIC67_Nitrogen_0.10_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC67_Nitrogen_0.10_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.1-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction39" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC5_Carbon_0.10_Internal_2_1.txt" } ],
              "name": "JIC5_Carbon_0.10_Internal_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC5_Carbon_0.10_Internal_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.1-aliquot9" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction49" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC18_Nitrogen_0.20_Internal_3_1.txt" } ],
              "name": "JIC18_Nitrogen_0.20_Internal_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC18_Nitrogen_0.20_Internal_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.2-aliquot10" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction35" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC94_Ethanol_0.10_External_1_1.txt" } ],
              "name": "JIC94_Ethanol_0.10_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC94_Ethanol_0.10_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-E-0.1-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction90" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.07-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.07-aliquot3" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction74",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC102_GlucoseO2_0.07_External_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.07-aliquot9" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.07-aliquot9" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction28",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC11_Nitrogen_0.07_Internal_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.2-aliquot10" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.2-aliquot10" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction35",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC18_Nitrogen_0.20_Internal_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.1-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.1-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction6",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC86_Sulphate_0.10_External_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC59_Carbon_0.10_External_2_1.txt" } ],
              "name": "JIC59_Carbon_0.10_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC59_Carbon_0.10_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.1-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction58" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC89_Sulphate_0.20_External_2_1.txt" } ],
              "name": "JIC89_Sulphate_0.20_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC89_Sulphate_0.20_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-S-0.2-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction8" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.07-aliquot7" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.07-aliquot7" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction4",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC84_Sulphate_0.07_External_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.07-aliquot10" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.07-aliquot10" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction47",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC3_Carbon_0.07_Internal_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-E-0.07-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-E-0.07-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction89",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC93_Ethanol_0.07_External_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.07-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.07-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction65",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC48_GlucoseO2_0.07_Internal_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.07-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.07-aliquot2" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction73",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC101_GlucoseO2_0.07_External_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC72_Nitrogen_0.20_External_3_1.txt" } ],
              "name": "JIC72_Nitrogen_0.20_External_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC72_Nitrogen_0.20_External_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.2-aliquot7" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction44" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC16_Nitrogen_0.20_Internal_1_1.txt" } ],
              "name": "JIC16_Nitrogen_0.20_Internal_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC16_Nitrogen_0.20_Internal_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.2-aliquot8" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction33" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.2-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.2-aliquot1" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction78",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC106_GlucoseO2_0.20_External_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC93_Ethanol_0.07_External_3_1.txt" } ],
              "name": "JIC93_Ethanol_0.07_External_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC93_Ethanol_0.07_External_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-E-0.07-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction89" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.07-aliquot9" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.07-aliquot9" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction46",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC2_Carbon_0.07_Internal_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC46_GlucoseO2_0.07_Internal_1_1.txt" } ],
              "name": "JIC46_GlucoseO2_0.07_Internal_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC46_GlucoseO2_0.07_Internal_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.07-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction63" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC65_Nitrogen_0.07_External_2_1.txt" } ],
              "name": "JIC65_Nitrogen_0.07_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC65_Nitrogen_0.07_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.07-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction37" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC79_Phosphate_0.20_External_1_1.txt" } ],
              "name": "JIC79_Phosphate_0.20_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC79_Phosphate_0.20_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.2-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction24" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-E-0.07-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-E-0.07-aliquot1" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction81",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC37_Ethanol_0.07_Internal_1_3" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.07-aliquot9" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.07-aliquot9" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction11",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC21_Phosphate_0.07_Internal_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC55_Carbon_0.07_External_1_3.txt" } ],
              "name": "JIC55_Carbon_0.07_External_1_3",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC55_Carbon_0.07_External_1_3",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.07-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction54" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC1_Carbon_0.07_Internal_1_1.txt" } ],
              "name": "JIC1_Carbon_0.07_Internal_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC1_Carbon_0.07_Internal_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.07-aliquot8" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction45" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-E-0.1-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-E-0.1-aliquot2" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction85",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC41_Ethanol_0.10_Internal_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC46_GlucoseO2_0.07_Internal_1_3.txt" } ],
              "name": "JIC46_GlucoseO2_0.07_Internal_1_3",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC46_GlucoseO2_0.07_Internal_1_3",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.07-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction63" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC63_Carbon_0.20_External_3_1.txt" } ],
              "name": "JIC63_Carbon_0.20_External_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC63_Carbon_0.20_External_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.2-aliquot7" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction62" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC91_Ethanol_0.07_External_1_1.txt" } ],
              "name": "JIC91_Ethanol_0.07_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC91_Ethanol_0.07_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-E-0.07-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction87" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC60_Carbon_0.10_External_3_1.txt" } ],
              "name": "JIC60_Carbon_0.10_External_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC60_Carbon_0.10_External_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.1-aliquot7" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction59" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC49_GlucoseO2_0.10_Internal_1_1.txt" } ],
              "name": "JIC49_GlucoseO2_0.10_Internal_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC49_GlucoseO2_0.10_Internal_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.1-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction66" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC82_Sulphate_0.07_External_1_1.txt" } ],
              "name": "JIC82_Sulphate_0.07_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC82_Sulphate_0.07_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-S-0.07-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction2" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.1-aliquot9" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.1-aliquot9" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction31",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC14_Nitrogen_0.10_Internal_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC39_Ethanol_0.07_Internal_3_1.txt" } ],
              "name": "JIC39_Ethanol_0.07_Internal_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC39_Ethanol_0.07_Internal_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-E-0.07-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction83" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC3_Carbon_0.07_Internal_3_1.txt" } ],
              "name": "JIC3_Carbon_0.07_Internal_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC3_Carbon_0.07_Internal_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.07-aliquot10" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction47" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.07-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.07-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction54",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC55_Carbon_0.07_External_1_3" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.2-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.2-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction24",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC79_Phosphate_0.20_External_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.2-aliquot8" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.2-aliquot8" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction15",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC25_Phosphate_0.20_Internal_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC10_Nitrogen_0.07_Internal_1_2.txt" } ],
              "name": "JIC10_Nitrogen_0.07_Internal_1_2",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC10_Nitrogen_0.07_Internal_1_2",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.07-aliquot8" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction27" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC54_GlucoseO2_0.20_Internal_3_1.txt" } ],
              "name": "JIC54_GlucoseO2_0.20_Internal_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC54_GlucoseO2_0.20_Internal_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.2-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction71" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC50_GlucoseO2_0.10_Internal_2_1.txt" } ],
              "name": "JIC50_GlucoseO2_0.10_Internal_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC50_GlucoseO2_0.10_Internal_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.1-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction67" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.07-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.07-aliquot1" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction72",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC100_GlucoseO2_0.07_External_1_2" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.1-aliquot8" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.1-aliquot8" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction12",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC22_Phosphate_0.10_Internal_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC38_Ethanol_0.07_Internal_2_1.txt" } ],
              "name": "JIC38_Ethanol_0.07_Internal_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC38_Ethanol_0.07_Internal_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-E-0.07-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction82" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.07-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.07-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction36",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC64_Nitrogen_0.07_External_1_3" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC48_GlucoseO2_0.07_Internal_3_1.txt" } ],
              "name": "JIC48_GlucoseO2_0.07_Internal_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC48_GlucoseO2_0.07_Internal_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.07-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction65" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.07-aliquot7" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.07-aliquot7" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction20",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC73_Phosphate_0.07_External_1_3" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC55_Carbon_0.07_External_1_1.txt" } ],
              "name": "JIC55_Carbon_0.07_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC55_Carbon_0.07_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.07-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction54" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC1_Carbon_0.07_Internal_1_2.txt" } ],
              "name": "JIC1_Carbon_0.07_Internal_1_2",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC1_Carbon_0.07_Internal_1_2",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.07-aliquot8" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction45" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC62_Carbon_0.20_External_2_1.txt" } ],
              "name": "JIC62_Carbon_0.20_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC62_Carbon_0.20_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.2-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction61" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC47_GlucoseO2_0.07_Internal_2_1.txt" } ],
              "name": "JIC47_GlucoseO2_0.07_Internal_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC47_GlucoseO2_0.07_Internal_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.07-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction64" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-/Users/eamonnmaguire/Dropbox/ISAtab" } ],
              "name": "JIC84_Sulphate_0.07_External_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC84_Sulphate_0.07_External_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-S-0.07-aliquot7" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction4" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.1-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.1-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction68",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC51_GlucoseO2_0.10_Internal_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC88_Sulphate_0.20_External_1_1.txt" } ],
              "name": "JIC88_Sulphate_0.20_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC88_Sulphate_0.20_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-S-0.2-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction7" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.1-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.1-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction39",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC67_Nitrogen_0.10_External_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC46_GlucoseO2_0.07_Internal_1_2.txt" } ],
              "name": "JIC46_GlucoseO2_0.07_Internal_1_2",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC46_GlucoseO2_0.07_Internal_1_2",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.07-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction63" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.07-aliquot8" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.07-aliquot8" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction45",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC1_Carbon_0.07_Internal_1_3" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.2-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.2-aliquot3" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction80",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC108_GlucoseO2_0.20_External_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC58_Carbon_0.10_External_1_1.txt" } ],
              "name": "JIC58_Carbon_0.10_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC58_Carbon_0.10_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.1-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction57" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC74_Phosphate_0.07_External_2_1.txt" } ],
              "name": "JIC74_Phosphate_0.07_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC74_Phosphate_0.07_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.07-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction18" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.2-aliquot9" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.2-aliquot9" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction52",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC8_Carbon_0.20_Internal_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.1-aliquot7" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.1-aliquot7" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction59",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC60_Carbon_0.10_External_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.1-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.1-aliquot2" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction76",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC104_GlucoseO2_0.10_External_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC25_Phosphate_0.20_Internal_1_1.txt" } ],
              "name": "JIC25_Phosphate_0.20_Internal_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC25_Phosphate_0.20_Internal_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.2-aliquot8" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction15" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC70_Nitrogen_0.20_External_1_1.txt" } ],
              "name": "JIC70_Nitrogen_0.20_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC70_Nitrogen_0.20_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.2-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction42" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.2-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.2-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction61",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC62_Carbon_0.20_External_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.2-aliquot10" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.2-aliquot10" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction53",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC9_Carbon_0.20_Internal_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC55_Carbon_0.07_External_1_2.txt" } ],
              "name": "JIC55_Carbon_0.07_External_1_2",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC55_Carbon_0.07_External_1_2",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.07-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction54" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-E-0.1-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-E-0.1-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction92",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC96_Ethanol_0.10_External_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC95_Ethanol_0.10_External_2_1.txt" } ],
              "name": "JIC95_Ethanol_0.10_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC95_Ethanol_0.10_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-E-0.1-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction91" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.1-aliquot9" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.1-aliquot9" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction13",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC23_Phosphate_0.10_Internal_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC85_Sulphate_0.10_External_1_1.txt" } ],
              "name": "JIC85_Sulphate_0.10_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC85_Sulphate_0.10_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-S-0.1-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction5" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.07-aliquot7" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.07-aliquot7" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction38",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC66_Nitrogen_0.07_External_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC73_Phosphate_0.07_External_1_1.txt" } ],
              "name": "JIC73_Phosphate_0.07_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC73_Phosphate_0.07_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.07-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction18" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC64_Nitrogen_0.07_External_1_3.txt" } ],
              "name": "JIC64_Nitrogen_0.07_External_1_3",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC64_Nitrogen_0.07_External_1_3",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.07-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction36" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.1-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.1-aliquot3" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction77",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC105_GlucoseO2_0.10_External_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC42_Ethanol_0.10_Internal_3_1.txt" } ],
              "name": "JIC42_Ethanol_0.10_Internal_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC42_Ethanol_0.10_Internal_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-E-0.1-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction86" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC40_Ethanol_0.10_Internal_1_1.txt" } ],
              "name": "JIC40_Ethanol_0.10_Internal_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC40_Ethanol_0.10_Internal_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-E-0.1-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction84" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC80_Phosphate_0.20_External_2_1.txt" } ],
              "name": "JIC80_Phosphate_0.20_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC80_Phosphate_0.20_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.2-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction25" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.07-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.07-aliquot4" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction63",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC46_GlucoseO2_0.07_Internal_1_3" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC17_Nitrogen_0.20_Internal_2_1.txt" } ],
              "name": "JIC17_Nitrogen_0.20_Internal_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC17_Nitrogen_0.20_Internal_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.2-aliquot9" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction34" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC6_Carbon_0.10_Internal_3_1.txt" } ],
              "name": "JIC6_Carbon_0.10_Internal_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC6_Carbon_0.10_Internal_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.1-aliquot10" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction50" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC66_Nitrogen_0.07_External_3_1.txt" } ],
              "name": "JIC66_Nitrogen_0.07_External_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC66_Nitrogen_0.07_External_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.07-aliquot7" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction38" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.07-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.07-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction64",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC47_GlucoseO2_0.07_Internal_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC108_GlucoseO2_0.20_External_3_1.txt" } ],
              "name": "JIC108_GlucoseO2_0.20_External_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC108_GlucoseO2_0.20_External_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.2-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction80" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC23_Phosphate_0.10_Internal_2_1.txt" } ],
              "name": "JIC23_Phosphate_0.10_Internal_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC23_Phosphate_0.10_Internal_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.1-aliquot9" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction13" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC11_Nitrogen_0.07_Internal_2_1.txt" } ],
              "name": "JIC11_Nitrogen_0.07_Internal_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC11_Nitrogen_0.07_Internal_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.07-aliquot9" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction28" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.1-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.1-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction67",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC50_GlucoseO2_0.10_Internal_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.1-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.1-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction22",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC77_Phosphate_0.10_External_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC37_Ethanol_0.07_Internal_1_1.txt" } ],
              "name": "JIC37_Ethanol_0.07_Internal_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC37_Ethanol_0.07_Internal_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-E-0.07-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction81" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.2-aliquot7" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.2-aliquot7" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction62",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC63_Carbon_0.20_External_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC24_Phosphate_0.10_Internal_3_1.txt" } ],
              "name": "JIC24_Phosphate_0.10_Internal_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC24_Phosphate_0.10_Internal_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.1-aliquot10" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction14" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.2-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.2-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction60",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC61_Carbon_0.20_External_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC22_Phosphate_0.10_Internal_1_1.txt" } ],
              "name": "JIC22_Phosphate_0.10_Internal_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC22_Phosphate_0.10_Internal_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.1-aliquot8" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction12" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC52_GlucoseO2_0.20_Internal_1_1.txt" } ],
              "name": "JIC52_GlucoseO2_0.20_Internal_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC52_GlucoseO2_0.20_Internal_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.2-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction69" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.1-aliquot10" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.1-aliquot10" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction32",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC15_Nitrogen_0.10_Internal_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.1-aliquot9" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.1-aliquot9" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction49",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC5_Carbon_0.10_Internal_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.2-aliquot7" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.2-aliquot7" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction26",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC81_Phosphate_0.20_External_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC107_GlucoseO2_0.20_External_2_1.txt" } ],
              "name": "JIC107_GlucoseO2_0.20_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC107_GlucoseO2_0.20_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.2-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction79" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.2-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.2-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction70",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC53_GlucoseO2_0.20_Internal_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.2-aliquot8" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.2-aliquot8" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction1",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC36_Sulphate_0.20_Internal_1_3" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.2-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.2-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction71",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC54_GlucoseO2_0.20_Internal_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-E-0.07-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-E-0.07-aliquot2" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction82",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC38_Ethanol_0.07_Internal_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-/Users/eamonnmaguire/Dropbox/Presentations" } ],
              "name": "JIC82_Sulphate_0.07_External_1_3",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC82_Sulphate_0.07_External_1_3",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-S-0.07-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction2" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.1-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.1-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction40",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC68_Nitrogen_0.10_External_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC96_Ethanol_0.10_External_3_1.txt" } ],
              "name": "JIC96_Ethanol_0.10_External_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC96_Ethanol_0.10_External_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-E-0.1-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction92" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.2-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.2-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction43",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC71_Nitrogen_0.20_External_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC20_Phosphate_0.07_Internal_1_1.txt" } ],
              "name": "JIC20_Phosphate_0.07_Internal_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC20_Phosphate_0.07_Internal_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.07-aliquot8" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction10" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC69_Nitrogen_0.10_External_3_1.txt" } ],
              "name": "JIC69_Nitrogen_0.10_External_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC69_Nitrogen_0.10_External_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.1-aliquot7" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction41" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-E-0.1-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-E-0.1-aliquot4" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction90",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC94_Ethanol_0.10_External_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.07-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.07-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction55",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC56_Carbon_0.07_External_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC103_GlucoseO2_0.10_External_1_1.txt" } ],
              "name": "JIC103_GlucoseO2_0.10_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC103_GlucoseO2_0.10_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.1-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction75" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.2-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.2-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction8",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC89_Sulphate_0.20_External_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC2_Carbon_0.07_Internal_2_1.txt" } ],
              "name": "JIC2_Carbon_0.07_Internal_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC2_Carbon_0.07_Internal_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.07-aliquot9" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction46" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC41_Ethanol_0.10_Internal_2_1.txt" } ],
              "name": "JIC41_Ethanol_0.10_Internal_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC41_Ethanol_0.10_Internal_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-E-0.1-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction85" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC76_Phosphate_0.10_External_1_1.txt" } ],
              "name": "JIC76_Phosphate_0.10_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC76_Phosphate_0.10_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.1-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction21" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.07-aliquot10" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.07-aliquot10" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction29",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC12_Nitrogen_0.07_Internal_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC61_Carbon_0.20_External_1_1.txt" } ],
              "name": "JIC61_Carbon_0.20_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC61_Carbon_0.20_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.2-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction60" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC77_Phosphate_0.10_External_2_1.txt" } ],
              "name": "JIC77_Phosphate_0.10_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC77_Phosphate_0.10_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.1-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction22" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.2-aliquot7" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.2-aliquot7" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction9",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC90_Sulphate_0.20_External_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC7_Carbon_0.20_Internal_1_1.txt" } ],
              "name": "JIC7_Carbon_0.20_Internal_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC7_Carbon_0.20_Internal_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.2-aliquot8" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction51" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.1-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.1-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction58",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC59_Carbon_0.10_External_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC100_GlucoseO2_0.07_External_1_2.txt" } ],
              "name": "JIC100_GlucoseO2_0.07_External_1_2",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC100_GlucoseO2_0.07_External_1_2",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.07-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction72" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC78_Phosphate_0.10_External_3_1.txt" } ],
              "name": "JIC78_Phosphate_0.10_External_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC78_Phosphate_0.10_External_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.1-aliquot7" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction23" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-E-0.1-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-E-0.1-aliquot1" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction84",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC40_Ethanol_0.10_Internal_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC64_Nitrogen_0.07_External_1_1.txt" } ],
              "name": "JIC64_Nitrogen_0.07_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC64_Nitrogen_0.07_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.07-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction36" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC68_Nitrogen_0.10_External_2_1.txt" } ],
              "name": "JIC68_Nitrogen_0.10_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC68_Nitrogen_0.10_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.1-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction40" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC56_Carbon_0.07_External_2_1.txt" } ],
              "name": "JIC56_Carbon_0.07_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC56_Carbon_0.07_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.07-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction55" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC26_Phosphate_0.20_Internal_2_1.txt" } ],
              "name": "JIC26_Phosphate_0.20_Internal_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC26_Phosphate_0.20_Internal_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.2-aliquot9" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction16" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.07-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.07-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction18",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC75_Phosphate_0.07_External_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC1_Carbon_0.07_Internal_1_3.txt" } ],
              "name": "JIC1_Carbon_0.07_Internal_1_3",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC1_Carbon_0.07_Internal_1_3",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.07-aliquot8" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction45" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC57_Carbon_0.07_External_3_1.txt" } ],
              "name": "JIC57_Carbon_0.07_External_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC57_Carbon_0.07_External_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-C-0.07-aliquot7" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction56" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC91_Ethanol_0.07_External_1_2.txt" } ],
              "name": "JIC91_Ethanol_0.07_External_1_2",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC91_Ethanol_0.07_External_1_2",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-E-0.07-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction87" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.1-aliquot7" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.1-aliquot7" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction41",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC69_Nitrogen_0.10_External_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-E-0.07-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-E-0.07-aliquot3" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction83",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC39_Ethanol_0.07_Internal_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC75_Phosphate_0.07_External_3_1.txt" } ],
              "name": "JIC75_Phosphate_0.07_External_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC75_Phosphate_0.07_External_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.07-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction18" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC21_Phosphate_0.07_Internal_2_1.txt" } ],
              "name": "JIC21_Phosphate_0.07_Internal_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC21_Phosphate_0.07_Internal_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.07-aliquot9" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction11" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.1-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.1-aliquot4" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction66",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC49_GlucoseO2_0.10_Internal_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.1-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.1-aliquot1" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction75",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC103_GlucoseO2_0.10_External_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.2-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.2-aliquot2" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction79",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC107_GlucoseO2_0.20_External_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC53_GlucoseO2_0.20_Internal_2_1.txt" } ],
              "name": "JIC53_GlucoseO2_0.20_Internal_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC53_GlucoseO2_0.20_Internal_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.2-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction70" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC104_GlucoseO2_0.10_External_2_1.txt" } ],
              "name": "JIC104_GlucoseO2_0.10_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC104_GlucoseO2_0.10_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.1-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction76" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC13_Nitrogen_0.10_Internal_1_1.txt" } ],
              "name": "JIC13_Nitrogen_0.10_Internal_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC13_Nitrogen_0.10_Internal_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.1-aliquot8" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction30" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC81_Phosphate_0.20_External_3_1.txt" } ],
              "name": "JIC81_Phosphate_0.20_External_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC81_Phosphate_0.20_External_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-P-0.2-aliquot7" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction26" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.2-aliquot10" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.2-aliquot10" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction17",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC27_Phosphate_0.20_Internal_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC101_GlucoseO2_0.07_External_2_1.txt" } ],
              "name": "JIC101_GlucoseO2_0.07_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC101_GlucoseO2_0.07_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.07-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction73" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC15_Nitrogen_0.10_Internal_3_1.txt" } ],
              "name": "JIC15_Nitrogen_0.10_Internal_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC15_Nitrogen_0.10_Internal_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.1-aliquot10" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction32" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.07-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.07-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction37",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC65_Nitrogen_0.07_External_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC86_Sulphate_0.10_External_2_1.txt" } ],
              "name": "JIC86_Sulphate_0.10_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC86_Sulphate_0.10_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-S-0.1-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction6" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.1-aliquot10" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.1-aliquot10" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction14",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC24_Phosphate_0.10_Internal_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.07-aliquot6" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.07-aliquot6" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction3",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC83_Sulphate_0.07_External_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-E-0.07-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-E-0.07-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction88",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC92_Ethanol_0.07_External_2_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-/Users/eamonnmaguire/Downloads/sample-data" } ],
              "name": "JIC83_Sulphate_0.07_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC83_Sulphate_0.07_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-S-0.07-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction3" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC100_GlucoseO2_0.07_External_1_1.txt" } ],
              "name": "JIC100_GlucoseO2_0.07_External_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC100_GlucoseO2_0.07_External_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-G-0.07-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction72" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.1-aliquot10" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.1-aliquot10" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction50",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC6_Carbon_0.10_Internal_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-/Users/eamonnmaguire/Dropbox/Presentation_Images" } ],
              "name": "JIC82_Sulphate_0.07_External_1_2",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC82_Sulphate_0.07_External_1_2",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-S-0.07-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction2" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC91_Ethanol_0.07_External_1_3.txt" } ],
              "name": "JIC91_Ethanol_0.07_External_1_3",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC91_Ethanol_0.07_External_1_3",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-E-0.07-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction87" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.1-aliquot7" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.1-aliquot7" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction23",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC78_Phosphate_0.10_External_3_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC12_Nitrogen_0.07_Internal_3_1.txt" } ],
              "name": "JIC12_Nitrogen_0.07_Internal_3_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC12_Nitrogen_0.07_Internal_3_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.07-aliquot10" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction29" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-G-0.2-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-G-0.2-aliquot4" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 4,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 200,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction69",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC52_GlucoseO2_0.20_Internal_1_1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC37_Ethanol_0.07_Internal_1_3.txt" } ],
              "name": "JIC37_Ethanol_0.07_Internal_1_3",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC37_Ethanol_0.07_Internal_1_3",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-E-0.07-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction81" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC64_Nitrogen_0.07_External_1_2.txt" } ],
              "name": "JIC64_Nitrogen_0.07_External_1_2",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC64_Nitrogen_0.07_External_1_2",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.07-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction36" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC71_Nitrogen_0.20_External_2_1.txt" } ],
              "name": "JIC71_Nitrogen_0.20_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC71_Nitrogen_0.20_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.2-aliquot6" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction43" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC10_Nitrogen_0.07_Internal_1_1.txt" } ],
              "name": "JIC10_Nitrogen_0.07_Internal_1_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC10_Nitrogen_0.07_Internal_1_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-N-0.07-aliquot8" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction27" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.07-aliquot5" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.07-aliquot5" } ],
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/standard_volume" },
                  "value": 20,
                  "unit": { "@id": "#Unit/microliter" }
                },
                {
                  "category": { "@id": "#parameter/sample_volume" },
                  "value": 1000,
                  "unit": { "@id": "#Unit/microliter" }
                }
              ],
              "@id": "#process/metabolite_extraction2",

              "comments": [],
              "nextProcess": { "@id": "#process/JIC82_Sulphate_0.07_External_1_3" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/metabolite_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/rawspectraldatafile-JIC92_Ethanol_0.07_External_2_1.txt" } ],
              "name": "JIC92_Ethanol_0.07_External_2_1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/JIC92_Ethanol_0.07_External_2_1",
              "parameterValues": [],
              "inputs": [ { "@id": "#material/extract-E-0.07-aliquot5" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/metabolite_extraction88" },
              "performer": ""
            }
          ],
          "@id": "#assay/a_metabolome.txt",
          "unitCategories": [
            {
              "@id": "#Unit/microliter",
              "termAccession": "http://purl.obolibrary.org/obo/UO_0000101",
              "annotationValue": "microliter",
              "termSource": "UO"
            }
          ],
          "characteristicCategories": [
            {
              "@id": "#characteristic_category/Material_Type",
              "characteristicType": {
                "annotationValue": "Material Type"
              }
            }
          ],
          "materials": {
            "otherMaterials": [
              {
                "@id": "#material/extract-G-0.1-aliquot1",
                "characteristics": [],
                "name": "extract-G-0.1-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.2-aliquot10",
                "characteristics": [],
                "name": "extract-N-0.2-aliquot10",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.1-aliquot3",
                "characteristics": [],
                "name": "extract-G-0.1-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.2-aliquot7",
                "characteristics": [],
                "name": "extract-P-0.2-aliquot7",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.2-aliquot8",
                "characteristics": [],
                "name": "extract-N-0.2-aliquot8",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.1-aliquot4",
                "characteristics": [],
                "name": "extract-G-0.1-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.2-aliquot5",
                "characteristics": [],
                "name": "extract-G-0.2-aliquot5",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.1-aliquot8",
                "characteristics": [],
                "name": "extract-P-0.1-aliquot8",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-E-0.07-aliquot1",
                "characteristics": [],
                "name": "extract-E-0.07-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.1-aliquot7",
                "characteristics": [],
                "name": "extract-N-0.1-aliquot7",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-E-0.1-aliquot6",
                "characteristics": [],
                "name": "extract-E-0.1-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.1-aliquot5",
                "characteristics": [],
                "name": "extract-N-0.1-aliquot5",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.1-aliquot6",
                "characteristics": [],
                "name": "extract-P-0.1-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.2-aliquot5",
                "characteristics": [],
                "name": "extract-N-0.2-aliquot5",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.2-aliquot9",
                "characteristics": [],
                "name": "extract-N-0.2-aliquot9",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.07-aliquot6",
                "characteristics": [],
                "name": "extract-P-0.07-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.07-aliquot10",
                "characteristics": [],
                "name": "extract-C-0.07-aliquot10",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-E-0.07-aliquot6",
                "characteristics": [],
                "name": "extract-E-0.07-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-S-0.07-aliquot5",
                "characteristics": [],
                "name": "extract-S-0.07-aliquot5",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-E-0.1-aliquot2",
                "characteristics": [],
                "name": "extract-E-0.1-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.07-aliquot10",
                "characteristics": [],
                "name": "extract-N-0.07-aliquot10",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.07-aliquot6",
                "characteristics": [],
                "name": "extract-N-0.07-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.2-aliquot6",
                "characteristics": [],
                "name": "extract-C-0.2-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-E-0.1-aliquot5",
                "characteristics": [],
                "name": "extract-E-0.1-aliquot5",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.07-aliquot1",
                "characteristics": [],
                "name": "extract-G-0.07-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.2-aliquot6",
                "characteristics": [],
                "name": "extract-G-0.2-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.1-aliquot5",
                "characteristics": [],
                "name": "extract-P-0.1-aliquot5",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.1-aliquot9",
                "characteristics": [],
                "name": "extract-N-0.1-aliquot9",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.1-aliquot9",
                "characteristics": [],
                "name": "extract-P-0.1-aliquot9",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.2-aliquot7",
                "characteristics": [],
                "name": "extract-N-0.2-aliquot7",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.2-aliquot7",
                "characteristics": [],
                "name": "extract-C-0.2-aliquot7",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.07-aliquot8",
                "characteristics": [],
                "name": "extract-P-0.07-aliquot8",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-E-0.1-aliquot4",
                "characteristics": [],
                "name": "extract-E-0.1-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.07-aliquot9",
                "characteristics": [],
                "name": "extract-N-0.07-aliquot9",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.07-aliquot5",
                "characteristics": [],
                "name": "extract-P-0.07-aliquot5",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.2-aliquot6",
                "characteristics": [],
                "name": "extract-P-0.2-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.1-aliquot7",
                "characteristics": [],
                "name": "extract-C-0.1-aliquot7",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.07-aliquot7",
                "characteristics": [],
                "name": "extract-C-0.07-aliquot7",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.07-aliquot6",
                "characteristics": [],
                "name": "extract-C-0.07-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.1-aliquot5",
                "characteristics": [],
                "name": "extract-C-0.1-aliquot5",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.07-aliquot3",
                "characteristics": [],
                "name": "extract-G-0.07-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.2-aliquot8",
                "characteristics": [],
                "name": "extract-P-0.2-aliquot8",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.1-aliquot8",
                "characteristics": [],
                "name": "extract-C-0.1-aliquot8",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-S-0.07-aliquot7",
                "characteristics": [],
                "name": "extract-S-0.07-aliquot7",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.07-aliquot6",
                "characteristics": [],
                "name": "extract-G-0.07-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.2-aliquot10",
                "characteristics": [],
                "name": "extract-P-0.2-aliquot10",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.1-aliquot2",
                "characteristics": [],
                "name": "extract-G-0.1-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-S-0.2-aliquot7",
                "characteristics": [],
                "name": "extract-S-0.2-aliquot7",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.07-aliquot8",
                "characteristics": [],
                "name": "extract-C-0.07-aliquot8",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-S-0.2-aliquot5",
                "characteristics": [],
                "name": "extract-S-0.2-aliquot5",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.2-aliquot4",
                "characteristics": [],
                "name": "extract-G-0.2-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-S-0.2-aliquot6",
                "characteristics": [],
                "name": "extract-S-0.2-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.2-aliquot6",
                "characteristics": [],
                "name": "extract-N-0.2-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.1-aliquot6",
                "characteristics": [],
                "name": "extract-C-0.1-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.1-aliquot6",
                "characteristics": [],
                "name": "extract-G-0.1-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.07-aliquot7",
                "characteristics": [],
                "name": "extract-N-0.07-aliquot7",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.07-aliquot5",
                "characteristics": [],
                "name": "extract-N-0.07-aliquot5",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.07-aliquot4",
                "characteristics": [],
                "name": "extract-G-0.07-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.2-aliquot1",
                "characteristics": [],
                "name": "extract-G-0.2-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.2-aliquot5",
                "characteristics": [],
                "name": "extract-P-0.2-aliquot5",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.2-aliquot9",
                "characteristics": [],
                "name": "extract-C-0.2-aliquot9",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.2-aliquot8",
                "characteristics": [],
                "name": "extract-C-0.2-aliquot8",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-E-0.07-aliquot3",
                "characteristics": [],
                "name": "extract-E-0.07-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.1-aliquot8",
                "characteristics": [],
                "name": "extract-N-0.1-aliquot8",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.1-aliquot9",
                "characteristics": [],
                "name": "extract-C-0.1-aliquot9",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-S-0.2-aliquot8",
                "characteristics": [],
                "name": "extract-S-0.2-aliquot8",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.2-aliquot3",
                "characteristics": [],
                "name": "extract-G-0.2-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.07-aliquot2",
                "characteristics": [],
                "name": "extract-G-0.07-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.2-aliquot9",
                "characteristics": [],
                "name": "extract-P-0.2-aliquot9",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.2-aliquot2",
                "characteristics": [],
                "name": "extract-G-0.2-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-E-0.07-aliquot4",
                "characteristics": [],
                "name": "extract-E-0.07-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.07-aliquot9",
                "characteristics": [],
                "name": "extract-C-0.07-aliquot9",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.2-aliquot5",
                "characteristics": [],
                "name": "extract-C-0.2-aliquot5",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.1-aliquot7",
                "characteristics": [],
                "name": "extract-P-0.1-aliquot7",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-S-0.1-aliquot6",
                "characteristics": [],
                "name": "extract-S-0.1-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.1-aliquot10",
                "characteristics": [],
                "name": "extract-C-0.1-aliquot10",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.07-aliquot7",
                "characteristics": [],
                "name": "extract-P-0.07-aliquot7",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-S-0.07-aliquot6",
                "characteristics": [],
                "name": "extract-S-0.07-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-E-0.1-aliquot1",
                "characteristics": [],
                "name": "extract-E-0.1-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-E-0.1-aliquot3",
                "characteristics": [],
                "name": "extract-E-0.1-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.1-aliquot6",
                "characteristics": [],
                "name": "extract-N-0.1-aliquot6",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-E-0.07-aliquot2",
                "characteristics": [],
                "name": "extract-E-0.07-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-S-0.1-aliquot5",
                "characteristics": [],
                "name": "extract-S-0.1-aliquot5",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.1-aliquot10",
                "characteristics": [],
                "name": "extract-P-0.1-aliquot10",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-E-0.07-aliquot5",
                "characteristics": [],
                "name": "extract-E-0.07-aliquot5",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.07-aliquot8",
                "characteristics": [],
                "name": "extract-N-0.07-aliquot8",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.07-aliquot5",
                "characteristics": [],
                "name": "extract-C-0.07-aliquot5",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.1-aliquot5",
                "characteristics": [],
                "name": "extract-G-0.1-aliquot5",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.2-aliquot10",
                "characteristics": [],
                "name": "extract-C-0.2-aliquot10",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.1-aliquot10",
                "characteristics": [],
                "name": "extract-N-0.1-aliquot10",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.07-aliquot9",
                "characteristics": [],
                "name": "extract-P-0.07-aliquot9",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-G-0.07-aliquot5",
                "characteristics": [],
                "name": "extract-G-0.07-aliquot5",
                "type": "Extract Name"
              }
            ],
            "samples": [
              { "@id": "#sample/sample-N-0.07-aliquot7" },
              { "@id": "#sample/sample-E-0.1-aliquot6" },
              { "@id": "#sample/sample-G-0.07-aliquot1" },
              { "@id": "#sample/sample-C-0.2-aliquot5" },
              { "@id": "#sample/sample-G-0.2-aliquot6" },
              { "@id": "#sample/sample-N-0.1-aliquot7" },
              { "@id": "#sample/sample-S-0.1-aliquot5" },
              { "@id": "#sample/sample-C-0.1-aliquot5" },
              { "@id": "#sample/sample-S-0.07-aliquot5" },
              { "@id": "#sample/sample-P-0.2-aliquot7" },
              { "@id": "#sample/sample-G-0.2-aliquot2" },
              { "@id": "#sample/sample-S-0.2-aliquot5" },
              { "@id": "#sample/sample-C-0.2-aliquot8" },
              { "@id": "#sample/sample-N-0.07-aliquot9" },
              { "@id": "#sample/sample-G-0.2-aliquot3" },
              { "@id": "#sample/sample-C-0.1-aliquot6" },
              { "@id": "#sample/sample-N-0.2-aliquot7" },
              { "@id": "#sample/sample-E-0.07-aliquot1" },
              { "@id": "#sample/sample-G-0.07-aliquot6" },
              { "@id": "#sample/sample-G-0.1-aliquot4" },
              { "@id": "#sample/sample-G-0.2-aliquot4" },
              { "@id": "#sample/sample-N-0.1-aliquot5" },
              { "@id": "#sample/sample-P-0.2-aliquot10" },
              { "@id": "#sample/sample-P-0.2-aliquot5" },
              { "@id": "#sample/sample-P-0.07-aliquot5" },
              { "@id": "#sample/sample-G-0.1-aliquot3" },
              { "@id": "#sample/sample-N-0.07-aliquot8" },
              { "@id": "#sample/sample-S-0.2-aliquot7" },
              { "@id": "#sample/sample-C-0.07-aliquot10" },
              { "@id": "#sample/sample-P-0.1-aliquot6" },
              { "@id": "#sample/sample-C-0.2-aliquot6" },
              { "@id": "#sample/sample-P-0.1-aliquot10" },
              { "@id": "#sample/sample-G-0.1-aliquot2" },
              { "@id": "#sample/sample-E-0.07-aliquot5" },
              { "@id": "#sample/sample-E-0.07-aliquot4" },
              { "@id": "#sample/sample-N-0.2-aliquot9" },
              { "@id": "#sample/sample-C-0.07-aliquot9" },
              { "@id": "#sample/sample-N-0.1-aliquot9" },
              { "@id": "#sample/sample-P-0.1-aliquot9" },
              { "@id": "#sample/sample-E-0.1-aliquot4" },
              { "@id": "#sample/sample-G-0.1-aliquot1" },
              { "@id": "#sample/sample-E-0.07-aliquot3" },
              { "@id": "#sample/sample-P-0.1-aliquot5" },
              { "@id": "#sample/sample-G-0.07-aliquot4" },
              { "@id": "#sample/sample-N-0.1-aliquot8" },
              { "@id": "#sample/sample-S-0.07-aliquot7" },
              { "@id": "#sample/sample-C-0.07-aliquot5" },
              { "@id": "#sample/sample-N-0.1-aliquot6" },
              { "@id": "#sample/sample-C-0.2-aliquot9" },
              { "@id": "#sample/sample-C-0.2-aliquot7" },
              { "@id": "#sample/sample-P-0.1-aliquot8" },
              { "@id": "#sample/sample-P-0.07-aliquot8" },
              { "@id": "#sample/sample-G-0.1-aliquot5" },
              { "@id": "#sample/sample-P-0.1-aliquot7" },
              { "@id": "#sample/sample-E-0.1-aliquot3" },
              { "@id": "#sample/sample-N-0.07-aliquot6" },
              { "@id": "#sample/sample-G-0.07-aliquot3" },
              { "@id": "#sample/sample-C-0.07-aliquot8" },
              { "@id": "#sample/sample-G-0.1-aliquot6" },
              { "@id": "#sample/sample-P-0.07-aliquot9" },
              { "@id": "#sample/sample-E-0.07-aliquot2" },
              { "@id": "#sample/sample-N-0.1-aliquot10" },
              { "@id": "#sample/sample-G-0.2-aliquot5" },
              { "@id": "#sample/sample-S-0.2-aliquot8" },
              { "@id": "#sample/sample-E-0.1-aliquot5" },
              { "@id": "#sample/sample-N-0.07-aliquot5" },
              { "@id": "#sample/sample-P-0.2-aliquot8" },
              { "@id": "#sample/sample-P-0.2-aliquot6" },
              { "@id": "#sample/sample-E-0.1-aliquot1" },
              { "@id": "#sample/sample-G-0.07-aliquot5" },
              { "@id": "#sample/sample-C-0.07-aliquot7" },
              { "@id": "#sample/sample-P-0.07-aliquot7" },
              { "@id": "#sample/sample-C-0.1-aliquot8" },
              { "@id": "#sample/sample-S-0.2-aliquot6" },
              { "@id": "#sample/sample-C-0.1-aliquot9" },
              { "@id": "#sample/sample-E-0.07-aliquot6" },
              { "@id": "#sample/sample-P-0.07-aliquot6" },
              { "@id": "#sample/sample-G-0.2-aliquot1" },
              { "@id": "#sample/sample-S-0.1-aliquot6" },
              { "@id": "#sample/sample-G-0.07-aliquot2" },
              { "@id": "#sample/sample-S-0.07-aliquot6" },
              { "@id": "#sample/sample-E-0.1-aliquot2" },
              { "@id": "#sample/sample-N-0.2-aliquot8" },
              { "@id": "#sample/sample-C-0.1-aliquot10" },
              { "@id": "#sample/sample-N-0.2-aliquot10" },
              { "@id": "#sample/sample-P-0.2-aliquot9" },
              { "@id": "#sample/sample-N-0.2-aliquot6" },
              { "@id": "#sample/sample-N-0.2-aliquot5" },
              { "@id": "#sample/sample-N-0.07-aliquot10" },
              { "@id": "#sample/sample-C-0.1-aliquot7" },
              { "@id": "#sample/sample-C-0.07-aliquot6" },
              { "@id": "#sample/sample-C-0.2-aliquot10" }
            ]
          },
          "technologyPlatform": "LC-MS/MS",
          "filename": "a_metabolome.txt"
        },
        {
          "measurementType": {
            "termAccession": "http://purl.obolibrary.org/obo/OBI_0000424",
            "termSource": "OBI",
            "annotationValue": "transcription profiling"
          },
          "dataFiles": [
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220982.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331220982.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222701.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331222701.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219131.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331219131.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222215.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331222215.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222054.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331222054.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222534.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331222534.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220431.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331220431.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331225235.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331225235.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/derivedarraydatafile-E-MEXP-115-processed-data-1341986893.txt",
              "comments": [],
              "name": "E-MEXP-115-processed-data-1341986893.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331224301.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331224301.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220272.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331220272.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219013.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331219013.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331224703.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331224703.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220784.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331220784.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331221668.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331221668.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220607.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331220607.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223667.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331223667.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219361.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331219361.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222917.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331222917.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331224884.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331224884.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331218449.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331218449.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331218116.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331218116.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331221345.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331221345.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223977.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331223977.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331221518.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331221518.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220090.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331220090.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331217737.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331217737.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331218271.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331218271.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219767.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331219767.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331221148.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331221148.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331218681.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331218681.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223835.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331223835.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331217979.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331217979.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219490.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331219490.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331225097.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331225097.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331224480.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331224480.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223501.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331223501.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222380.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331222380.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219634.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331219634.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331218842.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331218842.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331217860.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331217860.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219914.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331219914.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223115.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331223115.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223321.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331223321.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331217580.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331217580.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219245.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331219245.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331224145.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331224145.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331221873.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331221873.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331225401.txt",
              "comments": [],
              "name": "E-MEXP-115-raw-data-331225401.txt",
              "type": "Raw Data File"
            }
          ],
          "technologyType": {
            "termAccession": "http://purl.obolibrary.org/obo/OBI_0400148",
            "termSource": "OBI",
            "annotationValue": "DNA microarray"
          },
          "processSequence": [
            {
              "outputs": [ { "@id": "#material/labeledextract-P-0.07-aliquot2" } ],
              "inputs": [ { "@id": "#material/extract-P-0.07-aliquot2" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3934" },
              "@id": "#process/biotin_labeling26",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction26" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3930",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3930" },
              "@id": "#process/HYB:MEXP:3930",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-N-0.2-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling21" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.1-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.1-aliquot2" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction42",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling42" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220784.txt" } ],
              "name": "SCAN:MEXP:3928",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3928",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3928" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331218271.txt" } ],
              "name": "SCAN:MEXP:3912",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3912",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3912" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.2-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.2-aliquot1" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction9",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling9" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.1-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.1-aliquot1" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction29",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling29" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.2-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.2-aliquot3" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction47",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling47" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223501.txt" } ],
              "name": "SCAN:MEXP:3943",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3943",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3943" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.1-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.1-aliquot1" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction17",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling17" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3911",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3911" },
              "@id": "#process/HYB:MEXP:3911",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-C-0.1-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling8" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331217860.txt" } ],
              "name": "SCAN:MEXP:3909",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3909",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3909" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331218449.txt" } ],
              "name": "SCAN:MEXP:3913",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3913",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3913" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3929",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3929" },
              "@id": "#process/HYB:MEXP:3929",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-N-0.2-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling22" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3925",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3925" },
              "@id": "#process/HYB:MEXP:3925",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-N-0.1-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling19" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3919",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3919" },
              "@id": "#process/HYB:MEXP:3919",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-N-0.07-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling13" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.1-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.1-aliquot2" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction6",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling6" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-C-0.1-aliquot3" } ],
              "inputs": [ { "@id": "#material/extract-C-0.1-aliquot3" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3914" },
              "@id": "#process/biotin_labeling7",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction7" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-S-0.2-aliquot1" } ],
              "inputs": [ { "@id": "#material/extract-S-0.2-aliquot1" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3951" },
              "@id": "#process/biotin_labeling45",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction45" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331224145.txt" } ],
              "name": "SCAN:MEXP:3947",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3947",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3947" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-S-0.2-aliquot4" } ],
              "inputs": [ { "@id": "#material/extract-S-0.2-aliquot4" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3953" },
              "@id": "#process/biotin_labeling48",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction48" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331225401.txt" } ],
              "name": "SCAN:MEXP:3954",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3954",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3954" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.1-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.1-aliquot2" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction30",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling30" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.1-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.1-aliquot4" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction44",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling44" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3918",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3918" },
              "@id": "#process/HYB:MEXP:3918",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-C-0.2-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling11" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3921",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3921" },
              "@id": "#process/HYB:MEXP:3921",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-N-0.07-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling14" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.07-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.07-aliquot3" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction3",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling3" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.2-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.2-aliquot3" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction11",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling11" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-P-0.2-aliquot2" } ],
              "inputs": [ { "@id": "#material/extract-P-0.2-aliquot2" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3941" },
              "@id": "#process/biotin_labeling34",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction34" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3944",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3944" },
              "@id": "#process/HYB:MEXP:3944",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-S-0.07-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling37" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.2-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.2-aliquot2" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction34",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling34" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.07-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.07-aliquot1" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction37",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling37" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-S-0.07-aliquot1" } ],
              "inputs": [ { "@id": "#material/extract-S-0.07-aliquot1" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3944" },
              "@id": "#process/biotin_labeling37",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction37" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-S-0.1-aliquot2" } ],
              "inputs": [ { "@id": "#material/extract-S-0.1-aliquot2" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3947" },
              "@id": "#process/biotin_labeling42",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction42" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3926",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3926" },
              "@id": "#process/HYB:MEXP:3926",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-N-0.1-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling17" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-S-0.07-aliquot3" } ],
              "inputs": [ { "@id": "#material/extract-S-0.07-aliquot3" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3946" },
              "@id": "#process/biotin_labeling39",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction39" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.2-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.2-aliquot4" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction12",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling12" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3922",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3922" },
              "@id": "#process/HYB:MEXP:3922",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-N-0.07-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling15" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331225097.txt" } ],
              "name": "SCAN:MEXP:3952",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3952",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3952" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331221518.txt" } ],
              "name": "SCAN:MEXP:3932",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3932",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3932" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3910",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3910" },
              "@id": "#process/HYB:MEXP:3910",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-C-0.07-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling3" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-P-0.1-aliquot4" } ],
              "inputs": [ { "@id": "#material/extract-P-0.1-aliquot4" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3936" },
              "@id": "#process/biotin_labeling32",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction32" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-C-0.07-aliquot2" } ],
              "inputs": [ { "@id": "#material/extract-C-0.07-aliquot2" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3909" },
              "@id": "#process/biotin_labeling2",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction2" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-S-0.1-aliquot4" } ],
              "inputs": [ { "@id": "#material/extract-S-0.1-aliquot4" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3949" },
              "@id": "#process/biotin_labeling44",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction44" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-C-0.1-aliquot4" } ],
              "inputs": [ { "@id": "#material/extract-C-0.1-aliquot4" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3911" },
              "@id": "#process/biotin_labeling8",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction8" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-N-0.1-aliquot4" } ],
              "inputs": [ { "@id": "#material/extract-N-0.1-aliquot4" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3924" },
              "@id": "#process/biotin_labeling20",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction20" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3946",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3946" },
              "@id": "#process/HYB:MEXP:3946",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-S-0.07-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling39" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219767.txt" } ],
              "name": "SCAN:MEXP:3922",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3922",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3922" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-S-0.07-aliquot2" } ],
              "inputs": [ { "@id": "#material/extract-S-0.07-aliquot2" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3945" },
              "@id": "#process/biotin_labeling38",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction38" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3907",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3907" },
              "@id": "#process/HYB:MEXP:3907",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-C-0.07-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling4" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-N-0.07-aliquot4" } ],
              "inputs": [ { "@id": "#material/extract-N-0.07-aliquot4" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3920" },
              "@id": "#process/biotin_labeling16",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction16" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.2-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.2-aliquot1" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction33",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling33" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3920",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3920" },
              "@id": "#process/HYB:MEXP:3920",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-N-0.07-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling16" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220090.txt" } ],
              "name": "SCAN:MEXP:3924",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3924",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3924" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.2-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.2-aliquot4" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction48",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling48" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219013.txt" } ],
              "name": "SCAN:MEXP:3916",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3916",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3916" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.2-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.2-aliquot1" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction45",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling45" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-P-0.07-aliquot4" } ],
              "inputs": [ { "@id": "#material/extract-P-0.07-aliquot4" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3933" },
              "@id": "#process/biotin_labeling28",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction28" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3923",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3923" },
              "@id": "#process/HYB:MEXP:3923",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-N-0.1-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling18" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-P-0.1-aliquot3" } ],
              "inputs": [ { "@id": "#material/extract-P-0.1-aliquot3" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3937" },
              "@id": "#process/biotin_labeling31",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction31" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.1-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.1-aliquot1" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction5",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling5" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3912",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3912" },
              "@id": "#process/HYB:MEXP:3912",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-C-0.1-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling5" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.07-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.07-aliquot4" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction40",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling40" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.1-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.1-aliquot2" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction18",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling18" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331225235.txt" } ],
              "name": "SCAN:MEXP:3953",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3953",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3953" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219914.txt" } ],
              "name": "SCAN:MEXP:3923",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3923",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3923" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-C-0.1-aliquot1" } ],
              "inputs": [ { "@id": "#material/extract-C-0.1-aliquot1" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3912" },
              "@id": "#process/biotin_labeling5",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction5" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3936",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3936" },
              "@id": "#process/HYB:MEXP:3936",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-P-0.1-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling32" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-C-0.2-aliquot3" } ],
              "inputs": [ { "@id": "#material/extract-C-0.2-aliquot3" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3918" },
              "@id": "#process/biotin_labeling11",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction11" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.07-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.07-aliquot1" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction1",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3913",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3913" },
              "@id": "#process/HYB:MEXP:3913",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-C-0.1-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling6" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.07-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.07-aliquot4" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction16",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling16" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.1-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.1-aliquot3" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction43",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling43" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3952",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3952" },
              "@id": "#process/HYB:MEXP:3952",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-S-0.2-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling47" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222917.txt" } ],
              "name": "SCAN:MEXP:3940",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3940",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3940" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-P-0.07-aliquot3" } ],
              "inputs": [ { "@id": "#material/extract-P-0.07-aliquot3" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3931" },
              "@id": "#process/biotin_labeling27",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction27" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-N-0.07-aliquot2" } ],
              "inputs": [ { "@id": "#material/extract-N-0.07-aliquot2" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3921" },
              "@id": "#process/biotin_labeling14",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction14" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-N-0.07-aliquot3" } ],
              "inputs": [ { "@id": "#material/extract-N-0.07-aliquot3" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3922" },
              "@id": "#process/biotin_labeling15",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction15" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.07-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.07-aliquot3" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction39",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling39" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331217979.txt" } ],
              "name": "SCAN:MEXP:3910",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3910",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3910" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-P-0.2-aliquot3" } ],
              "inputs": [ { "@id": "#material/extract-P-0.2-aliquot3" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3942" },
              "@id": "#process/biotin_labeling35",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction35" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223835.txt" } ],
              "name": "SCAN:MEXP:3945",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3945",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3945" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-C-0.1-aliquot2" } ],
              "inputs": [ { "@id": "#material/extract-C-0.1-aliquot2" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3913" },
              "@id": "#process/biotin_labeling6",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction6" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220272.txt" } ],
              "name": "SCAN:MEXP:3925",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3925",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3925" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.2-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.2-aliquot3" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction23",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling23" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3942",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3942" },
              "@id": "#process/HYB:MEXP:3942",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-P-0.2-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling35" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3938",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3938" },
              "@id": "#process/HYB:MEXP:3938",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-P-0.1-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling29" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331221668.txt" } ],
              "name": "SCAN:MEXP:3933",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3933",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3933" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.2-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.2-aliquot2" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction22",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling22" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3908",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3908" },
              "@id": "#process/HYB:MEXP:3908",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-C-0.07-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling1" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3928",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3928" },
              "@id": "#process/HYB:MEXP:3928",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-N-0.2-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling23" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-C-0.07-aliquot4" } ],
              "inputs": [ { "@id": "#material/extract-C-0.07-aliquot4" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3907" },
              "@id": "#process/biotin_labeling4",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction4" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220982.txt" } ],
              "name": "SCAN:MEXP:3929",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3929",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3929" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.1-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.1-aliquot3" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction7",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling7" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/derivedarraydatafile-E-MEXP-115-processed-data-1341986893.txt" } ],
              "name": "GCRMA normalization",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/GCRMA_normalization",
              "parameterValues": [],
              "inputs": [
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331217737.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331217860.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331217979.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331217580.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331218271.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331218449.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331218681.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331218116.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219013.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331218842.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219245.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219131.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219361.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219634.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219767.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219490.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220431.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219914.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220272.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220090.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331221148.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220982.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220784.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220607.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331221518.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331221873.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331221345.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331221668.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222534.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222054.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222380.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222215.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222917.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223115.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223321.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222701.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223667.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223835.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223977.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223501.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331224301.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331224145.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331224703.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331224480.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331224884.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331225401.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331225097.txt" },
                { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331225235.txt" }
              ],
              "comments": [],
              "previousProcess": { "@id": "#process/SCAN:MEXP:3953" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3939",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3939" },
              "@id": "#process/HYB:MEXP:3939",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-P-0.2-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling36" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331224301.txt" } ],
              "name": "SCAN:MEXP:3948",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3948",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3948" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-P-0.2-aliquot1" } ],
              "inputs": [ { "@id": "#material/extract-P-0.2-aliquot1" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3940" },
              "@id": "#process/biotin_labeling33",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction33" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331218681.txt" } ],
              "name": "SCAN:MEXP:3914",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3914",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3914" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220607.txt" } ],
              "name": "SCAN:MEXP:3927",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3927",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3927" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-S-0.2-aliquot3" } ],
              "inputs": [ { "@id": "#material/extract-S-0.2-aliquot3" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3952" },
              "@id": "#process/biotin_labeling47",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction47" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.2-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.2-aliquot1" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction21",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling21" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222701.txt" } ],
              "name": "SCAN:MEXP:3939",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3939",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3939" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.1-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.1-aliquot1" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction41",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling41" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.07-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.07-aliquot1" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction25",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling25" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3932",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3932" },
              "@id": "#process/HYB:MEXP:3932",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-P-0.07-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling25" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.07-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.07-aliquot2" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction2",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling2" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.1-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.1-aliquot3" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction31",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling31" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-C-0.07-aliquot3" } ],
              "inputs": [ { "@id": "#material/extract-C-0.07-aliquot3" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3910" },
              "@id": "#process/biotin_labeling3",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction3" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-S-0.1-aliquot3" } ],
              "inputs": [ { "@id": "#material/extract-S-0.1-aliquot3" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3950" },
              "@id": "#process/biotin_labeling43",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction43" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219634.txt" } ],
              "name": "SCAN:MEXP:3921",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3921",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3921" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331218116.txt" } ],
              "name": "SCAN:MEXP:3911",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3911",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3911" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3943",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3943" },
              "@id": "#process/HYB:MEXP:3943",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-S-0.07-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling40" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331220431.txt" } ],
              "name": "SCAN:MEXP:3926",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3926",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3926" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-P-0.2-aliquot4" } ],
              "inputs": [ { "@id": "#material/extract-P-0.2-aliquot4" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3939" },
              "@id": "#process/biotin_labeling36",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction36" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3948",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3948" },
              "@id": "#process/HYB:MEXP:3948",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-S-0.1-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling41" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331221345.txt" } ],
              "name": "SCAN:MEXP:3931",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3931",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3931" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219490.txt" } ],
              "name": "SCAN:MEXP:3920",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3920",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3920" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3915",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3915" },
              "@id": "#process/HYB:MEXP:3915",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-C-0.2-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling10" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-S-0.07-aliquot4" } ],
              "inputs": [ { "@id": "#material/extract-S-0.07-aliquot4" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3943" },
              "@id": "#process/biotin_labeling40",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction40" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3916",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3916" },
              "@id": "#process/HYB:MEXP:3916",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-C-0.2-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling9" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-N-0.2-aliquot1" } ],
              "inputs": [ { "@id": "#material/extract-N-0.2-aliquot1" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3930" },
              "@id": "#process/biotin_labeling21",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction21" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3935",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3935" },
              "@id": "#process/HYB:MEXP:3935",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-P-0.1-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling30" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.1-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.1-aliquot4" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction32",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling32" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.07-aliquot1" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.07-aliquot1" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction13",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling13" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3953",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3953" },
              "@id": "#process/HYB:MEXP:3953",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-S-0.2-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling48" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331224480.txt" } ],
              "name": "SCAN:MEXP:3949",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3949",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3949" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.2-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.2-aliquot4" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction24",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling24" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.07-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.07-aliquot3" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction15",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling15" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222534.txt" } ],
              "name": "SCAN:MEXP:3938",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3938",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3938" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3917",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3917" },
              "@id": "#process/HYB:MEXP:3917",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-C-0.2-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling12" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222054.txt" } ],
              "name": "SCAN:MEXP:3935",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3935",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3935" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-P-0.1-aliquot2" } ],
              "inputs": [ { "@id": "#material/extract-P-0.1-aliquot2" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3935" },
              "@id": "#process/biotin_labeling30",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction30" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-N-0.1-aliquot1" } ],
              "inputs": [ { "@id": "#material/extract-N-0.1-aliquot1" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3926" },
              "@id": "#process/biotin_labeling17",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction17" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.07-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.07-aliquot2" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction26",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling26" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-C-0.2-aliquot4" } ],
              "inputs": [ { "@id": "#material/extract-C-0.2-aliquot4" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3917" },
              "@id": "#process/biotin_labeling12",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction12" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-C-0.2-aliquot2" } ],
              "inputs": [ { "@id": "#material/extract-C-0.2-aliquot2" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3915" },
              "@id": "#process/biotin_labeling10",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction10" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331224884.txt" } ],
              "name": "SCAN:MEXP:3951",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3951",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3951" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.07-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.07-aliquot4" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction28",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling28" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223115.txt" } ],
              "name": "SCAN:MEXP:3941",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3941",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3941" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-S-0.2-aliquot2" } ],
              "inputs": [ { "@id": "#material/extract-S-0.2-aliquot2" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3954" },
              "@id": "#process/biotin_labeling46",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction46" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3950",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3950" },
              "@id": "#process/HYB:MEXP:3950",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-S-0.1-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling43" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-N-0.2-aliquot2" } ],
              "inputs": [ { "@id": "#material/extract-N-0.2-aliquot2" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3929" },
              "@id": "#process/biotin_labeling22",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction22" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331224703.txt" } ],
              "name": "SCAN:MEXP:3950",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3950",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3950" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219131.txt" } ],
              "name": "SCAN:MEXP:3917",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3917",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3917" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3933",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3933" },
              "@id": "#process/HYB:MEXP:3933",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-P-0.07-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling28" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-N-0.1-aliquot3" } ],
              "inputs": [ { "@id": "#material/extract-N-0.1-aliquot3" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3925" },
              "@id": "#process/biotin_labeling19",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction19" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3941",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3941" },
              "@id": "#process/HYB:MEXP:3941",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-P-0.2-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling34" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219245.txt" } ],
              "name": "SCAN:MEXP:3918",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3918",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3918" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3927",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3927" },
              "@id": "#process/HYB:MEXP:3927",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-N-0.2-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling24" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3914",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3914" },
              "@id": "#process/HYB:MEXP:3914",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-C-0.1-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling7" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3940",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3940" },
              "@id": "#process/HYB:MEXP:3940",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-P-0.2-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling33" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-N-0.1-aliquot2" } ],
              "inputs": [ { "@id": "#material/extract-N-0.1-aliquot2" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3923" },
              "@id": "#process/biotin_labeling18",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction18" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.07-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.07-aliquot3" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction27",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling27" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3924",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3924" },
              "@id": "#process/HYB:MEXP:3924",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-N-0.1-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling20" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.1-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.1-aliquot4" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction8",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling8" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3949",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3949" },
              "@id": "#process/HYB:MEXP:3949",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-S-0.1-aliquot4" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling44" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.2-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.2-aliquot2" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction10",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling10" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3954",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3954" },
              "@id": "#process/HYB:MEXP:3954",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-S-0.2-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling46" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222380.txt" } ],
              "name": "SCAN:MEXP:3937",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3937",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3937" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223667.txt" } ],
              "name": "SCAN:MEXP:3944",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3944",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3944" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223321.txt" } ],
              "name": "SCAN:MEXP:3942",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3942",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3942" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-C-0.07-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-C-0.07-aliquot4" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction4",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling4" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331221873.txt" } ],
              "name": "SCAN:MEXP:3934",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3934",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3934" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331218842.txt" } ],
              "name": "SCAN:MEXP:3915",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3915",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3915" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.1-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.1-aliquot3" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction19",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling19" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331219361.txt" } ],
              "name": "SCAN:MEXP:3919",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3919",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3919" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331222215.txt" } ],
              "name": "SCAN:MEXP:3936",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3936",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3936" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.2-aliquot3" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.2-aliquot3" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction35",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling35" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-P-0.2-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-P-0.2-aliquot4" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction36",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling36" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.2-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.2-aliquot2" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction46",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling46" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3947",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3947" },
              "@id": "#process/HYB:MEXP:3947",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-S-0.1-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling42" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3937",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3937" },
              "@id": "#process/HYB:MEXP:3937",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-P-0.1-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling31" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3934",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3934" },
              "@id": "#process/HYB:MEXP:3934",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-P-0.07-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling26" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-C-0.2-aliquot1" } ],
              "inputs": [ { "@id": "#material/extract-C-0.2-aliquot1" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3916" },
              "@id": "#process/biotin_labeling9",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction9" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-S-0.1-aliquot1" } ],
              "inputs": [ { "@id": "#material/extract-S-0.1-aliquot1" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3948" },
              "@id": "#process/biotin_labeling41",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction41" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331223977.txt" } ],
              "name": "SCAN:MEXP:3946",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3946",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3946" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3951",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3951" },
              "@id": "#process/HYB:MEXP:3951",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-S-0.2-aliquot1" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling45" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-N-0.07-aliquot1" } ],
              "inputs": [ { "@id": "#material/extract-N-0.07-aliquot1" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3919" },
              "@id": "#process/biotin_labeling13",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction13" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3909",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3909" },
              "@id": "#process/HYB:MEXP:3909",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-C-0.07-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling2" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.1-aliquot4" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.1-aliquot4" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction20",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling20" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3931",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3931" },
              "@id": "#process/HYB:MEXP:3931",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-P-0.07-aliquot3" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling27" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/extract-N-0.07-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-N-0.07-aliquot2" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction14",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling14" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331221148.txt" } ],
              "name": "SCAN:MEXP:3930",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3930",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3930" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331217737.txt" } ],
              "name": "SCAN:MEXP:3908",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3908",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3908" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-C-0.07-aliquot1" } ],
              "inputs": [ { "@id": "#material/extract-C-0.07-aliquot1" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3908" },
              "@id": "#process/biotin_labeling1",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-P-0.1-aliquot1" } ],
              "inputs": [ { "@id": "#material/extract-P-0.1-aliquot1" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3938" },
              "@id": "#process/biotin_labeling29",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction29" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/extract-S-0.07-aliquot2" } ],
              "inputs": [ { "@id": "#sample/sample-S-0.07-aliquot2" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction38",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling38" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-P-0.07-aliquot1" } ],
              "inputs": [ { "@id": "#material/extract-P-0.07-aliquot1" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3932" },
              "@id": "#process/biotin_labeling25",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction25" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-N-0.2-aliquot4" } ],
              "inputs": [ { "@id": "#material/extract-N-0.2-aliquot4" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3927" },
              "@id": "#process/biotin_labeling24",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction24" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MEXP-115-raw-data-331217580.txt" } ],
              "name": "SCAN:MEXP:3907",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/GCRMA_normalization" },
              "@id": "#process/SCAN:MEXP:3907",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/HYB:MEXP:3907" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "HYB:MEXP:3945",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/SCAN:MEXP:3945" },
              "@id": "#process/HYB:MEXP:3945",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-S-0.07-aliquot2" } ],
              "comments": [],
              "previousProcess": { "@id": "#process/biotin_labeling38" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-N-0.2-aliquot3" } ],
              "inputs": [ { "@id": "#material/extract-N-0.2-aliquot3" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/HYB:MEXP:3928" },
              "@id": "#process/biotin_labeling23",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction23" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            }
          ],
          "@id": "#assay/a_transcriptome.txt",
          "unitCategories": [],
          "characteristicCategories": [],
          "materials": {
            "otherMaterials": [
              {
                "@id": "#material/extract-N-0.07-aliquot2",
                "characteristics": [],
                "name": "extract-N-0.07-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-N-0.1-aliquot2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-N-0.1-aliquot2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-C-0.2-aliquot3",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-C-0.2-aliquot3",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-S-0.2-aliquot3",
                "characteristics": [],
                "name": "extract-S-0.2-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-C-0.07-aliquot4",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-C-0.07-aliquot4",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-N-0.07-aliquot1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-N-0.07-aliquot1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-C-0.2-aliquot1",
                "characteristics": [],
                "name": "extract-C-0.2-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-S-0.2-aliquot3",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-S-0.2-aliquot3",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-N-0.2-aliquot1",
                "characteristics": [],
                "name": "extract-N-0.2-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-S-0.2-aliquot1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-S-0.2-aliquot1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-S-0.2-aliquot1",
                "characteristics": [],
                "name": "extract-S-0.2-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.1-aliquot2",
                "characteristics": [],
                "name": "extract-P-0.1-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.1-aliquot4",
                "characteristics": [],
                "name": "extract-P-0.1-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-S-0.07-aliquot1",
                "characteristics": [],
                "name": "extract-S-0.07-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.07-aliquot3",
                "characteristics": [],
                "name": "extract-P-0.07-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-P-0.2-aliquot4",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-P-0.2-aliquot4",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-P-0.07-aliquot2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-P-0.07-aliquot2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-P-0.07-aliquot3",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-P-0.07-aliquot3",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-C-0.07-aliquot2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-C-0.07-aliquot2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-P-0.2-aliquot2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-P-0.2-aliquot2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-S-0.1-aliquot4",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-S-0.1-aliquot4",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-P-0.2-aliquot3",
                "characteristics": [],
                "name": "extract-P-0.2-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-P-0.1-aliquot2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-P-0.1-aliquot2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-N-0.07-aliquot3",
                "characteristics": [],
                "name": "extract-N-0.07-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.07-aliquot4",
                "characteristics": [],
                "name": "extract-C-0.07-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.07-aliquot4",
                "characteristics": [],
                "name": "extract-N-0.07-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-S-0.07-aliquot4",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-S-0.07-aliquot4",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-C-0.1-aliquot1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-C-0.1-aliquot1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-P-0.07-aliquot2",
                "characteristics": [],
                "name": "extract-P-0.07-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-S-0.1-aliquot3",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-S-0.1-aliquot3",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-S-0.07-aliquot2",
                "characteristics": [],
                "name": "extract-S-0.07-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.1-aliquot1",
                "characteristics": [],
                "name": "extract-P-0.1-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-C-0.1-aliquot4",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-C-0.1-aliquot4",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-C-0.1-aliquot2",
                "characteristics": [],
                "name": "extract-C-0.1-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-S-0.2-aliquot2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-S-0.2-aliquot2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-P-0.1-aliquot3",
                "characteristics": [],
                "name": "extract-P-0.1-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-P-0.2-aliquot1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-P-0.2-aliquot1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-N-0.07-aliquot2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-N-0.07-aliquot2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-C-0.2-aliquot4",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-C-0.2-aliquot4",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-P-0.2-aliquot3",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-P-0.2-aliquot3",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-P-0.07-aliquot1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-P-0.07-aliquot1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-P-0.1-aliquot1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-P-0.1-aliquot1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-S-0.07-aliquot1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-S-0.07-aliquot1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-N-0.1-aliquot4",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-N-0.1-aliquot4",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-C-0.2-aliquot4",
                "characteristics": [],
                "name": "extract-C-0.2-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.2-aliquot3",
                "characteristics": [],
                "name": "extract-C-0.2-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.07-aliquot1",
                "characteristics": [],
                "name": "extract-N-0.07-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.07-aliquot1",
                "characteristics": [],
                "name": "extract-C-0.07-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.1-aliquot1",
                "characteristics": [],
                "name": "extract-N-0.1-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-S-0.07-aliquot3",
                "characteristics": [],
                "name": "extract-S-0.07-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-C-0.1-aliquot2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-C-0.1-aliquot2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-S-0.1-aliquot2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-S-0.1-aliquot2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-N-0.2-aliquot1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-N-0.2-aliquot1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-C-0.07-aliquot2",
                "characteristics": [],
                "name": "extract-C-0.07-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-C-0.2-aliquot1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-C-0.2-aliquot1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-N-0.1-aliquot4",
                "characteristics": [],
                "name": "extract-N-0.1-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-N-0.2-aliquot3",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-N-0.2-aliquot3",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-S-0.07-aliquot4",
                "characteristics": [],
                "name": "extract-S-0.07-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-C-0.2-aliquot2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-C-0.2-aliquot2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-C-0.1-aliquot1",
                "characteristics": [],
                "name": "extract-C-0.1-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.1-aliquot3",
                "characteristics": [],
                "name": "extract-N-0.1-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-N-0.2-aliquot2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-N-0.2-aliquot2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-S-0.1-aliquot4",
                "characteristics": [],
                "name": "extract-S-0.1-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.2-aliquot2",
                "characteristics": [],
                "name": "extract-C-0.2-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-N-0.1-aliquot1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-N-0.1-aliquot1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-C-0.1-aliquot3",
                "characteristics": [],
                "name": "extract-C-0.1-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-S-0.1-aliquot2",
                "characteristics": [],
                "name": "extract-S-0.1-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-S-0.1-aliquot1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-S-0.1-aliquot1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-N-0.07-aliquot4",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-N-0.07-aliquot4",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-P-0.2-aliquot4",
                "characteristics": [],
                "name": "extract-P-0.2-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-P-0.07-aliquot4",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-P-0.07-aliquot4",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-S-0.1-aliquot1",
                "characteristics": [],
                "name": "extract-S-0.1-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.2-aliquot3",
                "characteristics": [],
                "name": "extract-N-0.2-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-C-0.07-aliquot3",
                "characteristics": [],
                "name": "extract-C-0.07-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-S-0.2-aliquot4",
                "characteristics": [],
                "name": "extract-S-0.2-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-P-0.1-aliquot3",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-P-0.1-aliquot3",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-N-0.1-aliquot2",
                "characteristics": [],
                "name": "extract-N-0.1-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.2-aliquot2",
                "characteristics": [],
                "name": "extract-P-0.2-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-S-0.2-aliquot4",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-S-0.2-aliquot4",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-S-0.07-aliquot2",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-S-0.07-aliquot2",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-S-0.1-aliquot3",
                "characteristics": [],
                "name": "extract-S-0.1-aliquot3",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-P-0.1-aliquot4",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-P-0.1-aliquot4",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-C-0.07-aliquot3",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-C-0.07-aliquot3",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-N-0.2-aliquot4",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-N-0.2-aliquot4",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-P-0.2-aliquot1",
                "characteristics": [],
                "name": "extract-P-0.2-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.07-aliquot1",
                "characteristics": [],
                "name": "extract-P-0.07-aliquot1",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-C-0.1-aliquot3",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-C-0.1-aliquot3",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-S-0.2-aliquot2",
                "characteristics": [],
                "name": "extract-S-0.2-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-C-0.07-aliquot1",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-C-0.07-aliquot1",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-C-0.1-aliquot4",
                "characteristics": [],
                "name": "extract-C-0.1-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.2-aliquot2",
                "characteristics": [],
                "name": "extract-N-0.2-aliquot2",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-N-0.2-aliquot4",
                "characteristics": [],
                "name": "extract-N-0.2-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-P-0.07-aliquot4",
                "characteristics": [],
                "name": "extract-P-0.07-aliquot4",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-N-0.07-aliquot3",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-N-0.07-aliquot3",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-N-0.1-aliquot3",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-N-0.1-aliquot3",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-S-0.07-aliquot3",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "termAccession": "http://purl.obolibrary.org/obo/CHEBI_15956",
                      "termSource": "CHEBI",
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-S-0.07-aliquot3",
                "type": "Labeled Extract Name"
              }
            ],
            "samples": [
              { "@id": "#sample/sample-P-0.07-aliquot4" },
              { "@id": "#sample/sample-C-0.2-aliquot3" },
              { "@id": "#sample/sample-C-0.07-aliquot2" },
              { "@id": "#sample/sample-N-0.07-aliquot1" },
              { "@id": "#sample/sample-P-0.07-aliquot3" },
              { "@id": "#sample/sample-S-0.1-aliquot3" },
              { "@id": "#sample/sample-N-0.1-aliquot4" },
              { "@id": "#sample/sample-N-0.2-aliquot4" },
              { "@id": "#sample/sample-N-0.2-aliquot1" },
              { "@id": "#sample/sample-P-0.2-aliquot3" },
              { "@id": "#sample/sample-C-0.2-aliquot1" },
              { "@id": "#sample/sample-S-0.1-aliquot2" },
              { "@id": "#sample/sample-P-0.1-aliquot2" },
              { "@id": "#sample/sample-N-0.2-aliquot3" },
              { "@id": "#sample/sample-N-0.1-aliquot2" },
              { "@id": "#sample/sample-S-0.1-aliquot1" },
              { "@id": "#sample/sample-P-0.07-aliquot2" },
              { "@id": "#sample/sample-C-0.07-aliquot3" },
              { "@id": "#sample/sample-N-0.1-aliquot3" },
              { "@id": "#sample/sample-C-0.1-aliquot4" },
              { "@id": "#sample/sample-N-0.1-aliquot1" },
              { "@id": "#sample/sample-N-0.07-aliquot2" },
              { "@id": "#sample/sample-P-0.1-aliquot3" },
              { "@id": "#sample/sample-S-0.2-aliquot4" },
              { "@id": "#sample/sample-C-0.2-aliquot4" },
              { "@id": "#sample/sample-S-0.2-aliquot2" },
              { "@id": "#sample/sample-N-0.2-aliquot2" },
              { "@id": "#sample/sample-C-0.1-aliquot3" },
              { "@id": "#sample/sample-S-0.07-aliquot3" },
              { "@id": "#sample/sample-S-0.2-aliquot1" },
              { "@id": "#sample/sample-S-0.07-aliquot1" },
              { "@id": "#sample/sample-C-0.1-aliquot2" },
              { "@id": "#sample/sample-C-0.07-aliquot4" },
              { "@id": "#sample/sample-P-0.1-aliquot1" },
              { "@id": "#sample/sample-P-0.2-aliquot1" },
              { "@id": "#sample/sample-C-0.2-aliquot2" },
              { "@id": "#sample/sample-P-0.1-aliquot4" },
              { "@id": "#sample/sample-S-0.07-aliquot4" },
              { "@id": "#sample/sample-S-0.1-aliquot4" },
              { "@id": "#sample/sample-P-0.2-aliquot2" },
              { "@id": "#sample/sample-N-0.07-aliquot3" },
              { "@id": "#sample/sample-N-0.07-aliquot4" },
              { "@id": "#sample/sample-S-0.2-aliquot3" },
              { "@id": "#sample/sample-C-0.07-aliquot1" },
              { "@id": "#sample/sample-C-0.1-aliquot1" },
              { "@id": "#sample/sample-P-0.07-aliquot1" },
              { "@id": "#sample/sample-P-0.2-aliquot4" },
              { "@id": "#sample/sample-S-0.07-aliquot2" }
            ]
          },
          "technologyPlatform": "Affymetrix",
          "filename": "a_transcriptome.txt"
        }
      ],
      "filename": "s_BII-S-1.txt",
      "factors": [
        {
          "@id": "#factor/limiting_nutrient",
          "factorType": {
            "annotationValue": "chemical compound"
          },
          "factorName": "limiting nutrient"
        },
        {
          "@id": "#factor/rate",
          "factorType": {
            "termAccession": "http://purl.obolibrary.org/obo/PATO_0000161",
            "termSource": "PATO",
            "annotationValue": "rate"
          },
          "factorName": "rate"
        }
      ],
      "publications": [
        {
          "doi": "doi:10.1186/jbiol54",
          "pubMedID": "17439666",
          "status": {
            "annotationValue": "published"
          },
          "title": "Growth control of the eukaryote cell: a systems biology study in yeast.",
          "authorList": "Castrillo JI, Zeef LA, Hoyle DC, Zhang N, Hayes A, Gardner DC, Cornell MJ, Petty J, Hakes L, Wardleworth L, Rash B, Brown M, Dunn WB, Broadhurst D, O'Donoghue K, Hester SS, Dunkley TP, Hart SR, Swainston N, Li P, Gaskell SJ, Paton NW, Lilley KS, Kell DB, Oliver SG."
        }
      ],
      "@id": "#study/BII-S-1",
      "materials": {
        "otherMaterials": [],
        "samples": [
          {
            "@id": "#sample/sample-P-0.1-aliquot7",
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
          },
          {
            "@id": "#sample/sample-E-0.1-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture14" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
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
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-E-0.1-aliquot3"
          },
          {
            "@id": "#sample/sample-P-0.07-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture7" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-P-0.07-aliquot4"
          },
          {
            "@id": "#sample/sample-N-0.07-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture4" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-N-0.07-aliquot7"
          },
          {
            "@id": "#sample/sample-P-0.2-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture9" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-P-0.2-aliquot4"
          },
          {
            "@id": "#sample/sample-C-0.2-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture3" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-C-0.2-aliquot4"
          },
          {
            "@id": "#sample/sample-N-0.1-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture5" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
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
            "name": "sample-N-0.1-aliquot5"
          },
          {
            "@id": "#sample/sample-C-0.2-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture3" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-C-0.2-aliquot3"
          },
          {
            "@id": "#sample/sample-N-0.07-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture4" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-N-0.07-aliquot6"
          },
          {
            "@id": "#sample/sample-E-0.1-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture14" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
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
            "name": "sample-E-0.1-aliquot6"
          },
          {
            "@id": "#sample/sample-G-0.07-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture16" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-G-0.07-aliquot1"
          },
          {
            "@id": "#sample/sample-S-0.2-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture12" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.2-aliquot2"
          },
          {
            "@id": "#sample/sample-C-0.2-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture3" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
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
            "name": "sample-C-0.2-aliquot5"
          },
          {
            "@id": "#sample/sample-P-0.2-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture9" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
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
            "name": "sample-P-0.2-aliquot5"
          },
          {
            "@id": "#sample/sample-G-0.2-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture18" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-G-0.2-aliquot6"
          },
          {
            "@id": "#sample/sample-N-0.1-aliquot11",
            "derivesFrom": [ { "@id": "#source/source-culture5" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-N-0.1-aliquot11"
          },
          {
            "@id": "#sample/sample-E-0.2-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture15" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-E-0.2-aliquot4"
          },
          {
            "@id": "#sample/sample-C-0.2-aliquot10",
            "derivesFrom": [ { "@id": "#source/source-culture3" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-C-0.2-aliquot10"
          },
          {
            "@id": "#sample/sample-N-0.1-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture5" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
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
            "name": "sample-N-0.1-aliquot7"
          },
          {
            "@id": "#sample/sample-S-0.1-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture11" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
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
            "name": "sample-S-0.1-aliquot5"
          },
          {
            "@id": "#sample/sample-N-0.2-aliquot9",
            "derivesFrom": [ { "@id": "#source/source-culture6" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-N-0.2-aliquot9"
          },
          {
            "@id": "#sample/sample-C-0.1-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture2" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
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
            "name": "sample-C-0.1-aliquot5"
          },
          {
            "@id": "#sample/sample-E-0.2-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture15" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-E-0.2-aliquot5"
          },
          {
            "@id": "#sample/sample-S-0.07-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture10" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-S-0.07-aliquot5"
          },
          {
            "@id": "#sample/sample-S-0.07-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture10" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.07-aliquot3"
          },
          {
            "@id": "#sample/sample-C-0.07-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture1" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-C-0.07-aliquot2"
          },
          {
            "@id": "#sample/sample-N-0.07-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture4" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-N-0.07-aliquot3"
          },
          {
            "@id": "#sample/sample-G-0.1-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture17" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
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
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-G-0.1-aliquot6"
          },
          {
            "@id": "#sample/sample-N-0.07-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture4" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-N-0.07-aliquot1"
          },
          {
            "@id": "#sample/sample-S-0.2-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture12" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.2-aliquot3"
          },
          {
            "@id": "#sample/sample-P-0.07-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture7" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-P-0.07-aliquot3"
          },
          {
            "@id": "#sample/sample-E-0.07-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture13" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-E-0.07-aliquot2"
          },
          {
            "@id": "#sample/sample-G-0.2-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture18" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-G-0.2-aliquot5"
          },
          {
            "@id": "#sample/sample-C-0.2-aliquot11",
            "derivesFrom": [ { "@id": "#source/source-culture3" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-C-0.2-aliquot11"
          },
          {
            "@id": "#sample/sample-E-0.07-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture13" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-E-0.07-aliquot3"
          },
          {
            "@id": "#sample/sample-C-0.2-aliquot8",
            "derivesFrom": [ { "@id": "#source/source-culture3" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-C-0.2-aliquot8"
          },
          {
            "@id": "#sample/sample-N-0.07-aliquot9",
            "derivesFrom": [ { "@id": "#source/source-culture4" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-N-0.07-aliquot9"
          },
          {
            "@id": "#sample/sample-S-0.1-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture11" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.1-aliquot3"
          },
          {
            "@id": "#sample/sample-N-0.1-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture5" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-N-0.1-aliquot4"
          },
          {
            "@id": "#sample/sample-N-0.2-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture6" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-N-0.2-aliquot4"
          },
          {
            "@id": "#sample/sample-S-0.07-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture10" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.07-aliquot1"
          },
          {
            "@id": "#sample/sample-S-0.2-aliquot8",
            "derivesFrom": [ { "@id": "#source/source-culture12" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-S-0.2-aliquot8"
          },
          {
            "@id": "#sample/sample-S-0.07-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture10" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-S-0.07-aliquot6"
          },
          {
            "@id": "#sample/sample-N-0.2-aliquot11",
            "derivesFrom": [ { "@id": "#source/source-culture6" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-N-0.2-aliquot11"
          },
          {
            "@id": "#sample/sample-N-0.2-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture6" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
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
            "name": "sample-N-0.2-aliquot7"
          },
          {
            "@id": "#sample/sample-C-0.1-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture2" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
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
            "name": "sample-C-0.1-aliquot6"
          },
          {
            "@id": "#sample/sample-N-0.2-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture6" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-N-0.2-aliquot1"
          },
          {
            "@id": "#sample/sample-E-0.1-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture14" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
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
            "name": "sample-E-0.1-aliquot5"
          },
          {
            "@id": "#sample/sample-P-0.2-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture9" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-P-0.2-aliquot3"
          },
          {
            "@id": "#sample/sample-P-0.2-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture9" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-P-0.2-aliquot1"
          },
          {
            "@id": "#sample/sample-C-0.2-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture3" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-C-0.2-aliquot1"
          },
          {
            "@id": "#sample/sample-E-0.07-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture13" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-E-0.07-aliquot1"
          },
          {
            "@id": "#sample/sample-G-0.07-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture16" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-G-0.07-aliquot6"
          },
          {
            "@id": "#sample/sample-G-0.1-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture17" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
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
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-G-0.1-aliquot4"
          },
          {
            "@id": "#sample/sample-C-0.1-aliquot11",
            "derivesFrom": [ { "@id": "#source/source-culture2" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-C-0.1-aliquot11"
          },
          {
            "@id": "#sample/sample-C-0.2-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture3" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-C-0.2-aliquot2"
          },
          {
            "@id": "#sample/sample-P-0.07-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture7" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-P-0.07-aliquot6"
          },
          {
            "@id": "#sample/sample-N-0.2-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture6" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
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
            "name": "sample-N-0.2-aliquot5"
          },
          {
            "@id": "#sample/sample-G-0.1-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture17" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
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
            "name": "sample-G-0.1-aliquot3"
          },
          {
            "@id": "#sample/sample-S-0.1-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture11" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.1-aliquot2"
          },
          {
            "@id": "#sample/sample-S-0.1-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture11" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.1-aliquot7"
          },
          {
            "@id": "#sample/sample-S-0.2-aliquot9",
            "derivesFrom": [ { "@id": "#source/source-culture12" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.2-aliquot9"
          },
          {
            "@id": "#sample/sample-C-0.1-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture2" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-C-0.1-aliquot2"
          },
          {
            "@id": "#sample/sample-G-0.2-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture18" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-G-0.2-aliquot4"
          },
          {
            "@id": "#sample/sample-P-0.2-aliquot8",
            "derivesFrom": [ { "@id": "#source/source-culture9" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-P-0.2-aliquot8"
          },
          {
            "@id": "#sample/sample-P-0.1-aliquot2",
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
            "characteristics": [],
            "name": "sample-P-0.1-aliquot2"
          },
          {
            "@id": "#sample/sample-P-0.07-aliquot9",
            "derivesFrom": [ { "@id": "#source/source-culture7" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-P-0.07-aliquot9"
          },
          {
            "@id": "#sample/sample-P-0.2-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture9" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
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
            "name": "sample-P-0.2-aliquot6"
          },
          {
            "@id": "#sample/sample-P-0.2-aliquot10",
            "derivesFrom": [ { "@id": "#source/source-culture9" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-P-0.2-aliquot10"
          },
          {
            "@id": "#sample/sample-C-0.1-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture2" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
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
            "name": "sample-C-0.1-aliquot7"
          },
          {
            "@id": "#sample/sample-P-0.07-aliquot10",
            "derivesFrom": [ { "@id": "#source/source-culture7" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-P-0.07-aliquot10"
          },
          {
            "@id": "#sample/sample-S-0.07-aliquot8",
            "derivesFrom": [ { "@id": "#source/source-culture10" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.07-aliquot8"
          },
          {
            "@id": "#sample/sample-S-0.07-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture10" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.07-aliquot4"
          },
          {
            "@id": "#sample/sample-P-0.07-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture7" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-P-0.07-aliquot5"
          },
          {
            "@id": "#sample/sample-N-0.2-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture6" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-N-0.2-aliquot3"
          },
          {
            "@id": "#sample/sample-P-0.2-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture9" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-P-0.2-aliquot2"
          },
          {
            "@id": "#sample/sample-S-0.1-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture11" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.1-aliquot4"
          },
          {
            "@id": "#sample/sample-E-0.07-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture13" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-E-0.07-aliquot4"
          },
          {
            "@id": "#sample/sample-N-0.2-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture6" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-N-0.2-aliquot2"
          },
          {
            "@id": "#sample/sample-S-0.2-aliquot11",
            "derivesFrom": [ { "@id": "#source/source-culture12" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.2-aliquot11"
          },
          {
            "@id": "#sample/sample-N-0.1-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture5" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-N-0.1-aliquot2"
          },
          {
            "@id": "#sample/sample-E-0.2-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture15" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-E-0.2-aliquot1"
          },
          {
            "@id": "#sample/sample-N-0.07-aliquot8",
            "derivesFrom": [ { "@id": "#source/source-culture4" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-N-0.07-aliquot8"
          },
          {
            "@id": "#sample/sample-E-0.2-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture15" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-E-0.2-aliquot3"
          },
          {
            "@id": "#sample/sample-S-0.1-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture11" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.1-aliquot1"
          },
          {
            "@id": "#sample/sample-S-0.2-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture12" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
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
            "name": "sample-S-0.2-aliquot7"
          },
          {
            "@id": "#sample/sample-P-0.2-aliquot11",
            "derivesFrom": [ { "@id": "#source/source-culture9" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-P-0.2-aliquot11"
          },
          {
            "@id": "#sample/sample-P-0.07-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture7" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-P-0.07-aliquot2"
          },
          {
            "@id": "#sample/sample-C-0.07-aliquot10",
            "derivesFrom": [ { "@id": "#source/source-culture1" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-C-0.07-aliquot10"
          },
          {
            "@id": "#sample/sample-P-0.1-aliquot6",
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
            "name": "sample-P-0.1-aliquot6"
          },
          {
            "@id": "#sample/sample-C-0.2-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture3" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
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
            "name": "sample-C-0.2-aliquot6"
          },
          {
            "@id": "#sample/sample-S-0.1-aliquot11",
            "derivesFrom": [ { "@id": "#source/source-culture11" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.1-aliquot11"
          },
          {
            "@id": "#sample/sample-S-0.2-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture12" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.2-aliquot1"
          },
          {
            "@id": "#sample/sample-N-0.07-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture4" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-N-0.07-aliquot4"
          },
          {
            "@id": "#sample/sample-G-0.1-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture17" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
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
            "name": "sample-G-0.1-aliquot2"
          },
          {
            "@id": "#sample/sample-G-0.07-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture16" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-G-0.07-aliquot3"
          },
          {
            "@id": "#sample/sample-E-0.07-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture13" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-E-0.07-aliquot5"
          },
          {
            "@id": "#sample/sample-N-0.07-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture4" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-N-0.07-aliquot5"
          },
          {
            "@id": "#sample/sample-G-0.07-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture16" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-G-0.07-aliquot5"
          },
          {
            "@id": "#sample/sample-C-0.07-aliquot8",
            "derivesFrom": [ { "@id": "#source/source-culture1" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-C-0.07-aliquot8"
          },
          {
            "@id": "#sample/sample-C-0.07-aliquot9",
            "derivesFrom": [ { "@id": "#source/source-culture1" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-C-0.07-aliquot9"
          },
          {
            "@id": "#sample/sample-C-0.1-aliquot8",
            "derivesFrom": [ { "@id": "#source/source-culture2" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
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
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-C-0.1-aliquot8"
          },
          {
            "@id": "#sample/sample-S-0.2-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture12" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
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
            "name": "sample-S-0.2-aliquot6"
          },
          {
            "@id": "#sample/sample-C-0.1-aliquot9",
            "derivesFrom": [ { "@id": "#source/source-culture2" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
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
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-C-0.1-aliquot9"
          },
          {
            "@id": "#sample/sample-S-0.1-aliquot8",
            "derivesFrom": [ { "@id": "#source/source-culture11" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.1-aliquot8"
          },
          {
            "@id": "#sample/sample-N-0.1-aliquot10",
            "derivesFrom": [ { "@id": "#source/source-culture5" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
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
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-N-0.1-aliquot10"
          },
          {
            "@id": "#sample/sample-E-0.07-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture13" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-E-0.07-aliquot6"
          },
          {
            "@id": "#sample/sample-C-0.07-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture1" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-C-0.07-aliquot3"
          },
          {
            "@id": "#sample/sample-E-0.1-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture14" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
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
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-E-0.1-aliquot1"
          },
          {
            "@id": "#sample/sample-N-0.1-aliquot9",
            "derivesFrom": [ { "@id": "#source/source-culture5" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
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
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-N-0.1-aliquot9"
          },
          {
            "@id": "#sample/sample-P-0.1-aliquot11",
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
            "characteristics": [],
            "name": "sample-P-0.1-aliquot11"
          },
          {
            "@id": "#sample/sample-P-0.07-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture7" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-P-0.07-aliquot7"
          },
          {
            "@id": "#sample/sample-P-0.1-aliquot9",
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
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-P-0.1-aliquot9"
          },
          {
            "@id": "#sample/sample-E-0.1-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture14" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
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
            "name": "sample-E-0.1-aliquot4"
          },
          {
            "@id": "#sample/sample-G-0.1-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture17" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
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
            "name": "sample-G-0.1-aliquot1"
          },
          {
            "@id": "#sample/sample-C-0.07-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture1" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-C-0.07-aliquot1"
          },
          {
            "@id": "#sample/sample-G-0.2-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture18" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
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
            "name": "sample-G-0.2-aliquot2"
          },
          {
            "@id": "#sample/sample-S-0.2-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture12" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
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
            "name": "sample-S-0.2-aliquot5"
          },
          {
            "@id": "#sample/sample-S-0.07-aliquot9",
            "derivesFrom": [ { "@id": "#source/source-culture10" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.07-aliquot9"
          },
          {
            "@id": "#sample/sample-P-0.2-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture9" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
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
            "name": "sample-P-0.2-aliquot7"
          },
          {
            "@id": "#sample/sample-S-0.1-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture11" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
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
            "name": "sample-S-0.1-aliquot6"
          },
          {
            "@id": "#sample/sample-N-0.1-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture5" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-N-0.1-aliquot3"
          },
          {
            "@id": "#sample/sample-G-0.2-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture18" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
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
            "name": "sample-G-0.2-aliquot3"
          },
          {
            "@id": "#sample/sample-C-0.1-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture2" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-C-0.1-aliquot4"
          },
          {
            "@id": "#sample/sample-N-0.1-aliquot8",
            "derivesFrom": [ { "@id": "#source/source-culture5" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
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
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-N-0.1-aliquot8"
          },
          {
            "@id": "#sample/sample-G-0.2-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture18" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
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
            "name": "sample-G-0.2-aliquot1"
          },
          {
            "@id": "#sample/sample-P-0.1-aliquot10",
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
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-P-0.1-aliquot10"
          },
          {
            "@id": "#sample/sample-C-0.1-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture2" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-C-0.1-aliquot1"
          },
          {
            "@id": "#sample/sample-P-0.1-aliquot5",
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
            "name": "sample-P-0.1-aliquot5"
          },
          {
            "@id": "#sample/sample-E-0.1-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture14" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
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
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-E-0.1-aliquot2"
          },
          {
            "@id": "#sample/sample-G-0.07-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture16" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-G-0.07-aliquot4"
          },
          {
            "@id": "#sample/sample-N-0.1-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture5" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-N-0.1-aliquot1"
          },
          {
            "@id": "#sample/sample-G-0.07-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture16" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-G-0.07-aliquot2"
          },
          {
            "@id": "#sample/sample-E-0.2-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture15" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-E-0.2-aliquot6"
          },
          {
            "@id": "#sample/sample-N-0.2-aliquot8",
            "derivesFrom": [ { "@id": "#source/source-culture6" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-N-0.2-aliquot8"
          },
          {
            "@id": "#sample/sample-C-0.1-aliquot10",
            "derivesFrom": [ { "@id": "#source/source-culture2" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
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
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-C-0.1-aliquot10"
          },
          {
            "@id": "#sample/sample-C-0.07-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture1" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-C-0.07-aliquot5"
          },
          {
            "@id": "#sample/sample-P-0.1-aliquot1",
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
            "characteristics": [],
            "name": "sample-P-0.1-aliquot1"
          },
          {
            "@id": "#sample/sample-N-0.07-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture4" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-N-0.07-aliquot2"
          },
          {
            "@id": "#sample/sample-P-0.1-aliquot3",
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
            "characteristics": [],
            "name": "sample-P-0.1-aliquot3"
          },
          {
            "@id": "#sample/sample-C-0.1-aliquot3",
            "derivesFrom": [ { "@id": "#source/source-culture2" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-C-0.1-aliquot3"
          },
          {
            "@id": "#sample/sample-S-0.1-aliquot10",
            "derivesFrom": [ { "@id": "#source/source-culture11" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.1-aliquot10"
          },
          {
            "@id": "#sample/sample-S-0.07-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture10" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-S-0.07-aliquot7"
          },
          {
            "@id": "#sample/sample-P-0.2-aliquot9",
            "derivesFrom": [ { "@id": "#source/source-culture9" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-P-0.2-aliquot9"
          },
          {
            "@id": "#sample/sample-S-0.07-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture10" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.07-aliquot2"
          },
          {
            "@id": "#sample/sample-N-0.2-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture6" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
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
            "name": "sample-N-0.2-aliquot6"
          },
          {
            "@id": "#sample/sample-N-0.2-aliquot10",
            "derivesFrom": [ { "@id": "#source/source-culture6" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-N-0.2-aliquot10"
          },
          {
            "@id": "#sample/sample-S-0.07-aliquot10",
            "derivesFrom": [ { "@id": "#source/source-culture10" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.07-aliquot10"
          },
          {
            "@id": "#sample/sample-N-0.1-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture5" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
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
            "name": "sample-N-0.1-aliquot6"
          },
          {
            "@id": "#sample/sample-P-0.07-aliquot1",
            "derivesFrom": [ { "@id": "#source/source-culture7" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-P-0.07-aliquot1"
          },
          {
            "@id": "#sample/sample-C-0.2-aliquot9",
            "derivesFrom": [ { "@id": "#source/source-culture3" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-C-0.2-aliquot9"
          },
          {
            "@id": "#sample/sample-S-0.2-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture12" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.2-aliquot4"
          },
          {
            "@id": "#sample/sample-E-0.2-aliquot2",
            "derivesFrom": [ { "@id": "#source/source-culture15" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "ethanol"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-E-0.2-aliquot2"
          },
          {
            "@id": "#sample/sample-C-0.07-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture1" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-C-0.07-aliquot7"
          },
          {
            "@id": "#sample/sample-N-0.07-aliquot10",
            "derivesFrom": [ { "@id": "#source/source-culture4" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "nitrogen"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-N-0.07-aliquot10"
          },
          {
            "@id": "#sample/sample-S-0.2-aliquot10",
            "derivesFrom": [ { "@id": "#source/source-culture12" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.2-aliquot10"
          },
          {
            "@id": "#sample/sample-P-0.1-aliquot4",
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
            "characteristics": [],
            "name": "sample-P-0.1-aliquot4"
          },
          {
            "@id": "#sample/sample-C-0.07-aliquot6",
            "derivesFrom": [ { "@id": "#source/source-culture1" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
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
            "name": "sample-C-0.07-aliquot6"
          },
          {
            "@id": "#sample/sample-C-0.2-aliquot7",
            "derivesFrom": [ { "@id": "#source/source-culture3" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.2,
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
            "name": "sample-C-0.2-aliquot7"
          },
          {
            "@id": "#sample/sample-P-0.1-aliquot8",
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
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-P-0.1-aliquot8"
          },
          {
            "@id": "#sample/sample-S-0.1-aliquot9",
            "derivesFrom": [ { "@id": "#source/source-culture11" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "sulphur"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.1,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-S-0.1-aliquot9"
          },
          {
            "@id": "#sample/sample-P-0.07-aliquot8",
            "derivesFrom": [ { "@id": "#source/source-culture7" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "phosphorus"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/Material_Type" },
                "value": {
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-P-0.07-aliquot8"
          },
          {
            "@id": "#sample/sample-C-0.07-aliquot4",
            "derivesFrom": [ { "@id": "#source/source-culture1" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "carbon"
                }
              },
              {
                "category": { "@id": "#factor/rate" },
                "value": 0.07,
                "unit": { "@id": "#Unit/l/hour" }
              }
            ],
            "characteristics": [],
            "name": "sample-C-0.07-aliquot4"
          },
          {
            "@id": "#sample/sample-G-0.1-aliquot5",
            "derivesFrom": [ { "@id": "#source/source-culture17" } ],
            "factorValues": [
              {
                "category": { "@id": "#factor/limiting_nutrient" },
                "value": {
                  "annotationValue": "glucose"
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
                  "annotationValue": "internal"
                }
              }
            ],
            "name": "sample-G-0.1-aliquot5"
          }
        ],
        "sources": [
          {
            "@id": "#source/source-culture13",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture13"
          },
          {
            "@id": "#source/source-culture9",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture9"
          },
          {
            "@id": "#source/source-culture12",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture12"
          },
          {
            "@id": "#source/source-culture3",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture3"
          },
          {
            "@id": "#source/source-culture5",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture5"
          },
          {
            "@id": "#source/source-culture8",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture8"
          },
          {
            "@id": "#source/source-culture10",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture10"
          },
          {
            "@id": "#source/source-culture7",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture7"
          },
          {
            "@id": "#source/source-culture18",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture18"
          },
          {
            "@id": "#source/source-culture4",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture4"
          },
          {
            "@id": "#source/source-culture2",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture2"
          },
          {
            "@id": "#source/source-culture14",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture14"
          },
          {
            "@id": "#source/source-culture15",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture15"
          },
          {
            "@id": "#source/source-culture11",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture11"
          },
          {
            "@id": "#source/source-culture17",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture17"
          },
          {
            "@id": "#source/source-culture16",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture16"
          },
          {
            "@id": "#source/source-culture6",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture6"
          },
          {
            "@id": "#source/source-culture1",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-culture1"
          }
        ]
      },
      "identifier": "BII-S-1",
      "title": "Study of the impact of changes in flux on the transcriptome, proteome, endometabolome and exometabolome of the yeast Saccharomyces cerevisiae under different nutrient limitations",
      "protocols": [
        {
          "parameters": [],
          "components": [],
          "description": "",
          "version": "",
          "@id": "#protocol/unknown",
          "name": "unknown",
          "protocolType": { "annotationValue": "" }
        },
        {
          "parameters": [],
          "components": [],
          "description": "1. Biomass samples (45 ml) were taken via the sample port of the Applikon fermenters. The cells were pelleted by centrifugation for 5 min at 5000 rpm. The supernatant was removed and the RNA pellet resuspended in the residual medium to form a slurry. This was added in a dropwise manner directly into a 5 ml Teflon flask (B. Braun Biotech, Germany) containing liquid nitrogen and a 7 mm-diameter tungsten carbide ball. After allowing evaporation of the liquid nitrogen the flask was reassembled and the cells disrupted by agitation at 1500 rpm for 2 min in a Microdismembranator U (B. Braun Biotech, Germany) 2. The frozen powder was then dissolved in 1 ml of TriZol reagent (Sigma-Aldrich, UK), vortexed for 1 min, and then kept at room temperature for a further 5min. 3. Chloroform extraction was performed by addition of 0.2 ml chloroform, shaking vigorously or 15 s, then 5min incubation at room temperature. 4. Following centrifugation at 12,000 rpm for 5 min, the RNA (contained in the aqueous phase) was precipitated with 0.5 vol of 2-propanol at room temperature for 15 min. 5. After further centrifugation (12,000 rpm for 10 min at 4 C) the RNA pellet was washed twice with 70 % (v/v) ethanol, briefly air-dried, and redissolved in 0.5 ml diethyl pyrocarbonate (DEPC)-treated water. 6. The single-stranded RNA was precipitated once more by addition of 0.5 ml of LiCl buffer (4 M LiCl, 20 mM Tris-HCl, pH 7.5, 10 mM EDTA), thus removing tRNA and DNA from the sample. 7. After precipitation (20 C for 1h) and centrifugation (12,000 rpm, 30 min, 4 C), the RNA was washed twice in 70 % (v/v) ethanol prior to being dissolved in a minimal volume of DEPC-treated water. 8. Total RNA quality was checked using the RNA 6000 Nano Assay, and analysed on an Agilent 2100 Bioanalyser (Agilent Technologies). RNA was quantified using the Nanodrop ultra low volume spectrophotometer (Nanodrop Technologies).",
          "version": "",
          "@id": "#protocol/growth_protocol",
          "name": "growth protocol",
          "protocolType": {
            "annotationValue": "growth"
          }
        },
        {
          "parameters": [],
          "components": [],
          "description": "1. Biomass samples (45 ml) were taken via the sample port of the Applikon fermenters. The cells were pelleted by centrifugation for 5 min at 5000 rpm. The supernatant was removed and the RNA pellet resuspended in the residual medium to form a slurry. This was added in a dropwise manner directly into a 5 ml Teflon flask (B. Braun Biotech, Germany) containing liquid nitrogen and a 7 mm-diameter tungsten carbide ball. After allowing evaporation of the liquid nitrogen the flask was reassembled and the cells disrupted by agitation at 1500 rpm for 2 min in a Microdismembranator U (B. Braun Biotech, Germany) 2. The frozen powder was then dissolved in 1 ml of TriZol reagent (Sigma-Aldrich, UK), vortexed for 1 min, and then kept at room temperature for a further 5min. 3. Chloroform extraction was performed by addition of 0.2 ml chloroform, shaking vigorouslyor 15 s, then 5min incubation at room temperature. 4. Following centrifugation at 12,000 rpm for 5 min, the RNA (contained in the aqueous phase) was precipitated with 0.5 vol of 2-propanol at room temperature for 15 min. 5. After further centrifugation (12,000 rpm for 10 min at 4 C) the RNA pellet was washed twice with 70 % (v/v) ethanol, briefly air-dried, and redissolved in 0.5 ml diethyl pyrocarbonate (DEPC)-treated water. 6. The single-stranded RNA was precipitated once more by addition of 0.5 ml of LiCl bffer (4 M LiCl, 20 mM Tris-HCl, pH 7.5, 10 mM EDTA), thus removing tRNA and DNA from the sample. 7. After precipitation (20 C for 1 h) and centrifugation (12,000 rpm, 30 min, 4 C), the RNA was washed twice in 70 % (v/v) ethanol prior to being dissolved in a minimal volume of DEPC-treated water. 8. Total RNA quality was checked using the RNA 6000 Nano Assay, and analysed on an Agilent 2100 Bioanalyser (Agilent Technologies). RNA was quantified using the Nanodrop ultra low volume spectrophotometer (Nanodrop Technologies).",
          "version": "",
          "@id": "#protocol/mRNA_extraction",
          "name": "mRNA extraction",
          "protocolType": {
            "annotationValue": "mRNA extraction"
          }
        },
        {
          "parameters": [],
          "components": [],
          "description": "",
          "version": "",
          "@id": "#protocol/protein_extraction",
          "name": "protein extraction",
          "protocolType": {
            "annotationValue": "protein extraction"
          }
        },
        {
          "parameters": [],
          "components": [],
          "description": "This was done using Enzo BioArrayTM HighYieldTM RNA transcript labelling kit (T7) with 5 ul cDNA. The resultant cRNA was again purified using the GeneChip Sample Clean Up Module. The column was eluted in the first instance using 10 l RNase-free water, and for a second time using 11 ul RNase-free water. cRNA was quantified using the Nanodrop spectrophotometer. A total of 15 ug of cRNA (required for hybridisation) was fragmented. Fragmentation was carried out by using 2 ul of fragmentation buffer for every 8 ul cRNA.",
          "version": "",
          "@id": "#protocol/biotin_labeling",
          "name": "biotin labeling",
          "protocolType": {
            "annotationValue": "labeling"
          }
        },
        {
          "parameters": [],
          "components": [],
          "description": "",
          "version": "",
          "@id": "#protocol/ITRAQ_labeling",
          "name": "ITRAQ labeling",
          "protocolType": {
            "annotationValue": "labeling"
          }
        },
        {
          "parameters": [],
          "components": [],
          "description": "For each target, a hybridisation cocktail was made using the standard array recipe as described in the GeneChip Expression Analysis technical manual. GeneChip control oligonucleotide and 20x eukaryotic hybridisation controls were used. Hybridisation buffer was made as detailed in the GeneChip manual and the BSA and herring sperm DNA was purchased from Invitrogen. The cocktail was heated to 99 C for 5 min, transferred to 45 C for 5 min and then spun for 5 min to remove any insoluble material. Affymetrix Yeast Yg_s98 S. cerevisiae arrays were pre-hybridised with 200 ul 1x hybridisation buffer and incubated at 45 C for 10 min. 200 ul of the hybridisation cocktail was loaded onto the arrays. The probe array was incubated in a rotisserie at 45 C, rotating at 60 rpm. Following hybridisation, for 16 hr, chips were loaded onto a Fluidics station for washing and staining using the EukGe WS2v4 programme controlled using Microarray Suite 5 software.",
          "version": "",
          "@id": "#protocol/EukGE-WS4",
          "name": "EukGE-WS4",
          "protocolType": {
            "annotationValue": "hybridization"
          }
        },
        {
          "parameters": [
            {
              "@id": "#parameter/sample_volume",
              "parameterName": {
                "annotationValue": "sample volume"
              }
            },
            {
              "@id": "#parameter/standard_volume",
              "parameterName": {
                "annotationValue": "standard volume"
              }
            }
          ],
          "components": [],
          "description": "",
          "version": "",
          "@id": "#protocol/metabolite_extraction",
          "name": "metabolite extraction",
          "protocolType": {
            "termAccession": "http://purl.obolibrary.org/obo/OBI_0302884",
            "termSource": "OBI",
            "annotationValue": "extraction"
          }
        }
      ]
    },
    {
      "submissionDate": "2007-04-30",
      "processSequence": [
        {
          "outputs": [
            { "@id": "#sample/sample-NZ_0hrs_Grow_1" },
            { "@id": "#sample/sample-NZ_0hrs_Grow_2" }
          ],
          "inputs": [ { "@id": "#source/source-Saccharomyces_cerevisiae_FY1679_" } ],
          "parameterValues": [],
          "@id": "#process/growth1",
          
          "comments": [],
          "performer": "",
          "executesProtocol": { "@id": "#protocol/growth" }
        }
      ],
      "people": [
        {
          "phone": "",
          "firstName": "Stephen",
          "address": "Oxford Road, Manchester M13 9PT, UK",
              "lastName": "Oliver",
          "midInitials": "G",
          "@id": "#person/Oliver",
          "fax": "",
          "comments": [
            {
              "value": "",
              "name": "Study Person REF"
            }
          ],
          "roles": [
            {
              "annotationValue": "corresponding author"
            }
          ],
          "affiliation": "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
        },
        {
          "phone": "",
          "firstName": "Castrillo",
          "address": "Oxford Road, Manchester M13 9PT, UK",
              "lastName": "Juan",
          "midInitials": "I",
          "@id": "#person/Juan",
          "fax": "",
          "comments": [
            {
              "value": "",
              "name": "Study Person REF"
            }
          ],
          "roles": [
            {
              "annotationValue": "author"
            }
          ],
          "affiliation": "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
        },
        {
          "phone": "",
          "firstName": "Zeef",
          "address": "Oxford Road, Manchester M13 9PT, UK",
              "lastName": "Leo",
          "midInitials": "A",
          "@id": "#person/Leo",
          "fax": "",
          "comments": [
            {
              "value": "",
              "name": "Study Person REF"
            }
          ],
          "roles": [
            {
              "annotationValue": "author"
            }
          ],
          "affiliation": "Faculty of Life Sciences, Michael Smith Building, University of Manchester"
        }
      ],
      "comments": [
        {
          "value": "",
          "name": "Study Funding Agency"
        },
        {
          "value": "",
          "name": "Study Grant Number"
        }
      ],
      "description": "Comprehensive high-throughput analyses at the levels of mRNAs, proteins, and metabolites, and studies on gene expression patterns are required for systems biology studies of cell growth [4,26-29]. Although such comprehensive data sets are lacking, many studies have pointed to a central role for the target-of-rapamycin (TOR) signal transduction pathway in growth control. TOR is a serine/threonine kinase that has been conserved from yeasts to mammals; it integrates signals from nutrients or growth factors to regulate cell growth and cell-cycle progression coordinately. Although such comprehensive data sets are lacking, many studies have pointed to a central role for the target-of-rapamycin (TOR) signal transduction pathway in growth control. TOR is a serine/threonine kinase that has been conserved from yeasts to mammals; it integrates signals from nutrients or growth factors to regulate cell growth and cell-cycle progression coordinately. The effect of rapamycin were studied as follows: a culture growing at mid-exponential phase was divided into two. Rapamycin (200 ng/ml) was added to one half, and the drug's solvent to the other, as the control. Samples were taken at 0, 1, 2 and 4 h after treatment. Gene expression at the mRNA level was investigated by transcriptome analysis using Affymetrix hybridization arrays.",
      "unitCategories": [],
      "studyDesignDescriptors": [
        {
          "termAccession": "http://purl.obolibrary.org/obo/OBI_0500020",
          "termSource": "OBI",
          "annotationValue": "time series design"
        }
      ],
      "publicReleaseDate": "2009-03-10",
      "characteristicCategories": [
        {
          "@id": "#characteristic_category/mating_type",
          "characteristicType": {
            "annotationValue": "mating type"
          }
        }
      ],
      "assays": [
        {
          "measurementType": {
            "termAccession": "http://purl.obolibrary.org/obo/OBI_0000424",
            "termSource": "OBI",
            "annotationValue": "transcription profiling"
          },
          "dataFiles": [
            {
              "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648693.txt",
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "name": "E-MAXD-4-raw-data-426648693.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648765.txt",
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "name": "E-MAXD-4-raw-data-426648765.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648549.txt",
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "name": "E-MAXD-4-raw-data-426648549.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648603.txt",
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "name": "E-MAXD-4-raw-data-426648603.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648639.txt",
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "name": "E-MAXD-4-raw-data-426648639.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648621.txt",
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "name": "E-MAXD-4-raw-data-426648621.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648711.txt",
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "name": "E-MAXD-4-raw-data-426648711.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648567.txt",
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "name": "E-MAXD-4-raw-data-426648567.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648675.txt",
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "name": "E-MAXD-4-raw-data-426648675.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648729.txt",
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "name": "E-MAXD-4-raw-data-426648729.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/derivedarraydatafile-E-MAXD-4-processed-data-1342566476.txt",
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "name": "E-MAXD-4-processed-data-1342566476.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648657.txt",
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "name": "E-MAXD-4-raw-data-426648657.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648747.txt",
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "name": "E-MAXD-4-raw-data-426648747.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648783.txt",
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "name": "E-MAXD-4-raw-data-426648783.txt",
              "type": "Raw Data File"
            },
            {
              "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648585.txt",
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "name": "E-MAXD-4-raw-data-426648585.txt",
              "type": "Raw Data File"
            }
          ],
          "technologyType": {
            "termAccession": "http://purl.obolibrary.org/obo/OBI_0400148",
            "termSource": "OBI",
            "annotationValue": "DNA microarray"
          },
          "processSequence": [
            {
              "outputs": [ { "@id": "#material/labeledextract-NZ_1hrs_Drug_Sample_1_Labelled" } ],
              "inputs": [ { "@id": "#material/extract-NZ_1hrs_Drug_Sample_1_Extract" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/NZ_1hrs_Drug_Sample_1_Labelled_Hyb3" },
              "@id": "#process/biotin_labeling2",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-NZ_0hrs_Sample_2_Labelled" } ],
              "inputs": [ { "@id": "#material/extract-NZ_0hrs_Sample_2_Extract" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/NZ_0hrs_Sample_2_Labelled_Hyb2" },
              "@id": "#process/biotin_labeling8",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction2" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648675.txt" } ],
              "name": "NZ_0hrs_Sample_2_Labelled_Hyb2_Scan2",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/data_processing" },
              "@id": "#process/NZ_0hrs_Sample_2_Labelled_Hyb2_Scan2",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/NZ_0hrs_Sample_2_Labelled_Hyb2" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "NZ_1hrs_Vehicle_Sample_1_Labelled_Hyb5",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/NZ_1hrs_Vehicle_Sample_1_Labelled_Hyb5_Scan5" },
              "@id": "#process/NZ_1hrs_Vehicle_Sample_1_Labelled_Hyb5",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-NZ_1hrs_Vehicle_Sample_1_Labelled" } ],
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "previousProcess": { "@id": "#process/biotin_labeling7" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "NZ_4hrs_Drug_Sample_1_Labelled_Hyb11",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/NZ_4hrs_Drug_Sample_1_Labelled_Hyb11_Scan11" },
              "@id": "#process/NZ_4hrs_Drug_Sample_1_Labelled_Hyb11",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-NZ_4hrs_Drug_Sample_1_Labelled" } ],
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "previousProcess": { "@id": "#process/biotin_labeling4" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "NZ_0hrs_Sample_2_Labelled_Hyb2",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/NZ_0hrs_Sample_2_Labelled_Hyb2_Scan2" },
              "@id": "#process/NZ_0hrs_Sample_2_Labelled_Hyb2",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-NZ_0hrs_Sample_2_Labelled" } ],
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "previousProcess": { "@id": "#process/biotin_labeling8" },
              "performer": ""
            },
            {
              "outputs": [
                { "@id": "#material/extract-NZ_0hrs_Sample_2_Extract" },
                { "@id": "#material/extract-NZ_1hrs_Drug_Sample_2_Extract" },
                { "@id": "#material/extract-NZ_4hrs_Drug_Sample_2_Extract" },
                { "@id": "#material/extract-NZ_2hrs_Drug_Sample_2_Extract" },
                { "@id": "#material/extract-NZ_1hrs_Vehicle_Sample_2_Extract" },
                { "@id": "#material/extract-NZ_2hrs_Vehicle_Sample_2_Extract" },
                { "@id": "#material/extract-NZ_4hrs_Vehicle_Sample_2_Extract" }
              ],
              "inputs": [ { "@id": "#sample/sample-NZ_0hrs_Grow_2" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction2",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling14" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [],
              "name": "NZ_4hrs_Vehicle_Sample_2_Labelled_Hyb14",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/NZ_4hrs_Vehicle_Sample_2_Labelled_Hyb14_Scan14" },
              "@id": "#process/NZ_4hrs_Vehicle_Sample_2_Labelled_Hyb14",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-NZ_4hrs_Vehicle_Sample_2_Labelled" } ],
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "previousProcess": { "@id": "#process/biotin_labeling14" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648693.txt" } ],
              "name": "NZ_1hrs_Drug_Sample_2_Labelled_Hyb4_Scan4",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/data_processing" },
              "@id": "#process/NZ_1hrs_Drug_Sample_2_Labelled_Hyb4_Scan4",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/NZ_1hrs_Drug_Sample_2_Labelled_Hyb4" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-NZ_2hrs_Vehicle_Sample_2_Labelled" } ],
              "inputs": [ { "@id": "#material/extract-NZ_2hrs_Vehicle_Sample_2_Extract" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/NZ_2hrs_Vehicle_Sample_2_Labelled_Hyb10" },
              "@id": "#process/biotin_labeling13",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction2" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [],
              "name": "NZ_2hrs_Vehicle_Sample_2_Labelled_Hyb10",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/NZ_2hrs_Vehicle_Sample_2_Labelled_Hyb10_Scan10" },
              "@id": "#process/NZ_2hrs_Vehicle_Sample_2_Labelled_Hyb10",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-NZ_2hrs_Vehicle_Sample_2_Labelled" } ],
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "previousProcess": { "@id": "#process/biotin_labeling13" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648783.txt" } ],
              "name": "NZ_4hrs_Vehicle_Sample_2_Labelled_Hyb14_Scan14",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/data_processing" },
              "@id": "#process/NZ_4hrs_Vehicle_Sample_2_Labelled_Hyb14_Scan14",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/NZ_4hrs_Vehicle_Sample_2_Labelled_Hyb14" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "NZ_4hrs_Drug_Sample_2_Labelled_Hyb12",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/NZ_4hrs_Drug_Sample_2_Labelled_Hyb12_Scan12" },
              "@id": "#process/NZ_4hrs_Drug_Sample_2_Labelled_Hyb12",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-NZ_4hrs_Drug_Sample_2_Labelled" } ],
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "previousProcess": { "@id": "#process/biotin_labeling10" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648567.txt" } ],
              "name": "NZ_1hrs_Drug_Sample_1_Labelled_Hyb3_Scan3",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/data_processing" },
              "@id": "#process/NZ_1hrs_Drug_Sample_1_Labelled_Hyb3_Scan3",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/NZ_1hrs_Drug_Sample_1_Labelled_Hyb3" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648603.txt" } ],
              "name": "NZ_4hrs_Drug_Sample_1_Labelled_Hyb11_Scan11",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/data_processing" },
              "@id": "#process/NZ_4hrs_Drug_Sample_1_Labelled_Hyb11_Scan11",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/NZ_4hrs_Drug_Sample_1_Labelled_Hyb11" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "NZ_1hrs_Drug_Sample_1_Labelled_Hyb3",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/NZ_1hrs_Drug_Sample_1_Labelled_Hyb3_Scan3" },
              "@id": "#process/NZ_1hrs_Drug_Sample_1_Labelled_Hyb3",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-NZ_1hrs_Drug_Sample_1_Labelled" } ],
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "previousProcess": { "@id": "#process/biotin_labeling2" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648621.txt" } ],
              "name": "NZ_1hrs_Vehicle_Sample_1_Labelled_Hyb5_Scan5",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/data_processing" },
              "@id": "#process/NZ_1hrs_Vehicle_Sample_1_Labelled_Hyb5_Scan5",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/NZ_1hrs_Vehicle_Sample_1_Labelled_Hyb5" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648585.txt" } ],
              "name": "NZ_2hrs_Drug_Sample_1_Labelled_Hyb7_Scan7",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/data_processing" },
              "@id": "#process/NZ_2hrs_Drug_Sample_1_Labelled_Hyb7_Scan7",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/NZ_2hrs_Drug_Sample_1_Labelled_Hyb7" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-NZ_2hrs_Vehicle_Sample_1_Labelled" } ],
              "inputs": [ { "@id": "#material/extract-NZ_2hrs_Vehicle_Sample_1_Extract" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/NZ_2hrs_Vehicle_Sample_1_Labelled_Hyb9" },
              "@id": "#process/biotin_labeling5",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/derivedarraydatafile-E-MAXD-4-processed-data-1342566476.txt" } ],
              "name": "data processing",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "@id": "#process/data_processing",
              "parameterValues": [],
              "inputs": [
                { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648549.txt" },
                { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648567.txt" },
                { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648585.txt" },
                { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648603.txt" },
                { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648639.txt" },
                { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648657.txt" },
                { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648621.txt" },
                { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648675.txt" },
                { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648693.txt" },
                { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648729.txt" },
                { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648711.txt" },
                { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648747.txt" },
                { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648765.txt" },
                { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648783.txt" }
              ],
              "comments": [],
              "previousProcess": { "@id": "#process/NZ_4hrs_Vehicle_Sample_2_Labelled_Hyb14_Scan14" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648711.txt" } ],
              "name": "NZ_2hrs_Drug_Sample_2_Labelled_Hyb8_Scan8",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/data_processing" },
              "@id": "#process/NZ_2hrs_Drug_Sample_2_Labelled_Hyb8_Scan8",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/NZ_2hrs_Drug_Sample_2_Labelled_Hyb8" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-NZ_2hrs_Drug_Sample_2_Labelled" } ],
              "inputs": [ { "@id": "#material/extract-NZ_2hrs_Drug_Sample_2_Extract" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/NZ_2hrs_Drug_Sample_2_Labelled_Hyb8" },
              "@id": "#process/biotin_labeling11",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction2" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [
                { "@id": "#material/extract-NZ_0hrs_Sample_1_Extract" },
                { "@id": "#material/extract-NZ_1hrs_Drug_Sample_1_Extract" },
                { "@id": "#material/extract-NZ_2hrs_Drug_Sample_1_Extract" },
                { "@id": "#material/extract-NZ_4hrs_Drug_Sample_1_Extract" },
                { "@id": "#material/extract-NZ_2hrs_Vehicle_Sample_1_Extract" },
                { "@id": "#material/extract-NZ_4hrs_Vehicle_Sample_1_Extract" },
                { "@id": "#material/extract-NZ_1hrs_Vehicle_Sample_1_Extract" }
              ],
              "inputs": [ { "@id": "#sample/sample-NZ_0hrs_Grow_1" } ],
              "parameterValues": [],
              "@id": "#process/mRNA_extraction1",

              "comments": [],
              "nextProcess": { "@id": "#process/biotin_labeling7" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/mRNA_extraction" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648729.txt" } ],
              "name": "NZ_4hrs_Drug_Sample_2_Labelled_Hyb12_Scan12",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/data_processing" },
              "@id": "#process/NZ_4hrs_Drug_Sample_2_Labelled_Hyb12_Scan12",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/NZ_4hrs_Drug_Sample_2_Labelled_Hyb12" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648747.txt" } ],
              "name": "NZ_1hrs_Vehicle_Sample_2_Labelled_Hyb6_Scan6",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/data_processing" },
              "@id": "#process/NZ_1hrs_Vehicle_Sample_2_Labelled_Hyb6_Scan6",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/NZ_1hrs_Vehicle_Sample_2_Labelled_Hyb6" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-NZ_1hrs_Drug_Sample_2_Labelled" } ],
              "inputs": [ { "@id": "#material/extract-NZ_1hrs_Drug_Sample_2_Extract" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/NZ_1hrs_Drug_Sample_2_Labelled_Hyb4" },
              "@id": "#process/biotin_labeling9",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction2" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648765.txt" } ],
              "name": "NZ_2hrs_Vehicle_Sample_2_Labelled_Hyb10_Scan10",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/data_processing" },
              "@id": "#process/NZ_2hrs_Vehicle_Sample_2_Labelled_Hyb10_Scan10",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/NZ_2hrs_Vehicle_Sample_2_Labelled_Hyb10" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-NZ_1hrs_Vehicle_Sample_1_Labelled" } ],
              "inputs": [ { "@id": "#material/extract-NZ_1hrs_Vehicle_Sample_1_Extract" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/NZ_1hrs_Vehicle_Sample_1_Labelled_Hyb5" },
              "@id": "#process/biotin_labeling7",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-NZ_2hrs_Drug_Sample_1_Labelled" } ],
              "inputs": [ { "@id": "#material/extract-NZ_2hrs_Drug_Sample_1_Extract" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/NZ_2hrs_Drug_Sample_1_Labelled_Hyb7" },
              "@id": "#process/biotin_labeling3",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-NZ_4hrs_Vehicle_Sample_2_Labelled" } ],
              "inputs": [ { "@id": "#material/extract-NZ_4hrs_Vehicle_Sample_2_Extract" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/NZ_4hrs_Vehicle_Sample_2_Labelled_Hyb14" },
              "@id": "#process/biotin_labeling14",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction2" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [],
              "name": "NZ_1hrs_Vehicle_Sample_2_Labelled_Hyb6",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/NZ_1hrs_Vehicle_Sample_2_Labelled_Hyb6_Scan6" },
              "@id": "#process/NZ_1hrs_Vehicle_Sample_2_Labelled_Hyb6",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-NZ_1hrs_Vehicle_Sample_2_Labelled" } ],
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "previousProcess": { "@id": "#process/biotin_labeling12" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-NZ_1hrs_Vehicle_Sample_2_Labelled" } ],
              "inputs": [ { "@id": "#material/extract-NZ_1hrs_Vehicle_Sample_2_Extract" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/NZ_1hrs_Vehicle_Sample_2_Labelled_Hyb6" },
              "@id": "#process/biotin_labeling12",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction2" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648639.txt" } ],
              "name": "NZ_2hrs_Vehicle_Sample_1_Labelled_Hyb9_Scan9",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/data_processing" },
              "@id": "#process/NZ_2hrs_Vehicle_Sample_1_Labelled_Hyb9_Scan9",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/NZ_2hrs_Vehicle_Sample_1_Labelled_Hyb9" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-NZ_4hrs_Vehicle_Sample_1_Labelled" } ],
              "inputs": [ { "@id": "#material/extract-NZ_4hrs_Vehicle_Sample_1_Extract" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/NZ_4hrs_Vehicle_Sample_1_Labelled_Hyb13" },
              "@id": "#process/biotin_labeling6",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-NZ_4hrs_Drug_Sample_2_Labelled" } ],
              "inputs": [ { "@id": "#material/extract-NZ_4hrs_Drug_Sample_2_Extract" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/NZ_4hrs_Drug_Sample_2_Labelled_Hyb12" },
              "@id": "#process/biotin_labeling10",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction2" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-NZ_0hrs_Sample_1_Labelled" } ],
              "inputs": [ { "@id": "#material/extract-NZ_0hrs_Sample_1_Extract" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/NZ_0hrs_Sample_1_Labelled_Hyb1" },
              "@id": "#process/biotin_labeling1",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648549.txt" } ],
              "name": "NZ_0hrs_Sample_1_Labelled_Hyb1_Scan1",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/data_processing" },
              "@id": "#process/NZ_0hrs_Sample_1_Labelled_Hyb1_Scan1",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/NZ_0hrs_Sample_1_Labelled_Hyb1" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "NZ_1hrs_Drug_Sample_2_Labelled_Hyb4",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/NZ_1hrs_Drug_Sample_2_Labelled_Hyb4_Scan4" },
              "@id": "#process/NZ_1hrs_Drug_Sample_2_Labelled_Hyb4",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-NZ_1hrs_Drug_Sample_2_Labelled" } ],
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "previousProcess": { "@id": "#process/biotin_labeling9" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "NZ_2hrs_Drug_Sample_2_Labelled_Hyb8",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/NZ_2hrs_Drug_Sample_2_Labelled_Hyb8_Scan8" },
              "@id": "#process/NZ_2hrs_Drug_Sample_2_Labelled_Hyb8",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-NZ_2hrs_Drug_Sample_2_Labelled" } ],
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "previousProcess": { "@id": "#process/biotin_labeling11" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "NZ_4hrs_Vehicle_Sample_1_Labelled_Hyb13",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/NZ_4hrs_Vehicle_Sample_1_Labelled_Hyb13_Scan13" },
              "@id": "#process/NZ_4hrs_Vehicle_Sample_1_Labelled_Hyb13",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-NZ_4hrs_Vehicle_Sample_1_Labelled" } ],
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "previousProcess": { "@id": "#process/biotin_labeling6" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "NZ_2hrs_Vehicle_Sample_1_Labelled_Hyb9",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/NZ_2hrs_Vehicle_Sample_1_Labelled_Hyb9_Scan9" },
              "@id": "#process/NZ_2hrs_Vehicle_Sample_1_Labelled_Hyb9",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-NZ_2hrs_Vehicle_Sample_1_Labelled" } ],
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "previousProcess": { "@id": "#process/biotin_labeling5" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "NZ_0hrs_Sample_1_Labelled_Hyb1",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/NZ_0hrs_Sample_1_Labelled_Hyb1_Scan1" },
              "@id": "#process/NZ_0hrs_Sample_1_Labelled_Hyb1",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-NZ_0hrs_Sample_1_Labelled" } ],
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "previousProcess": { "@id": "#process/biotin_labeling1" },
              "performer": ""
            },
            {
              "outputs": [ { "@id": "#material/labeledextract-NZ_4hrs_Drug_Sample_1_Labelled" } ],
              "inputs": [ { "@id": "#material/extract-NZ_4hrs_Drug_Sample_1_Extract" } ],
              "parameterValues": [],
              "nextProcess": { "@id": "#process/NZ_4hrs_Drug_Sample_1_Labelled_Hyb11" },
              "@id": "#process/biotin_labeling4",

              "comments": [],
              "previousProcess": { "@id": "#process/mRNA_extraction1" },
              "performer": "",
              "executesProtocol": { "@id": "#protocol/biotin_labeling" }
            },
            {
              "outputs": [ { "@id": "#data/arraydatafile-E-MAXD-4-raw-data-426648657.txt" } ],
              "name": "NZ_4hrs_Vehicle_Sample_1_Labelled_Hyb13_Scan13",
              "executesProtocol": { "@id": "#protocol/unknown" },
              "nextProcess": { "@id": "#process/data_processing" },
              "@id": "#process/NZ_4hrs_Vehicle_Sample_1_Labelled_Hyb13_Scan13",
              "parameterValues": [],
              "inputs": [],
              "comments": [],
              "previousProcess": { "@id": "#process/NZ_4hrs_Vehicle_Sample_1_Labelled_Hyb13" },
              "performer": ""
            },
            {
              "outputs": [],
              "name": "NZ_2hrs_Drug_Sample_1_Labelled_Hyb7",
              "executesProtocol": { "@id": "#protocol/EukGE-WS4" },
              "nextProcess": { "@id": "#process/NZ_2hrs_Drug_Sample_1_Labelled_Hyb7_Scan7" },
              "@id": "#process/NZ_2hrs_Drug_Sample_1_Labelled_Hyb7",
              "parameterValues": [
                {
                  "category": { "@id": "#parameter/Array_Design_REF" },
                  "value": "A-AFFY-27"
                }
              ],
              "inputs": [ { "@id": "#material/labeledextract-NZ_2hrs_Drug_Sample_1_Labelled" } ],
              "comments": [
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Processed Data URL"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Accession"
                },
                {
                  "value": "E-MEXP-115",
                  "name": "ArrayExpress Raw Data URL"
                }
              ],
              "previousProcess": { "@id": "#process/biotin_labeling3" },
              "performer": ""
            }
          ],
          "@id": "#assay/a_microarray.txt",
          "unitCategories": [
            {
              "@id": "#Unit/ng_/ml",
              "annotationValue": "ng /ml"
            },
            {
              "@id": "#Unit/hour",
              "termAccession": "http://purl.obolibrary.org/obo/UO_0000032",
              "annotationValue": "hour",
              "termSource": "UO"
            }
          ],
          "characteristicCategories": [],
          "materials": {
            "otherMaterials": [
              {
                "@id": "#material/labeledextract-NZ_2hrs_Vehicle_Sample_2_Labelled",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-NZ_2hrs_Vehicle_Sample_2_Labelled",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-NZ_1hrs_Vehicle_Sample_2_Extract",
                "characteristics": [],
                "name": "extract-NZ_1hrs_Vehicle_Sample_2_Extract",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-NZ_4hrs_Drug_Sample_2_Extract",
                "characteristics": [],
                "name": "extract-NZ_4hrs_Drug_Sample_2_Extract",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-NZ_4hrs_Vehicle_Sample_1_Labelled",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-NZ_4hrs_Vehicle_Sample_1_Labelled",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-NZ_2hrs_Vehicle_Sample_1_Extract",
                "characteristics": [],
                "name": "extract-NZ_2hrs_Vehicle_Sample_1_Extract",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-NZ_4hrs_Vehicle_Sample_1_Extract",
                "characteristics": [],
                "name": "extract-NZ_4hrs_Vehicle_Sample_1_Extract",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-NZ_1hrs_Vehicle_Sample_1_Extract",
                "characteristics": [],
                "name": "extract-NZ_1hrs_Vehicle_Sample_1_Extract",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-NZ_0hrs_Sample_2_Extract",
                "characteristics": [],
                "name": "extract-NZ_0hrs_Sample_2_Extract",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-NZ_1hrs_Vehicle_Sample_2_Labelled",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-NZ_1hrs_Vehicle_Sample_2_Labelled",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-NZ_4hrs_Drug_Sample_1_Extract",
                "characteristics": [],
                "name": "extract-NZ_4hrs_Drug_Sample_1_Extract",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-NZ_2hrs_Drug_Sample_1_Extract",
                "characteristics": [],
                "name": "extract-NZ_2hrs_Drug_Sample_1_Extract",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-NZ_1hrs_Drug_Sample_1_Extract",
                "characteristics": [],
                "name": "extract-NZ_1hrs_Drug_Sample_1_Extract",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-NZ_4hrs_Drug_Sample_1_Labelled",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-NZ_4hrs_Drug_Sample_1_Labelled",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-NZ_1hrs_Drug_Sample_1_Labelled",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-NZ_1hrs_Drug_Sample_1_Labelled",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-NZ_4hrs_Vehicle_Sample_2_Labelled",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-NZ_4hrs_Vehicle_Sample_2_Labelled",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-NZ_2hrs_Vehicle_Sample_2_Extract",
                "characteristics": [],
                "name": "extract-NZ_2hrs_Vehicle_Sample_2_Extract",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-NZ_4hrs_Drug_Sample_2_Labelled",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-NZ_4hrs_Drug_Sample_2_Labelled",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-NZ_1hrs_Vehicle_Sample_1_Labelled",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-NZ_1hrs_Vehicle_Sample_1_Labelled",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-NZ_2hrs_Drug_Sample_2_Labelled",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-NZ_2hrs_Drug_Sample_2_Labelled",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-NZ_0hrs_Sample_1_Extract",
                "characteristics": [],
                "name": "extract-NZ_0hrs_Sample_1_Extract",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-NZ_4hrs_Vehicle_Sample_2_Extract",
                "characteristics": [],
                "name": "extract-NZ_4hrs_Vehicle_Sample_2_Extract",
                "type": "Extract Name"
              },
              {
                "@id": "#material/labeledextract-NZ_2hrs_Vehicle_Sample_1_Labelled",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-NZ_2hrs_Vehicle_Sample_1_Labelled",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-NZ_2hrs_Drug_Sample_1_Labelled",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-NZ_2hrs_Drug_Sample_1_Labelled",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-NZ_0hrs_Sample_1_Labelled",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-NZ_0hrs_Sample_1_Labelled",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-NZ_1hrs_Drug_Sample_2_Labelled",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-NZ_1hrs_Drug_Sample_2_Labelled",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/labeledextract-NZ_0hrs_Sample_2_Labelled",
                "characteristics": [
                  {
                    "category": { "@id": "#characteristic_category/Label" },
                    "value": {
                      "annotationValue": "biotin"
                    }
                  }
                ],
                "name": "labeledextract-NZ_0hrs_Sample_2_Labelled",
                "type": "Labeled Extract Name"
              },
              {
                "@id": "#material/extract-NZ_1hrs_Drug_Sample_2_Extract",
                "characteristics": [],
                "name": "extract-NZ_1hrs_Drug_Sample_2_Extract",
                "type": "Extract Name"
              },
              {
                "@id": "#material/extract-NZ_2hrs_Drug_Sample_2_Extract",
                "characteristics": [],
                "name": "extract-NZ_2hrs_Drug_Sample_2_Extract",
                "type": "Extract Name"
              }
            ],
            "samples": [
              { "@id": "#sample/sample-NZ_0hrs_Grow_1" },
              { "@id": "#sample/sample-NZ_0hrs_Grow_2" }
            ]
          },
          "technologyPlatform": "Affymetrix",
          "filename": "a_microarray.txt"
        }
      ],
      "filename": "s_BII-S-2.txt",
      "factors": [
        {
          "@id": "#factor/compound",
          "factorType": {
            "annotationValue": "compound"
          },
          "factorName": "compound"
        },
        {
          "@id": "#factor/exposure_time",
          "factorType": {
            "termAccession": "http://www.ebi.ac.uk/efo/EFO_0000721",
            "termSource": "EFO",
            "annotationValue": "time"
          },
          "factorName": "exposure time"
        },
        {
          "@id": "#factor/dose",
          "factorType": {
            "termAccession": "http://www.ebi.ac.uk/efo/EFO_0000428",
            "termSource": "EFO",
            "annotationValue": "dose"
          },
          "factorName": "dose"
        }
      ],
      "publications": [
        {
          "doi": "",
          "pubMedID": "17439666",
          "status": {
            "annotationValue": "indexed in Pubmed"
          },
          "title": "Growth control of the eukaryote cell: a systems biology study in yeast.",
          "authorList": ""
        }
      ],
      "@id": "#study/BII-S-2",
      "materials": {
        "otherMaterials": [],
        "samples": [
          {
            "@id": "#sample/sample-NZ_0hrs_Grow_2",
            "derivesFrom": [ { "@id": "#source/source-Saccharomyces_cerevisiae_FY1679_" } ],
            "factorValues": [],
            "characteristics": [],
            "name": "sample-NZ_0hrs_Grow_2"
          },
          {
            "@id": "#sample/sample-NZ_0hrs_Grow_1",
            "derivesFrom": [ { "@id": "#source/source-Saccharomyces_cerevisiae_FY1679_" } ],
            "factorValues": [],
            "characteristics": [],
            "name": "sample-NZ_0hrs_Grow_1"
          }
        ],
        "sources": [
          {
            "@id": "#source/source-Saccharomyces_cerevisiae_FY1679_",
            "characteristics": [
              {
                "category": { "@id": "#characteristic_category/genotype" },
                "value": {
                  "annotationValue": "KanMx4 MATa/MATalpha ura3-52/ura3-52 leu2-1/+trp1-63/+his3-D200/+ hoD KanMx4/hoD"
                }
              },
              {
                "category": { "@id": "#characteristic_category/strain" },
                "value": {
                  "annotationValue": "FY1679"
                }
              },
              {
                "category": { "@id": "#characteristic_category/mating_type" },
                "value": {
                  "annotationValue": "mating_type_alpha"
                }
              },
              {
                "category": { "@id": "#characteristic_category/organism" },
                "value": {
                  "termSource": "NEWT",
                  "annotationValue": "Saccharomyces cerevisiae (Baker's yeast)"
                }
              }
            ],
            "name": "source-Saccharomyces cerevisiae FY1679 "
          }
        ]
      },
      "identifier": "BII-S-2",
      "title": "A time course analysis of transcription response in yeast treated with rapamycin, a specific inhibitor of the TORC1 complex: impact on yeast growth",
      "protocols": [
        {
          "parameters": [],
          "components": [],
          "description": "",
          "version": "",
          "@id": "#protocol/unknown",
          "name": "unknown",
          "protocolType": { "annotationValue": "" }
        },
        {
          "parameters": [],
          "components": [],
          "description": "For each target, a hybridisation cocktail was made using the standard array recipe as described in the GeneChip Expression Analysis technical manual. GeneChip control oligonucleotide and 20x eukaryotic hybridisation controls were used. Hybridisation buffer was made as detailed in the GeneChip manual and the BSA and herring sperm DNA was purchased from Invitrogen. The cocktail was heated to 99 C for 5mins, transferred to 45 C for 5 min and then spun for 5 min to remove any insoluble material. Affymetrix Yeast Yg_s98 S. cerevisiae arrays were pre-hybridised with 200 l 1x hybridisation buffer and incubated at 45 C for 10 min. 200 l of the hybridisation cocktail was loaded onto the arrays. The probe array was incubated in a rotisserie at 45 C, rotating at 60 rpm. Following hybridisation, for 16hr, chips were loaded onto a Fluidics station for washing and staining using the EukGe WS2v4 programme controlled using Microarray Suite 5 software.",
          "version": "",
          "@id": "#protocol/EukGE-WS4",
          "name": "EukGE-WS4",
          "protocolType": {
            "annotationValue": "hybridization"
          }
        },
        {
          "parameters": [],
          "components": [],
          "description": "The culture was grown in YMB minimum media + 2% glucose + supplement to early exponential growth (OD ~0.32)",
          "version": "",
          "@id": "#protocol/growth",
          "name": "growth",
          "protocolType": {
            "annotationValue": "growth"
          }
        },
        {
          "parameters": [],
          "components": [],
          "description": "1. Biomass samples (45ml) were taken via the sample port of the Applikon fermenters. The cells were pelleted by centrifugation for 5min at 5000 rpm. The supernatant was removed and the RNA pellet resuspended in the residual medium to form a slurry. This was added in a dropwise manner directly into a 5ml Teflon flask (B. Braun Biotech, Germany) containing liquid nitrogen and a 7 mm-diameter tungsten carbide ball. After allowing evaporation of the liquid nitrogen the flask was reassembled and the cells disrupted by agitation at 1500 rpm for 2 min in a Microdismembranator U (B. Braun Biotech, Germany) 2. The frozen powder was then dissolved in 1 ml of TriZol reagent (Sigma-Aldrich, UK), vortexed for 1 min, and then kept at room temperature for a further 5 min. 3. Chloroform extraction was performed by addition of 0.2 ml chloroform, shaking vigorouslyor 15 s, then 5 min incubation at room temperature. 4. Following centrifugation at 12,000 rpm for 5 min, the RNA (contained in the aqueous phase) was precipitated with 0.5 vol of 2-propanol at room temperature for 15 min. 5. After further centrifugation (12,000 rpm for 10 min at 4 C) the RNA pellet was washed twice with 70 % (v/v) ethanol, briefly air-dried, and redissolved in 0.5 ml diethyl pyrocarbonate (DEPC)-treated water. 6. The single-stranded RNA was precipitated once more by addition of 0.5 ml of LiCl bffer (4 M LiCl, 20 mM Tris-HCl, pH 7.5, 10 mM EDTA), thus removing tRNA and DNA from the sample. 7. After precipitation (20 C for 1h) and centrifugation (12,000 rpm, 30 min, 4 C), the RNA was washed twice in 70 % (v/v) ethanol prior to being dissolved in a minimal volume of DEPC-treated water. 8. Total RNA quality was checked using the RNA 6000 Nano Assay, and analysed on an Agilent 2100 Bioanalyser (Agilent Technologies). RNA was quantified using the Nanodrop ultra low volume spectrophotometer (Nanodrop Technologies).",
          "version": "",
          "@id": "#protocol/mRNA_extraction",
          "name": "mRNA extraction",
          "protocolType": {
            "annotationValue": "mRNA extraction"
          }
        },
        {
          "parameters": [],
          "components": [],
          "description": "This was done using Enzo BioArrayTM HighYieldTM RNA transcript labelling kit (T7) with 5 ul cDNA. The resultant cRNA was again purified using the GeneChip Sample Clean Up Module. The column was eluted in the first instance using 10 l RNase-free water, and for a second time using 11 l RNase-free water. cRNA was quantified using the Nanodrop spectrophotometer. A total of 15 ug of cRNA (required for hybridisation) was fragmented. Fragmentation was carried out by using 2 ul of fragmentation buffer for every 8 ul cRNA.",
          "version": "",
          "@id": "#protocol/biotin_labeling",
          "name": "biotin labeling",
          "protocolType": {
            "annotationValue": "labeling"
          }
        }
      ]
    }
  ],
  "publicReleaseDate": "2009-03-10",
  "ontologySourceReferences": [
    {
      "file": "http://bioportal.bioontology.org/ontologies/1123",
      "description": "Ontology for Biomedical Investigations",
      "name": "OBI",
      "version": "47893"
    },
    {
      "file": "ArrayExpress Experimental Factor Ontology",
      "description": "BRENDA tissue / enzyme source",
      "name": "BTO",
      "version": "v 1.26"
    },
    {
      "file": "",
      "description": "NEWT UniProt Taxonomy Database",
      "name": "NEWT",
      "version": "v 1.26"
    },
    {
      "file": "",
      "description": "Unit Ontology",
      "name": "UO",
      "version": "v 1.26"
    },
    {
      "file": "",
      "description": "Chemical Entities of Biological Interest",
      "name": "CHEBI",
      "version": "v 1.26"
    },
    {
      "file": "",
      "description": "Phenotypic qualities (properties)",
      "name": "PATO",
      "version": "v 1.26"
    },
    {
      "file": "",
      "description": "ArrayExpress Experimental Factor Ontology",
      "name": "EFO",
      "version": "v 1.26"
    }
  ],
  "comments": [
    {
      "value": "isaconfig-default_v2013-02-13",
      "name": "Last Opened With Configuration"
    },
    {
      "value": "",
      "name": "Created With Configuration"
    }
  ],
  "identifier": "BII-I-1",
  "title": "Growth control of the eukaryote cell: a systems biology study in yeast"
}

    """