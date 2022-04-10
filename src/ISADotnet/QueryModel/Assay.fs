namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

open System.Collections.Generic
open System.Collections


type QAssay =
    {
        [<JsonPropertyName(@"filename")>]
        FileName : string option
        [<JsonPropertyName(@"measurementType")>]
        MeasurementType : OntologyAnnotation option
        [<JsonPropertyName(@"technologyType")>]
        TechnologyType : OntologyAnnotation option
        [<JsonPropertyName(@"technologyPlatform")>]
        TechnologyPlatform : string option
        [<JsonPropertyName(@"sheets")>]
        Sheets : QSheet list
    }

    static member create fileName measurementType technologyType technologyPlatform sheets : QAssay =
        {
            FileName = fileName
            MeasurementType = measurementType
            TechnologyType = technologyType
            TechnologyPlatform = technologyPlatform
            Sheets = sheets
        }

    static member fromAssay (assay : Assay) =
        let sheets = 
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
            |> List.map (fun (name,processes) -> QSheet.fromProcesses name processes)
        QAssay.create assay.FileName assay.MeasurementType assay.TechnologyType assay.TechnologyPlatform sheets
      
    member this.Protocol (i : int) =
        this.Sheets.[i]

    member this.Protocol (sheetName) =
        let sheet = 
            this.Sheets 
            |> List.tryFind (fun sheet -> sheet.SheetName = sheetName)
        match sheet with
        | Some s -> s
        | None -> failwith $"Assay \"{this.FileName}\" does not contain sheet with name \"{sheetName}\""

    member this.Protocols = this.Sheets

    member this.ProtocolCount =
        this.Sheets 
        |> List.length

    member this.ProtocolNames =
        this.Sheets 
        |> List.map (fun sheet -> sheet.SheetName)
       
    interface IEnumerable<QSheet> with
        member this.GetEnumerator() = (Seq.ofList this.Sheets).GetEnumerator()

    interface IEnumerable with
        member this.GetEnumerator() = (this :> IEnumerable<QSheet>).GetEnumerator() :> IEnumerator

    /// Returns the initial inputs final outputs of the assay, to which no processPoints
    static member getRootInputs (assay : QAssay) =
        let inputs = assay.Protocols |> List.collect (fun p -> p.Rows |> List.map (fun r -> r.Input))
        let outputs =  assay.Protocols |> List.collect (fun p -> p.Rows |> List.map (fun r -> r.Output)) |> Set.ofList
        inputs
        |> List.filter (fun i -> outputs.Contains i |> not)

    /// Returns the final outputs of the assay, which point to no further nodes
    static member getFinalOutputs (assay : QAssay) =
        let inputs = assay.Protocols |> List.collect (fun p -> p.Rows |> List.map (fun r -> r.Input)) |> Set.ofList
        let outputs =  assay.Protocols |> List.collect (fun p -> p.Rows |> List.map (fun r -> r.Output))
        outputs
        |> List.filter (fun i -> inputs.Contains i |> not)

    /// Returns the initial inputs final outputs of the assay, to which no processPoints
    static member getRootInputOf (assay : QAssay) (sample : string) =
        let mappings = 
            assay.Protocols 
            |> List.collect (fun p -> 
                p.Rows 
                |> List.map (fun r -> r.Output,r.Input)
                |> List.distinct
            ) 
            |> List.groupBy fst 
            |> List.map (fun (out,ins) -> out, ins |> List.map snd)
            |> Map.ofList
        let rec loop lastState state = 
            if lastState = state then state 
            else
                let newState = 
                    state 
                    |> List.collect (fun s -> 
                        mappings.TryFind s 
                        |> Option.defaultValue [s]
                    )
                loop state newState
        loop [] [sample]
        
    /// Returns the final outputs of the assay, which point to no further nodes
    static member getFinalOutputsOf (assay : QAssay) (sample : string) =
        let mappings = 
            assay.Protocols 
            |> List.collect (fun p -> 
                p.Rows 
                |> List.map (fun r -> r.Input,r.Output)
                |> List.distinct
            ) 
            |> List.groupBy fst 
            |> List.map (fun (inp,outs) -> inp, outs |> List.map snd)
            |> Map.ofList
        let rec loop lastState state = 
            if lastState = state then state 
            else
                let newState = 
                    state 
                    |> List.collect (fun s -> 
                        mappings.TryFind s 
                        |> Option.defaultValue [s]
                    )
                loop state newState
        loop [] [sample]

    member this.Nearest = 
        this.Sheets
        |> List.collect (fun sheet -> sheet.Values |> Seq.toList)
        |> IOValueCollection
   
    member this.SinkNearest = 
        this.Sheets
        |> List.collect (fun sheet -> 
            sheet.Rows
            |> List.collect (fun r ->               
                r.Input
                |> QAssay.getRootInputOf this |> List.distinct
                |> List.collect (fun inp -> 
                    r.Values
                    |> List.map (fun v -> 
                        KeyValuePair((inp,r.Output),v)
                    )
                )
            )
        )
        |> IOValueCollection

    member this.SourceNearest = 
        this.Sheets
        |> List.collect (fun sheet -> 
            sheet.Rows
            |> List.collect (fun r ->               
                r.Output
                |> QAssay.getFinalOutputsOf this |> List.distinct
                |> List.collect (fun out -> 
                    r.Values
                    |> List.map (fun v -> 
                        KeyValuePair((r.Input,out),v)
                    )
                )
            )
        )
        |> IOValueCollection

    member this.Global =
        this.Sheets
        |> List.collect (fun sheet -> 
            sheet.Rows
            |> List.collect (fun r ->  
                let outs = r.Output |> QAssay.getFinalOutputsOf this |> List.distinct
                let inps = r.Input |> QAssay.getRootInputOf this |> List.distinct
                outs
                |> List.collect (fun out -> 
                    inps
                    |> List.collect (fun inp ->
                        r.Values
                        |> List.map (fun v -> 
                            KeyValuePair((inp,out),v)
                        )
                    )
                )
            )
        )
        |> IOValueCollection

    static member toString (rwa : QAssay) =  JsonSerializer.Serialize<QAssay>(rwa,JsonExtensions.options)

    static member toFile (path : string) (rwa:QAssay) = 
        File.WriteAllText(path,QAssay.toString rwa)

    static member fromString (s:string) = 
        JsonSerializer.Deserialize<QAssay>(s,JsonExtensions.options)

    static member fromFile (path : string) = 
        File.ReadAllText path 
        |> QAssay.fromString
