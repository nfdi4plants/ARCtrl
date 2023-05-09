namespace ISADotNet.Json

open ISADotNet
open System.Text.Json
open System.IO

module Comment = 

    let fromString (s:string) = 
        JsonSerializer.Deserialize<Comment>(s,JsonExtensions.options)

    let toString (c:Comment) = 
        JsonSerializer.Serialize<Comment>(c,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (c:Comment) = 
        File.WriteAllText(path,toString c)
