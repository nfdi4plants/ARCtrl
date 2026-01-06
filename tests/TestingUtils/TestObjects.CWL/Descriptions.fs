module TestObjects.CWL.Descriptions

let label = """label: An example tool demonstrating metadata."""
let docSimple = """doc: Note that this is an example and the metadata is not necessarily consistent."""

let descriptionFileContentComplex = """doc: |
  DESeq2 example workflow for **differential gene expression analysis**
  
  This workflow runs DESeq2 on the output of the kallisto workflow
  and the metadata file.
  It runs an R script, deseq2.R, which ideally should be split into three sub scripts and accordingly three workflow steps
    1. Read kallsito data
    2. Prep / run deseq2
    3. Plot results

  ## DESeq2 docs:
    https://bioconductor.org/packages/release/bioc/html/DESeq2.html

  ## Importing kallisto output with tximport
    https://bioconductor.org/packages/release/bioc/vignettes/tximport/inst/doc/tximport.html#kallisto

  ## Multi-package containers
  - R and combinations of library dependencies are available as multi-package containers from [BioContainers](https://github.com/BioContainers/multi-package-containers)
  - Searched for `repo:BioContainers/multi-package-containers deseq2 tximport rhdf5`
  - and found `quay.io/biocontainers/mulled-v2-05fd88b9ac812a9149da2f2d881d62f01cc49835:a10f0e3a7a70fc45494f8781d33901086d2214d0-0` :tada:"""

let descriptionFileContentComplexDecoded = """DESeq2 example workflow for **differential gene expression analysis**

This workflow runs DESeq2 on the output of the kallisto workflow
and the metadata file.
It runs an R script, deseq2.R, which ideally should be split into three sub scripts and accordingly three workflow steps
1. Read kallsito data
2. Prep / run deseq2
3. Plot results
## DESeq2 docs:
https://bioconductor.org/packages/release/bioc/html/DESeq2.html
## Importing kallisto output with tximport
https://bioconductor.org/packages/release/bioc/vignettes/tximport/inst/doc/tximport.html#kallisto
## Multi-package containers
- R and combinations of library dependencies are available as multi-package containers from [BioContainers](https://github.com/BioContainers/multi-package-containers)
- Searched for `repo:BioContainers/multi-package-containers deseq2 tximport rhdf5`
- and found `quay.io/biocontainers/mulled-v2-05fd88b9ac812a9149da2f2d881d62f01cc49835:a10f0e3a7a70fc45494f8781d33901086d2214d0-0` :tada:"""

let descriptionFileContentComplexEncoded = """doc: DESeq2 example workflow for **differential gene expression analysis**

This workflow runs DESeq2 on the output of the kallisto workflow
and the metadata file.
It runs an R script, deseq2.R, which ideally should be split into three sub scripts and accordingly three workflow steps
  1. Read kallsito data
  2. Prep / run deseq2
  3. Plot results

## DESeq2 docs:
  https://bioconductor.org/packages/release/bioc/html/DESeq2.html

## Importing kallisto output with tximport
  https://bioconductor.org/packages/release/bioc/vignettes/tximport/inst/doc/tximport.html#kallisto

## Multi-package containers
- R and combinations of library dependencies are available as multi-package containers from [BioContainers](https://github.com/BioContainers/multi-package-containers)
- Searched for `repo:BioContainers/multi-package-containers deseq2 tximport rhdf5`
- and found `quay.io/biocontainers/mulled-v2-05fd88b9ac812a9149da2f2d881d62f01cc49835:a10f0e3a7a70fc45494f8781d33901086d2214d0-0` :tada:"""