namespace ISA.API

open ISA

module CommentList = 

    /// If a comment with the given key exists in the list, return its value, else return None
    let tryItem (key: string) (comments : Comment list) =
        comments
        |> List.tryPick (fun c -> 
            match c.Name with
            | Some n when n = key -> c.Value
            | _ -> None        
        )

    /// Returns true, if the key exists in the list
    let containsKey (key: string) (comments : Comment list) =
        comments
        |> List.exists (fun c -> 
            match c.Name with
            | Some n when n = key -> true
            | _ -> false        
        )

    /// If a comment with the given key exists in the list, return its value
    let item (key: string) (comments : Comment list) =
        (tryItem key comments).Value

    /// Create a map of comment keys to comment values
    let toMap (comments : Comment list) =
        comments
        |> List.choose (fun c -> 
            match c.Name with
            | Some n -> Some (n,c.Value)
            | _ -> None
        )
        |> Map.ofList
  
    /// Adds the given comment to the comment list  
    let add (comment : Comment) (comments : Comment list) =
        List.append comments [comment]

    /// Add the given comment to the comment list if it doesnt exist, else replace it 
    let set (comment : Comment) (comments : Comment list) =
        if containsKey comment.Name.Value comments then
            comments
            |> List.map (fun c -> if c.Name = comment.Name then comment else c)
        else
            List.append comments [comment]

    /// Returns a new comment list where comments with the given key are filtered out
    let dropByKey (key: string) (comments : Comment list) =
        comments
        |> List.filter (fun c -> 
            match c.Name with
            | Some n when n = key -> false
            | _ -> true        
        )