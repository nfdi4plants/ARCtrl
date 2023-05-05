namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISADotNet
open System.IO
open GEncode

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