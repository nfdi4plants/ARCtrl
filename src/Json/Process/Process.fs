namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process


module Process =    
    
    let genID (p:Process) : string = 
        match p.ID with
            | Some id -> URI.toString id
            | None -> match p.Name with
                        | Some n -> "#Process_" + n.Replace(" ","_")
                        | None -> "#EmptyProcess"

    module ISAJson =


        let rec encoder (studyName:string Option) (assayName:string Option) (idMap : IDTable.IDTableWrite option) (oa : Process) =
            let f (oa : Process) =
                [
                    Encode.tryInclude "@id" Encode.string (genID oa |> Some)
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
            | Some idMap -> IDTable.encode genID f oa idMap

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
