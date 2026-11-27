namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process


module Process =    
    
    module ROCrate =
        let genID (p:Process) : string = 
            match p.ID with
                | Some id -> URI.toString id
                | None -> match p.Name with
                            | Some n -> "#Process_" + n.Replace(" ","_")
                            | None -> "#EmptyProcess"

        let encoder (studyName:string Option) (assayName:string Option) (oa : Process) =            
            [
                "@id", Encode.string (oa |> genID) |> Some
                "@type", (Encode.list [Encode.string "Process"]) |> Some 
                Encode.tryInclude "name" Encode.string (oa.Name)
                Encode.tryInclude "executesProtocol" (Protocol.ROCrate.encoder studyName assayName oa.Name) (oa.ExecutesProtocol)
                Encode.tryIncludeListOpt "parameterValues" ProcessParameterValue.ROCrate.encoder (oa.ParameterValues)
                Encode.tryInclude "performer" Person.ROCrate.encodeAuthorListString (oa.Performer)
                Encode.tryInclude "date" Encode.string (oa.Date)
                Encode.tryIncludeListOpt "inputs" ProcessInput.ROCrate.encoder (oa.Inputs)
                Encode.tryIncludeListOpt "outputs" ProcessOutput.ROCrate.encoder (oa.Outputs)
                Encode.tryIncludeListOpt "comments" Comment.ROCrate.encoder (oa.Comments)
                "@context", ROCrateContext.Process.context_jsonvalue |> Some
            ]
            |> Encode.choose
            |> Encode.object

        let decoder : Decoder<Process> =
            Decode.object (fun get ->
                {
                    ID = get.Optional.Field "@id" Decode.uri
                    Name = get.Optional.Field "name" Decode.string
                    ExecutesProtocol = get.Optional.Field "executesProtocol" Protocol.ROCrate.decoder
                    ParameterValues = get.Optional.Field "parameterValues" (Decode.list ProcessParameterValue.ROCrate.decoder)
                    Performer = get.Optional.Field "performer" Person.ROCrate.decodeAuthorListString
                    Date = get.Optional.Field "date" Decode.string
                    PreviousProcess = None
                    NextProcess = None
                    Inputs = get.Optional.Field "inputs" (Decode.list ProcessInput.ROCrate.decoder)
                    Outputs = get.Optional.Field "outputs" (Decode.list ProcessOutput.ROCrate.decoder)
                    Comments = get.Optional.Field "comments" (Decode.list Comment.ROCrate.decoder)
                }
            )

    module ISAJson =

        let rec encoder (studyName:string Option) (assayName:string Option) (idMap : IDTable.IDTableWrite option) (oa : Process) =
            let f (oa : Process) =
                [
                    Encode.tryInclude "@id" Encode.string (ROCrate.genID oa |> Some)
                    Encode.tryInclude "name" Encode.string oa.Name
                    Encode.tryInclude "executesProtocol" (Protocol.ISAJson.encoder studyName assayName oa.Name idMap) oa.ExecutesProtocol
                    Encode.tryIncludeListOpt "parameterValues" (ProcessParameterValue.ISAJson.encoder idMap) oa.ParameterValues
                    Encode.tryInclude "performer" Encode.string oa.Performer
                    Encode.tryInclude "date" Encode.string oa.Date
                    Encode.tryInclude "previousProcess" (encoder studyName assayName idMap) oa.PreviousProcess
                    Encode.tryInclude "nextProcess" (encoder studyName assayName idMap) oa.NextProcess
                    Encode.tryIncludeListOpt "inputs" (ProcessInput.ISAJson.encoder idMap) oa.Inputs
                    Encode.tryIncludeListOpt "outputs" (ProcessOutput.ISAJson.encoder idMap) oa.Outputs
                    Encode.tryIncludeListOpt "comments" (Comment.ISAJson.encoder idMap) oa.Comments
                ]
                |> Encode.choose
                |> Encode.object
            match idMap with
            | None -> f oa
            | Some idMap -> IDTable.encode ROCrate.genID f oa idMap

        let decoder: Decoder<Process> =
            let rec decode() =
                Decode.object (fun get ->
                    {
                        ID = get.Optional.Field "@id" Decode.uri
                        Name = get.Optional.Field "name" Decode.string
                        ExecutesProtocol = get.Optional.Field "executesProtocol" Protocol.ISAJson.decoder
                        ParameterValues = get.Optional.Field "parameterValues" (Decode.list ProcessParameterValue.ISAJson.decoder)
                        Performer = get.Optional.Field "performer" Decode.string
                        Date = get.Optional.Field "date" Decode.string
                        PreviousProcess = get.Optional.Field "previousProcess" <| decode()
                        NextProcess = get.Optional.Field "nextProcess" <| decode()
                        Inputs = get.Optional.Field "inputs" (Decode.list ProcessInput.ISAJson.decoder)
                        Outputs = get.Optional.Field "outputs" (Decode.list ProcessOutput.ISAJson.decoder)
                        Comments = get.Optional.Field "comments" (Decode.list Comment.ISAJson.decoder)
                    }
                )
            decode()

        let private mergeLists (l1 : 'T list option) (l2 : 'T list option) =
            match l1, l2 with
            | Some l1, Some l2 -> List.append l1 l2 |> List.distinct |> Some
            | Some l, None | None, Some l -> Some l
            | None, None -> None

        /// Merges the values sharing an id and returns a function replacing a value by the merged one
        let private mergeByID (genID : 'T -> string) (merge : 'T -> 'T -> 'T) (values : 'T list) =
            let merged = System.Collections.Generic.Dictionary<string,'T>()
            for v in values do
                let id = genID v
                if merged.ContainsKey id then merged.[id] <- merge merged.[id] v
                else merged.Add(id, v)
            fun (v : 'T) ->
                let id = genID v
                if merged.ContainsKey id then merged.[id] else v

        /// Returns a function, which replaces the sources, samples and materials of processes by ones containing the annotations of all their occurrences in the given processes.
        ///
        /// Their id is created from their name, but which characteristics and factors they contain depends on the table they are used in. As only the first object with an id is written in full when using an id table, the others would be lost otherwise.
        let mergeMaterials (processes : Process list) : Process list -> Process list =
            let source =
                ProcessSequence.getSources processes
                |> mergeByID Source.ROCrate.genID (fun s1 s2 ->
                    { s1 with Characteristics = mergeLists s1.Characteristics s2.Characteristics }
                )
            let sample =
                ProcessSequence.getSamples processes
                |> mergeByID Sample.ROCrate.genID (fun s1 s2 ->
                    { s1 with
                        Characteristics = mergeLists s1.Characteristics s2.Characteristics
                        FactorValues = mergeLists s1.FactorValues s2.FactorValues
                        DerivesFrom = mergeLists s1.DerivesFrom s2.DerivesFrom
                    }
                )
            let material =
                ProcessSequence.getMaterials processes
                |> mergeByID Material.ROCrate.genID (fun m1 m2 ->
                    { m1 with
                        Characteristics = mergeLists m1.Characteristics m2.Characteristics
                        DerivesFrom = mergeLists m1.DerivesFrom m2.DerivesFrom
                    }
                )
            List.map (fun (p : Process) ->
                { p with
                    Inputs =
                        p.Inputs
                        |> Option.map (List.map (fun i ->
                            match i with
                            | ProcessInput.Source s -> ProcessInput.Source (source s)
                            | ProcessInput.Sample s -> ProcessInput.Sample (sample s)
                            | ProcessInput.Material m -> ProcessInput.Material (material m)
                            | ProcessInput.Data _ -> i
                        ))
                    Outputs =
                        p.Outputs
                        |> Option.map (List.map (fun o ->
                            match o with
                            | ProcessOutput.Sample s -> ProcessOutput.Sample (sample s)
                            | ProcessOutput.Material m -> ProcessOutput.Material (material m)
                            | ProcessOutput.Data _ -> o
                        ))
                }
            )
