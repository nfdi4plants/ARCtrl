namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISADotNet
open System.IO
open GEncode

module ProcessParameterValue =

    let fromString (s:string) = 
        JsonSerializer.Deserialize<ProcessParameterValue>(s,JsonExtensions.options)

    let toString (p:ProcessParameterValue) = 
        JsonSerializer.Serialize<ProcessParameterValue>(p,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (p:ProcessParameterValue) = 
        File.WriteAllText(path,toString p)

/// Functions for handling the ProcessInput Type
module ProcessInput =

    let fromString (s:string) = 
        JsonSerializer.Deserialize<ProcessInput>(s,JsonExtensions.options)

    let toString (p:ProcessInput) = 
        JsonSerializer.Serialize<ProcessInput>(p,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (p:ProcessInput) = 
        File.WriteAllText(path,toString p)

/// Functions for handling the ProcessOutput Type
module ProcessOutput =

    let fromString (s:string) = 
        JsonSerializer.Deserialize<ProcessOutput>(s,JsonExtensions.options)

    let toString (p:ProcessOutput) = 
        JsonSerializer.Serialize<ProcessOutput>(p,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (p:ProcessOutput) = 
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

module ProcessSequence = 

    let fromString (s:string) = 
        JsonSerializer.Deserialize<Process list>(s,JsonExtensions.options)

    let toString (p:Process list) = 
        JsonSerializer.Serialize<Process list>(p,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (p:Process list) = 
        File.WriteAllText(path,toString p)