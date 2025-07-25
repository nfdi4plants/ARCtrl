namespace ARCtrl

open ARCtrl.Helper
open Fable.Core

[<AttachMembers>]
// tanInfo given from outside was meant to allow for performance optimization, but it is not used in the code currently.
type OntologyAnnotation(?name,?tsr,?tan, (*?tanInfo, *)?comments) =
    let mutable _name : string option =
        match name with
        | Some "" | None -> None
        | name -> name
    let mutable _termSourceREF : string option =
        match tsr with
        | Some "" | None -> None
        | tsr -> tsr
    let mutable _termAccessionNumber : string option =
        match tan with
        | Some "" | Some ":" | None -> None
        | tan -> tan
    let mutable _comments : ResizeArray<Comment> = defaultArg comments <| ResizeArray()

    let mutable _tanInfo : {| IDSpace : string; LocalID : string |} option =
        //match tanInfo with
        //| Some tanInfo -> tanInfo
        //| None ->
        OntologyAnnotation.computeTanInfo (?tan = _termAccessionNumber, ?tsr = _termSourceREF)

    let recomputeTanInfo() =
        let newTanInfo = OntologyAnnotation.computeTanInfo(?tan = _termAccessionNumber, ?tsr = _termSourceREF)
        _tanInfo <- newTanInfo

    member this.Name
        with get() = _name
        and set(name) = _name <- name

    member this.TermSourceREF
        with get() = _termSourceREF
        and set(tsr) =
            _termSourceREF <- tsr
            recomputeTanInfo()

    member this.TermAccessionNumber
        with get() = _termAccessionNumber
        and set(tan) =
            _termAccessionNumber <- tan
            recomputeTanInfo()

    member this.Comments
        with get() = _comments
        and set(comments) = _comments <- comments

    static member make name tsr tan comments = OntologyAnnotation(?name=name, ?tsr=tsr, ?tan=tan, comments=comments)
    
    ///<summary>
    /// Create a ISAJson Ontology Annotation value from ISATab string entries, will try to reduce `termAccessionNumber` with regex matching.
    ///
    /// Exmp. 1: http://purl.obolibrary.org/obo/GO_000001 --> GO:000001
    ///</summary>
    ///<param name="name"></param>
    ///<param name="tsr">Term accession number</param>
    ///<param name="tan">Term accession number</param>
    ///<param name="comments">Term accession number</param>
    static member create(?name,?tsr,?tan, ?comments) : OntologyAnnotation =
        OntologyAnnotation.make name tsr tan (defaultArg comments <| ResizeArray())

    member this.TANInfo = _tanInfo

    member this.NameText = 
        this.Name
        |> Option.defaultValue ""

    /// Create a path in form of `http://purl.obolibrary.org/obo/MS_1000121` from it's Term Accession Source `MS` and Local Term Accession Number `1000121`. 
    static member createUriAnnotation (termSourceRef : string) (localTAN : string) =
        Helper.Url.createOAUri termSourceRef localTAN

    static member fromTermAnnotation (tan : string, ?name) =
        match Regex.tryParseTermAnnotation tan with
        | Some r ->
            let accession = r.IDSpace + ":" + r.LocalID
            OntologyAnnotation.create (?name = name, tsr = r.IDSpace, tan = accession)
        | None -> OntologyAnnotation.create (?name = name, tan = tan)

    /// Parses any value in `TermAccessionString` to term accession format "termsourceref:localtan". Exmp.: "MS:000001".
    ///
    /// If `TermAccessionString` cannot be parsed to this format, returns empty string!
    member this.TermAccessionShort = 
        match this.TANInfo with
        | Some id -> $"{id.IDSpace}:{id.LocalID}"
        | _ -> ""

    member this.TermAccessionOntobeeUrl = 
        match this.TANInfo with
        | Some id -> OntologyAnnotation.createUriAnnotation id.IDSpace id.LocalID
        | _ -> ""

    member this.TermAccessionAndOntobeeUrlIfShort = 
        match this.TermAccessionNumber with
        | Some tan -> 
            match tan with 
            | Regex.ActivePatterns.Regex Regex.Pattern.TermAnnotationShortPattern _ -> this.TermAccessionOntobeeUrl
            | _ -> tan
        | _ -> ""

    /// <summary>
    /// Get a ISATab string entries from an ISAJson Ontology Annotation object (name,source,accession)
    ///
    /// `asOntobeePurlUrl`: option to return term accession in Ontobee purl-url format (`http://purl.obolibrary.org/obo/MS_1000121`)
    /// </summary>
    static member toStringObject (oa : OntologyAnnotation, ?asOntobeePurlUrlIfShort: bool) =
        let asOntobeePurlUrlIfShort = Option.defaultValue false asOntobeePurlUrlIfShort
        {|
            TermName = oa.Name |> Option.defaultValue ""
            TermSourceREF = oa.TermSourceREF |> Option.defaultValue ""
            TermAccessionNumber = 
                if asOntobeePurlUrlIfShort then
                    let url = oa.TermAccessionAndOntobeeUrlIfShort
                    if url = "" then 
                        oa.TermAccessionNumber |> Option.defaultValue ""
                    else
                        url
                else
                    oa.TermAccessionNumber |> Option.defaultValue ""
        |}

    interface IISAPrintable with
        member this.Print() =
            this.ToString()
        member this.PrintCompact() =
            "OA " + this.NameText

    override this.ToString() =
        let sb = System.Text.StringBuilder()
        sb.Append("{") |> ignore
        [
            if this.Name.IsSome then
                sprintf "Name = %A" this.Name.Value
            if this.TermSourceREF.IsSome then
                sprintf "TSR = %A" this.TermSourceREF.Value
            if this.TermAccessionNumber.IsSome then
                sprintf "TAN = %A" this.TermAccessionNumber.Value
            if this.Comments.Count <> 0 then
                sprintf "Comments = %A" this.Comments
        ] 
        |> String.concat "; "
        |> sb.Append
        |> ignore
        sb.Append("}") |> ignore
        sb.ToString()

    member this.isEmpty() = 
           this.Name.IsNone && this.TermSourceREF.IsNone && this.TermAccessionNumber.IsNone && this.Comments.Count = 0

    override this.GetHashCode() = 
        // Same as above but don't create unneccessary arrays
        let nameHash = 
            match this.Name with
            | Some name -> hash name
            | None -> 0
        match this.TermSourceREF, this.TANInfo with
        | None, Some taninfo -> 
            HashCodes.mergeHashes (hash taninfo.IDSpace) (hash taninfo.LocalID)
        | Some tsr, Some taninfo -> 
            HashCodes.mergeHashes (hash tsr) (hash taninfo.LocalID)
        | Some tsr, None ->
            match this.TermAccessionNumber with
            | Some tan -> HashCodes.mergeHashes (hash tsr) (hash tan)
            | None -> hash tsr
        | None, None ->
            0    
        |> HashCodes.mergeHashes nameHash

    override this.Equals(obj) : bool =
        HashCodes.hash this = HashCodes.hash obj
   
    interface System.IComparable with
        member this.CompareTo(obj) =
            match obj with
            | :? OntologyAnnotation as oa -> 
                #if FABLE_COMPILER
                let hash = this.GetHashCode()
                let otherHash = oa.GetHashCode()
                if hash = otherHash then 0
                else if hash < otherHash then -1
                else 1                
                #else
                this.GetHashCode().CompareTo(oa.GetHashCode())
                #endif
            | _ -> 1

    member this.Copy() =
        let nextComments = this.Comments |> ResizeArray.map (fun c -> c.Copy())
        OntologyAnnotation.make this.Name this.TermSourceREF this.TermAccessionNumber nextComments

    static member computeTanInfo (?tan : string, ?tsr : string) =
        match tan with
        | Some tan -> 
            match Regex.tryParseTermAnnotation tan with
            | Some ta -> Some ta
            | None ->
                match tsr with
                | Some "" | None -> None
                | Some tsr -> 
                    Some {| IDSpace = tsr; LocalID = tan |}
        | None -> None
