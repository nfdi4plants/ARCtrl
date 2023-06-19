#r "nuget: Fable.Core, 4.0.0"

#I @"../src\ISA\ISA/bin\Debug\netstandard2.0"
#r "ISA.dll"


open ISA
open System.IO

let TableName = "Test"
let oa_species = OntologyAnnotation.fromString("species", "GO", "GO:0123456")
let oa_chlamy = OntologyAnnotation.fromString("Chlamy", "NCBI", "NCBI:0123456")
let oa_instrumentModel = OntologyAnnotation.fromString("instrument model", "MS", "MS:0123456")
let oa_SCIEXInstrumentModel = OntologyAnnotation.fromString("SCIEX instrument model", "MS", "MS:654321")
let oa_temperature = OntologyAnnotation.fromString("temperature","NCIT","NCIT:0123210")
let header_input = CompositeHeader.Input IOType.Source
let header_chara = CompositeHeader.Characteristic oa_species
let createCells_chara (count) = Array.init count (fun _ -> CompositeCell.createTerm oa_chlamy)
let createCells_freetext pretext (count) = Array.init count (fun i -> CompositeCell.createFreeText  $"{pretext}_{i}") 
let createCells_FreeText pretext (count) = Array.init count (fun i -> CompositeCell.createFreeText  $"{pretext}_{i}") 
let createCells_Term (count) = Array.init count (fun _ -> CompositeCell.createTerm oa_SCIEXInstrumentModel)
let createCells_Unitized (count) = Array.init count (fun i -> CompositeCell.createUnitized (string i,OntologyAnnotation.empty))
let column_input = CompositeColumn.create(CompositeHeader.Input IOType.Source, createCells_FreeText "Source" 5)
let column_output = CompositeColumn.create(CompositeHeader.Output IOType.Sample, createCells_FreeText "Sample" 5)
let column_component = CompositeColumn.create(CompositeHeader.Component oa_instrumentModel, createCells_Term 5)
let column_param = CompositeColumn.create(CompositeHeader.Parameter OntologyAnnotation.empty, createCells_Unitized 5)
/// Valid TestTable with 5 columns, no cells: 
///
/// Input [Source] -> 5 cells: [Source_1; Source_2..]
///
/// Output [Sample] -> 5 cells: [Sample_1; Sample_2..]
///
/// Component [instrument model] -> 5 cells: [SCIEX instrument model; SCIEX instrument model; ..]
///
/// Parameter [empty] -> 5 cells: [0, oa.empty; 1, oa.empty; 2, oa.empty ..]    
///
/// Parameter [empty] -> 5 cells: [0, oa.empty; 1, oa.empty; 2, oa.empty ..]   
let create_testTable() = 
    let t = ArcTable.init(TableName)
    let columns = [|
        column_input
        column_output
        column_component
        column_param
        column_param
    |]
    t.AddColumns(columns)
    t

/// This function can be used to put ArcTable.Values into a nice format for printing/writing to IO
let tableValues_printable (table:ArcTable) = 
    [
        for KeyValue((c,r),v) in table.Values do
            yield c,r,v
    ]
    |> List.groupBy (fun (x,y,z) -> x)
    |> List.collect (fun (c,arr) ->
        [
            yield $"[COL] {c} - {table.Headers.[c]}"
            for _,r,v in arr do
                yield
                    $"[ROW] {r} - {v}"
        ]
    )

let testTable = create_testTable()
tableValues_printable testTable
let newInputCol = CompositeColumn.create(CompositeHeader.Input IOType.RawDataFile, createCells_FreeText "NEW" 5)
let columns = [|
    column_param
    // newInputCol
    // column_param
    // column_param
|]
testTable.AddColumns(columns, 0, true)
tableValues_printable testTable
testTable.Headers |> Array.ofSeq |> Array.length

for colI = 4 to 0 do
    printfn "hello world %i" colI

for columnIndex in 4 .. -1 .. 0 do
    printfn "[columnIndex in lastColumnIndex .. index]: %i" columnIndex
    printfn "[numberOfNewColumns]"