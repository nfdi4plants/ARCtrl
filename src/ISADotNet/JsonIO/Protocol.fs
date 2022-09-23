namespace ISADotNet.Json

open ISADotNet
open System.Text.Json
open System.IO

module ProtocolParameter =

    let fromString (s:string) = 
        JsonSerializer.Deserialize<ProtocolParameter>(s,JsonExtensions.options)

    let toString (p:ProtocolParameter) = 
        JsonSerializer.Serialize<ProtocolParameter>(p,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (p:ProtocolParameter) = 
        File.WriteAllText(path,toString p)

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