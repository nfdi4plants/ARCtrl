namespace ISADotNet.Json

open ISADotNet
open System.Text.Json
open System.IO

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