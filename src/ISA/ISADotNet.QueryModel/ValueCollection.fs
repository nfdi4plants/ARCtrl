namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

open System.Collections.Generic
open System.Collections

/// Contains queryable ISAValues (Parameters, Factors, Characteristics)
type ValueCollection(values : ISAValue list) =
    
    new (values : ISAValue seq) =
        ValueCollection(values |> Seq.toList)

    /// Returns the nth Item in the collection
    member this.Item(i : int)  = values.[i]

    /// Returns an Item in the collection with the given header name
    member this.Item(category : string) =
        values 
        |> List.pick (fun v -> if v.Category.NameText = category then Some v else None)

    /// Returns an Item in the collection with the given header category
    member this.Item(category : OntologyAnnotation) = 
        values 
        |> List.pick (fun v -> if v.Category = category then Some v else None)

    /// Returns an Item in the collection whichs header category is a child of the given parent category
    member this.ItemWithParent(parentCategory : OntologyAnnotation) = 
        values 
        |> List.pick (fun v -> if v.Category.IsChildTermOf(parentCategory) then Some v else None)

    /// Returns the nth Item in the collection if it exists, else returns None
    member this.TryItem(i : int)  = if values.Length > i then Some values.[i] else None

    /// Returns an Item in the collection with the given header name, else returns None
    member this.TryItem(category : string) = 
        values
        |> List.tryPick (fun v -> if v.Category.NameText = category then Some v else None)

    /// Returns an Item in the collection with the given header category, else returns None
    member this.TryItem(category : OntologyAnnotation) = 
        values 
        |> List.tryPick (fun v -> if v.Category = category then Some v else None)

    /// Returns an Item in the collection whichs header category is a child of the given parent category, else returns None
    member this.TryItemWithParent(parentCategory : OntologyAnnotation) = 
        values 
        |> List.tryPick (fun v -> if v.Category.IsChildTermOf(parentCategory) then Some v else None)

    /// Get the values as list
    member this.Values = values

    /// Return a new ValueCollection with only the characteristic values
    member this.Characteristics(?Name) = 
        values
        |> List.filter (fun v -> 
            match Name with 
            | Some name -> 
                v.IsCharacteristicValue && v.NameText = name
            | None -> 
                v.IsCharacteristicValue
        )
        |> ValueCollection

    /// Return a new ValueCollection with only the parameter values
    member this.Parameters(?Name) = 
        values
        |> List.filter (fun v -> 
            match Name with 
            | Some name -> 
                v.IsParameterValue && v.NameText = name
            | None -> 
                v.IsParameterValue
        )
        |> ValueCollection

    /// Return a new ValueCollection with only the factor values
    member this.Factors(?Name) = 
        values
        |> List.filter (fun v -> 
            match Name with 
            | Some name -> 
                v.IsFactorValue && v.NameText = name
            | None -> 
                v.IsFactorValue
        )
        |> ValueCollection

    /// Return a new ValueCollection with only the factor values
    member this.Components(?Name) = 
        values
        |> List.filter (fun v -> 
            match Name with 
            | Some name -> 
                v.IsComponent && v.NameText = name
            | None -> 
                v.IsComponent
        )
        |> ValueCollection

    /// Return a new ValueCollection with only those values, for which the predicate applied on the header return true
    member this.Filter(predicate : OntologyAnnotation -> bool) = values |> List.filter (fun v -> predicate v.Category) |> ValueCollection

    /// Return a new ValueCollection with only those values, whichs header equals the given string
    member this.WithName(name : string) = 
        this.Filter (fun v -> v.NameText = name)

    /// Return a new ValueCollection with only those values, whichs header equals the given category
    member this.WithCategory(category : OntologyAnnotation) = 
        this.Filter((=) category)

    /// Return a new ValueCollection with only those values, whichs header equals the given category or an equivalent category
    ///
    /// Equivalency is deduced from XRef relationships in the given Ontology
    member this.WithEquivalentCategory(equivalentCategory : OntologyAnnotation, ont : Obo.OboOntology) = 
        this.Filter (fun v -> v.IsEquivalentTo(equivalentCategory, ont))

    /// Return a new ValueCollection with only those values, whichs header equals the given category or its child categories
    ///
    /// Equivalency is deduced from isA relationships in the SwateAPI
    member this.WithChildCategory(childCategory : OntologyAnnotation) = 
        this.Filter (fun v -> childCategory.IsChildTermOf(v))

    /// Return a new ValueCollection with only those values, whichs header equals the given category or its child categories
    ///
    /// Equivalency is deduced from isA relationships in the given Ontology
    member this.WithChildCategory(childCategory : OntologyAnnotation, ont : Obo.OboOntology) = 
        this.Filter (fun v -> childCategory.IsChildTermOf(v, ont))

    /// Return a new ValueCollection with only those values, whichs header equals the given category or its parent categories
    ///
    /// Equivalency is deduced from isA relationships in the SwateAPI
    member this.WithParentCategory(parentCategory : OntologyAnnotation) = 
        this.Filter (fun v -> v.IsChildTermOf(parentCategory))

    /// Return a new ValueCollection with only those values, whichs header equals the given category or its parent categories
    ///
    /// Equivalency is deduced from isA relationships in the given Ontology
    member this.WithParentCategory(parentCategory : OntologyAnnotation, ont : Obo.OboOntology) = 
        this.Filter (fun v -> v.IsChildTermOf(parentCategory,ont))

    /// Returns a new ValueCollection that contains no duplicate entries. 
    member this.Distinct() =
        values
        |> List.distinct
        |> ValueCollection

    /// Returns a new ValueCollection that contains no two entries with the same header Category
    member this.DistinctHeaderCategories() =
        values
        |> List.distinctBy (fun v -> v.Category)
        |> ValueCollection

    /// Returns true, if the ValueCollection contains a values, whichs header equals the given category or its child categories
    ///
    /// Equivalency is deduced from isA relationships in the SwateAPI
    member this.ContainsChildOf(parentCategory : OntologyAnnotation) =
        values
        |> List.exists (fun v -> v.Category.IsChildTermOf(parentCategory))

    /// Returns true, if the ValueCollection contains a values, whichs header equals the given category
    member this.Contains(category : OntologyAnnotation) =
        values
        |> List.exists (fun v -> v.Category = category)

    /// Returns true, if the ValueCollection contains a values, whichs headername equals the given category
    member this.Contains(name : string) =
        values
        |> List.exists (fun v -> v.NameText = name)

    interface IEnumerable<ISAValue> with
        member this.GetEnumerator() = (Seq.ofList values).GetEnumerator()

    interface IEnumerable with
        member this.GetEnumerator() = (this :> IEnumerable<ISAValue>).GetEnumerator() :> IEnumerator

    static member (@) (ps1 : ValueCollection,ps2 : ValueCollection) = ps1.Values @ ps2.Values |> ValueCollection

[<AutoOpen>]
module ValueCollectionExtensions =

    type ValueCollection with

        /// Return the number of values in the collection
        member this.IsEmpty = this.Values.IsEmpty

        /// Return the number of values in the collection
        member this.Length = this.Values.Length

        /// Return first ISAValue in collection
        member this.First = this.Values.Head

        /// Return first ISAValue in collection if it exists, else returns None
        member this.TryFirst = if this.IsEmpty then None else Some this.First

        /// Return first ISAValue in collection
        member this.Last = this.Values.[this.Length - 1]

/// Contains queryable ISAValues (Parameters, Factors, Characteristics)
type IOValueCollection(values : KeyValuePair<string*string,ISAValue> list) =

    /// Returns the nth Item in the collection
    member this.Item(i : int)  = values.[i]

    member this.Item(category : string) = values |> List.pick (fun kv -> if kv.Value.Category.NameText = category then Some kv.Key else None)

    member this.Item(ioKey : string*string) = values |> List.pick (fun kv -> if ioKey = kv.Key then Some kv.Value else None)

    member this.Item(category : OntologyAnnotation) = values |> List.pick (fun kv -> if kv.Value.Category = category then Some kv.Key else None)

    member this.WithInput(inp : string) = 
        values |> List.choose (fun kv -> if (fst kv.Key) = inp then Some kv.Value else None)
        |> ValueCollection

    member this.WithOutput(inp : string) = 
        values |> List.choose (fun kv -> if (snd kv.Key) = inp then Some kv.Value else None)
        |> ValueCollection

    member this.Values(?Name) = 
        values 
        |> List.choose (fun kv -> 
            match Name with
            | Some name -> 
                if kv.Value.NameText = name then Some kv.Value
                else None
            | None -> Some kv.Value
        )
        |> ValueCollection

    member this.Characteristics(?Name) = 
        values
        |> List.filter (fun kv -> 
            match Name with 
            | Some name -> 
                kv.Value.IsCharacteristicValue && kv.Value.NameText = name
            | None -> 
                kv.Value.IsCharacteristicValue
        )
        |> IOValueCollection

    member this.Parameters(?Name) = 
        values
        |> List.filter (fun kv -> 
            match Name with 
            | Some name -> 
                kv.Value.IsParameterValue && kv.Value.NameText = name
            | None -> 
                kv.Value.IsParameterValue
        )
        |> IOValueCollection

    member this.Factors(?Name) = 
        values
        |> List.filter (fun kv -> 
            match Name with 
            | Some name -> 
                kv.Value.IsFactorValue && kv.Value.NameText = name
            | None -> 
                kv.Value.IsFactorValue
        )
        |> IOValueCollection

    member this.Components(?Name) = 
        values
        |> List.filter (fun kv -> 
            match Name with 
            | Some name -> 
                kv.Value.IsComponent && kv.Value.NameText = name
            | None -> 
                kv.Value.IsComponent
        )
        |> IOValueCollection

    member this.WithCategory(category : OntologyAnnotation) = 
        values
        |> List.filter (fun kv -> kv.Value.Category = category)
        |> IOValueCollection

    member this.WithName(name : string) = 
        values
        |> List.filter (fun kv -> kv.Value.Category.NameText = name)
        |> IOValueCollection

    member this.GroupBySource =
        values
        |> List.groupBy (fun kv -> fst kv.Key)
        |> List.map (fun (source,vals) -> source, vals |> List.map (fun kv -> snd kv.Key,kv.Value))

    member this.GroupBySink =
        values
        |> List.groupBy (fun kv -> snd kv.Key)
               |> List.map (fun (sink,vals) -> sink, vals |> List.map (fun kv -> fst kv.Key,kv.Value))
    
    interface IEnumerable<KeyValuePair<string*string,ISAValue>> with
        member this.GetEnumerator() = (Seq.ofList values).GetEnumerator()

    interface IEnumerable with
        member this.GetEnumerator() = (this :> IEnumerable<KeyValuePair<string*string,ISAValue>>).GetEnumerator() :> IEnumerator

[<AutoOpen>]
module IOValueCollectionExtensions =

    type IOValueCollection with

        /// Return the number of values in the collection
        member this.Length = this.Values().Length

        /// Return first ISAValue in collection
        member this.First = this.Values().First

        /// Return first ISAValue in collection
        member this.Last = this.Values().Last