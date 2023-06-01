namespace ISA

type CompositeCell = 
    | Term of OntologyAnnotation
    | Freetext of string
    | WithUnit of string*OntologyAnnotation