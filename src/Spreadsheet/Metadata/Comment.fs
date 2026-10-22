namespace ARCtrl.Spreadsheet

open ARCtrl
open System.Collections.Generic
open System.Text.RegularExpressions
open ARCtrl.Helper.Regex.ActivePatterns

module Comment = 

    
    let commentValueKey = "commentValue"

    let commentPattern = $@"Comment\s*\[<(?<{commentValueKey}>.+)>\]"

    let commentPatternNoAngleBrackets = $@"Comment\s*\[(?<{commentValueKey}>.+)\]"

    let (|Comment|_|) (key : string) =
        
        match key with
        | Regex commentPattern r ->
            Some r.Groups.[commentValueKey].Value
        | Regex commentPatternNoAngleBrackets r -> 
            let v = r.Groups.[commentValueKey].Value
            if v = "<>" then None else Some v
        | _ -> None
        
   
    let wrapCommentKey k = 
        sprintf "Comment[%s]" k

    let fromString k v =
        Comment.make 
            (Option.fromValueWithDefault "" k) 
            (Option.fromValueWithDefault "" v)

    let toString (c:Comment) =
        Option.defaultValue "" c.Name,
        Option.defaultValue "" c.Value

    /// Adds the comment to the sparse matrix at the given column and returns its name.
    ///
    /// Fails if a comment with the same name was already added for this column, as ISA-XLSX does not allow duplicate comment names.
    let addToMatrix (i : int) (c : Comment) (matrix : Dictionary<string*int,string>) =
        let n, v = toString c
        if matrix.ContainsKey((n, i)) then
            failwithf "%s is used more than once. ISA-XLSX does not allow multiple comments with the same name." (wrapCommentKey n)
        matrix.Add((n, i), v)
        n

module Remark = 

    let remarkValueKey = "remarkValue"

    let remarkPattern = $@"#(?<{remarkValueKey}>.+)"


    let (|Remark|_|) (key : Option<string>) =
        key
        |> Option.bind (fun k ->
            match k with
            | Regex remarkPattern r ->
                Some r.Groups.[remarkValueKey].Value
            | _ -> None
        )


    let wrapRemark r = 
        sprintf "#%s" r