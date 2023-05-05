namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISADotNet
open System.IO
open GEncode

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