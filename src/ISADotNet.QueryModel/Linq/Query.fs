namespace ISADotNet.Querymodel.Linq

open System.Collections.Generic
open System.Collections
open Microsoft.FSharp.Linq.RuntimeHelpers
open Microsoft.FSharp.Quotations.Patterns
open FsSpreadsheet.DSL
open ISADotNet.QueryModel

[<NoComparison; NoEquality; Sealed>]
type CellSource<'T>(s : 'T,isOptional) =
    member this.IsOptional = isOptional
    member this.Source = s

[<NoComparison; NoEquality; Sealed>]
type ColumnSource<'T, 'Q>(s : seq<'T>,isOptional) =
    member this.IsOptional = isOptional
    member this.Source = s

[<NoComparison; NoEquality; Sealed>]
type OptionSource<'T>(s : 'T) =

    member this.Source = s

[<NoComparison; NoEquality; Sealed>]
type ExpressionSource<'T>(s : 'T) =

    member this.Source = s

[<NoComparison; NoEquality; Sealed>]
type QuerySource<'T, 'Q>(s : seq<'T>) =

    /// <summary>
    /// A property used to support the F# query syntax.  
    /// </summary>
    member this.Source = s


module Expression = 

    let eval<'T> q = LeafExpressionConverter.EvaluateQuotation q :?> 'T


type ISAQueryBuilder () =
    
    let mutable messages = []
    let reset () = messages <- []
    let addMessage message = 
        messages <- List.append messages [message]
    let formatError (err : #System.Exception) = 
        let s = 
            match messages with
            | [] -> ""
            | l -> List.reduce (fun m1 m2 -> m1 + "; " + m2) l
        $"Error when running query: {s}:\n{err.Message}"


    member _.For (source: QuerySource<'T, 'Q>, body: 'T -> QuerySource<'Result, 'Q2>) : QuerySource<'Result, 'Q> =
        printfn $"{source}"
        QuerySource (Seq.collect (fun x -> (body x).Source) source.Source)

    member _.Zero () =
        QuerySource Seq.empty

    member _.Yield value =
        QuerySource (Seq.singleton value)

    member _.YieldFrom (computation: QuerySource<'T, 'Q>) : QuerySource<'T, 'Q> =
        computation

    // Indicates to the F# compiler that an implicit quotation is added to use of 'query'
    member _.Quote  (quotation: Quotations.Expr<'T>) =
        quotation

    member _.Source (source: IEnumerable<'T>) : QuerySource<'T, System.Collections.IEnumerable> =
        QuerySource source

    [<CustomOperation("select",AllowIntoPattern=true)>] 
    member _.Select (source: QuerySource<'T, 'Q>, [<ProjectionParameter>] projection) : QuerySource<'U, 'Q> =
        QuerySource (Seq.map projection source.Source)

    member _.Where (source: QuerySource<'T, 'Q>, predicate) : QuerySource<'T, 'Q> =
        QuerySource (Seq.filter predicate source.Source)
        
    [<CustomOperation("hasName")>] 
    member _.HasName (source: QuerySource<ISAValue, 'Q>, name : string) : QuerySource<ISAValue, 'Q> =
        addMessage $"with isa category header {name}"
        QuerySource (Seq.filter (fun (v : ISAValue) -> v.NameText = name) source.Source)

    [<CustomOperation("valueText")>] 
    member _.ValueText (source: QuerySource<ISAValue, 'Q>) : QuerySource<string, 'Q> =
        addMessage $"get valueText"
        QuerySource (Seq.map (fun (v : ISAValue) -> v.ValueText) source.Source)

    //[<CustomOperation("valueText")>] 
    //member _.ValueText (source: ISAValue) : string =
    //    source.ValueText

    [<CustomOperation("addHeader")>] 
    member _.AddHeader (source: QuerySource<'T, 'Q>, header : 'T) : QuerySource<'T, 'Q> =
        addMessage $"add header {header}"
        QuerySource (Seq.append (seq [header]) source.Source)

    [<CustomOperation("requiredCell")>] 
    member _.RequiredCell (source: 'T) : CellSource<'T> =
        addMessage $"as required cell"
        CellSource (source,false)

    [<CustomOperation("optionalCell")>] 
    member _.OptionalCell (source: 'T) : CellSource<'T> =
        addMessage $"as optional cell"
        CellSource (source,true)

    [<CustomOperation("requiredColumn")>] 
    member _.RequiredColumn (source: QuerySource<'T, 'Q>) : ColumnSource<'T,'Q> =
        addMessage $"as required column"
        ColumnSource(source.Source,false)

    [<CustomOperation("optionalColumn")>] 
    member _.OptionalColumn (source: QuerySource<'T, 'Q>) : ColumnSource<'T,'Q> =
        addMessage $"as optional column"
        ColumnSource(source.Source,true)

    [<CustomOperation("option")>] 
    member _.Option (source: 'T) : OptionSource<'T> =
        addMessage $"as option"
        OptionSource(source)

    [<CustomOperation("expression")>] 
    member _.Expression (source: 'T) : ExpressionSource<'T> =
        addMessage $"as expression tree"
        ExpressionSource(source)

    //member _.Last (source: QuerySource<'T, 'Q>) =
    //    Enumerable.Last source.Source

    //member _.ExactlyOne (source: QuerySource<'T, 'Q>) =
    //    Enumerable.Single source.Source

    //member _.Count (source: QuerySource<'T, 'Q>) =
    //    Enumerable.Count source.Source

    //member _.Distinct (source: QuerySource<'T, 'Q> when 'T : equality) : QuerySource<'T, 'Q> =
    //    QuerySource (Enumerable.Distinct source.Source)

    //member _.Exists(source: QuerySource<'T, 'Q>, predicate) =
    //    Enumerable.Any (source.Source, Func<_, _>(predicate))

    //member _.All (source: QuerySource<'T, 'Q>, predicate) =
    //    Enumerable.All (source.Source, Func<_, _>(predicate))

    [<CustomOperation("head")>] 
    member _.Head (source: QuerySource<'T, 'Q>) =
        addMessage $"head"
        Seq.head source.Source

    //member _.Find (source: QuerySource<'T, 'Q>, predicate) =
    //    Enumerable.First (source.Source, Func<_, _>(predicate))

    //member _.HeadOrDefault (source: QuerySource<'T, 'Q>) =
    //    Enumerable.FirstOrDefault source.Source

    //member _.RunQueryAsValue  (q: Quotations.Expr<QuerySource<'T, IEnumerable>>) : 'T Option =
    //    try 
    //        eval<'T> q
    //        |> Some
    //    with 
    //    | _ -> None

    member _.RunQueryAsValue  (q: Quotations.Expr<'T>) : 'T =
        reset()
        Expression.eval<'T> q      

    member _.RunQueryAsOption  (q: Quotations.Expr<OptionSource<'T>>) : 'T Option =
        reset()
        match q with
        | Call(exprOpt, methodInfo, [subExpr]) ->
            try 
                Expression.eval<'T> subExpr
                |> Some
            with 
            | _ -> None

    member _.RunQueryAsCell  (q: Quotations.Expr<CellSource<'T>>)  : Missing<CellElement> = 
        reset()
        match q with
        | Call(exprOpt, methodInfo, [subExpr]) ->
        try 
            let value = Expression.eval<'T> subExpr |> FsSpreadsheet.DataType.InferCellValue
            let cell = CellElement(value,None)
            Missing.ok cell         
        with
        | err when methodInfo.Name = "OptionalCell" -> MissingOptional([formatError err])
        | err when methodInfo.Name = "RequiredCell" -> MissingRequired([formatError err])

    member _.RunQueryAsColumn  (q: Quotations.Expr<ColumnSource<'T,IEnumerable>>)  : Missing<ColumnElement list> = 
        reset()
        match q with
        | Call(exprOpt, methodInfo, [subExpr]) ->
        try 
            let values = 
                (LeafExpressionConverter.EvaluateQuotation subExpr :?> QuerySource<'T, IEnumerable>).Source
                |> Seq.map (FsSpreadsheet.DataType.InferCellValue >> ColumnElement.UnindexedCell)
                |> Seq.toList
            Missing.ok values        
        with
        | err when methodInfo.Name = "OptionalColumn" -> MissingOptional([formatError err])
        | err when methodInfo.Name = "RequiredColumn" -> MissingRequired([formatError err])

    member _.RunQueryAsExpression  (q: Quotations.Expr<ExpressionSource<'T>>) : Quotations.Expr<ExpressionSource<'T>> = //'T Option =
        reset()
        q         

    member _.RunQueryAsEnumerable (q: Quotations.Expr<QuerySource<'T, IEnumerable>>) : IEnumerable<'T> =
        reset()
        (LeafExpressionConverter.EvaluateQuotation q :?> QuerySource<'T, IEnumerable>).Source

    member this.Run q = 
        this.RunQueryAsEnumerable q

module Extensions =

    [<AutoOpen>]
    module ValueExtensions =
        type ISAQueryBuilder with
            [<CompiledName("RunQueryAsValue")>]
            member this.Run (q: Microsoft.FSharp.Quotations.Expr<'T>) = 
                this.RunQueryAsValue q

    [<AutoOpen>]
    module OptionExtensions =
        type ISAQueryBuilder with
            [<CompiledName("RunQueryAsOption")>]
            member this.Run (q: Microsoft.FSharp.Quotations.Expr<OptionSource<'T>>) = 
                this.RunQueryAsOption q

    [<AutoOpen>]
    module CellExtensions =
        type ISAQueryBuilder with
            [<CompiledName("RunQueryAsCell")>]
            member this.Run (q: Microsoft.FSharp.Quotations.Expr<CellSource<'T>>) = 
                this.RunQueryAsCell q

    [<AutoOpen>]
    module ColumnExtensions =
        type ISAQueryBuilder with
            [<CompiledName("RunQueryAsColumn")>]
            member this.Run (q: Microsoft.FSharp.Quotations.Expr<ColumnSource<'T, IEnumerable>>) = 
                this.RunQueryAsColumn q

    [<AutoOpen>]
    module ExpressionExtensions =
        type ISAQueryBuilder with
            [<CompiledName("RunQueryAsExpression")>]
            member this.Run (q: Microsoft.FSharp.Quotations.Expr<ExpressionSource<'T>>) = 
                this.RunQueryAsExpression q
