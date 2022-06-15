﻿namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

open System.Collections.Generic
open System.Collections


type QAssay(FileName : string option,MeasurementType : OntologyAnnotation option,TechnologyType : OntologyAnnotation option,TechnologyPlatform : string option,Sheets : QSheet list) =

    inherit QProcessSequence(Sheets)

    member this.FileName = FileName
    member this.MeasurementType = MeasurementType
    member this.TechnologyType = TechnologyType
    member this.TechnologyPlatform = TechnologyPlatform

    static member fromAssay (assay : Assay, ?ReferenceSheets : QSheet list) =
        let sheets = 
            match ReferenceSheets with
            | Some ref -> 
                assay.ProcessSequence 
                |> Option.defaultValue []
                |> fun sheets -> QProcessSequence(sheets,ref)
            | None ->
                assay.ProcessSequence 
                |> Option.defaultValue []
                |> QProcessSequence
            |> Seq.toList

        QAssay(assay.FileName,assay.MeasurementType,assay.TechnologyType,assay.TechnologyPlatform,sheets)


    member this.Protocol (sheetName : string) =
        base.Protocol(sheetName, $"Assay \"{this.FileName}\"")

    member this.Protocol (index : int) =
        base.Protocol(index, $"Assay \"{this.FileName}\"")
       
    //interface IEnumerable<QSheet> with
    //    member this.GetEnumerator() = (Seq.ofList this.Sheets).GetEnumerator()

    //interface IEnumerable with
    //    member this.GetEnumerator() = (this :> IEnumerable<QSheet>).GetEnumerator() :> IEnumerator

    /// Returns the initial inputs final outputs of the assay, to which no processPoints
    static member getRootInputs (assay : QAssay) = QProcessSequence.getRootInputs assay

    /// Returns the final outputs of the assay, which point to no further nodes
    static member getFinalOutputs (assay : QAssay) = QProcessSequence.getFinalOutputs assay

    /// Returns the initial inputs final outputs of the assay, to which no processPoints
    static member getRootInputOf (assay : QAssay) (sample : string) = QProcessSequence.getRootInputsOfBy (fun _ -> true) sample assay 
        
    /// Returns the final outputs of the assay, which point to no further nodes
    static member getFinalOutputsOf (assay : QAssay) (sample : string) = QProcessSequence.getFinalOutputsOfBy (fun _ -> true) sample assay

    static member toString (rwa : QAssay) =  JsonSerializer.Serialize<QAssay>(rwa,JsonExtensions.options)

    static member toFile (path : string) (rwa:QAssay) = 
        File.WriteAllText(path,QAssay.toString rwa)

    static member fromString (s:string) = 
        JsonSerializer.Deserialize<QAssay>(s,JsonExtensions.options)

    static member fromFile (path : string) = 
        File.ReadAllText path 
        |> QAssay.fromString
