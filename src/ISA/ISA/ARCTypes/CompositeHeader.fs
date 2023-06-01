namespace ISA

type CompositeHeader = 
    | Component         of OntologyAnnotation
    | Characteristic    of OntologyAnnotation
    | Factor            of OntologyAnnotation
    | Parameter         of OntologyAnnotation

    | ProtocolDescription
    | ProtocolUri
    | ProtocolVersion
    | ProtocolREF
    | ProtocolType

    | Performer
    | Date

    | FreeText of string

    with 

    static member deprecatedIOHeaderWarning (s : string) = ""

    static member getDeprecationWarning headers = 
        headers
        |> List.choose (fun i ->
            match i with 
            | FreeText s when s = "Sample" -> Some <| CompositeHeader.deprecatedIOHeaderWarning s
            | _ -> None   
        )