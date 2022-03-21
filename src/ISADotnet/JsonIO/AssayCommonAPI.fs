namespace ISADotNet.Json

open ISADotNet
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

open System.Collections.Generic
open System.Collections



module AssayCommonAPI = 

    [<AnyOf>]
    type ISAValue =
        | [<SerializationOrder(0)>] Parameter of ProcessParameterValue
        | [<SerializationOrder(1)>] Characteristic of MaterialAttributeValue
        | [<SerializationOrder(2)>] Factor of FactorValue

        /// Returns the ontology of the category of the Value as string
        member this.Category =
            match this with
            | Parameter p       -> p.Category.Value.ParameterName.Value
            | Characteristic c  -> c.Category.Value.CharacteristicType.Value
            | Factor f          -> f.Category.Value.FactorType.Value

        ///// Returns the name of the Value as string
        //member this.GetTableHeader =
        //    match this with
        //    | Parameter p       -> p.GetName
        //    | Characteristic c  -> c.GetName
        //    | Factor f          -> f.GetName

        /// Returns the name of the Value as string
        member this.Name = this.Category.GetName

        /// Returns the name of the Value with the number as string (e.g. "temperature #2")
        member this.NameWithNumber = this.Category.GetNameWithNumber
    
        /// Returns the name of the Value with the number as string (e.g. "temperature #2")
        member this.Number = this.Category.Number

        //member this.Value =
        //    match this with
        //    | Parameter p       -> p.GetValue
        //    | Characteristic c  -> c.GetValue
        //    | Factor f          -> f.GetValue

        member this.ValueString =
            match this with
            | Parameter p       -> p.GetValue
            | Characteristic c  -> c.GetValue
            | Factor f          -> f.GetValue

        member this.ValueWithUnit =
            match this with
            | Parameter p       -> p.GetValueWithUnit
            | Characteristic c  -> c.GetValueWithUnit
            | Factor f          -> f.GetValueWithUnit

    let combineValues (characteristics : MaterialAttributeValue list) (parameters : ProcessParameterValue list) (factors : FactorValue list) : ISAValue list =
        let l = 
            (characteristics |> List.map Characteristic)
            @ (parameters |> List.map Parameter)
            @ (factors |> List.map Factor)
        l

    type IPrintable =
       abstract member Print : unit -> unit

    type Row = 
        {
            [<JsonPropertyName(@"input")>]
            Input : string
            [<JsonPropertyName(@"output")>]
            Output : string
            [<JsonPropertyName(@"inputType")>]
            InputType : string option
            [<JsonPropertyName(@"outputType")>]
            OutputType : string option
            [<JsonPropertyName(@"values")>]
            Values : ISAValue list
        }

        static member create (?Input,?Output,?InputType,?OutputType,?Values) : Row =

            {
                Input = Input |> Option.defaultValue ""
                Output = Output |> Option.defaultValue ""
                InputType = InputType
                OutputType = OutputType
                Values = Values |> Option.defaultValue []
            }

        static member create(?Input,?Output,?InputType,?OutputType,?CharValues,?ParamValues,?FactorValues) : Row =
            {
                Input = Input |> Option.defaultValue ""
                Output = Output |> Option.defaultValue ""
                InputType = InputType
                OutputType = OutputType
                Values = combineValues (CharValues |> Option.defaultValue []) (ParamValues |> Option.defaultValue []) (FactorValues |> Option.defaultValue [])
            }

        static member fromProcess (proc : Process) : Row list =
            let parameterValues = proc.ParameterValues |> Option.defaultValue []
            (proc.Inputs.Value,proc.Outputs.Value)
            ||> List.map2 (fun inp out ->
                let characteristics = API.ProcessInput.tryGetCharacteristics inp |> Option.defaultValue []
                let factors = API.ProcessOutput.tryGetFactorValues out |> Option.defaultValue []

                let inputName = inp.GetName
                let outputName = out.GetName

                Row.create(inputName,outputName,ParamValues = parameterValues, CharValues = characteristics,FactorValues = factors)
            )

        member this.Item (i : int) =
            this.Values 
            |> Seq.item i

        member this.Item (s : string) =
            this.Values 
            |> List.find (fun v -> 
                s = v.NameWithNumber || v.Name = s
            )

        member this.Item (oa : OntologyAnnotation) =
            this.Values 
            |> List.pick (fun v -> 
                if v.Category = oa then Some v
                else None
            )

        interface IEnumerable<ISAValue> with
            member this.GetEnumerator() : System.Collections.Generic.IEnumerator<ISAValue> = (seq this.Values).GetEnumerator()

        interface IEnumerable with
            member this.GetEnumerator() = (this :> IEnumerable<ISAValue>).GetEnumerator() :> IEnumerator

    type RowWiseSheet = 
        {
            [<JsonPropertyName(@"sheetName")>]
            SheetName : string
            [<JsonPropertyName(@"rows")>]
            Rows : Row list
        }

        static member create sheetName rows : RowWiseSheet =
            {
                SheetName = sheetName
                Rows = rows
            }

        static member fromProcesses name (processes : Process list) =        
            processes
            |> List.collect (Row.fromProcess)
            |> RowWiseSheet.create name 

        member this.Item (input : string) =
            this.Rows 
            |> List.pick (fun r -> 
                if r.Input = input then Some r
                else None
            )

        interface IEnumerable<Row> with
            member this.GetEnumerator() = (seq this.Rows).GetEnumerator()

        interface IEnumerable with
            member this.GetEnumerator() = (this :> IEnumerable<Row>).GetEnumerator() :> IEnumerator

    type RowWiseAssay =
        {
            //[<JsonPropertyName(@"assayName")>]
            //AssayName : string
            [<JsonPropertyName(@"sheets")>]
            Sheets : RowWiseSheet list
        }

        interface IEnumerable<RowWiseSheet> with
            member this.GetEnumerator() = (Seq.ofList this.Sheets).GetEnumerator()

        interface IEnumerable with
            member this.GetEnumerator() = (this :> IEnumerable<RowWiseSheet>).GetEnumerator() :> IEnumerator

        static member create (*assayName*) sheets : RowWiseAssay =
            {
                //AssayName = assayName
                Sheets = sheets
            }

        static member fromAssay (assay : Assay) = 
            assay.ProcessSequence |> Option.defaultValue []
            |> List.groupBy (fun x -> 
                if x.ExecutesProtocol.IsSome && x.ExecutesProtocol.Value.Name.IsSome then
                    x.ExecutesProtocol.Value.Name.Value 
                else
                    // Data Stewards use '_' as seperator to distinguish between protocol template types.
                    // Exmp. 1SPL01_plants, in these cases we need to find the last '_' char and remove from that index.
                    let lastUnderScoreIndex = x.Name.Value.LastIndexOf '_'
                    x.Name.Value.Remove lastUnderScoreIndex
            )
            |> List.map (fun (name,processes) -> RowWiseSheet.fromProcesses name processes)
            |> RowWiseAssay.create (*(assay.FileName |> Option.defaultValue "")*)
    
        member this.Item (sheetName) =
            this.Sheets 
            |> List.pick (fun sheet -> 
                if sheet.SheetName = sheetName then 
                    Some sheet
                else None
            )

        

        static member toString (rwa : RowWiseAssay) =  JsonSerializer.Serialize<RowWiseAssay>(rwa,JsonExtensions.options)

        static member toFile (path : string) (rwa:RowWiseAssay) = 
            File.WriteAllText(path,RowWiseAssay.toString rwa)

        static member fromString (s:string) = 
            JsonSerializer.Deserialize<RowWiseAssay>(s,JsonExtensions.options)

        static member fromFile (path : string) = 
            File.ReadAllText path 
            |> RowWiseAssay.fromString

    type ParameterColumn = 
        {
            [<JsonPropertyName(@"category")>]
            Category    : ProtocolParameter option
            [<JsonPropertyName(@"values")>]
            Values       : Value option list
            [<JsonPropertyName(@"unit")>]
            Unit        : OntologyAnnotation option
        }

        static member create category values unit : ParameterColumn=
            {
                Category = category
                Values = values
                Unit = unit
            }

        static member fromParams (chars : ProcessParameterValue list) =
            chars
            |> List.reduce (fun char1 char2 -> 
                if char1.Category <> char2.Category then failwithf "Characteristic %A does not match characteristic %A" char1.Category char2.Category
                if char1.Unit <> char2.Unit then failwithf "Unit %A does not match Unit %A" char1.Unit char2.Unit
                char1      
            ) |> ignore
            ParameterColumn.create chars.[0].Category (chars |> List.map (fun char -> char.Value)) chars.[0].Unit 

    type FactorColumn = 
        {
            [<JsonPropertyName(@"category")>]
            Category    : Factor option
            [<JsonPropertyName(@"values")>]
            Values       : Value option list 
            [<JsonPropertyName(@"unit")>]
            Unit        : OntologyAnnotation option
        }

        static member create category values unit : FactorColumn =
            {
                Category = category
                Values = values
                Unit = unit
            }

        static member fromFactors (chars : FactorValue list) =
            chars
            |> List.reduce (fun char1 char2 -> 
                if char1.Category <> char2.Category then failwithf "Characteristic %A does not match characteristic %A" char1.Category char2.Category
                if char1.Unit <> char2.Unit then failwithf "Unit %A does not match Unit %A" char1.Unit char2.Unit
                char1      
            ) |> ignore
            FactorColumn.create chars.[0].Category (chars |> List.map (fun char -> char.Value)) chars.[0].Unit 

    type CharacteristicColumn = 
        {
            [<JsonPropertyName(@"category")>]
            Category    : MaterialAttribute option
            [<JsonPropertyName(@"values")>]
            Values       : Value option list
            [<JsonPropertyName(@"unit")>]
            Unit        : OntologyAnnotation option
        }

        static member create category values unit : CharacteristicColumn =
            {
                Category = category
                Values = values
                Unit = unit
            }

        static member fromCharacteristics (chars : MaterialAttributeValue list) =
            chars
            |> List.reduce (fun char1 char2 -> 
                if char1.Category <> char2.Category then failwithf "Characteristic %A does not match characteristic %A" char1.Category char2.Category
                if char1.Unit <> char2.Unit then failwithf "Unit %A does not match Unit %A" char1.Unit char2.Unit
                char1      
            ) |> ignore
            CharacteristicColumn.create chars.[0].Category (chars |> List.map (fun char -> char.Value)) chars.[0].Unit 

    //[<AnyOf>]
    //type Column = 
    //    | [<SerializationOrder(0)>] Parameter of ParameterColumn
    //    | [<SerializationOrder(1)>] Characteristic of CharacteristicColumn
    //    | [<SerializationOrder(2)>] Factor of FactorColumn

    type ColumnWiseSheet = 
        {
            [<JsonPropertyName(@"sheetName")>]
            SheetName : string
            [<JsonPropertyName(@"parameterColumns")>]
            ParameterColumns : ParameterColumn list
            [<JsonPropertyName(@"characteristicColumns")>]
            CharacteristicColumns : CharacteristicColumn list
            [<JsonPropertyName(@"factorColumns")>]
            FactorColumns : FactorColumn list
        }

        static member create sheetName paramColumns charColumns factorColumns = 
            {
                SheetName = sheetName
                ParameterColumns = paramColumns
                CharacteristicColumns = charColumns
                FactorColumns = factorColumns
            }

        //static member fromRowWiseSheet (rws : RowWiseSheet) = 
        //    let parameters = 
        //        rws.Rows
        //        |> List.map (fun row -> row.ParameterValues)
        //        |> List.transpose 
        //        |> List.map ParameterColumn.fromParams
        //    let characteristics = 
        //        rws.Rows
        //        |> List.map (fun row -> row.CharacteristicValues)
        //        |> List.transpose 
        //        |> List.map CharacteristicColumn.fromCharacteristics
        //    let factors = 
        //        rws.Rows
        //        |> List.map (fun row -> row.FactorValues)
        //        |> List.transpose 
        //        |> List.map FactorColumn.fromFactors
        //    ColumnWiseSheet.create rws.SheetName parameters characteristics factors

    type ColumnWiseAssay =
        {
            //[<JsonPropertyName(@"assayName")>]
            //AssayName : string
            [<JsonPropertyName(@"sheets")>]
            Sheets : ColumnWiseSheet list
        }

        static member create sheets = 
            {
                Sheets = sheets
            }

        //static member fromRowWiseAssay (rwa : RowWiseAssay) = 
        //    rwa.Sheets
        //    |> List.map ColumnWiseSheet.fromRowWiseSheet
        //    |> ColumnWiseAssay.create
