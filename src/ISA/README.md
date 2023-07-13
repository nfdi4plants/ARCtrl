### ISA datamodel

#### Requirements
1. MUST be parseable to a full representation of valid ISA-json
2. MUST contain structural information of isa tables (ISA-Tab and ISA-XLSX)
3. MUST allow for low level api calls (e.g. addParameterValue should be intuitive)
4. SHOULD be performant enough for use in GUI applications


#### Mögliche Lösungen:**

1. **Bestehendes ISA Schema nutzen**
    - Komplette strukturinformation via ISA comments (ISA assay spreadsheet json als comment auf Assay typ) 
    - Potentielle erweiterung für ISA schema -> ISA 2.0, schon direkt mit 1.0 compatibility lösung über comments
    - aus ISA json vs. XLSX wird ein uniformes format
    - Machen wir eh schon so -> kein extra arbeitsaufwand
    - import/export von standard ISA Json möglich
    
2. 'Structural' Metadata field on `ArcDataModel`
    - superset von ISA wird verhindert, wir können das einfach in unserem schema machen
    - problem: ISA json bleibt fallback für kommunikation mit externen tools
   


```mermaid
flowchart TD


subgraph ISAModel

    person[Person]
    publication[Publication]

    ch[CompositeHeader]
    cc[CompositeCells]
    oa[OntologyAnnotation]

    table[ArcTable<br><i>= Process + Protocol</i>]

    assay[ArcAssay]
    study[ArcStudy]
    investigation[ArcInvestigation]

end 

table --> ch
table --> cc
ch --> oa
cc --> oa

assay --> table
study --> table

study --> assay
investigation --> study

study --> person
investigation --> person
study --> publication
investigation --> publication

subgraph Legend
    json
    tab
end

%% Colorscheme
%% https://paletton.com/#uid=33m0E0kqOtKgGDvlJvDu0oyxCjs

style json fill:#5AA8B7,stroke:#0A6778,stroke-width:2px,color: black
style tab fill:#F04D63,stroke:#BA0C24,stroke-width:2px,color: black

style ch fill:#F04D63,stroke:#BA0C24,stroke-width:2px,color: black
style cc fill:#F04D63,stroke:#BA0C24,stroke-width:2px,color: black
style oa fill:#5AA8B7,stroke:#0A6778,stroke-width:2px,color: black
style person fill:#5AA8B7,stroke:#0A6778,stroke-width:2px,color: black
style publication fill:#5AA8B7,stroke:#0A6778,stroke-width:2px,color: black

style table fill:#F04D63,stroke:#BA0C24,stroke-width:4px,color: black
style assay stroke:#BA0C24,stroke-width:2px
style study stroke:#BA0C24,stroke-width:2px
style investigation stroke:#BA0C24,stroke-width:2px

```

### Annotation Term Handling

When creating an `Ontology Annotation` object, we should save the input string of the `Term Accession Number`. Like this, we can still get Short Accession and PURL when possible, but keep the possibility of returning the original input.

```mermaid
flowchart LR
obo[http://obo.obo/obo/EFO_0000721]
short[EFO:0000721]
other[http://www.ebi.ac.uk/efo/EFO_0000721]

subgraph OntologyAnnotation
    oboO[http://obo.obo/obo/EFO_0000721]
    shortO[EFO:0000721]
    otherO[http://www.ebi.ac.uk/efo/EFO_0000721]       
end

shortOut[EFO:0000721]
oboOut[http://obo.obo/obo/EFO_0000721]

otherAny[http://www.ebi.ac.uk/efo/EFO_0000721]

obo --> oboO
short --> shortO
other --> otherO

oboO --GetShort--> shortOut
oboO --GetPURL--> oboOut
oboO --GetOriginal--> oboOut

otherO --GetShort--> shortOut
otherO --GetPURL--> oboOut
otherO --GetOriginal--> otherAny

shortO --GetShort--> shortOut
shortO --GetPURL--> oboOut
shortO --GetOriginal--> shortOut
```