namespace ISADotNet.QueryModel.Linq

open Microsoft.FSharp.Linq.RuntimeHelpers
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns



module Helpers =

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

    let eval<'T> q = LeafExpressionConverter.EvaluateQuotation q :?> 'T

    //// Call = calling of a method
    //let getChildExpressionsOfCall (x : Expr) = 
    //    match x with
    //    | Call(_,_,z) -> z

    //// PropertyGet = 
    //let getChildExpressionsOfPropertyGet (x : Expr) = 
    //    match x with
    //    | PropertyGet(_,_,z) -> z

    //// PropertyGet = 
    //let getObjectOfPropertyGet (x : Expr) = 
    //    match x with
    //    | PropertyGet(_,p,_) ->p

    //// Coercion = Type casting
    //let getChildExpressionsOfCoercion (x : Expr) = 
    //    match x with
    //    | Coerce(sourceExpression,targetType) -> sourceExpression

    //// Coercion = Type casting
    //let getTypeOfCoercion (x : Expr) = 
    //    match x with
    //    | Coerce(sourceExpression,targetType) -> targetType