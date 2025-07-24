module CompositeCellTests

open TestingUtils
open ARCtrl
open ARCtrl.Spreadsheet

let main = 
    testList "CompositeCell" [
        testList "Data" [
            testCase "ReadFullPathWith./" <| fun () -> 
                let s = "./assays/MassHunter_targets/dataset/QuantResults/22-0005_exp001.batch_a.bin.batch.bin"
                let c = CompositeCell.dataFromStringCells None None [|s|]
                Expect.isTrue c.isData "Cell should be a data cell"
                let d = c.AsData
                let name = Expect.wantSome d.Name "Data cell should have a name"
                Expect.equal name s "FullPath should match the input string"
                Expect.isNone d.Format "Data cell should not have a format"
                Expect.isNone d.Selector "Data cell should not have a selector"
        ]
    ]