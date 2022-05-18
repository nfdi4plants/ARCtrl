namespace ISADotNet.QueryModel

open ISADotNet
open System.Text.Json.Serialization
open System.Text.Json
open System.IO

open System.Collections.Generic
open System.Collections


type QStudy(FileName : string option,Identifier : string option,Title : string option,Description : string option,SubmissionDate : string option,PublicReleaseDate : string option,Publications : Publication list option,Contacts : Person list option,StudyDesignDescriptors : OntologyAnnotation list option, Assays : QAssay list, Sheets : QSheet list) =

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
    member this.Assays = Assays

    static member fromStudy (study : Study) =
        let assays = study.Assays |> Option.map (List.map QAssay.fromAssay) |> Option.defaultValue []
        let sheets = 
            study.ProcessSequence |> Option.defaultValue []
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
            |> List.append (assays |> List.collect (fun a -> a.Protocols))
        QStudy(study.FileName,study.Identifier,study.Title,study.Description,study.SubmissionDate,study.PublicReleaseDate,study.Publications,study.Contacts,study.StudyDesignDescriptors,assays,sheets)

    member this.Protocol (sheetName : string) =
        base.Protocol(sheetName, $"Assay \"{this.FileName}\"")

    member this.Protocol (index : int) =
        base.Protocol(index, $"Assay \"{this.FileName}\"")
       
    //interface IEnumerable<QSheet> with
    //    member this.GetEnumerator() = (Seq.ofList this.Sheets).GetEnumerator()

    //interface IEnumerable with
    //    member this.GetEnumerator() = (this :> IEnumerable<QSheet>).GetEnumerator() :> IEnumerator

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
