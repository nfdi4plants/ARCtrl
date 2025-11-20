namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.FileSystem
open System.Collections.Generic
//open ColumnIndex

open ColumnIndex
open ARCtrl.Helper.Regex.ActivePatterns

type InvestigationConversion =

    static member composeInvestigation (investigation : ArcInvestigation, ?fs : FileSystem) =
        let name = match investigation.Title with | Some t -> t | None -> failwith "Investigation must have a title"
        let dateCreated = investigation.SubmissionDate |> Option.bind DateTime.tryFromString
        let datePublished =
            investigation.PublicReleaseDate
            |> Option.bind DateTime.tryFromString
            |> Option.defaultValue (System.DateTime.Now)
        //let dateModified = System.DateTime.Now
        let publications = 
            investigation.Publications
            |> ResizeArray.map (fun p -> ScholarlyArticleConversion.composeScholarlyArticle p)
            |> Option.fromSeq
        let creators =
            investigation.Contacts
            |> ResizeArray.map (fun c -> PersonConversion.composePerson c)
            |> Option.fromSeq
        let comments = 
            investigation.Comments
            |> ResizeArray.map (fun c -> BaseTypes.composeComment c)
            |> Option.fromSeq
        let hasParts =
            [
                yield! (investigation.Assays |> ResizeArray.map (fun a -> AssayConversion.composeAssay(a, ?fs = fs)))
                yield! (investigation.Studies |> ResizeArray.map (fun s -> StudyConversion.composeStudy(s, ?fs = fs)))
                yield! (investigation.Workflows |> ResizeArray.map (fun w -> WorkflowConversion.composeWorkflow(w, ?fs = fs)))
                yield! (investigation.Runs |> ResizeArray.map (fun r -> RunConversion.composeRun(r, ?fs = fs)))
            ]
            |> ResizeArray
            |> Option.fromSeq
        let mentions =
            ResizeArray [] // TODO
            |> Option.fromSeq
        LDDataset.createInvestigation(
            identifier = investigation.Identifier,
            name = name,
            ?description = investigation.Description,
            ?dateCreated = dateCreated,
            datePublished = datePublished,
            //dateModified = dateModified,
            ?creators = creators,
            ?citations = publications,
            ?hasParts = hasParts,
            ?mentions = mentions,
            ?comments = comments
        )

    static member decomposeInvestigation (investigation : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let title =
            match LDDataset.tryGetNameAsString(investigation, ?context = context) with
            | Some t -> Some t
            | None -> LDDataset.tryGetHeadlineAsString(investigation, ?context = context)
        let dateCreated = 
            LDDataset.tryGetDateCreatedAsDateTime(investigation, ?context = context)
            |> Option.map DateTime.toString
        let datePublished = 
            LDDataset.tryGetDatePublishedAsDateTime(investigation, ?context = context)
            |> Option.map DateTime.toString
        let publications = 
            LDDataset.getCitations(investigation, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun p -> ScholarlyArticleConversion.decomposeScholarlyArticle(p, ?graph = graph, ?context = context))
        let creators = 
            LDDataset.getCreators(investigation, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> PersonConversion.decomposePerson(c, ?graph = graph, ?context = context))
        let datasets = 
            LDDataset.getHasPartsAsDataset  (investigation, ?graph = graph, ?context = context)
        let studies = 
            datasets
            |> ResizeArray.filter (fun d -> LDDataset.validateStudy(d, ?context = context))
            |> ResizeArray.map (fun d -> StudyConversion.decomposeStudy(d, ?graph = graph, ?context = context))
        let assays = 
            datasets
            |> ResizeArray.filter (fun d -> LDDataset.validateAssay(d, ?context = context))
            |> ResizeArray.map (fun d -> AssayConversion.decomposeAssay(d, ?graph = graph, ?context = context))
        let workflows = 
            datasets
            |> ResizeArray.filter (fun d -> LDDataset.validateARCWorkflow(d, ?graph = graph, ?context = context))
            |> ResizeArray.map (fun d -> WorkflowConversion.decomposeWorkflow(d, ?graph = graph, ?context = context))
        let runs =
            datasets
            |> ResizeArray.filter (fun d -> LDDataset.validateARCRun(d, ?context = context))
            |> ResizeArray.map (fun d -> RunConversion.decomposeRun(d, ?graph = graph, ?context = context))
        let comments =
            LDDataset.getComments(investigation, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> BaseTypes.decomposeComment(c, ?context = context))
        ArcInvestigation.create(
            identifier = LDDataset.getIdentifierAsString(investigation, ?context = context),
            ?title = title,
            ?description = LDDataset.tryGetDescriptionAsString(investigation, ?context = context),
            ?submissionDate = dateCreated,
            ?publicReleaseDate = datePublished,
            contacts = creators,
            publications = publications,
            studies = studies,
            assays = assays,
            workflows = workflows,
            runs = runs,
            comments = comments
        )