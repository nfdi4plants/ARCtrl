namespace ISADotNet.XSLX

open DocumentFormat.OpenXml.Spreadsheet
open FSharpSpreadsheetML
open ISADotNet
open Comment
open Remark
open System.Collections.Generic

module Assays = 
    let readAssays (prefix : string) lineNumber (en:IEnumerator<Row>) =
        let rec loop 
            measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
            comments remarks lineNumber = 

            let create () = 
                let length =
                    [|measurementTypes;measurementTypeTermAccessionNumbers;measurementTypeTermSourceREFs;technologyTypes;technologyTypeTermAccessionNumbers;technologyTypeTermSourceREFs;technologyPlatforms;fileNames|]
                    |> Array.map Array.length
                    |> Array.max

                List.init length (fun i ->
                    let measurementType = 
                        OntologyAnnotation.create 
                            (Array.tryItemDefault i "" measurementTypes)
                            (Array.tryItemDefault i "" measurementTypeTermAccessionNumbers)
                            (Array.tryItemDefault i "" measurementTypeTermSourceREFs)
                            []
                        |> REF.Item
                    let technologyType = 
                        OntologyAnnotation.create 
                            (Array.tryItemDefault i "" technologyTypes)
                            (Array.tryItemDefault i "" technologyTypeTermAccessionNumbers)
                            (Array.tryItemDefault i "" technologyTypeTermSourceREFs)
                            []
                        |> REF.Item
                    let comments = 
                        List.map (fun (key,values) -> 
                            Comment.create "" key (Array.tryItemDefault i "" values)
                        ) comments
                    Assay.create
                        ""
                        (Array.tryItemDefault i "" fileNames)
                        measurementType
                        technologyType
                        (Array.tryItemDefault i "" technologyPlatforms)
                        []
                        ([],[])
                        []
                        []
                        []
                        comments
                    |> REF.Item
                )
            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v) |> Array.ofIndexedSeq
                match Array.tryItem 0 row , Array.trySkip 1 row with

                | Comment k, Some v -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        ((k,v) :: comments) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        comments (Remark.create lineNumber k :: remarks) (lineNumber + 1)

                | Some k, Some measurementTypes when k = prefix + " " + Assay.MeasurementTypeTab -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        comments remarks (lineNumber + 1)

                | Some k, Some measurementTypeTermAccessionNumbers when k = prefix + " " + Assay.MeasurementTypeTermAccessionNumberTab -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        comments remarks (lineNumber + 1)

                | Some k, Some measurementTypeTermSourceREFs when k = prefix + " " + Assay.MeasurementTypeTermSourceREFTab -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        comments remarks (lineNumber + 1)

                | Some k, Some technologyTypes when k = prefix + " " + Assay.TechnologyTypeTab -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        comments remarks (lineNumber + 1)

                | Some k, Some technologyTypeTermAccessionNumbers when k = prefix + " " + Assay.TechnologyTypeTermAccessionNumberTab -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        comments remarks (lineNumber + 1)

                | Some k, Some technologyTypeTermSourceREFs when k = prefix + " " + Assay.TechnologyTypeTermSourceREFTab -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        comments remarks (lineNumber + 1)

                | Some k, Some technologyPlatforms when k = prefix + " " + Assay.TechnologyPlatformTab -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        comments remarks (lineNumber + 1)

                | Some k, Some fileNames when k = prefix + " " + Assay.FileNameTab -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,create ()
                | _ -> None, lineNumber,remarks,create ()
            else
                None,lineNumber,remarks,create ()
        loop [||] [||] [||] [||] [||] [||] [||] [||] [] [] lineNumber

    
    let writeAssays prefix (assays : Assay list) =
        let commentKeys = assays |> List.collect (fun assay -> assay.Comments |> List.map (fun c -> c.Name)) |> List.distinct

        seq { 
            let measurementTypes,measurementTypeAccessions,measurementTypeSourceRefs = assays |> List.map (fun a -> dismantleOntology a.MeasurementType) |> List.unzip3
            let technologyTypes,technologyTypeAccessions,technologyTypeSourceRefs = assays |> List.map (fun a -> dismantleOntology a.TechnologyType) |> List.unzip3
            yield   ( Row.ofValues None 0u (prefix + " " + Assay.MeasurementTypeTab                      :: measurementTypes))
            yield   ( Row.ofValues None 0u (prefix + " " + Assay.MeasurementTypeTermAccessionNumberTab   :: measurementTypeAccessions))
            yield   ( Row.ofValues None 0u (prefix + " " + Assay.MeasurementTypeTermSourceREFTab         :: measurementTypeSourceRefs))
            yield   ( Row.ofValues None 0u (prefix + " " + Assay.TechnologyTypeTab                       :: technologyTypes))
            yield   ( Row.ofValues None 0u (prefix + " " + Assay.TechnologyTypeTermAccessionNumberTab    :: technologyTypeAccessions))
            yield   ( Row.ofValues None 0u (prefix + " " + Assay.TechnologyTypeTermSourceREFTab          :: technologyTypeSourceRefs ))
            yield   ( Row.ofValues None 0u (prefix + " " + Assay.TechnologyPlatformTab                   :: (assays |> List.map (fun assay -> assay.TechnologyPlatform))))
            yield   ( Row.ofValues None 0u (prefix + " " + Assay.FileNameTab                             :: (assays |> List.map (fun assay -> assay.FileName))))               
            
            for key in commentKeys do
                let values = 
                    assays |> List.map (fun assay -> 
                        List.tryPickDefault (fun (c : Comment) -> if c.Name = key then Some c.Value else None) "" assay.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }