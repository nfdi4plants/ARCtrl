namespace ISADotNet

module IO =
    open FSharp.Data
    
    module Json = 

        module private Json =

            let fromString (s:string) = 
                JsonValue.Parse s
    
            let item (name : string) (obj : JsonValue) =
                obj.Item name
    
            let tryItem (name : string) (obj : JsonValue) =
                try
                    obj.Item name
                    |> Some
                with
                | _ -> None
    
            let toArray (obj : JsonValue) =
                obj.AsArray()
    
            let toString (obj : JsonValue) =
                obj.AsString()
    
            let toFloat (obj : JsonValue) =
                obj.AsFloat()       
        
        let readComment (comment : JsonValue)= 
            {
                Name = Json.item Comment.NameLabel comment |> Json.toString
                Value = Json.tryItem Comment.ValueLabel comment |> Option.map Json.toString |> Option.defaultValue ""
            }
        
        let readOntologyAnnotation (ontology : JsonValue) =
            {
                Name =                  Json.item OntologyAnnotation.NameLabel ontology |> Json.toString
                TermSourceREF =         Json.tryItem OntologyAnnotation.TermSourceREFLabel ontology |> Option.map Json.toString |> Option.defaultValue ""
                TermAccessionNumber =   Json.tryItem OntologyAnnotation.TermAccessionNumberLabel ontology |> Option.map Json.toString |> Option.defaultValue ""
                Comments =              Json.tryItem OntologyAnnotation.CommentsLabel ontology |> Option.map (Json.toArray >> (Array.map readComment)) |> Option.defaultValue [||]
            }
        
        let readValue (value : JsonValue) =
            try readOntologyAnnotation value |> Ontology
            with
            | _ ->
                try Json.toFloat value |> Numeric
                with
                | _ -> Json.toString value |> Name
                        
        let readParameterValue (paramValue : JsonValue) =
            {
                Category = Json.item ParameterValue.CategoryLabel paramValue |> Json.item "parameterName" |> readOntologyAnnotation
                Value = Json.item ParameterValue.ValueLabel paramValue |> readValue
                Unit = Json.tryItem ParameterValue.UnitLabel paramValue |> Option.map readOntologyAnnotation       
            }
                      
        let readComponent (componentP : JsonValue) =
            {
                ComponentName = Json.item Component.NameLabel componentP |> Json.toString
                ComponentType = Json.tryItem Component.TypeLabel componentP |> Option.map readOntologyAnnotation
            }
                
        let readProtocol (protocol : JsonValue) =
            {       
                Name =          Json.item Protocol.NameLabel protocol |> Json.toString
                ProtocolType =  Json.tryItem Protocol.TypeLabel protocol |> Option.map readOntologyAnnotation
                Description =   Json.tryItem Protocol.DescriptionLabel protocol |> Option.map Json.toString |> Option.defaultValue ""
                Uri =           Json.tryItem Protocol.UriLabel protocol |> Option.map Json.toString |> Option.defaultValue ""
                Version =       Json.tryItem Protocol.VersionLabel protocol |> Option.map Json.toString |> Option.defaultValue ""
                Parameters =    Json.tryItem Protocol.ParametersLabel protocol |> Option.map (Json.toArray >> (Array.map (Json.item "parameterName" >> readOntologyAnnotation))) |> Option.defaultValue [||]
                Components =    Json.tryItem Protocol.ComponentsLabel protocol |> Option.map (Json.toArray >> (Array.map readComponent)) |> Option.defaultValue [||]
                Comments =      Json.tryItem Protocol.CommentsLabel protocol |> Option.map (Json.toArray >> (Array.map readComment)) |> Option.defaultValue [||]
            }
        
        let rec readProcess (processP : JsonValue) =
            {
                Name =              Json.item Process.NameLabel processP |> Json.toString
                ExecutesProtocol =  Json.tryItem Process.ExecutesProtocolLabel processP |> Option.map readProtocol
                ParameterValues =   Json.tryItem Process.ParameterValuesLabel processP |> Option.map (Json.toArray >> (Array.map readParameterValue)) |> Option.defaultValue [||]
                Performer =         Json.tryItem Process.PerformerLabel processP |> Option.map Json.toString |> Option.defaultValue ""
                Date =              Json.tryItem Process.DateLabel processP |> Option.map Json.toString |> Option.defaultValue ""
                PreviousProcess =   Json.tryItem Process.PreviousProcessLabel processP |> Option.map readProcess
                NextProcess =       Json.tryItem Process.NextProcessLabel processP |> Option.map readProcess
                Inputs =            Json.tryItem Process.InputsLabel processP |> Option.map (Json.toArray >> Array.map Json.toString) |> Option.defaultValue [||]
                Outputs =           Json.tryItem Process.OutputsLabel processP |> Option.map (Json.toArray >> Array.map Json.toString) |> Option.defaultValue [||]
                Comments =          Json.tryItem Process.CommentsLabel processP |> Option.map (Json.toArray >> (Array.map readComment)) |> Option.defaultValue [||]
            }
        
        let fromString (readerFunction : JsonValue -> 'T) (s : string) =
            Json.fromString s
            |> readerFunction

        let fromFile (readerFunction : JsonValue -> 'T) (path : string) =
            System.IO.File.ReadAllText path
            |> fromString readerFunction
