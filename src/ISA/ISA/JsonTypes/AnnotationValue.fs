namespace ISA

type AnnotationValue = 
    | Text of string
    | Float of float
    | Int of int

    static member empty = Text ""

    /// Create a ISAJson Annotation value from a ISATab string entry
    static member fromString (s : string) = 
        try s |> int |> AnnotationValue.Int
        with | _ -> 
            try s |> float |> AnnotationValue.Float
            with
            | _ -> AnnotationValue.Text s

    /// Get a ISATab string Annotation Name from a ISAJson object
    static member toString (v : AnnotationValue) = 
        match v with
        | Text s -> s
        | Int i -> string i
        | Float f -> string f