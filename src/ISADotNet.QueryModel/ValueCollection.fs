namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

open System.Collections.Generic
open System.Collections

type ValueCollection(values : ISAValue list) =
    
    new (values : ISAValue seq) =
        ValueCollection(values |> Seq.toList)

    member this.TryFirst = if values.IsEmpty then None else Some this.First

    member this.First = values.Head

    member this.Last = values.[values.Length - 1]

    member this.Item(i : int)  = values.[i]

    member this.Item(category : string) =
        values 
        |> List.pick (fun v -> if v.Category.NameText = category then Some v else None)

    member this.Item(category : OntologyAnnotation) = 
        values 
        |> List.pick (fun v -> if v.Category = category then Some v else None)

    member this.ItemWithParent(parentCategory : OntologyAnnotation) = 
        values 
        |> List.pick (fun v -> if v.Category.IsChildTermOf(parentCategory) then Some v else None)

    member this.TryItem(i : int)  = if values.Length > i then Some values.[i] else None

    member this.TryItem(category : string) = 
        values
        |> List.tryPick (fun v -> if v.Category.NameText = category then Some v else None)

    member this.TryItem(category : OntologyAnnotation) = 
        values 
        |> List.tryPick (fun v -> if v.Category = category then Some v else None)

    member this.TryItemWithParent(parentCategory : OntologyAnnotation) = 
        values 
        |> List.tryPick (fun v -> if v.Category.IsChildTermOf(parentCategory) then Some v else None)

    member this.Values = values

    member this.Filter(category : string) = values |> List.filter (fun v -> v.Category.NameText = category) |> ValueCollection
    
    member this.Filter(category : OntologyAnnotation) = values |> List.filter (fun v -> v.Category = category) |> ValueCollection

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

    member this.WithName(name : string) = 
        values
        |> List.filter (fun v -> v.Category.NameText = name)
        |> ValueCollection

    member this.WithCategory(category : OntologyAnnotation) = 
        values
        |> List.filter (fun v -> v.Category = category)
        |> ValueCollection

    member this.WithEquivalentCategory(equivalentCategory : OntologyAnnotation, ont : Obo.OboOntology) = 
        values
        |> List.filter (fun v -> v.Category.IsEquivalentTo(equivalentCategory, ont))
        |> ValueCollection

    member this.WithChildCategory(childCategory : OntologyAnnotation) = 
        values
        |> List.filter (fun v -> childCategory.IsChildTermOf(v.Category))
        |> ValueCollection

    member this.WithChildCategory(childCategory : OntologyAnnotation, ont : Obo.OboOntology) = 
        values
        |> List.filter (fun v -> childCategory.IsChildTermOf(v.Category, ont))
        |> ValueCollection

    member this.WithParentCategory(parentCategory : OntologyAnnotation) = 
        values
        |> List.filter (fun v -> v.Category.IsChildTermOf(parentCategory))
        |> ValueCollection

    member this.WithParentCategory(parentCategory : OntologyAnnotation, ont : Obo.OboOntology) = 
        values
        |> List.filter (fun v -> v.Category.IsChildTermOf(parentCategory,ont))
        |> ValueCollection

    member this.Distinct() =
        values
        |> List.distinct
        |> ValueCollection

    member this.ContainsChildOf(parentCategory : OntologyAnnotation) =
        values
        |> List.exists (fun v -> v.Category.IsChildTermOf(parentCategory))

    member this.Contains(category : OntologyAnnotation) =
        values
        |> List.exists (fun v -> v.Category = category)

    member this.Contains(name : string) =
        values
        |> List.exists (fun v -> v.NameText = name)

    interface IEnumerable<ISAValue> with
        member this.GetEnumerator() = (Seq.ofList values).GetEnumerator()

    interface IEnumerable with
        member this.GetEnumerator() = (this :> IEnumerable<ISAValue>).GetEnumerator() :> IEnumerator

    static member (@) (ps1 : ValueCollection,ps2 : ValueCollection) = ps1.Values @ ps2.Values |> ValueCollection


type IOValueCollection(values : KeyValuePair<string*string,ISAValue> list) =

    member this.First = values.Head

    member this.Last = values.[values.Length - 1]

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