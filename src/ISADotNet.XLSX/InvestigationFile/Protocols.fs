namespace ISADotNet.XSLX

open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open Comment
open Remark
open System.Collections.Generic

module Protocols = 
    let readProtocols (prefix : string) lineNumber (en:IEnumerator<Row>) =
        let rec loop 
            names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
            comments remarks lineNumber = 

            let create () = 
                let length =
                    [|names;protocolTypes;typeTermAccessionNumbers;typeTermSourceREFs;descriptions;uris;versions;parametersNames;parametersTermAccessionNumbers;parametersTermSourceREFs;componentsNames;componentsTypes;componentsTypeTermAccessionNumbers;componentsTypeTermSourceREFs|]
                    |> Array.map Array.length
                    |> Array.max

                List.init length (fun i ->
                    let protocolType = 
                        OntologyAnnotation.create 
                            (Array.tryItemDefault i "" protocolTypes)
                            (Array.tryItemDefault i "" typeTermAccessionNumbers)
                            (Array.tryItemDefault i "" typeTermSourceREFs)
                            []
                        |> REF.Item
                    let parameters = 
                        splitAndCreateParameters 
                            (Array.tryItemDefault i "" parametersNames)
                            (Array.tryItemDefault i "" parametersTermAccessionNumbers)
                            (Array.tryItemDefault i "" parametersTermSourceREFs)

                    let components = 
                        splitAndCreateComponents
                            (Array.tryItemDefault i "" componentsNames)
                            (Array.tryItemDefault i "" componentsTypes)
                            (Array.tryItemDefault i "" componentsTypeTermAccessionNumbers)
                            (Array.tryItemDefault i "" componentsTypeTermSourceREFs)
                    let comments = 
                        List.map (fun (key,values) -> 
                            Comment.create "" key (Array.tryItemDefault i "" values)
                        ) comments
                    Protocol.create
                        (Array.tryItemDefault i "" names)
                        protocolType
                        (Array.tryItemDefault i "" descriptions)
                        (Array.tryItemDefault i "" uris)
                        (Array.tryItemDefault i "" versions)
                        parameters
                        components
                        comments
                    |> REF.Item
                )
            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v) |> Array.ofIndexedSeq
                match Array.tryItem 0 row , Array.trySkip 1 row with

                | Comment k, Some v -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        ((k,v) :: comments) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments (Remark.create lineNumber k :: remarks) (lineNumber + 1)

                | Some k, Some names when k = prefix + " " + Protocol.NameTab -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some protocolTypes when k = prefix + " " + Protocol.ProtocolTypeTab -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some typeTermAccessionNumbers when k = prefix + " " + Protocol.TypeTermAccessionNumberTab -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some typeTermSourceREFs when k = prefix + " " + Protocol.TypeTermSourceREFTab -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some descriptions when k = prefix + " " + Protocol.DescriptionTab -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some uris when k = prefix + " " + Protocol.URITab -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some versions when k = prefix + " " + Protocol.VersionTab -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some parametersNames when k = prefix + " " + Protocol.ParametersNameTab -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some parametersTermAccessionNumbers when k = prefix + " " + Protocol.ParametersTermAccessionNumberTab -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some parametersTermSourceREFs when k = prefix + " " + Protocol.ParametersTermSourceREFTab -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some componentsNames when k = prefix + " " + Protocol.ComponentsNameTab -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some componentsTypes when k = prefix + " " + Protocol.ComponentsTypeTab -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some componentsTypeTermAccessionNumbers when k = prefix + " " + Protocol.ComponentsTypeTermAccessionNumberTab -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some componentsTypeTermSourceREFs when k = prefix + " " + Protocol.ComponentsTypeTermSourceREFTab -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,create ()
                | _ -> None, lineNumber,remarks,create ()
            else
                None,lineNumber,remarks,create ()
        loop [||] [||] [||] [||] [||] [||] [||] [||] [||] [||] [||] [||] [||] [||] [] [] lineNumber

    
    
    let writeProtocols prefix (protocols : Protocol list) =
        let commentKeys = protocols |> List.collect (fun protocol -> protocol.Comments |> List.map (fun c -> c.Name)) |> List.distinct
    
        seq {
            let protocolTypes,protocolTypeAccessions,protocolTypeSourceRefs = protocols |> List.map (fun p -> dismantleOntology p.ProtocolType) |> List.unzip3
            let parameterNames,parameterAccessions,parameterSourceRefs = protocols |> List.map (fun p -> mergeParameters p.Parameters) |> List.unzip3
            let componentNames,componentTypes,componentAccessions,componentSourceRefs = protocols |> List.map (fun p -> mergeComponents p.Components) |> List.unzip4
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.NameTab                                 :: (protocols |> List.map (fun protocol -> protocol.Name))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.ProtocolTypeTab                         :: protocolTypes))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.TypeTermAccessionNumberTab              :: protocolTypeAccessions))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.TypeTermSourceREFTab                    :: protocolTypeSourceRefs))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.DescriptionTab                          :: (protocols |> List.map (fun protocol -> protocol.Description))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.URITab                                  :: (protocols |> List.map (fun protocol -> protocol.Uri))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.VersionTab                              :: (protocols |> List.map (fun protocol -> protocol.Version))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.ParametersNameTab                       :: parameterNames))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.ParametersTermAccessionNumberTab        :: parameterAccessions))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.ParametersTermSourceREFTab              :: parameterSourceRefs))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.ComponentsNameTab                       :: componentNames))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.ComponentsTypeTab                       :: componentTypes))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.ComponentsTypeTermAccessionNumberTab    :: componentAccessions))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.ComponentsTypeTermSourceREFTab          :: componentSourceRefs))
    
            for key in commentKeys do
                let values = 
                    protocols |> List.map (fun protocol -> 
                        List.tryPickDefault (fun (c : Comment) -> if c.Name = key then Some c.Value else None) "" protocol.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }