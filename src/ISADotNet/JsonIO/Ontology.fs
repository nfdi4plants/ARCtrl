namespace ISADotNet.Json

open ISADotNet
open System.Text.Json
open System.IO

module OntologySourceReference =

    let fromString (s:string) = 
        JsonSerializer.Deserialize<OntologySourceReference>(s,JsonExtensions.options)

    let toString (oa:OntologySourceReference) = 
        JsonSerializer.Serialize<OntologySourceReference>(oa,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (oa:OntologySourceReference) = 
        File.WriteAllText(path,toString oa)

module OntologyAnnotation =  

    let fromString (s:string) = 
        JsonSerializer.Deserialize<OntologyAnnotation>(s,JsonExtensions.options)

    let toString (oa:OntologyAnnotation) = 
        JsonSerializer.Serialize<OntologyAnnotation>(oa,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (oa:OntologyAnnotation) = 
        File.WriteAllText(path,toString oa)