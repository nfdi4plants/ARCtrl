namespace ARCtrl

open System.Collections.Generic
open ARCtrl.Helper 
open ARCtrl

module DataMapAux = 
    
    [<Literal>]
    let dataMapName = "DataMap"

    let explicationHeader = CompositeHeader.Parameter

    let allowedHeaders = 1

    let validate (headers : ResizeArray<CompositeHeader>) (values : System.Collections.Generic.Dictionary<int*int,CompositeCell>) = 
        
        1

type DataMap(headers: ResizeArray<CompositeHeader>, values: System.Collections.Generic.Dictionary<int*int,CompositeCell>) = 
    
    let _ = DataMapAux.validate headers values

    let table = ArcTable(DataMapAux.dataMapName, headers, values)

