namespace ISADotNet.Json

open ISADotNet
open System.Text.Json
open System.IO

module Investigation =
    
    let fromString (s:string) = 
        JsonSerializer.Deserialize<Investigation>(s,JsonExtensions.options)
        |> fun i -> 
            if isNull (box i.Remarks) then
                {i with Remarks = []}
            else i

    let toString (i:Investigation) = 
        JsonSerializer.Serialize<Investigation>(i,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString
        

    let toFile (path : string) (i:Investigation) = 
        File.WriteAllText(path,toString i)