module ArcTableTests

#if FABLE_COMPILER
open Fable.Mocha
#else
open Expecto
#endif


open ISA
open ISA.Spreadsheet

open TestingUtils
open TestObjects.ArcTable

let private groupCols = 
    testCase "groupCols" (fun () ->
        let ws = 
            initTableCols
                [
                    Protocol.REF.appendLolColumn
                    Protocol.Type.appendCollectionColumn
                    Parameter.appendTemperatureColumn
                    Parameter.appendInstrumentColumn
                    Characteristic.appendOrganismColumn
                    Factor.appendTimeColumn
                ]
        let grouped = ArcTable.groupColumnsByHeader ws
        
        let expectedHeaderGroups = 
            [   
                Protocol.REF.lolHeaderV1
                Protocol.Type.collectionHeaderV1+ ";" + Protocol.Type.collectionHeaderV2+ ";" +  Protocol.Type.collectionHeaderV3
                Parameter.temperatureHeaderV1+ ";" +  Parameter.temperatureHeaderV2+ ";" +  Parameter.temperatureHeaderV3+ ";" +  Parameter.temperatureHeaderV4
                Parameter.instrumentHeaderV1+ ";" +  Parameter.instrumentHeaderV2+ ";" +  Parameter.instrumentHeaderV3
                Characteristic.organismHeaderV1+ ";" +  Characteristic.organismHeaderV2+ ";" +  Characteristic.organismHeaderV3
                Factor.timeHeaderV1+ ";" +  Factor.timeHeaderV2+ ";" +  Factor.timeHeaderV3+ ";" + Factor.timeHeaderV4          
            ]
        let actualHeaderGroups =
            grouped
            |> List.map (fun cols -> cols |> List.map (fun c -> c.[1].Value) |> List.reduce (fun a b -> a + ";" + b))
        Expect.sequenceEqual actualHeaderGroups expectedHeaderGroups "Header groups did not match"
    )

let private simpleTable = 
    testList "simpleTable" [
        let wsName = "MyWorksheet"
        let ws = 
                initWorksheet wsName
                    [
                        Protocol.REF.appendLolColumn
                        Protocol.Type.appendCollectionColumn
                        Parameter.appendTemperatureColumn
                        Parameter.appendInstrumentColumn
                        Characteristic.appendOrganismColumn
                        Factor.appendTimeColumn
                    ]
        testCase "Read" (fun () -> 
                    
            let table = ArcTable.tryFromFsWorksheet ws        
        
            Expect.isSome table "Table was not created"
            let table = table.Value

            Expect.equal table.Name wsName "Name did not match"
            Expect.equal table.ColumnCount 6 "Wrong number of columns"
            Expect.equal table.RowCount 1 "Wrong number of rows"

            let expectedHeaders = 
                [
                        Protocol.REF.lolHeader
                        Protocol.Type.collectionHeader
                        Parameter.temperatureHeader
                        Parameter.instrumentHeader
                        Characteristic.organismHeader
                        Factor.timeHeader
                ]
            mySequenceEqual table.Headers expectedHeaders "Headers did not match"

            let expectedCells = 
                [
                        Protocol.REF.lolValue
                        Protocol.Type.collectionValue
                        Parameter.temperatureValue
                        Parameter.instrumentValue
                        Characteristic.organismValue
                        Factor.timeValue
                ]
            mySequenceEqual (table.GetRow(0)) expectedCells "Cells did not match"
        )
        testCase "Write" (fun () -> 
            
            let table = ArcTable.tryFromFsWorksheet ws        
            Expect.isSome table "Table was not created"
            let out = ArcTable.toFsWorksheet table.Value
            Expect.workSheetEqual out ws "Worksheet was not correctly written"
           
        )
    ]

let main = 
    testList "ArcTableTests" [
        simpleTable
        groupCols
    ]