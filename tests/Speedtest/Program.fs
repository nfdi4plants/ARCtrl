
open ARCtrl
open ARCtrl.Json


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
    #if !FABLE_COMPILER
    elif Array.contains "--bigJson" argv then
        let createAssay() = 
            let a = ArcAssay.init("MyAssay")
            let t = a.InitTable("MyTable")
            t.AddColumn(CompositeHeader.Input IOType.Source)
            t.AddColumn(CompositeHeader.Parameter (OntologyAnnotation("MyParameter1")))
            t.AddColumn(CompositeHeader.Parameter (OntologyAnnotation("MyParameter2")))
            t.AddColumn(CompositeHeader.Parameter (OntologyAnnotation("MyParameter3")))
            t.AddColumn(CompositeHeader.Characteristic (OntologyAnnotation("MyCharacteristic")))
            t.AddColumn(CompositeHeader.Output IOType.Sample)
            let rowCount = 10000
            printfn "rowCount: %d" rowCount
            for i = 0 to rowCount - 1 do
                let cells =             
                    [|
                        CompositeCell.FreeText $"Source{i}"
                        CompositeCell.FreeText $"Parameter1_value"
                        CompositeCell.FreeText $"Parameter2_value"
                        CompositeCell.FreeText $"Parameter3_value{i - i % 10}"
                        CompositeCell.FreeText $"Characteristic_value"
                        CompositeCell.FreeText $"Sample{i}"
                    |]
                for j = 0 to cells.Length - 1 do
                    ArcTableAux.Unchecked.setCellAt (j,i,cells.[j]) t.Values
            a
        let toJson(a : ArcAssay) = 
            ArcAssay.toISAJsonString() a
        let toFS(a : string) =
            System.IO.File.WriteAllText((__SOURCE_DIRECTORY__ + "/big.json"), a)

        createAssay()
        |> toJson
        |> toFS
        1
    #endif
    else 
        //let argumentNumber = 
        //    #if FABLE_COMPILER_JAVASCRIPT
        //    1
        //    #else 
        //    0
        //    #endif
        let cpu = argv.[0]
        PerformanceReport.runReport cpu