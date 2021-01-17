namespace ISADotNet.Json

open ISADotNet
open System.Text.Json
open System.IO

module IO =

    module Protocol = 

        let fromString (s:string) = 
            JsonSerializer.Deserialize<Protocol>(s,JsonAnyOf.options)

        let toString (p:Protocol) = 
            JsonSerializer.Serialize<Protocol>(p,JsonAnyOf.options)

        let fromFile (path : string) = 
            File.ReadAllText path 
            |> fromString

        let toFile (path : string) (p:Protocol) = 
            File.WriteAllText(path,toString p)

    module Process = 

        let fromString (s:string) = 
            JsonSerializer.Deserialize<Process>(s,JsonAnyOf.options)

        let toString (p:Process) = 
            JsonSerializer.Serialize<Process>(p,JsonAnyOf.options)

        let fromFile (path : string) = 
            File.ReadAllText path 
            |> fromString

        let toFile (path : string) (p:Process) = 
            File.WriteAllText(path,toString p)


    module Investigation =
        
        let fromString (s:string) = 
            JsonSerializer.Deserialize<Investigation>(s,JsonAnyOf.options)

        let toString (i:Investigation) = 
            JsonSerializer.Serialize<Investigation>(i,JsonAnyOf.options)

        let fromFile (path : string) = 
            File.ReadAllText path 
            |> fromString

        let toFile (path : string) (i:Investigation) = 
            File.WriteAllText(path,toString i)
