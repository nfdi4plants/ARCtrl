namespace ISADotNet.InvestigationFile

open System.Collections.Generic
open FSharpSpreadsheetML
open DocumentFormat.OpenXml.Spreadsheet
open ISADotNet
open System.Text.RegularExpressions

type InvestigationInfo =
    {
    Identifier : string
    Title : string
    Description : string
    SubmissionDate : string
    PublicReleaseDate : string
    Comments : Comment list
    }


    static member create identifier title description submissionDate publicReleaseDate comments =
        {
        Identifier = identifier
        Title = title
        Description = description
        SubmissionDate = submissionDate
        PublicReleaseDate = publicReleaseDate
        Comments = comments
        }

    static member IdentifierTab = "Investigation Identifier"
    static member TitleTab = "Investigation Title"
    static member DescriptionTab = "Investigation Description"
    static member SubmissionDateTab = "Investigation Submission Date"
    static member PublicReleaseDateTab = "Investigation Public Release Date"

type StudyInfo =
    {
    Identifier : string
    Title : string
    Description : string
    SubmissionDate : string
    PublicReleaseDate : string
    FileName : string
    Comments : Comment list
    }


    static member create identifier title description submissionDate publicReleaseDate fileName comments =
        {
        Identifier = identifier
        Title = title
        Description = description
        SubmissionDate = submissionDate
        PublicReleaseDate = publicReleaseDate
        FileName = fileName
        Comments = comments
        }

    static member IdentifierTab = "Study Identifier"
    static member TitleTab = "Study Title"
    static member DescriptionTab = "Study Description"
    static member SubmissionDateTab = "Study Submission Date"
    static member PublicReleaseDateTab = "Study Public Release Date"
    static member FileNameTab = "Study File Name"

type InvestigationFile = 
    {
    Investigation : Investigation
    Remarks : (int*string) list
    }

    static member create investigation remarks = 
        {
            Investigation = investigation
            Remarks = remarks
        
        }

module Array = 
    
    let ofIndexedSeq (s : seq<int*string>) = 
        Array.init 
            (Seq.maxBy fst s |> fst |> (+) 1)
            (fun i -> 
                match Seq.tryFind (fst >> (=) i) s with
                | Some (i,x) -> x
                | None -> ""               
            )

    /// If at least i values exist in array a, builds a new array that contains the elements of the given array, exluding the first i elements
    let trySkip i a =
        try
            Array.skip i a
            |> Some
        with
        | _ -> None

    /// Returns Item of array at index i if existing, else returns default value
    let tryItemDefault i d a = 
        match Array.tryItem i a with
        | Some v -> v
        | None -> d

module List =

    let tryPickDefault (chooser : 'T -> 'U Option) (d : 'U) (list : 'T list) =
        match List.tryPick chooser list with
        | Some v -> v
        | None -> d

    let unzip4 (l : ('A*'B*'C*'D) list ) =
        let rec loop la lb lc ld l =
            match l with
            | (a,b,c,d) :: l -> loop (a::la) (b::lb) (c::lc) (d::ld) l
            | [] -> List.rev la, List.rev lb, List.rev lc, List.rev ld
        loop [] [] [] [] l

module IO =     

    let commentRegex = Regex(@"(?<=Comment\[<).*(?=>\])")

    let commentRegexNoAngleBrackets = Regex(@"(?<=Comment\[).*(?=\])")

    let remarkRegex = Regex(@"(?<=#).*")

    let (|Comment|_|) (key : Option<string>) =
        key
        |> Option.bind (fun k ->
            let r = commentRegex.Match(k)
            if r.Success then Some r.Value
            else 
                let r = commentRegexNoAngleBrackets.Match(k)
                if r.Success then Some r.Value
                else None
        )
    
    let (|Remark|_|) (key : Option<string>) =
        key
        |> Option.bind (fun k ->
            let r = remarkRegex.Match(k)
            if r.Success then Some r.Value
            else None
        )

    let splitAndCreateOntologies (terms:string) (accessions:string) (source:string) =
        let separator = ';'
        (terms.Split separator, accessions.Split separator, source.Split separator)
        |||> Array.map3 (fun t a s -> 
            OntologyAnnotation.create t a s []
            |> REF.Item
        )
        |> List.ofArray

    let splitAndCreateParameters (terms:string) (accessions:string) (source:string) =
        let separator = ';'
        (terms.Split separator, accessions.Split separator, source.Split separator)
        |||> Array.map3 (fun t a s -> 
            OntologyAnnotation.create t a s []
            |> REF.Item
            |> ProtocolParameter.create ""
            |> REF.Item
        )
        |> List.ofArray

    let splitAndCreateComponents (names:string) (terms:string) (accessions:string) (source:string) =
        let separator = ';'
        (terms.Split separator, accessions.Split separator, source.Split separator)
        |||> Array.map3 (fun t a s -> 
            OntologyAnnotation.create t a s []
            |> REF.Item
        )
        |> Array.map2 (fun n oa -> Component.create n oa |> REF.Item) (names.Split separator)
        |> List.ofArray
        
    let dismantleOntology (ontology : REF<OntologyAnnotation>) =
        match ontology with
        | Item oa -> oa.Name, oa.TermAccessionNumber, oa.TermSourceREF
        | ID s -> s, "" , ""

    let mergeOntologies (ontologies : REF<OntologyAnnotation> list) =
        let separator = ';'
        ontologies
        |> List.map (fun oa -> 
            match oa with
            | Item oa ->    oa.Name,oa.TermAccessionNumber,oa.TermSourceREF
            | ID s ->       s,"",""
        ) 
        |> List.reduce (fun (terms, accessions, sources) (term, accession, source) -> 
            sprintf "%s%c%s" terms      separator term,
            sprintf "%s%c%s" accessions separator accession,
            sprintf "%s%c%s" sources    separator source
        ) 

    let mergeParameters (parameters : REF<ProtocolParameter> list) =
        parameters
        |> List.map (fun pp -> 
            match pp with
            | Item pp -> pp.ParameterName 
            | ID s -> ID s)
        |> mergeOntologies

    let mergeComponents (components : REF<Component> list) =
        let separator = ';'
        components
        |> List.map (fun c -> 
            match c with
            | Item c -> c.ComponentName,c.ComponentType
            | ID s -> s,ID s)
        |> List.map (fun (n,oa) -> 
            match oa with
            | Item oa ->    n,oa.Name,oa.TermAccessionNumber,oa.TermSourceREF
            | ID s ->       n,s,"",""
        ) 
        |> List.reduce (fun (names,terms, accessions, sources) (name,term, accession, source) -> 
            sprintf "%s%c%s" names      separator name,
            sprintf "%s%c%s" terms      separator term,
            sprintf "%s%c%s" accessions separator accession,
            sprintf "%s%c%s" sources    separator source
        ) 


    let wrapCommentKey k = 
        sprintf "Comment[<%s>]" k

    let wrapRemark r = 
        sprintf "#%s" r

    let readInvestigationInfo lineNumber (en:IEnumerator<Row>) =
        let rec loop identifier title description submissionDate publicReleaseDate comments remarks lineNumber = 

            if en.MoveNext() then    

                let row = en.Current

                match Row.tryGetValueAt None 1u row, Row.tryGetValueAt None 2u row with

                | Comment k, Some v -> 
                    loop identifier title description submissionDate publicReleaseDate ((Comment.create "" k v) :: comments) remarks (lineNumber + 1)         
                | Comment k, None -> 
                    loop identifier title description submissionDate publicReleaseDate ((Comment.create "" k "") :: comments) remarks (lineNumber + 1)         

                | Remark k, _  -> 
                    loop identifier title description submissionDate publicReleaseDate comments ((lineNumber,k) :: remarks) (lineNumber + 1)

                | Some k, Some identifier when k = InvestigationInfo.IdentifierTab->              
                    loop identifier title description submissionDate publicReleaseDate comments remarks (lineNumber + 1)

                | Some k, Some title when k = InvestigationInfo.TitleTab ->
                    loop identifier title description submissionDate publicReleaseDate comments remarks (lineNumber + 1)

                |Some k, Some description when k = InvestigationInfo.DescriptionTab ->
                    loop identifier title description submissionDate publicReleaseDate comments remarks (lineNumber + 1)

                | Some k, Some submissionDate when k = InvestigationInfo.SubmissionDateTab ->
                    loop identifier title description submissionDate publicReleaseDate comments remarks (lineNumber + 1)

                | Some k, Some publicReleaseDate when k = InvestigationInfo.PublicReleaseDateTab ->
                    loop identifier title description submissionDate publicReleaseDate comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber, remarks, InvestigationInfo.create identifier title description submissionDate publicReleaseDate (comments |> List.rev)
                | _ -> 
                    None,lineNumber, remarks, InvestigationInfo.create identifier title description submissionDate publicReleaseDate (comments |> List.rev)
            else
                None,lineNumber, remarks, InvestigationInfo.create identifier title description submissionDate publicReleaseDate (comments |> List.rev)
        loop "" "" "" "" "" [] [] lineNumber

    let readStudyInfo lineNumber (en:IEnumerator<Row>) =
        let rec loop identifier title description submissionDate publicReleaseDate fileName comments remarks lineNumber = 

            if en.MoveNext() then        
                let row = en.Current
                match Row.tryGetValueAt None 1u row, Row.tryGetValueAt None 2u row with

                | Comment k, Some v -> 
                    loop identifier title description submissionDate publicReleaseDate fileName ((Comment.create "" k v) :: comments) remarks (lineNumber + 1)         
                | Comment k, None -> 
                    loop identifier title description submissionDate publicReleaseDate fileName ((Comment.create "" k "") :: comments) remarks (lineNumber + 1)         

                | Remark k, _  -> 
                    loop identifier title description submissionDate publicReleaseDate fileName comments ((lineNumber,k) :: remarks) (lineNumber + 1)

                | Some k, Some identifier when k = StudyInfo.IdentifierTab->              
                    loop identifier  title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                | Some k, Some title when k = StudyInfo.TitleTab ->
                    loop identifier title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                |Some k, Some description when k = StudyInfo.DescriptionTab ->
                    loop identifier title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                | Some k, Some submissionDate when k = StudyInfo.SubmissionDateTab ->
                    loop identifier title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                | Some k, Some publicReleaseDate when k = StudyInfo.PublicReleaseDateTab ->
                    loop identifier title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                | Some k, Some fileName when k = StudyInfo.FileNameTab ->
                    loop identifier title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber, remarks, StudyInfo.create identifier title description submissionDate publicReleaseDate fileName (comments |> List.rev)
                | _ -> 
                    None,lineNumber, remarks, StudyInfo.create identifier title description submissionDate publicReleaseDate fileName (comments |> List.rev)
            else
                None,lineNumber, remarks, StudyInfo.create identifier title description submissionDate publicReleaseDate fileName (comments |> List.rev)
        loop "" "" "" "" "" "" [] [] lineNumber

    let readTermSources lineNumber (en:IEnumerator<Row>) =
        let rec loop 
            names files versions descriptions
            comments remarks lineNumber = 

            let create () = 
                let length =
                    [|names;files;versions;descriptions|]
                    |> Array.map Array.length
                    |> Array.max

                List.init length (fun i ->
                    let comments = 
                        List.map (fun (key,values) -> 
                            Comment.create "" key (Array.tryItemDefault i "" values)
                        ) comments
                    OntologySourceReference.create
                        (Array.tryItemDefault i "" descriptions)
                        (Array.tryItemDefault i "" files)
                        (Array.tryItemDefault i "" names)
                        (Array.tryItemDefault i "" versions)
                        comments
                    |> REF.Item
                )
                
            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v) |> Array.ofIndexedSeq

                match Array.tryItem 0 row , Array.trySkip 1 row with

                | Comment k, Some v -> 
                    loop 
                        names files versions descriptions
                        ((k,v) :: comments) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop 
                        names files versions descriptions
                        comments ((lineNumber,k) :: remarks) (lineNumber + 1)

                | Some k, Some names when k = OntologySourceReference.NameTab -> 
                    loop 
                        names files versions descriptions
                        comments remarks (lineNumber + 1)

                | Some k, Some files when k = OntologySourceReference.FileTab -> 
                    loop 
                        names files versions descriptions
                        comments remarks (lineNumber + 1)

                | Some k, Some versions when k = OntologySourceReference.VersionTab -> 
                    loop 
                        names files versions descriptions
                        comments remarks (lineNumber + 1)

                | Some k, Some descriptions when k = OntologySourceReference.DescriptionTab -> 
                    loop 
                        names files versions descriptions
                        comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,create ()
                | _ -> None, lineNumber,remarks,create ()
            else
                None,lineNumber,remarks,create ()
        loop [||] [||] [||] [||] [] [] lineNumber



    let readPublications (prefix : string) lineNumber (en:IEnumerator<Row>) =
        let rec loop 
            pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
            comments remarks lineNumber = 

            let create () = 
                let length =
                    [|pubMedIDs;dois;authorLists;titles;statuss;statusTermAccessionNumbers;statusTermSourceREFs|]
                    |> Array.map Array.length
                    |> Array.max

                List.init length (fun i ->
                    let status = 
                        OntologyAnnotation.create 
                            (Array.tryItemDefault i "" statuss)
                            (Array.tryItemDefault i "" statusTermAccessionNumbers)
                            (Array.tryItemDefault i "" statusTermSourceREFs)
                            []
                        |> REF.Item
                    let comments = 
                        List.map (fun (key,values) -> 
                            Comment.create "" key (Array.tryItemDefault i "" values)
                        ) comments
                    Publication.create
                        (Array.tryItemDefault i "" pubMedIDs)
                        (Array.tryItemDefault i "" dois)
                        (Array.tryItemDefault i "" authorLists)
                        (Array.tryItemDefault i "" titles)
                        status
                        comments
                    |> REF.Item
                )
            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v) |> Array.ofIndexedSeq

                match Array.tryItem 0 row , Array.trySkip 1 row with

                | Comment k, Some v -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        ((k,v) :: comments) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments ((lineNumber,k) :: remarks) (lineNumber + 1)

                | Some k, Some pubMedIDs when k = prefix + " " + Publication.PubMedIDTab -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)
                | Some k, Some pubMedIDs when k = prefix.Replace(" Publication","") + " " + Publication.PubMedIDTab ->               
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some dois when k = prefix + " " + Publication.DOITab -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some authorLists when k = prefix + " " + Publication.AuthorListTab -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some titles when k = prefix + " " + Publication.TitleTab -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some statuss when k = prefix + " " + Publication.StatusTab -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some statusTermAccessionNumbers when k = prefix + " " + Publication.StatusTermAccessionNumberTab -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some statusTermSourceREFs when k = prefix + " " + Publication.StatusTermSourceREFTab -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,create ()
                | _ -> None, lineNumber,remarks,create ()
            else
                None,lineNumber,remarks,create ()
        loop [||] [||] [||] [||] [||] [||] [||] [] [] lineNumber
        



    let readPersons (prefix : string) lineNumber (en:IEnumerator<Row>) =
        let rec loop 
            lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
            comments remarks lineNumber = 

            let create () = 
                let length =
                    [|lastNames;firstNames;midInitialss;emails;phones;faxs;addresss;affiliations;roless;rolesTermAccessionNumbers;rolesTermSourceREFs|]
                    |> Array.map Array.length
                    |> Array.max

                List.init length (fun i ->
                    let roles = 
                        splitAndCreateOntologies
                            (Array.tryItemDefault i "" roless)
                            (Array.tryItemDefault i "" rolesTermAccessionNumbers)
                            (Array.tryItemDefault i "" rolesTermSourceREFs)
                    let comments = 
                        List.map (fun (key,values) -> 
                            Comment.create "" key (Array.tryItemDefault i "" values)
                        ) comments
                    Person.create
                        ""
                        (Array.tryItemDefault i "" lastNames)
                        (Array.tryItemDefault i "" firstNames)
                        (Array.tryItemDefault i "" midInitialss)
                        (Array.tryItemDefault i "" emails)
                        (Array.tryItemDefault i "" phones)
                        (Array.tryItemDefault i "" faxs)
                        (Array.tryItemDefault i "" addresss)
                        (Array.tryItemDefault i "" affiliations)
                        roles
                        comments
                    |> REF.Item
                )
            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v) |> Array.ofIndexedSeq
                match Array.tryItem 0 row , Array.trySkip 1 row with

                | Comment k, Some v -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        ((k,v) :: comments) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments ((lineNumber,k) :: remarks) (lineNumber + 1)

                | Some k, Some lastNames when k = prefix + " " + Person.LastNameTab -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some firstNames when k = prefix + " " + Person.FirstNameTab -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some midInitialss when k = prefix + " " + Person.MidInitialsTab -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some emails when k = prefix + " " + Person.EmailTab -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some phones when k = prefix + " " + Person.PhoneTab -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some faxs when k = prefix + " " + Person.FaxTab -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some addresss when k = prefix + " " + Person.AddressTab -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some affiliations when k = prefix + " " + Person.AffiliationTab -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some roless when k = prefix + " " + Person.RolesTab -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some rolesTermAccessionNumbers when k = prefix + " " + Person.RolesTermAccessionNumberTab -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some rolesTermSourceREFs when k = prefix + " " + Person.RolesTermSourceREFTab -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,create ()
                | _ -> None, lineNumber,remarks,create ()
            else
                None,lineNumber,remarks,create ()
        loop [||] [||] [||] [||] [||] [||] [||] [||] [||] [||] [||] [] [] lineNumber



    let readDesigns (prefix : string) lineNumber (en:IEnumerator<Row>) =
        let rec loop 
            designTypes typeTermAccessionNumbers typeTermSourceREFs
            comments remarks lineNumber = 

            let create () = 
                let length =
                    [|designTypes;typeTermAccessionNumbers;typeTermSourceREFs|]
                    |> Array.map Array.length
                    |> Array.max

                List.init length (fun i ->
                    let comments = 
                        List.map (fun (key,values) -> 
                            Comment.create "" key (Array.tryItemDefault i "" values)
                        ) comments
                    OntologyAnnotation.create
                        (Array.tryItemDefault i "" designTypes)
                        (Array.tryItemDefault i "" typeTermAccessionNumbers)
                        (Array.tryItemDefault i "" typeTermSourceREFs)
                        comments
                    |> REF.Item
                )
            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v) |> Array.ofIndexedSeq
                match Array.tryItem 0 row , Array.trySkip 1 row with

                | Comment k, Some v -> 
                    loop 
                        designTypes typeTermAccessionNumbers typeTermSourceREFs
                        ((k,v) :: comments) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop 
                        designTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments ((lineNumber,k) :: remarks) (lineNumber + 1)

                | Some k, Some designTypes when k = prefix + " " + Study.DesignTypeTab -> 
                    loop 
                        designTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some typeTermAccessionNumbers when k = prefix + " " + Study.DesignTypeTermAccessionNumberTab -> 
                    loop 
                        designTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some typeTermSourceREFs when k = prefix + " " + Study.DesignTypeTermSourceREFTab -> 
                    loop 
                        designTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,create ()
                | _ -> None, lineNumber,remarks,create ()
            else
                None,lineNumber,remarks,create ()
        loop [||] [||] [||] [] [] lineNumber



    let readFactors (prefix : string) lineNumber (en:IEnumerator<Row>) =
        let rec loop 
            names factorTypes typeTermAccessionNumbers typeTermSourceREFs
            comments remarks lineNumber = 

            let create () = 
                let length =
                    [|names;factorTypes;typeTermAccessionNumbers;typeTermSourceREFs|]
                    |> Array.map Array.length
                    |> Array.max

                List.init length (fun i ->
                    let factorType = 
                        OntologyAnnotation.create 
                            (Array.tryItemDefault i "" factorTypes)
                            (Array.tryItemDefault i "" typeTermAccessionNumbers)
                            (Array.tryItemDefault i "" typeTermSourceREFs)
                            []
                        |> REF.Item
                    let comments = 
                        List.map (fun (key,values) -> 
                            Comment.create "" key (Array.tryItemDefault i "" values)
                        ) comments
                    Factor.create
                        ""
                        (Array.tryItemDefault i "" names)
                        factorType
                        comments
                    |> REF.Item
                )
            if en.MoveNext() then  
                let row = en.Current |> Row.getIndexedValues None |> Seq.map (fun (i,v) -> int i - 1,v) |> Array.ofIndexedSeq
                match Array.tryItem 0 row , Array.trySkip 1 row with

                | Comment k, Some v -> 
                    loop 
                        names factorTypes typeTermAccessionNumbers typeTermSourceREFs
                        ((k,v) :: comments) remarks (lineNumber + 1)

                | Remark k, _  -> 
                    loop 
                        names factorTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments ((lineNumber,k) :: remarks) (lineNumber + 1)

                | Some k, Some names when k = prefix + " " + Factor.NameTab -> 
                    loop 
                        names factorTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some factorTypes when k = prefix + " " + Factor.FactorTypeTab -> 
                    loop 
                        names factorTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some typeTermAccessionNumbers when k = prefix + " " + Factor.TypeTermAccessionNumberTab -> 
                    loop 
                        names factorTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some typeTermSourceREFs when k = prefix + " " + Factor.TypeTermSourceREFTab -> 
                    loop 
                        names factorTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,create ()
                | _ -> None, lineNumber,remarks,create ()
            else
                None,lineNumber,remarks,create ()
        loop [||] [||] [||] [||] [] [] lineNumber



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
                        comments ((lineNumber,k) :: remarks) (lineNumber + 1)

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
                        comments ((lineNumber,k) :: remarks) (lineNumber + 1)

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

    let readStudy lineNumber (en:IEnumerator<Row>) = 

        let rec loop lastLine (studyInfo : StudyInfo) designDescriptors publications factors assays protocols contacts remarks lineNumber =
           
            match lastLine with

            | Some k when k = Study.DesignDescriptorsTab -> 
                let currentLine,lineNumber,newRemarks,designDescriptors = readDesigns Study.DesignDescriptorsTabPrefix (lineNumber + 1) en         
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | Some k when k = Study.PublicationsTab -> 
                let currentLine,lineNumber,newRemarks,publications = readPublications Study.PublicationsTabPrefix (lineNumber + 1) en       
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | Some k when k = Study.FactorsTab -> 
                let currentLine,lineNumber,newRemarks,factors = readFactors Study.FactorsTabPrefix (lineNumber + 1) en       
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | Some k when k = Study.AssaysTab -> 
                let currentLine,lineNumber,newRemarks,assays = readAssays Study.AssaysTabPrefix (lineNumber + 1) en       
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | Some k when k = Study.ProtocolsTab -> 
                let currentLine,lineNumber,newRemarks,protocols = readProtocols Study.ProtocolsTabPrefix (lineNumber + 1) en  
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | Some k when k = Study.ContactsTab -> 
                let currentLine,lineNumber,newRemarks,contacts = readPersons Study.ContactsTabPrefix (lineNumber + 1) en  
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | k -> 
                k,lineNumber,remarks, 
                    Study.create 
                        "" studyInfo.FileName studyInfo.Identifier studyInfo.Title studyInfo.Description studyInfo.SubmissionDate studyInfo.PublicReleaseDate 
                        publications contacts designDescriptors protocols ([],[],[]) [] assays factors [] [] studyInfo.Comments
                    |> REF.Item
    
        let currentLine,lineNumber,remarks,item = readStudyInfo lineNumber en  
        loop currentLine item [] [] [] [] [] [] remarks lineNumber


    let fromRows (rows:seq<Row>) =
        let en = rows.GetEnumerator()              
        
        let emptyInvestigationInfo = InvestigationInfo.create "" "" "" "" "" []

        let rec loop lastLine ontologySourceReferences investigationInfo publications contacts studies remarks lineNumber =
            match lastLine with

            | Some k when k = Investigation.OntologySourceReferenceTab -> 
                let currentLine,lineNumber,newRemarks,ontologySourceReferences = readTermSources (lineNumber + 1) en         
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = Investigation.InvestigationTab -> 
                let currentLine,lineNumber,newRemarks,investigationInfo = readInvestigationInfo (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = Investigation.PublicationsTab -> 
                let currentLine,lineNumber,newRemarks,publications = readPublications Investigation.PublicationsTabPrefix (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = Investigation.ContactsTab -> 
                let currentLine,lineNumber,newRemarks,contacts = readPersons Investigation.ContactsTabPrefix (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = Investigation.StudyTab -> 
                let currentLine,lineNumber,newRemarks,study = readStudy (lineNumber + 1) en  
                loop currentLine ontologySourceReferences investigationInfo publications contacts (study::studies) (List.append remarks newRemarks) lineNumber

            | k -> 
                Investigation.create 
                    "" "" investigationInfo.Identifier investigationInfo.Title investigationInfo.Description investigationInfo.SubmissionDate investigationInfo.PublicReleaseDate
                    ontologySourceReferences publications contacts (List.rev studies) investigationInfo.Comments
                |> fun i -> InvestigationFile.create i remarks

        if en.MoveNext () then
            let currentLine = en.Current |> Row.tryGetValueAt None 1u
            loop currentLine [] emptyInvestigationInfo [] [] [] [] 1
            
        else
            failwith "emptyInvestigationFile"
   
    let fromFile (path : string) =
        
        let doc = 
            Spreadsheet.fromFile path false

        try 
            doc
            |> Spreadsheet.getRowsBySheetIndex 0u
            |> fromRows 
        finally
        doc.Close()
    
    let writeTermSources (termSources : OntologySourceReference list) =
        let commentKeys = termSources |> List.collect (fun termSource -> termSource.Comments |> List.map (fun c -> c.Name))

        seq {
            yield   (Row.ofValues None 0u (OntologySourceReference.NameTab          :: (termSources |> List.map (fun termSource -> termSource.Name))))
            yield   (Row.ofValues None 0u (OntologySourceReference.FileTab          :: (termSources |> List.map (fun termSource -> termSource.File))))
            yield   (Row.ofValues None 0u (OntologySourceReference.VersionTab       :: (termSources |> List.map (fun termSource -> termSource.Version))))
            yield   (Row.ofValues None 0u (OntologySourceReference.DescriptionTab   :: (termSources |> List.map (fun termSource -> termSource.Description))))

            for key in commentKeys do
                let values = 
                    termSources |> List.map (fun termSource -> 
                        List.tryPickDefault (fun (c : Comment) -> if c.Name = key then Some c.Value else None) "" termSource.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }

    let writeInvestigationInfo (investigation : Investigation) =  
        seq {       
            yield   ( Row.ofValues None 0u [Investigation.IdentifierTab;         investigation.Identifier])
            yield   ( Row.ofValues None 0u [Investigation.TitleTab;              investigation.Title])
            yield   ( Row.ofValues None 0u [Investigation.DescriptionTab;        investigation.Description])
            yield   ( Row.ofValues None 0u [Investigation.SubmissionDateTab;     investigation.SubmissionDate])
            yield   ( Row.ofValues None 0u [Investigation.PublicReleaseDateTab;  investigation.PublicReleaseDate])
            yield!  (investigation.Comments |> List.map (fun c ->  Row.ofValues None 0u [wrapCommentKey c.Name;c.Value]))         
        }

    let writeStudyInfo (study : Study) =  

        seq {   
            yield   ( Row.ofValues None 0u [Study.IdentifierTab;         study.Identifier]          )
            yield   ( Row.ofValues None 0u [Study.TitleTab;              study.Title]               )
            yield   ( Row.ofValues None 0u [Study.DescriptionTab;        study.Description]         )
            yield   ( Row.ofValues None 0u [Study.SubmissionDateTab;     study.SubmissionDate]      )
            yield   ( Row.ofValues None 0u [Study.PublicReleaseDateTab;  study.PublicReleaseDate]   )
            yield   ( Row.ofValues None 0u [Study.FileNameTab;           study.FileName]            )
            yield!  (study.Comments |> List.map (fun c ->  Row.ofValues None 0u [wrapCommentKey c.Name;c.Value]))          
        }

    let writeDesigns prefix (designs : OntologyAnnotation list) =
        let commentKeys = designs |> List.collect (fun design -> design.Comments |> List.map (fun c -> c.Name))

        seq {
            yield   ( Row.ofValues None 0u (prefix + " " + Study.DesignTypeTab                      :: (designs |> List.map (fun design -> design.Name))))
            yield   ( Row.ofValues None 0u (prefix + " " + Study.DesignTypeTermAccessionNumberTab   :: (designs |> List.map (fun design -> design.TermAccessionNumber))))
            yield   ( Row.ofValues None 0u (prefix + " " + Study.DesignTypeTermSourceREFTab         :: (designs |> List.map (fun design -> design.TermSourceREF))))

            for key in commentKeys do
                let values = 
                    designs |> List.map (fun design -> 
                        List.tryPickDefault (fun (c : Comment) -> if c.Name = key then Some c.Value else None) "" design.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }

    let writePublications prefix (publications : Publication list) =
        let commentKeys = publications |> List.collect (fun publication -> publication.Comments |> List.map (fun c -> c.Name)) |> List.distinct

        seq {
            let statusTerms,statusTermAccession,statusTermSources = publications |> List.map (fun p -> dismantleOntology p.Status) |> List.unzip3
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.PubMedIDTab                      :: (publications |> List.map (fun publication -> publication.PubMedID))))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.DOITab                      :: (publications |> List.map (fun publication -> publication.DOI))))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.AuthorListTab                      :: (publications |> List.map (fun publication -> publication.Authors))))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.TitleTab                      :: (publications |> List.map (fun publication -> publication.Title))))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.StatusTab                      :: statusTerms))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.StatusTermAccessionNumberTab                      :: statusTermAccession))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.StatusTermSourceREFTab                      :: statusTermSources))

            for key in commentKeys do
                let values = 
                    publications |> List.map (fun publication -> 
                        List.tryPickDefault (fun (c : Comment) -> if c.Name = key then Some c.Value else None) "" publication.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }

    let writeFactors prefix (factors : Factor list) =
        let commentKeys = factors |> List.collect (fun factor -> factor.Comments |> List.map (fun c -> c.Name)) |> List.distinct

        seq {
            let factorTypes,factorTypeAccessions,factorTypeSourceRefs = factors |> List.map (fun p -> dismantleOntology p.FactorType) |> List.unzip3
            yield   ( Row.ofValues None 0u (prefix + " " + Factor.NameTab                      :: (factors |> List.map (fun factor -> factor.Name))))
            yield   ( Row.ofValues None 0u (prefix + " " + Factor.FactorTypeTab                      :: factorTypes))
            yield   ( Row.ofValues None 0u (prefix + " " + Factor.TypeTermAccessionNumberTab                      :: factorTypeAccessions))
            yield   ( Row.ofValues None 0u (prefix + " " + Factor.TypeTermSourceREFTab                      :: factorTypeSourceRefs))

            for key in commentKeys do
                let values = 
                    factors |> List.map (fun factor -> 
                        List.tryPickDefault (fun (c : Comment) -> if c.Name = key then Some c.Value else None) "" factor.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }

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

    let writePersons prefix (persons : Person list) =
        let commentKeys = persons |> List.collect (fun person -> person.Comments |> List.map (fun c -> c.Name)) |> List.distinct

        seq {
            let roleNames,roleAccessions,roleSourceRefs = persons |> List.map (fun p -> mergeOntologies p.Roles) |> List.unzip3
            yield   ( Row.ofValues None 0u (prefix + " " + Person.LastNameTab                      :: (persons |> List.map (fun person -> person.LastName))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.FirstNameTab                      :: (persons |> List.map (fun person -> person.FirstName))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.MidInitialsTab                      :: (persons |> List.map (fun person -> person.MidInitials))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.EmailTab                      :: (persons |> List.map (fun person -> person.EMail))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.PhoneTab                      :: (persons |> List.map (fun person -> person.Phone))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.FaxTab                      :: (persons |> List.map (fun person -> person.Fax))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.AddressTab                      :: (persons |> List.map (fun person -> person.Address))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.AffiliationTab                      :: (persons |> List.map (fun person -> person.Affiliation))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.RolesTab                      :: roleNames))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.RolesTermAccessionNumberTab   :: roleAccessions))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.RolesTermSourceREFTab         :: roleSourceRefs))

            for key in commentKeys do
                let values = 
                    persons |> List.map (fun person -> 
                        List.tryPickDefault (fun (c : Comment) -> if c.Name = key then Some c.Value else None) "" person.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }

    let writeStudy (study : Study) =
        seq {          
            yield! writeStudyInfo study

            yield  Row.ofValues None 0u [Study.DesignDescriptorsTab]
            yield! writeDesigns Study.DesignDescriptorsTabPrefix (study.StudyDesignDescriptors |> List.map REF.toItem)

            yield  Row.ofValues None 0u [Study.PublicationsTab]
            yield! writePublications Study.PublicationsTabPrefix (study.Publications |> List.map REF.toItem)

            yield  Row.ofValues None 0u [Study.FactorsTab]
            yield! writeFactors Study.FactorsTabPrefix (study.Factors |> List.map REF.toItem)

            yield  Row.ofValues None 0u [Study.AssaysTab]
            yield! writeAssays Study.AssaysTabPrefix (study.Assays |> List.map REF.toItem)

            yield  Row.ofValues None 0u [Study.ProtocolsTab]
            yield! writeProtocols Study.ProtocolsTabPrefix (study.Protocols |> List.map REF.toItem)

            yield  Row.ofValues None 0u [Study.ContactsTab]
            yield! writePersons Study.ContactsTabPrefix (study.Contacts |> List.map REF.toItem)
        }

    let toRows (investigationFile:InvestigationFile) : seq<Row> =
        let investigation = investigationFile.Investigation
        let insertRemarks (remarks:(int*string)list) (rows:seq<Row>) = 
            let rm = Map.ofList remarks
            let rec loop i l nl =
                match Map.tryFind i rm with
                | Some remark ->
                     Row.ofValues None 1u [wrapRemark remark] :: nl
                    |> loop (i+1) l
                | None -> 
                    match l with
                    | [] -> nl
                    | h :: t -> 
                        loop (i+1) t (h::nl)
            loop 1 (rows |> List.ofSeq) []
            |> List.rev
        seq {
            yield  Row.ofValues None 0u [Investigation.OntologySourceReferenceTab]
            yield! writeTermSources (investigation.OntologySourceReferences |> List.map REF.toItem)

            yield  Row.ofValues None 0u [Investigation.InvestigationTab]
            yield! writeInvestigationInfo investigation

            yield  Row.ofValues None 0u [Investigation.PublicationsTab]
            yield! writePublications Investigation.PublicationsTabPrefix (investigation.Publications |> List.map REF.toItem)

            yield  Row.ofValues None 0u [Investigation.ContactsTab]
            yield! writePersons Investigation.ContactsTabPrefix (investigation.Contacts |> List.map REF.toItem)

            for study in investigation.Studies do
                yield  Row.ofValues None 0u [Investigation.StudyTab]
                yield! writeStudy (study |> REF.toItem)
        }
        |> insertRemarks investigationFile.Remarks
        |> Seq.mapi (fun i row -> Row.updateRowIndex (i+1 |> uint) row)


    let toFile (path:string) (investigationFile:InvestigationFile) =

        let doc = Spreadsheet.initWithSST "isa_investigation" path
        let sheet = Spreadsheet.tryGetSheetBySheetIndex 0u doc |> Option.get

        investigationFile
        |> toRows
        |> Seq.fold (fun s r -> 
            SheetData.appendRow r s
        ) sheet
        |> ignore

        doc.Close()