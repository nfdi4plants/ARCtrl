namespace ARCtrl.Conversion

open ARCtrl.ROCrate
open ARCtrl
open ARCtrl.Helper
open ARCtrl.FileSystem
open System.Collections.Generic
//open ColumnIndex

open ColumnIndex
open ARCtrl.Helper.Regex.ActivePatterns



type ScholarlyArticleConversion =

    static member composeAuthor (author : string) : LDNode =
        try 
            ARCtrl.Json.Decode.fromJsonString Json.LDNode.decoder author
        with
        | _ -> LDPerson.create(givenName = author)

    static member splitAuthors (a : string) =
        let mutable bracketCount = 0
        let authors = ResizeArray<string>()
        let sb = System.Text.StringBuilder()
        for c in a do
            if c = '{' then 
                bracketCount <- bracketCount + 1
                sb.Append(c) |> ignore
            elif c = '}' then 
                bracketCount <- bracketCount - 1
                sb.Append(c) |> ignore
            elif c = ',' && bracketCount = 0 then
                authors.Add(sb.ToString())
                sb.Clear() |> ignore
            else 
                sb.Append(c) |> ignore
        authors.Add(sb.ToString())
        authors

    static member composeAuthors (authors : string) : ResizeArray<LDNode> =
        ScholarlyArticleConversion.splitAuthors authors
        |> Seq.map ScholarlyArticleConversion.composeAuthor
        |> ResizeArray

    static member decomposeAuthor (author : LDNode, ?context : LDContext) : string =
        let hasOnlyGivenName = 
            author.GetPropertyNames(?context = context)
            |> Seq.filter(fun n -> n <> LDPerson.givenName)
            |> Seq.isEmpty
        if hasOnlyGivenName then
            LDPerson.getGivenNameAsString(author, ?context = context)
        else
            Json.LDNode.encoder author
            |> ARCtrl.Json.Encode.toJsonString 0

    static member decomposeAuthors (authors : ResizeArray<LDNode>, ?context : LDContext) : string =
        authors
        |> ResizeArray.map (fun a -> ScholarlyArticleConversion.decomposeAuthor (a,?context = context))
        |> String.concat ","

    static member composeScholarlyArticle (publication : Publication) =
        let title = match publication.Title with | Some t -> t | None -> failwith "Publication must have a title"
        let authors = 
            publication.Authors
            |> Option.map ScholarlyArticleConversion.composeAuthors
        let comments = 
            publication.Comments
            |> ResizeArray.map (BaseTypes.composeComment)
            |> Option.fromSeq
        let identifiers = ResizeArray [
            if publication.DOI.IsSome && publication.DOI.Value <> "" then
                LDPropertyValue.createDOI publication.DOI.Value
            if publication.PubMedID.IsSome && publication.PubMedID.Value <> "" then
                LDPropertyValue.createPubMedID publication.PubMedID.Value
        ]
        let status = publication.Status |> Option.map BaseTypes.composeDefinedTerm
        let scholarlyArticle = 
            LDScholarlyArticle.create(
                headline = title,
                identifiers = identifiers,
                ?authors = authors,
                //?url = publication.DOI,
                ?creativeWorkStatus = status,
                ?comments = comments            
            )
        scholarlyArticle

    static member decomposeScholarlyArticle (sa : LDNode, ?graph : LDGraph, ?context : LDContext) =
        let title = LDScholarlyArticle.getHeadlineAsString(sa, ?context = context)
        let authors = 
            LDScholarlyArticle.getAuthors(sa, ?graph = graph, ?context = context)
            |> Option.fromSeq
            |> Option.map (fun a -> ScholarlyArticleConversion.decomposeAuthors(a, ?context = context))
        let comments = 
            LDScholarlyArticle.getComments(sa, ?graph = graph, ?context = context)
            |> ResizeArray.map (fun c -> BaseTypes.decomposeComment(c, ?context = context))
        let status = 
            LDScholarlyArticle.tryGetCreativeWorkStatus(sa, ?graph = graph, ?context = context)
            |> Option.map (fun s -> BaseTypes.decomposeDefinedTerm(s, ?context = context))
        let identifiers = LDScholarlyArticle.getIdentifiersAsPropertyValue(sa, ?graph = graph, ?context = context)
        let doi =
            identifiers
            |> ResizeArray.tryPick (fun i -> LDPropertyValue.tryGetAsDOI(i, ?context = context))
            |> function
               | Some p -> Some p
               | None -> LDScholarlyArticle.tryGetSameAsAsString(sa, ?context = context)
        let pubMedID =
            identifiers
            |> ResizeArray.tryPick (fun i -> LDPropertyValue.tryGetAsPubMedID(i, ?context = context))
            |> function
               | Some p -> Some p
               | None -> LDScholarlyArticle.tryGetUrlAsString(sa, ?context = context)
        ARCtrl.Publication.create(
            title = title,
            ?authors = authors,
            ?status = status,
            comments = comments,
            ?doi = doi,
            ?pubMedID = pubMedID
        )
