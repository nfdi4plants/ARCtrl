namespace ISADotNet.QueryModel.Linq

open System.Collections.Generic
open System.Collections
open Microsoft.FSharp.Linq.RuntimeHelpers
open Microsoft.FSharp.Quotations.Patterns
open FsSpreadsheet.DSL
open ISADotNet.QueryModel
open Helpers

module Spreadsheet =

    type CellBuilder() =
        inherit ISAQueryBuilder()

        //let mutable isOptional = false
        //let index = None
        
        [<CustomOperation("required")>] 
        member this.Required (source) =
            this.AddMessage $"as required"
            //isOptional <- false
            source

        [<CustomOperation("optional")>] 
            member this.Optional (source: 'T)  =
                this.AddMessage $"as optional"
                //isOptional <- true
                OptionSource(source)

        //member this.IsOptional = isOptional

    [<AutoOpen>]
    module ValueExtensions =
        type CellBuilder with
            [<CompiledName("RunQueryAsCell")>]
            member this.Run (q: Microsoft.FSharp.Quotations.Expr<'T>) = 
                try 
                    let value = eval<'T> q |> FsSpreadsheet.DataType.InferCellValue
                    let cell = CellElement(value,None)
                    Missing.ok cell         
                with
                //| err when this.IsOptional -> MissingOptional([this.FormatError err])
                | err  -> MissingRequired([this.FormatError err])               

    [<AutoOpen>]
    module OptionCellExtensions =
        type CellBuilder with
            [<CompiledName("RunQueryAsOptionalCell")>]
            member this.Run (q: Microsoft.FSharp.Quotations.Expr<OptionSource<'T>>) = 
                this.Reset()
                let subExpr = 
                    match q with
                    | Call(exprOpt, methodInfo, [subExpr]) -> Result.Ok subExpr
                    | Call(exprOpt, methodInfo, [ValueWithName(a,b,c);subExpr]) -> Result.Ok subExpr    
                    | x ->                     
                        Result.Error $"could not parse option expression as it was not a call: {x}"
                match subExpr with
                | Result.Ok subExpr -> 
                    try 
                        let value = eval<'T> subExpr |> FsSpreadsheet.DataType.InferCellValue
                        let cell = CellElement(value,None)
                        Missing.ok cell 
                    with 
                    | err -> MissingOptional([this.FormatError err])   
                | Result.Error err -> MissingOptional([this.FormatError err])   
       
    [<AutoOpen>]
    module EnumerableExtensions =
        type CellBuilder with
            [<CompiledName("RunQueryAsCellCollection")>]
            member this.Run (q: Quotations.Expr<QuerySource<'T, IEnumerable>>) = 
                this.RunQueryAsEnumerable q
                |> Seq.map (fun v ->                     
                    let value = v |> FsSpreadsheet.DataType.InferCellValue
                    let cell = CellElement(value,None)
                    Missing.ok cell)
                //|> Seq.toList

    [<AutoOpen>]
    module OptionEnumerableExtensions =
        type CellBuilder with
            [<CompiledName("RunQueryAsOptionalCollection")>]
            member this.Run (q: Quotations.Expr<OptionSource<QuerySource<'T, IEnumerable>>>) = 
                this.Reset()
                let subExpr = 
                    match q with
                    | Call(exprOpt, methodInfo, [subExpr]) -> Result.Ok subExpr
                    | Call(exprOpt, methodInfo, [ValueWithName(a,b,c);subExpr]) -> Result.Ok subExpr    
                    | x ->                     
                        Result.Error $"could not parse option expression as it was not a call: {x}"
                match subExpr with
                | Result.Ok subExpr -> 
                    try 
                        (LeafExpressionConverter.EvaluateQuotation subExpr :?> QuerySource<'T, IEnumerable>).Source
                        |> Seq.map (fun v ->                     
                            let value = v |> FsSpreadsheet.DataType.InferCellValue
                            let cell = CellElement(value,None)
                            Missing.ok cell)                        
                    with 
                    | err -> seq [MissingOptional([this.FormatError err])]   
                | Result.Error err -> seq [MissingOptional([this.FormatError err])]   

    let cells = CellBuilder()