namespace ISADotNet.Json

open ISADotNet
open System.Text.Json
open System.IO

module Publication = 

    let fromString (s:string) = 
        JsonSerializer.Deserialize<Publication>(s,JsonExtensions.options)

    let toString (p:Publication) = 
        JsonSerializer.Serialize<Publication>(p,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (p:Publication) = 
        File.WriteAllText(path,toString p)