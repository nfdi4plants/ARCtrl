module TestObjects.ArcTable

open FsSpreadsheet
open ISA

module Parameter = 

    let temperatureHeader = 
        CompositeHeader.Parameter 
            (OntologyAnnotation.fromString("temperature","PATO","PATO:0000146"))
    let temperatureValue = 
        CompositeCell.createUnitized
            ("5",
            OntologyAnnotation.fromString("degree celsius","UO","UO:0000027"))

    let temperatureHeaderV1 = "Parameter [temperature]"
    let temperatureHeaderV2 = "Unit"
    let temperatureHeaderV3 = "Term Source REF (PATO:0000146)"
    let temperatureHeaderV4 = "Term Accession Number (PATO:0000146)"

    let temperatureValueV1 = "5"
    let temperatureValueV2 = "degree celsius"
    let temperatureValueV3 = "UO"
    let temperatureValueV4 = "http://purl.obolibrary.org/obo/UO_0000027"

    let appendTemperatureColumn (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.RangeAddress.Range = "A1:A1" then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs temperatureHeaderV1
        t.Cell(FsAddress(1, colCount + 2),c).SetValueAs temperatureHeaderV2
        t.Cell(FsAddress(1, colCount + 3),c).SetValueAs temperatureHeaderV3
        t.Cell(FsAddress(1, colCount + 4),c).SetValueAs temperatureHeaderV4
        t.Cell(FsAddress(2, colCount + 1),c).SetValueAs temperatureValueV1
        t.Cell(FsAddress(2, colCount + 2),c).SetValueAs temperatureValueV2 
        t.Cell(FsAddress(2, colCount + 3),c).SetValueAs temperatureValueV3
        t.Cell(FsAddress(2, colCount + 4),c).SetValueAs temperatureValueV4

    let instrumentHeader = 
        CompositeHeader.Parameter 
            (OntologyAnnotation.fromString("instrument model","MS","MS:1000031"))
    let instrumentValue = 
        CompositeCell.createTermFromString
            ("Thermo Fisher Scientific instrument model","MS","http://purl.obolibrary.org/obo/MS_1000483")


    let instrumentHeaderV1 = "Parameter [instrument model]"
    let instrumentHeaderV2 = "Term Source REF (MS:1000031)"
    let instrumentHeaderV3 = "Term Accession Number (MS:1000031)"

    let instrumentValueV1 = "Thermo Fisher Scientific instrument model"
    let instrumentValueV2 = "MS"
    let instrumentValueV3 = "http://purl.obolibrary.org/obo/MS_1000483"

    let appendInstrumentColumn (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.RangeAddress.Range = "A1:A1" then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs instrumentHeaderV1
        t.Cell(FsAddress(1, colCount + 2),c).SetValueAs instrumentHeaderV2
        t.Cell(FsAddress(1, colCount + 3),c).SetValueAs instrumentHeaderV3
        t.Cell(FsAddress(2, colCount + 1),c).SetValueAs instrumentValueV1
        t.Cell(FsAddress(2, colCount + 2),c).SetValueAs instrumentValueV2
        t.Cell(FsAddress(2, colCount + 3),c).SetValueAs instrumentValueV3


module Characteristic = 

    let organismHeader = 
        CompositeHeader.Characteristic 
            (OntologyAnnotation.fromString("organism","OBI","OBI:0100026"))
    let organismValue = 
        CompositeCell.createTermFromString
            ("Arabidopsis thaliana","NCBITaxon","http://purl.obolibrary.org/obo/NCBITaxon_3702")

    let organismHeaderV1 = "Characteristic [organism]"
    let organismHeaderV2 = "Term Source REF (OBI:0100026)"
    let organismHeaderV3 = "Term Accession Number (OBI:0100026)"

    let organismValueV1 = "Arabidopsis thaliana"
    let organismValueV2 = "NCBITaxon"
    let organismValueV3 = "http://purl.obolibrary.org/obo/NCBITaxon_3702"

    let appendOrganismColumn (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.RangeAddress.Range = "A1:A1" then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs organismHeaderV1
        t.Cell(FsAddress(1, colCount + 2),c).SetValueAs organismHeaderV2
        t.Cell(FsAddress(1, colCount + 3),c).SetValueAs organismHeaderV3
        t.Cell(FsAddress(2, colCount + 1),c).SetValueAs organismValueV1
        t.Cell(FsAddress(2, colCount + 2),c).SetValueAs organismValueV2
        t.Cell(FsAddress(2, colCount + 3),c).SetValueAs organismValueV3

module Factor = 

    let timeHeader = 
        CompositeHeader.Factor 
            (OntologyAnnotation.fromString("time","PATO","PATO:0000165"))
    let timeValue = 
        CompositeCell.createUnitized
            ("10",
            OntologyAnnotation.fromString("hour","UO","UO:0000032"))

    let timeHeaderV1 = "Factor [time]"
    let timeHeaderV2 = "Unit"
    let timeHeaderV3 = "Term Source REF (PATO:0000165)"
    let timeHeaderV4 = "Term Accession Number (PATO:0000165)"

    let timeValueV1 = "10"
    let timeValueV2 = "hour"
    let timeValueV3 = "UO"
    let timeValueV4 = "http://purl.obolibrary.org/obo/UO_0000032"

    let appendTimeColumn (c : FsCellsCollection) (t : FsTable) = 
        let colCount = if t.RangeAddress.Range = "A1:A1" then 0 else t.ColumnCount()
        t.Cell(FsAddress(1, colCount + 1),c).SetValueAs timeHeaderV1
        t.Cell(FsAddress(1, colCount + 2),c).SetValueAs timeHeaderV2
        t.Cell(FsAddress(1, colCount + 3),c).SetValueAs timeHeaderV3
        t.Cell(FsAddress(1, colCount + 4),c).SetValueAs timeHeaderV4
        t.Cell(FsAddress(2, colCount + 1),c).SetValueAs timeValueV1
        t.Cell(FsAddress(2, colCount + 2),c).SetValueAs timeValueV2 
        t.Cell(FsAddress(2, colCount + 3),c).SetValueAs timeValueV3
        t.Cell(FsAddress(2, colCount + 4),c).SetValueAs timeValueV4

module Protocol =
    module REF = 

        let lolHeader = 
            CompositeHeader.ProtocolREF

        let lolValue = CompositeCell.FreeText "LOL"

        let lolHeaderV1 = "Protocol REF"
        let lolValueV1 = "LOL"

        let appendLolColumn (c : FsCellsCollection) (t : FsTable) = 
            let colCount = if t.RangeAddress.Range = "A1:A1" then 0 else t.ColumnCount()
            t.Cell(FsAddress(1, colCount + 1),c).SetValueAs lolHeaderV1
            t.Cell(FsAddress(2, colCount + 1),c).SetValueAs lolValueV1

    module Type = 
        
        let collectionHeader = 
            CompositeHeader.ProtocolType
        
        let collectionValue =
            CompositeCell.createTermFromString
                ("sample collection protocol","DPBO","http://purl.obolibrary.org/obo/DPBO_1000169")

        let collectionHeaderV1 = "Protocol Type"
        let collectionHeaderV2 = "Term Source REF (PATO:0000165)"
        let collectionHeaderV3 = "Term Accession Number (PATO:0000165)"

        let collectionValueV1 = "sample collection protocol"
        let collectionValueV2 = "DPBO"
        let collectionValueV3 = "http://purl.obolibrary.org/obo/DPBO_1000169"

        let appendCollectionColumn (c : FsCellsCollection) (t : FsTable) = 
            let colCount = if t.RangeAddress.Range = "A1:A1" then 0 else t.ColumnCount()
            t.Cell(FsAddress(1, colCount + 1),c).SetValueAs collectionHeaderV1
            t.Cell(FsAddress(1, colCount + 2),c).SetValueAs collectionHeaderV2
            t.Cell(FsAddress(1, colCount + 3),c).SetValueAs collectionHeaderV3
            t.Cell(FsAddress(2, colCount + 1),c).SetValueAs collectionValueV1
            t.Cell(FsAddress(2, colCount + 2),c).SetValueAs collectionValueV2
            t.Cell(FsAddress(2, colCount + 3),c).SetValueAs collectionValueV3

let initTable (appendOperations : (FsCellsCollection -> FsTable -> unit) list)= 
    let c = FsCellsCollection()
    let t = FsTable("AnnotationTable", FsRangeAddress("A1:A1"))
    appendOperations 
    |> List.iter (fun o -> o c t)
    c,t

let initTableCols (appendOperations : (FsCellsCollection -> FsTable -> unit) list)= 
    let c,t = initTable appendOperations
    t.Columns(c)
    |> Seq.toList