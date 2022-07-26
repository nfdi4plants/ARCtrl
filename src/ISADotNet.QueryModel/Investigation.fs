namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

open System.Collections.Generic
open System.Collections

/// Queryable representation of an ISA Investigation. Implements the QProcessSequence interface
type QInvestigation
    (
        FileName : string option,
        Identifier : string option,
        Title : string option,
        Description : string option,
        SubmissionDate : string option,
        PublicReleaseDate : string option,
        OntologySourceReferences : OntologySourceReference list option, 
        Publications : Publication list option,
        Contacts : Person list option, 
        Comments : QCommentCollection, 
        Studies : QStudy list, 
        Sheets : QSheet list) =

    inherit QProcessSequence(Sheets)

    member this.FileName = FileName
    member this.Identifier = Identifier
    member this.Title = Title
    member this.Description = Description
    member this.SubmissionDate = SubmissionDate
    member this.PublicReleaseDate = PublicReleaseDate
    member this.OntologySourceReferences = OntologySourceReferences
    member this.Publications = Publications
    member this.Contacts = Contacts
    member this.Comments = Comments
    member this.Studies = Studies

    static member fromInvestigation (investigation : Investigation) =
               
        let comments = QCommentCollection(investigation.Comments)

        let sheets = 
            investigation.Studies
            |> Option.defaultValue []
            |> List.collect (fun s -> 
                s.Assays
                |> Option.map (List.collect (fun a -> a.ProcessSequence |> Option.defaultValue []) )
                |> Option.defaultValue []
                |> List.append (s.ProcessSequence |> Option.defaultValue [])
                |> QProcessSequence
                |> Seq.toList
            )

        let studies = 
            investigation.Studies 
            |> Option.map (List.map (fun s -> QStudy.fromStudy(s,sheets)))
            |> Option.defaultValue []

        QInvestigation(
            investigation.FileName,
            investigation.Identifier,
            investigation.Title,
            investigation.Description,
            investigation.SubmissionDate,
            investigation.PublicReleaseDate,
            investigation.OntologySourceReferences,
            investigation.Publications,
            investigation.Contacts,
            comments,
            studies,
            sheets)


    /// Returns the QStudy with the given name
    member this.Study(studyName : string) = 
        this.Studies
        |> List.find (fun s -> s.Identifier.Value = studyName)
        
    /// Returns the nth QStudy
    member this.Study(i : int) = 
        this.Studies
        |> List.item i 

    /// Returns the QAssay with the given name (registered in the study with the given study name)
    member this.Assay(assayName : string, ?StudyName : string) = 
        match StudyName with
        | Some sn ->
            this.Study(sn).Assay(assayName)
        | None ->
            this.Studies
            |> List.collect (fun s -> s.Assays)
            |> List.find (fun a -> a.FileName.Value.Contains assayName)

    /// get the protocol or sheet (in ISATab logic) with the given name
    member this.Protocol (sheetName : string) =
        base.Protocol(sheetName, $"Assay \"{this.FileName}\"")

    /// get the nth protocol or sheet (in ISATab logic) 
    member this.Protocol (index : int) =
        base.Protocol(index, $"Assay \"{this.FileName}\"")

    /// Returns the initial inputs final outputs of the assay, to which no processPoints
    static member getRootInputs (investigation : QInvestigation) = QProcessSequence.getRootInputs investigation

    /// Returns the final outputs of the investigation, which point to no further nodes
    static member getFinalOutputs (investigation : QInvestigation) = QProcessSequence.getFinalOutputs investigation

    /// Returns the initial inputs final outputs of the investigation, to which no processPoints
    static member getRootInputOf (investigation : QInvestigation) (sample : string) = QProcessSequence.getRootInputsOfBy (fun _ -> true) sample investigation 
        
    /// Returns the final outputs of the investigation, which point to no further nodes
    static member getFinalOutputsOf (investigation : QInvestigation) (sample : string) = QProcessSequence.getFinalOutputsOfBy (fun _ -> true) sample investigation

    static member toString (rwa : QInvestigation) =  JsonSerializer.Serialize<QInvestigation>(rwa,JsonExtensions.options)

    static member toFile (path : string) (rwa:QInvestigation) = 
        File.WriteAllText(path,QInvestigation.toString rwa)

    static member fromString (s:string) = 
        JsonSerializer.Deserialize<QInvestigation>(s,JsonExtensions.options)

    static member fromFile (path : string) = 
        File.ReadAllText path 
        |> QInvestigation.fromString