namespace ISADotNet.Json

open ISADotNet
open System.Text.Json
open System.IO
open FSharp.SystemTextJson

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

module Protocol = 

    let fromString (s:string) = 
        JsonSerializer.Deserialize<Protocol>(s,JsonExtensions.options)

    let toString (p:Protocol) = 
        JsonSerializer.Serialize<Protocol>(p,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (p:Protocol) = 
        File.WriteAllText(path,toString p)

module Process = 

    let fromString (s:string) = 
        JsonSerializer.Deserialize<Process>(s,JsonExtensions.options)

    let toString (p:Process) = 
        JsonSerializer.Serialize<Process>(p,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (p:Process) = 
        File.WriteAllText(path,toString p)

module Assay = 

    let fromString (s:string) = 
        JsonSerializer.Deserialize<Assay>(s,JsonExtensions.options)

    let toString (a:Assay) = 
        JsonSerializer.Serialize<Assay>(a,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (a:Assay) = 
        File.WriteAllText(path,toString a)

module Person = 

    let fromString (s:string) = 
        JsonSerializer.Deserialize<Person>(s,JsonExtensions.options)

    let toString (p:Person) = 
        JsonSerializer.Serialize<Person>(p,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (p:Person) = 
        File.WriteAllText(path,toString p)

module Study =
    
    let fromString (s:string) = 
        JsonSerializer.Deserialize<Study>(s,JsonExtensions.options)

    let toString (s:Study) = 
        JsonSerializer.Serialize<Study>(s,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (s:Study) = 
        File.WriteAllText(path,toString s)

module Investigation =
        
    let fromString (s:string) = 
        JsonSerializer.Deserialize<Investigation>(s,JsonExtensions.options)

    let toString (i:Investigation) = 
        JsonSerializer.Serialize<Investigation>(i,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (i:Investigation) = 
        File.WriteAllText(path,toString i)
