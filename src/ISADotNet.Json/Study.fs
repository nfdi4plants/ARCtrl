namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISADotNet
open System.IO
open GEncode

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