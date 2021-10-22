namespace ISADotNet.API

open ISADotNet

module CommentList = 

    /// If a comment with the given key exists in the list, return its value, else return None
    let tryItem (key: string) (comments : Comment list) =
        comments
        |> List.tryPick (fun c -> 
            match c.Name with
            | Some n when n = key -> c.Value
            | _ -> None        
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