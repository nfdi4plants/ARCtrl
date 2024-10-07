namespace ARCtrl.Process

open ARCtrl
open ARCtrl.Helper 


type FactorValue =
    {
        ID : URI option
        Category : Factor option
        Value : Value option
        Unit : OntologyAnnotation option
    }

    static member make id category value unit =
        {
            ID      = id
            Category = category
            Value = value
            Unit = unit         
        }

    static member create(?Id,?Category,?Value,?Unit) : FactorValue =
        FactorValue.make Id Category Value Unit

    static member empty =
        FactorValue.create()

    member this.ValueText =
        this.Value
        |> Option.map (fun oa ->
            match oa with
            | Value.Ontology oa  -> oa.NameText
            | Value.Float f -> string f
            | Value.Int i   -> string i
            | Value.Name s  -> s
        )
        |> Option.defaultValue ""

    member this.ValueWithUnitText =
        let unit = 
            this.Unit |> Option.map (fun oa -> oa.NameText)
        let v = this.ValueText
        match unit with
        | Some u    -> sprintf "%s %s" v u
        | None      -> v

    member this.NameText =
        this.Category
        |> Option.map (fun factor -> factor.NameText)
        |> Option.defaultValue ""

    member this.MapCategory(f : OntologyAnnotation -> OntologyAnnotation) =
        {this with Category = this.Category |> Option.map (fun p -> p.MapCategory f) }

    member this.SetCategory(c : OntologyAnnotation) =
        {this with Category = 
                            match this.Category with
                            | Some p -> Some (p.SetCategory c)
                            | None -> Some (Factor.create(FactorType = c))
        }

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            let category = this.Category |> Option.map (fun f -> f.NameText)
            let unit = this.Unit |> Option.map (fun oa -> oa.NameText)
            let value = 
                this.Value
                |> Option.map (fun v ->
                    let s = (v :> IISAPrintable).PrintCompact()
                    match unit with
                    | Some u -> s + " " + u
                    | None -> s
                )
            match category,value with
            | Some category, Some value -> category + ":" + value
            | Some category, None -> category + ":" + "No Value"
            | None, Some value -> value
            | None, None -> ""

    /// Returns the name of the factor value as string
    static member getNameAsString (fv : FactorValue) =
        fv.NameText

    /// Returns true if the given name matches the name of the factor value
    static member nameEqualsString (name : string) (fv : FactorValue) =
        fv.NameText = name

    interface IPropertyValue with

        member this.AlternateName() = None
        member this.MeasurementMethod() = None 
        member this.Description() = None
        member this.GetCategory() =
            this.Category
            |> Option.bind (fun f -> f.FactorType)

        member this.GetValue() =
            this.Value

        member this.GetUnit() =
            this.Unit

        member this.GetAdditionalType() =
            "FactorValue"

    // TODO: @hlweil why are there unused params?
    static member createAsPV (alternateName : string option) (measurementMethod : string option) (description : string option) (category : OntologyAnnotation option) (value : Value option) (unit : OntologyAnnotation option) =
        let category = category |> Option.map (fun c -> Factor.create(FactorType = c))
        FactorValue.create(?Category = category, ?Value = value, ?Unit = unit)

    ///// Returns the value of the factor value as string if it exists (with unit)
    //static member tryGetValueAsString (fv : FactorValue) =
    //    let unit = fv.Unit |> Option.map (OntologyAnnotation.getNameText)
    //    fv.Value
    //    |> Option.map (fun v ->
    //        let s = v |> Value.toString
    //        match unit with
    //        | Some u -> s + " " + u
    //        | None -> s
    //    )

    ///// Returns the value of the factor value as string (with unit)
    //static member getValueAsString (fv : FactorValue) =
    //    tryGetValueAsString fv
    //    |> Option.defaultValue ""