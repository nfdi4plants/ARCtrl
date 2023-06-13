/// <summary>
/// This module contains unified regex patterns and matching functions to parse isa tab column headers to BuildingBlock information.
/// </summary>
module ISA.Regex

open System

module Pattern =

    /// This pattern is only used to remove any leftover #id attributes from previous Swate version. 
    /// `"Parameter [biological replicate#2]"` This #id is deprecated but the pattern can still be used to remove any from files. 
    /// Was deprecated before 2023.
    [<LiteralAttribute>]
    let IdPattern = "#\d+" //  @"(?<=#)\d+(?=[\)\]])" <- Cannot be used in IE11

    /// <summary> This pattern captures characters between squared brackets, without id: Parameter [biological replicate#2] -> [biological replicate]
    /// 
    /// TODO: Could be redesigned to use capture groups, then it could return inner without brackets
    /// </summary>
    [<LiteralAttribute>]
    let SquaredBracketsTermNamePattern = "\[.*\]" //  @"(?<= \[)[^#\]]*(?=[\]#])" <- Cannot be used in IE11

    /// Used to get unit name from Excel numberFormat: 0.00 "degree Celsius" --> degree Celsius
    [<LiteralAttribute>]
    let ExcelNumberFormat = "\"(?<numberFormat>(.*?))\""

    /// Hits Unit column header
    [<LiteralAttribute>]
    let UnitPattern = @"Unit"

    /// Hits Term Source REF and Term Accession Number column headers
    ///
    /// Example 1: "Term Source REF (MS:1003022)"
    ///
    /// Example 2: "Term Accession Number (MS:1003022)"
    ///
    /// the id part "MS:1003022" is captured as `id` group.
    [<LiteralAttribute>]
    let ReferenceColumnPattern = @"(Term Source REF|Term Accession Number)\s\((?<id>.+)\)"   

    /// Hits Term Accession Number column header
    ///
    /// Example 1: "Term Source REF (MS:1003022)"
    ///
    /// the id part "MS:1003022" is captured as `id` group.
    [<LiteralAttribute>]
    let TermSourceREFColumnPattern = @"Term Source REF\s\((?<id>.+)\)" 

    /// Hits Term Source REF column header
    ///
    /// Example 1: "Term Accession Number (MS:1003022)"
    ///
    /// the id part "MS:1003022" is captured as `id` group.
    [<LiteralAttribute>]
    let TermAccessionNumberColumnPattern = @"Term Accession Number\s\((?<id>.+)\)" 

    /// Hits term accession, without id: ENVO:01001831
    [<LiteralAttribute>]
    let TermAnnotationShortPattern = @"(?<idspace>\w+?):(?<localid>\w+)" //prev: @"[\w]+?:[\d]+"


    // https://obofoundry.org/id-policy.html#mapping-of-owl-ids-to-obo-format-ids
    /// <summary>Regex pattern is designed to hit only Foundry-compliant URIs.</summary>
    [<LiteralAttribute>]
    let TermAnnotationURIPattern = @"http://purl.obolibrary.org/obo/(?<idspace>\w+?)_(?<localid>\w+)"

    /// Watch this closely, this could hit some edge cases we do not want to cover.
    [<LiteralAttribute>]
    let TermAnnotationURIPattern_lessRestrictive = @".*\/(?<idspace>\w+?)[:_](?<localid>\w+)"

    /// This pattern is used to match both Input and Output columns and capture the IOType as `iotype` group.
    [<LiteralAttribute>]
    let IOTypePattern = @"(Input|Output)\s\[(?<iotype>.+)\]"

    /// This pattern is used to match Input column and capture the IOType as `iotype` group.
    [<LiteralAttribute>]
    let InputPattern = @"Input\s\[(?<iotype>.+)\]"

    /// This pattern is used to match Output column and capture the IOType as `iotype` group.
    [<LiteralAttribute>]
    let OutputPattern = @"Output\s\[(?<iotype>.+)\]"

    /// This pattern matches any column header starting with some text, followed by one whitespace and a term name inside squared brackets.
    ///
    /// Captures column type as named group: "termcolumntype" (e.g. Component, Characteristic .. ).
    ///
    /// Captures term name as named group: "termname" (e.g. instrument model).
    ///
    /// Exmp. 1: Parameter [instrument model] --> termcolumntype: Parameter; termname: instrument model
    ///
    /// Exmp. 2: Characteristic [species] --> termcolumntype: Characteristic; termname: species
    [<LiteralAttribute>]
    let TermColumnPattern = @"(?<termcolumntype>.+)\s\[(?<termname>.+)\]"

module ActivePatterns =
    
    open System.Text.RegularExpressions
    
    /// Matches, if the input string matches the given regex pattern.
    let (|Regex|_|) pattern (input : string) =
        let m = Regex.Match(input.Trim(), pattern)
        if m.Success then Some(m)
        else None

    /// Matches any column header starting with some text, followed by one whitespace and a term name inside squared brackets.
    let (|TermColumn|_|) input = 
        match input with
        | Regex Pattern.TermColumnPattern r ->
            {|TermColumnType = r.Groups.["termcolumntype"].Value; TermName = r.Groups.["termname"].Value|}
            |> Some
        | _ -> None

    let (|Unit|_|) input = 
        match input with
        | Regex Pattern.UnitPattern _ -> Some()
         | _ -> None

    let (|Parameter|_|) input = 
        match input with
        | TermColumn r ->
            match r.TermColumnType with
            | "Parameter" 
            | "Parameter Value"             -> Some r.TermName
            | _ -> None
        | _ -> None

    let (|Factor|_|) input = 
        match input with
        | TermColumn r ->
            match r.TermColumnType with
            | "Factor" 
            | "Factor Value"             -> Some r.TermName
            | _ -> None
        | _ -> None

    let (|Characteristic|_|) input = 
        match input with
        | TermColumn r ->
            match r.TermColumnType with
            | "Characteristic" 
            | "Characteristics"
            | "Characteristics Value" -> Some r.TermName
            | _ -> None
        | _ -> None

    let (|TermAnnotation|_|) input =
        match input with
        | Regex Pattern.TermAnnotationShortPattern value 
        | Regex Pattern.TermAnnotationURIPattern value 
        | Regex Pattern.TermAnnotationURIPattern_lessRestrictive value ->
            let idspace = value.Groups.["idspace"].Value
            let localid = value.Groups.["localid"].Value
            {|IdSpace = idspace; LocalId = localid|}
            |> Some
        | _ ->
            None

    let (|TermSourceRef|_|) input = 
        match input with
        | Regex Pattern.TermSourceREFColumnPattern r ->
            match r.Groups.["id"].Value with
            | TermAnnotation r -> Some r 
            | _ -> None
         | _ -> None

    let (|TermAccessionNumber|_|) input = 
        match input with
        | Regex Pattern.TermAccessionNumberColumnPattern r ->
            match r.Groups.["id"].Value with
            | TermAnnotation r -> Some r 
            | _ -> None
         | _ -> None

    let (|Input|_|) input = 
        match input with
        | Regex Pattern.InputPattern r ->
            Some r.Groups.["iotype"].Value          
         | _ -> None

    let (|Output|_|) input =
        match input with
        | Regex Pattern.OutputPattern r ->
            Some r.Groups.["iotype"].Value
         | _ -> None

open Pattern
open ActivePatterns
open System
open System.Text.RegularExpressions
    

/// <summary>
/// This function can be used to extract `IDSPACE:LOCALID` (or: `Term Accession`) from Swate header strings or obofoundry conform URI strings.
/// 
/// **Example 1:** "http://purl.obolibrary.org/obo/GO_000001" --> "GO:000001"
/// 
/// **Example 2:** "Term Source REF (NFDI4PSO:0000064)" --> "NFDI4PSO:0000064"
/// </summary>
let tryParseTermAnnotation (str:string) =
    match str.Trim() with
    | Regex TermAnnotationShortPattern value 
    | Regex TermAnnotationURIPattern value 
    | Regex TermAnnotationURIPattern_lessRestrictive value ->
        let idspace = value.Groups.["idspace"].Value
        let localid = value.Groups.["localid"].Value
        {|IdSpace = idspace; LocalId = localid|}
        |> Some
    | _ ->
        None

/// Tries to parse 'str' to term accession and returns it in the format `Some "idspace:localid"`. Exmp.: `Some "MS:000001"`
let tryGetTermAnnotationShortString (str:string) = 
    tryParseTermAnnotation str
    |> Option.map (fun r -> r.IdSpace + ":" + r.LocalId)

/// Parses 'str' to term accession and returns it in the format "idspace:localid". Exmp.: "MS:000001"
let getTermAnnotationShortString (str:string) =
    match tryGetTermAnnotationShortString str with
    | Some s -> s
    | None -> failwith $"Unable to parse '{str}' to term accession."

/// <summary>
/// This function is used to parse Excel numberFormat string to term name.
/// 
/// **Example 1:** "0.00 "degree Celsius"" --> "degree Celsius"
/// </summary>
let tryParseExcelNumberFormat (headerStr:string) =
    match headerStr.Trim() with
    | Regex ExcelNumberFormat value ->
        // remove quotes at beginning and end of matched string
        let numberFormat = value.Groups.["numberFormat"].Value
        Some numberFormat
    | _ ->
        None

/// <summary>
/// This function is used to match both Input and Output columns and capture the IOType as `iotype` group.
/// 
/// **Example 1:** "Input [Sample]" --> "Sample"
/// </summary>
let tryParseIOTypeHeader (headerStr: string) =
    match headerStr.Trim() with
    | Regex IOTypePattern value ->
        // remove quotes at beginning and end of matched string
        let numberFormat = value.Groups.["iotype"].Value
        Some numberFormat
    | _ ->
        None
