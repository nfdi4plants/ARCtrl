


[<EntryPoint>]
let main argv =
    
    LargeStudy.createStudy 10000
    |> LargeStudy.toWorkbook
    |> LargeStudy.fromWorkbook
    |> ignore
    1