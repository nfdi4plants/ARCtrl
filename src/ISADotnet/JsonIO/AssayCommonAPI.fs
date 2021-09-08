namespace ISADotNet.Json

open ISADotNet
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

module AssayCommonAPI = 

    //[<AnyOf>]
    //type NamedValue =
    //    | [<SerializationOrder(0)>] Parameter of ProcessParameterValue
    //    | [<SerializationOrder(1)>] Characteristic of MaterialAttributeValue
    //    | [<SerializationOrder(2)>] Factor of FactorValue

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
            [<JsonPropertyName(@"parameterValues")>]
            ParameterValues : ProcessParameterValue list
            [<JsonPropertyName(@"characteristicValues")>]
            CharacteristicValues : MaterialAttributeValue list
            [<JsonPropertyName(@"factorValues")>]
            FactorValues : FactorValue list
        }

        static member create(?Input,?Output,?InputType,?OutputType,?ParamValues,?CharValues,?FactorValues) : Row=
            {
                Input = Input |> Option.defaultValue ""
                Output = Output |> Option.defaultValue ""
                InputType = InputType
                OutputType = OutputType
                ParameterValues = ParamValues |> Option.defaultValue []
                CharacteristicValues = CharValues |> Option.defaultValue []
                FactorValues = FactorValues |> Option.defaultValue []
            }

        static member fromProcess (proc : Process) : Row list =
            let parameterValues = proc.ParameterValues |> Option.defaultValue []
            (proc.Inputs.Value,proc.Outputs.Value)
            ||> List.map2 (fun inp out ->
                let inpCharacteristics = API.ProcessInput.tryGetCharacteristics inp |> Option.defaultValue []
                let outCharacteristics = API.ProcessOutput.tryGetCharacteristics out |> Option.defaultValue []
                let characteristics = Set.intersect (set inpCharacteristics) (set outCharacteristics) |> Set.toList
                let factors = API.ProcessOutput.tryGetFactorValues out |> Option.defaultValue []

                let inputName = inp.GetName
                let outputName = out.GetName

                Row.create(inputName,outputName,ParamValues = parameterValues, CharValues = characteristics,FactorValues = factors)
            )


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

    type RowWiseAssay =
        {
            //[<JsonPropertyName(@"assayName")>]
            //AssayName : string
            [<JsonPropertyName(@"sheets")>]
            Sheets : RowWiseSheet list
        }

        static member create (*assayName*) sheets : RowWiseAssay =
            {
                //AssayName = assayName
                Sheets = sheets
            }

        static member fromAssay (assay : Assay) = 
            assay.ProcessSequence |> Option.defaultValue []
            |> List.groupBy (fun x -> x.Name.Value.Split '_' |> Array.item 0)
            |> List.map (fun (name,processes) -> RowWiseSheet.fromProcesses name processes)
            |> RowWiseAssay.create (*(assay.FileName |> Option.defaultValue "")*)

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

        static member fromRowWiseSheet (rws : RowWiseSheet) = 
            let parameters = 
                rws.Rows
                |> List.map (fun row -> row.ParameterValues)
                |> List.transpose 
                |> List.map ParameterColumn.fromParams
            let characteristics = 
                rws.Rows
                |> List.map (fun row -> row.CharacteristicValues)
                |> List.transpose 
                |> List.map CharacteristicColumn.fromCharacteristics
            let factors = 
                rws.Rows
                |> List.map (fun row -> row.FactorValues)
                |> List.transpose 
                |> List.map FactorColumn.fromFactors
            ColumnWiseSheet.create rws.SheetName parameters characteristics factors

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

        static member fromRowWiseSheet (rwa : RowWiseAssay) = 
            rwa.Sheets
            |> List.map ColumnWiseSheet.fromRowWiseSheet
            |> ColumnWiseAssay.create
