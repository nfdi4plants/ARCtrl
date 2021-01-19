namespace ISADotNet.Json

open ISADotNet
open System.Text.Json
open System.IO


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
