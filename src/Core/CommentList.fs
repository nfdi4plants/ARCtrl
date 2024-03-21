module ARCtrl.CommentArray

 
open ARCtrl.Helper 

/// If a comment with the given key exists in the [], return its value, else return None
let tryItem (key: string) (comments : ResizeArray<Comment>) =
    comments
    |> Seq.tryPick (fun c -> 
        match c.Name with
        | Some n when n = key -> c.Value
        | _ -> None        
    )

/// Returns true, if the key exists in the []
let containsKey (key: string) (comments : ResizeArray<Comment>) =
    comments
    |> Seq.exists (fun c -> 
        match c.Name with
        | Some n when n = key -> true
        | _ -> false        
    )

/// If a comment with the given key exists in the [], return its value
let item (key: string) (comments : ResizeArray<Comment>) =
    (tryItem key comments).Value

/// Create a map of comment keys to comment values
let toMap (comments : ResizeArray<Comment>) =
    comments
    |> ResizeArray.choose (fun c -> 
        match c.Name with
        | Some n -> Some (n,c.Value)
        | _ -> None
    )
    |> Map.ofSeq
  

/// Add the given comment to the comment [] if it doesnt exist, else replace it 
let set (comment : Comment) (comments : ResizeArray<Comment>) =
    if containsKey comment.Name.Value comments then
        comments
        |> ResizeArray.map (fun c -> if c.Name = comment.Name then comment else c)
    else
        ResizeArray.appendSingleton comment comments 

/// Returns a new comment [] where comments with the given key are filtered out
let dropByKey (key: string) (comments : ResizeArray<Comment>) =
    comments
    |> ResizeArray.filter (fun c -> 
        match c.Name with
        | Some n when n = key -> false
        | _ -> true        
    )