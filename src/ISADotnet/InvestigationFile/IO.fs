namespace ISADotNet.InvestigationFile

open System.Collections.Generic
open FSharpSpreadsheetML
open DocumentFormat.OpenXml.Spreadsheet
open ISADotNet.InvestigationFile
open System.Text.RegularExpressions

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
                    loop identifier title description submissionDate publicReleaseDate ((k,v) :: comments) remarks (lineNumber + 1)         
                | Comment k, None -> 
                    loop identifier title description submissionDate publicReleaseDate ((k,"") :: comments) remarks (lineNumber + 1)         

                | Remark k, _  -> 
                    loop identifier title description submissionDate publicReleaseDate comments ((lineNumber,k) :: remarks) (lineNumber + 1)

                | Some k, Some identifier when k = InvestigationInfo.IdentifierLabel->              
                    loop identifier title description submissionDate publicReleaseDate comments remarks (lineNumber + 1)

                | Some k, Some title when k = InvestigationInfo.TitleLabel ->
                    loop identifier title description submissionDate publicReleaseDate comments remarks (lineNumber + 1)

                |Some k, Some description when k = InvestigationInfo.DescriptionLabel ->
                    loop identifier title description submissionDate publicReleaseDate comments remarks (lineNumber + 1)

                | Some k, Some submissionDate when k = InvestigationInfo.SubmissionDateLabel ->
                    loop identifier title description submissionDate publicReleaseDate comments remarks (lineNumber + 1)

                | Some k, Some publicReleaseDate when k = InvestigationInfo.PublicReleaseDateLabel ->
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
                    loop identifier title description submissionDate publicReleaseDate fileName ((k,v) :: comments) remarks (lineNumber + 1)         
                | Comment k, None -> 
                    loop identifier title description submissionDate publicReleaseDate fileName ((k,"") :: comments) remarks (lineNumber + 1)         

                | Remark k, _  -> 
                    loop identifier title description submissionDate publicReleaseDate fileName comments ((lineNumber,k) :: remarks) (lineNumber + 1)

                | Some k, Some identifier when k = StudyInfo.IdentifierLabel->              
                    loop identifier  title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                | Some k, Some title when k = StudyInfo.TitleLabel ->
                    loop identifier title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                |Some k, Some description when k = StudyInfo.DescriptionLabel ->
                    loop identifier title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                | Some k, Some submissionDate when k = StudyInfo.SubmissionDateLabel ->
                    loop identifier title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                | Some k, Some publicReleaseDate when k = StudyInfo.PublicReleaseDateLabel ->
                    loop identifier title description submissionDate publicReleaseDate fileName comments remarks (lineNumber + 1)

                | Some k, Some fileName when k = StudyInfo.FileNameLabel ->
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
                    TermSource.create
                        (Array.tryItemDefault i "" names)
                        (Array.tryItemDefault i "" files)
                        (Array.tryItemDefault i "" versions)
                        (Array.tryItemDefault i "" descriptions)
                        (List.map (fun (key,values) -> key,Array.tryItemDefault i "" values) comments)
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

                | Some k, Some names when k = TermSource.NameLabel -> 
                    loop 
                        names files versions descriptions
                        comments remarks (lineNumber + 1)

                | Some k, Some files when k = TermSource.FileLabel -> 
                    loop 
                        names files versions descriptions
                        comments remarks (lineNumber + 1)

                | Some k, Some versions when k = TermSource.VersionLabel -> 
                    loop 
                        names files versions descriptions
                        comments remarks (lineNumber + 1)

                | Some k, Some descriptions when k = TermSource.DescriptionLabel -> 
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
                    Publication.create
                        (Array.tryItemDefault i "" pubMedIDs)
                        (Array.tryItemDefault i "" dois)
                        (Array.tryItemDefault i "" authorLists)
                        (Array.tryItemDefault i "" titles)
                        (Array.tryItemDefault i "" statuss)
                        (Array.tryItemDefault i "" statusTermAccessionNumbers)
                        (Array.tryItemDefault i "" statusTermSourceREFs)
                        (List.map (fun (key,values) -> key,Array.tryItemDefault i "" values) comments)
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

                | Some k, Some pubMedIDs when k = prefix + " " + Publication.PubMedIDLabel -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)
                | Some k, Some pubMedIDs when k = prefix.Replace(" Publication","") + " " + Publication.PubMedIDLabel ->               
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some dois when k = prefix + " " + Publication.DOILabel -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some authorLists when k = prefix + " " + Publication.AuthorListLabel -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some titles when k = prefix + " " + Publication.TitleLabel -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some statuss when k = prefix + " " + Publication.StatusLabel -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some statusTermAccessionNumbers when k = prefix + " " + Publication.StatusTermAccessionNumberLabel -> 
                    loop 
                        pubMedIDs dois authorLists titles statuss statusTermAccessionNumbers statusTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some statusTermSourceREFs when k = prefix + " " + Publication.StatusTermSourceREFLabel -> 
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
                    Person.create
                        (Array.tryItemDefault i "" lastNames)
                        (Array.tryItemDefault i "" firstNames)
                        (Array.tryItemDefault i "" midInitialss)
                        (Array.tryItemDefault i "" emails)
                        (Array.tryItemDefault i "" phones)
                        (Array.tryItemDefault i "" faxs)
                        (Array.tryItemDefault i "" addresss)
                        (Array.tryItemDefault i "" affiliations)
                        (Array.tryItemDefault i "" roless)
                        (Array.tryItemDefault i "" rolesTermAccessionNumbers)
                        (Array.tryItemDefault i "" rolesTermSourceREFs)
                        (List.map (fun (key,values) -> key,Array.tryItemDefault i "" values) comments)
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

                | Some k, Some lastNames when k = prefix + " " + Person.LastNameLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some firstNames when k = prefix + " " + Person.FirstNameLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some midInitialss when k = prefix + " " + Person.MidInitialsLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some emails when k = prefix + " " + Person.EmailLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some phones when k = prefix + " " + Person.PhoneLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some faxs when k = prefix + " " + Person.FaxLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some addresss when k = prefix + " " + Person.AddressLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some affiliations when k = prefix + " " + Person.AffiliationLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some roless when k = prefix + " " + Person.RolesLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some rolesTermAccessionNumbers when k = prefix + " " + Person.RolesTermAccessionNumberLabel -> 
                    loop 
                        lastNames firstNames midInitialss emails phones faxs addresss affiliations roless rolesTermAccessionNumbers rolesTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some rolesTermSourceREFs when k = prefix + " " + Person.RolesTermSourceREFLabel -> 
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
                    Design.create
                        (Array.tryItemDefault i "" designTypes)
                        (Array.tryItemDefault i "" typeTermAccessionNumbers)
                        (Array.tryItemDefault i "" typeTermSourceREFs)
                        (List.map (fun (key,values) -> key,Array.tryItemDefault i "" values) comments)
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

                | Some k, Some designTypes when k = prefix + " " + Design.DesignTypeLabel -> 
                    loop 
                        designTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some typeTermAccessionNumbers when k = prefix + " " + Design.TypeTermAccessionNumberLabel -> 
                    loop 
                        designTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some typeTermSourceREFs when k = prefix + " " + Design.TypeTermSourceREFLabel -> 
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
                    Factor.create
                        (Array.tryItemDefault i "" names)
                        (Array.tryItemDefault i "" factorTypes)
                        (Array.tryItemDefault i "" typeTermAccessionNumbers)
                        (Array.tryItemDefault i "" typeTermSourceREFs)
                        (List.map (fun (key,values) -> key,Array.tryItemDefault i "" values) comments)
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

                | Some k, Some names when k = prefix + " " + Factor.NameLabel -> 
                    loop 
                        names factorTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some factorTypes when k = prefix + " " + Factor.FactorTypeLabel -> 
                    loop 
                        names factorTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some typeTermAccessionNumbers when k = prefix + " " + Factor.TypeTermAccessionNumberLabel -> 
                    loop 
                        names factorTypes typeTermAccessionNumbers typeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some typeTermSourceREFs when k = prefix + " " + Factor.TypeTermSourceREFLabel -> 
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
                    Assay.create
                        (Array.tryItemDefault i "" measurementTypes)
                        (Array.tryItemDefault i "" measurementTypeTermAccessionNumbers)
                        (Array.tryItemDefault i "" measurementTypeTermSourceREFs)
                        (Array.tryItemDefault i "" technologyTypes)
                        (Array.tryItemDefault i "" technologyTypeTermAccessionNumbers)
                        (Array.tryItemDefault i "" technologyTypeTermSourceREFs)
                        (Array.tryItemDefault i "" technologyPlatforms)
                        (Array.tryItemDefault i "" fileNames)
                        (List.map (fun (key,values) -> key,Array.tryItemDefault i "" values) comments)
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

                | Some k, Some measurementTypes when k = prefix + " " + Assay.MeasurementTypeLabel -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        comments remarks (lineNumber + 1)

                | Some k, Some measurementTypeTermAccessionNumbers when k = prefix + " " + Assay.MeasurementTypeTermAccessionNumberLabel -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        comments remarks (lineNumber + 1)

                | Some k, Some measurementTypeTermSourceREFs when k = prefix + " " + Assay.MeasurementTypeTermSourceREFLabel -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        comments remarks (lineNumber + 1)

                | Some k, Some technologyTypes when k = prefix + " " + Assay.TechnologyTypeLabel -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        comments remarks (lineNumber + 1)

                | Some k, Some technologyTypeTermAccessionNumbers when k = prefix + " " + Assay.TechnologyTypeTermAccessionNumberLabel -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        comments remarks (lineNumber + 1)

                | Some k, Some technologyTypeTermSourceREFs when k = prefix + " " + Assay.TechnologyTypeTermSourceREFLabel -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        comments remarks (lineNumber + 1)

                | Some k, Some technologyPlatforms when k = prefix + " " + Assay.TechnologyPlatformLabel -> 
                    loop 
                        measurementTypes measurementTypeTermAccessionNumbers measurementTypeTermSourceREFs technologyTypes technologyTypeTermAccessionNumbers technologyTypeTermSourceREFs technologyPlatforms fileNames
                        comments remarks (lineNumber + 1)

                | Some k, Some fileNames when k = prefix + " " + Assay.FileNameLabel -> 
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
                    Protocol.create
                        (Array.tryItemDefault i "" names)
                        (Array.tryItemDefault i "" protocolTypes)
                        (Array.tryItemDefault i "" typeTermAccessionNumbers)
                        (Array.tryItemDefault i "" typeTermSourceREFs)
                        (Array.tryItemDefault i "" descriptions)
                        (Array.tryItemDefault i "" uris)
                        (Array.tryItemDefault i "" versions)
                        (Array.tryItemDefault i "" parametersNames)
                        (Array.tryItemDefault i "" parametersTermAccessionNumbers)
                        (Array.tryItemDefault i "" parametersTermSourceREFs)
                        (Array.tryItemDefault i "" componentsNames)
                        (Array.tryItemDefault i "" componentsTypes)
                        (Array.tryItemDefault i "" componentsTypeTermAccessionNumbers)
                        (Array.tryItemDefault i "" componentsTypeTermSourceREFs)
                        (List.map (fun (key,values) -> key,Array.tryItemDefault i "" values) comments)
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

                | Some k, Some names when k = prefix + " " + Protocol.NameLabel -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some protocolTypes when k = prefix + " " + Protocol.ProtocolTypeLabel -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some typeTermAccessionNumbers when k = prefix + " " + Protocol.TypeTermAccessionNumberLabel -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some typeTermSourceREFs when k = prefix + " " + Protocol.TypeTermSourceREFLabel -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some descriptions when k = prefix + " " + Protocol.DescriptionLabel -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some uris when k = prefix + " " + Protocol.URILabel -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some versions when k = prefix + " " + Protocol.VersionLabel -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some parametersNames when k = prefix + " " + Protocol.ParametersNameLabel -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some parametersTermAccessionNumbers when k = prefix + " " + Protocol.ParametersTermAccessionNumberLabel -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some parametersTermSourceREFs when k = prefix + " " + Protocol.ParametersTermSourceREFLabel -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some componentsNames when k = prefix + " " + Protocol.ComponentsNameLabel -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some componentsTypes when k = prefix + " " + Protocol.ComponentsTypeLabel -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some componentsTypeTermAccessionNumbers when k = prefix + " " + Protocol.ComponentsTypeTermAccessionNumberLabel -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, Some componentsTypeTermSourceREFs when k = prefix + " " + Protocol.ComponentsTypeTermSourceREFLabel -> 
                    loop 
                        names protocolTypes typeTermAccessionNumbers typeTermSourceREFs descriptions uris versions parametersNames parametersTermAccessionNumbers parametersTermSourceREFs componentsNames componentsTypes componentsTypeTermAccessionNumbers componentsTypeTermSourceREFs
                        comments remarks (lineNumber + 1)

                | Some k, _ -> Some k,lineNumber,remarks,create ()
                | _ -> None, lineNumber,remarks,create ()
            else
                None,lineNumber,remarks,create ()
        loop [||] [||] [||] [||] [||] [||] [||] [||] [||] [||] [||] [||] [||] [||] [] [] lineNumber

    let readStudy lineNumber (en:IEnumerator<Row>) = 

        let rec loop lastLine studyInfo designDescriptors publications factors assays protocols contacts remarks lineNumber =
           
            match lastLine with

            | Some k when k = Study.DesignDescriptorsLabel -> 
                let currentLine,lineNumber,newRemarks,designDescriptors = readDesigns Study.DesignDescriptorsPrefix (lineNumber + 1) en         
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | Some k when k = Study.PublicationsLabel -> 
                let currentLine,lineNumber,newRemarks,publications = readPublications Study.PublicationsPrefix (lineNumber + 1) en       
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | Some k when k = Study.FactorsLabel -> 
                let currentLine,lineNumber,newRemarks,factors = readFactors Study.FactorsPrefix (lineNumber + 1) en       
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | Some k when k = Study.AssaysLabel -> 
                let currentLine,lineNumber,newRemarks,assays = readAssays Study.AssaysPrefix (lineNumber + 1) en       
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | Some k when k = Study.ProtocolsLabel -> 
                let currentLine,lineNumber,newRemarks,protocols = readProtocols Study.ProtocolsPrefix (lineNumber + 1) en  
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | Some k when k = Study.ContactsLabel -> 
                let currentLine,lineNumber,newRemarks,contacts = readPersons Study.ContactsPrefix (lineNumber + 1) en  
                loop currentLine studyInfo designDescriptors publications factors assays protocols contacts (List.append remarks newRemarks) lineNumber

            | k -> 
                k,lineNumber,remarks, Study.create studyInfo designDescriptors publications factors assays protocols contacts

    
        let currentLine,lineNumber,remarks,item = readStudyInfo lineNumber en  
        loop currentLine item [] [] [] [] [] [] remarks lineNumber


    let fromRows (rows:seq<Row>) =
        let en = rows.GetEnumerator()              
        
        let emptyInvestigationInfo = InvestigationInfo.create "" "" "" "" "" []

        let rec loop lastLine ontologySourceReferences investigationInfo publications contacts studies remarks lineNumber =
            match lastLine with

            | Some k when k = Investigation.OntologySourceReferenceLabel -> 
                let currentLine,lineNumber,newRemarks,ontologySourceReferences = readTermSources (lineNumber + 1) en         
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = Investigation.InvestigationLabel -> 
                let currentLine,lineNumber,newRemarks,investigationInfo = readInvestigationInfo (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = Investigation.PublicationsLabel -> 
                let currentLine,lineNumber,newRemarks,publications = readPublications Investigation.PublicationsPrefix (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = Investigation.ContactsLabel -> 
                let currentLine,lineNumber,newRemarks,contacts = readPersons Investigation.ContactsPrefix (lineNumber + 1) en       
                loop currentLine ontologySourceReferences investigationInfo publications contacts studies (List.append remarks newRemarks) lineNumber

            | Some k when k = Investigation.StudyLabel -> 
                let currentLine,lineNumber,newRemarks,study = readStudy (lineNumber + 1) en  
                loop currentLine ontologySourceReferences investigationInfo publications contacts (study::studies) (List.append remarks newRemarks) lineNumber

            | k -> 
                Investigation.create ontologySourceReferences investigationInfo publications contacts (List.rev studies) remarks
    
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
    
    let writeTermSources (termSources : TermSource list) =
        let commentKeys = termSources |> List.collect (fun termSource -> termSource.Comments |> List.map fst)

        seq {
            yield   (Row.ofValues None 0u (TermSource.NameLabel                      :: (termSources |> List.map (fun termSource -> termSource.Name))))
            yield   (Row.ofValues None 0u (TermSource.FileLabel                      :: (termSources |> List.map (fun termSource -> termSource.File))))
            yield   (Row.ofValues None 0u (TermSource.VersionLabel                      :: (termSources |> List.map (fun termSource -> termSource.Version))))
            yield   (Row.ofValues None 0u (TermSource.DescriptionLabel                      :: (termSources |> List.map (fun termSource -> termSource.Description))))

            for key in commentKeys do
                let values = 
                    termSources |> List.map (fun termSource -> 
                        List.tryPickDefault (fun (k,v) -> if k = key then Some v else None) "" termSource.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }

    let writeInvestigationInfo (investigationInfo : InvestigationInfo) =  
        seq {       
            yield   ( Row.ofValues None 0u [InvestigationInfo.IdentifierLabel;         investigationInfo.Identifier])
            yield   ( Row.ofValues None 0u [InvestigationInfo.TitleLabel;              investigationInfo.Title])
            yield   ( Row.ofValues None 0u [InvestigationInfo.DescriptionLabel;        investigationInfo.Description])
            yield   ( Row.ofValues None 0u [InvestigationInfo.SubmissionDateLabel;     investigationInfo.SubmissionDate])
            yield   ( Row.ofValues None 0u [InvestigationInfo.PublicReleaseDateLabel;  investigationInfo.PublicReleaseDate])
            yield!  (investigationInfo.Comments |> List.map (fun (k,v) ->  Row.ofValues None 0u [wrapCommentKey k;v]))         
        }

    let writeStudyInfo (studyInfo : StudyInfo) =  

        seq {   
            yield   ( Row.ofValues None 0u [StudyInfo.IdentifierLabel;         studyInfo.Identifier]          )
            yield   ( Row.ofValues None 0u [StudyInfo.TitleLabel;              studyInfo.Title]               )
            yield   ( Row.ofValues None 0u [StudyInfo.DescriptionLabel;        studyInfo.Description]         )
            yield   ( Row.ofValues None 0u [StudyInfo.SubmissionDateLabel;     studyInfo.SubmissionDate]      )
            yield   ( Row.ofValues None 0u [StudyInfo.PublicReleaseDateLabel;  studyInfo.PublicReleaseDate]   )
            yield   ( Row.ofValues None 0u [StudyInfo.FileNameLabel;           studyInfo.FileName]            )
            yield!  (studyInfo.Comments |> List.map (fun (k,v) ->  Row.ofValues None 0u [wrapCommentKey k;v]))         
        }

    let writeDesigns prefix (designs : Design list) =
        let commentKeys = designs |> List.collect (fun design -> design.Comments |> List.map fst) |> List.distinct

        seq {
            yield   ( Row.ofValues None 0u (prefix + " " + Design.DesignTypeLabel                      :: (designs |> List.map (fun design -> design.DesignType))))
            yield   ( Row.ofValues None 0u (prefix + " " + Design.TypeTermAccessionNumberLabel                      :: (designs |> List.map (fun design -> design.TypeTermAccessionNumber))))
            yield   ( Row.ofValues None 0u (prefix + " " + Design.TypeTermSourceREFLabel                      :: (designs |> List.map (fun design -> design.TypeTermSourceREF))))

            for key in commentKeys do
                let values = 
                    designs |> List.map (fun design -> 
                        List.tryPickDefault (fun (k,v) -> if k = key then Some v else None) "" design.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }

    let writePublications prefix (publications : Publication list) =
        let commentKeys = publications |> List.collect (fun publication -> publication.Comments |> List.map fst) |> List.distinct

        seq {
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.PubMedIDLabel                      :: (publications |> List.map (fun publication -> publication.PubMedID))))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.DOILabel                      :: (publications |> List.map (fun publication -> publication.DOI))))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.AuthorListLabel                      :: (publications |> List.map (fun publication -> publication.AuthorList))))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.TitleLabel                      :: (publications |> List.map (fun publication -> publication.Title))))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.StatusLabel                      :: (publications |> List.map (fun publication -> publication.Status))))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.StatusTermAccessionNumberLabel                      :: (publications |> List.map (fun publication -> publication.StatusTermAccessionNumber))))
            yield   ( Row.ofValues None 0u (prefix + " " + Publication.StatusTermSourceREFLabel                      :: (publications |> List.map (fun publication -> publication.StatusTermSourceREF))))

            for key in commentKeys do
                let values = 
                    publications |> List.map (fun publication -> 
                        List.tryPickDefault (fun (k,v) -> if k = key then Some v else None) "" publication.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }

    let writeFactors prefix (factors : Factor list) =
        let commentKeys = factors |> List.collect (fun factor -> factor.Comments |> List.map fst) |> List.distinct

        seq {
            yield   ( Row.ofValues None 0u (prefix + " " + Factor.NameLabel                      :: (factors |> List.map (fun factor -> factor.Name))))
            yield   ( Row.ofValues None 0u (prefix + " " + Factor.FactorTypeLabel                      :: (factors |> List.map (fun factor -> factor.FactorType))))
            yield   ( Row.ofValues None 0u (prefix + " " + Factor.TypeTermAccessionNumberLabel                      :: (factors |> List.map (fun factor -> factor.TypeTermAccessionNumber))))
            yield   ( Row.ofValues None 0u (prefix + " " + Factor.TypeTermSourceREFLabel                      :: (factors |> List.map (fun factor -> factor.TypeTermSourceREF))))

            for key in commentKeys do
                let values = 
                    factors |> List.map (fun factor -> 
                        List.tryPickDefault (fun (k,v) -> if k = key then Some v else None) "" factor.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }

    let writeAssays prefix (assays : Assay list) =
        let commentKeys = assays |> List.collect (fun assay -> assay.Comments |> List.map fst) |> List.distinct

        seq { 
            yield   ( Row.ofValues None 0u (prefix + " " + Assay.MeasurementTypeLabel                      :: (assays |> List.map (fun assay -> assay.MeasurementType))))
            yield   ( Row.ofValues None 0u (prefix + " " + Assay.MeasurementTypeTermAccessionNumberLabel   :: (assays |> List.map (fun assay -> assay.MeasurementTypeTermAccessionNumber))))
            yield   ( Row.ofValues None 0u (prefix + " " + Assay.MeasurementTypeTermSourceREFLabel         :: (assays |> List.map (fun assay -> assay.MeasurementTypeTermSourceREF))))
            yield   ( Row.ofValues None 0u (prefix + " " + Assay.TechnologyTypeLabel                       :: (assays |> List.map (fun assay -> assay.TechnologyType))))
            yield   ( Row.ofValues None 0u (prefix + " " + Assay.TechnologyTypeTermAccessionNumberLabel    :: (assays |> List.map (fun assay -> assay.TechnologyTypeTermAccessionNumber))))
            yield   ( Row.ofValues None 0u (prefix + " " + Assay.TechnologyTypeTermSourceREFLabel          :: (assays |> List.map (fun assay -> assay.TechnologyTypeTermSourceREF))))
            yield   ( Row.ofValues None 0u (prefix + " " + Assay.TechnologyPlatformLabel                   :: (assays |> List.map (fun assay -> assay.TechnologyPlatform))))
            yield   ( Row.ofValues None 0u (prefix + " " + Assay.FileNameLabel                             :: (assays |> List.map (fun assay -> assay.FileName))))               
            
            for key in commentKeys do
                let values = 
                    assays |> List.map (fun assay -> 
                        List.tryPickDefault (fun (k,v) -> if k = key then Some v else None) "" assay.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }

    let writeProtocols prefix (protocols : Protocol list) =
        let commentKeys = protocols |> List.collect (fun protocol -> protocol.Comments |> List.map fst) |> List.distinct

        seq {
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.NameLabel                      :: (protocols |> List.map (fun protocol -> protocol.Name))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.ProtocolTypeLabel                      :: (protocols |> List.map (fun protocol -> protocol.ProtocolType))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.TypeTermAccessionNumberLabel                      :: (protocols |> List.map (fun protocol -> protocol.TypeTermAccessionNumber))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.TypeTermSourceREFLabel                      :: (protocols |> List.map (fun protocol -> protocol.TypeTermSourceREF))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.DescriptionLabel                      :: (protocols |> List.map (fun protocol -> protocol.Description))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.URILabel                      :: (protocols |> List.map (fun protocol -> protocol.URI))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.VersionLabel                      :: (protocols |> List.map (fun protocol -> protocol.Version))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.ParametersNameLabel                      :: (protocols |> List.map (fun protocol -> protocol.ParametersName))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.ParametersTermAccessionNumberLabel                      :: (protocols |> List.map (fun protocol -> protocol.ParametersTermAccessionNumber))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.ParametersTermSourceREFLabel                      :: (protocols |> List.map (fun protocol -> protocol.ParametersTermSourceREF))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.ComponentsNameLabel                      :: (protocols |> List.map (fun protocol -> protocol.ComponentsName))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.ComponentsTypeLabel                      :: (protocols |> List.map (fun protocol -> protocol.ComponentsType))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.ComponentsTypeTermAccessionNumberLabel                      :: (protocols |> List.map (fun protocol -> protocol.ComponentsTypeTermAccessionNumber))))
            yield   ( Row.ofValues None 0u (prefix + " " + Protocol.ComponentsTypeTermSourceREFLabel                      :: (protocols |> List.map (fun protocol -> protocol.ComponentsTypeTermSourceREF))))

            for key in commentKeys do
                let values = 
                    protocols |> List.map (fun protocol -> 
                        List.tryPickDefault (fun (k,v) -> if k = key then Some v else None) "" protocol.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }

    let writePersons prefix (persons : Person list) =
        let commentKeys = persons |> List.collect (fun person -> person.Comments |> List.map fst) |> List.distinct

        seq {
            yield   ( Row.ofValues None 0u (prefix + " " + Person.LastNameLabel                      :: (persons |> List.map (fun person -> person.LastName))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.FirstNameLabel                      :: (persons |> List.map (fun person -> person.FirstName))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.MidInitialsLabel                      :: (persons |> List.map (fun person -> person.MidInitials))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.EmailLabel                      :: (persons |> List.map (fun person -> person.Email))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.PhoneLabel                      :: (persons |> List.map (fun person -> person.Phone))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.FaxLabel                      :: (persons |> List.map (fun person -> person.Fax))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.AddressLabel                      :: (persons |> List.map (fun person -> person.Address))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.AffiliationLabel                      :: (persons |> List.map (fun person -> person.Affiliation))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.RolesLabel                      :: (persons |> List.map (fun person -> person.Roles))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.RolesTermAccessionNumberLabel                      :: (persons |> List.map (fun person -> person.RolesTermAccessionNumber))))
            yield   ( Row.ofValues None 0u (prefix + " " + Person.RolesTermSourceREFLabel                      :: (persons |> List.map (fun person -> person.RolesTermSourceREF))))

            for key in commentKeys do
                let values = 
                    persons |> List.map (fun person -> 
                        List.tryPickDefault (fun (k,v) -> if k = key then Some v else None) "" person.Comments
                    )
                yield ( Row.ofValues None 0u (wrapCommentKey key :: values))
        }

    let writeStudy (study : Study) =
        seq {          
            yield! writeStudyInfo study.Info

            yield  Row.ofValues None 0u [Study.DesignDescriptorsLabel]
            yield! writeDesigns Study.DesignDescriptorsPrefix study.DesignDescriptors

            yield  Row.ofValues None 0u [Study.PublicationsLabel]
            yield! writePublications Study.PublicationsPrefix study.Publications

            yield  Row.ofValues None 0u [Study.FactorsLabel]
            yield! writeFactors Study.FactorsPrefix study.Factors

            yield  Row.ofValues None 0u [Study.AssaysLabel]
            yield! writeAssays Study.AssaysPrefix study.Assays

            yield  Row.ofValues None 0u [Study.ProtocolsLabel]
            yield! writeProtocols Study.ProtocolsPrefix study.Protocols

            yield  Row.ofValues None 0u [Study.ContactsLabel]
            yield! writePersons Study.ContactsPrefix study.Contacts
        }

    let toRows (investigation:Investigation) : seq<Row> =
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
            yield  Row.ofValues None 0u [Investigation.OntologySourceReferenceLabel]
            yield! writeTermSources investigation.OntologySourceReference

            yield  Row.ofValues None 0u [Investigation.InvestigationLabel]
            yield! writeInvestigationInfo investigation.Info

            yield  Row.ofValues None 0u [Investigation.PublicationsLabel]
            yield! writePublications Investigation.PublicationsPrefix investigation.Publications

            yield  Row.ofValues None 0u [Investigation.ContactsLabel]
            yield! writePersons Investigation.ContactsPrefix investigation.Contacts

            for study in investigation.Studies do
                yield  Row.ofValues None 0u [Investigation.StudyLabel]
                yield! writeStudy study
        }
        |> insertRemarks investigation.Remarks
        |> Seq.mapi (fun i row -> Row.updateRowIndex (i+1 |> uint) row)


    let toFile (path:string) (investigation:Investigation) =

        let doc = Spreadsheet.initWithSST "isa_investigation" path
        let sheet = Spreadsheet.tryGetSheetBySheetIndex 0u doc |> Option.get

        investigation
        |> toRows
        |> Seq.fold (fun s r -> 
            SheetData.appendRow r s
        ) sheet
        |> ignore

        doc.Close()