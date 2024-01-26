


[<EntryPoint>]
let main argv =
    
    if Array.contains "--manyStudies" argv then
        ManyStudies.write() |> ignore
        1
    elif Array.contains "--largeStudy" argv then

        LargeStudy.createStudy 10000
        |> LargeStudy.toWorkbook
        |> LargeStudy.fromWorkbook
        |> ignore
        1
    else 
        0
