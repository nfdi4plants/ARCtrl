namespace ISADotNet.QueryModel.Linq

open System.Collections.Generic
open System.Collections
open Microsoft.FSharp.Linq.RuntimeHelpers
open Microsoft.FSharp.Quotations.Patterns
open FsSpreadsheet.DSL
open ISADotNet.QueryModel
open Helpers

module Query =

    [<AutoOpen>]
    module ValueExtensions =
        type ISAQueryBuilder with
            [<CompiledName("RunQueryAsValue")>]
            member this.Run (q: Microsoft.FSharp.Quotations.Expr<'T>) = 
                this.RunQueryAsValue q

    [<AutoOpen>]
    module OptionExtensions =
        type ISAQueryBuilder with
                      
            [<CustomOperation("option")>] 
            member this.Option (source: 'T) : OptionSource<'T> =
                this.AddMessage $"as option"
                OptionSource(source)

            [<CompiledName("RunQueryAsOption")>]
            member this.Run (q: Microsoft.FSharp.Quotations.Expr<OptionSource<'T>>) = 
                this.RunQueryAsOption q

    [<AutoOpen>]
    module ExpressionExtensions =
        type ISAQueryBuilder with
            [<CompiledName("RunQueryAsExpression")>]
            member this.Run (q: Microsoft.FSharp.Quotations.Expr<ExpressionSource<'T>>) = 
                this.RunQueryAsExpression q

    [<AutoOpen>]
    module EnumerableExtensions =
        type ISAQueryBuilder with
            [<CompiledName("RunQueryAsEnumerable")>]
            member this.Run (q: Quotations.Expr<QuerySource<'T, IEnumerable>>) = 
                this.RunQueryAsEnumerable q

    let isa = ISAQueryBuilder ()