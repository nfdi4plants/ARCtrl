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

    /// This pattern captures all input coming before an opening square bracket or normal bracket (with whitespace).
    [<LiteralAttribute>]
    let CoreNamePattern = "^[^[(]*"

    /// Hits term accession, without id: ENVO:01001831
    [<LiteralAttribute>]
    let TermAccessionPattern = @"(?<idspace>\w+?):(?<localid>\w+)" //prev: @"[\w]+?:[\d]+"

    // https://obofoundry.org/id-policy.html#mapping-of-owl-ids-to-obo-format-ids
    /// <summary>Regex pattern is designed to hit only Foundry-compliant URIs.</summary>
    [<LiteralAttribute>]
    let TermAccessionPatternURI = @"http://purl.obolibrary.org/obo/(?<idspace>\w+?)_(?<localid>\w+)"

    /// Watch this closely, this could hit some edge cases we do not want to cover.
    [<LiteralAttribute>]
    let TermAccessionPatternURI_lessRestrictive = @".*\/(?<idspace>\w+?)[:_](?<localid>\w+)"

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

module Aux =
    
    open System.Text.RegularExpressions
    
    /// (|Regex|_|) pattern input
    let (|Regex|_|) pattern input =
        let m = Regex.Match(input, pattern)
        if m.Success then Some(m)
        else None

open Pattern
open Aux
open System
open System.Text.RegularExpressions
    

/// <summary>
/// This function can be used to extract `IDSPACE:LOCALID` (or: `Term Accession`) from Swate header strings or obofoundry conform URI strings.
/// 
/// **Example 1:** "http://purl.obolibrary.org/obo/GO_000001" --> "GO:000001"
/// 
/// **Example 2:** "Term Source REF (NFDI4PSO:0000064)" --> "NFDI4PSO:0000064"
/// </summary>
let tryParseTermAccession (str:string) =
    match str.Trim() with
    | Regex TermAccessionPattern value 
    | Regex TermAccessionPatternURI value 
    | Regex TermAccessionPatternURI_lessRestrictive value ->
        let idspace = value.Groups.["idspace"].Value
        let localid = value.Groups.["localid"].Value
        {|IdSpace = idspace; LocalId = localid|}
        |> Some
    | _ ->
        None

/// Tries to parse 'str' to term accession and returns it in the format `Some "idspace:localid"`. Exmp.: `Some "MS:000001"`
let tryGetTermAccessionString (str:string) = 
    tryParseTermAccession str
    |> Option.map (fun r -> r.IdSpace + ":" + r.LocalId)

/// Parses 'str' to term accession and returns it in the format "idspace:localid". Exmp.: "MS:000001"
let getTermAccessionString (str:string) =
    match tryGetTermAccessionString str with
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
