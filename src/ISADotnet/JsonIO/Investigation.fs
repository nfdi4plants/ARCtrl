namespace ISADotNet.Json

open ISADotNet
open System.Text.Json
open System.IO

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