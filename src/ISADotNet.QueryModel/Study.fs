namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

open System.Collections.Generic
open System.Collections

/// Queryable representation of an ISA Study. Implements the QProcessSequence interface
type QStudy
    (
        FileName : string option,
        Identifier : string option,
        Title : string option,
        Description : string option,
        SubmissionDate : string option,
        PublicReleaseDate : string option,
        Publications : Publication list option,
        Contacts : Person list option,
        StudyDesignDescriptors : OntologyAnnotation list option, 
        Comments : QCommentCollection, 
        Assays : QAssay list, 
        Sheets : QSheet list) =

    inherit QProcessSequence(Sheets)

    member this.FileName = FileName
    member this.Identifier = Identifier
    member this.Title = Title
    member this.Description = Description
    member this.SubmissionDate = SubmissionDate
    member this.PublicReleaseDate = PublicReleaseDate
    member this.Publications = Publications
    member this.Contacts = Contacts
    member this.StudyDesignDescriptors = StudyDesignDescriptors
    member this.Comments = Comments
    member this.Assays = Assays

    static member fromStudy (study : Study, ?ReferenceSheets : QSheet list) =
        
        let comments = QCommentCollection(study.Comments)
            
        let refSheets = 
            study.Assays 
            |> Option.map (List.collect (fun a -> a.ProcessSequence |> Option.defaultValue []) )
            |> Option.defaultValue []
            |> List.append (study.ProcessSequence |> Option.defaultValue [])
            |> fun s ->
                match ReferenceSheets with
                | Some ref -> QProcessSequence(s,ref)
                | None -> QProcessSequence(s)
            |> Seq.toList

        let sheets = QProcessSequence(study.ProcessSequence |> Option.defaultValue [],refSheets) |> Seq.toList

        let assays = 
            study.Assays 
            |> Option.map (List.map (fun a -> QAssay.fromAssay(a,refSheets)))
            |> Option.defaultValue []

        QStudy(study.FileName,study.Identifier,study.Title,study.Description,study.SubmissionDate,study.PublicReleaseDate,study.Publications,study.Contacts,study.StudyDesignDescriptors,comments,assays,sheets)

    member this.FullProcessSequence =        
        this.Assays
        |> List.collect (fun a -> a.Sheets)
        |> List.append this.Sheets
        |> QProcessSequence

    /// Returns the QAssay with the given name
    member this.Assay(assayName : string) = 
        this.Assays
        |> List.find (fun a -> a.FileName.Value.Contains assayName)
        
    /// Returns the nth QAssay
    member this.Assay(i : int) = 
        this.Assays
        |> List.item i 

    /// get the protocol or sheet (in ISATab logic) with the given name
    member this.Protocol (sheetName : string) =
        base.Protocol(sheetName, $"Assay \"{this.FileName}\"")

    /// get the nth protocol or sheet (in ISATab logic) 
    member this.Protocol (index : int) =
        base.Protocol(index, $"Assay \"{this.FileName}\"")

    /// Returns the initial inputs final outputs of the assay, to which no processPoints
    static member getRootInputs (study : QStudy) = QProcessSequence.getRootInputs study

    /// Returns the final outputs of the study, which point to no further nodes
    static member getFinalOutputs (study : QStudy) = QProcessSequence.getFinalOutputs study

    /// Returns the initial inputs final outputs of the study, to which no processPoints
    static member getRootInputOf (study : QStudy) (sample : string) = QProcessSequence.getRootInputsOfBy (fun _ -> true) sample study 
        
    /// Returns the final outputs of the study, which point to no further nodes
    static member getFinalOutputsOf (study : QStudy) (sample : string) = QProcessSequence.getFinalOutputsOfBy (fun _ -> true) sample study

    static member toString (rwa : QStudy) =  JsonSerializer.Serialize<QStudy>(rwa,JsonExtensions.options)

    static member toFile (path : string) (rwa:QStudy) = 
        File.WriteAllText(path,QStudy.toString rwa)

    static member fromString (s:string) = 
        JsonSerializer.Deserialize<QStudy>(s,JsonExtensions.options)

    static member fromFile (path : string) = 
        File.ReadAllText path 
        |> QStudy.fromString
