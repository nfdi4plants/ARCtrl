namespace ISADotNet.QueryModel.Linq

open System.Collections.Generic
open System.Collections
open Microsoft.FSharp.Linq.RuntimeHelpers
open Microsoft.FSharp.Quotations.Patterns
open FsSpreadsheet.DSL
open ISADotNet.QueryModel
open Helpers


type ISAQueryBuilder () =
    
    // ---- Message handling ----

    let mutable messages = []
    let reset () = messages <- []
    let addMessage message = 
        messages <- List.append messages [message]
    let formatError (err : string) = 
        let s = 
            match messages with
            | [] -> ""
            | l -> List.reduce (fun m1 m2 -> m1 + "; " + m2) l
        $"Error when running query: {s}:\n{err}"

    member this.Reset() = reset()
    member this.AddMessage(message) = addMessage message
    member this.FormatError(err : string) = formatError err
    member this.FormatError(err : #System.Exception) = formatError err.Message

    // ---- Computation Expression Helpers ----

    member _.Zero () =
        QuerySource Seq.empty

    member _.Yield value =
        QuerySource (Seq.singleton value)

    member _.YieldFrom (computation: QuerySource<'T, 'Q>) : QuerySource<'T, 'Q> =
        computation

    member _.For (source: QuerySource<'T, 'Q>, body: 'T -> QuerySource<'Result, 'Q2>) : QuerySource<'Result, 'Q> =
        printfn $"{source}"
        QuerySource (Seq.collect (fun x -> (body x).Source) source.Source)

    member _.Quote  (quotation: Quotations.Expr<'T>) =
        quotation

    member _.Source (source: IEnumerable<'T>) : QuerySource<'T, System.Collections.IEnumerable> =
        QuerySource source


    // ---- Mapping operators ----

    /// Map all values in the collection using the given projection
    [<CustomOperation("select",AllowIntoPattern=true)>] 
    member this.Select (source: QuerySource<'T, 'Q>, [<ProjectionParameter>] projection) : QuerySource<'U, 'Q> =
        QuerySource (Seq.map projection source.Source)

    /// Map all values in the collection to their value text
    [<CustomOperation("selectValueText")>] 
    member this.SelectValueText (source: QuerySource<ISAValue, 'Q>) : QuerySource<string, 'Q> =
        addMessage $"get valueText"
        this.Select(source,(fun (v : ISAValue) -> v.ValueText))

    /// Map all values in the collection to their value with unit text
    [<CustomOperation("selectValueWithUnitText")>] 
    member this.SelectValueWithUnitText (source: QuerySource<ISAValue, 'Q>) : QuerySource<string, 'Q> =
        addMessage $"get valueWithUnitText"
        this.Select(source,(fun (v : ISAValue) -> v.ValueWithUnitText))

    /// Map all isa values in the collection to synonymous values in the target ontology
    [<CustomOperation("asValueOfOntology")>] 
    member this.AsValueOfOntology (source: QuerySource<ISAValue, 'Q>, targetOntology) : QuerySource<ISAValue, 'Q> =
        addMessage $"as Value of target ontology"
        this.Select(source,(fun (v : ISAValue) -> v.GetAs(targetOntology,Obo.OboOntology.create [] [])))

    /// Map all isa values in the collection to synonymous values in the target ontology
    [<CustomOperation("asValueOfOntology")>] 
    member this.AsValueOfOntology (source: QuerySource<ISAValue, 'Q>, obo : Obo.OboOntology ,targetOntology) : QuerySource<ISAValue, 'Q> =
        addMessage $"as Value of target ontology"
        this.Select(source,(fun (v : ISAValue) -> v.GetAs(targetOntology,obo)))

    // ---- Filter operators ----

    /// Returns a collection containing only the values for which the predicate returned true.
    [<CustomOperation("where")>] 
    member this.Where (source: QuerySource<'T, 'Q>, predicate) : QuerySource<'T, 'Q> =
        QuerySource (Seq.filter predicate source.Source)
        
    /// Returns a collection containing only the isa values whose category has the given name.
    [<CustomOperation("whereName")>] 
    member this.WhereName (source: QuerySource<ISAValue, 'Q>, name : string) : QuerySource<ISAValue, 'Q> =
        addMessage $"with isa category header {name}"
        this.Where(source,(fun (v : ISAValue) -> v.NameText = name))

    /// Returns a collection containing only the isa values whose categorys are child categories to the given parentCategory.
    [<CustomOperation("whereCategoryIsChildOf")>] 
    member this.WhereCategoryIsChildOf (source: QuerySource<ISAValue, 'Q>, obo : Obo.OboOntology, category : ISADotNet.OntologyAnnotation) : QuerySource<ISAValue, 'Q> =
        addMessage $"with parent isa category {category.NameText}"
        this.Where(source,(fun (v : ISAValue) -> v.HasParentCategory(category,obo)))

    /// Returns a collection containing only the isa values whose categorys are child categories to the given parentCategory.
    [<CustomOperation("whereCategoryIsChildOf")>] 
    member this.WhereCategoryIsChildOf (source: QuerySource<ISAValue, 'Q>, category : ISADotNet.OntologyAnnotation) : QuerySource<ISAValue, 'Q> =
        addMessage $"with parent isa category {category.NameText}"
        this.Where(source,(fun (v : ISAValue) -> v.HasParentCategory(category)))


    // ---- Value selection operators ----

    /// Return the first item of the collection
    [<CustomOperation("head")>] 
    member this.Head (source: QuerySource<'T, 'Q>) =
        addMessage $"head"
        Seq.head source.Source

    /// Return the first item of the collection. If the collection is empty, return a default Value instead
    [<CustomOperation("headOrDefault")>] 
    member this.HeadOrDefault (source: QuerySource<'T, 'Q>, defaultValue : 'T) =
        addMessage $"headOrDefault {defaultValue}"
        Seq.tryHead source.Source
        |> Option.defaultValue defaultValue

    /// Return the last item of the collection
    [<CustomOperation("last")>] 
    member this.Last (source: QuerySource<'T, 'Q>) =
        addMessage $"last"
        Seq.last source.Source

    /// Only return a value if it is the only one in the collection else fail
    [<CustomOperation("exactlyOne")>] 
    member this.ExactlyOne (source: QuerySource<'T, 'Q>) =
        addMessage $"exactly one"
        Seq.exactlyOne source.Source

    /// Only return a value if it is the only one in the collection else fail
    [<CustomOperation("exactlyN")>] 
    member this.ExactlyN (source: QuerySource<'T, 'Q>, n : int) =
        addMessage $"exactly {n}"
        let nSeq = Seq.length source.Source
        if nSeq = n then source
        else failwith $"queried sequence contained {nSeq} elements but was expected to have {n}"

    /// Apply a function to each element in the collection, threading the result as an accumulator to the next step
    [<CustomOperation("reduce")>] 
    member this.Reduce (source: QuerySource<'T, 'Q>, reduction : 'T -> 'T -> 'T) =
        addMessage $"reduce"
        Seq.reduce reduction source.Source

    /// Concatenate all string in the sequence with the given separator
    [<CustomOperation("concat")>] 
    member this.Concat (source: QuerySource<string, 'Q>, separator : char) =
        addMessage $"concat with separator {separator}"
        if source.Source = [] then ""
        else 
            source.Source 
            |> Seq.reduce (fun a b -> a + string separator + b)

    [<CustomOperation("find")>] 
    member this.Find (source: QuerySource<'T, 'Q>, predicate) =
        addMessage $"find"
        this.Where(source,predicate)
        |> this.Head

    
    [<CustomOperation("addHeader")>] 
    member this.AddHeader (source: QuerySource<'T, 'Q>, header : 'T) : QuerySource<'T, 'Q> =
        addMessage $"add header {header}"
        QuerySource (Seq.append (seq [header]) source.Source)


    // ---- Additional operators ----

    /// Return the number of values the collection contains.
    [<CustomOperation("count")>] 
    member this.Count (source: QuerySource<'T, 'Q>) =
        addMessage $"count"
        Seq.length source.Source

    /// Return a collection that contains no duplicate entries.
    [<CustomOperation("distinct")>] 
    member _.Distinct (source: QuerySource<'T, 'Q> when 'T : equality) : QuerySource<'T, 'Q> =
        addMessage $"distinct"
        QuerySource (Seq.distinct source.Source)

    /// Return a collection that contains no duplicate entries according to the keys returned by the projection function.
    [<CustomOperation("distinctBy")>] 
    member _.DistinctBy (source: QuerySource<'T, 'Q> , projection : 'T -> 'U when 'U : equality) : QuerySource<'T, 'Q> =
        addMessage $"distinctBy"
        QuerySource (Seq.distinctBy projection source.Source)

    /// Return true, if any value in the collection satisfies the given predicate.
    [<CustomOperation("exists")>] 
    member _.Exists(source: QuerySource<'T, 'Q>, predicate) =
        addMessage $"exists"
        Seq.exists predicate source.Source


    /// ---- Protocol operators ----

    /// Returns a collection containing only the isa values whose categorys are child categories to the given parentCategory.
    [<CustomOperation("whereSoftwareProtocol")>] 
    member this.WhereSoftwareProtocol (source: QuerySource<QSheet, 'Q>) : QuerySource<QSheet, 'Q> =
        addMessage $"whereSoftwareProtocol"
        let ioTypeIsData (ioType : IOType option) =
            match ioType with
            | Some iot -> iot.isData
            | None -> false
        this.Where(
            source,
            (fun (v : QSheet) -> 
                v.Inputs |> List.exists (snd >> ioTypeIsData)
                && (v.Outputs |> List.exists (snd >> ioTypeIsData))
            )
        )

    /// Returns a collection containing only the isa values whose categorys are child categories to the given parentCategory.
    [<CustomOperation("whereProtocolTypeIsChildOf")>] 
    member this.WhereProtocolTypeIsChildOf (source: QuerySource<QSheet, 'Q>, obo : Obo.OboOntology, category : ISADotNet.OntologyAnnotation) : QuerySource<QSheet, 'Q> =
        addMessage $"with parent isa category {category.NameText}"
        this.Where(
            source,
            (fun (v : QSheet) -> v.Protocols.Head.IsChildProtocolOf(category,obo))
        )

    /// Map all values in the collection to their value with unit text
    [<CustomOperation("selectDescriptionText")>] 
    member this.SelectDescriptionText (source: QuerySource<QSheet, 'Q>) : QuerySource<string, 'Q> =
        addMessage $"select Description Text"
        this.Select(source,(fun (v : QSheet) -> 
            match v.Protocols.Head.Description with
            | Some d -> d
            | None -> failwith "Protocol does not contain description")
        )

    /// Return unevaluated computation expression 
    [<CustomOperation("expression")>] 
    member _.Expression (source: 'T) : ExpressionSource<'T> =
        addMessage $"as expression tree"
        ExpressionSource(source)

    member _.RunQueryAsValue  (q: Quotations.Expr<'T>) : 'T =
        reset()
        eval<'T> q      

    member _.RunQueryAsOption  (q: Quotations.Expr<OptionSource<'T>>) : 'T Option =
        reset()
        let subExpr = 
            match q with
            | Call(exprOpt, methodInfo, [subExpr]) -> Some subExpr
            | Call(exprOpt, methodInfo, [ValueWithName(a,b,c);subExpr]) -> Some subExpr    
            | x -> 
                printfn "could not parse option expression as it was not a call: %O" x
                None
        subExpr
        |> Option.bind (fun subExpr -> 
            try 
                eval<'T> subExpr
                |> Some
            with 
            | _ -> None       
        )
        
    member _.RunQueryAsExpression  (q: Quotations.Expr<ExpressionSource<'T>>) : Quotations.Expr<ExpressionSource<'T>> = //'T Option =
        reset()
        q         

    member _.RunQueryAsEnumerable (q: Quotations.Expr<QuerySource<'T, IEnumerable>>) : IEnumerable<'T> =
        reset()
        (LeafExpressionConverter.EvaluateQuotation q :?> QuerySource<'T, IEnumerable>).Source

    member this.Run q = 
        this.RunQueryAsExpression q
