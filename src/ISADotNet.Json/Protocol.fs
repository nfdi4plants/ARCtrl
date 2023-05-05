namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISADotNet
open System.IO
open GEncode
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