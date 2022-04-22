namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

open System.Collections.Generic
open System.Collections


type QProcessSequence (sheets : QSheet list) =

    member this.Sheets = sheets

    new (processSequence : Process list) =
        let sheets = 
            processSequence
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
        QProcessSequence(sheets)

    static member fromAssay (assay : Assay) =
        
        QProcessSequence(assay.ProcessSequence |> Option.defaultValue [])
      

    member this.Protocol (i : int, ?EntityName) =
        let sheet = 
            this.Sheets 
            |> List.tryItem i
        match sheet with
        | Some s -> s
        | None -> failwith $"""{EntityName |> Option.defaultValue "ProcessSequence"} does not contain sheet with index {i} """

    member this.Protocol (sheetName, ?EntityName) =
        let sheet = 
            this.Sheets 
            |> List.tryFind (fun sheet -> sheet.SheetName = sheetName)
        match sheet with
        | Some s -> s
        | None -> failwith $"""{EntityName |> Option.defaultValue "ProcessSequence"} does not contain sheet with name "{sheetName}" """

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
    static member getRootInputs (ps : #QProcessSequence) =
        let inputs = ps.Protocols |> List.collect (fun p -> p.Rows |> List.map (fun r -> r.Input))
        let outputs =  ps.Protocols |> List.collect (fun p -> p.Rows |> List.map (fun r -> r.Output)) |> Set.ofList
        inputs
        |> List.filter (fun i -> outputs.Contains i |> not)

    /// Returns the final outputs of the assay, which point to no further nodes
    static member getFinalOutputs (ps : #QProcessSequence) =
        let inputs = ps.Protocols |> List.collect (fun p -> p.Rows |> List.map (fun r -> r.Input)) |> Set.ofList
        let outputs =  ps.Protocols |> List.collect (fun p -> p.Rows |> List.map (fun r -> r.Output))
        outputs
        |> List.filter (fun i -> inputs.Contains i |> not)

    /// Returns the initial inputs final outputs of the assay, to which no processPoints
    static member getRootInputOf (ps : #QProcessSequence) (sample : string) =
        let mappings = 
            ps.Protocols 
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
       
     /// Returns the initial inputs final outputs of the assay, to which no processPoints
    static member getPreviousValuesOf (ps : #QProcessSequence) (sample : string) =
        let mappings = 
            ps.Protocols 
            |> List.collect (fun p -> 
                p.Rows 
                |> List.map (fun r -> r.Output,r)
                |> List.distinct
            ) 

            |> Map.ofList
        let rec loop values lastState state = 
            if lastState = state then values 
            else
                let newState,newValues = 
                    state 
                    |> List.map (fun s -> 
                        mappings.TryFind s 
                        |> Option.map (fun r -> r.Input,r.Values)
                        |> Option.defaultValue (s,[])
                    )
                    |> List.unzip
                    |> fun (s,vs) -> s, vs |> List.concat
                loop (newValues@values) state newState
        loop [] [] [sample]  
        |> ValueCollection

    /// Returns the initial inputs final outputs of the assay, to which no processPoints
    static member getSucceedingValuesOf (ps : #QProcessSequence) (sample : string) =
        let mappings = 
            ps.Protocols 
            |> List.collect (fun p -> 
                p.Rows 
                |> List.map (fun r -> r.Input,r)
                |> List.distinct
            ) 

            |> Map.ofList
        let rec loop values lastState state = 
            if lastState = state then values 
            else
                let newState,newValues = 
                    state 
                    |> List.map (fun s -> 
                        mappings.TryFind s 
                        |> Option.map (fun r -> r.Output,r.Values)
                        |> Option.defaultValue (s,[])
                    )
                    |> List.unzip
                    |> fun (s,vs) -> s, vs |> List.concat
                loop (values@newValues) state newState
        loop [] [] [sample]
        |> ValueCollection

    /// Returns the final outputs of the assay, which point to no further nodes
    static member getFinalOutputsOf (ps : #QProcessSequence) (sample : string) =
        let mappings = 
            ps.Protocols 
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
                |> QProcessSequence.getRootInputOf this |> List.distinct
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
                |> QProcessSequence.getFinalOutputsOf this |> List.distinct
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
                let outs = r.Output |> QProcessSequence.getFinalOutputsOf this |> List.distinct
                let inps = r.Input |> QProcessSequence.getRootInputOf this |> List.distinct
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

    member this.ValuesOf(name) =
        (QProcessSequence.getPreviousValuesOf this name).Values @ (QProcessSequence.getSucceedingValuesOf this name).Values
        |> ValueCollection

    member this.PreviousValuesOf(name) =
        QProcessSequence.getPreviousValuesOf this name

    member this.SucceedingValuesOf(name) =
        QProcessSequence.getSucceedingValuesOf this name

    member this.CharacteristicsOf(name) =
        this.ValuesOf(name).Characteristics

    member this.PreviousCharacteristicsOf(name) =
        this.PreviousValuesOf(name).Characteristics

    member this.SucceedingCharacteristicsOf(name) =
        this.SucceedingValuesOf(name).Characteristics

    //static member toString (rwa : QAssay) =  JsonSerializer.Serialize<QAssay>(rwa,JsonExtensions.options)

    //static member toFile (path : string) (rwa:QAssay) = 
    //    File.WriteAllText(path,QAssay.toString rwa)

    //static member fromString (s:string) = 
    //    JsonSerializer.Deserialize<QAssay>(s,JsonExtensions.options)

    //static member fromFile (path : string) = 
    //    File.ReadAllText path 
    //    |> QAssay.fromString
