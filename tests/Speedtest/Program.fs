
open ARCtrl
open ARCtrl.ISA

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
    elif Array.contains "--addRows" argv then
        let t1,t2 = AddRows.prepareTables()
        AddRows.oldF t1
        AddRows.newF t2
        1  
    elif Array.contains "--fillMissing" argv then
        let t1,t2,t3,t4 = FillMissing.prepareTables()
        FillMissing.firstF t1
        FillMissing.oldF t2
        FillMissing.newF t3
        FillMissing.newSeqF t4
        1

    else 
        0
