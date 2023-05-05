namespace ISADotNet.Json

#if FABLE_COMPILER
open Thoth.Json
#else
open Thoth.Json.Net
#endif
open ISADotNet
open System.IO
open GEncode

module Value = 

    let fromString (s:string) = 
        JsonSerializer.Deserialize<Value>(s,JsonExtensions.options)

    let toString (v:Value) = 
        JsonSerializer.Serialize<Value>(v,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (v:Value) = 
        File.WriteAllText(path,toString v)

module Factor =  

    let fromString (s:string) = 
        JsonSerializer.Deserialize<Factor>(s,JsonExtensions.options)

    let toString (f:Factor) = 
        JsonSerializer.Serialize<Factor>(f,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (f:Factor) = 
        File.WriteAllText(path,toString f)


module FactorValue =

    let fromString (s:string) = 
        JsonSerializer.Deserialize<FactorValue>(s,JsonExtensions.options)

    let toString (f:FactorValue) = 
        JsonSerializer.Serialize<FactorValue>(f,JsonExtensions.options)

    let fromFile (path : string) = 
        File.ReadAllText path 
        |> fromString

    let toFile (path : string) (f:FactorValue) = 
        File.WriteAllText(path,toString f)