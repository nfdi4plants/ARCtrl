#r "nuget: ISADotNet"

open ISADotNet

fsi.AddPrinter(fun (x:option<IISAPrintable>) -> 
    match x with
    | Some v -> v.PrintCompact()
    | None -> "None"
)   

fsi.AddPrinter(fun (x:seq<IISAPrintable>) -> 
    x 
    |> Seq.map (fun x -> x.PrintCompact())
    |> Seq.reduce (fun a b -> a + "; " + b)
    |> sprintf "[%s]"
)   

fsi.AddPrinter(fun (x:list<IISAPrintable>) -> 
    x 
    |> Seq.map (fun x -> x.PrintCompact())
    |> Seq.reduce (fun a b -> a + "; " + b)
    |> sprintf "[%s]"
)   