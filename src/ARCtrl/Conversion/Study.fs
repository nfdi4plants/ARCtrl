namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.FileSystem
open System.Collections.Generic
//open ColumnIndex

open ColumnIndex
open ARCtrl.Helper.Regex.ActivePatterns


type StudyConversion = 

    static member composeStudy (study : ArcStudy, ?fs : FileSystem) =
        let dateCreated = study.SubmissionDate |> Option.bind DateTime.tryFromString
        let datePublished = study.PublicReleaseDate |> Option.bind DateTime.tryFromString
        let dateModified = System.DateTime.Now
        let publications = 
            study.Publications
            |> ResizeArray.map (fun p -> ScholarlyArticleConversion.composeScholarlyArticle p)
            |> Option.fromSeq
        let creators =
            study.Contacts
            |> ResizeArray.map (fun c -> PersonConversion.composePerson c)
            |> Option.fromSeq
        let processSequence = 
            ArcTables(study.Tables).GetProcesses(studyName = study.Identifier, ?fs = fs)
            |> ResizeArray
            |> Option.fromSeq
        let fragmentDescriptors =
            study.Datamap
            |> Option.map DatamapConversion.composeFragmentDescriptors
        let dataFiles = 
            processSequence
            |> Option.map (fun ps -> AssayConversion.getDataFilesFromProcesses(ps, ?fragmentDescriptors = fragmentDescriptors))
        let comments = 
            study.Comments
            |> ResizeArray.map (fun c -> BaseTypes.composeComment c)
            |> Option.fromSeq
        LDDataset.createStudy(
            identifier = study.Identifier,
            ?name = study.Title,
            ?description = study.Description,
            ?dateCreated = dateCreated,
            ?datePublished = datePublished,
            dateModified = dateModified,
            ?creators = creators,
            ?citations = publications,
            ?hasParts = dataFiles,
            ?variableMeasureds = fragmentDescriptors,
            ?abouts = processSequence,
            ?comments = comments
        )

    static member decomposeStudy (study : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let dateCreated = 
            LDDataset.tryGetDateCreatedAsDateTime(study, ?context = context)
            |> Option.map DateTime.toString
        let datePublished = 
            LDDataset.tryGetDatePublishedAsDateTime(study, ?context = context)
            |> Option.map DateTime.toString
        let publications = 
            LDDataset.getCitations(study, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun p -> ScholarlyArticleConversion.decomposeScholarlyArticle(p, ?graph = graph, ?context = context))
        let creators = 
            LDDataset.getCreators(study, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> PersonConversion.decomposePerson(c, ?graph = graph, ?context = context))
        let dataMap = 
            LDDataset.getVariableMeasuredAsFragmentDescriptors(study, ?graph = graph, ?context = context)
            |> fun fds -> DatamapConversion.decomposeFragmentDescriptors(fds, ?graph = graph, ?context = context)
            |> Option.fromValueWithDefault (Datamap.init())
        let tables = 
            LDDataset.getAboutsAsLabProcess(study, ?graph = graph, ?context = context)
            |> fun ps -> ArcTables.fromProcesses(List.ofSeq ps, ?graph = graph, ?context = context)
        let comments =
            LDDataset.getComments(study, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> BaseTypes.decomposeComment(c, ?context = context))
        ArcStudy.create(
            identifier = LDDataset.getIdentifierAsString(study, ?context = context),
            ?title = LDDataset.tryGetNameAsString(study, ?context = context),
            ?description = LDDataset.tryGetDescriptionAsString(study, ?context = context),
            ?submissionDate = dateCreated,
            ?publicReleaseDate = datePublished,
            ?datamap = dataMap,
            contacts = creators,
            publications = publications,
            tables = tables.Tables,
            comments = comments
        )
