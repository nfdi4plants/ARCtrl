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

    /// Used to get unit name from Excel numberFormat: 0.00 "degree Celsius"
    [<LiteralAttribute>]
    let DoubleQuotesPattern = "\"(.*?)\""

    /// This pattern captures all input coming before an opening square bracket or normal bracket (with whitespace).
    [<LiteralAttribute>]
    let CoreNamePattern = "^[^[(]*"

    /// Hits term accession, without id: ENVO:01001831
    [<LiteralAttribute>]
    let TermAccessionPattern = @"[\w]+?:[\d]+"

    // https://obofoundry.org/id-policy.html#mapping-of-owl-ids-to-obo-format-ids
    /// <summary>Regex pattern is designed to hit only Foundry-compliant URIs.</summary>
    [<LiteralAttribute>]
    let TermAccessionPatternURI = @"http://purl.obolibrary.org/obo/(?<idspace>[\w]+?)_(?<localid>[\d]+)"

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
/// This function can be used to extract term name from squared brackets in isa tab column header.
/// 
/// **Example 1:** "Characteristics [Sample type]" --> "Sample type"
/// </summary>
let parseSquaredTermNameBrackets (headerStr:string) =
    match headerStr with
    | Regex SquaredBracketsTermNamePattern value ->
        // trim whitespace AND remove brackets
        value.Value.Trim().[1..value.Length-2]
        // remove #id pattern
        |> fun str -> Regex.Replace(str, IdPattern, "")
        |> Some 
    | _ ->
        None

/// <summary>
/// This function can be used to extract isa tab column header type from annotation table.
/// 
/// **Example 1:** "Characteristics [Sample type]" --> "Characteristics"
/// 
/// **Example 2:** "Term Accession Number" --> "Term Accession Number"
/// 
/// **Example 3:** "Data File Name" --> "Data File Name"
/// </summary>
let parseCoreName (headerStr:string) =
    match headerStr with
    | Regex CoreNamePattern value ->
        value.Value.Trim()
        |> Some
    | _ ->
        None

/// <summary>
/// This function can be used to extract `IDSPACE:LOCALID` (or: `Term Accession`) from Swate header strings or obofoundry conform URI strings.
/// 
/// **Example 1:** "http://purl.obolibrary.org/obo/GO_000001" --> "GO:000001"
/// 
/// **Example 2:** "Term Source REF (NFDI4PSO:0000064)" --> "NFDI4PSO:0000064"
/// </summary>
let parseTermAccession (headerStr:string) =
    match headerStr.Trim() with
    | Regex TermAccessionPattern value ->
        value.Value.Trim()
        |> Some
    | Regex TermAccessionPatternURI value ->
        let idspace = value.Groups.["idspace"].Value
        let localid = value.Groups.["localid"].Value
        idspace + ":" + localid
        |> Some
    | _ ->
        None

/// <summary>
/// This function is used to parse Excel numberFormat string to term name.
/// 
/// **Example 1:** "0.00 "degree Celsius"" --> "degree Celsius"
/// </summary>
let parseDoubleQuotes (headerStr:string) =
    match headerStr with
    | Regex DoubleQuotesPattern value ->
        // remove quotes at beginning and end of matched string
        value.Value.[1..value.Length-2].Trim()
        |> Some
    | _ ->
        None
