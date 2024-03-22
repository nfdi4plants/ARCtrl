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

        let rec encoder (studyName:string Option) (assayName:string Option) (oa : Process) =            
            [
                "@id", Encode.string (oa |> genID)
                "@type", (Encode.list [Encode.string "Process"])
                Encode.tryInclude "name" Encode.string (oa.Name)
                Encode.tryInclude "executesProtocol" (Protocol.ROCrate.encoder studyName assayName oa.Name) (oa.ExecutesProtocol)
                Encode.tryIncludeListOpt "parameterValues" ProcessParameterValue.ROCrate.encoder (oa.ParameterValues)
                Encode.tryInclude "performer" Person.ROCrate.encodeAuthorListString (oa.Performer)
                Encode.tryInclude "date" Encode.string (oa.Date)
                Encode.tryIncludeListOpt "inputs" ProcessInput.ROCrate.encoder (oa.Inputs)
                Encode.tryIncludeListOpt "outputs" ProcessOutput.ROCrate.encoder (oa.Outputs)
                Encode.tryIncludeListOpt "comments" Comment.ROCrate.encoder (oa.Comments)
                "@context", ROCrateContext.Process.context_jsonvalue
            ]
            |> Encode.choose
            |> Encode.object

        let rec decoder : Decoder<Process> =
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

        let rec encoder (oa : Process) = 
            [
                Encode.tryInclude "@id" Encode.string oa.ID
                Encode.tryInclude "name" Encode.string oa.Name
                Encode.tryInclude "executesProtocol" Protocol.ISAJson.encoder oa.ExecutesProtocol
                Encode.tryIncludeListOpt "parameterValues" ProcessParameterValue.ISAJson.encoder oa.ParameterValues
                Encode.tryInclude "performer" Encode.string oa.Performer
                Encode.tryInclude "date" Encode.string oa.Date
                Encode.tryInclude "previousProcess" encoder oa.PreviousProcess
                Encode.tryInclude "nextProcess" encoder oa.NextProcess
                Encode.tryIncludeListOpt "inputs" ProcessInput.ISAJson.encoder oa.Inputs
                Encode.tryIncludeListOpt "outputs" ProcessOutput.ISAJson.encoder oa.Outputs
                Encode.tryIncludeListOpt "comments" Comment.ISAJson.encoder oa.Comments
            ]
            |> Encode.choose
            |> Encode.object

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

[<AutoOpen>]
module ProcessExtensions =
    
    type Process with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Process.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:Process) ->
                Process.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)

        static member fromROCrateString (s:string) =
            Decode.fromJsonString Process.ROCrate.decoder s

        static member toROCrateString (studyName:string Option) (assayName:string Option) (?spaces) =
            fun (f:Process) ->
                Process.ROCrate.encoder studyName assayName f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)