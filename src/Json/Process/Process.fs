namespace ARCtrl.Json

open Thoth.Json.Core

open ARCtrl
open ARCtrl.Process


module Process =    
    
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

    //let genID (p:Process) : string = 
    //    match p.ID with
    //        | Some id -> URI.toString id
    //        | None -> match p.Name with
    //                    | Some n -> "#Process_" + n.Replace(" ","_")
    //                    | None -> "#EmptyProcess"

    //let rec encoder (options : ConverterOptions) (studyName:string Option) (assayName:string Option) (oa : Process) = 
    //    let performer = if options.IsJsonLD then ROCrateHelper.Person.authorListStrinEncoder else Encode.string
    //    [
    //        if options.SetID then 
    //            "@id", Encode.string (oa |> genID)
    //        else 
    //            Encode.tryInclude "@id" Encode.string (oa.ID)
    //        if options.IsJsonLD then 
    //            "@type", (Encode.list [Encode.string "Process"])
    //        Encode.tryInclude "name" Encode.string (oa.Name)
    //        Encode.tryInclude "executesProtocol" (Protocol.encoder options studyName assayName oa.Name) (oa.ExecutesProtocol)
    //        Encode.tryIncludeList "parameterValues" (ProcessParameterValue.encoder options) (oa.ParameterValues)
    //        Encode.tryInclude "performer" performer (oa.Performer)
    //        Encode.tryInclude "date" Encode.string (oa.Date)
    //        if not options.IsJsonLD then
    //            Encode.tryInclude "previousProcess" (encoder options studyName assayName) (oa.PreviousProcess)
    //            Encode.tryInclude "nextProcess" (encoder options studyName assayName) (oa.NextProcess)
    //        Encode.tryIncludeList "inputs" (ProcessInput.encoder options) (oa.Inputs)
    //        Encode.tryIncludeList "outputs" (ProcessOutput.encoder options) (oa.Outputs)
    //        Encode.tryIncludeList "comments" (Comment.encoder options) (oa.Comments)
    //        if options.IsJsonLD then 
    //            "@context", ROCrateContext.Process.context_jsonvalue
    //    ]
    //    |> Encode.choose
    //    |> Encode.object

    //let rec decoder (options : ConverterOptions) : Decoder<Process> =
    //    Decode.object (fun get ->
    //        {
    //            ID = get.Optional.Field "@id" GDecode.uri
    //            Name = get.Optional.Field "name" Decode.string
    //            ExecutesProtocol = get.Optional.Field "executesProtocol" (Protocol.decoder options)
    //            ParameterValues = get.Optional.Field "parameterValues" (Decode.list (ProcessParameterValue.decoder options))
    //            Performer = get.Optional.Field "performer" Decode.string
    //            Date = get.Optional.Field "date" Decode.string
    //            PreviousProcess = get.Optional.Field "previousProcess" (decoder options)
    //            NextProcess = get.Optional.Field "nextProcess" (decoder options)
    //            Inputs = get.Optional.Field "inputs" (Decode.list (ProcessInput.decoder options))
    //            Outputs = get.Optional.Field "outputs" (Decode.list (ProcessOutput.decoder options))
    //            Comments = get.Optional.Field "comments" (Decode.list (Comment.decoder options))
    //        }
    //    )

[<AutoOpen>]
module ProcessExtensions =
    
    type Process with

        static member fromISAJsonString (s:string) = 
            Decode.fromJsonString Process.ISAJson.decoder s   

        static member toISAJsonString(?spaces) =
            fun (f:Process) ->
                Process.ISAJson.encoder f
                |> Encode.toJsonString (Encode.defaultSpaces spaces)